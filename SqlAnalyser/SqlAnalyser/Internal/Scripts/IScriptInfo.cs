using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using SqlAnalyser.Internal.Batches;
using SqlAnalyser.Internal.Identifiers;

namespace SqlAnalyser.Internal.Scripts
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
    }
}