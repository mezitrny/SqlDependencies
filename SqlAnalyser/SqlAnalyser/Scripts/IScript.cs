using System.Collections.Generic;
using Microsoft.SqlServer.Management.SqlParser.SqlCodeDom;
using RoseByte.SqlAnalyser.SqlServer.Internal.Identifiers;

namespace RoseByte.SqlAnalyser.SqlServer.Internal.Batches
{
    public interface IScript
    {
        int Order { get; }
        string Sql { get; }
        IEnumerable<IdentifierInfo> Definitions { get; }
        IEnumerable<IdentifierInfo> References { get; }
        SqlBatch Value { get; }
	}
}