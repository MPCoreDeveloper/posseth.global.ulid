# Posseth.UlidFactory
![ulid](https://raw.githubusercontent.com/MPCoreDeveloper/posseth.global.ulid/master/assets/ulid.png)
## Overview
This library is a C# library that provides a Ulid type in .Net 
(C#,VB.Net tested by DEV)

## What is a ULID?
A ULID (Universally Unique Lexicographically Sortable Identifier) is a 128-bit identifier that is designed to be unique and time-ordered.
It is similar to a UUID (Universally Unique Identifier) but has the additional property of being sortable by time.
A ULID consists of two components: a 48-bit timestamp and a 80-bit random value.
The timestamp is encoded in Crockford's Base32 format, which allows the ULID to be lexicographically sortable.
The random value provides uniqueness and helps prevent collisions between ULIDs generated at the same time.

## Features
Posseth.UlidFactory library provides the following features:
ULID generation Parsing and DateTime , Epoch , UnixTime conversion from the ulid.
it can handle new Ulids but also existing Ulids read from Database and optionally convert them to DateTime , Epoch , UnixTime

## Installation
To install Posseth.UlidFactory, add the following reference to your project:
using Posseth.UlidFactory;

## Usage
To use Posseth.UlidFactory in a C# project, add the following using statement to your C# file:


Generating a New ULID
You can generate a new ULID using the NewUlid method. This can be done with the current timestamp or with a specified timestamp.
```csharp	
using System;
using Posseth.UlidFactory;

class Program
{
    static void Main()
    {
        // Generate a new ULID with the current timestamp
        Ulid newUlid = Ulid.NewUlid();
        Console.WriteLine("New ULID: " + newUlid);

        // Generate a new ULID with a specified DateTime
        DateTime specifiedTime = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        Ulid ulidWithTime = Ulid.NewUlid(specifiedTime);
        Console.WriteLine("ULID with specified time: " + ulidWithTime);

        // Generate a new ULID with a specified Unix timestamp
        long unixTimestamp = 1672444800000; // Unix timestamp for 2023-01-01 00:00:00 UTC
        Ulid ulidWithUnixTime = Ulid.NewUlid(unixTimestamp);
        Console.WriteLine("ULID with specified Unix timestamp: " + ulidWithUnixTime);
    }
}
```
Parsing an Existing ULID String
You can parse an existing ULID string using the Parse method. If the ULID string is invalid, an exception will be thrown.
Alternatively, you can us the TryParse method to avoid exceptions and handle invalid ULIDs more gracefully.
```csharp	
using System;
using Posseth.UlidFactory;

class Program
{
    static void Main()
    {
        // Parse a valid ULID string
        string ulidString = "01BX5ZZKBKACTAV9WEVGEMMVRZ";
        try
        {
            Ulid parsedUlid = Ulid.Parse(ulidString);
            Console.WriteLine("Parsed ULID: " + parsedUlid);
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine("Invalid ULID string: " + ex.Message);
        }

        // Try to parse a ULID string and handle invalid ULID gracefully
        Ulid? ulid;
        bool success = Ulid.TryParse(ulidString, out ulid);
        if (success)
        {
            Console.WriteLine("Successfully parsed ULID: " + ulid);
        }
        else
        {
            Console.WriteLine("Failed to parse ULID.");
        }
    }
}

```
Extracting the Timestamp from a ULID
You can extract the timestamp from a ULID and convert it to a DateTime or Unix timestamp using the ToDateTime, ToEpoch, or ToUnixTime methods.
```csharp	
using System;
using Posseth.UlidFactory;

class Program
{
    static void Main()
    {
        // Generate a new ULID
        Ulid newUlid = Ulid.NewUlid();
        Console.WriteLine("New ULID: " + newUlid);

        // Get the timestamp as a DateTime
        DateTime timestamp = newUlid.ToDateTime();
        Console.WriteLine("Timestamp as DateTime: " + timestamp);

        // Get the Unix timestamp in milliseconds
        long unixTimestamp = newUlid.ToUnixTime();
        Console.WriteLine("Timestamp as Unix time: " + unixTimestamp);

        // Extract timestamp using static method
        DateTime extractedTimestamp = Ulid.GetTimestampFromUlid(newUlid);
        Console.WriteLine("Extracted timestamp: " + extractedTimestamp);
    }
}

```
In summary, the Ulid class provides methods for generating, parsing, and extracting timestamps from ULIDs.
By following the examples above, you can incorporate ULID generation and handling in your C# applications.
The class ensures that the ULIDs generated are unique and time-ordered, providing a robust solution for use cases requiring such identifiers.

# Posseth.Global.UlidFactory.MSSQL
This library is a C# library that provides a Ulid type in .Net MSSQL CLR 

## Installation
Build the project and add the Posseth.Global.UlidFactory.MSSQL.dll assembly to your MSSQL database

First, you need to enable CLR integration on your SQL Server instance by running the following command:
```sql
sp_configure 'clr enabled', 1;
RECONFIGURE;
```
First you need to create a file hash of the assembly
```powershell
certutil -hashfile "C:\path\to\your\Posseth.Global.UlidFactory.MSSQL.dll" SHA512
```
the result should be something like this
"b424f44500e83e72c8b4b60528250a40a4eabfddbb6b9c0e93208e6ddc63815e80180f26814f85bdad5ba2c88209d732397c7599e809da6df5146cc94daa250e"

Before you add the assembly to the database you need to tell MSSQL that you trust the assembly by running the following command
```sql
-- Step 1: Declare a variabel and convert the hash to a binary value
DECLARE @binaryHash varbinary(64);
SET @binaryHash = CONVERT(varbinary(64), 0xB424F44500E83E72C8B4B60528250A40A4EABFDDBB6B9C0E93208E6DDC63815E80180F26814F85BDAD5BA2C88209D732397C7599E809DA6DF5146CC94DAA250E, 1);

-- Step 2:  use the variabel as parameter in the stored procedure
EXEC sp_add_trusted_assembly 
    @hash = @binaryHash;

```
Now you can add the assembly to the database
```sql
CREATE ASSEMBLY UlidFactoryMSSQL
FROM 'C:\path\to\your\Posseth.Global.UlidFactory.MSSQL.dll'
WITH PERMISSION_SET = SAFE;

```
The execute should give you 'Commands completed successfully.'
now you can create the Ulid type in the database
```sql
CREATE FUNCTION dbo.GenerateUlid()
RETURNS NVARCHAR(100)
AS EXTERNAL NAME [UlidFactoryMSSQL].[Posseth.Global.UlidFactory.MSSQL.CLR.UlidFunctionsHelpers].[GenerateUlid];
GO

CREATE FUNCTION dbo.GenerateUlidWithTimestamp(@timestamp DATETIME)
RETURNS NVARCHAR(100)
AS EXTERNAL NAME [UlidFactoryMSSQL].[Posseth.Global.UlidFactory.MSSQL.CLR.UlidFunctionsHelpers].[GenerateUlidWithTimestamp];
GO

CREATE FUNCTION dbo.ExtractDateFromUlid(@ulidString NVARCHAR(100))
RETURNS DATETIME
AS EXTERNAL NAME [UlidFactoryMSSQL].[Posseth.Global.UlidFactory.MSSQL.CLR.UlidFunctionsHelpers].[ExtractDateFromUlid];
GO
```
The execute should give you 'Commands completed successfully.'
Now you can use the functions in your database
```sql
SELECT dbo.GenerateUlid();
SELECT dbo.GenerateUlidWithTimestamp(GETDATE());
SELECT dbo.ExtractDateFromUlid('01F8MECHZX3TBDSZ7FBJ4H7FJ6');
```
The execute should give you a new Ulid, a new Ulid with a timestamp and the timestamp extracted from the Ulid (1982-11-13 14:32:34.560)
for your convenience I added the compiled DLL to the repository so you can use the above commands to add the assembly to your database