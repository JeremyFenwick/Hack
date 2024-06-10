namespace Assembler;

public class CodeGenerator
{
    private SymbolTable _symbolTableWithLabels;
    private Dictionary<int, string> _parsedCode;
    private int _variableCounter = 16;
    private List<string> _binaryCode = new();

    public CodeGenerator(SymbolTable symbolTableWithLabels, Dictionary<int, string> parsedCode)
    {
        _symbolTableWithLabels = symbolTableWithLabels;
        _parsedCode = parsedCode;
        CodeGenRoutine();
    }

    public List<string> GetBinaryCode()
    {
        return _binaryCode;
    }

    private void CodeGenRoutine()
    {
        foreach (var (key, value) in _parsedCode)
        {
            string codeLine;
            // Check for an L instruction
            if (value[0] == '@' && char.IsLetter(value[1]) && char.IsUpper(value[1]))
            {
                var lBinaryString = LInstructionToBinary(value);
                _binaryCode.Add(lBinaryString);
                continue;
            }
            // Check for an A instruction
            if (value[0] == '@')
            {
                var aBinaryString = AInstructionToBinary(value);
                _binaryCode.Add(aBinaryString);
                continue;
            }
            // Else, it must be a C instruction
            var cBinaryString = CInstructionToBinary(value);
            _binaryCode.Add(cBinaryString);
        }
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
    public string CInstructionToBinary(string line)
    {
        string binaryBuild = "111";
        var destIndex = line.IndexOf('=');
        // Handle the comp value
        var startCompIndex = destIndex == -1 ? 0 : destIndex + 1;
        var compText = "";
        foreach (var character in line[startCompIndex..])
        {
            if (character == ';') break;
            compText += character;
        }
        var compValue = CInstructionLookup.CompToBinary(compText);
        binaryBuild += compValue;
        // Handle the dest value
        if (destIndex == -1)
        {
            var destValue = CInstructionLookup.DestToBinary("null");
            binaryBuild += destValue;
        }
        else
        {
            var destSearchValue = line.Substring(0, destIndex);
            var destValue = CInstructionLookup.DestToBinary(destSearchValue);
            binaryBuild += destValue;
        }
        // Handle the jump value
        var jumpIndex = line.IndexOf(';');
        if (jumpIndex == -1)
        {
            var jumpValue = CInstructionLookup.JumpToBinary("null");
            binaryBuild += jumpValue;
        }
        else
        {
            var jumpSearchValue = line.Substring(jumpIndex + 1);
            var jumpValue = CInstructionLookup.JumpToBinary(jumpSearchValue);
            binaryBuild += jumpValue;
        }
        return binaryBuild;
    }
}