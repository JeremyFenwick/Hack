using Compiler.Core.SyntaxAnalyzer;
using Microsoft.Extensions.Logging;

namespace Compiler.Test;

public class VmCompilation
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
            "    function void main() {",
            "        do Output.printInt(1 + (2 * 3));",
            "        return;",
            "    }",
            "}"
        };
        var compilationEngine = GenerateCompilationEngine(codeList);
        compilationEngine.BeginCompilationRoutine();
        
        Assert.That(compilationEngine.CodeLines.Last!.Value, Is.EqualTo("return"));
    }
}