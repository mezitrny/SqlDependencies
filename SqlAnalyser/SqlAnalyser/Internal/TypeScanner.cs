using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace SqlAnalyser.Internal
{
    public class TypeScanner
    {
        public BatchTypes ScanScriptType(TSqlBatch batch)
        {
            if (batch.Statements.Count == 0)
            {
                return BatchTypes.Empty;
            }

            if (batch.Statements.Count == 1)
            {
                var statement = batch.Statements.First();

                if (statement is CreateProcedureStatement || statement is AlterProcedureStatement)
                {
                    return BatchTypes.Procedure;
                }
                
                if (statement is CreateFunctionStatement || statement is AlterFunctionStatement)
                {
                    return BatchTypes.Function;
                }
            }
            
            return batch.Statements.Any(x => x is CreateTableStatement) ? BatchTypes.Table : BatchTypes.Other;
        }
    }
}