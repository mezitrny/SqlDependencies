using System;

namespace SqlAnalyser.Internal
{
    public class IdentifierInfo
    {
        public BatchTypes BatchTypes { get; }
        public string Name { get; }
        public Qualifier Server { get; }
        public Qualifier Database { get; }
        public Qualifier Schema { get; }
        
        public string Identifier => string.Join(".", Server.Name, Database.Name, Schema.Name, Name);
        public string ShortIdentifier => string.Join(".", Server.ShortName, Database.ShortName, Schema.ShortName, Name);
        public string FullIdentifier => string.Join(".", Server.FullName, Database.FullName, Schema.FullName, Name);
        
        public IdentifierInfo(BatchTypes type, string name, string schema, string database, string server)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new Exception($"Name must be a valid SQL identifier, not: {name}");
            }
            
            BatchTypes = type;
            Server = new Qualifier(server);
            Database = new Qualifier(database);
            Schema = new Qualifier(schema);
            Name = name;
        }
        
        public IdentifierInfo(BatchTypes type, string name, string schema = null)
        : this(type, name, schema, null, null)
        { }

        public override string ToString() => $"{BatchTypes}: {FullIdentifier}";
        
        public override int GetHashCode()
        {
            return (BatchTypes, Name, Schema, Database, Server).GetHashCode();
        }

        public override bool Equals(object obj) => Equals(obj as IdentifierInfo);

        public bool Equals(IdentifierInfo other)
        {
            return other != null
                   && BatchTypes == other.BatchTypes
                   && Name == other.Name
                   && Schema == other.Schema
                   && Database == other.Database
                   && Server == other.Server;
        }

        public static bool operator ==(IdentifierInfo a, IdentifierInfo b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (((object)a == null) || ((object)b == null)) return false;

            return a.Equals(b);
        }

        public static bool operator !=(IdentifierInfo a, IdentifierInfo b) => !(a == b);
    }
}