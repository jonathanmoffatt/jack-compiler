using System.IO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JackCompiler.Tests
{
    #region WhenCompiling

    [TestClass]
    public class WhenCompiling
    {
        private StringWriter output;
        private VmCompiler classUnderTest;

        [TestInitialize]
        public void Setup()
        {
            output = new StringWriter();
            classUnderTest = new VmCompiler(new Node(NodeType.Class), output);
        }

        [TestMethod]
        public void WritesToTheProvidedStream()
        {
            classUnderTest.Compile();
            output.ToString().Should().Be("hello world\n");
        }
    }

    #endregion
}
