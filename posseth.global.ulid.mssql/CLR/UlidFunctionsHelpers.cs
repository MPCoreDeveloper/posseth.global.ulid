//Michel Posseth - 2024-05-26 YYYY-MM-DD
// made type methods static / more robust
// Entry point for SQL Server CLR functions
using System;
using System.Data;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
namespace Posseth.Global.UlidFactory.MSSQL.CLR
{
    public class UlidFunctionsHelpers
    {

        [SqlFunction(IsDeterministic = true, IsPrecise = true)]
        public static SqlString GenerateUlid()
        {
            return new SqlString(Ulid.NewUlid().ToString());
        }
        [SqlFunction(IsDeterministic = true, IsPrecise = true)]
        public static SqlString GenerateUlidWithTimestamp(SqlDateTime timestamp)
        {
            return new SqlString(Ulid.NewUlid(timestamp.Value).ToString());
        }
        [SqlFunction(IsDeterministic = true, IsPrecise = true)]
        public static SqlDateTime ExtractDateFromUlid(SqlString ulidString)
        {
            if (!Ulid.TryParse(ulidString.Value, out Ulid ulid))
                return SqlDateTime.Null;

            return (ulid != null && ulid.HasValue()) ? new SqlDateTime(ulid.ToDateTime()) : SqlDateTime.Null;

        }

    }
}