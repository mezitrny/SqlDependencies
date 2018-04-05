using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using SqlAnalyser.Internal.Identifiers;

namespace SqlAnalyser.Internal.Visitors
{
    public interface IReferenceVisitor
    {
        (IEnumerable<IdentifierInfo>, IEnumerable<IdentifierInfo>) GetReferences(TSqlBatch batch, 
            IEnumerable<IdentifierInfo> doers, string schema = null, string database = null, string server = null);
    }
}