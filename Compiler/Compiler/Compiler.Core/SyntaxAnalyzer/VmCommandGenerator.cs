using Compiler.Core.Enums;

namespace Compiler.Core.SyntaxAnalyzer;

public static class VmCommandGenerator
{
    public static string GeneratePushCommand(string segment, int index)
    {
        return $"push {segment} {index}";
    }

    public static string GeneratePopCommand(string segment, int index)
    {
        return $"pop {segment} {index}";
    }

    public static string GeneratePushConstantCommand(string constant)
    {
        return $"push constant {constant.ToString().ToLower()}";
    }

    public static string GenerateArithmetic(Command command)
    {
        return command switch
        {
            Command.Add => "add",
            Command.Multiply => "call Math.multiply 2",
            Command.Subtract => "sub",
            Command.Negative => "neg",
            Command.Equal => "eq",
            Command.GreaterThan => "gt",
            Command.LessThan => "lt",
            Command.And => "and",
            Command.Or => "or",
            Command.Not => "not",
            _ => throw new Exception($"Unknown arithmetic symbol: {command}")
        };
    }

    public static string GenerateLabel(string label)
    {
        return $"label {label}";
    }

    public static string GenerateGoto(string destination)
    {
        return $"goto {destination}";
    }

    public static string GenerateIfGoto(string destination)
    {
        return $"if-goto {destination}";
    }

    public static string GenerateCall(string name, int variables)
    {
        return $"call {name} {variables}";
    }
    
    public static string GenerateFunction(string name, int variables)
    {
        return $"function {name} {variables}";
    }

    public static string GenerateReturn()
    {
        return "return";
    }
}