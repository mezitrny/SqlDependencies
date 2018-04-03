using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace SqlAnalyser
{
    public class BatchInfo
    {
        private string _defaultDatabase;
        public string DefaultDatabase
        {
            get => _defaultDatabase;
            set
            {
                if (value != _defaultDatabase)
                {
                    foreach (var referenceInfo in _references)
                    {
                        referenceInfo.Database.DefaultName = value;
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
                if (value != _defaultServer)
                {
                    foreach (var referenceInfo in _references)
                    {
                        referenceInfo.Server.DefaultName = value;
                    }
                    
                    _defaultServer = value;
                }
            }
        }
        
        private string _defaultSchema;
        public string DefaultSchema
        {
            get => _defaultSchema;
            set
            {
                if (value != _defaultSchema)
                {
                    foreach (var referenceInfo in _references)
                    {
                        referenceInfo.Schema.DefaultName = value;
                    }
                    
                    _defaultSchema = value;
                }
            }
        }
        
        private string _sql;
        public string Sql
        {
            get
            {
                if (_sql == null)
                {
                    _sql = string.Join(
                        string.Empty, 
                        Value.ScriptTokenStream
                            .Skip(Value.FirstTokenIndex)
                            .Take(Value.FragmentLength)
                            .Select(x => x.Text));
                }

                return _sql;
            }
        }

        public ScriptInfo Script { get; }
        
        public int Order { get; }

        private List<ReferenceInfo> _references;

        public IEnumerable<ReferenceInfo> References
        {
            get
            {
                if (_references == null)
                {
                    var scanner = new ReferenceScanner(
                        Script?.DefaultSchema,
                        Script?.DefaultDatabase,
                        Script?.DefaultServer);
                    
                    _references = scanner.GetReferences(Value).ToList();
                }

                return _references;
            }
            
        }

        private Scripts? _batchType;
        public Scripts BatchType
        {
            get
            {
                if (!_batchType.HasValue)
                {
                    _batchType = new TypeScanner().ScanScriptType(Value);
                }

                return _batchType.Value;
            }
        }

        public TSqlBatch Value { get; }

        public BatchInfo(TSqlBatch batch, int order)
        {
            Order = order;
            Value = batch;
        }
        
        public BatchInfo(TSqlBatch batch, int order, string schema, string database, string server)
        :this(batch, order)
        {
            DefaultSchema = schema;
            DefaultDatabase = database;
            DefaultServer = server;
        }
        
    }
}