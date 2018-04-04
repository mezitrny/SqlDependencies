using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace SqlAnalyser.Internal
{
    public class ReferenceScanner : TSqlFragmentVisitor
    {
	    private List<IdentifierInfo> _doers;
			
		public ReferenceScanner(string schema = null, string database = null, string server = null)
		{
			_defaultSchema = schema ?? string.Empty;
			_defaultDatabase = database ?? string.Empty;
			_defaultServer = server ?? string.Empty;
		}
		
		public ReferenceScanner(IEnumerable<IdentifierInfo> doers)
		{
			_doers = doers.ToList();
			
			var doer = _doers.FirstOrDefault();
			_defaultSchema = doer?.Schema.DefaultName ?? string.Empty;
			_defaultDatabase = doer?.Database.DefaultName ?? string.Empty;
			_defaultServer = doer?.Server.DefaultName ?? string.Empty;
		}
		
		private readonly string _defaultSchema;
		private readonly string _defaultDatabase;
		private readonly string _defaultServer;
		
		private List<IdentifierInfo> _references;
		
	    public IEnumerable<IdentifierInfo> GetReferences(TSqlBatch batch, out IEnumerable<IdentifierInfo> doers)
	    {
		    if (_doers == null)
		    {
			    _doers = new NameVisitor().GetReferences(batch).ToList();
		    }

		    doers = _doers;
		    
		    _references = new List<IdentifierInfo>();
			
		    ExplicitVisit(batch);

		    return _references;
	    }
	    
	    public IEnumerable<IdentifierInfo> GetReferences(TSqlBatch batch, IEnumerable<IdentifierInfo> doers)
	    {
		    _doers = doers.ToList();

		    return GetReferences(batch);
	    }
	    
		public IEnumerable<IdentifierInfo> GetReferences(TSqlBatch batch) => GetReferences(batch, out var nothing);
		
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
			_references.Add(new IdentifierInfo(
				BatchTypes.Function, 
				node.FunctionName.Value));
			
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