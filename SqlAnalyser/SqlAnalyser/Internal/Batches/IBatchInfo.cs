using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using RoseByte.SqlAnalyser.SqlServer.Internal.Identifiers;

namespace RoseByte.SqlAnalyser.SqlServer.Internal.Batches
{
    public interface IBatchInfo
    {
        int Order { get; }
        string DefaultDatabase { get; }
        string DefaultServer { get; }
        string DefaultSchema { get; }
        string Sql { get; }
        IEnumerable<IdentifierInfo> Doers { get; }
        IEnumerable<IdentifierInfo> References { get; }
        BatchTypes BatchType { get; }
        TSqlBatch Value { get; }
    }
}