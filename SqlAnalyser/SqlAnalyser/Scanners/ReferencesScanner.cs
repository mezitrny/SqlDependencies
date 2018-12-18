using System.Collections.Generic;
using Microsoft.SqlServer.Management.SqlParser.SqlCodeDom;
using RoseByte.SqlAnalyser.SqlServer.Internal.Identifiers;

namespace RoseByte.SqlAnalyser.SqlServer.Internal
{
	public class ReferencesScanner
	{
		private readonly List<string> _ctes = new List<string>();

		public IEnumerable<IdentifierInfo> GetReferences(SqlBatch batch) => Visit(batch);
		
		private static IdentifierInfo ParseIdentifier(SqlObjectIdentifier sql, IdentifierTypes type)
		{
			return new IdentifierInfo(type, sql.ObjectName.Value, sql.SchemaName.Value,
				sql.DatabaseName.Value, sql.ServerName.Value);
		}

		private IEnumerable<IdentifierInfo> Visit(SqlCodeObject item)
		{
			switch (item)
			{
				case SqlTableRefExpression table:
					if (!_ctes.Contains(table.ObjectIdentifier.ObjectName.Value))
					{
						yield return ParseIdentifier(table.ObjectIdentifier, IdentifierTypes.Table);
					}
					break;
				case SqlTableValuedFunctionRefExpression function:
					yield return ParseIdentifier(function.ObjectIdentifier, IdentifierTypes.Function);
					break;
				case SqlAlterFunctionStatement alterFunction:
					yield return ParseIdentifier(alterFunction.Definition.Name, IdentifierTypes.Function);
					foreach (var subItem in item.Children)
					{
						foreach (var identifierInfo in Visit(subItem))
						{
							yield return identifierInfo;
						}
					}
					break;
				case SqlAlterProcedureStatement alterProcedure:
					yield return ParseIdentifier(alterProcedure.Definition.Name, IdentifierTypes.Procedure);
					foreach (var subItem in item.Children)
					{
						foreach (var identifierInfo in Visit(subItem))
						{
							yield return identifierInfo;
						}
					}
					break;
				case SqlBuiltinScalarFunctionCallExpression scalarBuildin:
					yield return new IdentifierInfo(IdentifierTypes.Function, scalarBuildin.FunctionName, null, null, null);
					break;
				case SqlUserDefinedScalarFunctionCallExpression scalar:
					yield return ParseIdentifier(scalar.ObjectIdentifier, IdentifierTypes.Function);
					break;
				case SqlExecuteModuleStatement execute:
					yield return ParseIdentifier(execute.Module.ObjectIdentifier, IdentifierTypes.Procedure);
					break;
				case SqlCommonTableExpression cte:
					_ctes.Add(cte.Name.Value);
					break;
				default:
					foreach (var subItem in item.Children)
					{
						foreach (var identifierInfo in Visit(subItem))
						{
							yield return identifierInfo;
						}
					}
					break;
			}
		}
	}
}