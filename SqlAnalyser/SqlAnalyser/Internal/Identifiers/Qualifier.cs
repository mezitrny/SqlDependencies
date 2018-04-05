namespace SqlAnalyser.Internal.Identifiers
{
    public class Qualifier
    {
        public string Name { get; }
        public string DefaultName { get; set; }
        public QualifierTypes Type { get; }

        public Qualifier(string name, QualifierTypes type)
        {
            Name = name;
            Type = type;
        }

        public string FullName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Name))
                {
                    return Name;
                }

                if (!string.IsNullOrWhiteSpace(DefaultName))
                {
                    return DefaultName;
                }

                return string.Empty;
            }
        }
        
        public string ShortName
        {
            get
            {
                if (Name == DefaultName)
                {
                    return string.Empty;
                }

                return FullName;
            }
        }
        
        public override int GetHashCode()
        {
            return string.Concat(Type, Name ?? string.Empty).GetHashCode();
        }

        public override bool Equals(object obj) => Equals(obj as IdentifierInfo);

        public bool Equals(Qualifier other)
        {
            if (other == null)
            {
                return false;
            }

            return Type == other.Type && FullName == other.FullName;
        }

        public static bool operator ==(Qualifier left, Qualifier right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (((object)left == null) || ((object)right == null)) return false;

            return left.Equals(right);
        }

        public static bool operator !=(Qualifier left, Qualifier right) => !(left == right);
    }
}