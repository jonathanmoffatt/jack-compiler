using System.Collections.Generic;

namespace JackCompiler
{
    public class Parser
    {
        private List<Token> tokens;
        private readonly IGrammarian grammarian;

        public Node Tree { get; set; }

        public Parser(IGrammarian grammarian)
        {
            this.grammarian = grammarian;
        }

        public void Parse(ITokeniser tokeniser)
        {
            tokens = new List<Token>();
            var queue = new Queue<Token>();
            Token token = tokeniser.GetNextToken();
            while (token != null)
            {
                tokens.Add(token);
                queue.Enqueue(token);
                token = tokeniser.GetNextToken();
            }
            grammarian.LoadTokens(queue);
            Tree = grammarian.ParseClass();
        }

        public Token[] Tokens => tokens.ToArray();

    }
}
