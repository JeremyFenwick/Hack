using Compiler.Core;
using Compiler.Core.Enums;

namespace Compiler.Test;

public class TokenizerTesting
{
    [Test]
    public void SingleLineTokenizerTest()
    {
        var codeList = new List<string>
        {
            "let x = 100;"
        };

        var tokenizer = new Tokenizer(codeList);
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.Token.TokenValue, Is.EquivalentTo("let"));
            Assert.That(tokenizer.Token.TokenType, Is.EqualTo(TokenType.Keyword));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.Token.TokenValue, Is.EquivalentTo("x"));
            Assert.That(tokenizer.Token.TokenType, Is.EqualTo(TokenType.Identifier));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.Token.TokenValue, Is.EquivalentTo("="));
            Assert.That(tokenizer.Token.TokenType, Is.EqualTo(TokenType.Symbol));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.Token.TokenValue, Is.EquivalentTo("100"));
            Assert.That(tokenizer.Token.TokenType, Is.EqualTo(TokenType.IntConst));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.Token.TokenValue, Is.EquivalentTo(";"));
            Assert.That(tokenizer.Token.TokenType, Is.EqualTo(TokenType.Symbol));
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

        var tokenizer = new Tokenizer(codeList);

        for (int i = 0; i < 5; i++)
        {
            tokenizer.Advance();
        }
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.Token.TokenValue, Is.EquivalentTo("let"));
            Assert.That(tokenizer.Token.TokenType, Is.EqualTo(TokenType.Keyword));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.Token.TokenValue, Is.EquivalentTo("y"));
            Assert.That(tokenizer.Token.TokenType, Is.EqualTo(TokenType.Identifier));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.Token.TokenValue, Is.EquivalentTo("="));
            Assert.That(tokenizer.Token.TokenType, Is.EqualTo(TokenType.Symbol));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.Token.TokenValue, Is.EquivalentTo("hello   World"));
            Assert.That(tokenizer.Token.TokenType, Is.EqualTo(TokenType.StringConst));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.Token.TokenValue, Is.EquivalentTo(";"));
            Assert.That(tokenizer.Token.TokenType, Is.EqualTo(TokenType.Symbol));
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

        var tokenizer = new Tokenizer(codeList);
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.Token.TokenValue, Is.EquivalentTo("function"));
            Assert.That(tokenizer.Token.TokenType, Is.EqualTo(TokenType.Keyword));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.Token.TokenValue, Is.EquivalentTo("void"));
            Assert.That(tokenizer.Token.TokenType, Is.EqualTo(TokenType.Keyword));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.Token.TokenValue, Is.EquivalentTo("main"));
            Assert.That(tokenizer.Token.TokenType, Is.EqualTo(TokenType.Identifier));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.Token.TokenValue, Is.EquivalentTo("("));
            Assert.That(tokenizer.Token.TokenType, Is.EqualTo(TokenType.Symbol));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.Token.TokenValue, Is.EquivalentTo(")"));
            Assert.That(tokenizer.Token.TokenType, Is.EqualTo(TokenType.Symbol));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.Token.TokenValue, Is.EquivalentTo("{"));
            Assert.That(tokenizer.Token.TokenType, Is.EqualTo(TokenType.Symbol));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.Token.TokenValue, Is.EquivalentTo("let"));
            Assert.That(tokenizer.Token.TokenType, Is.EqualTo(TokenType.Keyword));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.Token.TokenValue, Is.EquivalentTo("a"));
            Assert.That(tokenizer.Token.TokenType, Is.EqualTo(TokenType.Identifier));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.Token.TokenValue, Is.EquivalentTo("["));
            Assert.That(tokenizer.Token.TokenType, Is.EqualTo(TokenType.Symbol));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.Token.TokenValue, Is.EquivalentTo("i"));
            Assert.That(tokenizer.Token.TokenType, Is.EqualTo(TokenType.Identifier));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });

        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.Token.TokenValue, Is.EquivalentTo("]"));
            Assert.That(tokenizer.Token.TokenType, Is.EqualTo(TokenType.Symbol));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.Token.TokenValue, Is.EquivalentTo("="));
            Assert.That(tokenizer.Token.TokenType, Is.EqualTo(TokenType.Symbol));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.Token.TokenValue, Is.EquivalentTo("Keyboard"));
            Assert.That(tokenizer.Token.TokenType, Is.EqualTo(TokenType.Identifier));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.Token.TokenValue, Is.EquivalentTo("."));
            Assert.That(tokenizer.Token.TokenType, Is.EqualTo(TokenType.Symbol));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.Token.TokenValue, Is.EquivalentTo("readInt"));
            Assert.That(tokenizer.Token.TokenType, Is.EqualTo(TokenType.Identifier));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.Token.TokenValue, Is.EquivalentTo("("));
            Assert.That(tokenizer.Token.TokenType, Is.EqualTo(TokenType.Symbol));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.Token.TokenValue, Is.EquivalentTo("ENTER THE NEXT NUMBER: "));
            Assert.That(tokenizer.Token.TokenType, Is.EqualTo(TokenType.StringConst));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.Token.TokenValue, Is.EquivalentTo(")"));
            Assert.That(tokenizer.Token.TokenType, Is.EqualTo(TokenType.Symbol));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });

        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.Token.TokenValue, Is.EquivalentTo(";"));
            Assert.That(tokenizer.Token.TokenType, Is.EqualTo(TokenType.Symbol));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.Token.TokenValue, Is.EquivalentTo("}"));
            Assert.That(tokenizer.Token.TokenType, Is.EqualTo(TokenType.Symbol));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(false));
        // "function void main() {",
        // "let a[i] = Keyboard.readInt(\"ENTER THE NEXT NUMBER: \");",
        // "}",
        // tokenizer.Advance();
        // Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(false));
    }
    
    [Test]
    public void TestSkipNewlineCharacters()
    {
        var codeList = new List<string>
        {
            "let x = \"Hello \nWorld\";"
        };

        var tokenizer = new Tokenizer(codeList);
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.Token.TokenValue, Is.EquivalentTo("let"));
            Assert.That(tokenizer.Token.TokenType, Is.EqualTo(TokenType.Keyword));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.Token.TokenValue, Is.EquivalentTo("x"));
            Assert.That(tokenizer.Token.TokenType, Is.EqualTo(TokenType.Identifier));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.Token.TokenValue, Is.EquivalentTo("="));
            Assert.That(tokenizer.Token.TokenType, Is.EqualTo(TokenType.Symbol));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.Token.TokenValue, Is.EquivalentTo("Hello World"));
            Assert.That(tokenizer.Token.TokenType, Is.EqualTo(TokenType.StringConst));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.Multiple(() =>
        {
            Assert.That(tokenizer.Token.TokenValue, Is.EquivalentTo(";"));
            Assert.That(tokenizer.Token.TokenType, Is.EqualTo(TokenType.Symbol));
            Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(true));
        });
        
        tokenizer.Advance();
        Assert.That(tokenizer.HasMoreTokens, Is.EqualTo(false));
    }
}