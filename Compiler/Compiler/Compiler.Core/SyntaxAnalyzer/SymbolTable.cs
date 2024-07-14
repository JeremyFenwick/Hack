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

    public void AddSymbol(string name, Kind kind, string type)
    {
        int index;
        switch (kind)
        {
            case Kind.Static:
                index = _staticIndex;
                _staticIndex++;
                break;
            case Kind.Argument:
                index = _argumentIndex;
                _argumentIndex++;
                break;
            case Kind.Field:
                index = _fieldIndex;
                _fieldIndex++;
                break;
            case Kind.Variable:
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
}