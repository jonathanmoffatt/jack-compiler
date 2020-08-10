﻿using System.Collections.Generic;
using System.Xml.Linq;

namespace JackAnalyser
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

        public Token[] Tokens() => tokens.ToArray();

        public XDocument ToXml() => new XDocument(Tree.ToXml());

        public XDocument TokensXml()
        {
            var xml = new XDocument();
            XElement root = new XElement("tokens");
            xml.Add(root);
            foreach (Token token in tokens)
            {
                root.Add(token.ToXml());
            }
            return xml;
        }

    }
}
