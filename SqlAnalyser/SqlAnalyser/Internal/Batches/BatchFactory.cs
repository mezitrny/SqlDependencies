using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace SqlAnalyser.Internal.Batches
{
    public class BatchFactory : IBatchFactory
    {
        public (IEnumerable<IBatchInfo>, IEnumerable<ParseError>) Generate(string sql, SqlVersion version, 
            string schema, string database, string server)
        {
            var batches = SqlParser.Parse(sql, version, out var errors);

            var result = batches
                .Select((batch, order) => new BatchInfo(batch, order, schema, database, server))
                .Cast<IBatchInfo>()
                .ToList();

            return (result, errors);
        }
    }
}