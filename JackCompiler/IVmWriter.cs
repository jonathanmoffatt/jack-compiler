namespace JackCompiler
{
    public interface IVmWriter
    {
        void Arithmetic(string op);
        void Call(string call, int expressionCount);
        void Function(string className, string name, int numberOfVariableDeclarations);
        void Pop(Identifier identifier);
        void Push(Identifier identifier);
        void PushConstant(string constant);
        void PushFalse();
        void PushTrue();
        void Return();
        int GetWhileLoopNumber();
        void WhileStart(int whileLoopNumber);
        void WhileBreak(int whileLoopNumber);
        void WhileFinish(int whileLoopNumber);
    }
}