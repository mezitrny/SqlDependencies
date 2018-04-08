using Microsoft.SqlServer.Management.Smo;

namespace SqlAnalyser
{
    public class ServerInfo
    {
        private Scripter _scripter;
        private Scripter Scripter => _scripter ?? (_scripter = new Scripter(Server));
        private Server Server { get; }

        public ServerInfo(string server)
        {
            Server = new Server(server);
        }

        DatabaseInfo AnalyzeDatabase(string Database)
        {
            
        }
    }
}