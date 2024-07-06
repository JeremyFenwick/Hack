using Compiler.Core.Enums;
using Compiler.Core.Interfaces;

namespace Compiler.Core;

public class Tokenizer : ITokenizer
{
    private Queue<string> _jackCode;
    private LinkedList<string> _currentLine = null!;
    private readonly List<char> _symbols =
    [
        '{', '}', '(', ')', '[', ']', '.', ',', ';', '+', '-', '*', '/', '|', '<', '>', '=', '~'
    ];

    private readonly List<string> _keywords =
    [
        "class", "constructor", "function", "method", "field", "static", "var",
        "int", "char", "boolean", "void", "true", "false", "null", "this", "let",
        "do", "if", "else", "while", "return"
    ];

    public bool HasMoreTokens { get; private set; }
    public Token Token { get; private set; } = null!;
    public string CurrentLine => string.Join(" ", _currentLine);

    public Tokenizer(IEnumerable<string> jackCode)
    {
        _jackCode = RemoveComments(jackCode);
        NextCodeLine();
    }

    public bool Advance()
    {
        if (_currentLine.Count == 0)
        {
            NextCodeLine();
        }
        
        if (!HasMoreTokens)
        {
            return false;
        }
        
        var workingCodeSnippet = _currentLine.First!.Value;
        _currentLine.RemoveFirst();
        
        if (string.IsNullOrEmpty(workingCodeSnippet))
        {
            Advance();
        }
        
        if (_symbols.Contains(workingCodeSnippet[0]))
        {
            ParseSymbolToken(workingCodeSnippet);
            return true;
        }
        
        if (_keywords.Contains(workingCodeSnippet))
        {
            ParseKeywordToken(workingCodeSnippet);
            return true;
        }

        if (workingCodeSnippet[0] == '"')
        {
            ParseStringToken(workingCodeSnippet);
            return true;
        }

        if (char.IsDigit(workingCodeSnippet[0]))
        {
            ParseIntegerConstantToken(workingCodeSnippet);
            return true;
        }

        if (char.IsLetter(workingCodeSnippet[0]) || workingCodeSnippet[0] == '_')
        {
            ParseIdentifierToken(workingCodeSnippet);
            return true;
        }
        
        throw new Exception("Unknown case in the advance function");
    }

    private void ParseIdentifierToken(string codeSnippet)
    {
        var result = "";
        for (int i = 0; i < codeSnippet.Length; i++)
        {
            if (char.IsLetterOrDigit(codeSnippet[i]) || codeSnippet[i] == '_')
            {
                result += codeSnippet[i];
                continue;
            }

            if (_symbols.Contains(codeSnippet[i]))
            {
                _currentLine.AddFirst(codeSnippet.Substring(i));  
                break;
            }

            throw new Exception("Illegal character found while parsing identifier");
        }
        Token = new Token()
        {
            TokenType = TokenType.Identifier,
            TokenValue = result
        };
    }

    private void ParseStringToken(string codeSnippet)
    {
        var result = "";
        // Begin at 1 to ignore the first open quotes
        for (int i = 1; i < codeSnippet.Length; i++)
        {
            if (codeSnippet[i] == '"' && i == codeSnippet.Length - 1)
            {
                break;
            }
            // If we are at the end of the string token but there is more code, add the leftover code back
            if (codeSnippet[i] == '"' && i < codeSnippet.Length - 1)
            {
                _currentLine.AddFirst(codeSnippet.Substring(i + 1));  
                break;
            }
            // If there is a newline skip both characters. Note the out-of-bounds check in the middle
            if (codeSnippet[i] == '\n' && i <= codeSnippet.Length - 1)
            {
                continue;
            }
            if (i == codeSnippet.Length - 1 && codeSnippet[i] != '"')
            {
                throw new Exception("String parsing failure! String did not end with close quotes");
            }
            result += codeSnippet[i];
        }
        Token = new Token()
        {
            TokenType = TokenType.StringConst,
            TokenValue = result
        };
    }
    
    private void ParseSymbolToken(string codeSnippet)
    {
        Token = new Token()
        {
            TokenType = TokenType.Symbol,
            TokenValue = codeSnippet[0].ToString()
        };
        
        if (codeSnippet.Length > 1)
        {
            _currentLine.AddFirst(codeSnippet.Substring(1));  
        }
    }
    
    private void ParseIntegerConstantToken(string codeSnippet)
    {
        var result = "";
        for (int i = 0; i < codeSnippet.Length; i++)
        {
            if (char.IsDigit(codeSnippet[i]))
            {
                result += codeSnippet[i];
                continue;
            }
            if (_symbols.Contains(codeSnippet[i]))
            {
                _currentLine.AddFirst(codeSnippet.Substring(i));   
                break;
            }
            throw new Exception("Illegal character found while parsing integer constant");
        }
        
        Token = new Token()
        {
            TokenType = TokenType.IntConst,
            TokenValue = result
        };
        
    }

    private void ParseKeywordToken(string codeSnippet)
    {
        Token = new Token()
        {
            TokenType = TokenType.Keyword,
            TokenValue = codeSnippet
        };
    }
    
    private static Queue<string> RemoveComments(IEnumerable<string> codeList)
    {
        var finalQueue = new Queue<string>();
        var ignoreLines = false;
        
        foreach (var codeLine in codeList)
        {
            var parsedLine = codeLine;
            var commentBlockIndex = parsedLine.IndexOf("/**", StringComparison.Ordinal);
            var endCommentBlockIndex = parsedLine.IndexOf("*/", StringComparison.Ordinal);
            if (ignoreLines && endCommentBlockIndex >= 0)
            {
                // +2 to pass over '*/'
                parsedLine = parsedLine.Substring(endCommentBlockIndex + 2);
                ignoreLines = false;
            }
            else if (ignoreLines)
            {
                continue;
            }
            else if (commentBlockIndex >= 0 && endCommentBlockIndex >= 0)
            {
                // +2 to pass over '*/'
                parsedLine = string.Concat(parsedLine.AsSpan(0, commentBlockIndex), parsedLine.AsSpan(endCommentBlockIndex + 2));
            }
            else if (commentBlockIndex >= 0)
            {
                parsedLine = parsedLine.Substring(0, commentBlockIndex);
                ignoreLines = true;
            }

            parsedLine = RemoveNormalComments(parsedLine);
            parsedLine = TrimSpacesAndTabs(parsedLine);
            parsedLine = AllSpacesToSingle(parsedLine);

            if (!string.IsNullOrEmpty(parsedLine))
            {
                finalQueue.Enqueue(parsedLine);
            }
        }
        return finalQueue;
    }

    private static string TrimSpacesAndTabs(string codeLine)
    {
        char[] charsToTrim = [' ', '\t'];
        return codeLine.Trim(charsToTrim);
    }

    private static string RemoveNormalComments(string codeLine)
    {
        var commentIndex = codeLine.IndexOf("//", StringComparison.Ordinal);
        if (commentIndex >= 0)
        {
            codeLine = codeLine.Substring(0, commentIndex);
        }
        return codeLine;
    }

    private static string AllSpacesToSingle(string codeLine)
    {
        // We need to ignore duplicate spaces inside quotes
        var insideQuotes = false;
        var previousCharacterWasSpace = false;
        var result = "";
        foreach (var character in codeLine)
        {
            if (!insideQuotes && previousCharacterWasSpace && character == ' ')
            {
                continue;
            }
            if (character == '"')
            {
                insideQuotes = !insideQuotes;
            }
            if (character == ' ')
            {
                previousCharacterWasSpace = true;
            }
            else
            {
                previousCharacterWasSpace = false;
            }
            result += character;
        }
        return result;
        // Older solution that doesn't deal with the special case of string constants
        // return Regex.Replace(codeLine, @"\s{2,}", " ");
    }

    private bool NextCodeLine()
    {
        if (_jackCode.Count == 0)
        {
            HasMoreTokens = false;
            return false;
        }
        _currentLine = SplitLine(_jackCode.Dequeue());
        HasMoreTokens = true;
        return true;
    }

    private LinkedList<string> SplitLine(string codeLine)
    {
        var insideQuotes = false;
        var workingString = "";
        var result = new LinkedList<string>();
        // We split out the first word before a space, unless quotes are involved
        for (int i = 0; i < codeLine.Length; i++)
        {
            if (codeLine[i] == '"')
            {
                insideQuotes = !insideQuotes;
            }
            if (codeLine[i] == ' ' && !insideQuotes)
            {
                result.AddLast(workingString);
                workingString = "";
                continue;
            }
            workingString += codeLine[i];
            // If we are at the end, add what is left
            if (i == codeLine.Length - 1)
            {
                result.AddLast(workingString);
            }
        }
        return result;
    }
}