﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using RoseByte.SqlAnalyser.SqlServer.Internal.Identifiers;
using RoseByte.SqlAnalyser.SqlServer.Internal.Visitors;

namespace RoseByte.SqlAnalyser.SqlServer.Internal.Batches
{
    public class BatchInfo : IBatchInfo
    {
        private static List<BatchTypes> RoutineTypes = new List<BatchTypes>
        {
            BatchTypes.Function, BatchTypes.Procedure, BatchTypes.View
        };
        internal IDoerVisitor DoerVisitor { private get; set; } = new DoerVisitor();
        internal IReferenceVisitor ReferenceVisitor { private get; set; } = new ReferenceVisitor();
        
        public int Order { get; }
        public string DefaultDatabase { get; }
        public string DefaultServer { get; }
        public string DefaultSchema { get; }
        
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

                    if (types.Count == 1)
                    {
                        _batchType = types.First();
                    }
                    else if (types.Count(x => RoutineTypes.Contains(x)) == 1)
                    {
                        _batchType = types.First(x => RoutineTypes.Contains(x));
                    }
                    else
                    {
                        _batchType = BatchTypes.Other;
                    }
                }

                return _batchType.Value;
            }
        }

        public TSqlBatch Value { get; }

        internal BatchInfo(TSqlBatch batch, int order, IEnumerable<IdentifierInfo> doers, 
            IEnumerable<IdentifierInfo> references)
        {
            Order = order;
            Value = batch;
            Doers = doers;
            References = references;
        }
        
        public BatchInfo(TSqlBatch batch, int order, string schema, string database, string server)
        {
            Order = order;
            Value = batch;
            DefaultSchema = schema;
            DefaultDatabase = database;
            DefaultServer = server;
        }
        
    }
}