using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using RoseByte.SqlAnalyser.SqlServer.Internal.Batches;
using RoseByte.SqlAnalyser.SqlServer.Internal.Identifiers;

namespace RoseByte.SqlAnalyser.SqlServer.Internal.Scripts
{
    public interface IScriptInfo
    {
        string Sql { get; }
        SqlVersion Version { get; }
        string DefaultDatabase { get; }
        string DefaultServer { get; }
        string DefaultSchema { get; }
        IEnumerable<ParseError> Errors { get; }
        bool Valid { get; }
        IEnumerable<IBatchInfo> Batches { get; }
        IEnumerable<IdentifierInfo> Doers { get; }
        IEnumerable<IdentifierInfo> References { get; }
        IdentifierInfo Doer { get; }
        BatchTypes BatchType { get; }
    }
}