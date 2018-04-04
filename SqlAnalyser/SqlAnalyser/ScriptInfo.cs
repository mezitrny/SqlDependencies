using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using SqlAnalyser.Internal;

namespace SqlAnalyser
{
    public class ScriptInfo
    {
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
                    foreach (var batchInfo in _batches)
                    {
                        batchInfo.DefaultDatabase = value;
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
                foreach (var batchInfo in _batches)
                {
                    batchInfo.DefaultServer = value;
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
                foreach (var batchInfo in _batches)
                {
                    batchInfo.DefaultSchema = value;
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
        
        private IList<ParseError> _errors;
        public IEnumerable<ParseError> Errors
        {
            get
            {
                if (_errors == null)
                {
                    var batches = SqlParser.Parse(Sql, Version, out _errors);

                    if (_errors.Any())
                    {
                        return _errors;
                    }
                    
                    _batches = batches
                        .Select((batch, order) => new BatchInfo(batch, order, DefaultSchema, DefaultDatabase, DefaultServer))
                        .ToList();
                }

                return _errors;
            }
        }
        
        public bool Valid => !Errors.Any();

        private List<BatchInfo> _batches;
        public IEnumerable<BatchInfo> Batches
        {
            get
            {
                if (_batches == null)
                {
                    _batches = SqlParser.Parse(Sql, Version, out _errors)
                        .Select((batch, i) => new BatchInfo(batch, i, DefaultSchema, DefaultDatabase, DefaultServer))
                        .ToList();
                }

                return _batches;
            }
        }
        
        private List<IdentifierInfo> _doers;
        public IEnumerable<IdentifierInfo> Doers
        {
            get
            {
                if (_doers == null)
                {
                    _doers = new List<IdentifierInfo>();

                    foreach (var batch in Batches)
                    {
                         _doers.AddRange(batch.Doers);
                    }
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
                    _references = new List<IdentifierInfo>();

                    foreach (var batch in Batches)
                    {
                        _references.AddRange(batch.References);
                    }
                }

                return _references;
            }
        }

        private BatchTypes? _batchType;
        public BatchTypes BatchType
        {
            get
            {
                if (!_batchType.HasValue)
                {
                    var types = Doers.Distinct().Select(x => x.BatchTypes).ToList();

                    _batchType = types.Count == 1 ? types.First() : Internal.BatchTypes.Other;
                }

                return _batchType.Value;
            }
        }
    }
}