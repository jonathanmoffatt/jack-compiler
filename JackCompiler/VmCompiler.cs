﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace JackCompiler
{
    public class VmCompiler
    {
        private IVmWriter vmWriter;
        private int classSize;
        private string className;

        public void Compile(Node tree, IVmWriter vmWriter)
        {
            this.vmWriter = vmWriter;
            ProcessClass(tree);
        }

        private void ProcessClass(Node tree)
        {
            Expect(tree, NodeType.Class);
            Queue<NodeBase> children = GetChildren(tree);
            Expect(children.Dequeue(), NodeType.Keyword, "class");
            className = Expect(children.Dequeue(), NodeType.Identifier);
            Expect(children.Dequeue(), NodeType.Symbol, "{");
            classSize = ProcessClassVariableDeclarations(children);
            while (PeekType(children) == NodeType.SubroutineDeclaration)
            {
                ProcessSubroutineDeclaration(children.Dequeue());
            }
            Expect(children.Dequeue(), NodeType.Symbol, "}");
        }

        private int ProcessClassVariableDeclarations(Queue<NodeBase> children)
        {
            int numberOfFieldDeclarations = 0;
            while (PeekType(children) == NodeType.ClassVariableDeclaration)
            {
                Queue<NodeBase> variableDeclarationChildren = GetChildren(children.Dequeue());
                string fieldOrStatic = Expect(variableDeclarationChildren.Dequeue(), NodeType.Keyword);
                if (fieldOrStatic == "field")
                {
                    numberOfFieldDeclarations++;
                    foreach (var varDecChild in variableDeclarationChildren)
                    {
                        if (IsSymbol(varDecChild, ",")) numberOfFieldDeclarations++;
                    }
                }
            }
            return numberOfFieldDeclarations;
        }

        private void ProcessSubroutineDeclaration(NodeBase subroutineDeclaration)
        {
            Expect(subroutineDeclaration, NodeType.SubroutineDeclaration);
            Queue<NodeBase> children = GetChildren(subroutineDeclaration);
            string functionMethodOrConstructor = Expect(children.Dequeue(), NodeType.Keyword); // function or method or constructor
            children.Dequeue(); // return type
            string name = Expect(children.Dequeue(), NodeType.Identifier);
            Expect(children.Dequeue(), NodeType.Symbol, "(");
            NodeBase parameterList = children.Dequeue();
            Expect(parameterList, NodeType.ParameterList);
            Expect(children.Dequeue(), NodeType.Symbol, ")");
            ProcessSubroutineBody(children.Dequeue(), functionMethodOrConstructor, name);
        }

        private void ProcessSubroutineBody(NodeBase subroutineBody, string functionMethodOrConstructor, string name)
        {
            Expect(subroutineBody, NodeType.SubroutineBody);
            Queue<NodeBase> children = GetChildren(subroutineBody);
            Expect(children.Dequeue(), NodeType.Symbol, "{");
            int numberOfVariableDeclarations = ProcessVariableDeclarations(children);
            switch(functionMethodOrConstructor)
            {
                case "constructor":
                    vmWriter.Constructor(className, name, classSize, numberOfVariableDeclarations);
                    break;
                case "function":
                    vmWriter.Function(className, name, numberOfVariableDeclarations);
                    break;
                case "method":
                    vmWriter.Method(className, name, numberOfVariableDeclarations);
                    break;
            }
            ProcessStatements(children.Dequeue());
            Expect(children.Dequeue(), NodeType.Symbol, "}");
        }

        private int ProcessVariableDeclarations(Queue<NodeBase> children)
        {
            int numberOfVariableDeclarations = 0;
            while (PeekType(children) == NodeType.VariableDeclaration)
            {
                Queue<NodeBase> variableDeclaration = GetChildren(children.Dequeue());
                numberOfVariableDeclarations++;
                foreach(var varDecChild in variableDeclaration)
                {
                    if (IsSymbol(varDecChild, ",")) numberOfVariableDeclarations++;
                }
            }
            return numberOfVariableDeclarations;
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
                case NodeType.LetStatement:
                    ProcessLetStatement(statement);
                    break;
                case NodeType.WhileStatement:
                    ProcessWhileStatement(statement);
                    break;
                case NodeType.IfStatement:
                    ProcessIfStatement(statement);
                    break;
                default:
                    throw GenerateNotImplementedException(statement.Type.ToString());
            }
        }

        private void ProcessIfStatement(NodeBase statement)
        {
            Queue<NodeBase> children = GetChildren(statement);
            Expect(children.Dequeue(), NodeType.Keyword, "if");
            Expect(children.Dequeue(), NodeType.Symbol, "(");

            ProcessExpression(children.Dequeue());
            int ifStatementNumber = vmWriter.GetIfStatementNumber();
            vmWriter.IfStart(ifStatementNumber);

            Expect(children.Dequeue(), NodeType.Symbol, ")");
            Expect(children.Dequeue(), NodeType.Symbol, "{");

            ProcessStatements(children.Dequeue());

            Expect(children.Dequeue(), NodeType.Symbol, "}");

            if (PeekType(children) == NodeType.Keyword && PeekValue(children) == "else")
            {
                vmWriter.IfElse(ifStatementNumber);
                Expect(children.Dequeue(), NodeType.Keyword, "else");
                Expect(children.Dequeue(), NodeType.Symbol, "{");
                ProcessStatements(children.Dequeue());
                Expect(children.Dequeue(), NodeType.Symbol, "}");
                vmWriter.IfElseEnd(ifStatementNumber);
            }
            else
            {
                vmWriter.IfEnd(ifStatementNumber);
            }
        }

        private void ProcessWhileStatement(NodeBase statement)
        {
            Queue<NodeBase> children = GetChildren(statement);
            Expect(children.Dequeue(), NodeType.Keyword, "while");
            Expect(children.Dequeue(), NodeType.Symbol, "(");

            int whileStatementNumber = vmWriter.GetWhileStatementNumber();
            vmWriter.WhileStart(whileStatementNumber);
            ProcessExpression(children.Dequeue());
            vmWriter.WhileBreak(whileStatementNumber);

            Expect(children.Dequeue(), NodeType.Symbol, ")");
            Expect(children.Dequeue(), NodeType.Symbol, "{");

            ProcessStatements(children.Dequeue());

            Expect(children.Dequeue(), NodeType.Symbol, "}");
            vmWriter.WhileEnd(whileStatementNumber);
        }

        private void ProcessLetStatement(NodeBase statement)
        {
            Queue<NodeBase> children = GetChildren(statement);
            Expect(children.Dequeue(), NodeType.Keyword, "let");
            Identifier identifier = GetIdentifier(children.Dequeue());
            if (PeekValue(children) == "[")
            {
                Expect(children.Dequeue(), NodeType.Symbol, "[");
                ProcessExpression(children.Dequeue());
                vmWriter.Push(identifier);
                vmWriter.IndexArray();
                Expect(children.Dequeue(), NodeType.Symbol, "]");
                Expect(children.Dequeue(), NodeType.Symbol, "=");
                ProcessExpression(children.Dequeue());
                Expect(children.Dequeue(), NodeType.Symbol, ";");
                vmWriter.AssignArray();
            }
            else
            {
                Expect(children.Dequeue(), NodeType.Symbol, "=");
                ProcessExpression(children.Dequeue());
                Expect(children.Dequeue(), NodeType.Symbol, ";");
                vmWriter.Pop(identifier);
            }
        }

        private void ProcessReturnStatement(NodeBase statement)
        {
            Queue<NodeBase> children = GetChildren(statement);
            Expect(children.Dequeue(), NodeType.Keyword, "return");
            if (PeekType(children) == NodeType.Expression)
            {
                ProcessExpression(children.Dequeue());
                vmWriter.Return();
            }
            else
            {
                vmWriter.ReturnNothing();
            }
            Expect(children.Dequeue(), NodeType.Symbol, ";");
        }

        private void ProcessDoStatement(NodeBase statement)
        {
            Queue<NodeBase> children = GetChildren(statement);
            Expect(children.Dequeue(), NodeType.Keyword, "do");

            var identifier = GetIdentifier(children.Dequeue());
            ProcessSubroutineCall(children, identifier);

            Expect(children.Dequeue(), NodeType.Symbol, ";");
            vmWriter.DiscardCallResult();
        }

        private void ProcessSubroutineCall(Queue<NodeBase> children, Identifier identifier)
        {
            string call;
            Token token = GetSymbol(children.Dequeue());
            if (token.Value == ".")
            {
                var classMemberIdentifier = GetIdentifier(children.Dequeue());
                call = $"{(identifier.ClassType != null ? identifier.ClassType : identifier.Value)}.{classMemberIdentifier.Value}";
                token = GetSymbol(children.Dequeue());
            }
            else
            {
                call = $"{className}.{identifier.Value}";
            }
            int argumentCount = 0;
            if (identifier.Kind != IdentifierKind.Class)
            {
                vmWriter.Push(identifier);
                argumentCount++;
            }
            Expect(token, NodeType.Symbol, "(");
            argumentCount += ProcessExpressionList(children.Dequeue());
            Expect(children.Dequeue(), NodeType.Symbol, ")");
            vmWriter.Call(call, argumentCount);
        }

        private int ProcessExpressionList(NodeBase expressionList)
        {
            Expect(expressionList, NodeType.ExpressionList);
            Queue<NodeBase> expressions = GetChildren(expressionList);
            int expressionCount = 0;
            while(expressions.Any())
            {
                ProcessExpression(expressions.Dequeue());
                expressionCount++;
                if (PeekValue(expressions) == ",") expressions.Dequeue();
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
                vmWriter.Arithmetic(op.Value);
            }
        }

        private void ProcessTerm(NodeBase term)
        {
            Expect(term, NodeType.Term);
            Queue<NodeBase> children = GetChildren(term);
            if (PeekValue(children) == "(")
            {
                Expect(children.Dequeue(), NodeType.Symbol, "(");
                ProcessExpression(children.Dequeue());
                Expect(children.Dequeue(), NodeType.Symbol, ")");
            }
            else
            {
                NodeType? type = PeekType(children);
                switch (type)
                {
                    case NodeType.IntegerConstant:
                        vmWriter.PushConstant(GetValue(children.Dequeue()));
                        break;
                    case NodeType.StringConstant:
                        vmWriter.PushStringConstant(GetValue(children.Dequeue()));
                        break;
                    case NodeType.Keyword:
                        string keywordValue = GetValue(children.Dequeue());
                        switch(keywordValue)
                        {
                            case "true":
                                vmWriter.PushTrue();
                                break;
                            case "false":
                                vmWriter.PushFalse();
                                break;
                            case "this":
                                vmWriter.PushThis();
                                break;
                            case "null":
                                vmWriter.PushConstant("0");
                                break;
                            default:
                                throw GenerateNotImplementedException(keywordValue);
                        }
                        break;
                    case NodeType.Symbol:
                        string unaryOp = Expect(children.Dequeue(), NodeType.Symbol);
                        ProcessTerm(children.Dequeue());
                        vmWriter.Unary(unaryOp);
                        break;
                    case NodeType.Identifier:
                        Identifier identifier = GetIdentifier(children.Dequeue());
                        if (PeekValue(children) == "." || PeekValue(children) == "(")
                        {
                            ProcessSubroutineCall(children, identifier);
                        }
                        else if (PeekValue(children) == "[")
                        {
                            Expect(children.Dequeue(), NodeType.Symbol, "[");
                            ProcessExpression(children.Dequeue());
                            vmWriter.Push(identifier);
                            vmWriter.AccessArray();
                        }
                        else
                        {
                            vmWriter.Push(identifier);
                        }
                        break;
                    default:
                        throw GenerateNotImplementedException(type.ToString());
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
                throw GenerateException($"expected an identifier, got {nodeBase} instead");
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
                throw GenerateException($"expected a symbol, got {nodeBase} instead");
            return token;
        }

        private Token GetKeyword(NodeBase nodeBase)
        {
            Token token = nodeBase as Token;
            if (token == null || token.Type != NodeType.Keyword)
                throw GenerateException($"expected a keyword, got {nodeBase} instead");
            return token;
        }

        private string Expect(NodeBase nodeBase, NodeType nodeType, string value = null)
        {
            Token token = nodeBase as Token;
            if (nodeBase.Type != nodeType)
                throw GenerateException($"expected node type to be {nodeType}, but got {nodeBase.Type}");
            if (value != null && token != null && token.Value != value)
                throw GenerateException($"expected node to have value '{value}', but got {token.Value}");
            return token != null ? token.Value : null;
        }

        private NodeType? PeekType(Queue<NodeBase> queue)
        {
            return queue.Any() ? queue.Peek().Type : (NodeType?)null;
        }

        private string PeekValue(Queue<NodeBase> queue)
        {
            return queue.Any() ? GetValue(queue.Peek()) : null;
        }

        private string GetValue(NodeBase nodeBase)
        {
            return nodeBase is Token ? ((Token)nodeBase).Value : null;
        }

        private ArgumentException GenerateException(string message)
        {
            return new ArgumentException($"{message}. VM generated so far:\n{vmWriter}");
        }

        private NotImplementedException GenerateNotImplementedException(string missing)
        {
            return new NotImplementedException($"\nNot yet implemented \"{missing}\"\nVM generated so far:\n{vmWriter}");
        }
    }
}
