namespace Assembler;

public class Parser
{
    private List<string> _rawAssembly;
    private Dictionary<int, string> _parsedAssembly = new();
    private SymbolTable _symbolTable;
    private int _codeLineIncrement = 0;

    public Parser(List<String> rawAssembly, SymbolTable symbolTable)
    {
        _rawAssembly = rawAssembly;
        _symbolTable = symbolTable;
        InitalPass();
    }
    
    private void InitalPass()
    {
        foreach (var line in _rawAssembly)
        {
            // Remove whitespace from line
            var lineWithoutWhitespace = line.Replace(" ", string.Empty);
            // Remove comments
            var commentIndex = lineWithoutWhitespace.IndexOf("//", StringComparison.Ordinal);
            var unCommentedLine = lineWithoutWhitespace;
            if (commentIndex >= 0)
            {
                unCommentedLine = lineWithoutWhitespace.Substring(0, commentIndex);
            }
            // If the line is empty, continue
            if (string.IsNullOrEmpty(unCommentedLine)) continue;
            // Deal with the case where the line is a label symbol
            if (unCommentedLine[0] == '(')
            {
                var value = (_codeLineIncrement);
                var key = unCommentedLine.Substring(1, unCommentedLine.Length - 2);
                _symbolTable.AddSymbol(key, value);
                continue;
            }
            // Add the line with its index. Increment the index
            _parsedAssembly.Add(_codeLineIncrement, unCommentedLine);
            _codeLineIncrement++;
        }
    }

    public Dictionary<int, String> GetParsedAssembly()
    {
        return _parsedAssembly;
    }

    public SymbolTable GetSymbolTableWithLabels()
    {
        return _symbolTable;
    }
}