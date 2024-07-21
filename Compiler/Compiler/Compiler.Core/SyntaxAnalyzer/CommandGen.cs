using Compiler.Core.Enums;

namespace Compiler.Core.SyntaxAnalyzer;

public static class CommandGen
{
    public static string Push(string segment, int index)
    {
        return $"push {segment} {index}";
    }

    public static string Pop(string segment, int index)
    {
        return $"pop {segment} {index}";
    }

    public static string PushConstant(string constant)
    {
        return $"push constant {constant.ToString().ToLower()}";
    }

    public static string Arithmetic(Command command)
    {
        return command switch
        {
            Command.Add => "add",
            Command.Multiply => "call Math.multiply 2",
            Command.Divide => "call Math.divide 2",
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

    public static string Label(string label)
    {
        return $"label {label}";
    }

    public static string Goto(string destination)
    {
        return $"goto {destination}";
    }

    public static string IfGoto(string destination)
    {
        return $"if-goto {destination}";
    }

    public static string Call(string name, int variables)
    {
        return $"call {name} {variables}";
    }
    
    public static string Function(string name, int variables)
    {
        return $"function {name} {variables}";
    }

    public static string Return()
    {
        return "return";
    }
}