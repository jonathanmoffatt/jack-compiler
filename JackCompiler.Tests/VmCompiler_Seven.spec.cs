using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JackCompiler.Tests
{
    #region WhenCompilingSeven

    [TestClass]
    public class WhenCompilingSeven
    {
        private VmCompiler classUnderTest;

        const string jackInput = @"
// This file is part of www.nand2tetris.org
// and the book ""The Elements of Computing Systems""
// by Nisan and Schocken, MIT Press.
// File name: projects/11/Seven/Main.jack

/**
 * Computes the value of 1 + (2 * 3) and prints the result
 * at the top-left of the screen.  
 */
class Main
        {

            function void main()
            {
                do Output.printInt(1 + (2 * 3));
      return;
            }

        }
";
        const string vmOutput = @"function Main.main 0
push constant 1
push constant 2
push constant 3
call Math.multiply 2
add
call Output.printInt 1
pop temp 0
push constant 0
return
";

        [TestInitialize]
        public void Setup()
        {
            classUnderTest = new VmCompiler();
        }

        [TestMethod]
        public void GeneratesTheCorrectVmCode()
        {
            classUnderTest.CompilingJackCode(jackInput).Should().Be(vmOutput);
        }
    }

    #endregion
}
