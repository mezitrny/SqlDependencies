using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace SqlAnalyser.Internal
{
    public class DoerVisitor : TSqlFragmentVisitor
		{
			public DoerVisitor(string schema = null, string database = null, string server = null)
			{
				_defaultSchema = schema ?? string.Empty;
				_defaultDatabase = database ?? string.Empty;
				_defaultServer = server ?? string.Empty;
			}
			
			private readonly string _defaultSchema;
			private readonly string _defaultDatabase;
			private readonly string _defaultServer;
			
			private List<IdentifierInfo> _names;
			private TSqlBatch _batch;

			public IEnumerable<IdentifierInfo> GetReferences(TSqlBatch batch)
			{
				_names = new List<IdentifierInfo>();
				_batch = batch;
				
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
					BatchTypes.Procedure,
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
		}
}