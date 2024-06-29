// Set new Local-3 to R13
@LCL
D=M // D = 1017
@3
D=D+A
@R13
M=D
// Pop the stack
@0
M=M-1
A=M
D=M
// Set R13 to popped value
@R13
A=M
M=D






