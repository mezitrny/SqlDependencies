using System.Collections.Generic;
using SqlAnalyser.Internal;

namespace SqlAnalyser
{
    public interface IDatabaseInfo
    {
        ScriptInfo AnalyseScript(string sql);
        ScriptInfo FetchScript(string name, BatchTypes type);
        IEnumerable<ScriptInfo> FetchScripts(BatchTypes type);
    }
}