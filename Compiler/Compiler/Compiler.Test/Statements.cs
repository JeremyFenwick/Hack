using Compiler.Core;
using Compiler.Core.SyntaxAnalyzer;
using Microsoft.Extensions.Logging;

namespace Compiler.Test;

public class Statements
{
    private CompilationEngine GenerateCompilationEngine(List<string> codeList)
    {
        using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = factory.CreateLogger<Tests>();
        var tokenizer = new Core.SyntaxAnalyzer.Tokenizer(codeList, logger);
        return new CompilationEngine(tokenizer, logger);
    }

    [Test]
    public void ReturnStatementTest()
    {
        var codeList = new List<string>
        {
            "class main {",
            "    function void main() {",
            "        var int x, y;",
            "        return x;",
            "    }",
            "}"
        };
        var compilationEngine = GenerateCompilationEngine(codeList);
        compilationEngine.BeginCompilationRoutine();
        Assert.That(compilationEngine.XmlLines.Last.Value, Is.EquivalentTo("</class>"));
    }
    
    [Test]
    public void DoStatementTest()
    {
        var codeList = new List<string>
        {
            "class main {",
            "    function void main() {",
            "        do exampleFunction(x, y);",
            "    }",
            "}"
        };
        var compilationEngine = GenerateCompilationEngine(codeList);
        compilationEngine.BeginCompilationRoutine();
        Assert.That(compilationEngine.XmlLines.Last.Value, Is.EquivalentTo("</class>"));
    }
    
    [Test]
    public void AlternateDoStatementTest()
    {
        var codeList = new List<string>
        {
            "class main {",
            "    function void main() {",
            "        do otherClass.exampleFunction(x, y);",
            "    }",
            "}"
        };
        var compilationEngine = GenerateCompilationEngine(codeList);
        compilationEngine.BeginCompilationRoutine();
        Assert.That(compilationEngine.XmlLines.Last.Value, Is.EquivalentTo("</class>"));
    }
    
    [Test]
    public void WhileStatementTest()
    {
        var codeList = new List<string>
        {
            "class main {",
            "    function void main() {",
            "    while (true) {",
            "            do exampleFunction();",
            "        }",
            "    }",
            "}"
        };
        var compilationEngine = GenerateCompilationEngine(codeList);
        compilationEngine.BeginCompilationRoutine();
        Assert.That(compilationEngine.XmlLines.Last.Value, Is.EquivalentTo("</class>"));
    }
    
    [Test]
    public void IfStatementTest()
    {
        var codeList = new List<string>
        {
            "class main {",
            "    function void main() {",
            "        if ( x = 100 ) {",
            "            do exampleFunction();",
            "        }",
            "        else {",
            "            return x;",
            "        }",
            "    }",
            "}"
        };
        var compilationEngine = GenerateCompilationEngine(codeList);
        compilationEngine.BeginCompilationRoutine();
        Assert.That(compilationEngine.XmlLines.Last.Value, Is.EquivalentTo("</class>"));
    }
    
    [Test]
    public void AdvancedIfStatementTest()
    {
        var codeList = new List<string>
        {
            "class main {",
            "    function void main() {",
            "        if ( x[3] > 100 ) {",
            "            do exampleFunction();",
            "        }",
            "        else {",
            "            return x + y[0];",
            "        }",
            "    }",
            "}"
        };
        var compilationEngine = GenerateCompilationEngine(codeList);
        compilationEngine.BeginCompilationRoutine();
        Assert.That(compilationEngine.XmlLines.Last.Value, Is.EquivalentTo("</class>"));
    }
    
    [Test]
    public void LetStatementTest()
    {
        var codeList = new List<string>
        {
            "class main {",
            "    function void main() {",
            "        let x = 100;",
            "        let y[100] = x > 150;",
            "        return x;",
            "    }",
            "}"
        };
        var compilationEngine = GenerateCompilationEngine(codeList);
        compilationEngine.BeginCompilationRoutine();
        Assert.That(compilationEngine.XmlLines.Last.Value, Is.EquivalentTo("</class>"));
    }
    [Test]
    public void LetStringStatementTest()
    {
        var codeList = new List<string>
        {
            "class main {",
            "    function void main() {",
            "        let x = \"HELLO WORLD\";",
            "        return x;",
            "    }",
            "}"
        };
        var compilationEngine = GenerateCompilationEngine(codeList);
        compilationEngine.BeginCompilationRoutine();
        Assert.That(compilationEngine.XmlLines.Last.Value, Is.EquivalentTo("</class>"));
    }
}