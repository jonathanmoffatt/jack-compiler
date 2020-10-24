using System;
namespace JackCompiler
{
    public enum NodeType
    {
        [ElementName("class")]
        Class,
        [ElementName("classVarDec")]
        ClassVariableDeclaration,
        [ElementName("expression")]
        Expression,
        [ElementName("expressionList")]
        ExpressionList,
        [ElementName("identifier")]
        Identifier,
        [ElementName("ifStatement")]
        IfStatement,
        [ElementName("integerConstant")]
        IntegerConstant,
        [ElementName("keyword")]
        Keyword,
        [ElementName("letStatement")]
        LetStatement,
        [ElementName("parameterList")]
        ParameterList,
        [ElementName("returnStatement")]
        ReturnStatement,
        [ElementName("statements")]
        Statements,
        [ElementName("stringConstant")]
        StringConstant,
        [ElementName("subroutineBody")]
        SubroutineBody,
        [ElementName("subroutineDec")]
        SubroutineDeclaration,
        [ElementName("symbol")]
        Symbol,
        [ElementName("term")]
        Term,
        [ElementName("varDec")]
        VariableDeclaration,
        [ElementName("whileStatement")]
        WhileStatement,
        [ElementName("doStatement")]
        DoStatement
    }

    public class ElementNameAttribute : Attribute
    {
        public ElementNameAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
