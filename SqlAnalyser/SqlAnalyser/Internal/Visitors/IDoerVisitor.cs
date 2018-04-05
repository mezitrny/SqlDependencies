using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using SqlAnalyser.Internal.Identifiers;

namespace SqlAnalyser.Internal.Visitors
{
    public interface IDoerVisitor
    {
        IEnumerable<IdentifierInfo> GetReferences(TSqlBatch batch, string schema = null, string database = null, 
            string server = null);
    }
}