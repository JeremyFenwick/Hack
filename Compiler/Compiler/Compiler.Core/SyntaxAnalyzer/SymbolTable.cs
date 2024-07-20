using Compiler.Core.Enums;

namespace Compiler.Core.SyntaxAnalyzer;

public class SymbolTable
{
    private int _staticIndex;
    private int _fieldIndex;
    private int _argumentIndex;
    private int _variableIndex;
    private readonly Dictionary<string, Symbol> _symbolTable;

    public SymbolTable()
    {
        _staticIndex = 0;
        _fieldIndex = 0;
        _argumentIndex = 0;
        _variableIndex = 0;
        _symbolTable = new Dictionary<string, Symbol>();
    }

    public void Reset()
    {
        _staticIndex = 0;
        _fieldIndex = 0;
        _argumentIndex = 0;
        _variableIndex = 0;
        _symbolTable.Clear();
    }

    public void AddSymbol(string name, string kind, string type)
    {
        int index;
        switch (kind)
        {
            case "static":
                index = _staticIndex;
                _staticIndex++;
                break;
            case "argument":
                index = _argumentIndex;
                _argumentIndex++;
                break;
            case "field":
                index = _fieldIndex;
                _fieldIndex++;
                break;
            case "variable":
                index = _variableIndex;
                _variableIndex++;
                break;
            default:
                throw new Exception("Unknown symbol kind. Exiting...");
        }
        
        var newSymbol = new Symbol
        {
            Name = name,
            Kind = kind,
            Type = type,
            Index = index
        };
        
        _symbolTable.Add(name, newSymbol);
    }

    public Symbol? GetSymbol(string name)
    {
        return _symbolTable.GetValueOrDefault(name);
    }

    public bool ContainsSymbolName(string name)
    {
        return _symbolTable.ContainsKey(name);
    }
    
    public int NumberOfVariables()
    {
        var counter = 0;
        foreach (var (key, value) in _symbolTable)
        {
            if (value.Kind is "variable" or "local") counter++;
        }
        return counter;
    }
    
    public int NumberOfFields()
    {
        var counter = 0;
        foreach (var (key, value) in _symbolTable)
        {
            if (value.Kind is "field") counter++;
        }
        return counter;
    }
}