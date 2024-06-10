using Assembler;

// Arg1 is the input file (it must be an asm file)
var selectedFile = args[0];
List<string> rawAssembly;
// Check the file extension
if (Path.GetExtension(selectedFile) != ".asm")
{
    throw new Exception("The input file must be an assembly (asm) file!");
}
// Attempt to load the file
try
{
    rawAssembly = File.ReadLines(selectedFile).ToList();
}
catch (Exception e)
{
    throw new Exception(e.Message);
}
// Now generate the binary code
var defaultSymbolTable = new SymbolTable();
// Set up the parser
var parser = new Parser(rawAssembly, defaultSymbolTable);
var parsedCode = parser.GetParsedAssembly();
var symbolTableWithLabels = parser.GetSymbolTableWithLabels();
// Set up the code generator
var codeGen = new CodeGenerator(symbolTableWithLabels, parsedCode);
// Get the binary code
var binaryCode = codeGen.GetBinaryCode();
// Write the code to a .hack file
System.IO.File.WriteAllLines(Path.GetPathRoot(selectedFile) + 
                             Path.GetFileNameWithoutExtension(selectedFile) + 
                             ".hack", binaryCode);
