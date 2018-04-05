using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace SqlAnalyser.Internal.Batches
{
    public interface IBatchFactory
    {
        (IEnumerable<IBatchInfo>, IEnumerable<ParseError>) Generate(string sql, SqlVersion version, string schema, 
            string database, string server);
    }
}