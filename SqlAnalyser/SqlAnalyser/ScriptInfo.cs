using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using SqlAnalyser.Internal;
using SqlAnalyser.Internal.Batches;

namespace SqlAnalyser
{
    public class ScriptInfo
    {
        internal IBatchFactory BatchFactory = new BatchFactory();
        
        public string Sql { get; }
        public SqlVersion Version { get; }

        private string _defaultDatabase;
        public string DefaultDatabase
        {
            get => _defaultDatabase;
            set
            {
                if (value != _defaultDatabase)
                {
                    if (_batches != null)
                    {
                        foreach (var batchInfo in _batches)
                        {
                            batchInfo.DefaultDatabase = value;
                        }
                    }
                    
                    _defaultDatabase = value;
                }
            }
        }
        
        private string _defaultServer;
        public string DefaultServer
        {
            get => _defaultServer;
            set
            {
                if (_batches != null)
                {
                    foreach (var batchInfo in _batches)
                    {
                        batchInfo.DefaultServer = value;
                    }
                }

                _defaultServer = value;
            }
        }
        
        private string _defaultSchema;
        public string DefaultSchema
        {
            get => _defaultSchema;
            set
            {
                if (_batches != null)
                {
                    foreach (var batchInfo in _batches)
                    {
                        batchInfo.DefaultSchema = value;
                    }
                }

                _defaultSchema = value;
            }
        }

        public ScriptInfo(string sql, SqlVersion version)
        {
            Sql = sql;
            Version = version;
        }
        
        public ScriptInfo(string sql, SqlVersion version, string database, string server, string schema)
        : this(sql, version)
        {
            DefaultDatabase = database;
            DefaultServer = server;
            DefaultSchema = schema;
        }
        
        public ScriptInfo(string sql, SqlVersion version, params IBatchInfo[] batches)
            : this(sql, version)
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