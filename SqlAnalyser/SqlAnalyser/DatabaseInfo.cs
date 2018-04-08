using System.Data.SqlClient;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using RoseByte.AdoSession;
using SqlAnalyser.Internal.Helpers;
using SqlAnalyser.Internal.Identifiers;
using SqlAnalyser.Internal.Scripts;

namespace SqlAnalyser
{
    public class DatabaseInfo : IDatabaseInfo
    {
        private Session Session { get; }
        public string DefaultSchema { get; }
        public string DatabaseName { get; }
        public string ServerName { get; }
        public SqlVersion Version { get; }

        public DatabaseInfo(string connection, SqlVersion? version = null, string defaultSchema = null)
        {
            (ServerName, DatabaseName) = new SqlConnectionStringBuilder(connection).GetServerDatabase();
            Session = new Session(connection);
            Version = version ?? Session.GetVersion();
            DefaultSchema = defaultSchema ?? Session.GetScalar<string>("SELECT SCHEMA_NAME()");
        }

        public ScriptInfo AnalyseScript(string sql)
        {
            return new ScriptInfo(sql, Version, DatabaseName, ServerName, DefaultSchema);
        }

        public ScriptInfo FetchScript(IdentifierInfo identifier)
        {
            var script = Session.GetScalar<string>(
                "OBJECT_DEFINITION(OBJECT_ID('@Id'))",
                new ParameterSet() {new Parameter<int>("Id", identifier.ShortIdentifier)});
            
            return AnalyseScript(script);
        }
    }
}