using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JackCompiler.Tests
{
    #region WhenCompilingSquare_Main

    [TestClass]
    public class WhenCompilingSquare_Main
    {
        private VmCompiler classUnderTest;

        const string jackInput = @"
// This file is part of www.nand2tetris.org
// and the book ""The Elements of Computing Systems""
// by Nisan and Schocken, MIT Press.
// File name: projects/11/Square/Main.jack

// (same as projects/09/Square/Main.jack)

/** Initializes a new Square Dance game and starts running it. */
class Main
        {
            function void main()
            {
                var SquareGame game;
                let game = SquareGame.new();
                do game.run();
        do game.dispose();
        return;
            }
        }
;
";
        const string vmOutput = @"function Main.main 1
call SquareGame.new 0
pop local 0
push local 0
call SquareGame.run 1
pop temp 0
push local 0
call SquareGame.dispose 1
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

    #region WhenCompilingSquare_SquareGame

    [TestClass]
    public class WhenCompilingSquare_SquareGame
    {
        private VmCompiler classUnderTest;

        const string jackInput = @"
// This file is part of www.nand2tetris.org
// and the book ""The Elements of Computing Systems""
// by Nisan and Schocken, MIT Press.
// File name: projects/11/Square/SquareGame.jack

// (same as projects/09/Square/SquareGame.jack)

/**
 * Implements the Square Dance game.
 * This simple game allows the user to move a black square around
 * the screen, and change the square's size during the movement.
 * When the game starts, a square of 30 by 30 pixels is shown at the
 * top-left corner of the screen. The user controls the square as follows.
 * The 4 arrow keys are used to move the square up, down, left, and right.
 * The 'z' and 'x' keys are used, respectively, to decrement and increment
 * the square's size. The 'q' key is used to quit the game.
 */

class SquareGame {
   field Square square; // the square of this game
   field int direction; // the square's current direction: 
                        // 0=none, 1=up, 2=down, 3=left, 4=right

   /** Constructs a new Square Game. */
   constructor SquareGame new() {
      // Creates a 30 by 30 pixels square and positions it at the top-left
      // of the screen.
      let square = Square.new(0, 0, 30);
      let direction = 0;  // initial state is no movement
      return this;
   }

   /** Disposes this game. */
   method void dispose() {
      do square.dispose();
      do Memory.deAlloc(this);
      return;
   }

   /** Moves the square in the current direction. */
   method void moveSquare() {
      if (direction = 1) { do square.moveUp(); }
      if (direction = 2) { do square.moveDown(); }
      if (direction = 3) { do square.moveLeft(); }
      if (direction = 4) { do square.moveRight(); }
      do Sys.wait(5);  // delays the next movement
      return;
   }

   /** Runs the game: handles the user's inputs and moves the square accordingly */
   method void run() {
      var char key;  // the key currently pressed by the user
      var boolean exit;
      let exit = false;
      
      while (~exit) {
         // waits for a key to be pressed
         while (key = 0) {
            let key = Keyboard.keyPressed();
            do moveSquare();
         }
         if (key = 81)  { let exit = true; }     // q key
         if (key = 90)  { do square.decSize(); } // z key
         if (key = 88)  { do square.incSize(); } // x key
         if (key = 131) { let direction = 1; }   // up arrow
         if (key = 133) { let direction = 2; }   // down arrow
         if (key = 130) { let direction = 3; }   // left arrow
         if (key = 132) { let direction = 4; }   // right arrow

         // waits for the key to be released
         while (~(key = 0)) {
            let key = Keyboard.keyPressed();
            do moveSquare();
         }
     } // while
     return;
   }
}
";
        const string vmOutput = @"function SquareGame.new 0
push constant 2
call Memory.alloc 1
pop pointer 0
push constant 0
push constant 0
push constant 30
call Square.new 3
pop this 0
push constant 0
pop this 1
push pointer 0
return
function SquareGame.dispose 0
push argument 0
pop pointer 0
push this 0
call Square.dispose 1
pop temp 0
push pointer 0
call Memory.deAlloc 1
pop temp 0
push constant 0
return
function SquareGame.moveSquare 0
push argument 0
pop pointer 0
push this 1
push constant 1
eq
if-goto IF_TRUE0
goto IF_FALSE0
label IF_TRUE0
push this 0
call Square.moveUp 1
pop temp 0
label IF_FALSE0
push this 1
push constant 2
eq
if-goto IF_TRUE1
goto IF_FALSE1
label IF_TRUE1
push this 0
call Square.moveDown 1
pop temp 0
label IF_FALSE1
push this 1
push constant 3
eq
if-goto IF_TRUE2
goto IF_FALSE2
label IF_TRUE2
push this 0
call Square.moveLeft 1
pop temp 0
label IF_FALSE2
push this 1
push constant 4
eq
if-goto IF_TRUE3
goto IF_FALSE3
label IF_TRUE3
push this 0
call Square.moveRight 1
pop temp 0
label IF_FALSE3
push constant 5
call Sys.wait 1
pop temp 0
push constant 0
return
function SquareGame.run 2
push argument 0
pop pointer 0
push constant 0
pop local 1
label WHILE_EXP0
push local 1
not
not
if-goto WHILE_END0
label WHILE_EXP1
push local 0
push constant 0
eq
not
if-goto WHILE_END1
call Keyboard.keyPressed 0
pop local 0
push pointer 0
call SquareGame.moveSquare 1
pop temp 0
goto WHILE_EXP1
label WHILE_END1
push local 0
push constant 81
eq
if-goto IF_TRUE0
goto IF_FALSE0
label IF_TRUE0
push constant 0
not
pop local 1
label IF_FALSE0
push local 0
push constant 90
eq
if-goto IF_TRUE1
goto IF_FALSE1
label IF_TRUE1
push this 0
call Square.decSize 1
pop temp 0
label IF_FALSE1
push local 0
push constant 88
eq
if-goto IF_TRUE2
goto IF_FALSE2
label IF_TRUE2
push this 0
call Square.incSize 1
pop temp 0
label IF_FALSE2
push local 0
push constant 131
eq
if-goto IF_TRUE3
goto IF_FALSE3
label IF_TRUE3
push constant 1
pop this 1
label IF_FALSE3
push local 0
push constant 133
eq
if-goto IF_TRUE4
goto IF_FALSE4
label IF_TRUE4
push constant 2
pop this 1
label IF_FALSE4
push local 0
push constant 130
eq
if-goto IF_TRUE5
goto IF_FALSE5
label IF_TRUE5
push constant 3
pop this 1
label IF_FALSE5
push local 0
push constant 132
eq
if-goto IF_TRUE6
goto IF_FALSE6
label IF_TRUE6
push constant 4
pop this 1
label IF_FALSE6
label WHILE_EXP2
push local 0
push constant 0
eq
not
not
if-goto WHILE_END2
call Keyboard.keyPressed 0
pop local 0
push pointer 0
call SquareGame.moveSquare 1
pop temp 0
goto WHILE_EXP2
label WHILE_END2
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

    #region WhenCompilingSquare_Square

    [TestClass]
    public class WhenCompilingSquare_Square
    {
        private VmCompiler classUnderTest;

        const string jackInput = @"
// This file is part of www.nand2tetris.org
// and the book ""The Elements of Computing Systems""
// by Nisan and Schocken, MIT Press.
// File name: projects/11/Square/Square.jack

// (same as projects/09/Square/Square.jack)

/** Implements a graphical square. */
class Square {

   field int x, y; // screen location of the square's top-left corner
   field int size; // length of this square, in pixels

   /** Constructs a new square with a given location and size. */
   constructor Square new(int Ax, int Ay, int Asize) {
      let x = Ax;
      let y = Ay;
      let size = Asize;
      do draw();
      return this;
   }

   /** Disposes this square. */
   method void dispose() {
      do Memory.deAlloc(this);
      return;
   }

   /** Draws the square on the screen. */
   method void draw() {
      do Screen.setColor(true);
      do Screen.drawRectangle(x, y, x + size, y + size);
      return;
   }

   /** Erases the square from the screen. */
   method void erase() {
      do Screen.setColor(false);
      do Screen.drawRectangle(x, y, x + size, y + size);
      return;
   }

    /** Increments the square size by 2 pixels. */
   method void incSize() {
      if (((y + size) < 254) & ((x + size) < 510)) {
         do erase();
         let size = size + 2;
         do draw();
      }
      return;
   }

   /** Decrements the square size by 2 pixels. */
   method void decSize() {
      if (size > 2) {
         do erase();
         let size = size - 2;
         do draw();
      }
      return;
   }

   /** Moves the square up by 2 pixels. */
   method void moveUp() {
      if (y > 1) {
         do Screen.setColor(false);
         do Screen.drawRectangle(x, (y + size) - 1, x + size, y + size);
         let y = y - 2;
         do Screen.setColor(true);
         do Screen.drawRectangle(x, y, x + size, y + 1);
      }
      return;
   }

   /** Moves the square down by 2 pixels. */
   method void moveDown() {
      if ((y + size) < 254) {
         do Screen.setColor(false);
         do Screen.drawRectangle(x, y, x + size, y + 1);
         let y = y + 2;
         do Screen.setColor(true);
         do Screen.drawRectangle(x, (y + size) - 1, x + size, y + size);
      }
      return;
   }

   /** Moves the square left by 2 pixels. */
   method void moveLeft() {
      if (x > 1) {
         do Screen.setColor(false);
         do Screen.drawRectangle((x + size) - 1, y, x + size, y + size);
         let x = x - 2;
         do Screen.setColor(true);
         do Screen.drawRectangle(x, y, x + 1, y + size);
      }
      return;
   }

   /** Moves the square right by 2 pixels. */
   method void moveRight() {
      if ((x + size) < 510) {
         do Screen.setColor(false);
         do Screen.drawRectangle(x, y, x + 1, y + size);
         let x = x + 2;
         do Screen.setColor(true);
         do Screen.drawRectangle((x + size) - 1, y, x + size, y + size);
      }
      return;
   }
}
";
        const string vmOutput = @"function Square.new 0
push constant 3
call Memory.alloc 1
pop pointer 0
push argument 0
pop this 0
push argument 1
pop this 1
push argument 2
pop this 2
push pointer 0
call Square.draw 1
pop temp 0
push pointer 0
return
function Square.dispose 0
push argument 0
pop pointer 0
push pointer 0
call Memory.deAlloc 1
pop temp 0
push constant 0
return
function Square.draw 0
push argument 0
pop pointer 0
push constant 0
not
call Screen.setColor 1
pop temp 0
push this 0
push this 1
push this 0
push this 2
add
push this 1
push this 2
add
call Screen.drawRectangle 4
pop temp 0
push constant 0
return
function Square.erase 0
push argument 0
pop pointer 0
push constant 0
call Screen.setColor 1
pop temp 0
push this 0
push this 1
push this 0
push this 2
add
push this 1
push this 2
add
call Screen.drawRectangle 4
pop temp 0
push constant 0
return
function Square.incSize 0
push argument 0
pop pointer 0
push this 1
push this 2
add
push constant 254
lt
push this 0
push this 2
add
push constant 510
lt
and
if-goto IF_TRUE0
goto IF_FALSE0
label IF_TRUE0
push pointer 0
call Square.erase 1
pop temp 0
push this 2
push constant 2
add
pop this 2
push pointer 0
call Square.draw 1
pop temp 0
label IF_FALSE0
push constant 0
return
function Square.decSize 0
push argument 0
pop pointer 0
push this 2
push constant 2
gt
if-goto IF_TRUE0
goto IF_FALSE0
label IF_TRUE0
push pointer 0
call Square.erase 1
pop temp 0
push this 2
push constant 2
sub
pop this 2
push pointer 0
call Square.draw 1
pop temp 0
label IF_FALSE0
push constant 0
return
function Square.moveUp 0
push argument 0
pop pointer 0
push this 1
push constant 1
gt
if-goto IF_TRUE0
goto IF_FALSE0
label IF_TRUE0
push constant 0
call Screen.setColor 1
pop temp 0
push this 0
push this 1
push this 2
add
push constant 1
sub
push this 0
push this 2
add
push this 1
push this 2
add
call Screen.drawRectangle 4
pop temp 0
push this 1
push constant 2
sub
pop this 1
push constant 0
not
call Screen.setColor 1
pop temp 0
push this 0
push this 1
push this 0
push this 2
add
push this 1
push constant 1
add
call Screen.drawRectangle 4
pop temp 0
label IF_FALSE0
push constant 0
return
function Square.moveDown 0
push argument 0
pop pointer 0
push this 1
push this 2
add
push constant 254
lt
if-goto IF_TRUE0
goto IF_FALSE0
label IF_TRUE0
push constant 0
call Screen.setColor 1
pop temp 0
push this 0
push this 1
push this 0
push this 2
add
push this 1
push constant 1
add
call Screen.drawRectangle 4
pop temp 0
push this 1
push constant 2
add
pop this 1
push constant 0
not
call Screen.setColor 1
pop temp 0
push this 0
push this 1
push this 2
add
push constant 1
sub
push this 0
push this 2
add
push this 1
push this 2
add
call Screen.drawRectangle 4
pop temp 0
label IF_FALSE0
push constant 0
return
function Square.moveLeft 0
push argument 0
pop pointer 0
push this 0
push constant 1
gt
if-goto IF_TRUE0
goto IF_FALSE0
label IF_TRUE0
push constant 0
call Screen.setColor 1
pop temp 0
push this 0
push this 2
add
push constant 1
sub
push this 1
push this 0
push this 2
add
push this 1
push this 2
add
call Screen.drawRectangle 4
pop temp 0
push this 0
push constant 2
sub
pop this 0
push constant 0
not
call Screen.setColor 1
pop temp 0
push this 0
push this 1
push this 0
push constant 1
add
push this 1
push this 2
add
call Screen.drawRectangle 4
pop temp 0
label IF_FALSE0
push constant 0
return
function Square.moveRight 0
push argument 0
pop pointer 0
push this 0
push this 2
add
push constant 510
lt
if-goto IF_TRUE0
goto IF_FALSE0
label IF_TRUE0
push constant 0
call Screen.setColor 1
pop temp 0
push this 0
push this 1
push this 0
push constant 1
add
push this 1
push this 2
add
call Screen.drawRectangle 4
pop temp 0
push this 0
push constant 2
add
pop this 0
push constant 0
not
call Screen.setColor 1
pop temp 0
push this 0
push this 2
add
push constant 1
sub
push this 1
push this 0
push this 2
add
push this 1
push this 2
add
call Screen.drawRectangle 4
pop temp 0
label IF_FALSE0
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
