using System;
using System.IO;

namespace JackCompiler
{
    public class VmWriter : IVmWriter
    {
        private TextWriter writer;

        public VmWriter(TextWriter writer)
        {
            this.writer = writer;
        }

        public void Function(string className, string name, int numberOfVariableDeclarations)
        {
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

        public void Return()
        {
            writer.WriteLine("push constant 0");
            writer.WriteLine("return");
        }

        public void Call(string call, int expressionCount)
        {
            writer.WriteLine($"call {call} {expressionCount}");
            writer.WriteLine("pop temp 0");
        }

        public void Arithmetic(string op)
        {
            switch (op)
            {
                case "+":
                    writer.WriteLine("add");
                    break;
                case "*":
                    writer.WriteLine("call Math.multiply 2");
                    break;
                case "neg":
                    writer.WriteLine("neg");
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
    }
}
