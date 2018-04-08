using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using SqlAnalyser.Internal.Batches;
using SqlAnalyser.Internal.Identifiers;

namespace SqlAnalyser.Internal.Scripts
{
    public class ScriptInfo : IScriptInfo
    {
        internal IBatchFactory BatchFactory = new BatchFactory();
        
        public string Sql { get; }
        public SqlVersion Version { get; }
        public string DefaultDatabase { get; }
        public string DefaultServer { get; }
        public string DefaultSchema { get; }

        public ScriptInfo(string sql, SqlVersion version, string database, string server, string schema)
        {
            Sql = sql;
            Version = version;
            DefaultDatabase = database;
            DefaultServer = server;
            DefaultSchema = schema;
        }
        
        internal ScriptInfo(string sql, SqlVersion version, string database, string server, string schema, 
            params IBatchInfo[] batches) : this(sql, version, database, server, schema)
        {
            Batches = batches;
        }
        
        private IList<ParseError> _errors;
        public IEnumerable<ParseError> Errors
        {
            get
            {
                if (_errors == null)
                {
                    (Batches, Errors) = BatchFactory
                        .Generate(Sql, Version, DefaultSchema, DefaultDatabase, DefaultServer);
                }

                return _errors;
            }
            private set => _errors = value.ToList();
        }
        
        public bool Valid => !Errors.Any();

        private List<IBatchInfo> _batches;
        public IEnumerable<IBatchInfo> Batches
        {
            get
            {
                if (_batches == null)
                {
                    (Batches, Errors) = BatchFactory
                        .Generate(Sql, Version, DefaultSchema, DefaultDatabase, DefaultServer);
                }

                return _batches;
            }
            private set => _batches = value.ToList();
        }
        
        private List<IdentifierInfo> _doers;
        public IEnumerable<IdentifierInfo> Doers
        {
            get
            {
                if (_doers == null)
                {
                    _doers = Batches.SelectMany(x => x.Doers).Distinct().ToList();
                }

                return _doers;
            }
        }

        private List<IdentifierInfo> _references;
        public IEnumerable<IdentifierInfo> References
        {
            get
            {
                if (_references == null)
                {
                    _references = Batches.SelectMany(x => x.References).Distinct().ToList();
                }

                return _references;
            }
        }
    }
}