using System.IO;
using System.Text;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JackCompiler.Tests
{
    #region WhenWorkingWithTheTokeniser

    [TestClass]
    public class WhenWorkingWithTheTokeniser
    {
        [TestMethod]
        public void RecognisesAKeywordToken()
        {
            using var t = new Tokeniser("class");
            t.GetNextToken().Type.Should().Be(NodeType.Keyword);
        }

        [TestMethod]
        public void RecognisesTheClassKeyword()
        {
            using var t = new Tokeniser("class");
            t.GetNextToken().Value.Should().Be("class");
        }

        [TestMethod]
        public void WalksThroughTheInput()
        {
            using var t = new Tokeniser("static field let");
            t.GetNextToken().Value.Should().Be("static");
            t.GetNextToken().Value.Should().Be("field");
            t.GetNextToken().Value.Should().Be("let");
        }

        [TestMethod]
        public void SkipsOverWhitespace()
        {
            using var t = new Tokeniser("    static     field\nlet   \r\n\tclass");
            t.GetNextToken().Value.Should().Be("static");
            t.GetNextToken().Value.Should().Be("field");
            t.GetNextToken().Value.Should().Be("let");
            t.GetNextToken().Value.Should().Be("class");
        }

        [TestMethod]
        public void NextTokenIsNullIfAtEnd()
        {
            using var t = new Tokeniser("class");
            t.GetNextToken();
            t.GetNextToken().Should().BeNull();
        }

        [TestMethod]
        public void IgnoresWhitespaceWhenCheckingIfAtEnd()
        {
            using var t = new Tokeniser("class\n\t\t  \n ");
            t.GetNextToken();
            t.GetNextToken().Should().BeNull();
        }

        [TestMethod]
        public void IsImmediatelyAtEndIfInputIsEmpty()
        {
            using var t = new Tokeniser("");
            t.GetNextToken().Should().BeNull();
        }

        [TestMethod]
        public void IsImmediatelyAtEndIfInputIsAllWhitespace()
        {
            using var t = new Tokeniser("    \n   \t  \r\n   ");
            t.GetNextToken().Should().BeNull();
        }

        [TestMethod]
        public void WorksWhenPassedAStream()
        {
            byte[] bytes = Encoding.UTF8.GetBytes("static field\n");
            using var stream = new MemoryStream(bytes);
            using var t = new Tokeniser(stream);
            t.GetNextToken().Value.Should().Be("static");
            t.GetNextToken().Value.Should().Be("field");
            t.GetNextToken().Should().BeNull();
        }

        [TestMethod]
        public void RecognisesAllKeywords()
        {
            using var t = new Tokeniser("class constructor function method field static var int char boolean void true false null this let do if else while return");
            Token token = t.GetNextToken();
            while (token != null)
            {
                token.Type.Should().Be(NodeType.Keyword);
                token = t.GetNextToken();
            }
        }

        [TestMethod]
        public void RecognisesAllSymbols()
        {
            using var t = new Tokeniser("{ } ( ) [ ] . , ; + - * / & | < > = ~");
            Token token = t.GetNextToken();
            while (token != null)
            {
                token.Type.Should().Be(NodeType.Symbol);
                token = t.GetNextToken();
            }
        }

        [TestMethod]
        public void RecognisesIntegerConstants()
        {
            using var t = new Tokeniser("12345");
            Token token = t.GetNextToken();
            token.Type.Should().Be(NodeType.IntegerConstant);
            token.Value.Should().Be("12345");
        }

        [TestMethod]
        public void RejectsIntegersContainingStrings()
        {
            using var t = new Tokeniser("123four5");
            Token token = t.GetNextToken();
            token.Type.Should().NotBe(NodeType.IntegerConstant);
        }

        [TestMethod]
        public void RecognisesStringConstants()
        {
            using var t = new Tokeniser("\"the quick brown fox, jumps over 23 ______ (of something)\"");
            Token token = t.GetNextToken();
            token.Type.Should().Be(NodeType.StringConstant);
            token.Value.Should().Be("the quick brown fox, jumps over 23 ______ (of something)");
        }

        [TestMethod]
        public void RecognisesStraightforwardIdentifiers()
        {
            using var t = new Tokeniser("counter");
            Token token = t.GetNextToken();
            token.Type.Should().Be(NodeType.Identifier);
            token.Value.Should().Be("counter");
        }

        [TestMethod]
        public void RecognisesMoreComplexIdentifiers()
        {
            using var t = new Tokeniser("first_3_entries");
            Token token = t.GetNextToken();
            token.Type.Should().Be(NodeType.Identifier);
            token.Value.Should().Be("first_3_entries");
        }

        [TestMethod]
        public void IgnoresLineComments()
        {
            using var t = new Tokeniser("// this is a comment\nreturn");
            t.GetNextToken().Value.Should().Be("return");
            t.GetNextToken().Should().BeNull();
        }

        [TestMethod]
        public void IgnoresLineCommentsAtTheEndOfLines()
        {
            using var t = new Tokeniser("return;  // this is a comment\n}");
            t.GetNextToken().Value.Should().Be("return");
            t.GetNextToken().Value.Should().Be(";");
            t.GetNextToken().Value.Should().Be("}");
            t.GetNextToken().Should().BeNull();
        }

        [TestMethod]
        public void DoesNotGetFooledByDoubleSlashesInStringLiterals()
        {
            using var t = new Tokeniser("return \"// this is not a comment\";\n");
            t.GetNextToken().Value.Should().Be("return");
            t.GetNextToken().Value.Should().Be("// this is not a comment");
            t.GetNextToken().Value.Should().Be(";");
            t.GetNextToken().Should().BeNull();
        }


        [TestMethod]
        public void IgnoresBlockComments()
        {
            using var t = new Tokeniser("/* get outta\nhere\n*/\nreturn");
            t.GetNextToken().Value.Should().Be("return");
            t.GetNextToken().Should().BeNull();
        }
        [TestMethod]
        public void IgnoresBlockCommentsWithinALine()
        {
            using var t = new Tokeniser("/* get outta here */ return");
            t.GetNextToken().Value.Should().Be("return");
            t.GetNextToken().Should().BeNull();
        }

        [TestMethod]
        public void IgnoresBlockCommentsAcrossMultipleLines()
        {
            using var t = new Tokeniser("/* get outta\nhere */\nreturn");
            t.GetNextToken().Value.Should().Be("return");
            t.GetNextToken().Should().BeNull();
        }

        [TestMethod]
        public void HandlesCombinationsOfTypes()
        {
            using var t = new Tokeniser("let first_3_entries=\"three\";let x=x+1;\nreturn x;\n");

            Token token = t.GetNextToken();
            token.Value.Should().Be("let");
            token.Type.Should().Be(NodeType.Keyword);

            token = t.GetNextToken();
            token.Value.Should().Be("first_3_entries");
            token.Type.Should().Be(NodeType.Identifier);

            token = t.GetNextToken();
            token.Value.Should().Be("=");
            token.Type.Should().Be(NodeType.Symbol);

            token = t.GetNextToken();
            token.Value.Should().Be("three");
            token.Type.Should().Be(NodeType.StringConstant);

            token = t.GetNextToken();
            token.Value.Should().Be(";");
            token.Type.Should().Be(NodeType.Symbol);

            token = t.GetNextToken();
            token.Value.Should().Be("let");
            token.Type.Should().Be(NodeType.Keyword);

            token = t.GetNextToken();
            token.Value.Should().Be("x");
            token.Type.Should().Be(NodeType.Identifier);

            token = t.GetNextToken();
            token.Value.Should().Be("=");
            token.Type.Should().Be(NodeType.Symbol);

            token = t.GetNextToken();
            token.Value.Should().Be("x");
            token.Type.Should().Be(NodeType.Identifier);

            token = t.GetNextToken();
            token.Value.Should().Be("+");
            token.Type.Should().Be(NodeType.Symbol);

            token = t.GetNextToken();
            token.Value.Should().Be("1");
            token.Type.Should().Be(NodeType.IntegerConstant);

            token = t.GetNextToken();
            token.Value.Should().Be(";");
            token.Type.Should().Be(NodeType.Symbol);

            token = t.GetNextToken();
            token.Value.Should().Be("return");
            token.Type.Should().Be(NodeType.Keyword);

            token = t.GetNextToken();
            token.Value.Should().Be("x");
            token.Type.Should().Be(NodeType.Identifier);

            token = t.GetNextToken();
            token.Value.Should().Be(";");
            token.Type.Should().Be(NodeType.Symbol);

            t.GetNextToken().Should().BeNull();
        }
    }

    #endregion
}
