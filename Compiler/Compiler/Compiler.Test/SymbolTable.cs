using Compiler.Core.SyntaxAnalyzer;
using Microsoft.Extensions.Logging;

namespace Compiler.Test;

public class SymbolTable
{
    private VmCompilationEngine GenerateCompilationEngine(List<string> codeList)
    {
        using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = factory.CreateLogger<Tests>();
        var tokenizer = new Core.SyntaxAnalyzer.Tokenizer(codeList, logger);
        return new VmCompilationEngine(tokenizer, logger, true);
    }
    
    [Test]
    public void SimpleFieldTest()
    {
        var codeList = new List<string>
        {
            "class main {",
            "field bool x;",
            "}"
        };
        var compilationEngine = GenerateCompilationEngine(codeList);
        compilationEngine.BeginCompilationRoutine();
        
        Assert.That(compilationEngine.ClassSymbolTable.GetSymbol("x"), !Is.Null);
    }
    
    [Test]
    public void AdvancedFieldTest()
    {
        var codeList = new List<string>
        {
            "class main {",
            "field bool x, y;",
            "static array a, b;",
            "}"
        };
        var compilationEngine = GenerateCompilationEngine(codeList);
        compilationEngine.BeginCompilationRoutine();
        
        Assert.Multiple(() =>
        {
            Assert.That(compilationEngine.ClassSymbolTable.GetSymbol("x"), !Is.Null);
            Assert.That(compilationEngine.ClassSymbolTable.GetSymbol("y"), !Is.Null);
            Assert.That(compilationEngine.ClassSymbolTable.GetSymbol("a"), !Is.Null);
            Assert.That(compilationEngine.ClassSymbolTable.GetSymbol("b"), !Is.Null);
        });
    }
    
    [Test]
    public void SimpleSubroutineVariableTest()
    {
        var codeList = new List<string>
        {
            "class main {",
            "function int exampleFunction(int x, bool y) ", 
            "    {",
            "        var Array a, b;",
            "        var Bool c, d;",
            "    }", 
            "}"
        };
        var compilationEngine = GenerateCompilationEngine(codeList);
        compilationEngine.BeginCompilationRoutine();
        
        Assert.Multiple(() =>
        {
            Assert.That(compilationEngine.SubroutineSymbolTable.GetSymbol("x"), !Is.Null);
            Assert.That(compilationEngine.SubroutineSymbolTable.GetSymbol("y"), !Is.Null);
            Assert.That(compilationEngine.SubroutineSymbolTable.GetSymbol("a"), !Is.Null);
            Assert.That(compilationEngine.SubroutineSymbolTable.GetSymbol("b"), !Is.Null);
            Assert.That(compilationEngine.SubroutineSymbolTable.GetSymbol("c"), !Is.Null);
            Assert.That(compilationEngine.SubroutineSymbolTable.GetSymbol("d"), !Is.Null);
        });
    }
    
    [Test]
    public void AdvancedSubroutineVariableTest()
    {
        var codeList = new List<string>
        {
            "class main {",
            "field bool x, y;",
            "function int exampleFunction(int x, bool y) ", 
            "    {",
            "        var Array a, b;",
            "        var Bool c, d;",
            "    }", 
            "}"
        };
        var compilationEngine = GenerateCompilationEngine(codeList);
        compilationEngine.BeginCompilationRoutine();
        
        Assert.Multiple(() =>
        {
            Assert.That(compilationEngine.ClassSymbolTable.GetSymbol("x"), !Is.Null);
            Assert.That(compilationEngine.ClassSymbolTable.GetSymbol("y"), !Is.Null);
            Assert.That(compilationEngine.SubroutineSymbolTable.GetSymbol("x"), !Is.Null);
            Assert.That(compilationEngine.SubroutineSymbolTable.GetSymbol("y"), !Is.Null);
            Assert.That(compilationEngine.SubroutineSymbolTable.GetSymbol("a"), !Is.Null);
            Assert.That(compilationEngine.SubroutineSymbolTable.GetSymbol("b"), !Is.Null);
            Assert.That(compilationEngine.SubroutineSymbolTable.GetSymbol("c"), !Is.Null);
            Assert.That(compilationEngine.SubroutineSymbolTable.GetSymbol("d"), !Is.Null);
        });
    }
}