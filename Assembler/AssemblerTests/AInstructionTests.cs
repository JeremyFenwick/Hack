using Assembler;

namespace AssemblerTests;

public class AInstructionTests
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
    public void SimpleAInstruction()
    {
        var codeGen = SetUpCodeGen();
        var result = codeGen.AInstructionToBinary("@112");
        Assert.That(result, Is.EqualTo("0000000001110000"));
    }
    
    [Test]
    public void CheckCreationOfNewVariables()
    {
        var codeGen = SetUpCodeGen();
        var result = codeGen.AInstructionToBinary("@newvar");
        Assert.That(result, Is.EqualTo("0000000000010000"));
        var secondResult = codeGen.AInstructionToBinary("@newvar");
        Assert.That(secondResult, Is.EqualTo("0000000000010000"));
        var thirdResult = codeGen.AInstructionToBinary("@secondnewvar");
        Assert.That(thirdResult, Is.EqualTo("0000000000010001"));
    }
}