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
        void ReturnNothing();
        int GetWhileStatementNumber();
        void WhileStart(int whileLoopNumber);
        void WhileBreak(int whileLoopNumber);
        void WhileEnd(int whileLoopNumber);
        int GetIfStatementNumber();
        void IfStart(int ifStatementNumber);
        void IfElse(int ifStatementNumber);
        void IfElseEnd(int ifStatementNumber);
        void Unary(string op);
        void Return();
        void DiscardCallResult();
        void Constructor(string className, string name, int classSize, int numberOfVariableDeclarations);
        void PushThis();
        void Method(string className, string name, int numberOfVariableDeclarations);
        void IfEnd(int ifStatementNumber);
        void PushStringConstant(string v);
        void IndexArray();
        void AssignArray();
        void AccessArray();
    }
}