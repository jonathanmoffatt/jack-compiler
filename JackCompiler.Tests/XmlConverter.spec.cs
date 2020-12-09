using System.Xml;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JackCompiler.Tests
{
    #region ConvertingTokensToXml

    [TestClass]
    public class ConvertingTokensToXml
    {
        private XmlConverter classUnderTest;
        private Token t1;
        private Token t2;

        [TestInitialize]
        public void Setup()
        {
            classUnderTest = new XmlConverter();
            t1 = new Token(NodeType.IntegerConstant, "100");
            t2 = new Token(NodeType.Identifier, "x");
        }

        [TestMethod]
        public void ReturnsXmlDocument()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<tokens><integerConstant>100</integerConstant><identifier>x</identifier></tokens>");
            classUnderTest.ConvertTokens(t1, t2).Should().BeEquivalentTo(doc);
        }
    }

    #endregion

    #region ConvertingIdentifierToXml

    [TestClass]
    public class ConvertingIdentifierToXml
    {
        private XmlConverter classUnderTest;

        [TestInitialize]
        public void Setup()
        {
            classUnderTest = new XmlConverter();
        }

        [TestMethod]
        public void IncludesTokenInformationInTheXml()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<tokens><identifier kind='argument' number='3' isDefinition='true'>x</identifier></tokens>");
            var identifier = new Identifier("x", IdentifierKind.Argument, true, 3);
            classUnderTest.ConvertTokens(identifier).Should().BeEquivalentTo(doc);
        }

        [TestMethod]
        public void LeavesOutNumberIfThereIsntOne()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<tokens><identifier kind='argument' isDefinition='false'>x</identifier></tokens>");
            var identifier = new Identifier("x", IdentifierKind.Argument, false);
            classUnderTest.ConvertTokens(identifier).Should().BeEquivalentTo(doc);
        }

        [TestMethod]
        public void IncludesClassNameInTheXml()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<tokens><identifier kind='argument' number='3' isDefinition='true' classType='HorizontalLocation'>x</identifier></tokens>");
            var identifier = new Identifier("x", IdentifierKind.Argument, true, 3);
            identifier.ClassType = "HorizontalLocation";
            classUnderTest.ConvertTokens(identifier).Should().BeEquivalentTo(doc);
        }
    }

    #endregion

    #region ConvertingTreeToXml

    [TestClass]
    public class ConvertingTreeToXml
    {
        private XmlConverter classUnderText;
        private Node node;

        [TestInitialize]
        public void Setup()
        {
            classUnderText = new XmlConverter();
            node = new Node(NodeType.Class);
            node.AddChild(new Token(NodeType.Keyword, "class"));
            Node subDec = new Node(NodeType.SubroutineDeclaration);
            node.AddChild(subDec);
            subDec.AddChild(new Token(NodeType.Keyword, "function"));
        }

        [TestMethod]
        public void ReturnsExpectedXml()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(@"
                <class>
                    <keyword>class</keyword>
                    <subroutineDec>
                        <keyword>function</keyword>
                    </subroutineDec>
                </class>");
            classUnderText.ConvertNode(node).Should().BeEquivalentTo(doc);
        }

        [TestMethod]
        public void IncludesLineBreakInEmptyElements()
        {
            // to keep the stupid grader happy
            classUnderText.ConvertNode(new Node(NodeType.Class)).OuterXml.Should().Be("<class>\n</class>");
        }
    }

    #endregion
}
