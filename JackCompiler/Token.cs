namespace JackCompiler
{
    public class Token : NodeBase
    {
        public string Value { get; private set; }

        public Token(NodeType type, string tokenValue)
        {
            Type = type;
            Value = tokenValue;
        }

        public override string ToString()
        {
            return $"{Type} '{Value}'";
        }
    }
}
