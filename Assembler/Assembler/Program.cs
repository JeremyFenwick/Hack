using Assembler;

// Arg1 is the input file (it must be an asm file)
string selectedFile;
try
{
    selectedFile = args[0];

}
catch
{
    selectedFile = "";
}
List<string> rawAssembly;

// Get the path from the console
if (string.IsNullOrEmpty(selectedFile))
{
    Console.WriteLine("Enter an assembly file. You must provide the full path (i.e. C:\\Documents\\Example.asm)");
    do
    {
        var userInput = Console.ReadLine();
        if (string.IsNullOrEmpty(userInput)) continue;
        selectedFile = userInput;
        break;
    } while (true);
}

// Check the file extension
if (Path.GetExtension(selectedFile) != ".asm")
{
    throw new Exception("The input file extension must be assembly (.asm)!");
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
System.IO.File.WriteAllLines(Path.GetDirectoryName(selectedFile) + "\\" +
                             Path.GetFileNameWithoutExtension(selectedFile) + 
                             ".hack", binaryCode);
