using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using RoseByte.SqlAnalyser.SqlServer.Internal.Batches;
using RoseByte.SqlAnalyser.SqlServer.Internal.Identifiers;

namespace RoseByte.SqlAnalyser.SqlServer.Internal.Scripts
{
    public class ScriptInfo : IScriptInfo
    {
        private static readonly List<BatchTypes> OtherTypes = new List<BatchTypes>
        {
            BatchTypes.Empty, BatchTypes.Other
        };
        
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

        private BatchTypes? _type;
        public BatchTypes Type
        {
            get
            {
                if (!_type.HasValue)
                {
                    _type = Batches.Count() == 1 
                        ? Batches.First().BatchType 
                        : Batches.Any() ? BatchTypes.Other : BatchTypes.Empty;
                }

                return _type.Value;
            }
        }

        private IdentifierInfo _identifier;
        public IdentifierInfo Identifier
        {
            get
            {
                if (_identifier == null)
                {
                    if (Batches.Count() != 1)
                    {
                        return null;
                    }
                    if (Batches.First().Doers.Count() == 1)
                    {
                        _identifier = Batches.First().Doers.First();
                    }
                    else if (OtherTypes.Contains(Batches.First().BatchType))
                    {
                        return null;
                    }
                    else if (Batches.Count(x => !OtherTypes.Contains(x.BatchType)) == 1)
                    {
                        _identifier = Batches.First().Doers.First(x => !OtherTypes.Contains(x.BatchTypes));
                    }
                }

                return _identifier;
            }
        } 
        
        private List<IdentifierInfo> _doers;
        public IEnumerable<IdentifierInfo> Doers
        {
            get { return _doers ?? (_doers = Batches.SelectMany(x => x.Doers).Distinct().ToList()); }
        }

        private List<IdentifierInfo> _references;
        public IEnumerable<IdentifierInfo> References
        {
            get { return _references ?? (_references = Batches.SelectMany(x => x.References).Distinct().ToList()); }
        }
    }
}