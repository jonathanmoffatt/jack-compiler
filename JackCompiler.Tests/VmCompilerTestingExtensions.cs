using System;
using System.IO;

namespace JackCompiler.Tests
{
    internal static class VmCompilerTestingExtensions
    {
        public static string CompilingJackCode(this VmCompiler vmCompiler, string jackCode)
        {
            using (var tokeniser = new Tokeniser(jackCode))
            {
                var output = new StringWriter();
                var writer = new VmWriter(output);
                var parser = new Parser(new Grammarian());
                parser.Parse(tokeniser);
                var xmlConverter = new XmlConverter();
                Console.WriteLine(xmlConverter.ConvertNode(parser.Tree).OuterXml);
                vmCompiler.Compile(parser.Tree, writer);
                return writer.ToString();
            }

        }
    }
}
