using Assembler;

namespace AssemblerTests;

public class CInstructionTests
{
    private CodeGenerator SetUpCodeGen()
    {
        var dummyCode = new List<string>()
        {
            "HELLO",
            "WORLD",
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
    public void CompOnlyInstruction()
    {
        var codeGen = SetUpCodeGen();
        var result = codeGen.CInstructionToBinary("A+1");
        Assert.That(result, Is.EqualTo("1110110111000000"));
    }
    
    [Test]
    public void CompAndDestInstruction()
    {
        var codeGen = SetUpCodeGen();
        var result = codeGen.CInstructionToBinary("D=A+1");
        Assert.That(result, Is.EqualTo("1110110111010000"));
    }
    
    [Test]
    public void FullInstruction()
    {
        var codeGen = SetUpCodeGen();
        var result = codeGen.CInstructionToBinary("ADM=D+M;JMP");
        Assert.That(result, Is.EqualTo("1111000010111111"));
    }
}