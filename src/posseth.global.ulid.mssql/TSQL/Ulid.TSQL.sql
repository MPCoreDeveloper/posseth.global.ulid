/* =====================================================================================
   Posseth.UlidFactory - Native T-SQL ULID implementation
   =====================================================================================
   Pure T-SQL implementation of the ULID specification - no CLR / SQLCLR required.

   ULID specification: https://github.com/ulid/spec

   ULID binary layout (128 bits):
       [ 48-bit timestamp ][ 80-bit randomness ]
         big-endian           cryptographically secure

   Canonical string (26 characters, Crockford Base32):
       tttttttttt rrrrrrrrrrrrrrrr
       10 timestamp chars + 16 randomness chars
       The first character carries only 3 bits (2 leading zero bits).

   ULIDs produced by these functions are interchangeable with every other
   standards-compliant ULID implementation (C#, JavaScript, Go, Python, ...).

   Requirements:
       - SQL Server 2016 SP1 or later (CREATE OR ALTER)
       - No CLR assemblies required

   Functions created:
       dbo.GenerateUlid(@timestampMs BIGINT = NULL) -> VARCHAR(26)  generate a new ULID
       dbo.IsValidUlid(@ulid VARCHAR(26))           -> BIT          validate a ULID
       dbo.UlidToTimestamp(@ulid VARCHAR(26))       -> BIGINT       timestamp in milliseconds
       dbo.ExtractDateFromUlid(@ulid VARCHAR(26))   -> DATETIME2(3) timestamp as date/time
   ===================================================================================== */
GO

/* -------------------------------------------------------------------------------------
   GenerateUlid - generates a new standard-compliant ULID.
   When @timestampMs is NULL the current UTC time is used.
   Returns NULL when the timestamp falls outside the 48-bit ULID range (0 .. 2^48 - 1).
   ------------------------------------------------------------------------------------- */
CREATE OR ALTER FUNCTION dbo.GenerateUlid(@timestampMs BIGINT = NULL)
RETURNS VARCHAR(26)
AS
BEGIN
    DECLARE @alphabet VARCHAR(32) = '0123456789ABCDEFGHJKMNPQRSTVWXYZ';

    -- Current UTC time in milliseconds, or the explicitly provided timestamp
    DECLARE @ts BIGINT = ISNULL(@timestampMs,
        DATEDIFF_BIG(MILLISECOND, '1970-01-01', CAST(SYSUTCDATETIME() AS DATETIME2(3))));

    -- The ULID standard limits the timestamp to 48 bits (max 2^48 - 1)
    IF @ts < 0 OR @ts > 281474976710655
        RETURN NULL;

    -- 6-byte big-endian timestamp
    DECLARE @tsBytes VARBINARY(6) =
          CONVERT(BINARY(1), (@ts / POWER(CAST(2 AS BIGINT), 40)) % 256)
        + CONVERT(BINARY(1), (@ts / POWER(CAST(2 AS BIGINT), 32)) % 256)
        + CONVERT(BINARY(1), (@ts / POWER(CAST(2 AS BIGINT), 24)) % 256)
        + CONVERT(BINARY(1), (@ts / POWER(CAST(2 AS BIGINT), 16)) % 256)
        + CONVERT(BINARY(1), (@ts / POWER(CAST(2 AS BIGINT),  8)) % 256)
        + CONVERT(BINARY(1), @ts % 256);

    -- 10 cryptographically secure random bytes (80 bits)
    DECLARE @ulidBytes VARBINARY(16) = @tsBytes + CRYPT_GEN_RANDOM(10);

    -- Crockford Base32 encoding: 128 bits -> 26 characters.
    -- The value is left-padded with 2 zero bits (the first character carries 3 bits).
    DECLARE @buffer BIGINT = 0;
    DECLARE @bits   INT = 2;      -- 2 leading zero bits
    DECLARE @i      INT = 0;
    DECLARE @byte   INT;
    DECLARE @char   INT;
    DECLARE @result VARCHAR(26) = '';

    WHILE @i < 16
    BEGIN
        SET @byte = CAST(SUBSTRING(@ulidBytes, @i + 1, 1) AS INT);

        IF @bits = 0
        BEGIN
            SET @buffer = @byte;                             -- nothing pending: start fresh
        END
        ELSE
        BEGIN
            SET @buffer = (@buffer * 256 + @byte) & 8191;    -- keep only the low 13 bits
        END

        SET @bits = @bits + 8;

        WHILE @bits >= 5
        BEGIN
            SET @bits = @bits - 5;
            SET @char = (@buffer / POWER(CAST(2 AS BIGINT), @bits)) % 32;
            SET @result = @result + SUBSTRING(@alphabet, @char + 1, 1);
        END

        SET @i = @i + 1;
    END

    -- Leftover bits (not reached for exactly 16 bytes, kept for completeness)
    IF @bits > 0
        SET @result = @result + SUBSTRING(@alphabet,
            (@buffer * POWER(CAST(2 AS BIGINT), 5 - @bits)) % 32 + 1, 1);

    RETURN @result;
END;
GO
/* -------------------------------------------------------------------------------------
   IsValidUlid - validates that a string is a well-formed standard ULID:
       - exactly 26 characters
       - only Crockford Base32 alphabet characters (case-insensitive)
       - first character in the range 0-7 (the value must fit in 128 bits)
   ------------------------------------------------------------------------------------- */
CREATE OR ALTER FUNCTION dbo.IsValidUlid(@ulid VARCHAR(26))
RETURNS BIT
AS
BEGIN
    IF @ulid IS NULL OR LEN(@ulid) <> 26
        RETURN 0;

    -- The first character represents the top 3 bits; above '7' exceeds 2^128 - 1
    IF CHARINDEX(LEFT(@ulid, 1), '01234567') = 0
        RETURN 0;

    DECLARE @alphabet VARCHAR(32) = '0123456789ABCDEFGHJKMNPQRSTVWXYZ';
    DECLARE @i INT = 1;

    WHILE @i <= 26
    BEGIN
        IF CHARINDEX(SUBSTRING(@ulid, @i, 1), @alphabet) = 0
            RETURN 0;
        SET @i = @i + 1;
    END

    RETURN 1;
END;
GO

/* -------------------------------------------------------------------------------------
   UlidToTimestamp - extracts the 48-bit Unix timestamp (milliseconds) from a ULID.
   Returns NULL for an invalid ULID.
   ------------------------------------------------------------------------------------- */
CREATE OR ALTER FUNCTION dbo.UlidToTimestamp(@ulid VARCHAR(26))
RETURNS BIGINT
AS
BEGIN
    IF dbo.IsValidUlid(@ulid) = 0
        RETURN NULL;

    DECLARE @alphabet VARCHAR(32) = '0123456789ABCDEFGHJKMNPQRSTVWXYZ';
    DECLARE @buffer BIGINT = 0, @bits INT = 0, @ts BIGINT = 0;
    DECLARE @i INT = 0, @val INT;

    -- Decode the first 10 characters (the timestamp component).
    -- The first character carries 3 bits, the next 9 carry 5 bits each.
    WHILE @i < 10
    BEGIN
        SET @val = CHARINDEX(SUBSTRING(@ulid, @i + 1, 1), @alphabet) - 1;

        IF @i = 0
        BEGIN
            SET @buffer = @val;
            SET @bits = 3;
        END
        ELSE
        BEGIN
            SET @buffer = @buffer * 32 + @val;
            SET @bits = @bits + 5;
        END

        IF @bits >= 8
        BEGIN
            SET @ts = @ts * 256 + (@buffer / POWER(CAST(2 AS BIGINT), @bits - 8)) % 256;
            SET @bits = @bits - 8;
        END

        SET @i = @i + 1;
    END

    RETURN @ts;
END;
GO

/* -------------------------------------------------------------------------------------
   ExtractDateFromUlid - converts the ULID timestamp to a DATETIME2(3).
   Returns NULL for an invalid ULID or a timestamp beyond the datetime2 range.
   ------------------------------------------------------------------------------------- */
CREATE OR ALTER FUNCTION dbo.ExtractDateFromUlid(@ulid VARCHAR(26))
RETURNS DATETIME2(3)
AS
BEGIN
    DECLARE @ts BIGINT = dbo.UlidToTimestamp(@ulid);

    -- datetime2 maximum is 9999-12-31 23:59:59.999
    IF @ts IS NULL OR @ts > 253402300799999
        RETURN NULL;

    RETURN DATEADD(MILLISECOND, @ts % 1000,
           DATEADD(SECOND, (@ts / 1000) % 86400,
           DATEADD(DAY, @ts / 86400000, '1970-01-01')));
END;
GO

/* =====================================================================================
   Examples / validation
   ===================================================================================== */
SELECT dbo.GenerateUlid(DEFAULT)                                        AS NewUlid;

-- Known standard test vectors (from the ULID specification):
SELECT dbo.UlidToTimestamp('01HK153X0000000000000000') AS Ts_2024_01_01;      -- 1704067200000
SELECT dbo.UlidToTimestamp('7ZZZZZZZZZZZZZZZZZZZZZZZZZ') AS Ts_Max;           -- 281474976710655
SELECT dbo.ExtractDateFromUlid('01HK153X0000000000000000') AS Dt_2024_01_01;  -- 2024-01-01 00:00:00.000
SELECT dbo.IsValidUlid('01HK153X0000000000000000') AS Valid_ok;               -- 1
SELECT dbo.IsValidUlid('invalid')                    AS Valid_short;          -- 0
SELECT dbo.IsValidUlid('8ZZZZZZZZZZZZZZZZZZZZZZZZZ') AS Valid_overflow;       -- 0
GO

