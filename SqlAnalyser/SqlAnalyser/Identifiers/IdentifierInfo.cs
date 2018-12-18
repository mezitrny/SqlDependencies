using System;

namespace RoseByte.SqlAnalyser.SqlServer.Internal.Identifiers
{
    public class IdentifierInfo
    {
        public IdentifierTypes Type { get; }
        public string ObjectName { get; }
        public string ServerName { get; }
        public string DatabaseName { get; }
        public string SchemaName { get; }
        public string Name => string.Join(".", ServerName, DatabaseName, SchemaName, ObjectName).Trim('.');
        
        public IdentifierInfo(IdentifierTypes type, string objectName, string schema, string database, string server)
        {
            if (string.IsNullOrWhiteSpace(objectName))
            {
                throw new Exception($"ObjectName must be a valid SQL identifier, not: {objectName}");
            }
            
            Type = type;
            ServerName = server;
            DatabaseName = database;
            SchemaName = schema;
            ObjectName = objectName;
        }
        
        public override string ToString() => $"{Type}: {Name}";
        
        public override int GetHashCode()
        {
            return (Type, ObjectName, SchemaName, DatabaseName, ServerName).GetHashCode();
        }

        public override bool Equals(object obj) => Equals(obj as IdentifierInfo);

        public bool Equals(IdentifierInfo other)
        {
            return other != null
                   && Type == other.Type
                   && ObjectName == other.ObjectName
                   && SchemaName == other.SchemaName
                   && DatabaseName == other.DatabaseName
                   && ServerName == other.ServerName;
        }

        public static bool operator ==(IdentifierInfo a, IdentifierInfo b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a is null || b is null) return false;
            return a.Equals(b);
        }

        public static bool operator !=(IdentifierInfo a, IdentifierInfo b) => !(a == b);
    }
}