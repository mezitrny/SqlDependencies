using System.Collections.Generic;
using SqlAnalyser.Internal;
using SqlAnalyser.Internal.Identifiers;

namespace SqlAnalyser
{
    public class DatabaseInfo
    {
        public IEnumerable<ScriptInfo> Fetch(IdentifierInfo identifier)
        {
            return new ScriptInfo[] { };
        }
        
        public IEnumerable<ScriptInfo> Fetch(BatchTypes type)
        {
            return new ScriptInfo[] { };
        }
        
        public IEnumerable<ScriptInfo> FetchDoers(ScriptInfo script)
        {
            return new ScriptInfo[] { };
        }
        
        public IEnumerable<ScriptInfo> FetchDependencies(ScriptInfo script)
        {
            return new ScriptInfo[] { };
        }
    }
}