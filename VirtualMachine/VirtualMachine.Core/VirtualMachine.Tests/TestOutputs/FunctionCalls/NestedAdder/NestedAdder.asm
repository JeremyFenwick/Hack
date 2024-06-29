// SP = 256
@256
D=A
@SP
M=D
// call Sys.init
// PUSH RETURNADDRESS
@UNIQUERETURN-1
D=A
@SP
M=M+1
A=M-1
M=D
// PUSH LCL
@LCL
D=M
@SP
M=M+1
A=M-1
M=D
// PUSH ARG
@ARG
D=M
@SP
M=M+1
A=M-1
M=D
// PUSH THIS
@THIS
D=M
@SP
M=M+1
A=M-1
M=D
// PUSH THAT
@THAT
D=M
@SP
M=M+1
A=M-1
M=D
// Reposition ARG
@SP
D=M
@5
D=D-A
@0
D=D-A
@ARG
M=D
// Reposition LCL
@SP
D=M
@LCL
M=D
// Goto function
@Sys.init
0;JMP
// Set return label
(UNIQUERETURN-1)
// function Sys.init 0
(Sys.init)
// call Main.main
// PUSH RETURNADDRESS
@UNIQUERETURN-2
D=A
@SP
M=M+1
A=M-1
M=D
// PUSH LCL
@LCL
D=M
@SP
M=M+1
A=M-1
M=D
// PUSH ARG
@ARG
D=M
@SP
M=M+1
A=M-1
M=D
// PUSH THIS
@THIS
D=M
@SP
M=M+1
A=M-1
M=D
// PUSH THAT
@THAT
D=M
@SP
M=M+1
A=M-1
M=D
// Reposition ARG
@SP
D=M
@5
D=D-A
@0
D=D-A
@ARG
M=D
// Reposition LCL
@SP
D=M
@LCL
M=D
// Goto function
@Main.main
0;JMP
// Set return label
(UNIQUERETURN-2)
// label LOOP
(LOOP)
@LOOP
0;JMP
@LOOP
0;JMP
@LOOP
0;JMP
// function Main.main 0
(Main.main)
// push constant 4000
@4000
D=A
@SP
M=M+1
A=M-1
M=D
// push constant 2000
@2000
D=A
@SP
M=M+1
A=M-1
M=D
// push constant 3000
@3000
D=A
@SP
M=M+1
A=M-1
M=D
// call Main.SecondAdder 2
// PUSH RETURNADDRESS
@UNIQUERETURN-1
D=A
@SP
M=M+1
A=M-1
M=D
// PUSH LCL
@LCL
D=M
@SP
M=M+1
A=M-1
M=D
// PUSH ARG
@ARG
D=M
@SP
M=M+1
A=M-1
M=D
// PUSH THIS
@THIS
D=M
@SP
M=M+1
A=M-1
M=D
// PUSH THAT
@THAT
D=M
@SP
M=M+1
A=M-1
M=D
// Reposition ARG
@SP
D=M
@5
D=D-A
@2
D=D-A
@ARG
M=D
// Reposition LCL
@SP
D=M
@LCL
M=D
// Goto function
@Main.SecondAdder
0;JMP
// Set return label
(UNIQUERETURN-1)
// add
@SP
M=M-1
A=M
D=M
A=A-1
M=D+M
// return
// Set R13 to endFrame (LCL)
@LCL
D=M
@R13
M=D
// Set R14 to returnAddress endFrame - 5
@R13
D=M
@5
D=D-A
@R14
M=D
@R14
A=M
D=M
@R15
M=D
// Pop the stack and place the value in arg 0
@SP
M=M-1
A=M
D=M
@ARG
A=M
M=D
// Set the stack pointer to ARG + 1 (continues from above)
@ARG
A=M
D=A+1
@SP
M=D
// Restore THAT from endFrame - 1
@R13
M=M-1
A=M
D=M
@THAT
M=D
// Restore THIS from endFrame - 2
@R13
M=M-1
A=M
D=M
@THIS
M=D
// Restore ARG from endFrame - 3
@R13
M=M-1
A=M
D=M
@ARG
M=D
// Restore LCL from endFrame - 4
@R13
M=M-1
A=M
D=M
@LCL
M=D
// GOTO returnAddress (stored in @R15)
@R15
A=M
0;JMP
// function Main.SecondAdder 0
(Main.SecondAdder)
// push argument 0
@0
D=A
@ARG
A=M+D
D=M
@SP
M=M+1
A=M-1
M=D
// push argument 1
@1
D=A
@ARG
A=M+D
D=M
@SP
M=M+1
A=M-1
M=D
// add
@SP
M=M-1
A=M
D=M
A=A-1
M=D+M
// return
// Set R13 to endFrame (LCL)
@LCL
D=M
@R13
M=D
// Set R14 to returnAddress endFrame - 5
@R13
D=M
@5
D=D-A
@R14
M=D
@R14
A=M
D=M
@R15
M=D
// Pop the stack and place the value in arg 0
@SP
M=M-1
A=M
D=M
@ARG
A=M
M=D
// Set the stack pointer to ARG + 1 (continues from above)
@ARG
A=M
D=A+1
@SP
M=D
// Restore THAT from endFrame - 1
@R13
M=M-1
A=M
D=M
@THAT
M=D
// Restore THIS from endFrame - 2
@R13
M=M-1
A=M
D=M
@THIS
M=D
// Restore ARG from endFrame - 3
@R13
M=M-1
A=M
D=M
@ARG
M=D
// Restore LCL from endFrame - 4
@R13
M=M-1
A=M
D=M
@LCL
M=D
// GOTO returnAddress (stored in @R15)
@R15
A=M
0;JMP
