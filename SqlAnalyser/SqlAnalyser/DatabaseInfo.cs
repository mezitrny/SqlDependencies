using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using RoseByte.AdoSession;
using SqlAnalyser.Internal;
using SqlAnalyser.Internal.Identifiers;

namespace SqlAnalyser
{
    public class DatabaseInfo : IDatabaseInfo
    {
        public string ConnectionString { get; }
        private Session Session { get; }

        private string _defaultSchema;
        public string DefaultSchema
        {
            get
            {
                if (_defaultSchema == null)
                {
                    _defaultSchema = Session.GetScalar<string>("SELECT SCHEMA_NAME()"); 
                    Session.CloseConnection();
                }

                return _defaultSchema;
            }
        }

        public string DatabaseName { get; }
        public string ServerName { get; }

        private SqlVersion? _version;
        public SqlVersion Version
        {
            get
            {
                if (!_version.HasValue)
                {
                    var version = new SqlConnection(ConnectionString).ServerVersion?
                        .Split('.').FirstOrDefault();

                    switch (version)
                    {
                        case "8":
                            _version = SqlVersion.Sql80;
                            break;
                        case "9":
                            _version = SqlVersion.Sql90;
                            break;
                        case "10":
                            _version = SqlVersion.Sql100;
                            break;
                        case "11":
                            _version = SqlVersion.Sql110;
                            break;
                        case "12":
                            _version = SqlVersion.Sql120;
                            break;
                        case "13":
                            _version = SqlVersion.Sql130;
                            break;
                        case "14":
                            _version = SqlVersion.Sql140;
                            break;
                        default:
                            _version = SqlVersion.Sql140;
                            break;
                    }
                }

                return _version.Value;
            }
        }

        public DatabaseInfo(string connection)
        {
            Session = new Session(connection);
            var cb = new SqlConnectionStringBuilder(connection);
            DatabaseName = cb.InitialCatalog;
            ServerName = cb.DataSource;
        }
        
        public DatabaseInfo(IDbConnection connection)
        {
            ConnectionString = connection.ConnectionString;
        }
        
        public ScriptInfo AnalyseScript(string sql)
        {
            return new ScriptInfo(sql, Version, DatabaseName, ServerName, DefaultSchema);
        }

        public ScriptInfo FetchScript(string name)
        {
            return AnalyseScript(Session.GetScalar<string>("OBJECT_DEFINITION(OBJECT_ID('TestProc1'))"));
        }

        public IEnumerable<ScriptInfo> FetchScripts(BatchTypes type)
        {
            throw new System.NotImplementedException();
        }
    }
}