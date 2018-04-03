using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace SqlAnalyser
{
    public class ReferenceScanner : TSqlFragmentVisitor
		{
			public ReferenceScanner(string schema = "null", string database = null, string server = null)
			{
				_defaultSchema = schema ?? string.Empty;
				_defaultDatabase = database ?? string.Empty;
				_defaultServer = server ?? string.Empty;
			}
			
			private readonly string _defaultSchema;
			private readonly string _defaultDatabase;
			private readonly string _defaultServer;
			
			private List<ReferenceInfo> _references;
			private TSqlBatch _batch;

			private ReferenceInfo _myName;

			private ReferenceInfo MyName
			{
				get
				{
					if (_myName == null)
					{
						var statement = _batch.Statements.SingleOrDefault();
						if (statement is ProcedureStatementBody proc)
						{
							_myName = new ReferenceInfo(
								Scripts.Procedure,
								proc.ProcedureReference.Name.BaseIdentifier.Value,
								proc.ProcedureReference.Name.SchemaIdentifier?.Value ?? string.Empty,
								proc.ProcedureReference.Name.DatabaseIdentifier?.Value ?? string.Empty,
								proc.ProcedureReference.Name.ServerIdentifier?.Value ?? string.Empty);

							_myName.Schema.DefaultName = _defaultSchema;
							_myName.Database.DefaultName = _defaultDatabase;
							_myName.Server.DefaultName = _defaultServer;
						}
						
						if (statement is CreateOrAlterFunctionStatement func)
						{
							_myName = new ReferenceInfo(
								Scripts.Function,
								func.Name.BaseIdentifier.Value,
								func.Name.SchemaIdentifier?.Value ?? string.Empty,
								func.Name.DatabaseIdentifier?.Value ?? string.Empty,
								func.Name.ServerIdentifier?.Value ?? string.Empty);
							
							_myName.Schema.DefaultName = _defaultSchema;
							_myName.Database.DefaultName = _defaultDatabase;
							_myName.Server.DefaultName = _defaultServer;
						}
					}

					return _myName;
				}
			}

			public IEnumerable<ReferenceInfo> GetReferences(TSqlBatch batch)
			{
				_references = new List<ReferenceInfo>();
				_batch = batch;
				_myName = null;
				
				ExplicitVisit(batch);

				return _references;
			}
			
			public override void Visit(ProcedureReference node)
			{
				var reference = new ReferenceInfo(
					Scripts.Procedure,
					node.Name.BaseIdentifier.Value,
					node.Name.SchemaIdentifier?.Value ?? string.Empty,
					node.Name.DatabaseIdentifier?.Value ?? string.Empty,
					node.Name.ServerIdentifier?.Value ?? string.Empty);
				
				reference.Schema.DefaultName = _defaultSchema;
				reference.Database.DefaultName = _defaultDatabase;
				reference.Server.DefaultName = _defaultServer;

				if (reference != MyName)
				{
					_references.Add(reference);
				}
				
				base.Visit(node);
			}

			public override void Visit(FunctionCall node)
			{
				_references.Add(new ReferenceInfo(
					Scripts.Function, 
					node.FunctionName.Value));
				
				base.Visit(node);
			}
						
			public override void Visit(NamedTableReference node)
			{
				var reference = new ReferenceInfo(
					Scripts.Table, 
					node.SchemaObject.BaseIdentifier.Value,
					node.SchemaObject.SchemaIdentifier?.Value ?? string.Empty,
					node.SchemaObject.DatabaseIdentifier?.Value ?? string.Empty,
					node.SchemaObject.ServerIdentifier?.Value ?? string.Empty);
				
				reference.Schema.DefaultName = _defaultSchema;
				reference.Database.DefaultName = _defaultDatabase;
				reference.Server.DefaultName = _defaultServer;

				_references.Add(reference);
				
				base.Visit(node);
			}
		}
}