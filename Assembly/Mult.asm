// Multiply two numbers
// PSEUDOCODE (C#)
// val x = 4;
// val y = 5;
// int result = 0;
// for (var i = 0, i < 4, i++)
// {
//   result + y;
// }

// RAM[0] and RAM[1] contain the numbers to be multiplied
// RAM[2] contains the result

// Set the x value
@0
D=M
@xvalue
M=D

// Set the y value
@1
D=M
@yvalue
M=D

// Set the result
@result
M=0

// Begin the counter
(MULTIPLY)
// Check if x value is 0. If so, go to the end
@xvalue
D=M
@END
D;JEQ
// Add the y value to the result
@yvalue
D=M
@result
M=D+M

// Remove one from the x value
@xvalue
M=M-1
// Continue the loop
@MULTIPLY
0;JMP

(END)
@END
0;JMP