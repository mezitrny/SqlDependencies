using RoseByte.SqlAnalyser.SqlServer.Internal.Identifiers;
using RoseByte.SqlAnalyser.SqlServer.Internal.Scripts;

namespace RoseByte.SqlAnalyser.SqlServer
{
    public interface IDatabaseInfo
    {
        IScriptInfo AnalyseScript(string sql);
        IScriptInfo FetchScript(IdentifierInfo identifier);
    }
}