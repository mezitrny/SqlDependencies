using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace SqlAnalyser.Internal.Batches
{
    public interface IBatchInfo
    {
        int Order { get; }
        string DefaultDatabase { get; set; }
        string DefaultServer { get; set; }
        string DefaultSchema { get; set; }
        string Sql { get; }
        IEnumerable<IdentifierInfo> Doers { get; }
        IEnumerable<IdentifierInfo> References { get; }
        BatchTypes BatchType { get; }
        TSqlBatch Value { get; }
    }
}