using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace SqlAnalyser
{
    public class ReferenceScanner : TSqlFragmentVisitor
		{
			public ReferenceScanner(string schema = "dbo", string database = null, string server = null)
			{
				_defaultSchema = schema;
				_defaultDatabase = database;
				_defaultServer = server;
			}
			
			private readonly string _defaultSchema;
			private readonly string _defaultDatabase;
			private readonly string _defaultServer;
			
			private List<Reference> _references;
			private TSqlBatch _batch;

			private Reference _myName;

			private Reference MyName
			{
				get
				{
					if (_myName == null)
					{
						var statement = _batch.Statements.SingleOrDefault();
						if (statement is ProcedureStatementBody proc)
						{
							_myName = new Reference(
								Scripts.Procedure,
								proc.ProcedureReference.Name.BaseIdentifier.Value,
								proc.ProcedureReference.Name.SchemaIdentifier?.Value ?? _defaultSchema,
								proc.ProcedureReference.Name.DatabaseIdentifier?.Value ?? _defaultDatabase,
								proc.ProcedureReference.Name.ServerIdentifier?.Value ?? _defaultServer);
						}
						
						if (statement is CreateOrAlterFunctionStatement func)
						{
							_myName = new Reference(
								Scripts.Function,
								func.Name.BaseIdentifier.Value,
								func.Name.SchemaIdentifier?.Value ?? _defaultSchema,
								func.Name.DatabaseIdentifier?.Value ?? _defaultDatabase,
								func.Name.ServerIdentifier?.Value ?? _defaultServer);
						}
					}

					return _myName;
				}
			}

			public IEnumerable<Reference> GetReferences(TSqlBatch batch)
			{
				_references = new List<Reference>();
				_batch = batch;
				_myName = null;
				
				ExplicitVisit(batch);

				return _references;
			}
			
			public override void Visit(ProcedureReference node)
			{
				var reference = new Reference(
					Scripts.Procedure,
					node.Name.BaseIdentifier.Value,
					node.Name.SchemaIdentifier?.Value ?? _defaultSchema,
					node.Name.DatabaseIdentifier?.Value ?? _defaultDatabase,
					node.Name.ServerIdentifier?.Value ?? _defaultServer);

				if (reference != MyName)
				{
					_references.Add(reference);
				}
				
				base.Visit(node);
			}

			public override void Visit(FunctionCall node)
			{
				_references.Add(new Reference(
					Scripts.Function, 
					node.FunctionName.Value));
				
				base.Visit(node);
			}
						
			public override void Visit(NamedTableReference node)
			{
				_references.Add(new Reference(
					Scripts.Table, 
					node.SchemaObject.BaseIdentifier.Value,
					node.SchemaObject.SchemaIdentifier?.Value ?? _defaultSchema,
					node.SchemaObject.DatabaseIdentifier?.Value ?? _defaultDatabase,
					node.SchemaObject.ServerIdentifier?.Value ?? _defaultServer));
				
				base.Visit(node);
			}
		}
}