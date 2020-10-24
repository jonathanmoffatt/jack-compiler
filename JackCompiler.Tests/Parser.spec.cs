using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;

namespace JackCompiler.Tests
{
    #region WhenRunningTheParser

    [TestClass]
    public class WhenRunningTheParser
    {
        private AutoMocker mocker;
        private Parser classUnderTest;
        private Mock<ITokeniser> tokeniser;

        [TestInitialize]
        public void Setup()
        {
            mocker = new AutoMocker();
            classUnderTest = mocker.CreateInstance<Parser>();
            tokeniser = new Mock<ITokeniser>();
        }

        [TestMethod]
        public void DoesNothingIfTheFirstTokenIsNull()
        {
            tokeniser.Setup(t => t.GetNextToken()).Returns(() => null);
            classUnderTest.Parse(tokeniser.Object);
            classUnderTest.Tokens.Should().BeEmpty();
        }

        [TestMethod]
        public void GetsTokensUntilThereAreNoMore()
        {
            tokeniser
                .SetupSequence(t => t.GetNextToken())
                .Returns(new Token(NodeType.Keyword, "class"))
                .Returns(new Token(NodeType.Identifier, "blah"))
                .Returns(new Token(NodeType.Symbol, "{"))
                .Returns(new Token(NodeType.Symbol, "}"))
                .Returns(() => null);
            classUnderTest.Parse(tokeniser.Object);
            classUnderTest.Tokens.Should().HaveCount(4);
        }

        [TestMethod]
        public void PassesTokensToTheGrammarian()
        {
            Token t1 = new Token(NodeType.Keyword, "class");
            Token t2 = new Token(NodeType.Identifier, "blah");
            Token t3 = new Token(NodeType.Symbol, "{");
            Token t4 = new Token(NodeType.Symbol, "}");
            tokeniser
                .SetupSequence(t => t.GetNextToken())
                .Returns(t1)
                .Returns(t2)
                .Returns(t3)
                .Returns(t4)
                .Returns(() => null);
            Node classNode = new Node(NodeType.Class);
            mocker
                .GetMock<IGrammarian>()
                .Setup(f => f.ParseClass())
                .Returns(classNode);
            classUnderTest.Parse(tokeniser.Object);
            classUnderTest.Tree.Should().Be(classNode);
            mocker
                .GetMock<IGrammarian>()
                .Verify(f => f.LoadTokens(It.Is<Queue<Token>>(q =>
                    q.Contains(t1) &&
                    q.Contains(t2) &&
                    q.Contains(t3) &&
                    q.Contains(t4)
                )));
        }
    }

    #endregion
}
