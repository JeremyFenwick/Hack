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
        TokenToXmlLine(CurrentToken, TokenType.Keyword, true, ["class"]);
        // Name of class
        TokenToXmlLine(CurrentToken, TokenType.Identifier, false);
        TokenToXmlLine(CurrentToken, TokenType.Symbol, true, ["{"]);

        while (true)
        {
            if (CurrentToken.TokenValue is "static" or "field")
            {
                ClassVariableDeclaration();
            }
            else
            {
                break;
            }
        }
        
        TokenToXmlLine(CurrentToken, TokenType.Symbol, true, ["}"]);
        _indentationLevel--;
        WriteXmlLine(true, "class");
    }

    private void ClassVariableDeclaration()
    {
        WriteXmlLine(false, "classVariableDeclaration");
        _indentationLevel++;
        TokenToXmlLine(CurrentToken, TokenType.Keyword, true, ["static", "field"]);
        // Type of field (bool, int, char or a className)
        TokenToXmlLine(CurrentToken, TokenType.Identifier, false);
        // Loop over the variables declared as there may be more than one
        do
        {
            TokenToXmlLine(CurrentToken, TokenType.Identifier, false);
            if (CurrentToken.TokenValue != ",")
            {
                break;
            }
            TokenToXmlLine(CurrentToken, TokenType.Symbol, true, [","]);
        } while (true);
        TokenToXmlLine(CurrentToken, TokenType.Symbol, true, [";"]);
        _indentationLevel--;
        WriteXmlLine(true, "classVariableDeclaration");
    }

    private void SubroutineDeclaration()
    {
        WriteXmlLine(false, "subroutineDeclaration");
        _indentationLevel++;
        
        TokenToXmlLine(CurrentToken, TokenType.Keyword, true, ["constructor", "function", "method"]);
        TokenToXmlLine(CurrentToken, TokenType.Identifier, true, ["int", "bool", "char", "void"]);
        // Subroutine name
        TokenToXmlLine(CurrentToken, TokenType.Identifier, false);
        TokenToXmlLine(CurrentToken, TokenType.Symbol, true, ["("]);
        // Parameter/s?
        while (true)
        {
            if (CurrentToken.TokenValue != ")")
            {
                
            }
            else
            {
                break;
            }
        }
        TokenToXmlLine(CurrentToken, TokenType.Symbol, true, ["("]);
        
        _indentationLevel--;
        WriteXmlLine(true, "subroutineDeclaration");
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
        Environment.Exit(1);
    }
}