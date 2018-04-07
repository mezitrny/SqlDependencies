using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using SqlAnalyser.Internal;
using SqlAnalyser.Internal.Identifiers;

namespace SqlAnalyser
{
    public class DatabaseInfo : IDatabaseInfo
    {
        public string ConnectionString { get; }
        private SqlConnection Connection { get; set; }

        public string DefaultSchema
        {
            get { return Connection.GetSchema().ToString(); }
        }

        public string DatabaseName { get; }
        public string ServerName { get; }
        public SqlVersion Version { get; }

        public DatabaseInfo(string connection)
        {
            var cb = new SqlConnectionStringBuilder(connection);
            DatabaseName = cb.InitialCatalog;
            ServerName = cb.DataSource;
        }
        
        public DatabaseInfo(SqlConnection connection)
        {
            ConnectionString = connection.ConnectionString;
        }
        
        public DatabaseInfo(SqlVersion version, string defaultSchema, string databaseName, string serverName)
        {
            DefaultSchema = defaultSchema;
            DatabaseName = databaseName;
            ServerName = serverName;
            Version = version;
        }
        
        public ScriptInfo AnalyseScript(string sql)
        {
            throw new System.NotImplementedException();
        }

        public ScriptInfo FetchScript(string name, BatchTypes type)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<ScriptInfo> FetchScripts(BatchTypes type)
        {
            throw new System.NotImplementedException();
        }
    }
}