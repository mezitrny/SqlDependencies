using System.Collections.Generic;
using Microsoft.SqlServer.Management.SqlParser.SqlCodeDom;
using RoseByte.SqlAnalyser.SqlServer.Internal.Identifiers;

namespace RoseByte.SqlAnalyser.SqlServer.Internal
{
	public class DefinitionsScanner
	{
		public IEnumerable<IdentifierInfo> GetDefinitions(SqlBatch batch) => Visit(batch);
		
		private IdentifierInfo ParseIdentifier(SqlObjectIdentifier sql, IdentifierTypes type)
		{
			return new IdentifierInfo(type, sql.ObjectName.Value, sql.SchemaName.Value,
				sql.DatabaseName.Value, sql.ServerName.Value);
		}

		private IEnumerable<IdentifierInfo> Visit(SqlCodeObject item)
		{
			switch (item)
			{
				case SqlCreateFunctionStatement function:
					yield return ParseIdentifier(function.Definition.Name, IdentifierTypes.Function);
					break;
				case SqlCreateProcedureStatement procedure:
					yield return ParseIdentifier(procedure.Definition.Name, IdentifierTypes.Procedure);
					break;
				case SqlCreateTableStatement table:
					yield return ParseIdentifier(table.Name, IdentifierTypes.Table);
					break;
				case SqlCreateViewStatement table:
					yield return ParseIdentifier(table.Definition.Name, IdentifierTypes.View);
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