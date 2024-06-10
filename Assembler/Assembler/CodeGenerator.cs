namespace Assembler;

public class CodeGenerator
{
    private SymbolTable _symbolTableWithLabels;
    private Dictionary<int, string> _parsedCode;
    private int _variableCounter = 16;

    public CodeGenerator(SymbolTable symbolTableWithLabels, Dictionary<int, string> parsedCode)
    {
        _symbolTableWithLabels = symbolTableWithLabels;
        _parsedCode = parsedCode;
    }
    
    // Handle A instructions
    public string AInstructionToBinary(string line)
    {
        var value = line.Substring(1);
        // Handle the case where it is a number
        if (char.IsDigit(line[1]))
        {
            if (int.TryParse(value, out var intLine))
            {
                return Convert.ToString(intLine, 2).PadLeft(16, '0');
            }
            else
            {
                throw new Exception("Could not convert A instruction to binary. It appears to be failed string -> int conversion.");
            }
        }
        // Handle the case where it is an existing variable
        if (_symbolTableWithLabels.CheckContainsKey(value))
        {
            var variableValue = _symbolTableWithLabels.GetSymbolValue(value);
            return Convert.ToString(variableValue, 2).PadLeft(16, '0');
        }
        // Handle the case where it is a new variable
        else
        {
            _symbolTableWithLabels.AddSymbol(value, _variableCounter);
            var variableValue = _symbolTableWithLabels.GetSymbolValue(value);
            _variableCounter++;
            return Convert.ToString(variableValue, 2).PadLeft(16, '0');
        }
    }
    
    // Handle L instructions
    public string LInstructionToBinary(string line)
    {
        var value = line.Substring(1);
        if (_symbolTableWithLabels.CheckContainsKey(value))
        {
            var variableValue = _symbolTableWithLabels.GetSymbolValue(value);
            return Convert.ToString(variableValue, 2).PadLeft(16, '0');
        }
        else
        {
            throw new Exception("Failed L instruction conversion. Label not found.");
        }
    }
    
    // Handle C instructions
}