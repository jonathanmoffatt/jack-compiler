namespace JackCompiler
{
    public class Identifier : Token
    {
        public IdentifierKind Kind { get; private set; }
        public bool IsDefinition { get; private set; }
        public int? Number { get; private set; }

        public Identifier(string identifier, IdentifierKind kind, bool isDefinition, int? number = null): base(NodeType.Identifier, identifier)
        {
            Kind = kind;
            IsDefinition = isDefinition;
            Number = number;
        }
    }
}
