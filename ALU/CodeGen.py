for x in range(2, 16):
  print(f"FullAdder(a=a[{x}], b=b[{x}], c=tempCarry{x}, sum=out[{x}], carry=tempCarry{x+1});")

for x in range(1, 16):
  print(f"HalfAdder(a=in[{x}], b=tempCarry{x}, sum=out[{x}], carry=tempCarry{x+1});")
