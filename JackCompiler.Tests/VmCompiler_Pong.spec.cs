using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JackCompiler.Tests
{
    #region WhenCompilingPong_Ball

    [TestClass]
    public class WhenCompilingPong_Ball
    {
        private VmCompiler classUnderTest;

        const string jackInput = @"
// This file is part of www.nand2tetris.org
// and the book ""The Elements of Computing Systems""
// by Nisan and Schocken, MIT Press.
// File name: projects/11/Pong/Ball.jack

/**
 * A graphical ball. Characterized by a screen location and distance of 
 * last destination. Has methods for drawing, erasing and moving on the screen.
 * The ball is displayed as a filled, 6-by-6 pixles rectangle. 
 */
class Ball {

    field int x, y;               // the ball's screen location (in pixels)
    field int lengthx, lengthy;   // distance of last destination (in pixels)

    field int d, straightD, diagonalD;            // used for straight line movement computation
    field boolean invert, positivex, positivey;   // (same)
   
    field int leftWall, rightWall, topWall, bottomWall;  // wall locations
   
    field int wall;   // last wall that the ball was bounced off of

    /** Constructs a new ball with the given initial location and wall locations. */
    constructor Ball new(int Ax, int Ay,
                         int AleftWall, int ArightWall, int AtopWall, int AbottomWall) {    	
	    let x = Ax;		
	    let y = Ay;
	    let leftWall = AleftWall;
	    let rightWall = ArightWall - 6;    // -6 for ball size
	    let topWall = AtopWall; 
	    let bottomWall = AbottomWall - 6;  // -6 for ball size
	    let wall = 0;
        do show();
        return this;
    }

    /** Deallocates the Ball's memory. */
    method void dispose() {
        do Memory.deAlloc(this);
        return;
    }

    /** Shows the ball. */
    method void show() {
        do Screen.setColor(true);
        do draw();
        return;
    }

    /** Hides the ball. */
    method void hide() {
        do Screen.setColor(false);
	    do draw();
        return;
    }

    /** Draws the ball. */
    method void draw() {
	    do Screen.drawRectangle(x, y, x + 5, y + 5);
	    return;
    }

    /** Returns the ball's left edge. */
    method int getLeft() {
        return x;
    }

    /** Returns the ball's right edge. */
    method int getRight() {
        return x + 5;
    }

    /** Computes and sets the ball's destination. */
    method void setDestination(int destx, int desty) {
        var int dx, dy, temp;
  	    let lengthx = destx - x;
	    let lengthy = desty - y;
        let dx = Math.abs(lengthx);
        let dy = Math.abs(lengthy);
        let invert = (dx < dy);

        if (invert) {
            let temp = dx; // swap dx, dy
            let dx = dy;
            let dy = temp;
   	        let positivex = (y < desty);
            let positivey = (x < destx);
        }
        else {
	        let positivex = (x < destx);
            let positivey = (y < desty);
        }

        let d = (2 * dy) - dx;
        let straightD = 2 * dy;
        let diagonalD = 2 * (dy - dx);

	    return;
    }

    /**
     * Moves the ball one unit towards its destination.
     * If the ball has reached a wall, returns 0.
     * Else, returns a value according to the wall:
     * 1 (left wall), 2 (right wall), 3 (top wall), 4 (bottom wall).
     */
    method int move() {

	    do hide();

        if (d < 0) { let d = d + straightD; }
        else {
            let d = d + diagonalD;

            if (positivey) {
                if (invert) { let x = x + 4; }
                else { let y = y + 4; }
            }
            else {
                if (invert) { let x = x - 4; }
                else { let y = y - 4; }
            }
	    }

        if (positivex) {
            if (invert) { let y = y + 4; }
            else { let x = x + 4; }
	    }
	    else {
            if (invert) { let y = y - 4; }
            else { let x = x - 4; }
	    }

	    if (~(x > leftWall)) {
	        let wall = 1;    
	        let x = leftWall;
	    }
        if (~(x < rightWall)) {
	        let wall = 2;    
	        let x = rightWall;
	    }
        if (~(y > topWall)) {
            let wall = 3;    
	        let y = topWall;
        }
        if (~(y < bottomWall)) {
            let wall = 4;    
	        let y = bottomWall;
        }

	    do show();

	    return wall;
    }

    /**
     * Bounces off the current wall: sets the new destination
     * of the ball according to the ball's angle and the given
     * bouncing direction (-1/0/1=left/center/right or up/center/down).
     */
    method void bounce(int bouncingDirection) {
        var int newx, newy, divLengthx, divLengthy, factor;

	    // dividing by 10 first since results are too big
        let divLengthx = lengthx / 10;
        let divLengthy = lengthy / 10;
	    if (bouncingDirection = 0) { let factor = 10; }
	    else {
	        if (((~(lengthx < 0)) & (bouncingDirection = 1)) | ((lengthx < 0) & (bouncingDirection = (-1)))) {
                let factor = 20; // bounce direction is in ball direction
            }
	        else { let factor = 5; } // bounce direction is against ball direction
	    }

	    if (wall = 1) {
	        let newx = 506;
	        let newy = (divLengthy * (-50)) / divLengthx;
            let newy = y + (newy * factor);
	    }
        else {
            if (wall = 2) {
                let newx = 0;
                let newy = (divLengthy * 50) / divLengthx;
                let newy = y + (newy * factor);
	        }
	        else {
                if (wall = 3) {
		            let newy = 250;
		            let newx = (divLengthx * (-25)) / divLengthy;
                    let newx = x + (newx * factor);
		        }
                else { // assumes wall = 4
		            let newy = 0;
		            let newx = (divLengthx * 25) / divLengthy;
                    let newx = x + (newx * factor);
		        }
            }
        }

        do setDestination(newx, newy);
        return;
    }
}
";
        const string vmOutput = @"function Ball.new 0
push constant 15
call Memory.alloc 1
pop pointer 0
push argument 0
pop this 0
push argument 1
pop this 1
push argument 2
pop this 10
push argument 3
push constant 6
sub
pop this 11
push argument 4
pop this 12
push argument 5
push constant 6
sub
pop this 13
push constant 0
pop this 14
push pointer 0
call Ball.show 1
pop temp 0
push pointer 0
return
function Ball.dispose 0
push argument 0
pop pointer 0
push pointer 0
call Memory.deAlloc 1
pop temp 0
push constant 0
return
function Ball.show 0
push argument 0
pop pointer 0
push constant 0
not
call Screen.setColor 1
pop temp 0
push pointer 0
call Ball.draw 1
pop temp 0
push constant 0
return
function Ball.hide 0
push argument 0
pop pointer 0
push constant 0
call Screen.setColor 1
pop temp 0
push pointer 0
call Ball.draw 1
pop temp 0
push constant 0
return
function Ball.draw 0
push argument 0
pop pointer 0
push this 0
push this 1
push this 0
push constant 5
add
push this 1
push constant 5
add
call Screen.drawRectangle 4
pop temp 0
push constant 0
return
function Ball.getLeft 0
push argument 0
pop pointer 0
push this 0
return
function Ball.getRight 0
push argument 0
pop pointer 0
push this 0
push constant 5
add
return
function Ball.setDestination 3
push argument 0
pop pointer 0
push argument 1
push this 0
sub
pop this 2
push argument 2
push this 1
sub
pop this 3
push this 2
call Math.abs 1
pop local 0
push this 3
call Math.abs 1
pop local 1
push local 0
push local 1
lt
pop this 7
push this 7
if-goto IF_TRUE0
goto IF_FALSE0
label IF_TRUE0
push local 0
pop local 2
push local 1
pop local 0
push local 2
pop local 1
push this 1
push argument 2
lt
pop this 8
push this 0
push argument 1
lt
pop this 9
goto IF_END0
label IF_FALSE0
push this 0
push argument 1
lt
pop this 8
push this 1
push argument 2
lt
pop this 9
label IF_END0
push constant 2
push local 1
call Math.multiply 2
push local 0
sub
pop this 4
push constant 2
push local 1
call Math.multiply 2
pop this 5
push constant 2
push local 1
push local 0
sub
call Math.multiply 2
pop this 6
push constant 0
return
function Ball.move 0
push argument 0
pop pointer 0
push pointer 0
call Ball.hide 1
pop temp 0
push this 4
push constant 0
lt
if-goto IF_TRUE0
goto IF_FALSE0
label IF_TRUE0
push this 4
push this 5
add
pop this 4
goto IF_END0
label IF_FALSE0
push this 4
push this 6
add
pop this 4
push this 9
if-goto IF_TRUE1
goto IF_FALSE1
label IF_TRUE1
push this 7
if-goto IF_TRUE2
goto IF_FALSE2
label IF_TRUE2
push this 0
push constant 4
add
pop this 0
goto IF_END2
label IF_FALSE2
push this 1
push constant 4
add
pop this 1
label IF_END2
goto IF_END1
label IF_FALSE1
push this 7
if-goto IF_TRUE3
goto IF_FALSE3
label IF_TRUE3
push this 0
push constant 4
sub
pop this 0
goto IF_END3
label IF_FALSE3
push this 1
push constant 4
sub
pop this 1
label IF_END3
label IF_END1
label IF_END0
push this 8
if-goto IF_TRUE4
goto IF_FALSE4
label IF_TRUE4
push this 7
if-goto IF_TRUE5
goto IF_FALSE5
label IF_TRUE5
push this 1
push constant 4
add
pop this 1
goto IF_END5
label IF_FALSE5
push this 0
push constant 4
add
pop this 0
label IF_END5
goto IF_END4
label IF_FALSE4
push this 7
if-goto IF_TRUE6
goto IF_FALSE6
label IF_TRUE6
push this 1
push constant 4
sub
pop this 1
goto IF_END6
label IF_FALSE6
push this 0
push constant 4
sub
pop this 0
label IF_END6
label IF_END4
push this 0
push this 10
gt
not
if-goto IF_TRUE7
goto IF_FALSE7
label IF_TRUE7
push constant 1
pop this 14
push this 10
pop this 0
label IF_FALSE7
push this 0
push this 11
lt
not
if-goto IF_TRUE8
goto IF_FALSE8
label IF_TRUE8
push constant 2
pop this 14
push this 11
pop this 0
label IF_FALSE8
push this 1
push this 12
gt
not
if-goto IF_TRUE9
goto IF_FALSE9
label IF_TRUE9
push constant 3
pop this 14
push this 12
pop this 1
label IF_FALSE9
push this 1
push this 13
lt
not
if-goto IF_TRUE10
goto IF_FALSE10
label IF_TRUE10
push constant 4
pop this 14
push this 13
pop this 1
label IF_FALSE10
push pointer 0
call Ball.show 1
pop temp 0
push this 14
return
function Ball.bounce 5
push argument 0
pop pointer 0
push this 2
push constant 10
call Math.divide 2
pop local 2
push this 3
push constant 10
call Math.divide 2
pop local 3
push argument 1
push constant 0
eq
if-goto IF_TRUE0
goto IF_FALSE0
label IF_TRUE0
push constant 10
pop local 4
goto IF_END0
label IF_FALSE0
push this 2
push constant 0
lt
not
push argument 1
push constant 1
eq
and
push this 2
push constant 0
lt
push argument 1
push constant 1
neg
eq
and
or
if-goto IF_TRUE1
goto IF_FALSE1
label IF_TRUE1
push constant 20
pop local 4
goto IF_END1
label IF_FALSE1
push constant 5
pop local 4
label IF_END1
label IF_END0
push this 14
push constant 1
eq
if-goto IF_TRUE2
goto IF_FALSE2
label IF_TRUE2
push constant 506
pop local 0
push local 3
push constant 50
neg
call Math.multiply 2
push local 2
call Math.divide 2
pop local 1
push this 1
push local 1
push local 4
call Math.multiply 2
add
pop local 1
goto IF_END2
label IF_FALSE2
push this 14
push constant 2
eq
if-goto IF_TRUE3
goto IF_FALSE3
label IF_TRUE3
push constant 0
pop local 0
push local 3
push constant 50
call Math.multiply 2
push local 2
call Math.divide 2
pop local 1
push this 1
push local 1
push local 4
call Math.multiply 2
add
pop local 1
goto IF_END3
label IF_FALSE3
push this 14
push constant 3
eq
if-goto IF_TRUE4
goto IF_FALSE4
label IF_TRUE4
push constant 250
pop local 1
push local 2
push constant 25
neg
call Math.multiply 2
push local 3
call Math.divide 2
pop local 0
push this 0
push local 0
push local 4
call Math.multiply 2
add
pop local 0
goto IF_END4
label IF_FALSE4
push constant 0
pop local 1
push local 2
push constant 25
call Math.multiply 2
push local 3
call Math.divide 2
pop local 0
push this 0
push local 0
push local 4
call Math.multiply 2
add
pop local 0
label IF_END4
label IF_END3
label IF_END2
push pointer 0
push local 0
push local 1
call Ball.setDestination 3
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
