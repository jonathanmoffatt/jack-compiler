using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JackCompiler.Tests
{
    #region WhenCompilingComplexArrays

    [TestClass]
    public class WhenCompilingComplexArrays
    {
        private VmCompiler classUnderTest;

        const string jackInput = @"
// This file is part of www.nand2tetris.org
// and the book ""The Elements of Computing Systems""
// by Nisan and Schocken, MIT Press.
// File name: projects/11/ComplexArrays/Main.jack

/**
 * Performs several complex array processing tests.
 * For each test, the expected result is printed, along with the
 * actual result. In each test, the two results should be equal.
 */
class Main {

    function void main() {
        var Array a, b, c;
        
        let a = Array.new(10);
        let b = Array.new(5);
        let c = Array.new(1);
        
        let a[3] = 2;
        let a[4] = 8;
        let a[5] = 4;
        let b[a[3]] = a[3] + 3;  // b[2] = 5
        let a[b[a[3]]] = a[a[5]] * b[((7 - a[3]) - Main.double(2)) + 1];  // a[5] = 8 * 5 = 40
        let c[0] = null;
        let c = c[0];
        
        do Output.printString(""Test 1: expected result: 5; actual result: "");
        do Output.printInt(b[2]);
        do Output.println();
        do Output.printString(""Test 2: expected result: 40; actual result: "");
        do Output.printInt(a[5]);
        do Output.println();
        do Output.printString(""Test 3: expected result: 0; actual result: "");
        do Output.printInt(c);
        do Output.println();
        
        let c = null;

        if (c = null) {
            do Main.fill(a, 10);
            let c = a[3];
            let c[1] = 33;
            let c = a[7];
            let c[1] = 77;
            let b = a[3];
            let b[1] = b[1] + c[1];  // b[1] = 33 + 77 = 110;
        }

        do Output.printString(""Test 4: expected result: 77; actual result: "");
        do Output.printInt(c[1]);
        do Output.println();
        do Output.printString(""Test 5: expected result: 110; actual result: "");
        do Output.printInt(b[1]);
        do Output.println();
        return;
    }
    
    function int double(int a) {
    	return a * 2;
    }
    
    function void fill(Array a, int size) {
        while (size > 0) {
            let size = size - 1;
            let a[size] = Array.new(3);
        }
        return;
    }
}
";
        const string vmOutput = @"function Main.main 3
push constant 10
call Array.new 1
pop local 0
push constant 5
call Array.new 1
pop local 1
push constant 1
call Array.new 1
pop local 2
push constant 3
push local 0
add
push constant 2
pop temp 0
pop pointer 1
push temp 0
pop that 0
push constant 4
push local 0
add
push constant 8
pop temp 0
pop pointer 1
push temp 0
pop that 0
push constant 5
push local 0
add
push constant 4
pop temp 0
pop pointer 1
push temp 0
pop that 0
push constant 3
push local 0
add
pop pointer 1
push that 0
push local 1
add
push constant 3
push local 0
add
pop pointer 1
push that 0
push constant 3
add
pop temp 0
pop pointer 1
push temp 0
pop that 0
push constant 3
push local 0
add
pop pointer 1
push that 0
push local 1
add
pop pointer 1
push that 0
push local 0
add
push constant 5
push local 0
add
pop pointer 1
push that 0
push local 0
add
pop pointer 1
push that 0
push constant 7
push constant 3
push local 0
add
pop pointer 1
push that 0
sub
push constant 2
call Main.double 1
sub
push constant 1
add
push local 1
add
pop pointer 1
push that 0
call Math.multiply 2
pop temp 0
pop pointer 1
push temp 0
pop that 0
push constant 0
push local 2
add
push constant 0
pop temp 0
pop pointer 1
push temp 0
pop that 0
push constant 0
push local 2
add
pop pointer 1
push that 0
pop local 2
push constant 43
call String.new 1
push constant 84
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 115
call String.appendChar 2
push constant 116
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 49
call String.appendChar 2
push constant 58
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 120
call String.appendChar 2
push constant 112
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 99
call String.appendChar 2
push constant 116
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 100
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 114
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 115
call String.appendChar 2
push constant 117
call String.appendChar 2
push constant 108
call String.appendChar 2
push constant 116
call String.appendChar 2
push constant 58
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 53
call String.appendChar 2
push constant 59
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 97
call String.appendChar 2
push constant 99
call String.appendChar 2
push constant 116
call String.appendChar 2
push constant 117
call String.appendChar 2
push constant 97
call String.appendChar 2
push constant 108
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 114
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 115
call String.appendChar 2
push constant 117
call String.appendChar 2
push constant 108
call String.appendChar 2
push constant 116
call String.appendChar 2
push constant 58
call String.appendChar 2
push constant 32
call String.appendChar 2
call Output.printString 1
pop temp 0
push constant 2
push local 1
add
pop pointer 1
push that 0
call Output.printInt 1
pop temp 0
call Output.println 0
pop temp 0
push constant 44
call String.new 1
push constant 84
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 115
call String.appendChar 2
push constant 116
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 50
call String.appendChar 2
push constant 58
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 120
call String.appendChar 2
push constant 112
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 99
call String.appendChar 2
push constant 116
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 100
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 114
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 115
call String.appendChar 2
push constant 117
call String.appendChar 2
push constant 108
call String.appendChar 2
push constant 116
call String.appendChar 2
push constant 58
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 52
call String.appendChar 2
push constant 48
call String.appendChar 2
push constant 59
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 97
call String.appendChar 2
push constant 99
call String.appendChar 2
push constant 116
call String.appendChar 2
push constant 117
call String.appendChar 2
push constant 97
call String.appendChar 2
push constant 108
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 114
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 115
call String.appendChar 2
push constant 117
call String.appendChar 2
push constant 108
call String.appendChar 2
push constant 116
call String.appendChar 2
push constant 58
call String.appendChar 2
push constant 32
call String.appendChar 2
call Output.printString 1
pop temp 0
push constant 5
push local 0
add
pop pointer 1
push that 0
call Output.printInt 1
pop temp 0
call Output.println 0
pop temp 0
push constant 43
call String.new 1
push constant 84
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 115
call String.appendChar 2
push constant 116
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 51
call String.appendChar 2
push constant 58
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 120
call String.appendChar 2
push constant 112
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 99
call String.appendChar 2
push constant 116
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 100
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 114
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 115
call String.appendChar 2
push constant 117
call String.appendChar 2
push constant 108
call String.appendChar 2
push constant 116
call String.appendChar 2
push constant 58
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 48
call String.appendChar 2
push constant 59
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 97
call String.appendChar 2
push constant 99
call String.appendChar 2
push constant 116
call String.appendChar 2
push constant 117
call String.appendChar 2
push constant 97
call String.appendChar 2
push constant 108
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 114
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 115
call String.appendChar 2
push constant 117
call String.appendChar 2
push constant 108
call String.appendChar 2
push constant 116
call String.appendChar 2
push constant 58
call String.appendChar 2
push constant 32
call String.appendChar 2
call Output.printString 1
pop temp 0
push local 2
call Output.printInt 1
pop temp 0
call Output.println 0
pop temp 0
push constant 0
pop local 2
push local 2
push constant 0
eq
if-goto IF_TRUE0
goto IF_FALSE0
label IF_TRUE0
push local 0
push constant 10
call Main.fill 2
pop temp 0
push constant 3
push local 0
add
pop pointer 1
push that 0
pop local 2
push constant 1
push local 2
add
push constant 33
pop temp 0
pop pointer 1
push temp 0
pop that 0
push constant 7
push local 0
add
pop pointer 1
push that 0
pop local 2
push constant 1
push local 2
add
push constant 77
pop temp 0
pop pointer 1
push temp 0
pop that 0
push constant 3
push local 0
add
pop pointer 1
push that 0
pop local 1
push constant 1
push local 1
add
push constant 1
push local 1
add
pop pointer 1
push that 0
push constant 1
push local 2
add
pop pointer 1
push that 0
add
pop temp 0
pop pointer 1
push temp 0
pop that 0
label IF_FALSE0
push constant 44
call String.new 1
push constant 84
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 115
call String.appendChar 2
push constant 116
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 52
call String.appendChar 2
push constant 58
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 120
call String.appendChar 2
push constant 112
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 99
call String.appendChar 2
push constant 116
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 100
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 114
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 115
call String.appendChar 2
push constant 117
call String.appendChar 2
push constant 108
call String.appendChar 2
push constant 116
call String.appendChar 2
push constant 58
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 55
call String.appendChar 2
push constant 55
call String.appendChar 2
push constant 59
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 97
call String.appendChar 2
push constant 99
call String.appendChar 2
push constant 116
call String.appendChar 2
push constant 117
call String.appendChar 2
push constant 97
call String.appendChar 2
push constant 108
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 114
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 115
call String.appendChar 2
push constant 117
call String.appendChar 2
push constant 108
call String.appendChar 2
push constant 116
call String.appendChar 2
push constant 58
call String.appendChar 2
push constant 32
call String.appendChar 2
call Output.printString 1
pop temp 0
push constant 1
push local 2
add
pop pointer 1
push that 0
call Output.printInt 1
pop temp 0
call Output.println 0
pop temp 0
push constant 45
call String.new 1
push constant 84
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 115
call String.appendChar 2
push constant 116
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 53
call String.appendChar 2
push constant 58
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 120
call String.appendChar 2
push constant 112
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 99
call String.appendChar 2
push constant 116
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 100
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 114
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 115
call String.appendChar 2
push constant 117
call String.appendChar 2
push constant 108
call String.appendChar 2
push constant 116
call String.appendChar 2
push constant 58
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 49
call String.appendChar 2
push constant 49
call String.appendChar 2
push constant 48
call String.appendChar 2
push constant 59
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 97
call String.appendChar 2
push constant 99
call String.appendChar 2
push constant 116
call String.appendChar 2
push constant 117
call String.appendChar 2
push constant 97
call String.appendChar 2
push constant 108
call String.appendChar 2
push constant 32
call String.appendChar 2
push constant 114
call String.appendChar 2
push constant 101
call String.appendChar 2
push constant 115
call String.appendChar 2
push constant 117
call String.appendChar 2
push constant 108
call String.appendChar 2
push constant 116
call String.appendChar 2
push constant 58
call String.appendChar 2
push constant 32
call String.appendChar 2
call Output.printString 1
pop temp 0
push constant 1
push local 1
add
pop pointer 1
push that 0
call Output.printInt 1
pop temp 0
call Output.println 0
pop temp 0
push constant 0
return
function Main.double 0
push argument 0
push constant 2
call Math.multiply 2
return
function Main.fill 0
label WHILE_EXP0
push argument 1
push constant 0
gt
not
if-goto WHILE_END0
push argument 1
push constant 1
sub
pop argument 1
push argument 1
push argument 0
add
push constant 3
call Array.new 1
pop temp 0
pop pointer 1
push temp 0
pop that 0
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
