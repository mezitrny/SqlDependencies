using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using RoseByte.SqlAnalyser.SqlServer.Internal.Batches;
using RoseByte.SqlAnalyser.SqlServer.Internal.Identifiers;

namespace RoseByte.SqlAnalyser.SqlServer.Internal.Visitors
{
    public class DoerVisitor : TSqlFragmentVisitor, IDoerVisitor
		{
			private string _defaultSchema;
			private string _defaultDatabase;
			private string _defaultServer;
			
			private List<IdentifierInfo> _names;

			public IEnumerable<IdentifierInfo> GetReferences(TSqlBatch batch, string schema = null, 
				string database = null, string server = null)
			{
				_defaultSchema = schema ?? string.Empty;
				_defaultDatabase = database ?? string.Empty;
				_defaultServer = server ?? string.Empty;
				
				_names = new List<IdentifierInfo>();
				
				ExplicitVisit(batch);

				return _names;
			}
			
			public override void Visit(CreateTableStatement table)
			{
				var reference = new IdentifierInfo(
					BatchTypes.Table,
					table.SchemaObjectName.BaseIdentifier.Value,
					table.SchemaObjectName.SchemaIdentifier?.Value,
					table.SchemaObjectName.DatabaseIdentifier?.Value,
					table.SchemaObjectName.ServerIdentifier?.Value);
				
				reference.Schema.DefaultName = _defaultSchema;
				reference.Database.DefaultName = _defaultDatabase;
				reference.Server.DefaultName = _defaultServer;
				
				_names.Add(reference);

				base.Visit(table);
			}
			
			public override void Visit(AlterTableStatement table)
			{
				var reference = new IdentifierInfo(
					BatchTypes.Table,
					table.SchemaObjectName.BaseIdentifier.Value,
					table.SchemaObjectName.SchemaIdentifier?.Value,
					table.SchemaObjectName.DatabaseIdentifier?.Value,
					table.SchemaObjectName.ServerIdentifier?.Value);
				
				reference.Schema.DefaultName = _defaultSchema;
				reference.Database.DefaultName = _defaultDatabase;
				reference.Server.DefaultName = _defaultServer;
				
				_names.Add(reference);

				base.Visit(table);
			}
			
			public override void Visit(FunctionStatementBody node)
			{
				var reference = new IdentifierInfo(
					BatchTypes.Function,
					node.Name.BaseIdentifier.Value,
					node.Name.SchemaIdentifier?.Value ?? string.Empty,
					node.Name.DatabaseIdentifier?.Value ?? string.Empty,
					node.Name.ServerIdentifier?.Value ?? string.Empty);
				
				reference.Schema.DefaultName = _defaultSchema;
				reference.Database.DefaultName = _defaultDatabase;
				reference.Server.DefaultName = _defaultServer;
				
				_names.Add(reference);

				base.Visit(node);
			}
			
			public override void Visit(ProcedureStatementBody node)
			{
				var reference = new IdentifierInfo(
					BatchTypes.Procedure,
					node.ProcedureReference.Name.BaseIdentifier.Value,
					node.ProcedureReference.Name.SchemaIdentifier?.Value ?? string.Empty,
					node.ProcedureReference.Name.DatabaseIdentifier?.Value ?? string.Empty,
					node.ProcedureReference.Name.ServerIdentifier?.Value ?? string.Empty);
				
				reference.Schema.DefaultName = _defaultSchema;
				reference.Database.DefaultName = _defaultDatabase;
				reference.Server.DefaultName = _defaultServer;
				
				_names.Add(reference);

				base.Visit(node);
			}

			public override void Visit(ViewStatementBody node)
			{
				var reference = new IdentifierInfo(
					BatchTypes.View,
					node.SchemaObjectName.BaseIdentifier.Value,
					node.SchemaObjectName.SchemaIdentifier?.Value ?? string.Empty,
					node.SchemaObjectName.DatabaseIdentifier?.Value ?? string.Empty,
					node.SchemaObjectName.ServerIdentifier?.Value ?? string.Empty);
				
				reference.Schema.DefaultName = _defaultSchema;
				reference.Database.DefaultName = _defaultDatabase;
				reference.Server.DefaultName = _defaultServer;
				
				_names.Add(reference);

				base.Visit(node);
			}
		}
}