using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using RoseByte.SqlAnalyser.SqlServer.Internal.Batches;
using RoseByte.SqlAnalyser.SqlServer.Internal.Identifiers;

namespace RoseByte.SqlAnalyser.SqlServer.Internal.Visitors
{
    public class ReferenceVisitor : TSqlFragmentVisitor, IReferenceVisitor
    {
	    
	    private List<IdentifierInfo> _doers;
			
		private string _defaultSchema;
		private string _defaultDatabase;
		private string _defaultServer;
		
		private List<IdentifierInfo> _references;
		
	    public (IEnumerable<IdentifierInfo>, IEnumerable<IdentifierInfo>) GetReferences(TSqlBatch batch, 
		    string schema = null, string database = null, string server = null)
	    {
		    if (_doers == null)
		    {
			    _doers = new DoerVisitor().GetReferences(batch, schema, database, server).ToList();
		    }
		    
		    _defaultSchema = schema ?? string.Empty;
		    _defaultDatabase = database ?? string.Empty;
		    _defaultServer = server ?? string.Empty;
		    _references = new List<IdentifierInfo>();
			
		    ExplicitVisit(batch);

		    return (_references, _doers);
	    }
	    
	    public (IEnumerable<IdentifierInfo>, IEnumerable<IdentifierInfo>) GetReferences(TSqlBatch batch,
		    IEnumerable<IdentifierInfo> doers, string schema = null, string database = null, string server = null)
	    {
		    _doers = doers.ToList();
			
		    var doer = _doers.FirstOrDefault();
		    
		    return GetReferences(
			    batch, 
			    doer?.Schema.DefaultName ?? schema ?? string.Empty, 
			    doer?.Database.DefaultName ?? database ?? string.Empty, 
			    doer?.Server.DefaultName ?? server ?? string.Empty);
	    }
			
		
		public override void Visit(ProcedureReference node)
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

			if (!_doers.Contains(reference))
			{
				_references.Add(reference);
			}
			
			base.Visit(node);
		}

		public override void Visit(FunctionCall node)
		{
			string schema = null;
			string database = null;
			string server = null;

			if (node.CallTarget is MultiPartIdentifierCallTarget multiPartIdentifier)
			{
				var identifiers = multiPartIdentifier.MultiPartIdentifier.Identifiers;
				schema = identifiers.Count > 0 ? identifiers.Last().Value : null;
				database = identifiers.Count > 1 ? identifiers[identifiers.Count - 2].Value : null;
				server = identifiers.Count > 2 ? identifiers[identifiers.Count - 3].Value : null;
			}
			
			var reference = new IdentifierInfo(BatchTypes.Function, node.FunctionName.Value, schema, database, server);
			
			reference.Schema.DefaultName = _defaultSchema;
			reference.Database.DefaultName = _defaultDatabase;
			reference.Server.DefaultName = _defaultServer;
			
			_references.Add(reference);
			
			base.Visit(node);
		}
					
		public override void Visit(NamedTableReference node)
		{
			var reference = new IdentifierInfo(
				BatchTypes.Table, 
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