using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using SqlAnalyser.Internal.Identifiers;
using SqlAnalyser.Internal.Visitors;

namespace SqlAnalyser.Internal.Batches
{
    public class BatchInfo : IBatchInfo
    {
        internal IDoerVisitor DoerVisitor = new DoerVisitor();
        internal IReferenceVisitor ReferenceVisitor = new ReferenceVisitor();
        
        public int Order { get; }
        
        private string _defaultDatabase;
        public string DefaultDatabase
        {
            get => _defaultDatabase;
            set
            {
                if (value != _defaultDatabase)
                {
                    if (_references != null)
                    {
                        foreach (var referenceInfo in _references)
                        {
                            referenceInfo.Database.DefaultName = value;
                        }
                    }

                    if (_doers != null)
                    {
                        foreach (var referenceInfo in _doers)
                        {
                            referenceInfo.Database.DefaultName = value;
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
                if (value != _defaultServer)
                {
                    if (_references != null)
                    {
                        foreach (var referenceInfo in _references)
                        {
                            referenceInfo.Server.DefaultName = value;
                        }
                    }

                    if (_doers != null)
                    {
                        foreach (var referenceInfo in _doers)
                        {
                            referenceInfo.Server.DefaultName = value;
                        }
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
                    if (_references != null)
                    {
                        foreach (var referenceInfo in _references)
                        {
                            referenceInfo.Schema.DefaultName = value;
                        }
                    }

                    if (_doers != null)
                    {
                        foreach (var referenceInfo in _doers)
                        {
                            referenceInfo.Schema.DefaultName = value;
                        }
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
                            .Take(Value.LastTokenIndex + 1 - Value.FirstTokenIndex)
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
                    _doers = DoerVisitor.GetReferences(Value, DefaultSchema, DefaultDatabase, DefaultServer)
                        .ToList();
                }

                return _doers;
            }
            private set => _doers = value.ToList();
        }

        private List<IdentifierInfo> _references;
        public IEnumerable<IdentifierInfo> References
        {
            get
            {
                if (_references == null)
                {
                    (var refs, var doers) = ReferenceVisitor
                        .GetReferences(Value, Doers, DefaultSchema, DefaultDatabase, DefaultServer);

                    References = refs;
                    
                    if (doers != null)
                    {
                        Doers = doers;
                    }
                }

                return _references;
            }
            private set => _references = value.ToList();
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
        
        public BatchInfo(TSqlBatch batch, int order, IEnumerable<IdentifierInfo> doers, IEnumerable<IdentifierInfo> references)
        {
            Order = order;
            Value = batch;
            Doers = doers;
            References = references;
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