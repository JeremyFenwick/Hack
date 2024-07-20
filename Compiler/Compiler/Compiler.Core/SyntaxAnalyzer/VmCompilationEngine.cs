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
    private readonly Dictionary<string, Command> _ops = new()
    {
        { "+", Command.Add },
        { "-", Command.Subtract },
        { "*", Command.Multiply },
        { "/", Command.Divide },
        { "&", Command.And },
        { "|", Command.Or },
        { "<", Command.LessThan },
        { ">", Command.GreaterThan },
        { "=", Command.Equal }
    };
    private readonly bool _debug;
    private (string className, string subroutineName, int Index) _labelTuple;
    
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
        _logger.LogInformation("Beginning compilation routine...");
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
        var variableKind = CurrentToken.TokenValue == "static" ? "static" : "field";
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
        // function or method or constructor
        var subroutine = CurrentToken.TokenValue;
        NextToken();
        // void
        NextToken();
        // exampleFunction
        var functionName = CurrentToken.TokenValue;
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
                SubroutineSymbolTable.AddSymbol(CurrentToken.TokenValue, "argument", argumentType);
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
        
        // Initialize the unique labels for loops
        _labelTuple.className = $"{className}";
        _labelTuple.subroutineName = $"{functionName}";
        _labelTuple.Index = 0;
        
        SubroutineBody(subroutine);

        if (!_debug)
        {
            SubroutineSymbolTable.Reset();
        }
    }

    private void SubroutineBody(string subroutineType)
    {
        if (subroutineType == "method")
        {
            SubroutineSymbolTable.AddSymbol("this", "argument", _labelTuple.className);
        }
        // {
        NextToken();
        // Handle variable declarations
        while (true)
        {
            if (CurrentToken.TokenValue !=  "var")
            {
                break;
            }
            SubroutineVariableDeclaration();
        }
        var commandString = CommandGen.Function($"{_labelTuple.className}.{_labelTuple.subroutineName}", SubroutineSymbolTable.NumberOfVariables());
        CodeLines.AddLast(commandString);
        if (subroutineType == "constructor")
        {
            // push the number of fields
            var numberOfFields = ClassSymbolTable.NumberOfFields().ToString();
            CodeLines.AddLast(CommandGen.PushConstant(numberOfFields));
            // use alloc to allocate memory
            CodeLines.AddLast(CommandGen.Call("Memory.alloc", 1));
            // set pointer 0 (the 'this' base address) to the return value
            CodeLines.AddLast(CommandGen.Pop("pointer", 0));
        }
        if (subroutineType == "method")
        {            
            // push the first argument onto the stack
            CodeLines.AddLast(CommandGen.Push("argument", 0));
            // pop argument 0 into pointer 0
            CodeLines.AddLast(CommandGen.Pop("pointer", 0));
        }
        // Handle statements
        
        if (_statementKeywords.Contains(CurrentToken.TokenValue))
        {
            StatementLoop();
        }
        // }
        NextToken();
    }

    private void StatementLoop() 
    {
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
    }
    
    private void ReturnStatement()
    {
        // return
        NextToken();
        // Handle the case where the return statement has an expression
        if (CurrentToken.TokenValue != ";")
        {
            Expression();
            CodeLines.AddLast(CommandGen.Return());
        }
        else
        {
            // add a dummy value to the stack, since we must return something
            CodeLines.AddLast(CommandGen.PushConstant("0"));
            CodeLines.AddLast(CommandGen.Return());
        }
        NextToken();
    }

    private void DoStatement()
    {
        // do
        NextToken();
        // This function is LL(2) so we need to look ahead to know what to do
        var lastToken = CurrentToken;
        NextToken();

        // Handle method call. Inlined for clarity as separating creates a mess
        if (CurrentToken.TokenValue == "(")
        {
            // fun
            var functionName = lastToken.TokenValue;
            // push pointer 0 onto the stack
            CodeLines.AddLast(CommandGen.Push("pointer", 0));
            // (
            NextToken();
            var numberOfArguments = ExpressionList();
            // )
            NextToken();
            
            var commandString = CommandGen.Call($"{_labelTuple.className}.{functionName}", numberOfArguments + 1);
            CodeLines.AddLast(commandString);
        }
        // Handle alternate subroutine call (method)
        else
        {            
            // class
            string className = lastToken.TokenValue;
            var isMethod = SymbolExists(className);
            // .
            NextToken();
            // method
            var method = CurrentToken.TokenValue;
            NextToken();
            // (
            NextToken();
            // Expressions 
            if (isMethod)
            {
                var symbol = FetchSymbol(className);
                className = symbol.Type;
                var segment = KindToSegment(symbol.Kind);
                CodeLines.AddLast(CommandGen.Push(segment, symbol.Index));
            }
            var numberOfArguments = ExpressionList();
            // )
            NextToken();
            if (isMethod) numberOfArguments++;
            var commandString = CommandGen.Call($"{className}.{method}", numberOfArguments);
            CodeLines.AddLast(commandString);
        }
        // ;
        NextToken();
        // do expressions do not care about the return values, so pop the stack to temp 0
        CodeLines.AddLast(CommandGen.Pop("temp", 0));
    }

    private void WhileStatement()
    {
        // setup the unique labels
        _labelTuple.Index++;
        var labelOne = $"{_labelTuple.className}.{_labelTuple.subroutineName}-{_labelTuple.Index}";
        _labelTuple.Index++;
        var labelTwo = $"{_labelTuple.className}.{_labelTuple.subroutineName}-{_labelTuple.Index}";
        // while
        NextToken();
        // (
        NextToken();
        CodeLines.AddLast(CommandGen.Label(labelOne));
        Expression();
        CodeLines.AddLast(CommandGen.Arithmetic(Command.Not));
        CodeLines.AddLast(CommandGen.IfGoto(labelTwo));
        // )
        NextToken();
        // {
        NextToken();
        StatementLoop();
        CodeLines.AddLast(CommandGen.Goto(labelOne));
        // }
        NextToken();
        CodeLines.AddLast(CommandGen.Label(labelTwo));
    }

    private void IfStatement()
    {
        // setup the unique labels
        _labelTuple.Index++;
        var labelOne = $"{_labelTuple.className}.{_labelTuple.subroutineName}-{_labelTuple.Index}";
        _labelTuple.Index++;
        var labelTwo = $"{_labelTuple.className}.{_labelTuple.subroutineName}-{_labelTuple.Index}";
        // if
        NextToken();
        // (
        NextToken();
        Expression();
        // )
        NextToken();
        // Push not to invert the statement result
        CodeLines.AddLast(CommandGen.Arithmetic(Command.Not));
        // {
        NextToken();
        CodeLines.AddLast(CommandGen.IfGoto(labelOne));
        StatementLoop();
        CodeLines.AddLast(CommandGen.Goto(labelTwo));
        // }
        NextToken();
        CodeLines.AddLast(CommandGen.Label(labelOne));
        if (CurrentToken.TokenValue == "else")
        {
            // else
            NextToken();
            // {
            NextToken();
            
            StatementLoop();
            // }
            NextToken();
        }
        CodeLines.AddLast(CommandGen.Label(labelTwo));
    }

    private void LetStatement()
    {
        // let
        NextToken();
        // varName
        var varName = CurrentToken.TokenValue;
        NextToken();
        if (CurrentToken.TokenValue == "[")
        {
            throw new NotImplementedException("Have not implemented arrays");
            // [
            NextToken();
            Expression();
            // ]
            NextToken();
        }
        // =
        NextToken();
        Expression();
        // ;
        NextToken();
        var symbol = FetchSymbol(varName);
        var segment = KindToSegment(symbol.Kind);
        CodeLines.AddLast(CommandGen.Pop(segment, symbol.Index));
    }
    
    private void Expression()
    {
        // There will be at least one term. Deal with the case where there is more than one with an op
        Term();
        
        while (true)
        {
            if (_ops.ContainsKey(CurrentToken.TokenValue))
            {
                var op = CurrentToken.TokenValue;
                NextToken();
                Term();
                CodeLines.AddLast(CommandGen.Arithmetic(_ops[op]));
            }
            break;
        }
        
    }

    private void Term()
    {
        // This function is LL(2) so we need to look ahead to know what to do
        var lastToken = CurrentToken;
        NextToken();
        // Handle integer constant
        if (lastToken.TokenType == TokenType.IntConst)
        {
            CodeLines.AddLast(CommandGen.PushConstant(lastToken.TokenValue));
        } 
        // Handle string constant
        else if (lastToken.TokenType == TokenType.StringConst)
        {
            TokenToXmlLine(lastToken, false);
        }
        // Handle nested expression
        else if (lastToken.TokenValue == "(")
        {
            Expression();
            // )
            NextToken();
        }
        // Handle unaryOp
        else if (lastToken.TokenValue is "~" or "-")
        {
            Term();
            if (lastToken.TokenValue == "~")
            {
                CodeLines.AddLast(CommandGen.Arithmetic(Command.Not));
            }
            else
            {
                CodeLines.AddLast(CommandGen.Arithmetic(Command.Negative));
            }
        }
        // Handle keyword constant
        else if (lastToken.TokenValue is "true" or "false" or "null" or "this")
        {
            if (lastToken.TokenValue is "false" or "null")
            {
                CodeLines.AddLast(CommandGen.Push("constant", 0));
            } 
            else if (lastToken.TokenValue is "true")
            {
                CodeLines.AddLast(CommandGen.Push("constant", 1));
                CodeLines.AddLast(CommandGen.Arithmetic(Command.Negative));
            }
            else
            {
                CodeLines.AddLast(CommandGen.Push("pointer", 0));
            }
        }
        // Handle indexed variable name
        else if (CurrentToken.TokenValue == "[")
        {
            TokenToXmlLine(lastToken, false);
            TokenToXmlLine(CurrentToken, TokenType.Symbol, "[");
            Expression();
            TokenToXmlLine(CurrentToken, TokenType.Symbol, "]");
        }
        // Method call. Inlined for clarity as separating creates a mess
        else if (CurrentToken.TokenValue == "(")
        {
            var functionName = lastToken.TokenValue;
            // push the pointer onto the stack
            CodeLines.AddLast(CommandGen.Push("pointer", 0));
            // (
            NextToken();
            var numberOfArguments = ExpressionList();
            // )
            NextToken();
            CodeLines.AddLast(CommandGen.Call($"{_labelTuple.className}.{functionName}", numberOfArguments + 1));
        }
        // Handle method or function call
        else if (CurrentToken.TokenValue == ".")
        {
            // class
            string className = lastToken.TokenValue;
            var isMethod = SymbolExists(className);
            NextToken();
            // function/method Name
            var method = CurrentToken.TokenValue; 
            NextToken();
            // The first argument must be the memory address of the target object
            // This only happens if we are not calling a constructor with 'new'
            if (isMethod)
            {
                var symbol = FetchSymbol(className);
                className = symbol.Type;
                var segment = KindToSegment(symbol.Kind);
                CodeLines.AddLast(CommandGen.Push(segment, symbol.Index));
            }
            // (
            NextToken();
            var numberOfArguments = ExpressionList();
            if (isMethod) numberOfArguments++;
            // )
            NextToken();
            CodeLines.AddLast(CommandGen.Call($"{className}.{method}", numberOfArguments));
        }
        // Handle the varName case
        else
        {
            var symbol = FetchSymbol(lastToken.TokenValue);
            var command = CommandGen.Push(KindToSegment(symbol.Kind), symbol.Index);
            CodeLines.AddLast(command);
        }
    }

    private int ExpressionList()
    {
        var numberOfExpressions = 0;
        if (CurrentToken.TokenValue != ")")
        {
            while (true)
            {
                Expression();
                numberOfExpressions++;
                if (CurrentToken.TokenValue == ",")
                {
                    NextToken();
                    continue;
                }
                break;
            }
        }
        return numberOfExpressions;
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
            SubroutineSymbolTable.AddSymbol(CurrentToken.TokenValue, "variable", variableType);
            NextToken();
            if (CurrentToken.TokenValue != ",")
            {
                break;
            }

            NextToken();
        } while (true);
        NextToken();
    }

    private Symbol FetchSymbol(string symbolName)
    {
        if (SubroutineSymbolTable.ContainsSymbolName(symbolName))
        {
            var symbol = SubroutineSymbolTable.GetSymbol(symbolName);
            return symbol!;
        }
        else if (ClassSymbolTable.ContainsSymbolName(symbolName))
        {
            var symbol = ClassSymbolTable.GetSymbol(symbolName);
            return symbol!;
        }
        else
        {
            throw new Exception($"Could not fetch unknown symbol name: {symbolName}");
        }
    }

    private bool SymbolExists(string symbolName)
    {
        var result = SubroutineSymbolTable.ContainsSymbolName(symbolName);
        if (result == false)
        {
            return ClassSymbolTable.ContainsSymbolName(symbolName);
        }
        else
        {
            return true;
        }
    }

    private static string KindToSegment(string kind)
    {
        return kind switch
        {
            "static" => "static",
            "field" => "this",
            "local" => "local",
            "argument" => "argument",
            "variable" => "local",
            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null)
        };
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