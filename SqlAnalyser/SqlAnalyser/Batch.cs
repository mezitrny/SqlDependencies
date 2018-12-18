using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.Management.SqlParser.Parser;
using RoseByte.SqlAnalyser.SqlServer.Internal.Batches;
using RoseByte.SqlAnalyser.SqlServer.Internal.Identifiers;

namespace RoseByte.SqlAnalyser.SqlServer
{
    public class Batch : IBatch
    {
		public string Sql { get; }

		public Batch(string sql) => Sql = sql;
		internal Batch(string sql, params IScript[] batches) : this(sql) => _batches = batches.ToList();
		public Batch(ParseResult sql) : this(sql.Script.Sql) => SetBatches(sql);
	    
        private IList<Error> _errors;
        public IEnumerable<Error> Errors
        {
            get
            {
                if (_errors == null)
                {
					LoadBatches();
				}
                return _errors;
            }
        }

	    private List<IScript> _batches;
        public IEnumerable<IScript> Batches
        {
	        get
	        {
		        if (_batches == null)
		        {
					LoadBatches();
				}
		        return _batches;
	        }
        }

	    private void SetBatches(ParseResult sql)
	    {
			_batches = sql.Script.Batches
				.Select((batch, order) => new Script(batch, order))
				.Cast<IScript>()
				.ToList();
		    _errors = sql.Errors.ToList();
	    }

	    private void LoadBatches() => SetBatches(Parser.Parse(Sql));
	    
	    private List<IdentifierInfo> _definitions;
        public IEnumerable<IdentifierInfo> Definitions => 
	        _definitions ?? (_definitions = Batches.SelectMany(x => x.Definitions).Distinct().ToList());

	    private List<IdentifierInfo> _references;
        public IEnumerable<IdentifierInfo> References => 
	        _references ?? (_references = Batches.SelectMany(x => x.References).Distinct().ToList());
    }
}