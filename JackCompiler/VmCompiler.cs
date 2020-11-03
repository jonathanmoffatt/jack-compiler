using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace JackCompiler
{
    public class VmCompiler
    {
        private TextWriter vmFile;

        public void Compile(Node tree, TextWriter vmFile)
        {
            this.vmFile = vmFile;
            ProcessClass(tree);
        }

        private void ProcessClass(Node tree)
        {
            Expect(tree, NodeType.Class);
            Queue<NodeBase> children = GetChildren(tree);
            Expect(children.Dequeue(), NodeType.Keyword, "class");
            string className = Expect(children.Dequeue(), NodeType.Identifier);
            Expect(children.Dequeue(), NodeType.Symbol, "{");
            ProcessSubroutineDeclaration(children.Dequeue(), className);
            Expect(children.Dequeue(), NodeType.Symbol, "}");
        }

        private void ProcessSubroutineDeclaration(NodeBase subroutineDeclaration, string className)
        {
            Expect(subroutineDeclaration, NodeType.SubroutineDeclaration);
            Queue<NodeBase> children = GetChildren(subroutineDeclaration);
            string functionOrMethod = Expect(children.Dequeue(), NodeType.Keyword);
            bool isMethod = functionOrMethod == "method";
            Token returnType = GetKeyword(children.Dequeue());
            string name = Expect(children.Dequeue(), NodeType.Identifier);
            Expect(children.Dequeue(), NodeType.Symbol, "(");
            NodeBase parameterList = children.Dequeue();
            Expect(parameterList, NodeType.ParameterList);
            int parameterCount = ((Node)parameterList).Children.Count();
            if (isMethod)
                parameterCount++;
            vmFile.WriteLine($"function {className}.{name} {parameterCount}");
            Expect(children.Dequeue(), NodeType.Symbol, ")");
            ProcessSubroutineBody(children.Dequeue());
        }

        private void ProcessSubroutineBody(NodeBase subroutineBody)
        {
            Expect(subroutineBody, NodeType.SubroutineBody);
            Queue<NodeBase> children = GetChildren(subroutineBody);
            Expect(children.Dequeue(), NodeType.Symbol, "{");
            ProcessStatements(children.Dequeue());
            Expect(children.Dequeue(), NodeType.Symbol, "}");
        }

        private void ProcessStatements(NodeBase statements)
        {
            Expect(statements, NodeType.Statements);
            foreach (var child in GetChildren(statements))
            {
                ProcessStatement(child);
            }
        }

        private void ProcessStatement(NodeBase statement)
        {
            switch (statement.Type)
            {
                case NodeType.DoStatement:
                    ProcessDoStatement(statement);
                    break;
                case NodeType.ReturnStatement:
                    ProcessReturnStatement(statement);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void ProcessReturnStatement(NodeBase statement)
        {
            Queue<NodeBase> children = GetChildren(statement);
            Expect(children.Dequeue(), NodeType.Keyword, "return");
            var returnValue = children.Dequeue();
            if (IsSymbol(returnValue, ";"))
            {
                vmFile.WriteLine("push constant 0");
                vmFile.WriteLine("return");
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private void ProcessDoStatement(NodeBase statement)
        {
            Queue<NodeBase> children = GetChildren(statement);
            Expect(children.Dequeue(), NodeType.Keyword, "do");
            var identifier = GetIdentifier(children.Dequeue());
            string call = identifier.Value;
            Token token = GetSymbol(children.Dequeue());
            if (token.Value == ".")
            {
                var identifier2 = GetIdentifier(children.Dequeue());
                call = $"{call}.{identifier2.Value}";
                token = GetSymbol(children.Dequeue());
            }
            Expect(token, NodeType.Symbol, "(");
            int expressionCount = ProcessExpressionList(children.Dequeue());
            Expect(children.Dequeue(), NodeType.Symbol, ")");
            Expect(children.Dequeue(), NodeType.Symbol, ";");

            vmFile.WriteLine($"call {call} {expressionCount}");
            vmFile.WriteLine("pop temp 0");
        }

        private int ProcessExpressionList(NodeBase expressionList)
        {
            Expect(expressionList, NodeType.ExpressionList);
            Queue<NodeBase> expressions = GetChildren(expressionList);
            int expressionCount = expressions.Count;
            foreach (var expression in expressions)
            {
                ProcessExpression(expression);
            }
            return expressionCount;
        }

        private void ProcessExpression(NodeBase expression)
        {
            Expect(expression, NodeType.Expression);
            Queue<NodeBase> children = GetChildren(expression);
            ProcessTerm(children.Dequeue());
            if (children.Any())
            {
                Token op = GetSymbol(children.Dequeue());
                ProcessTerm(children.Dequeue());
                switch (op.Value)
                {
                    case "+":
                        vmFile.WriteLine("add");
                        break;
                    case "*":
                        vmFile.WriteLine("call Math.multiply 2");
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        private void ProcessTerm(NodeBase term)
        {
            Expect(term, NodeType.Term);
            Queue<NodeBase> children = GetChildren(term);
            NodeBase firstChild = children.Dequeue();
            if (IsSymbol(firstChild, "("))
            {
                ProcessExpression(children.Dequeue());
                Expect(children.Dequeue(), NodeType.Symbol, ")");
            }
            else
            {
                switch(firstChild.Type)
                {
                    case NodeType.IntegerConstant:
                        vmFile.WriteLine($"push constant {((Token)firstChild).Value}");
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        private Queue<NodeBase> GetChildren(NodeBase parent)
        {
            return new Queue<NodeBase>(((Node)parent).Children);
        }

        private Identifier GetIdentifier(NodeBase nodeBase)
        {
            if (!(nodeBase is Identifier))
                throw new ArgumentException($"expected an identifier, got {nodeBase} instead");
            return (Identifier)nodeBase;
        }

        private bool IsSymbol(NodeBase nodeBase, string symbol)
        {
            return nodeBase.Type == NodeType.Symbol && ((Token)nodeBase).Value == symbol;
        }

        private Token GetSymbol(NodeBase nodeBase)
        {
            Token token = nodeBase as Token;
            if (token == null || token.Type != NodeType.Symbol)
                throw new ArgumentException($"expected a symbol, got {nodeBase} instead");
            return token;
        }

        private Token GetKeyword(NodeBase nodeBase)
        {
            Token token = nodeBase as Token;
            if (token == null || token.Type != NodeType.Keyword)
                throw new ArgumentException($"expected a keyword, got {nodeBase} instead");
            return token;
        }

        private string Expect(NodeBase nodeBase, NodeType nodeType, string value = null)
        {
            Token token = nodeBase as Token;
            if (nodeBase.Type != nodeType)
                throw new ArgumentException($"expected node type to be {nodeType}");
            if (value != null && token != null && token.Value != value)
                throw new ArgumentException($"expected node to have value '{value}'");
            return token != null ? token.Value : null;
        }
    }
}
