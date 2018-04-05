using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace SqlAnalyser.Internal.Batches
{
    public class BatchInfo : IBatchInfo
    {
        public int Order { get; }
        
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
                    
                    foreach (var referenceInfo in _doers)
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
                    
                    foreach (var referenceInfo in _doers)
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
                    
                    foreach (var referenceInfo in _doers)
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

        private List<IdentifierInfo> _doers;
        public IEnumerable<IdentifierInfo> Doers
        {
            get
            {
                if (_doers == null)
                {
                    _doers = new DoerVisitor(DefaultSchema, DefaultDatabase, DefaultServer).GetReferences(Value)
                        .ToList();
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
                    _references = new ReferenceVisitor(DefaultSchema, DefaultDatabase, DefaultServer)
                        .GetReferences(Value, Doers).ToList();
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

                    _batchType = types.Count == 1 ? types.First() : BatchTypes.Other;
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