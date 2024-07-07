using Compiler.Core;

namespace Compiler.Test;

public class Tests
{
    public Core.Tokenizer SetupTokenizer(List<string> codeList)
    {
        // using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
        // var logger = factory.CreateLogger<Tests>();
        var tokenizer = new Core.Tokenizer(codeList);
        return tokenizer;
    }
    
    [Test]
    public void SimpleCommentRemoval()
    {
        var codeList = new List<string>();
        codeList.Add("// First test comment");
        codeList.Add("let x = 100;");

        var tokenizer = SetupTokenizer(codeList);

        Assert.That(tokenizer.CurrentLine, Is.EquivalentTo("let x = 100;"));
    }
    
    [Test]
    public void ComplexCommentRemoval()
    {
        var codeList = new List<string>();
        codeList.Add("/** First test comment */");
        codeList.Add("let x = 100;");

        var tokenizer = SetupTokenizer(codeList);
        Assert.That(tokenizer.CurrentLine, Is.EquivalentTo("let x = 100;"));
    }
    
    [Test]
    public void MultiLineComplexCommentRemoval()
    {
        var codeList = new List<string>();
        codeList.Add("/** First test comment");
        codeList.Add("Second test comment");
        codeList.Add("Third test comment */");
        codeList.Add("let x = 100;");

        var tokenizer = SetupTokenizer(codeList);

        Assert.That(tokenizer.CurrentLine, Is.EquivalentTo("let x = 100;"));
    }
    
    [Test]
    public void InlineComplexCommentRemoval()
    {
        var codeList = new List<string>();
        codeList.Add("/** First test comment");
        codeList.Add("Second test comment");
        codeList.Add("Third test comment */let x = 100;");

        var tokenizer = SetupTokenizer(codeList);
        Assert.That(tokenizer.CurrentLine, Is.EquivalentTo("let x = 100;"));
    }
    
    [Test]
    public void SecondInlineComplexCommentRemoval()
    {
        var codeList = new List<string>();
        codeList.Add("let x = 100;/** First test comment");
        codeList.Add("Second test comment");
        codeList.Add("Third test comment */let y = 200;");

        var tokenizer = SetupTokenizer(codeList);
        Assert.That(tokenizer.CurrentLine, Is.EquivalentTo("let x = 100;"));
    }
    
    [Test]
    public void DoubleSpacingTests()
    {
        var codeList = new List<string>();
        codeList.Add("let x = \" hello  world \"");

        var tokenizer = SetupTokenizer(codeList);
        Assert.That(tokenizer.CurrentLine, Is.EquivalentTo("let x = \" hello  world \""));
    }
}