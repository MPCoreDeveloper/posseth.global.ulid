# Posseth.Global.UlidFactory

## Overview
MyLibrary is a C# library that provides...ULID generation and date time conversion from the ulid.

## Installation
To install MyLibrary, add the following reference to your project:
Posseth.Global.UlidFactory

## Usage
To use Posseth.Global.UlidFactory in a C# project, add the following using statement to your C# file:

```csharp	
using Posseth.Global.UlidFactory;

// Generate a new ULID
 string ulid = Ulid.NewUlid();	

// Convert a ULID to a DateTime
 DateTime dateTimeFromUlid = Ulid.GetTimestampFromUlid(ulid);
```


