using System.Data.SqlClient;

namespace SqlAnalyser.Internal.Helpers
{
    public static class SqlConnectionStringBuilderExtensions
    {
        public static (string, string) GetServerDatabase(this SqlConnectionStringBuilder builder)
        {
            return (builder.DataSource, builder.InitialCatalog);
        }
    }
}