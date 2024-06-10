using System.Diagnostics;
using Assembler;

namespace AssemblerTests;

public class Tests
{
    [Test]
    public void RemoveComments()
    {
        var dummyCode = new List<string>()
        {
            "// First comment",
            "HELLO //   Second comment",
            "WORLD  // Third comment"
        };
        var expectedCode = new List<string>()
        {
            "HELLO",
            "WORLD"
        };
        
        var defaultSymbolTable = new SymbolTable();
        var parser = new Parser(dummyCode, defaultSymbolTable);
        var parsedCode = parser.GetParsedAssembly();
        
        Assert.Multiple(() =>
        {
            Assert.That(parsedCode[0], Is.EqualTo(expectedCode[0]));
            Assert.That(parsedCode[1], Is.EqualTo(expectedCode[1]));
        });
    }

    [Test]
    public void LabelParsing()
    {
        var dummyCode = new List<string>()
        {
            "// First comment",
            "(START)",
            "HELLO //   Second comment",
            "(END)",
            "// Third comment",
            "WORLD  // Fourth comment"
        };
        var expectedCode = new List<string>()
        {
            "HELLO",
            "WORLD"
        };
        
        var defaultSymbolTable = new SymbolTable();
        var parser = new Parser(dummyCode, defaultSymbolTable);
        var parsedCode = parser.GetParsedAssembly();
        var symbolTableWithLabels = parser.GetSymbolTableWithLabels();
        
        Assert.Multiple(() =>
        {
            Assert.That(parsedCode[0], Is.EqualTo(expectedCode[0]));
            Assert.That(parsedCode[1], Is.EqualTo(expectedCode[1]));
            Assert.That(symbolTableWithLabels.GetSymbolValue("START"), Is.EqualTo(0));
            Assert.That(symbolTableWithLabels.GetSymbolValue("END"), Is.EqualTo(1));
        });
    }
}