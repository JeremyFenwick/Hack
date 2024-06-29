using System.Text.RegularExpressions;
using VirtualMachine.Core.Enums;

namespace VirtualMachine.Core;


public class Parser
{
    private List<string> _rawVmCode;
    private int _rawCodeIndex = 0;
    private int _commandNumber = 1;
    public Command? CurrentCommand;
    public bool HasMoreLines;
    public Parser(List<String> rawVmCode)
    {
        _rawVmCode = rawVmCode;
        HasMoreLines = _rawVmCode.Count > 0;
    }

    public bool Advance()
    {
        if (!HasMoreLines) return false;
        var codeLine = _rawVmCode[_rawCodeIndex];
        // Turn all spaces into a single space
        codeLine = Regex.Replace(codeLine, @"\s{2,}", " ");
        // Remove comments
        var commentIndex = codeLine.IndexOf("//", StringComparison.Ordinal);
        if (commentIndex >= 0)
        {
            codeLine = codeLine.Substring(0, commentIndex);
        }
        // Trim the beginning and end of the command
        char[] charsToTrim = [' ', '\t'];
        codeLine = codeLine.Trim(charsToTrim);   
        // If line is blank, go to the next line
        if (string.IsNullOrEmpty(codeLine))
        {
            _rawCodeIndex++;
            HasMoreLines = _rawCodeIndex < _rawVmCode.Count;
            // If another line remains, advance to that line
            if (HasMoreLines) Advance();
            return false;
        }
        // Convert the command to a list
        var codeList = codeLine.Split(" ");
        // Determine the command type based on the first value
         var commandType = codeList[0] switch
        {
            "push" => CommandType.Push,
            "pop"  => CommandType.Pop,
            "add"  => CommandType.Arithmetic,
            "sub"  => CommandType.Arithmetic,
            "neg"  => CommandType.Arithmetic,
            "eq"  => CommandType.Arithmetic,
            "gt"  => CommandType.Arithmetic,
            "lt"  => CommandType.Arithmetic,
            "and"  => CommandType.Arithmetic,
            "or"  => CommandType.Arithmetic,
            "not"  => CommandType.Arithmetic,
            "label" => CommandType.Label,
            "goto" => CommandType.Goto,
            "if-goto" => CommandType.If,
            "function" => CommandType.Function,
            "call" => CommandType.Call,
            "return" => CommandType.Return,
            _ => throw new ArgumentOutOfRangeException()
        };
        // Set the parsed command to the current command
        CurrentCommand = new Command()
        {
            CommandText = codeList[0],
            CommandType = commandType,
            CommandNumber = _commandNumber,
            ArgumentOne = codeList.Length > 1 ? codeList[1] : null,
            ArgumentTwo = codeList.Length > 2 ? int.Parse(codeList[2]) : null,
            AssemblyCommandList = new List<string>()
        };
        // Increment the code & command indexes
        _rawCodeIndex++;
        _commandNumber++;
        // Determine if there is another line remaining
        HasMoreLines = _rawCodeIndex < _rawVmCode.Count;
        return true;
    }
}