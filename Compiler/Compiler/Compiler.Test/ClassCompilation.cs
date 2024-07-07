using Compiler.Core;
using Microsoft.Extensions.Logging;

namespace Compiler.Test;

public class ClassCompilation
{
    public CompilationEngine GenerateCompilationEngine(List<string> codeList)
    {
        using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = factory.CreateLogger<Tests>();
        var tokenizer = new Core.Tokenizer(codeList, logger);
        return new CompilationEngine(tokenizer, logger);
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
        Assert.That(compilationEngine.XmlLines.Last.Value, Is.EquivalentTo("</ class >"));
    }
    
    [Test]
    public void SimpleClassTestWithMultipleVariables()
    {
        var codeList = new List<string>
        {
            "class main {",
            "field bool x, z, q;",
            "static char y;",
            "}"
        };
        var compilationEngine = GenerateCompilationEngine(codeList);
        compilationEngine.BeginCompilationRoutine();
        Assert.That(compilationEngine.XmlLines.Last.Value, Is.EquivalentTo("</ class >"));
    }
}