using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using RoseByte.AdoSession.Interfaces;

namespace RoseByte.SqlAnalyser.SqlServer.Internal.Helpers
{
    public static class SessionExtensions
    {
        public static SqlVersion GetVersion(this ISession session)
        {
            if (!int.TryParse(session.GetScalar<string>("SELECT @@VERSION")?
                .Split('.').FirstOrDefault(), out var version))
            {
                return SqlVersion.Sql140;
            }
            
            return (SqlVersion)(version - 8);
        }
    }
}