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
    
    [Test]
    public void SimpleLetTest()
    {
        var codeList = new List<string>
        {
            "class main {",
            "    function void main() {",
            "        var int x;",
            "        let x = 100;",
            "        return x;",
            "    }",
            "}"
        };
        var compilationEngine = GenerateCompilationEngine(codeList);
        compilationEngine.BeginCompilationRoutine();
        
        Assert.That(compilationEngine.CodeLines.Last!.Value, Is.EqualTo("return"));
    }
    
    [Test]
    public void BooleanLetTest()
    {
        var codeList = new List<string>
        {
            "class main {",
            "    function void main() {",
            "        var bool x, y;",
            "        let x = true;",
            "        let y = false;",
            "        return;",
            "    }",
            "}"
        };
        var compilationEngine = GenerateCompilationEngine(codeList);
        compilationEngine.BeginCompilationRoutine();
        
        Assert.That(compilationEngine.CodeLines.Last!.Value, Is.EqualTo("return"));
    }
    
    [Test]
    public void IfStatementTest()
    {
        var codeList = new List<string>
        {
            "class main {",
            "    function void main() {",
            "        var int x;",
            "        if (true) {",
            "            let x = 100;",
            "        }",
            "        else {",
            "            let x = 200;",
            "        }",
            "        return x;",
            "    }",
            "}"
        };
        var compilationEngine = GenerateCompilationEngine(codeList);
        compilationEngine.BeginCompilationRoutine();
        
        Assert.That(compilationEngine.CodeLines.Last!.Value, Is.EqualTo("return"));
    }
    
    [Test]
    public void WhileStatementTest()
    {
        var codeList = new List<string>
        {
            "class main {",
            "    function void main() {",
            "        while (2 > 1) {",
            "            do Output.printInt(1);",
            "        }",
            "        return;",
            "    }",
            "}"
        };
        var compilationEngine = GenerateCompilationEngine(codeList);
        compilationEngine.BeginCompilationRoutine();
        
        Assert.That(compilationEngine.CodeLines.Last!.Value, Is.EqualTo("return"));
    }
    
    [Test]
    public void ConstructorTest()
    {
        var codeList = new List<string>
        {
            "class Point {",
            "    field int x, y;",
            "    constructor Point new(int ax, int ay) {",
            "        let x = ax;",
            "        let y = ay;",
            "        return this;",
            "    }",
            "}"
        };
        var compilationEngine = GenerateCompilationEngine(codeList);
        compilationEngine.BeginCompilationRoutine();
        
        Assert.That(compilationEngine.CodeLines.Last!.Value, Is.EqualTo("return"));
    }
    
    [Test]
    public void ConstructorWithStaticsTest()
    {
        var codeList = new List<string>
        {
            "class Point {",
            "    field int x, y;",
            "    static int pointCount;",
            "    constructor Point new(int ax, int ay) {",
            "        let x = ax;",
            "        let y = ay;",
            "        let pointCount = pointCount + 1;",
            "        return this;",
            "    }",
            "}"
        };
        var compilationEngine = GenerateCompilationEngine(codeList);
        compilationEngine.BeginCompilationRoutine();
        
        Assert.That(compilationEngine.CodeLines.Last!.Value, Is.EqualTo("return"));
    }
}