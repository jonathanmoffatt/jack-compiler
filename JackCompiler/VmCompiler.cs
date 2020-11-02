using System.IO;

namespace JackCompiler
{
    public class VmCompiler
    {
        public void Compile(Node tree, TextWriter vmFile)
        {
            vmFile.WriteLine("hello world");
        }
    }
}
