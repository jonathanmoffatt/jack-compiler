using System.IO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JackCompiler.Tests
{
    internal static class VmCompilerTestingExtensions
    {
        public static string CompilingJackCode(this VmCompiler vmCompiler, string jackCode)
        {
            using (var tokeniser = new Tokeniser(jackCode))
            {
                var output = new StringWriter();
                var parser = new Parser(new Grammarian());
                parser.Parse(tokeniser);
                vmCompiler.Compile(parser.Tree, output);
                return output.ToString();
            }

        }
    }
    #region WhenCompiling

    [TestClass]
    public class WhenCompiling
    {
        private VmCompiler classUnderTest;

        [TestInitialize]
        public void Setup()
        {
            classUnderTest = new VmCompiler();
        }

        [TestMethod]
        public void WritesToTheProvidedStream()
        {
            classUnderTest.CompilingJackCode("class something {}").Should().Be("hello world\n");
        }
    }

    #endregion
}
