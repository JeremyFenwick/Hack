using Compiler.Core;
using Compiler.Core.Enums;
using Microsoft.Extensions.Logging;

namespace Compiler.Test;

public class TokenizerTesting
{
    public Tokenizer SetupTokenizer(List<string> codeList)
    {
        using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = factory.CreateLogger<Tests>();
        var tokenizer = new Tokenizer(codeList, logger);
        return tokenizer;
    }
    
    [Test]
    public void SingleLineTokenizerTest()
    {
        var codeList = new List<string>
        {
            "let x = 100;"
        };

        var tokenizer = SetupTokenizer(codeList);
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.CurrentToken.TokenValue, Is.EquivalentTo("let"));
            Assert.That(tokenizer.CurrentToken.TokenType, Is.EqualTo(TokenType.Keyword));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.CurrentToken.TokenValue, Is.EquivalentTo("x"));
            Assert.That(tokenizer.CurrentToken.TokenType, Is.EqualTo(TokenType.Identifier));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.CurrentToken.TokenValue, Is.EquivalentTo("="));
            Assert.That(tokenizer.CurrentToken.TokenType, Is.EqualTo(TokenType.Symbol));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.CurrentToken.TokenValue, Is.EquivalentTo("100"));
            Assert.That(tokenizer.CurrentToken.TokenType, Is.EqualTo(TokenType.IntConst));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.CurrentToken.TokenValue, Is.EquivalentTo(";"));
            Assert.That(tokenizer.CurrentToken.TokenType, Is.EqualTo(TokenType.Symbol));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(false));
    }
    
    [Test]
    public void MultiLineTokenizerTest()
    {
        var codeList = new List<string>
        {
            "let x = 100;",
            "let y = \"hello   World\";",
            
        };

        var tokenizer = SetupTokenizer(codeList);

        for (int i = 0; i < 5; i++)
        {
            tokenizer.Advance();
        }
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.CurrentToken.TokenValue, Is.EquivalentTo("let"));
            Assert.That(tokenizer.CurrentToken.TokenType, Is.EqualTo(TokenType.Keyword));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.CurrentToken.TokenValue, Is.EquivalentTo("y"));
            Assert.That(tokenizer.CurrentToken.TokenType, Is.EqualTo(TokenType.Identifier));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.CurrentToken.TokenValue, Is.EquivalentTo("="));
            Assert.That(tokenizer.CurrentToken.TokenType, Is.EqualTo(TokenType.Symbol));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.CurrentToken.TokenValue, Is.EquivalentTo("hello   World"));
            Assert.That(tokenizer.CurrentToken.TokenType, Is.EqualTo(TokenType.StringConst));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.CurrentToken.TokenValue, Is.EquivalentTo(";"));
            Assert.That(tokenizer.CurrentToken.TokenType, Is.EqualTo(TokenType.Symbol));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(false));
    }
    
    [Test]
    public void AdvancedMultiLineTokenizerTest()
    {
        var codeList = new List<string>
        {
            "function void main() { // FIRST COMMENT ",
            "/** SECOND COMMENT */",
            "let a[i] = Keyboard.readInt(\"ENTER THE NEXT NUMBER: \");",
            "}",
        };

        var tokenizer = SetupTokenizer(codeList);
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.CurrentToken.TokenValue, Is.EquivalentTo("function"));
            Assert.That(tokenizer.CurrentToken.TokenType, Is.EqualTo(TokenType.Keyword));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.CurrentToken.TokenValue, Is.EquivalentTo("void"));
            Assert.That(tokenizer.CurrentToken.TokenType, Is.EqualTo(TokenType.Keyword));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.CurrentToken.TokenValue, Is.EquivalentTo("main"));
            Assert.That(tokenizer.CurrentToken.TokenType, Is.EqualTo(TokenType.Identifier));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.CurrentToken.TokenValue, Is.EquivalentTo("("));
            Assert.That(tokenizer.CurrentToken.TokenType, Is.EqualTo(TokenType.Symbol));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.CurrentToken.TokenValue, Is.EquivalentTo(")"));
            Assert.That(tokenizer.CurrentToken.TokenType, Is.EqualTo(TokenType.Symbol));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.CurrentToken.TokenValue, Is.EquivalentTo("{"));
            Assert.That(tokenizer.CurrentToken.TokenType, Is.EqualTo(TokenType.Symbol));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.CurrentToken.TokenValue, Is.EquivalentTo("let"));
            Assert.That(tokenizer.CurrentToken.TokenType, Is.EqualTo(TokenType.Keyword));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.CurrentToken.TokenValue, Is.EquivalentTo("a"));
            Assert.That(tokenizer.CurrentToken.TokenType, Is.EqualTo(TokenType.Identifier));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.CurrentToken.TokenValue, Is.EquivalentTo("["));
            Assert.That(tokenizer.CurrentToken.TokenType, Is.EqualTo(TokenType.Symbol));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.CurrentToken.TokenValue, Is.EquivalentTo("i"));
            Assert.That(tokenizer.CurrentToken.TokenType, Is.EqualTo(TokenType.Identifier));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });

        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.CurrentToken.TokenValue, Is.EquivalentTo("]"));
            Assert.That(tokenizer.CurrentToken.TokenType, Is.EqualTo(TokenType.Symbol));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.CurrentToken.TokenValue, Is.EquivalentTo("="));
            Assert.That(tokenizer.CurrentToken.TokenType, Is.EqualTo(TokenType.Symbol));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.CurrentToken.TokenValue, Is.EquivalentTo("Keyboard"));
            Assert.That(tokenizer.CurrentToken.TokenType, Is.EqualTo(TokenType.Identifier));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.CurrentToken.TokenValue, Is.EquivalentTo("."));
            Assert.That(tokenizer.CurrentToken.TokenType, Is.EqualTo(TokenType.Symbol));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.CurrentToken.TokenValue, Is.EquivalentTo("readInt"));
            Assert.That(tokenizer.CurrentToken.TokenType, Is.EqualTo(TokenType.Identifier));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.CurrentToken.TokenValue, Is.EquivalentTo("("));
            Assert.That(tokenizer.CurrentToken.TokenType, Is.EqualTo(TokenType.Symbol));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.CurrentToken.TokenValue, Is.EquivalentTo("ENTER THE NEXT NUMBER: "));
            Assert.That(tokenizer.CurrentToken.TokenType, Is.EqualTo(TokenType.StringConst));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.CurrentToken.TokenValue, Is.EquivalentTo(")"));
            Assert.That(tokenizer.CurrentToken.TokenType, Is.EqualTo(TokenType.Symbol));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });

        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.CurrentToken.TokenValue, Is.EquivalentTo(";"));
            Assert.That(tokenizer.CurrentToken.TokenType, Is.EqualTo(TokenType.Symbol));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.CurrentToken.TokenValue, Is.EquivalentTo("}"));
            Assert.That(tokenizer.CurrentToken.TokenType, Is.EqualTo(TokenType.Symbol));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(false));
    }
    
    [Test]
    public void TestSkipNewlineCharacters()
    {
        var codeList = new List<string>
        {
            "let x = \"Hello \nWorld\";"
        };

        var tokenizer = SetupTokenizer(codeList);
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.CurrentToken.TokenValue, Is.EquivalentTo("let"));
            Assert.That(tokenizer.CurrentToken.TokenType, Is.EqualTo(TokenType.Keyword));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.CurrentToken.TokenValue, Is.EquivalentTo("x"));
            Assert.That(tokenizer.CurrentToken.TokenType, Is.EqualTo(TokenType.Identifier));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.CurrentToken.TokenValue, Is.EquivalentTo("="));
            Assert.That(tokenizer.CurrentToken.TokenType, Is.EqualTo(TokenType.Symbol));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.CurrentToken.TokenValue, Is.EquivalentTo("Hello World"));
            Assert.That(tokenizer.CurrentToken.TokenType, Is.EqualTo(TokenType.StringConst));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.CurrentToken.TokenValue, Is.EquivalentTo(";"));
            Assert.That(tokenizer.CurrentToken.TokenType, Is.EqualTo(TokenType.Symbol));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(false));
    }
}