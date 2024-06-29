// Set RAM[0] to 256
@256
D=A
// Now use *RAM[0] and set it to 100
// We are putting 100 on the stack at RAM[256]
@0
M=D
@100
D=A
@0
A=M
M=D
// Now increment the pointer at RAM[0]
@0
M=M+1