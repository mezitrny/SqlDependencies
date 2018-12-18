using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.Management.SqlParser.SqlCodeDom;
using RoseByte.SqlAnalyser.SqlServer.Internal.Identifiers;

namespace RoseByte.SqlAnalyser.SqlServer.Internal.Batches
{
    public class Script : IScript
    {
        public int Order { get; }
		public SqlBatch Value { get; }
		public string Sql => Value.Sql;
        
        private List<IdentifierInfo> _definitions;
        public IEnumerable<IdentifierInfo> Definitions
        {
	        get
	        {
		        if (_definitions == null)
		        {
			        var scanner = new DefinitionsScanner();
			        _definitions = scanner.GetDefinitions(Value).Distinct().ToList();
		        }
		        return _definitions;
	        }
        }

		private List<IdentifierInfo> _references;
	    public IEnumerable<IdentifierInfo> References
	    {
		    get
		    {
			    if (_references == null)
			    {
				    var scanner = new ReferencesScanner();
					_references = scanner.GetReferences(Value).Distinct().ToList();
				}
			    return _references;
		    }
	    }

        public Script(SqlBatch batch, int order)
        {
            Order = order;
            Value = batch;
        }
    }
}