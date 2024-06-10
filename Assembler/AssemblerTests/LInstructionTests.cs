using Assembler;

namespace AssemblerTests;

public class LInstructionTests
{
    private CodeGenerator SetUpCodeGen()
    {
        var dummyCode = new List<string>()
        {
            "D=M",
            "(START)",
            "(LOOP)",
            "@KBD",
            "@SCREEN",
            "D=A",
            "(NEXT-LOOP)",
            "@KBD"
        };
        // Set up the symbol table
        var defaultSymbolTable = new SymbolTable();
        // Set up the parser
        var parser = new Parser(dummyCode, defaultSymbolTable);
        var parsedCode = parser.GetParsedAssembly();
        var symbolTableWithLabels = parser.GetSymbolTableWithLabels();
        // Set up the code generator
        return new CodeGenerator(symbolTableWithLabels, parsedCode);
    }
    
    [Test]
    public void FirstInstruction()
    {
        var codeGen = SetUpCodeGen();
        var result = codeGen.LInstructionToBinary("@START");
        Assert.That(result, Is.EqualTo("0000000000000001"));
    }
    
    [Test]
    public void SecondInstruction()
    {
        var codeGen = SetUpCodeGen();
        var result = codeGen.LInstructionToBinary("@NEXT-LOOP");
        Assert.That(result, Is.EqualTo("0000000000000100"));
    }
}