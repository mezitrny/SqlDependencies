using System.Text;

namespace SqlAnalyser
{
    public class Reference
    {
        public Scripts Scripts { get; }
        public string Server { get; }
        public string Database { get; }
        public string Schema { get; }
        public string Name { get; }

        private string _identifier;
        public string Identifier
        {
            get
            {
                if (_identifier == null)
                {
                    var sb = new StringBuilder();

                    if (Server != null)
                    {
                        sb.Append(string.Concat(Server, "."));
                    }
				
                    if (Database != null)
                    {
                        sb.Append(string.Concat(Database, "."));
                    }
				
                    if (Schema != null)
                    {
                        sb.Append(string.Concat(Schema, "."));
                    }

                    sb.Append(Name);

                    _identifier = sb.ToString();
                }

                return _identifier;
            }
        }
        
        public Reference(Scripts type, string name, string schema, string database, string server)
        {
            Scripts = type;
            Server = server;
            Database = database;
            Schema = schema;
            Name = name;
        }
        
        public Reference(Scripts type, string name, string schema = null)
        : this(type, name, schema, null, null)
        { }

        public override string ToString() => $"{Scripts}: {Identifier}";
        
        public override int GetHashCode()
        {
            return (Scripts, Name, Schema, Database, Server).GetHashCode();
        }

        public override bool Equals(object obj) => Equals(obj as Reference);

        public bool Equals(Reference other)
        {
            return other != null
                   && Scripts == other.Scripts
                   && Name == other.Name
                   && Schema == other.Schema
                   && Database == other.Database
                   && Server == other.Server;
        }

        public static bool operator ==(Reference a, Reference b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (((object)a == null) || ((object)b == null)) return false;

            return a.Equals(b);
        }

        public static bool operator !=(Reference a, Reference b) => !(a == b);
    }
}