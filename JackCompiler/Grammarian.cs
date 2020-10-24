using System;
using System.Collections.Generic;

namespace JackCompiler
{
    public class Grammarian : IGrammarian
    {
        private Queue<Token> tokens;

        public IGrammarian LoadTokens(params Token[] tokens)
        {
            this.tokens = new Queue<Token>(tokens);
            return this;
        }

        public IGrammarian LoadTokens(Queue<Token> tokens)
        {
            this.tokens = tokens;
            return this;
        }

        public Node ParseClass()
        {
            Node root = new Node(NodeType.Class);
            root.AddChild(Dequeue());
            DequeueIdentifier(root, "class expected a className identifier");
            DequeueSymbol(root, "{");
            while (Peek() == "static" || Peek() == "field")
            {
                root.AddChild(ParseClassVariableDeclaration());
            }
            while (Peek() == "constructor" || Peek() == "function" || Peek() == "method")
            {
                root.AddChild(ParseSubroutineDeclaration());
            }
            DequeueSymbol(root, "}");
            return root;
        }

        public Node ParseClassVariableDeclaration()
        {
            var cvd = new Node(NodeType.ClassVariableDeclaration);
            DequeueKeyword(cvd);
            DequeueType(cvd);
            bool another;
            do
            {
                DequeueIdentifier(cvd, "class variable declaration expected a variable name");
                another = Peek() == ",";
                if (another) cvd.AddChild(tokens.Dequeue());
            } while (another);
            DequeueSymbol(cvd, ";");
            return cvd;
        }

        public Node ParseSubroutineDeclaration()
        {
            var sd = new Node(NodeType.SubroutineDeclaration);
            DequeueKeyword(sd);
            DequeueType(sd);
            DequeueIdentifier(sd, "expected subroutine name");
            DequeueSymbol(sd, "(");
            sd.AddChild(ParseParameterList());
            DequeueSymbol(sd, ")");
            sd.AddChild(ParseSubroutineBody());
            return sd;
        }

        public Node ParseLetStatement()
        {
            if (Peek() != "let") return null;
            var statement = new Node(NodeType.LetStatement);
            DequeueKeyword(statement);
            DequeueIdentifier(statement, "let statement expected an identifier");
            DequeueSymbol(statement, "=");
            statement.AddChild(ParseExpression());
            DequeueSymbol(statement, ";");
            return statement;
        }

        public Node ParseIfStatement()
        {
            if (Peek() != "if") return null;
            var statement = new Node(NodeType.IfStatement);
            DequeueKeyword(statement);
            DequeueSymbol(statement, "(");
            statement.AddChild(ParseExpression());
            DequeueSymbol(statement, ")");
            DequeueSymbol(statement, "{");
            statement.AddChild(ParseStatements());
            DequeueSymbol(statement, "}");
            if (Peek() == "else")
            {
                DequeueKeyword(statement);
                DequeueSymbol(statement, "{");
                statement.AddChild(ParseStatements());
                DequeueSymbol(statement, "}");
            }
            return statement;
        }

        public Node ParseReturnStatement()
        {
            if (Peek() != "return") return null;
            var statement = new Node(NodeType.ReturnStatement);
            DequeueKeyword(statement);
            if (Peek() != ";")
                statement.AddChild(ParseExpression());
            DequeueSymbol(statement, ";");
            return statement;
        }

        public Node ParseWhileStatement()
        {
            if (Peek() != "while") return null;
            var statement = new Node(NodeType.WhileStatement);
            DequeueKeyword(statement);
            DequeueSymbol(statement, "(");
            statement.AddChild(ParseExpression());
            DequeueSymbol(statement, ")");
            DequeueSymbol(statement, "{");
            statement.AddChild(ParseStatements());
            DequeueSymbol(statement, "}");
            return statement;
        }

        public Node ParseDoStatement()
        {
            if (Peek() != "do") return null;
            var statement = new Node(NodeType.DoStatement);
            DequeueKeyword(statement);
            DequeueIdentifier(statement, "expected identifier after do statement");
            if (Peek() == ".")
            {
                DequeueSymbol(statement, ".");
                DequeueIdentifier(statement, "expected identifier after '.' in do statement");
            }
            DequeueSymbol(statement, "(");
            statement.AddChild(ParseExpressionList());
            DequeueSymbol(statement, ")");
            DequeueSymbol(statement, ";");
            return statement;
        }

        public Node ParseParameterList()
        {
            var pl = new Node(NodeType.ParameterList);
            bool another = Peek() != ")";
            while (another)
            {
                DequeueType(pl);
                DequeueIdentifier(pl, "expected parameter list identifier");
                another = Peek() == ",";
                if (another) DequeueSymbol(pl, ",");
            };
            return pl;
        }

        public Node ParseSubroutineBody()
        {
            var body = new Node(NodeType.SubroutineBody);
            DequeueSymbol(body, "{");
            body.AddChildren(ParseVariableDeclarations());
            body.AddChild(ParseStatements());
            DequeueSymbol(body, "}");
            return body;
        }

        public Node[] ParseVariableDeclarations()
        {
            var variableDeclarations = new List<Node>();
            while (Peek() == "var")
            {
                var variables = new Node(NodeType.VariableDeclaration);
                DequeueKeyword(variables);
                DequeueType(variables);
                bool more;
                do
                {
                    DequeueIdentifier(variables, "variable declarations expected an identifier");
                    more = Peek() == ",";
                    if (more) DequeueSymbol(variables, ",");
                } while (more);
                DequeueSymbol(variables, ";");
                variableDeclarations.Add(variables);
            }
            return variableDeclarations.ToArray();
        }

        public Node ParseStatements()
        {
            if (Peek() == "}") return null;
            var statements = new Node(NodeType.Statements);
            while (Peek() != "}")
            {
                statements.AddChild(ParseLetStatement());
                statements.AddChild(ParseIfStatement());
                statements.AddChild(ParseReturnStatement());
                statements.AddChild(ParseWhileStatement());
                statements.AddChild(ParseDoStatement());
            }
            return statements;
        }

        public Node ParseExpression()
        {
            var expression = new Node(NodeType.Expression);
            expression.AddChild(ParseTerm());
            while (IsOperator(Peek()))
            {
                DequeueSymbol(expression, Peek());
                expression.AddChild(ParseTerm());
            }
            return expression;
        }

        public Node ParseTerm()
        {
            var term = new Node(NodeType.Term);
            if (IsUnaryOperator(Peek()))
            {
                DequeueSymbol(term, Peek());
                term.AddChild(ParseTerm());
            }
            else if (Peek() == "(")
            {
                DequeueSymbol(term, "(");
                term.AddChild(ParseExpression());
                DequeueSymbol(term, ")");
            }
            else
            {
                term.AddChild(Dequeue());
                if (Peek() == "[")
                {
                    DequeueSymbol(term, "[");
                    term.AddChild(ParseExpression());
                    DequeueSymbol(term, "]");
                }
                if (Peek() == ".")
                {
                    DequeueSymbol(term, ".");
                    DequeueIdentifier(term, "expected subroutineName");
                }
                if (Peek() == "(")
                {
                    DequeueSymbol(term, "(");
                    term.AddChild(ParseExpressionList());
                    DequeueSymbol(term, ")");
                }
            }
            return term;
        }

        public Node ParseExpressionList()
        {
            var expressionList = new Node(NodeType.ExpressionList);
            while (Peek() != ")")
            {
                expressionList.AddChild(ParseExpression());
                if (Peek() == ",")
                    DequeueSymbol(expressionList, ",");
            }
            return expressionList;
        }

        private void DequeueType(Node parent)
        {
            Token type = Dequeue();
            if (type != null)
                parent.AddChild(type);
            else
                throw new ApplicationException("class variable definition expected a type, reached end of file instead");
        }

        private void DequeueKeyword(Node parent)
        {
            Token keyword = tokens.Dequeue();
            if (keyword.Type != NodeType.Keyword)
                throw new ApplicationException($"expected keyword, got {keyword} instead");
            parent.AddChild(keyword);
        }

        private Token DequeueIdentifier(Node parent, string error)
        {
            Token token = Dequeue();
            if (token?.Type == NodeType.Identifier)
            {
                parent.AddChild(token);
                if (Peek() == "[")
                {
                    DequeueSymbol(parent, "[");
                    parent.AddChild(ParseExpression());
                    DequeueSymbol(parent, "]");
                }
            }
            else
            {
                string suffix = token == null ? ", reached end of file instead" : $", got {token} instead";
                throw new ApplicationException(error + suffix);
            }
            return token;
        }

        private Token DequeueSymbol(Node parent, string symbol)
        {
            Token token = Dequeue();
            if (token == null || token.Type != NodeType.Symbol || token.Value != symbol)
            {
                string suffix = token == null ? "reached end of file instead" : $"got {token} instead";
                throw new ApplicationException($"expected symbol '{symbol}', {suffix}");
            }
            return parent.AddChild(token);
        }

        private Token Dequeue()
        {
            return tokens.Count > 0 ? tokens.Dequeue() : null;
        }

        private string Peek()
        {
            return tokens.Count > 0 ? tokens.Peek().Value : null;
        }

        private bool IsOperator(string s)
        {
            return s != null && "+-*/&|<>=".Contains(s);
        }

        private bool IsUnaryOperator(string s)
        {
            return s != null && (s == "-" || s == "~");
        }
    }
}
