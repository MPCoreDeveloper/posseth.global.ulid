# Posseth.UlidFactory
![ulid](https://raw.githubusercontent.com/MPCoreDeveloper/posseth.global.ulid/master/assets/ulid.png)

[![NuGet](https://img.shields.io/nuget/v/Posseth.UlidFactory.svg)](https://www.nuget.org/packages/Posseth.UlidFactory/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-10.0-blue.svg)](https://dotnet.microsoft.com/download)
[![Sponsor](https://img.shields.io/badge/Sponsor-GitHub-ea4aaa?logo=github)](https://github.com/sponsors/MPCoreDeveloper)

## Overview
This library is a C# library that provides a Ulid type in .NET (C#, VB.NET tested by DEV)

## What is a ULID?
A ULID (Universally Unique Lexicographically Sortable Identifier) is a 128-bit identifier that is designed to be unique and time-ordered.
It is similar to a UUID (Universally Unique Identifier) but has the additional property of being sortable by time.
A ULID consists of two components: a 48-bit timestamp and a 80-bit random value.
The timestamp is encoded in Crockford's Base32 format, which allows the ULID to be lexicographically sortable.
The random value provides uniqueness and helps prevent collisions between ULIDs generated at the same time.

## Features
Posseth.UlidFactory library provides the following features:
- ULID generation with current or custom timestamps
- Parsing and validation of ULID strings
- DateTime, Epoch, and UnixTime conversion from ULIDs
- Handle new ULIDs or existing ULIDs read from databases
- High-performance implementation with minimal allocations
- SQL Server CLR integration support

## Recent Improvements (C# 14 & .NET 10 Compatibility)
The library has been fully modernized to leverage the latest C# 14 and .NET 10 features:

### Performance & Modern Syntax
- **Record Type**: Converted to a `record` for built-in immutability, equality, and hashing support
- **File-Scoped Namespaces**: Adopted file-scoped namespaces to reduce indentation and improve readability
- **Span<T> and ReadOnlySpan<T>**: Utilized with `stackalloc` to minimize heap allocations and enhance performance
- **Expression-Bodied Members**: Simplified method implementations for better readability
- **Pattern Matching**: Used `or` patterns and `when` clauses for cleaner conditional logic
- **var Keyword**: Improved code readability throughout the codebase

### Enhanced Error Handling
- **ArgumentOutOfRangeException.ThrowIfNegative**: Modern validation for timestamp parameters
- **ArgumentException.ThrowIfNullOrEmpty**: Cleaner null/empty checks
- **RandomNumberGenerator.Fill**: Direct span filling for better performance
- **Collection Expressions**: Used `[]` syntax for empty arrays

### Test Infrastructure
- **Updated to xUnit 2.9.3**: Latest test framework version
- **Microsoft.NET.Test.Sdk 18.0.1**: Modern test SDK
- **Removed obsolete packages**: Cleaned up Microsoft.CodeAnalysis.Testing dependencies

These changes maintain full backward compatibility while aligning with C# 14 and .NET 10 best practices, resulting in better performance and maintainability.

## Installation

### NuGet Package
```powershell
Install-Package Posseth.UlidFactory
```

Or via .NET CLI:
```bash
dotnet add package Posseth.UlidFactory
```

### Manual Reference
Add the following using statement to your C# file:
```csharp
using Posseth.UlidFactory;
```

## Usage

### Generating a New ULID
You can generate a new ULID using the `NewUlid` method. This can be done with the current timestamp or with a specified timestamp.

```csharp	
using Posseth.UlidFactory;

// Generate a new ULID with the current timestamp
var newUlid = Ulid.NewUlid();
Console.WriteLine($"New ULID: {newUlid}");

// Generate a new ULID with a specified DateTime
var specifiedTime = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc);
var ulidWithTime = Ulid.NewUlid(specifiedTime);
Console.WriteLine($"ULID with specified time: {ulidWithTime}");

// Generate a new ULID with a specified Unix timestamp
const long unixTimestamp = 1672444800000; // Unix timestamp for 2023-01-01 00:00:00 UTC
var ulidWithUnixTime = Ulid.NewUlid(unixTimestamp);
Console.WriteLine($"ULID with specified Unix timestamp: {ulidWithUnixTime}");
```

### Parsing an Existing ULID String
You can parse an existing ULID string using the `Parse` method. If the ULID string is invalid, an exception will be thrown.
Alternatively, you can use the `TryParse` method to avoid exceptions and handle invalid ULIDs more gracefully.

```csharp	
using Posseth.UlidFactory;

// Parse a valid ULID string
const string ulidString = "01BX5ZZKBKACTAV9WEVGEMMVRZ";
try
{
    var parsedUlid = Ulid.Parse(ulidString);
    Console.WriteLine($"Parsed ULID: {parsedUlid}");
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Invalid ULID string: {ex.Message}");
}

// Try to parse a ULID string and handle invalid ULID gracefully
var success = Ulid.TryParse(ulidString, out var ulid);
if (success)
{
    Console.WriteLine($"Successfully parsed ULID: {ulid}");
}
else
{
    Console.WriteLine("Failed to parse ULID.");
}
```

### Extracting the Timestamp from a ULID
You can extract the timestamp from a ULID and convert it to a DateTime or Unix timestamp using the `ToDateTime`, `ToEpoch`, or `ToUnixTime` methods.

```csharp	
using Posseth.UlidFactory;

// Generate a new ULID
var newUlid = Ulid.NewUlid();
Console.WriteLine($"New ULID: {newUlid}");

// Get the timestamp as a DateTime
var timestamp = newUlid.ToDateTime();
Console.WriteLine($"Timestamp as DateTime: {timestamp}");

// Get the Unix timestamp in milliseconds
var unixTimestamp = newUlid.ToUnixTime();
Console.WriteLine($"Timestamp as Unix time: {unixTimestamp}");

// Extract timestamp using static method
var extractedTimestamp = Ulid.GetTimestampFromUlid(newUlid);
Console.WriteLine($"Extracted timestamp: {extractedTimestamp}");
```

## API Reference

### Static Methods
- `NewUlid()` - Generate a ULID with current timestamp
- `NewUlid(DateTimeOffset timestamp)` - Generate a ULID with specified DateTimeOffset
- `NewUlid(DateTime timestamp)` - Generate a ULID with specified DateTime
- `NewUlid(long timestamp)` - Generate a ULID with Unix timestamp (milliseconds)
- `Parse(string ulidString)` - Parse a ULID string (throws on invalid input)
- `TryParse(string ulidString, out Ulid? ulid)` - Try to parse a ULID string (returns bool)
- `GetTimestampFromUlid(Ulid ulid)` - Extract DateTime from a ULID

### Instance Methods
- `ToDateTime()` - Convert ULID to DateTime
- `ToEpoch()` - Convert ULID to Unix timestamp (milliseconds)
- `ToUnixTime()` - Convert ULID to Unix timestamp (milliseconds) - alias for ToEpoch
- `HasValue()` - Check if ULID has a valid value
- `ToString()` - Get string representation of ULID

---

# Posseth.Global.UlidFactory.MSSQL
SQL Server CLR integration for ULID generation and manipulation.

## Requirements
- SQL Server 2016 or higher
- CLR integration enabled
- .NET Framework 4.8.1

## Installation

### Step 1: Enable CLR Integration
First, you need to enable CLR integration on your SQL Server instance:

```sql
sp_configure 'clr enabled', 1;
RECONFIGURE;
```

### Step 2: Create Assembly Hash
Build the project and create a file hash of the assembly:

```powershell
certutil -hashfile "C:\path\to\your\Posseth.Global.UlidFactory.MSSQL.dll" SHA512
```

The result should look like:
```
b424f44500e83e72c8b4b60528250a40a4eabfddbb6b9c0e93208e6ddc63815e80180f26814f85bdad5ba2c88209d732397c7599e809da6df5146cc94daa250e
```

### Step 3: Trust the Assembly
Before adding the assembly to the database, you need to tell SQL Server that you trust it:

```sql
-- Step 1: Declare a variable and convert the hash to a binary value
DECLARE @binaryHash varbinary(64);
SET @binaryHash = CONVERT(varbinary(64), 0xB424F44500E83E72C8B4B60528250A40A4EABFDDBB6B9C0E93208E6DDC63815E80180F26814F85BDAD5BA2C88209D732397C7599E809DA6DF5146CC94DAA250E, 1);

-- Step 2: Use the variable as parameter in the stored procedure
EXEC sp_add_trusted_assembly @hash = @binaryHash;
```

### Step 4: Create the Assembly
Now you can add the assembly to the database:

```sql
CREATE ASSEMBLY UlidFactoryMSSQL
FROM 'C:\path\to\your\Posseth.Global.UlidFactory.MSSQL.dll'
WITH PERMISSION_SET = SAFE;
```

You should see: **'Commands completed successfully.'**

### Step 5: Create the Functions
Create the CLR functions in your database:

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

You should see: **'Commands completed successfully.'**

## Usage in SQL Server

### Generate ULIDs
```sql
-- Generate a new ULID with current timestamp
SELECT dbo.GenerateUlid();
-- Result: 01BX5ZZKBKACTAV9WEVGEMMVRZ

-- Generate a ULID with specific timestamp
SELECT dbo.GenerateUlidWithTimestamp(GETDATE());
SELECT dbo.GenerateUlidWithTimestamp('2023-01-01 00:00:00');

-- Extract DateTime from ULID
SELECT dbo.ExtractDateFromUlid('01F8MECHZX3TBDSZ7FBJ4H7FJ6');
-- Result: 1982-11-13 14:32:34.560
```

### Practical Examples
```sql
-- Use ULID as primary key in table
CREATE TABLE Orders (
    OrderId NVARCHAR(26) PRIMARY KEY DEFAULT (dbo.GenerateUlid()),
    CustomerName NVARCHAR(100),
    OrderDate DATETIME DEFAULT GETDATE()
);

-- Insert records with auto-generated ULID
INSERT INTO Orders (CustomerName) VALUES ('John Doe');
INSERT INTO Orders (CustomerName) VALUES ('Jane Smith');

-- Query with timestamp extraction
SELECT 
    OrderId,
    CustomerName,
    dbo.ExtractDateFromUlid(OrderId) AS UlidTimestamp,
    OrderDate
FROM Orders;
```

## Pre-compiled DLL
For your convenience, a compiled DLL is included in the repository. You can use the above commands to add the assembly to your database without building the project yourself.

---

## Support This Project ❤️

If you find this library useful, please consider supporting the development:

[![Sponsor on GitHub](https://img.shields.io/badge/Sponsor-GitHub-ea4aaa?style=for-the-badge&logo=github)](https://github.com/sponsors/MPCoreDeveloper)

Your support helps maintain and improve this project. Thank you! 🙏

---

## Summary
The Ulid class provides methods for generating, parsing, and extracting timestamps from ULIDs.
By following the examples above, you can incorporate ULID generation and handling in your C# and SQL Server applications.
The class ensures that the ULIDs generated are unique and time-ordered, providing a robust solution for use cases requiring such identifiers.

## License
This project is licensed under the MIT License - see the [LICENSE.txt](LICENSE.txt) file for details.

## Contributing
Contributions are welcome! Please feel free to submit a Pull Request.

## Author
**Michel Posseth** - [MPCoreDeveloper](https://github.com/MPCoreDeveloper)

## Acknowledgments
- Based on the ULID specification
- Built with modern C# 14 and .NET 10 features
- Tested on Visual Studio 2026