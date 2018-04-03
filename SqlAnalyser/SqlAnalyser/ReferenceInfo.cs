using System;
using System.Text;

namespace SqlAnalyser
{
    public class ReferenceInfo
    {
        public Scripts Scripts { get; }
        public string Name { get; }
        public Qualifier Server { get; }
        public Qualifier Database { get; }
        public Qualifier Schema { get; }
        
        public string Identifier => string.Join(".", Server.Name, Database.Name, Schema.Name, Name);
        public string ShortIdentifier => string.Join(".", Server.ShortName, Database.ShortName, Schema.ShortName, Name);
        public string FullIdentifier => string.Join(".", Server.FullName, Database.FullName, Schema.FullName, Name);
        
        public ReferenceInfo(Scripts type, string name, string schema, string database, string server)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new Exception($"Name must be a valid SQL identifier, not: {name}");
            }
            
            Scripts = type;
            Server = new Qualifier(server);
            Database = new Qualifier(database);
            Schema = new Qualifier(schema);
            Name = name;
        }
        
        public ReferenceInfo(Scripts type, string name, string schema = null)
        : this(type, name, schema, null, null)
        { }

        public override string ToString() => $"{Scripts}: {FullIdentifier}";
        
        public override int GetHashCode()
        {
            return (Scripts, Name, Schema, Database, Server).GetHashCode();
        }

        public override bool Equals(object obj) => Equals(obj as ReferenceInfo);

        public bool Equals(ReferenceInfo other)
        {
            return other != null
                   && Scripts == other.Scripts
                   && Name == other.Name
                   && Schema == other.Schema
                   && Database == other.Database
                   && Server == other.Server;
        }

        public static bool operator ==(ReferenceInfo a, ReferenceInfo b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (((object)a == null) || ((object)b == null)) return false;

            return a.Equals(b);
        }

        public static bool operator !=(ReferenceInfo a, ReferenceInfo b) => !(a == b);
    }
}