using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace SqlAnalyser
{
    public class TypeScanner
    {
        public Scripts ScanScriptType(TSqlBatch batch)
        {
            if (batch.Statements.Count == 0)
            {
                return Scripts.Empty;
            }

            if (batch.Statements.Count == 1)
            {
                var statement = batch.Statements.First();

                if (statement is CreateProcedureStatement || statement is AlterProcedureStatement)
                {
                    return Scripts.Procedure;
                }
                
                if (statement is CreateFunctionStatement || statement is AlterFunctionStatement)
                {
                    return Scripts.Function;
                }
            }
            
            return batch.Statements.Any(x => x is CreateTableStatement) ? Scripts.Table : Scripts.Other;
        }
    }
}