using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JackCompiler.Tests
{
    #region WhenCompilingConvertToBin

    [TestClass]
    public class WhenCompilingConvertToBin
    {
        private VmCompiler classUnderTest;

        const string jackInput = @"
// This file is part of www.nand2tetris.org
// and the book ""The Elements of Computing Systems""
// by Nisan and Schocken, MIT Press.
// File name: projects/11/ConvertToBin/Main.jack

/**
 * Unpacks a 16-bit number into its binary representation:
 * Takes the 16-bit number stored in RAM[8000] and stores its individual 
 * bits in RAM[8001..8016] (each location will contain 0 or 1).
 * Before the conversion, RAM[8001]..RAM[8016] are initialized to -1.
 * 
 * The program should be tested as follows:
 * 1) Load the program into the supplied VM emulator
 * 2) Put some value in RAM[8000]
 * 3) Switch to ""no animation""
 * 4) Run the program (give it enough time to run)
 * 5) Stop the program
 * 6) Check that RAM[8001]..RAM[8016] contains the correct binary result, and
 *    that none of these memory locations contains -1.
 */
class Main
        {

            /**
             * Initializes RAM[8001]..RAM[8016] to -1,
             * and converts the value in RAM[8000] to binary.
             */
            function void main()
            {
                var int value;
                do Main.fillMemory(8001, 16, -1); // sets RAM[8001]..RAM[8016] to -1

                let value = Memory.peek(8000);    // reads a value from RAM[8000]
                do Main.convert(value);           // performs the conversion
        return;
            }

            /** Converts the given decimal value to binary, and puts 
             *  the resulting bits in RAM[8001]..RAM[8016]. */
            function void convert(int value)
            {
                var int mask, position;
                var boolean loop;

                let loop = true;
                while (loop)
                {
                    let position = position + 1;
                    let mask = Main.nextMask(mask);

                    if (~(position > 16))
                    {

                        if (~((value & mask) = 0))
                        {
                            do Memory.poke(8000 + position, 1);

                   }
                        else
                        {
                            do Memory.poke(8000 + position, 0);

                  }
                    }
                    else
                    {
                        let loop = false;
                    }
                }
                return;
            }

            /** Returns the next mask (the mask that should follow the given mask). */
            function int nextMask(int mask)
            {
                if (mask = 0)
                {
                    return 1;
                }
                else
                {
                    return mask * 2;
                }
            }

            /** Fills 'length' consecutive memory locations with 'value',
              * starting at 'startAddress'. */
            function void fillMemory(int startAddress, int length, int value)
            {
                while (length > 0)
                {
                    do Memory.poke(startAddress, value);

                    let length = length - 1;
                    let startAddress = startAddress + 1;
                }
                return;
            }
        }
";

        const string vmOutput = @"function Main.main 1
push constant 8001
push constant 16
push constant 1
neg
call Main.fillMemory 3
pop temp 0
push constant 8000
call Memory.peek 1
pop local 0
push local 0
call Main.convert 1
pop temp 0
push constant 0
return
function Main.convert 3
push constant 0
not
pop local 2
label WHILE_EXP0
push local 2
not
if-goto WHILE_END0
push local 1
push constant 1
add
pop local 1
push local 0
call Main.nextMask 1
pop local 0
push local 1
push constant 16
gt
not
if-goto IF_TRUE0
goto IF_FALSE0
label IF_TRUE0
push argument 0
push local 0
and
push constant 0
eq
not
if-goto IF_TRUE1
goto IF_FALSE1
label IF_TRUE1
push constant 8000
push local 1
add
push constant 1
call Memory.poke 2
pop temp 0
goto IF_END1
label IF_FALSE1
push constant 8000
push local 1
add
push constant 0
call Memory.poke 2
pop temp 0
label IF_END1
goto IF_END0
label IF_FALSE0
push constant 0
pop local 2
label IF_END0
goto WHILE_EXP0
label WHILE_END0
push constant 0
return
function Main.nextMask 0
push argument 0
push constant 0
eq
if-goto IF_TRUE0
goto IF_FALSE0
label IF_TRUE0
push constant 1
return
goto IF_END0
label IF_FALSE0
push argument 0
push constant 2
call Math.multiply 2
return
label IF_END0
function Main.fillMemory 0
label WHILE_EXP0
push argument 1
push constant 0
gt
not
if-goto WHILE_END0
push argument 0
push argument 2
call Memory.poke 2
pop temp 0
push argument 1
push constant 1
sub
pop argument 1
push argument 0
push constant 1
add
pop argument 0
goto WHILE_EXP0
label WHILE_END0
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
