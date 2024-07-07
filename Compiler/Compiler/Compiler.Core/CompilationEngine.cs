using Compiler.Core.Enums;
using Compiler.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Compiler.Core;

public class CompilationEngine
{
    private ITokenizer _tokenizer;
    private ILogger _logger;
    private int _indentationLevel;

    public Token CurrentToken { get; private set; }
    public LinkedList<string> XmlLines { get; private set; }
    
    public CompilationEngine(ITokenizer tokenizer, ILogger logger)
    {
        _logger = logger;
        _tokenizer = tokenizer;
        XmlLines = new LinkedList<string>();
        _indentationLevel = 0;
        CurrentToken = null!;
    }

    public void BeginCompilationRoutine()
    {
        _logger.LogInformation("Beginning compilation routine...");
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
        TokenToXmlLine(CurrentToken, TokenType.Keyword, true, ["class"]);
        // main
        TokenToXmlLine(CurrentToken, TokenType.Identifier, false);
        // {
        TokenToXmlLine(CurrentToken, TokenType.Symbol, true, ["{"]);
        // field int example;
        // function int exampleFunction(int x, bool y)
        while (true)
        {
            if (CurrentToken.TokenValue is "static" or "field")
            {
                ClassVariableDeclaration();
            } else if (CurrentToken.TokenValue is "constructor" or "function" or "method")
            {
                ClassSubroutineDeclaration();
            }
            else
            {
                break;
            }
        }
        // }
        TokenToXmlLine(CurrentToken, TokenType.Symbol, true, ["}"]);
        _indentationLevel--;
        WriteXmlLine(true, "class");
    }

    private void ClassVariableDeclaration()
    {
        WriteXmlLine(false, "classVariableDeclaration");
        _indentationLevel++;
        // static
        TokenToXmlLine(CurrentToken, TokenType.Keyword, true, ["static", "field"]);
        // int
        TokenToXmlLine(CurrentToken, TokenType.Identifier, false);
        // x, y, z
        do
        {
            TokenToXmlLine(CurrentToken, TokenType.Identifier, false);
            if (CurrentToken.TokenValue != ",")
            {
                break;
            }
            TokenToXmlLine(CurrentToken, TokenType.Symbol, true, [","]);
        } while (true);
        // ;
        TokenToXmlLine(CurrentToken, TokenType.Symbol, true, [";"]);
        _indentationLevel--;
        WriteXmlLine(true, "classVariableDeclaration");
    }

    private void ClassSubroutineDeclaration()
    {
        WriteXmlLine(false, "subroutineDeclaration");
        _indentationLevel++;
        // function
        TokenToXmlLine(CurrentToken, TokenType.Keyword, true, ["constructor", "function", "method"]);
        // void
        TokenToXmlLine(CurrentToken, TokenType.Keyword, true, ["int", "bool", "char", "void"]);
        // exampleFunction
        TokenToXmlLine(CurrentToken, TokenType.Identifier, false);
        // (
        TokenToXmlLine(CurrentToken, TokenType.Symbol, true, ["("]);
        WriteXmlLine(false, "parameterList");
        _indentationLevel++;
        // int x, bool y
        while (true)
        {
            if (CurrentToken.TokenValue != ")")
            {
                TokenToXmlLine(CurrentToken, TokenType.Keyword, true, ["int", "bool", "char"]);
                TokenToXmlLine(CurrentToken, TokenType.Identifier, false);
            }
            if (CurrentToken.TokenValue == ",")
            {
                TokenToXmlLine(CurrentToken, TokenType.Symbol, true, [","]);
            }
            else if (CurrentToken.TokenValue == ")")
            {
                break;
            }
        }
        _indentationLevel--;
        WriteXmlLine(false, "parameterList");
        // )
        TokenToXmlLine(CurrentToken, TokenType.Symbol, true, [")"]);
        
        _indentationLevel--;
        WriteXmlLine(true, "subroutineDeclaration");
        WriteXmlLine(false, "subroutineBody");
        
        WriteXmlLine(true, "subroutineBody");
    }

    private void TokenToXmlLine(Token token, TokenType expectedTokenType, bool checkValues, List<string>? expectedValues = null, bool advanceToken = true)
    {
        if (checkValues && expectedValues == null)
        {
            _logger.LogCritical("Expected values cannot be null when checking token values");
            TerminateCompilationRoutine();
        }
        if (token.TokenType != expectedTokenType)
        {
            _logger.LogError($"Token - {token.TokenValue} did not have the expected Tokentype. Expected: {expectedTokenType}, Actual: {token.TokenType}");
            TerminateCompilationRoutine();
        }
        if (checkValues && !expectedValues!.Contains(token.TokenValue))
        {
            _logger.LogError($"Token - {token.TokenValue} did not have the expected TokenValue. Expected in: {string.Join("", expectedValues)}, Actual: {token.TokenValue}");
            TerminateCompilationRoutine();
        }

        var indentationSpacing = new string('\t', _indentationLevel);
        var resultingString = $"{indentationSpacing}<{token.TokenType}> {token.TokenValue} </{token.TokenType}>";
        XmlLines.AddLast(resultingString);

        if (advanceToken)
        {
            NextToken();
        }
    }

    private void NextToken()
    {
        _tokenizer.Advance();
        CurrentToken = _tokenizer.CurrentToken;
    }

    private void WriteXmlLine(bool closingTag, string line)
    {
        var indentationSpacing = new string('\t', _indentationLevel);
        XmlLines.AddLast(!closingTag ? $"{indentationSpacing}< {line} >" : $"{indentationSpacing}</ {line} >");
    }

    private void TerminateCompilationRoutine()
    {
        _logger.LogCritical("Encountered an unrecoverable error. Exiting...");
        // Environment.Exit(1);
    }
}