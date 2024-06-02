# Posseth.Global.UlidFactory

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
Posseth.Global.UlidFactory library provides the following features:
ULID generation Parsing and DateTime , Epoch , UnixTime conversion from the ulid.
it can handle new Ulids but also existing Ulids read from Database and optionally convert them to DateTime , Epoch , UnixTime

## Installation
To install Posseth.Global.UlidFactory, add the following reference to your project:
using Posseth.Global.UlidFactory;

## Usage
To use Posseth.Global.UlidFactory in a C# project, add the following using statement to your C# file:


Generating a New ULID
You can generate a new ULID using the NewUlid method. This can be done with the current timestamp or with a specified timestamp.
```csharp	
using System;
using Posseth.Global.UlidFactory;

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
using Posseth.Global.UlidFactory;

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
using Posseth.Global.UlidFactory;

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


