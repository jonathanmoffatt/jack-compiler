using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace JackCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!Initialise(args))
                return;
            string sourceFileOrDirectory = args[0];
            bool isSingleFile = File.Exists(sourceFileOrDirectory);

            string[] sourceFiles = GetSourceFiles(sourceFileOrDirectory, isSingleFile);
            foreach (string sourceFile in sourceFiles)
            {
                Process(sourceFile);
            }
            Console.WriteLine("Done.");
        }

        private static void Process(string sourceFile)
        {
            Console.WriteLine($"Processing {Path.GetFileName(sourceFile)} ...");
            using (var fs = new FileStream(sourceFile, FileMode.Open))
            using (var tokeniser = new Tokeniser(fs))
            {
                var parser = new Parser(new Grammarian());
                var xmlConverter = new XmlConverter();
                parser.Parse(tokeniser);
                SaveXml(sourceFile, xmlConverter.ConvertTokens(parser.Tokens), true);
                SaveXml(sourceFile, xmlConverter.ConvertNode(parser.Tree), false);
            }
        }

        private static void SaveXml(string sourceFile, XmlDocument xml, bool isTokens)
        {
            string fileName = GetOutputFileName(sourceFile, isTokens ? "T" : "");
            XmlWriterSettings settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true,
                Indent = true,
                Encoding = new UTF8Encoding(false)
            };
            using (var xmlWriter = XmlWriter.Create(fileName, settings))
            {
                xml.Save(xmlWriter);
            }
            Console.WriteLine($"... {(isTokens ? "tokens" : "parsed output")} written to {Path.GetFileName(fileName)}");
        }

        private static bool Initialise(string[] args)
        {
            Console.WriteLine("JACK Analyser");
            Console.WriteLine("-------------");
            string error = GetValidationErrors(args);
            if (error != null)
            {
                Console.WriteLine("Usage (simple):      dotnet ./JackAnalyser.dll [source-file]");
                Console.WriteLine("Usage (complex):     dotnet ./JackAnalyser.dll [source-directory]");
                Console.WriteLine();
                Console.WriteLine("source-file:");
                Console.WriteLine("    Path to file containing JACK code (must have a .jack file extension).");
                Console.WriteLine("    Results will be written to a file named after the source file, but with an .xml file extension. Any existing file with this name will be overwritten.");
                Console.WriteLine("    An intermediate results file will also be written containing the tokens only. It will also be named after the source file with an .xml file extension, but will have a T suffix. Any existing file with this name will be overwritten.");
                Console.WriteLine("");
                Console.WriteLine("source-directory:");
                Console.WriteLine("    Path to directory containing .jack files.");
                Console.WriteLine("    Results will be generated for all .jack files in the directory, using the same conventions described above.");
                Console.WriteLine();
                Console.WriteLine(error);
                return false;
            }
            return true;
        }

        private static string GetValidationErrors(string[] args)
        {
            if (args.Length == 0)
                return "No source file or directory specified.";
            else
            {
                string sourceFileOrDirectory = args[0];
                bool isDirectory = Directory.Exists(sourceFileOrDirectory);
                bool isFile = File.Exists(sourceFileOrDirectory);
                if (!isDirectory && !isFile)
                    return $"Path {sourceFileOrDirectory} does not exist.";
                if (isFile && Path.GetExtension(sourceFileOrDirectory) != ".jack")
                    return $"Source file {sourceFileOrDirectory} does not have a .jack file extension.";
                if (isDirectory && !Directory.GetFiles(sourceFileOrDirectory).Any(f => Path.GetExtension(f) == ".jack"))
                    return $"Source directory {sourceFileOrDirectory} does not contain any .jack files.";
            }
            return null;
        }

        private static string[] GetSourceFiles(string sourceFileOrDirectory, bool isSingleFile)
        {
            Console.WriteLine($"Processing {(isSingleFile ? "file" : "directory")} {sourceFileOrDirectory}");
            if (isSingleFile)
                return new[] { sourceFileOrDirectory };
            string[] files = Directory.GetFiles(sourceFileOrDirectory).Where(f => Path.GetExtension(f) == ".jack").ToArray();
            Console.WriteLine("Files in directory:");
            foreach (string file in files)
            {
                Console.WriteLine($"    {file}");
            }
            return files;
        }

        private static string GetOutputFileName(string sourceFile, string suffix = "")
        {
            string dir = Path.GetDirectoryName(sourceFile);
            string fn = Path.GetFileNameWithoutExtension(sourceFile);
            char sep = Path.DirectorySeparatorChar;
            return $"{dir}{sep}{fn}{suffix}.xml";
        }

    }
}
