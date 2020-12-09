using System;
using System.Xml;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JackCompiler.Tests
{
    internal static class NodeTestExtensions
    {
        public static void ShouldGenerateXml(this NodeBase node, string expectedXml)
        {
            var converter = new XmlConverter();
            var doc = new XmlDocument();
            doc.LoadXml(expectedXml);
            XmlDocument xmlDocument = converter.ConvertNode(node);
            xmlDocument.Should().BeEquivalentTo(doc);
        }
    }

    #region ClassGrammar

    [TestClass]
    public class ClassGrammar
    {
        private Grammarian classUnderTest;
        private Token t1, t2, t3, t4;

        [TestInitialize]
        public void Setup()
        {
            t1 = new Token(NodeType.Keyword, "class");
            t2 = new Token(NodeType.Identifier, "blah");
            t3 = new Token(NodeType.Symbol, "{");
            t4 = new Token(NodeType.Symbol, "}");
            classUnderTest = new Grammarian();
        }

        [TestMethod]
        public void ReturnsAClassNode()
        {
            classUnderTest.LoadTokens(t1, t2, t3, t4).ParseClass().ShouldGenerateXml(@"
                <class>
                  <keyword>class</keyword>
                  <identifier kind='class' isDefinition='true'>blah</identifier>
                  <symbol>{</symbol>
                  <symbol>}</symbol>
                </class>");
        }

        [TestMethod]
        public void ThrowsExceptionIfClassIsMissingClassName()
        {
            classUnderTest.LoadTokens(t1, t3, t4);
            classUnderTest
                .Invoking(c => c.ParseClass())
                .Should().Throw<ApplicationException>()
                .WithMessage("class expected a className identifier, got Symbol '{' instead");
        }

        [TestMethod]
        public void ThrowsExceptionIfClassIsMissingOpeningBrace()
        {
            classUnderTest.LoadTokens(t1, t2, t4);
            classUnderTest
                .Invoking(c => c.ParseClass())
                .Should().Throw<ApplicationException>()
                .WithMessage("expected symbol '{', got Symbol '}' instead");
        }

        [TestMethod]
        public void ThrowsExceptionIfClassIsMissingClosingBrace()
        {
            classUnderTest.LoadTokens(t1, t2, t3);
            classUnderTest
                .Invoking(c => c.ParseClass())
                .Should().Throw<ApplicationException>()
                .WithMessage("expected symbol '}', reached end of file instead");
        }

        [TestMethod]
        public void ThrowsExceptionIfClassIsMissingEverything()
        {
            classUnderTest.LoadTokens(t1);
            classUnderTest
                .Invoking(c => c.ParseClass())
                .Should().Throw<ApplicationException>()
                .WithMessage("class expected a className identifier, reached end of file instead");
        }

    }

    #endregion

    #region ClassVariableDeclarationGrammar

    [TestClass]
    public class ClassVariableDeclarationGrammar
    {
        private Grammarian classUnderTest;
        private Token cvd1, cvd1a, cvd2, cvd3, cvd4, cvd5, cvd6;

        [TestInitialize]
        public void Setup()
        {
            cvd1 = new Token(NodeType.Keyword, "static");
            cvd1a = new Token(NodeType.Keyword, "field");
            cvd2 = new Token(NodeType.Keyword, "boolean");
            cvd3 = new Token(NodeType.Identifier, "hasStarted");
            cvd4 = new Token(NodeType.Symbol, ",");
            cvd5 = new Token(NodeType.Identifier, "hasFinished");
            cvd6 = new Token(NodeType.Symbol, ";");
            classUnderTest = new Grammarian();
        }

        [TestMethod]
        public void RecognisesStaticClassVariableDeclaration()
        {
            classUnderTest
                .LoadTokens(cvd1, cvd2, cvd3, cvd4, cvd5, cvd6)
                .ParseClassVariableDeclaration()
                .ShouldGenerateXml(@"
                  <classVarDec>
                    <keyword>static</keyword>
                    <keyword>boolean</keyword>
                    <identifier kind='static' isDefinition='true' number='0'>hasStarted</identifier>
                    <symbol>,</symbol>
                    <identifier kind='static' isDefinition='true' number='1'>hasFinished</identifier>
                    <symbol>;</symbol>
                  </classVarDec>
            ");
        }

        [TestMethod]
        public void RecognisesFieldClassVariableDeclarations()
        {
            classUnderTest
                .LoadTokens(cvd1a, cvd2, cvd3, cvd4, cvd5, cvd6)
                .ParseClassVariableDeclaration()
                .ShouldGenerateXml(@"
                  <classVarDec>
                    <keyword>field</keyword>
                    <keyword>boolean</keyword>
                    <identifier kind='field' isDefinition='true' number='0'>hasStarted</identifier>
                    <symbol>,</symbol>
                    <identifier kind='field' isDefinition='true' number='1'>hasFinished</identifier>
                    <symbol>;</symbol>
                  </classVarDec>
            ");
        }

        [TestMethod]
        public void ThrowsExceptionIfClassVariableDefinitionTypeMissing()
        {
            classUnderTest.LoadTokens(cvd1);
            classUnderTest
                .Invoking(c => c.ParseClassVariableDeclaration())
                .Should().Throw<ApplicationException>()
                .WithMessage("class variable definition expected a type, reached end of file instead");
        }

        [TestMethod]
        public void ThrowsExceptionIfClassVariableDefinitionVariableNameMissing()
        {
            classUnderTest.LoadTokens(cvd1, cvd2);
            classUnderTest
                .Invoking(c => c.ParseClassVariableDeclaration())
                .Should().Throw<ApplicationException>()
                .WithMessage("class static or field declaration expected a variable name, reached end of file instead");
        }
    }

    #endregion

    #region SubroutineDeclarationGrammar

    [TestClass]
    public class SubroutineDeclarationGrammar
    {
        private Grammarian classUnderTest;
        private Token sd1, sd1a, sd1b, sd2, sd3, sd4, sd5, sd6, sd7, sd8, sd9, sd10, sd11, sd12;
        private Token vd1, vd2, vd3, vd4, vd5, vd6, vd7, vd8, vd9, vd10;

        [TestInitialize]
        public void Setup()
        {
            sd1 = new Token(NodeType.Keyword, "constructor");
            sd1a = new Token(NodeType.Keyword, "function");
            sd1b = new Token(NodeType.Keyword, "method");
            sd2 = new Token(NodeType.Keyword, "void");
            sd3 = new Token(NodeType.Identifier, "doSomething");
            sd4 = new Token(NodeType.Symbol, "(");
            sd5 = new Token(NodeType.Keyword, "int");
            sd6 = new Token(NodeType.Identifier, "x");
            sd7 = new Token(NodeType.Symbol, ",");
            sd8 = new Token(NodeType.Identifier, "Game");
            sd9 = new Token(NodeType.Identifier, "game");
            sd10 = new Token(NodeType.Symbol, ")");
            sd11 = new Token(NodeType.Symbol, "{");
            vd1 = new Token(NodeType.Keyword, "var");
            vd2 = new Token(NodeType.Keyword, "boolean");
            vd3 = new Token(NodeType.Identifier, "hasStarted");
            vd4 = new Token(NodeType.Symbol, ",");
            vd5 = new Token(NodeType.Identifier, "hasFinished");
            vd6 = new Token(NodeType.Symbol, ";");
            vd7 = new Token(NodeType.Keyword, "var");
            vd8 = new Token(NodeType.Identifier, "Player");
            vd9 = new Token(NodeType.Identifier, "player");
            vd10 = new Token(NodeType.Symbol, ";");
            sd12 = new Token(NodeType.Symbol, "}");
            classUnderTest = new Grammarian();
        }

        [TestMethod]
        public void ReturnsASubroutineDeclaration()
        {
            classUnderTest
                .LoadTokens(sd1, sd2, sd3, sd4, sd5, sd6, sd7, sd8, sd9, sd10, sd11, sd12)
                .ParseSubroutineDeclaration()
                .ShouldGenerateXml(@"
                <subroutineDec>
                    <keyword>constructor</keyword>
                    <keyword>void</keyword>
                    <identifier kind='subroutine' isDefinition='true'>doSomething</identifier>
                    <symbol>(</symbol>
                    <parameterList>
                        <keyword>int</keyword>
                        <identifier kind='argument' isDefinition='true' number='0'>x</identifier>
                        <symbol>,</symbol>
                        <identifier kind='class' isDefinition='false'>Game</identifier>
                        <identifier kind='argument' isDefinition='true' number='1' classType='Game'>game</identifier>
                    </parameterList>
                    <symbol>)</symbol>
                    <subroutineBody>
                        <symbol>{</symbol>
                        <symbol>}</symbol>
                    </subroutineBody>
                </subroutineDec>
            ");
        }

        [TestMethod]
        public void ShouldHandleFunctionSubroutines()
        {
            classUnderTest
                .LoadTokens(sd1a, sd2, sd3, sd4, sd5, sd6, sd7, sd8, sd9, sd10, sd11, sd12)
                .ParseSubroutineDeclaration()
                .ShouldGenerateXml(@"
                  <subroutineDec>
                    <keyword>function</keyword>
                    <keyword>void</keyword>
                    <identifier kind='subroutine' isDefinition='true'>doSomething</identifier>
                    <symbol>(</symbol>
                    <parameterList>
                      <keyword>int</keyword>
                      <identifier kind='argument' isDefinition='true' number='0'>x</identifier>
                      <symbol>,</symbol>
                      <identifier kind='class' isDefinition='false'>Game</identifier>
                      <identifier kind='argument' isDefinition='true' number='1' classType='Game'>game</identifier>
                    </parameterList>
                    <symbol>)</symbol>
                    <subroutineBody>
                      <symbol>{</symbol>
                      <symbol>}</symbol>
                    </subroutineBody>
                  </subroutineDec>
            ");
        }

        [TestMethod]
        public void ShouldHandleMethodSubroutines()
        {
            classUnderTest
                .LoadTokens(sd1b, sd2, sd3, sd4, sd5, sd6, sd7, sd8, sd9, sd10, sd11, sd12)
                .ParseSubroutineDeclaration()
                .ShouldGenerateXml(@"
                  <subroutineDec>
                    <keyword>method</keyword>
                    <keyword>void</keyword>
                    <identifier kind='subroutine' isDefinition='true'>doSomething</identifier>
                    <symbol>(</symbol>
                    <parameterList>
                      <keyword>int</keyword>
                      <identifier kind='argument' isDefinition='true' number='0'>x</identifier>
                      <symbol>,</symbol>
                      <identifier kind='class' isDefinition='false'>Game</identifier>
                      <identifier kind='argument' isDefinition='true' number='1' classType='Game'>game</identifier>
                    </parameterList>
                    <symbol>)</symbol>
                    <subroutineBody>
                      <symbol>{</symbol>
                      <symbol>}</symbol>
                    </subroutineBody>
                  </subroutineDec>
            ");
        }

        [TestMethod]
        public void ShouldHandleVariableDeclarations()
        {
            classUnderTest
                .LoadTokens(sd1, sd2, sd3, sd4, sd5, sd6, sd7, sd8, sd9, sd10, sd11, vd1, vd2, vd3, vd4, vd5, vd6, vd7, vd8, vd9, vd10, sd12)
                .ParseSubroutineDeclaration()
                .ShouldGenerateXml(@"
                      <subroutineDec>
                        <keyword>constructor</keyword>
                        <keyword>void</keyword>
                        <identifier kind='subroutine' isDefinition='true'>doSomething</identifier>
                        <symbol>(</symbol>
                        <parameterList>
                          <keyword>int</keyword>
                          <identifier kind='argument' isDefinition='true' number='0'>x</identifier>
                          <symbol>,</symbol>
                          <identifier kind='class' isDefinition='false'>Game</identifier>
                          <identifier kind='argument' isDefinition='true' number='1' classType='Game'>game</identifier>
                        </parameterList>
                        <symbol>)</symbol>
                        <subroutineBody>
                          <symbol>{</symbol>
                          <varDec>
                            <keyword>var</keyword>
                            <keyword>boolean</keyword>
                            <identifier kind='var' isDefinition='true' number='0'>hasStarted</identifier>
                            <symbol>,</symbol>
                            <identifier kind='var' isDefinition='true' number='1'>hasFinished</identifier>
                            <symbol>;</symbol>
                          </varDec>
                          <varDec>
                            <keyword>var</keyword>
                            <identifier kind='class' isDefinition='false'>Player</identifier>
                            <identifier kind='var' isDefinition='true' number='2' classType='Player'>player</identifier>
                            <symbol>;</symbol>
                          </varDec>
                          <symbol>}</symbol>
                        </subroutineBody>
                      </subroutineDec>
            ");
        }
    }

    #endregion

    #region MultipleSubroutineGrammar

    [TestClass]
    public class MultipleSubroutineGrammar
    {
        private Grammarian classUnderTest;
        private Token[] tokens;
        //private Token cd1, cd2, cd3, cd4;
        //private Token sda1, sda2, sda3, sda4, sdaa1, sdaa2, sda5, sda6, sda7, sda8, sda9, sda10, sda11;
        //private Token sdb1, sdb2, sdb3, sdb4, sdba1, sdba2, sdb5, sdb6, sdb7, sdb8, sdb9, sdb10, sdb11;

        [TestInitialize]
        public void Setup()
        {
            tokens = new[] {
                new Token(NodeType.Keyword, "class"),
                new Token(NodeType.Identifier, "MyClass"),
                new Token(NodeType.Symbol, "{"),
                new Token(NodeType.Keyword, "function"),
                new Token(NodeType.Keyword, "void"),
                new Token(NodeType.Identifier, "doSomething"),
                new Token(NodeType.Symbol, "("),
                new Token(NodeType.Keyword, "int"),
                new Token(NodeType.Identifier, "x"),
                new Token(NodeType.Symbol, ")"),
                new Token(NodeType.Symbol, "{"),
                new Token(NodeType.Keyword, "var"),
                new Token(NodeType.Keyword, "boolean"),
                new Token(NodeType.Identifier, "hasStarted"),
                new Token(NodeType.Symbol, ";"),
                new Token(NodeType.Symbol, "}"),
                new Token(NodeType.Keyword, "function"),
                new Token(NodeType.Keyword, "void"),
                new Token(NodeType.Identifier, "doSomethingElse"),
                new Token(NodeType.Symbol, "("),
                new Token(NodeType.Keyword, "int"),
                new Token(NodeType.Identifier, "x"),
                new Token(NodeType.Symbol, ")"),
                new Token(NodeType.Symbol, "{"),
                new Token(NodeType.Keyword, "var"),
                new Token(NodeType.Keyword, "boolean"),
                new Token(NodeType.Identifier, "hasStarted"),
                new Token(NodeType.Symbol, ";"),
                new Token(NodeType.Symbol, "}"),
                new Token(NodeType.Symbol, "}")
            };
            classUnderTest = new Grammarian();
        }

        [TestMethod]
        public void ShouldResetSubroutineSymbolTable()
        {
            classUnderTest
                .LoadTokens(tokens)
                .ParseClass()
                .ShouldGenerateXml(@"
                    <class>
                        <keyword>class</keyword>
                        <identifier kind='class' isDefinition='true'>MyClass</identifier>
                        <symbol>{</symbol>
                        <subroutineDec>
                            <keyword>function</keyword>
                            <keyword>void</keyword>
                            <identifier kind='subroutine' isDefinition='true'>doSomething</identifier>
                            <symbol>(</symbol>
                            <parameterList>
                                <keyword>int</keyword>
                                <identifier kind='argument' number='0' isDefinition='true'>x</identifier>
                            </parameterList>
                            <symbol>)</symbol>
                            <subroutineBody>
                                <symbol>{</symbol>
                                <varDec>
                                    <keyword>var</keyword>
                                    <keyword>boolean</keyword>
                                    <identifier kind='var' number='0' isDefinition='true'>hasStarted</identifier>
                                    <symbol>;</symbol>
                                </varDec>
                                <symbol>}</symbol>
                            </subroutineBody>
                        </subroutineDec>
                        <subroutineDec>
                            <keyword>function</keyword>
                            <keyword>void</keyword>
                            <identifier kind='subroutine' isDefinition='true'>doSomethingElse</identifier>
                            <symbol>(</symbol>
                            <parameterList>
                                <keyword>int</keyword>
                                <identifier kind='argument' number='0' isDefinition='true'>x</identifier>
                            </parameterList>
                            <symbol>)</symbol>
                            <subroutineBody>
                                <symbol>{</symbol>
                                <varDec>
                                    <keyword>var</keyword>
                                    <keyword>boolean</keyword>
                                    <identifier kind='var' number='0' isDefinition='true'>hasStarted</identifier>
                                    <symbol>;</symbol>
                                </varDec>
                                <symbol>}</symbol>
                            </subroutineBody>
                        </subroutineDec>
                        <symbol>}</symbol>
                    </class>
                ");
        }
    }

    #endregion

    #region SimpleLetStatementGrammar

    [TestClass]
    public class SimpleLetStatementGrammar
    {
        private Grammarian classUnderTest;

        [TestInitialize]
        public void Setup()
        {
            classUnderTest = new Grammarian();
            classUnderTest.LoadTokens(
                new Token(NodeType.Keyword, "let"),
                new Token(NodeType.Identifier, "x"),
                new Token(NodeType.Symbol, "="),
                new Token(NodeType.IntegerConstant, "1234"),
                new Token(NodeType.Symbol, ";")
            );
            classUnderTest.AddToSymbolTable("x", IdentifierKind.Var, null);
        }

        [TestMethod]
        public void RecognisesLetStatementWithSimpleExpression()
        {
            classUnderTest.ParseLetStatement().ShouldGenerateXml(@"
                <letStatement>
                    <keyword>let</keyword>
                    <identifier kind='var' isDefinition='false' number='0'>x</identifier>
                    <symbol>=</symbol>
                    <expression>
                    <term>
                        <integerConstant>1234</integerConstant>
                    </term>
                    </expression>
                    <symbol>;</symbol>
                </letStatement>
            ");
        }
    }

    #endregion

    #region ComplexLetStatementGrammar

    [TestClass]
    public class ComplexLetStatementGrammar
    {
        private Grammarian classUnderTest;

        [TestInitialize]
        public void Setup()
        {
            classUnderTest = new Grammarian();
            classUnderTest.LoadTokens(
                new Token(NodeType.Keyword, "let"),
                new Token(NodeType.Identifier, "y"),
                new Token(NodeType.Symbol, "["),
                new Token(NodeType.Identifier, "x"),
                new Token(NodeType.Symbol, "+"),
                new Token(NodeType.IntegerConstant, "1"),
                new Token(NodeType.Symbol, "]"),
                new Token(NodeType.Symbol, "="),
                new Token(NodeType.Symbol, "~"),
                new Token(NodeType.Identifier, "finished"),
                new Token(NodeType.Symbol, ";")
            );
        }

        [TestMethod]
        public void RecognisesLetStatementWithMoreComplexExpression()
        {
            classUnderTest.AddToSymbolTable("y", IdentifierKind.Var, null);
            classUnderTest.AddToSymbolTable("x", IdentifierKind.Field, null);
            classUnderTest.AddToSymbolTable("finished", IdentifierKind.Field, null);
            classUnderTest.ParseLetStatement()
                .ShouldGenerateXml(@"
                            <letStatement>
                              <keyword>let</keyword>
                              <identifier kind='var' isDefinition='false' number='0'>y</identifier>
                              <symbol>[</symbol>
                              <expression>
                                <term>
                                  <identifier kind='field' isDefinition='false' number='0'>x</identifier>
                                </term>
                                <symbol>+</symbol>
                                <term>
                                  <integerConstant>1</integerConstant>
                                </term>
                              </expression>
                              <symbol>]</symbol>
                              <symbol>=</symbol>
                              <expression>
                                <term>
                                  <symbol>~</symbol>
                                  <term>
                                    <identifier kind='field' isDefinition='false' number='1'>finished</identifier>
                                  </term>
                                </term>
                              </expression>
                              <symbol>;</symbol>
                            </letStatement>
                ");
        }
    }

    #endregion

    #region ReturnStatementGrammar

    [TestClass]
    public class ReturnStatementGrammar
    {
        private Grammarian classUnderTest;
        private Token t1, t2, t3;

        [TestInitialize]
        public void Setup()
        {
            classUnderTest = new Grammarian();
            t1 = new Token(NodeType.Keyword, "return");
            t2 = new Token(NodeType.Identifier, "result");
            t3 = new Token(NodeType.Symbol, ";");
        }

        [TestMethod]
        public void ShouldParseCorrectlyWithNoReturnValue()
        {
            classUnderTest.LoadTokens(t1, t3)
                .ParseReturnStatement()
                .ShouldGenerateXml(@"
                    <returnStatement>
                        <keyword>return</keyword>
                        <symbol>;</symbol>
                    </returnStatement>
                ");
        }

        [TestMethod]
        public void ShouldParseCorrectlyWithReturnExpression()
        {
            classUnderTest.LoadTokens(t1, t2, t3);
            classUnderTest.AddToSymbolTable("result", IdentifierKind.Var, "ResultClass");
            classUnderTest
                .ParseReturnStatement()
                .ShouldGenerateXml(@"
                    <returnStatement>
                        <keyword>return</keyword>
                        <expression>
                            <term>
                                <identifier kind='var' isDefinition='false' number='0' classType='ResultClass'>result</identifier>
                            </term>
                        </expression>
                        <symbol>;</symbol>
                    </returnStatement>
                ");
        }
    }

    #endregion

    #region IfStatementGrammar

    [TestClass]
    public class IfStatementGrammar
    {
        private Grammarian classUnderTest;
        private Token t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16, t17, t18, t19, t20;

        [TestInitialize]
        public void Setup()
        {
            classUnderTest = new Grammarian();
            t1 = new Token(NodeType.Keyword, "if");
            t2 = new Token(NodeType.Symbol, "(");
            t3 = new Token(NodeType.Symbol, "(");
            t4 = new Token(NodeType.Identifier, "x");
            t5 = new Token(NodeType.Symbol, "*");
            t6 = new Token(NodeType.IntegerConstant, "5");
            t7 = new Token(NodeType.Symbol, ")");
            t8 = new Token(NodeType.Symbol, ">");
            t9 = new Token(NodeType.IntegerConstant, "30");
            t10 = new Token(NodeType.Symbol, ")");
            t11 = new Token(NodeType.Symbol, "{");
            t12 = new Token(NodeType.Keyword, "return");
            t13 = new Token(NodeType.Symbol, ";");
            t14 = new Token(NodeType.Symbol, "}");
            t15 = new Token(NodeType.Keyword, "else");
            t16 = new Token(NodeType.Symbol, "{");
            t17 = new Token(NodeType.Keyword, "return");
            t18 = new Token(NodeType.Identifier, "result");
            t19 = new Token(NodeType.Symbol, ";");
            t20 = new Token(NodeType.Symbol, "}");
        }

        [TestMethod]
        public void ParsesCorrectlyWithoutAnElseBlock()
        {
            classUnderTest.LoadTokens(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14);
            classUnderTest.AddToSymbolTable("x", IdentifierKind.Static, null);
            classUnderTest.ParseIfStatement().ShouldGenerateXml(@"
                <ifStatement>
                  <keyword>if</keyword>
                  <symbol>(</symbol>
                  <expression>
                    <term>
                        <symbol>(</symbol>
                        <expression>
                            <term>
                                <identifier kind='static' isDefinition='false' number='0'>x</identifier>
                            </term>
                            <symbol>*</symbol>
                            <term>
                                <integerConstant>5</integerConstant>
                            </term>
                        </expression>
                        <symbol>)</symbol>
                    </term>
                    <symbol>&gt;</symbol>
                    <term>
                        <integerConstant>30</integerConstant>
                    </term>
                </expression>
                <symbol>)</symbol>
                <symbol>{</symbol>
                <statements>
                    <returnStatement>
                      <keyword>return</keyword>
                      <symbol>;</symbol>
                    </returnStatement>
                </statements>
                <symbol>}</symbol>
              </ifStatement>
            ");
        }

        [TestMethod]
        public void ParsesCorrectlyWithAnElseBlock()
        {
            classUnderTest.LoadTokens(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16, t17, t18, t19, t20);
            classUnderTest.AddToSymbolTable("x", IdentifierKind.Static, null);
            classUnderTest.AddToSymbolTable("result", IdentifierKind.Var, "ResultClass");
            classUnderTest.ParseIfStatement().ShouldGenerateXml(@"
                <ifStatement>
                  <keyword>if</keyword>
                  <symbol>(</symbol>
                  <expression>
                    <term>
                        <symbol>(</symbol>
                        <expression>
                            <term>
                                <identifier kind='static' isDefinition='false' number='0'>x</identifier>
                            </term>
                            <symbol>*</symbol>
                            <term>
                                <integerConstant>5</integerConstant>
                            </term>
                        </expression>
                        <symbol>)</symbol>
                    </term>
                    <symbol>&gt;</symbol>
                    <term>
                        <integerConstant>30</integerConstant>
                    </term>
                </expression>
                <symbol>)</symbol>
                <symbol>{</symbol>
                <statements>
                    <returnStatement>
                      <keyword>return</keyword>
                      <symbol>;</symbol>
                    </returnStatement>
                </statements>
                <symbol>}</symbol>
                <keyword>else</keyword>
                <symbol>{</symbol>
                <statements>
                    <returnStatement>
                        <keyword>return</keyword>
                        <expression>
                            <term>
                                <identifier kind='var' isDefinition='false' number='0' classType='ResultClass'>result</identifier>
                            </term>
                        </expression>
                        <symbol>;</symbol>
                    </returnStatement>
                </statements>
                <symbol>}</symbol>
              </ifStatement>
            ");
        }
    }

    #endregion

    #region WhileStatementGrammar

    [TestClass]
    public class WhileStatementGrammar
    {
        private Grammarian classUnderTest;

        [TestInitialize]
        public void Setup()
        {
            classUnderTest = new Grammarian();
            classUnderTest.LoadTokens(
                new Token(NodeType.Keyword, "while"),
                new Token(NodeType.Symbol, "("),
                new Token(NodeType.Identifier, "inProgress"),
                new Token(NodeType.Symbol, ")"),
                new Token(NodeType.Symbol, "{"),
                new Token(NodeType.Keyword, "let"),
                new Token(NodeType.Identifier, "x"),
                new Token(NodeType.Symbol, "="),
                new Token(NodeType.Identifier, "y"),
                new Token(NodeType.Symbol, ";"),
                new Token(NodeType.Symbol, "}")
            );
            classUnderTest.AddToSymbolTable("x", IdentifierKind.Var, null);
            classUnderTest.AddToSymbolTable("y", IdentifierKind.Var, null);
            classUnderTest.AddToSymbolTable("inProgress", IdentifierKind.Field, null);
        }

        [TestMethod]
        public void ParsesCorrectly()
        {
            classUnderTest.ParseWhileStatement().ShouldGenerateXml(@"
                <whileStatement>
                  <keyword>while</keyword>
                  <symbol>(</symbol>
                  <expression>
                    <term>
                        <identifier kind='field' isDefinition='false' number='0'>inProgress</identifier>
                    </term>
                  </expression>
                <symbol>)</symbol>
                <symbol>{</symbol>
                <statements>
                    <letStatement>
                        <keyword>let</keyword>
                        <identifier kind='var' isDefinition='false' number='0'>x</identifier>
                        <symbol>=</symbol>
                        <expression>
                            <term>
                                <identifier kind='var' isDefinition='false' number='1'>y</identifier>
                            </term>
                        </expression>
                        <symbol>;</symbol>
                    </letStatement>
                </statements>
                <symbol>}</symbol>
              </whileStatement>
            ");
        }
    }

    #endregion

    #region SimpleSubroutineCallGrammar

    [TestClass]
    public class SimpleSubroutineCallGrammar
    {
        private Grammarian classUnderTest;

        [TestInitialize]
        public void Setup()
        {
            classUnderTest = new Grammarian();
            classUnderTest.LoadTokens(
                new Token(NodeType.Identifier, "subroutineName"),
                new Token(NodeType.Symbol, "("),
                new Token(NodeType.IntegerConstant, "5"),
                new Token(NodeType.Symbol, ","),
                new Token(NodeType.StringConstant, "rhubarb"),
                new Token(NodeType.Symbol, ")")
            );
        }

        [TestMethod]
        public void ParsesCorrectly()
        {
            classUnderTest.ParseExpression().ShouldGenerateXml(@"
                <expression>
                    <term>
                        <identifier kind='subroutine' isDefinition='false'>subroutineName</identifier>
                        <symbol>(</symbol>
                        <expressionList>
                            <expression>
                                <term>
                                    <integerConstant>5</integerConstant>
                                </term>
                            </expression>
                            <symbol>,</symbol>
                            <expression>
                                <term>
                                    <stringConstant>rhubarb</stringConstant>
                                </term>
                            </expression>
                        </expressionList>
                        <symbol>)</symbol>
                    </term>
                </expression>
            ");
        }
    }

    #endregion

    #region SubroutineCallGrammarWithPrefix

    [TestClass]
    public class SubroutineCallGrammarWithPrefix
    {
        private Grammarian classUnderTest;

        [TestInitialize]
        public void Setup()
        {
            classUnderTest = new Grammarian();
            classUnderTest.LoadTokens(
                new Token(NodeType.Identifier, "myClass"),
                new Token(NodeType.Symbol, "."),
                new Token(NodeType.Identifier, "doSomething"),
                new Token(NodeType.Symbol, "("),
                new Token(NodeType.IntegerConstant, "5"),
                new Token(NodeType.Symbol, ")")
            );
        }

        [TestMethod]
        public void ParsesCorrectly()
        {
            classUnderTest.ParseExpression().ShouldGenerateXml(@"
                <expression>
                    <term>
                        <identifier kind='class' isDefinition='false'>myClass</identifier>
                        <symbol>.</symbol>
                        <identifier kind='subroutine' isDefinition='false'>doSomething</identifier>
                        <symbol>(</symbol>
                        <expressionList>
                            <expression>
                                <term>
                                    <integerConstant>5</integerConstant>
                                </term>
                            </expression>
                        </expressionList>
                        <symbol>)</symbol>
                    </term>
                </expression>
            ");
        }
    }

    #endregion

    #region SimpleDoStatementGrammar

    [TestClass]
    public class SimpleDoStatementGrammar
    {
        private Grammarian classUnderTest;

        [TestInitialize]
        public void Setup()
        {
            classUnderTest = new Grammarian();
            classUnderTest.LoadTokens(
                new Token(NodeType.Keyword, "do"),
                new Token(NodeType.Identifier, "something"),
                new Token(NodeType.Symbol, "("),
                new Token(NodeType.Identifier, "x"),
                new Token(NodeType.Symbol, ")"),
                new Token(NodeType.Symbol, ";")
            );
            classUnderTest.AddToSymbolTable("x", IdentifierKind.Var, null);
        }

        [TestMethod]
        public void ParsesCorrectly()
        {
            classUnderTest.ParseDoStatement().ShouldGenerateXml(@"
                <doStatement>
                    <keyword>do</keyword>
                    <identifier kind='subroutine' isDefinition='false'>something</identifier>
                    <symbol>(</symbol>
                    <expressionList>
                        <expression>
                            <term>
                                <identifier kind='var' isDefinition='false' number='0'>x</identifier>
                            </term>
                        </expression>
                    </expressionList>
                    <symbol>)</symbol>
                    <symbol>;</symbol>
                </doStatement>
            ");
        }
    }

    #endregion

    #region MoreComplexDoStatementGrammar

    [TestClass]
    public class MoreComplexDoStatementGrammar
    {
        private Grammarian classUnderTest;

        [TestInitialize]
        public void Setup()
        {
            classUnderTest = new Grammarian();
            classUnderTest.LoadTokens(
                new Token(NodeType.Keyword, "do"),
                new Token(NodeType.Identifier, "myClass"),
                new Token(NodeType.Symbol, "."),
                new Token(NodeType.Identifier, "something"),
                new Token(NodeType.Symbol, "("),
                new Token(NodeType.IntegerConstant, "5"),
                new Token(NodeType.Symbol, "+"),
                new Token(NodeType.IntegerConstant, "3"),
                new Token(NodeType.Symbol, ","),
                new Token(NodeType.Identifier, "blah"),
                new Token(NodeType.Symbol, ")"),
                new Token(NodeType.Symbol, ";")
            );
            classUnderTest.AddToSymbolTable("blah", IdentifierKind.Argument, null);
            classUnderTest.AddToSymbolTable("myClass", IdentifierKind.Field, "MyClass");
        }

        [TestMethod]
        public void ParsesCorrectly()
        {
            classUnderTest.ParseDoStatement().ShouldGenerateXml(@"
                <doStatement>
                    <keyword>do</keyword>
                    <identifier kind='field' isDefinition='false' number='0' classType='MyClass'>myClass</identifier>
                    <symbol>.</symbol>
                    <identifier kind='subroutine' isDefinition='false'>something</identifier>
                    <symbol>(</symbol>
                    <expressionList>
                        <expression>
                            <term>
                                <integerConstant>5</integerConstant>
                            </term>
                            <symbol>+</symbol>
                            <term>
                                <integerConstant>3</integerConstant>
                            </term>
                        </expression>
                        <symbol>,</symbol>
                        <expression>
                            <term>
                                <identifier kind='argument' isDefinition='false' number='0'>blah</identifier>
                            </term>
                        </expression>
                    </expressionList>
                    <symbol>)</symbol>
                    <symbol>;</symbol>
                </doStatement>
                ");
        }
    }

    #endregion
}
