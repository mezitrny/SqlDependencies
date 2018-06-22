using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using RoseByte.SqlAnalyser.SqlServer.Internal.Identifiers;

namespace RoseByte.SqlAnalyser.SqlServer.Internal.Visitors
{
    public interface IDoerVisitor
    {
        IEnumerable<IdentifierInfo> GetReferences(TSqlBatch batch, string schema = null, string database = null, 
            string server = null);
    }
}