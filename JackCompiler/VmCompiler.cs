using System;
using System.IO;

namespace JackCompiler
{
    public class VmCompiler
    {
        private readonly Node tree;
        private readonly TextWriter vmFile;

        public VmCompiler(Node tree, TextWriter vmFile)
        {
            this.tree = tree;
            this.vmFile = vmFile;
        }

        public void Compile()
        {
            vmFile.WriteLine("hello world");
        }
    }
}
