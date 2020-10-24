using System.Collections.Generic;

namespace JackCompiler
{
    public interface IGrammarian
    {
        IGrammarian LoadTokens(Queue<Token> tokens);
        Node ParseClass();
        Node ParseClassVariableDeclaration();
        Node ParseExpression();
        Node ParseExpressionList();
        Node ParseParameterList();
        Node ParseReturnStatement();
        Node ParseStatements();
        Node ParseSubroutineBody();
        Node ParseSubroutineDeclaration();
        Node ParseTerm();
        Node[] ParseVariableDeclarations();
    }
}
