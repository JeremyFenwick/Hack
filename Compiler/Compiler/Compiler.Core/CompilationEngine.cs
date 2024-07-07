using Compiler.Core.Enums;
using Compiler.Core.Interfaces;

namespace Compiler.Core;

public class CompilationEngine
{
    private ITokenizer _tokenizer;
    // private ILogger _logger;
    private int _indentationLevel;
    private readonly string[] _statementKeywords = ["let", "if", "while", "do", "return"];
    
    public Token CurrentToken { get; private set; }
    public LinkedList<string> XmlLines { get; private set; }
    
    public CompilationEngine(ITokenizer tokenizer)
    {
        // _logger = logger;
        _tokenizer = tokenizer;
        XmlLines = new LinkedList<string>();
        _indentationLevel = 0;
        CurrentToken = null!;
    }

    public void BeginCompilationRoutine()
    {
        // _logger.LogInformation("Beginning compilation routine...");
        while (_tokenizer.HasMoreTokens)
        {
            NextToken();
            CompileClass();
        }
    }

    private void CompileClass()
    {
        WriteXmlLine(false, "class");
        _indentationLevel++;
        // class
        TokenToXmlLine(CurrentToken, TokenType.Keyword, "class");
        // main
        TokenToXmlLine(CurrentToken, TokenType.Identifier);
        // {
        TokenToXmlLine(CurrentToken, TokenType.Symbol, "{");
        // field int example;
        // function int exampleFunction(int x, bool y)
        while (true)
        {
            if (CurrentToken.TokenValue is "static" or "field")
            {
                ClassVariableDeclaration();
            } 
            else if (CurrentToken.TokenValue is "constructor" or "function" or "method")
            {
                ClassSubroutineDeclaration();
            }
            else
            {
                break;
            }
        }
        // }
        TokenToXmlLine(CurrentToken, TokenType.Symbol, "}");
        _indentationLevel--;
        WriteXmlLine(true, "class");
    }

    private void ClassVariableDeclaration()
    {
        WriteXmlLine(false, "classVariableDeclaration");
        _indentationLevel++;
        // static
        TokenToXmlLine(CurrentToken, TokenType.Keyword, ["static", "field"]);
        // int or className
        TokenToXmlLine(CurrentToken);
        // x, y, z
        do
        {
            TokenToXmlLine(CurrentToken, TokenType.Identifier);
            if (CurrentToken.TokenValue != ",")
            {
                break;
            }
            TokenToXmlLine(CurrentToken, TokenType.Symbol, ",");
        } while (true);
        // ;
        TokenToXmlLine(CurrentToken, TokenType.Symbol, ";");
        _indentationLevel--;
        WriteXmlLine(true, "classVariableDeclaration");
    }

    private void ClassSubroutineDeclaration()
    {
        WriteXmlLine(false, "subroutineDeclaration");
        _indentationLevel++;
        // function
        TokenToXmlLine(CurrentToken, TokenType.Keyword, ["constructor", "function", "method"]);
        // void
        TokenToXmlLine(CurrentToken, TokenType.Keyword, ["int", "bool", "char", "void"]);
        // exampleFunction
        TokenToXmlLine(CurrentToken, TokenType.Identifier);
        // (
        TokenToXmlLine(CurrentToken, TokenType.Symbol, "(");
        WriteXmlLine(false, "parameterList");
        _indentationLevel++;
        // int x, bool y
        while (true)
        {
            if (CurrentToken.TokenValue != ")")
            {
                TokenToXmlLine(CurrentToken, TokenType.Keyword, ["int", "bool", "char"]);
                TokenToXmlLine(CurrentToken, TokenType.Identifier);
            }
            if (CurrentToken.TokenValue == ",")
            {
                TokenToXmlLine(CurrentToken, TokenType.Symbol, ",");
            }
            else if (CurrentToken.TokenValue == ")")
            {
                break;
            }
        }
        _indentationLevel--;
        WriteXmlLine(false, "parameterList");
        // )
        TokenToXmlLine(CurrentToken, TokenType.Symbol, ")");
        
        _indentationLevel--;
        WriteXmlLine(true, "subroutineDeclaration");
        SubroutineBody();
    }

    private void SubroutineBody()
    {
        WriteXmlLine(false, "subroutineBody");
        _indentationLevel++;
        TokenToXmlLine(CurrentToken, TokenType.Symbol, "{");
        
        // Handle variable declarations
        WriteXmlLine(false, "variableDeclaration");
        while (true)
        {
            if (CurrentToken.TokenValue !=  "var")
            {
                break;
            }
            
            SubroutineVariableDeclaration();
        }
        WriteXmlLine(true, "variableDeclaration");
        
        // Handle statements
        WriteXmlLine(false, "statements");
        while (true)
        {
            if (!_statementKeywords.Contains(CurrentToken.TokenValue))
            {
                break;
            }
            StatementSplitter();
        }
        WriteXmlLine(true, "statements");

        TokenToXmlLine(CurrentToken, TokenType.Symbol, "}");
        _indentationLevel--;
        WriteXmlLine(true, "subroutineBody");
    }

    private void StatementSplitter() 
    {
        switch (CurrentToken.TokenValue)
        {
            case "let":
                LetStatement();
                break;
            case "if":
                IfStatement();
                break;
            case "while":
                WhileStatement();
                break;
            case "do":
                DoStatement();
                break;
            case "return":
                ReturnStatement();
                break;
            default:
                throw new Exception("Unknown statement type. Cannot be split");
        }
    }
    
    private void ReturnStatement()
    {
        throw new NotImplementedException();
    }

    private void DoStatement()
    {
        throw new NotImplementedException();
    }

    private void WhileStatement()
    {
        throw new NotImplementedException();
    }

    private void IfStatement()
    {
        throw new NotImplementedException();
    }

    private void LetStatement()
    {
        throw new NotImplementedException();
    }

    private void SubroutineVariableDeclaration()
    {
        WriteXmlLine(false, "variableDeclaration");
        _indentationLevel++;
        
        TokenToXmlLine(CurrentToken, TokenType.Keyword, "var");
        TokenToXmlLine(CurrentToken, TokenType.Keyword, ["int", "bool", "char"]);
        do
        {
            TokenToXmlLine(CurrentToken, TokenType.Identifier);
            if (CurrentToken.TokenValue != ",")
            {
                break;
            }
            TokenToXmlLine(CurrentToken, TokenType.Symbol, ",");
        } while (true);
        TokenToXmlLine(CurrentToken, TokenType.Symbol, ";");

        _indentationLevel--;
        WriteXmlLine(true, "variableDeclaration");
    }
    private void TokenToXmlLine(Token token, TokenType expectedTokenType, string expectedValue)
    {
        if (token.TokenType != expectedTokenType)
        {
            throw new Exception($"Token - {token.TokenValue} did not have the expected TokenType. Expected: {expectedTokenType}, Actual: {token.TokenType}");
        }
        if (expectedValue != token.TokenValue)
        {
            throw new Exception($"Token - {token.TokenValue} did not have the expected TokenValue. Expected: {expectedValue}, Actual: {token.TokenValue}");
        }

        var indentationSpacing = new string('\t', _indentationLevel);
        var resultingString = $"{indentationSpacing}<{token.TokenType}> {token.TokenValue} </{token.TokenType}>";
        XmlLines.AddLast(resultingString);
        
        NextToken();
    }
    private void TokenToXmlLine(Token token, TokenType expectedTokenType, List<string> expectedValues)
    {
        if (token.TokenType != expectedTokenType)
        {
            throw new Exception($"Token - {token.TokenValue} did not have the expected Tokentype. Expected: {expectedTokenType}, Actual: {token.TokenType}");
        }
        if (!expectedValues.Contains(token.TokenValue))
        {
            throw new Exception($"Token - {token.TokenValue} did not have the expected TokenValue. Expected in: {string.Join("", expectedValues)}, Actual: {token.TokenValue}");
        }

        var indentationSpacing = new string('\t', _indentationLevel);
        var resultingString = $"{indentationSpacing}<{token.TokenType}> {token.TokenValue} </{token.TokenType}>";
        XmlLines.AddLast(resultingString);
        
        NextToken();
    }
    
    private void TokenToXmlLine(Token token, TokenType expectedTokenType)
    {
        if (token.TokenType != expectedTokenType)
        {
            throw new Exception($"Token - {token.TokenValue} did not have the expected Tokentype. Expected: {expectedTokenType}, Actual: {token.TokenType}");
        }

        var indentationSpacing = new string('\t', _indentationLevel);
        var resultingString = $"{indentationSpacing}<{token.TokenType}> {token.TokenValue} </{token.TokenType}>";
        XmlLines.AddLast(resultingString);
        
        NextToken();
    }

    private void TokenToXmlLine(Token token)
    {
        var indentationSpacing = new string('\t', _indentationLevel);
        var resultingString = $"{indentationSpacing}<{token.TokenType}> {token.TokenValue} </{token.TokenType}>";
        XmlLines.AddLast(resultingString);
        
        NextToken();
    }

    private void NextToken()
    {
        _tokenizer.Advance();
        CurrentToken = _tokenizer.CurrentToken;
    }

    private void WriteXmlLine(bool closingTag, string line)
    {
        var indentationSpacing = new string('\t', _indentationLevel);
        XmlLines.AddLast(!closingTag ? $"{indentationSpacing}<{line}>" : $"{indentationSpacing}</{line}>");
    }

    private void TerminateCompilationRoutine()
    {
        // _logger.LogCritical("Encountered an unrecoverable error. Exiting...");
        // Environment.Exit(1);
    }
}