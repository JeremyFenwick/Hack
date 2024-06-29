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
// push constant 4000
@4000
D=A
@SP
M=M+1
A=M-1
M=D
// pop pointer 0
@SP
M=M-1
A=M
D=M
@THIS
M=D
// push constant 5000
@5000
D=A
@SP
M=M+1
A=M-1
M=D
// pop pointer 1
@SP
M=M-1
A=M
D=M
@THAT
M=D
// call Sys.main
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
@Sys.main
0;JMP
// Set return label
(UNIQUERETURN-2)
// pop temp 1
@SP
M=M-1
A=M
D=M
@6
M=D
// label LOOP
(LOOP)
@LOOP
0;JMP
// function Sys.main 5
(Sys.main)
@0
D=A
@SP
M=M+1
A=M-1
M=D
@0
D=A
@SP
M=M+1
A=M-1
M=D
@0
D=A
@SP
M=M+1
A=M-1
M=D
@0
D=A
@SP
M=M+1
A=M-1
M=D
@0
D=A
@SP
M=M+1
A=M-1
M=D
// push constant 4001
@4001
D=A
@SP
M=M+1
A=M-1
M=D
// pop pointer 0
@SP
M=M-1
A=M
D=M
@THIS
M=D
// push constant 5001
@5001
D=A
@SP
M=M+1
A=M-1
M=D
// pop pointer 1
@SP
M=M-1
A=M
D=M
@THAT
M=D
// push constant 200
@200
D=A
@SP
M=M+1
A=M-1
M=D
// pop local 1
@LCL
D=M
@1
D=D+A
@R13
M=D
@SP
M=M-1
A=M
D=M
@R13
A=M
M=D
// push constant 40
@40
D=A
@SP
M=M+1
A=M-1
M=D
// pop local 2
@LCL
D=M
@2
D=D+A
@R13
M=D
@SP
M=M-1
A=M
D=M
@R13
A=M
M=D
// push constant 6
@6
D=A
@SP
M=M+1
A=M-1
M=D
// pop local 3
@LCL
D=M
@3
D=D+A
@R13
M=D
@SP
M=M-1
A=M
D=M
@R13
A=M
M=D
// push constant 123
@123
D=A
@SP
M=M+1
A=M-1
M=D
// call Sys.add12 1
// PUSH RETURNADDRESS
@UNIQUERETURN-3
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
@1
D=D-A
@ARG
M=D
// Reposition LCL
@SP
D=M
@LCL
M=D
// Goto function
@Sys.add12
0;JMP
// Set return label
(UNIQUERETURN-3)
// pop temp 0
@SP
M=M-1
A=M
D=M
@5
M=D
// push local 0
@0
D=A
@LCL
A=M+D
D=M
@SP
M=M+1
A=M-1
M=D
// push local 1
@1
D=A
@LCL
A=M+D
D=M
@SP
M=M+1
A=M-1
M=D
// push local 2
@2
D=A
@LCL
A=M+D
D=M
@SP
M=M+1
A=M-1
M=D
// push local 3
@3
D=A
@LCL
A=M+D
D=M
@SP
M=M+1
A=M-1
M=D
// push local 4
@4
D=A
@LCL
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
// add
@SP
M=M-1
A=M
D=M
A=A-1
M=D+M
// add
@SP
M=M-1
A=M
D=M
A=A-1
M=D+M
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
// function Sys.add12 0
(Sys.add12)
// push constant 4002
@4002
D=A
@SP
M=M+1
A=M-1
M=D
// pop pointer 0
@SP
M=M-1
A=M
D=M
@THIS
M=D
// push constant 5002
@5002
D=A
@SP
M=M+1
A=M-1
M=D
// pop pointer 1
@SP
M=M-1
A=M
D=M
@THAT
M=D
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
// push constant 12
@12
D=A
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
