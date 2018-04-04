using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace SqlAnalyser.Internal
{
    public class NameScanner
    {
        public IdentifierInfo ScanForNames(TSqlBatch batch, ReferenceTypes type)
        {
            if (type != ReferenceTypes.Table && type != ReferenceTypes.Function && type != ReferenceTypes.Procedure)
            {
                return null;
            }
            
            var names = new List<IdentifierInfo>();

            if (batch.Statements.Count == 1)
            {
                var statement = batch.Statements.Single();

                if (statement is ProcedureStatementBody proc)
                {
                    return
                        new IdentifierInfo(
                            BatchTypes.Procedure,
                            proc.ProcedureReference.Name.BaseIdentifier.Value,
                            proc.ProcedureReference.Name.SchemaIdentifier?.Value ?? string.Empty,
                            proc.ProcedureReference.Name.DatabaseIdentifier?.Value ?? string.Empty,
                            proc.ProcedureReference.Name.ServerIdentifier?.Value ?? string.Empty);
                }
						
                if (statement is CreateOrAlterFunctionStatement func)
                {
                    return
                        new IdentifierInfo(
                            BatchTypes.Function,
                            func.Name.BaseIdentifier.Value,
                            func.Name.SchemaIdentifier?.Value ?? string.Empty,
                            func.Name.DatabaseIdentifier?.Value ?? string.Empty,
                            func.Name.ServerIdentifier?.Value ?? string.Empty);
                }
                
                if (statement is CreateTableStatement table)
                {
                    return
                        new IdentifierInfo(
                            BatchTypes.Table,
                            table.SchemaObjectName.BaseIdentifier.Value,
                            table.SchemaObjectName.SchemaIdentifier?.Value,
                            table.SchemaObjectName.DatabaseIdentifier?.Value,
                            table.SchemaObjectName.ServerIdentifier?.Value);
                }
            }


            return null;
            //return batch.Statements.Any(x => x is CreateTableStatement) ? BatchTypes.Table : BatchTypes.Other;
        }
    }
}