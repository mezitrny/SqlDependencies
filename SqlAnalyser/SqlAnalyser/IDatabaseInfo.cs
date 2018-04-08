using System.Collections.Generic;
using SqlAnalyser.Internal;
using SqlAnalyser.Internal.Identifiers;
using SqlAnalyser.Internal.Scripts;

namespace SqlAnalyser
{
    public interface IDatabaseInfo
    {
        ScriptInfo AnalyseScript(string sql);
        ScriptInfo FetchScript(IdentifierInfo identifier);
    }
}