using System;
using System.IO;

namespace JackCompiler
{
    public class VmWriter : IVmWriter
    {
        private TextWriter writer;
        private int whileLoopNumber;
        private int ifStatementNumber;

        public VmWriter(TextWriter writer)
        {
            this.writer = writer;
        }

        public void Function(string className, string name, int numberOfVariableDeclarations)
        {
            whileLoopNumber = 0;
            ifStatementNumber = 0;
            writer.WriteLine($"function {className}.{name} {numberOfVariableDeclarations}");
        }

        public void Pop(Identifier identifier)
        {
            switch (identifier.Kind)
            {
                case IdentifierKind.Var:
                    writer.WriteLine($"pop local {identifier.Number}");
                    break;
                case IdentifierKind.Argument:
                    writer.WriteLine($"pop argument {identifier.Number}");
                    break;
                case IdentifierKind.Static:
                    writer.WriteLine($"pop static {identifier.Number}");
                    break;
                case IdentifierKind.Field:
                    writer.WriteLine($"pop this {identifier.Number}");
                    break;
                default:
                    throw new ArgumentException($"{identifier.Kind} is not valid for pop");
            }
        }

        public void Push(Identifier identifier)
        {
            switch (identifier.Kind)
            {
                case IdentifierKind.Var:
                    writer.WriteLine($"push local {identifier.Number}");
                    break;
                case IdentifierKind.Argument:
                    writer.WriteLine($"push argument {identifier.Number}");
                    break;
                case IdentifierKind.Static:
                    writer.WriteLine($"push static {identifier.Number}");
                    break;
                case IdentifierKind.Field:
                    writer.WriteLine($"push this {identifier.Number}");
                    break;
                default:
                    throw new ArgumentException($"{identifier.Kind} is not valid for push");
            }
        }

        public void ReturnNothing()
        {
            writer.WriteLine("push constant 0");
            writer.WriteLine("return");
        }

        public void Return()
        {
            writer.WriteLine("return");
        }

        public void Call(string call, int expressionCount, bool isClassStaticMemberCall)
        {
            if (isClassStaticMemberCall)
            {
                expressionCount++;
                writer.WriteLine("push local 0");
            }
            writer.WriteLine($"call {call} {expressionCount}");
        }

        public void DiscardCallResult()
        {
            writer.WriteLine("pop temp 0");
        }

        public void Arithmetic(string op)
        {
            switch (op)
            {
                case "+":
                    writer.WriteLine("add");
                    break;
                case "-":
                    writer.WriteLine("sub");
                    break;
                case "*":
                    writer.WriteLine("call Math.multiply 2");
                    break;
                case ">":
                    writer.WriteLine("gt");
                    break;
                case "<":
                    writer.WriteLine("lt");
                    break;
                case "&":
                    writer.WriteLine("and");
                    break;
                case "=":
                    writer.WriteLine("eq");
                    break;
                default:
                    throw GenerateNotImplementedException(op);
            }
        }

        public void Unary(string op)
        {
            switch (op)
            {
                case "-":
                    writer.WriteLine("neg");
                    break;
                case "~":
                    writer.WriteLine("not");
                    break;
                default:
                    throw GenerateNotImplementedException(op);
            }
        }

        public void PushConstant(string constant)
        {
            writer.WriteLine($"push constant {constant}");
        }

        public void PushFalse()
        {
            writer.WriteLine("push constant 0");
        }

        public void PushTrue()
        {
            PushFalse();
            writer.WriteLine("not");
        }

        public override string ToString()
        {
            return writer.ToString();
        }

        private NotImplementedException GenerateNotImplementedException(string missing)
        {
            return new NotImplementedException($"\nNot yet implemented \"{missing}\"\nVM generated so far:\n{writer}");
        }

        public int GetWhileStatementNumber()
        {
            return whileLoopNumber++;
        }

        public void WhileStart(int whileLoopNumber)
        {
            writer.WriteLine($"label WHILE_EXP{whileLoopNumber}");
        }

        public void WhileBreak(int whileLoopNumber)
        {
            writer.WriteLine("not");
            writer.WriteLine($"if-goto WHILE_END{whileLoopNumber}");
        }

        public void WhileEnd(int whileLoopNumber)
        {
            writer.WriteLine($"goto WHILE_EXP{whileLoopNumber}");
            writer.WriteLine($"label WHILE_END{whileLoopNumber}");
        }

        public int GetIfStatementNumber()
        {
            return ifStatementNumber++;
        }

        public void IfStart(int ifStatementNumber)
        {
            writer.WriteLine($"if-goto IF_TRUE{ifStatementNumber}");
            writer.WriteLine($"goto IF_FALSE{ifStatementNumber}");
            writer.WriteLine($"label IF_TRUE{ifStatementNumber}");
        }

        public void IfElse(int ifStatementNumber)
        {
            writer.WriteLine($"goto IF_END{ifStatementNumber}");
            writer.WriteLine($"label IF_FALSE{ifStatementNumber}");
        }

        public void IfEnd(int ifStatementNumber)
        {
            writer.WriteLine($"label IF_END{ifStatementNumber}");
        }
    }
}
