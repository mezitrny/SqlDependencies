using System.Collections.Generic;
using Microsoft.SqlServer.Management.SqlParser.Parser;
using RoseByte.SqlAnalyser.SqlServer.Internal.Batches;
using RoseByte.SqlAnalyser.SqlServer.Internal.Identifiers;

namespace RoseByte.SqlAnalyser.SqlServer
{
    public interface IBatch
    {
        string Sql { get; }
        IEnumerable<Error> Errors { get; }
        IEnumerable<IScript> Batches { get; }
        IEnumerable<IdentifierInfo> Definitions { get; }
        IEnumerable<IdentifierInfo> References { get; }
    }
}