using Compiler.Core;
using Compiler.Core.SyntaxAnalyzer;
using Microsoft.Extensions.Logging;

namespace Compiler.Test;

public class ClassCompilation
{
    private XmlCompilationEngine GenerateCompilationEngine(List<string> codeList)
    {
        using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = factory.CreateLogger<Tests>();
        var tokenizer = new Core.SyntaxAnalyzer.Tokenizer(codeList, logger);
        return new XmlCompilationEngine(tokenizer, logger);
    }

    [Test]
    public void SimpleClassTest()
    {
        var codeList = new List<string>
        {
            "class main {",
            "field bool x;",
            "}"
        };
        var compilationEngine = GenerateCompilationEngine(codeList);
        compilationEngine.BeginCompilationRoutine();
        Assert.That(compilationEngine.XmlLines.Last.Value, Is.EquivalentTo("</class>"));
    }
    
    [Test]
    public void SimpleClassTestWithMultipleVariables()
    {
        var codeList = new List<string>
        {
            "class main {",
            "field bool x, z, q;",
            "}"
        };
        var compilationEngine = GenerateCompilationEngine(codeList);
        compilationEngine.BeginCompilationRoutine();
        Assert.That(compilationEngine.XmlLines.Last.Value, Is.EquivalentTo("</class>"));
    }
    
    [Test]
    public void SimpleClassWithSubroutine()
    {
        var codeList = new List<string>
        {
            "class main {",
            "function int exampleFunction(int x, bool y) " +
            "   {" +
            "   }",
            "}"
        };
        var compilationEngine = GenerateCompilationEngine(codeList);
        compilationEngine.BeginCompilationRoutine();
        Assert.That(compilationEngine.XmlLines.Last.Value, Is.EquivalentTo("</class>"));
    }
    
    [Test]
    public void SimpleClassWithVariableDeclaration()
    {
        var codeList = new List<string>
        {
            "class main {",
            "function int exampleFunction(int x, bool y) " +
            "   {" +
            "       var bool x;" +
            "       var int y, z;" +
            "   }",
            "}"
        };
        var compilationEngine = GenerateCompilationEngine(codeList);
        compilationEngine.BeginCompilationRoutine();
        Assert.That(compilationEngine.XmlLines.Last.Value, Is.EquivalentTo("</class>"));
    }
}