// When a key is pressed, fill the screen
// PSEUDOCODE (C#)
// while 
// {
//   if (KEYBOARD > 0)  
//   { 
//     var Data = -1
//     for register in Screen
//     {
//       register = -1
//     } 
//   }
//   else 
//   {
//       break;
//   }
// } (true)

// Start the loop
(LOOP) 
// Check if the keyboard is currently pressed. If so go to blackscreen, else go to whitescreen
@KBD
D=M
@BLACKSCREEN
D;JGT
@WHITESCREEN
D;JMP

(BLACKSCREEN)
// Set the target register. When it hits the final register break the loop. 8192 registers in total
@SCREEN
D=A
@targetregister
M=D

(BLACKOUT)
// Check if we are done 
@24576
D=A
@targetregister
D=D-M
@LOOP
D;JLE
// Turn the next register black
@targetregister
A=M
M=-1

// Add to the counter
@targetregister
M=M+1
@BLACKOUT
D;JMP

(WHITESCREEN)
// Set the target register. When it hits the final register break the loop. 8192 registers in total
@SCREEN
D=A
@targetregister
M=D

(WHITEOUT)
// Check if we are done 
@24576
D=A
@targetregister
D=D-M
@LOOP
D;JLE
// Turn the next register white
@targetregister
A=M
M=0

// Add to the counter
@targetregister
M=M+1
@WHITEOUT
D;JMP