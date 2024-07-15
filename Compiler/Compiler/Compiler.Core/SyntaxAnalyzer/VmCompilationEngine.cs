using Compiler.Core.Enums;
using Compiler.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.CompilerServices;

namespace Compiler.Core.SyntaxAnalyzer;

public class VmCompilationEngine : ICompilationEngine
{
    private ITokenizer _tokenizer;
    private ILogger _logger;
    private byte _indentationLevel;
    private readonly List<string> _statementKeywords = ["let", "if", "while", "do", "return"];
    private readonly List<string> _ops = ["+", "-", "*", "/", "&", "|", "<", ">", "="];
    private readonly bool _debug;
    public Token CurrentToken { get; private set; }
    public LinkedList<string> CodeLines { get; private set; }
    public SymbolTable ClassSymbolTable { get; private set; }
    public SymbolTable SubroutineSymbolTable { get; private set; }

    public VmCompilationEngine(ITokenizer tokenizer, ILogger logger, bool debug = false)
    {
        _logger = logger;
        _tokenizer = tokenizer;
        CodeLines = new LinkedList<string>();
        _indentationLevel = 0;
        CurrentToken = null!;
        ClassSymbolTable = new SymbolTable();
        SubroutineSymbolTable = new SymbolTable();
        _debug = debug;
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
        // class
        NextToken();
        // main
        var className = CurrentToken.TokenValue;
        NextToken();
        // {
        NextToken();
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
                ClassSubroutineDeclaration(className);
            }
            else
            {
                break;
            }
        }
        // }
        NextToken();
        if (!_debug)
        {
            ClassSymbolTable.Reset();
        }
    }

    private void ClassVariableDeclaration()
    {
        // static or field
        var variableKind = CurrentToken.TokenValue == "static" ? Kind.Static : Kind.Field;
        NextToken();
        // int or className
        var variableType = CurrentToken.TokenValue;
        NextToken();
        // x, y, z
        do
        {
            ClassSymbolTable.AddSymbol(CurrentToken.TokenValue, variableKind, variableType);
            NextToken();
            if (CurrentToken.TokenValue != ",")
            {
                break;
            }
            NextToken();
        } while (true);
        // ;
        NextToken();
    }

    private void ClassSubroutineDeclaration(string className)
    {
        SubroutineSymbolTable.AddSymbol("this", Kind.Argument, className);
        // function
        NextToken();
        // void
        NextToken();
        // exampleFunction
        NextToken();
        // (
        NextToken();
        // int x, bool y
        while (true)
        {
            if (CurrentToken.TokenValue != ")")
            {
                var argumentType = CurrentToken.TokenValue;
                NextToken();
                SubroutineSymbolTable.AddSymbol(CurrentToken.TokenValue, Kind.Argument, argumentType);
                NextToken();
            }
            if (CurrentToken.TokenValue == ",")
            {
                NextToken();
            }
            else if (CurrentToken.TokenValue == ")")
            {
                break;
            }
        }
        // )
        NextToken();
        
        SubroutineBody();

        if (!_debug)
        {
            SubroutineSymbolTable.Reset();
        }
    }

    private void SubroutineBody()
    {
        WriteXmlLine(false, "subroutineBody");
        _indentationLevel++;
        TokenToXmlLine(CurrentToken, TokenType.Symbol, "{");
        
        // Handle variable declarations
        while (true)
        {
            if (CurrentToken.TokenValue !=  "var")
            {
                break;
            }
            SubroutineVariableDeclaration();
        }
        // Handle statements
        
        if (_statementKeywords.Contains(CurrentToken.TokenValue))
        {
            StatementLoop();
        }
        
        TokenToXmlLine(CurrentToken, TokenType.Symbol, "}");
        _indentationLevel--;
        WriteXmlLine(true, "subroutineBody");
    }

    private void StatementLoop() 
    {
        WriteXmlLine(false, "statements");
        _indentationLevel++;
        
        do
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
                    throw new Exception("Unknown statement type");
            }
            if (CurrentToken.TokenValue == "}") break;
            
        } while (true);
            
        _indentationLevel--;
        WriteXmlLine(true, "statements");
    }
    
    private void ReturnStatement()
    {
        WriteXmlLine(false, "returnStatement");
        _indentationLevel++;
        
        TokenToXmlLine(CurrentToken, TokenType.Keyword, "return");
        // Handle the case where the return statement has an expression
        if (CurrentToken.TokenValue != ";")
        {
            Expression();
        }
        TokenToXmlLine(CurrentToken, TokenType.Symbol, ";");

        _indentationLevel--;
        WriteXmlLine(true, "returnStatement");
    }

    private void DoStatement()
    {
        WriteXmlLine(false, "doStatement");
        _indentationLevel++;
        
        TokenToXmlLine(CurrentToken, TokenType.Keyword, "do");
        // This function is LL(2) so we need to look ahead to know what to do
        var lastToken = CurrentToken;
        NextToken();

        // Handle subroutine call. Inlined for clarity as separating creates a mess
        if (CurrentToken.TokenValue == "(")
        {

            TokenToXmlLine(lastToken, false);
            TokenToXmlLine(CurrentToken, TokenType.Symbol, "(");
            ExpressionList();
            TokenToXmlLine(CurrentToken, TokenType.Symbol, ")");

        }
        // Handle alternate subroutine call
        else
        {

            TokenToXmlLine(lastToken, false);
            TokenToXmlLine(CurrentToken, TokenType.Symbol, ".");
            TokenToXmlLine(CurrentToken, TokenType.Identifier);
            TokenToXmlLine(CurrentToken, TokenType.Symbol, "(");
            ExpressionList();
            TokenToXmlLine(CurrentToken, TokenType.Symbol, ")");

        }
        TokenToXmlLine(CurrentToken, TokenType.Symbol, ";");

        _indentationLevel--;
        WriteXmlLine(true, "doStatement");
    }

    private void WhileStatement()
    {
        WriteXmlLine(false, "whileStatement");
        _indentationLevel++;

        TokenToXmlLine(CurrentToken, TokenType.Keyword, "while");
        TokenToXmlLine(CurrentToken, TokenType.Symbol, "(");
        Expression();
        TokenToXmlLine(CurrentToken, TokenType.Symbol, ")");
        TokenToXmlLine(CurrentToken, TokenType.Symbol, "{");
        
        StatementLoop();

        TokenToXmlLine(CurrentToken, TokenType.Symbol, "}");

        _indentationLevel--;
        WriteXmlLine(true, "whileStatement");
    }

    private void IfStatement()
    {
        WriteXmlLine(false, "ifStatement");
        _indentationLevel++;
        
        TokenToXmlLine(CurrentToken, TokenType.Keyword, "if");
        TokenToXmlLine(CurrentToken, TokenType.Symbol, "(");
        Expression();
        TokenToXmlLine(CurrentToken, TokenType.Symbol, ")");
        TokenToXmlLine(CurrentToken, TokenType.Symbol, "{");
        
        StatementLoop();

        TokenToXmlLine(CurrentToken, TokenType.Symbol, "}");
        if (CurrentToken.TokenValue == "else")
        {
            TokenToXmlLine(CurrentToken, TokenType.Keyword, "else");
            TokenToXmlLine(CurrentToken, TokenType.Symbol, "{");
            
            StatementLoop();

            TokenToXmlLine(CurrentToken, TokenType.Symbol, "}");
        }
        
        _indentationLevel--;
        WriteXmlLine(true, "ifStatement");       
    }

    private void LetStatement()
    {
        WriteXmlLine(false, "letStatement");
        _indentationLevel++;
        
        TokenToXmlLine(CurrentToken, TokenType.Keyword, "let");
        TokenToXmlLine(CurrentToken, TokenType.Identifier);
        if (CurrentToken.TokenValue == "[")
        {
            TokenToXmlLine(CurrentToken, TokenType.Symbol, "[");
            Expression();
            TokenToXmlLine(CurrentToken, TokenType.Symbol, "]");
        }
        TokenToXmlLine(CurrentToken, TokenType.Symbol, "=");
        Expression();
        TokenToXmlLine(CurrentToken, TokenType.Symbol, ";");
        
        _indentationLevel--;
        WriteXmlLine(true, "letStatement");        
    }
    
    private void Expression()
    {
        WriteXmlLine(false, "expression");
        _indentationLevel++;
        
        // There will be at least one term. Deal with the case where there is more than one with an op
        while (true)
        {
            Term();
            if (_ops.Contains(CurrentToken.TokenValue))
            {
                TokenToXmlLine(CurrentToken, TokenType.Symbol, _ops);
                continue;
            }
            break;
        }
        
        _indentationLevel--;
        WriteXmlLine(true, "expression");    
    }

    private void Term()
    {
        WriteXmlLine(false, "term");
        _indentationLevel++;
        // This function is LL(2) so we need to look ahead to know what to do
        var lastToken = CurrentToken;
        NextToken();
        // Handle integer constant
        if (lastToken.TokenType == TokenType.IntConst)
        {
            TokenToXmlLine(lastToken, false);
        } 
        // Handle string constant
        else if (lastToken.TokenType == TokenType.StringConst)
        {
            TokenToXmlLine(lastToken, false);
        }
        // Handle nested expression
        else if (lastToken.TokenValue == "(")
        {
            TokenToXmlLine(lastToken, false);
            Expression();
            TokenToXmlLine(CurrentToken, TokenType.Symbol, ")");
        }
        // Handle unaryOp
        else if (lastToken.TokenValue is "~" or "-")
        {
            TokenToXmlLine(lastToken, false);
            Term();
        }
        // Handle keyword constant
        else if (lastToken.TokenValue is "true" or "false" or "null" or "this")
        {
            TokenToXmlLine(lastToken, false);
        }
        // Handle indexed variable name
        else if (CurrentToken.TokenValue == "[")
        {
            TokenToXmlLine(lastToken, false);
            TokenToXmlLine(CurrentToken, TokenType.Symbol, "[");
            Expression();
            TokenToXmlLine(CurrentToken, TokenType.Symbol, "]");
        }
        // Inlined for clarity as separating creates a mess
        else if (CurrentToken.TokenValue == "(")
        {
            TokenToXmlLine(lastToken, false);
            TokenToXmlLine(CurrentToken, TokenType.Symbol, "(");
            ExpressionList();
            TokenToXmlLine(CurrentToken, TokenType.Symbol, ")");
        }
        // Handle alternate subroutine call
        else if (CurrentToken.TokenValue == ".")
        {
            TokenToXmlLine(lastToken, false);
            TokenToXmlLine(CurrentToken, TokenType.Symbol, ".");
            TokenToXmlLine(CurrentToken, TokenType.Identifier);
            TokenToXmlLine(CurrentToken, TokenType.Symbol, "(");
            ExpressionList();
            TokenToXmlLine(CurrentToken, TokenType.Symbol, ")");
        }
        // Handle the varName case
        else
        {
            TokenToXmlLine(lastToken, false);
        }
        _indentationLevel--;
        WriteXmlLine(true, "term");
    }

    private void ExpressionList()
    {
        WriteXmlLine(false, "expressionList");
        _indentationLevel++;

        if (CurrentToken.TokenValue != ")")
        {
            while (true)
            {
                Expression();
                if (CurrentToken.TokenValue == ",")
                {
                    TokenToXmlLine(CurrentToken, TokenType.Symbol, ",");
                    continue;
                }
                break;
            }
        }
        
        _indentationLevel--;
        WriteXmlLine(true, "expressionList");
    }

    private void SubroutineVariableDeclaration()
    {
        // var
        NextToken();
        // type (int)
        var variableType = CurrentToken.TokenValue;
        NextToken();
        do
        {
            // symbol name
            SubroutineSymbolTable.AddSymbol(CurrentToken.TokenValue, Kind.Variable, variableType);
            NextToken();
            if (CurrentToken.TokenValue != ",")
            {
                break;
            }

            NextToken();
        } while (true);
        NextToken();
    }
    
    private void TokenToXmlLine(Token token, TokenType expectedTokenType, string expectedValue)
    {
        if (token.TokenType != expectedTokenType)
        {
            TerminateCompilationRoutine("COMPILATION ENGINE: " + $"Token - {token.TokenValue} did not have the expected TokenType. Expected: {expectedTokenType}, Actual: {token.TokenType}");
        }
        if (expectedValue != token.TokenValue)
        {
            TerminateCompilationRoutine("COMPILATION ENGINE: " + $"Token - {token.TokenValue} did not have the expected TokenValue. Expected: {expectedValue}, Actual: {token.TokenValue}");
        }

        CodeLines.AddLast(XmlFormatter(token));
        _logger.LogDebug("COMPILATION ENGINE: " + $"Adding token to xml line: {CurrentToken.TokenValue}");
        NextToken();
    }
    
    private void TokenToXmlLine(Token token, TokenType expectedTokenType, List<string> expectedValues)
    {
        if (token.TokenType != expectedTokenType)
        {
            TerminateCompilationRoutine("COMPILATION ENGINE: " + $"Token - {token.TokenValue} did not have the expected Tokentype. Expected: {expectedTokenType}, Actual: {token.TokenType}");
        }
        if (!expectedValues.Contains(token.TokenValue))
        {
            TerminateCompilationRoutine("COMPILATION ENGINE: " + $"Token - {token.TokenValue} did not have the expected TokenValue. Expected in: {string.Join("", expectedValues)}, Actual: {token.TokenValue}");
        }

        CodeLines.AddLast(XmlFormatter(token));
        _logger.LogDebug("COMPILATION ENGINE: " + $"Adding token to xml line: {CurrentToken.TokenValue}");
        NextToken();
    }
    
    private void TokenToXmlLine(Token token, TokenType expectedTokenType)
    {
        if (token.TokenType != expectedTokenType)
        {
            throw new Exception($"Token - {token.TokenValue} did not have the expected Tokentype. Expected: {expectedTokenType}, Actual: {token.TokenType}");
        }

        CodeLines.AddLast(XmlFormatter(token));
        _logger.LogDebug("COMPILATION ENGINE: " + $"Adding token to xml line: {CurrentToken.TokenValue}");
        NextToken();
    }

    private void TokenToXmlLine(Token token)
    {
        CodeLines.AddLast(XmlFormatter(token));
        _logger.LogDebug("COMPILATION ENGINE: " + $"Adding token to xml line: {CurrentToken.TokenValue}");
        NextToken();
    }
    
    private void TokenToXmlLine(Token token, bool advanceToken)
    {
        CodeLines.AddLast(XmlFormatter(token));
        _logger.LogDebug("COMPILATION ENGINE: " + $"Adding token to xml line: {CurrentToken.TokenValue}");

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
        CodeLines.AddLast(!closingTag ? $"{indentationSpacing}<{line}>" : $"{indentationSpacing}</{line}>");
        _logger.LogDebug($"Adding token to xml line: {CurrentToken.TokenValue}");
    }

    private void TerminateCompilationRoutine(string message)
    {
        _logger.LogCritical("COMPILATION ENGINE: " + message);
        Console.ReadLine();
        Environment.Exit(1);
    }

    private string XmlFormatter(Token token)
    {
        var indentationSpacing = new string('\t', _indentationLevel);
        return $"{indentationSpacing}<{token.TokenType.ToString().ToLower()}> {token.TokenValue} </{token.TokenType.ToString().ToLower()}>";
    }
}