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


        public void Constructor(string className, string name, int classSize, int numberOfVariableDeclarations)
        {
            Function(className, name, numberOfVariableDeclarations);
            if (classSize > 0)
            {
                writer.WriteLine($"push constant {classSize}");
                writer.WriteLine("call Memory.alloc 1");
                writer.WriteLine("pop pointer 0");
            }
        }

        public void Function(string className, string name, int numberOfVariableDeclarations)
        {
            whileLoopNumber = 0;
            ifStatementNumber = 0;
            writer.WriteLine($"function {className}.{name} {numberOfVariableDeclarations}");
        }

        public void Method(string className, string name, int numberOfVariableDeclarations)
        {
            whileLoopNumber = 0;
            ifStatementNumber = 0;
            Function(className, name, numberOfVariableDeclarations);
            writer.WriteLine("push argument 0");
            writer.WriteLine("pop pointer 0");
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
                case IdentifierKind.Subroutine:
                    writer.WriteLine($"push pointer 0");
                    break;
                default:
                    throw new ArgumentException($"{identifier.Kind} is not valid for push");
            }
        }

        public void PushConstant(string constant)
        {
            writer.WriteLine($"push constant {constant}");
        }

        private void PushConstant(int constant)
        {
            PushConstant(constant.ToString());
        }

        private void PushConstant(char constant)
        {
            PushConstant((int)constant);
        }

        public void PushStringConstant(string constant)
        {
            int length = constant.Length;
            PushConstant(length.ToString());
            writer.WriteLine("call String.new 1");
            foreach(char c in constant)
            {
                PushConstant(c);
                writer.WriteLine("call String.appendChar 2");
            }
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

        public void PushThis()
        {
            writer.WriteLine("push pointer 0");
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

        public void Call(string call, int expressionCount)
        {
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
                case "|":
                    writer.WriteLine("or");
                    break;
                case "=":
                    writer.WriteLine("eq");
                    break;
                case "/":
                    writer.WriteLine("call Math.divide 2");
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

        public void IfElseEnd(int ifStatementNumber)
        {
            writer.WriteLine($"label IF_END{ifStatementNumber}");
        }

        public void IfEnd(int ifStatementNumber)
        {
            writer.WriteLine($"label IF_FALSE{ifStatementNumber}");
        }

        public void IndexArray()
        {
            writer.WriteLine("add");
        }

        public void AccessArray()
        {
            writer.WriteLine("add");
            writer.WriteLine("pop pointer 1");
            writer.WriteLine("push that 0");
        }

        public void AssignArray()
        {
            writer.WriteLine("pop temp 0");
            writer.WriteLine("pop pointer 1");
            writer.WriteLine("push temp 0");
            writer.WriteLine("pop that 0");
        }

        public override string ToString()
        {
            return writer.ToString();
        }

    }
}
