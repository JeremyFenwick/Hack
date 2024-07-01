using VirtualMachine.Core.Enums;

namespace VirtualMachine.Core;

public class CommandCodeGenerator
{
    private int _uniqueInteger = 1;
    private string _fileName;

    public CommandCodeGenerator(string fileName)
    {
        _fileName = fileName;
    }
    public Command CommandCodeGen(Command command)
    {
        switch (command.CommandType)
        {
            case CommandType.Push:
                return PushHandler(command);;
            case CommandType.Pop:
                return PopHandler(command);
            case CommandType.Arithmetic:
                return ArithmeticHandler(command);
            case CommandType.Label:
                return LabelHandler(command);
            case CommandType.Goto:
                return GotoHandler(command);
            case CommandType.If:
                return GotoIfHandler(command);
            case CommandType.Call:
                return CallHandler(command);
            case CommandType.Function:
                return FunctionHandler(command);
            case CommandType.Return:
                return ReturnHandler(command);
            default:
                throw new NotImplementedException();
        }
    }

    private Command PushHandler(Command command)
    {
        // Add the command in as a comment
        command.AssemblyCommandList.Add($"// {command.CommandText} {command.ArgumentOne} {command.ArgumentTwo}");
        if (command.ArgumentOne == "constant")
        {
            command.AssemblyCommandList.Add($"@{command.ArgumentTwo}");
            command.AssemblyCommandList.Add("D=A");

        }
        else if (command.ArgumentOne == "pointer")
        {
            string name = command.ArgumentTwo switch
            {
                0 => "THIS",
                1 => "THAT",
                _ => throw new Exception($"Integer for pointer: {command.ArgumentTwo} is not 0 or 1.")
            };
            command.AssemblyCommandList.Add($"@{name}");
            command.AssemblyCommandList.Add("D=M");
        }
        else if (command.ArgumentOne == "temp")
        {
            command.AssemblyCommandList.Add($"@{command.ArgumentTwo + 5}");
            command.AssemblyCommandList.Add("D=M");
        }
        else if (command.ArgumentOne is "local" or "argument" or "this" or "that")
        {
            string name = command.ArgumentOne switch
            {
                "local" => "LCL",
                "argument" => "ARG",
                "this" => "THIS",
                "that" => "THAT",
            };
            command.AssemblyCommandList.Add($"@{command.ArgumentTwo}");
            command.AssemblyCommandList.Add("D=A");
            command.AssemblyCommandList.Add($"@{name}");
            command.AssemblyCommandList.Add("A=M+D");
            command.AssemblyCommandList.Add("D=M");
        }
        else if (command.ArgumentOne == "static")
        {
            command.AssemblyCommandList.Add($"@static.{_fileName}.{command.ArgumentTwo}");
            command.AssemblyCommandList.Add("D=M");
        }
        else
        {
            throw new Exception($"Unknown value for Command argument one: {command.ArgumentOne}");
        }
        // Increment SP and Load D into M 
        command.AssemblyCommandList.Add("@SP");
        command.AssemblyCommandList.Add("M=M+1");
        command.AssemblyCommandList.Add("A=M-1");
        command.AssemblyCommandList.Add("M=D");

        return command;
    }
    
    private Command PopHandler(Command command)
    {
        // Add the command in as a comment
        command.AssemblyCommandList.Add($"// {command.CommandText} {command.ArgumentOne} {command.ArgumentTwo}");
        if (command.ArgumentOne is "local" or "argument" or "this" or "that")
        {
            string name = command.ArgumentOne switch
            {
                "local" => "LCL",
                "argument" => "ARG",
                "this" => "THIS",
                "that" => "THAT",
            };
            // Load the target address into R13
            command.AssemblyCommandList.Add($"@{name}");
            command.AssemblyCommandList.Add("D=M");
            command.AssemblyCommandList.Add($"@{command.ArgumentTwo}");
            command.AssemblyCommandList.Add("D=D+A");
            command.AssemblyCommandList.Add("@R13");
            command.AssemblyCommandList.Add("M=D");
            // Pop the stack
            command.AssemblyCommandList.Add("@SP");
            command.AssemblyCommandList.Add("M=M-1");
            command.AssemblyCommandList.Add("A=M");
            command.AssemblyCommandList.Add("D=M");
            // Set the R13 address to the popped value
            command.AssemblyCommandList.Add("@R13");
            command.AssemblyCommandList.Add("A=M");
            command.AssemblyCommandList.Add("M=D");
        }
        else if (command.ArgumentOne == "temp")
        {
            // Pop the stack
            command.AssemblyCommandList.Add("@SP");
            command.AssemblyCommandList.Add("M=M-1");
            command.AssemblyCommandList.Add("A=M");
            command.AssemblyCommandList.Add("D=M");
            // Load D into the temp address
            command.AssemblyCommandList.Add($"@{command.ArgumentTwo + 5}");
            command.AssemblyCommandList.Add("M=D");
        }
        else if (command.ArgumentOne == "static")
        {
            // Pop the stack
            command.AssemblyCommandList.Add("@SP");
            command.AssemblyCommandList.Add("M=M-1");
            command.AssemblyCommandList.Add("A=M");
            command.AssemblyCommandList.Add("D=M");
            // Load D into the static variable address
            command.AssemblyCommandList.Add($"@static.{_fileName}.{command.ArgumentTwo}");
            command.AssemblyCommandList.Add("M=D");
        }
        else if (command.ArgumentOne == "pointer")
        {
            string name = command.ArgumentTwo switch
            {
                0 => "THIS",
                1 => "THAT",
                _ => throw new Exception($"Integer for pointer: {command.ArgumentTwo} is not 0 or 1.")
            };
            // Pop the stack
            command.AssemblyCommandList.Add("@SP");
            command.AssemblyCommandList.Add("M=M-1");
            command.AssemblyCommandList.Add("A=M");
            command.AssemblyCommandList.Add("D=M");
            // Load D into the pointer address
            command.AssemblyCommandList.Add($"@{name}");
            command.AssemblyCommandList.Add("M=D");
        }
        else
        {
            throw new Exception($"Unknown value for Command argument one: {command.ArgumentOne}");
        }

        return command;
    }

    private Command ArithmeticHandler(Command command)
    {
        // Add the command in as a comment
        command.AssemblyCommandList.Add($"// {command.CommandText}");
        if (command.CommandText == "add")
        {
            command.AssemblyCommandList.Add("@SP");
            command.AssemblyCommandList.Add("M=M-1");
            command.AssemblyCommandList.Add("A=M");
            command.AssemblyCommandList.Add("D=M");
            command.AssemblyCommandList.Add("A=A-1");
            command.AssemblyCommandList.Add("M=D+M");
        }
        else if (command.CommandText == "sub")
        {
            command.AssemblyCommandList.Add("@SP");
            command.AssemblyCommandList.Add("M=M-1");
            command.AssemblyCommandList.Add("A=M");
            command.AssemblyCommandList.Add("D=M");
            command.AssemblyCommandList.Add("A=A-1");
            command.AssemblyCommandList.Add("M=M-D");
        } 
        else if (command.CommandText == "neg")
        {
            command.AssemblyCommandList.Add("@SP");
            command.AssemblyCommandList.Add("A=M-1");
            command.AssemblyCommandList.Add("M=-M");
        }
        else if (command.CommandText == "eq")
        {
            // Pop the stack
            command.AssemblyCommandList.Add("@SP");
            command.AssemblyCommandList.Add("M=M-1");
            command.AssemblyCommandList.Add("A=M");
            command.AssemblyCommandList.Add("D=M");
            // Set D and set the pointer SP -1 M = 0 
            command.AssemblyCommandList.Add("A=A-1");
            command.AssemblyCommandList.Add("D=D-M");
            command.AssemblyCommandList.Add("M=0");
            // If D != 0, jump. Else continue
            command.AssemblyCommandList.Add($"@UNIQUEJUMP-{_uniqueInteger}");
            command.AssemblyCommandList.Add($"D;JNE");
            command.AssemblyCommandList.Add($"@SP");
            command.AssemblyCommandList.Add($"A=M-1");
            command.AssemblyCommandList.Add($"M=-1");
            command.AssemblyCommandList.Add($"(UNIQUEJUMP-{_uniqueInteger})");
            _uniqueInteger++;
        }
        else if (command.CommandText == "gt")
        {
            // Pop the stack
            command.AssemblyCommandList.Add("@SP");
            command.AssemblyCommandList.Add("M=M-1");
            command.AssemblyCommandList.Add("A=M");
            command.AssemblyCommandList.Add("D=M");
            // Set D and set the pointer SP -1 M = 0 
            command.AssemblyCommandList.Add("A=A-1");
            command.AssemblyCommandList.Add("D=M-D");
            command.AssemblyCommandList.Add("M=0");
            // If D <= 0, jump. Else continue
            command.AssemblyCommandList.Add($"@UNIQUEJUMP-{_uniqueInteger}");
            command.AssemblyCommandList.Add($"D;JLE");
            command.AssemblyCommandList.Add($"@SP");
            command.AssemblyCommandList.Add($"A=M-1");
            command.AssemblyCommandList.Add($"M=-1");
            command.AssemblyCommandList.Add($"(UNIQUEJUMP-{_uniqueInteger})");
            _uniqueInteger++;
        }
        else if (command.CommandText == "lt")
        {
            // Pop the stack
            command.AssemblyCommandList.Add("@SP");
            command.AssemblyCommandList.Add("M=M-1");
            command.AssemblyCommandList.Add("A=M");
            command.AssemblyCommandList.Add("D=M");
            // Set D and set the pointer SP -1 M = 0 
            command.AssemblyCommandList.Add("A=A-1");
            command.AssemblyCommandList.Add("D=M-D");
            command.AssemblyCommandList.Add("M=0");
            // If D >= 0, jump. Else continue
            command.AssemblyCommandList.Add($"@UNIQUEJUMP-{_uniqueInteger}");
            command.AssemblyCommandList.Add($"D;JGE");
            command.AssemblyCommandList.Add($"@SP");
            command.AssemblyCommandList.Add($"A=M-1");
            command.AssemblyCommandList.Add($"M=-1");
            command.AssemblyCommandList.Add($"(UNIQUEJUMP-{_uniqueInteger})");
            _uniqueInteger++;
        }
        else if (command.CommandText == "and")
        {
            // Pop the stack
            command.AssemblyCommandList.Add("@SP");
            command.AssemblyCommandList.Add("M=M-1");
            command.AssemblyCommandList.Add("A=M");
            command.AssemblyCommandList.Add("D=M");
            // Set AND of D&M to the A-1 pointer memory
            command.AssemblyCommandList.Add("A=A-1");
            command.AssemblyCommandList.Add("D=D&M");
            command.AssemblyCommandList.Add("M=D");
        }
        else if (command.CommandText == "or")
        {
            // Pop the stack
            command.AssemblyCommandList.Add("@SP");
            command.AssemblyCommandList.Add("M=M-1");
            command.AssemblyCommandList.Add("A=M");
            command.AssemblyCommandList.Add("D=M");
            // Set OR of D|M to the A-1 pointer memory
            command.AssemblyCommandList.Add("A=A-1");
            command.AssemblyCommandList.Add("D=D|M");
            command.AssemblyCommandList.Add("M=D");
        }
        else if (command.CommandText == "not")
        {
            // Set SP-1 to Not
            command.AssemblyCommandList.Add("@SP");
            command.AssemblyCommandList.Add("A=M-1");
            command.AssemblyCommandList.Add("M=!M");
        }
        else
        {
            throw new Exception($"Unknown value for Command argument one: {command.ArgumentOne}");
        }
        
        return command;
    }
    
    private Command LabelHandler(Command command)
    {
        command.AssemblyCommandList.Add($"// {command.CommandText} {command.ArgumentOne}");
        command.AssemblyCommandList.Add($"({command.ArgumentOne})");
        return command;
    }

    private Command GotoHandler(Command command)
    {
        command.AssemblyCommandList.Add($"@{command.ArgumentOne}");
        command.AssemblyCommandList.Add($"0;JMP");
        return command;
    }

    private Command GotoIfHandler(Command command)
    {
        command.AssemblyCommandList.Add($"// {command.CommandText} {command.ArgumentOne}");
        // Pop the stack
        command.AssemblyCommandList.Add("@SP");
        command.AssemblyCommandList.Add("M=M-1");
        command.AssemblyCommandList.Add("A=M");
        command.AssemblyCommandList.Add("D=M");
        // If d is true, jump to the label
        command.AssemblyCommandList.Add($"@{command.ArgumentOne}");
        command.AssemblyCommandList.Add($"D;JNE");

        return command;
    }
    private Command CallHandler(Command command)
    {
        var zeroArgFunction = (command.ArgumentTwo is null or 0);
        switch (zeroArgFunction)
        {
            case false:
                command.AssemblyCommandList.Add($"// {command.CommandText} {command.ArgumentOne} {command.ArgumentTwo}");
                break;
            case true:
                command.AssemblyCommandList.Add($"// {command.CommandText} {command.ArgumentOne}");
                break;
        }

        // PUSH RETURNADDRESS
        command.AssemblyCommandList.Add($"// PUSH RETURNADDRESS");
        // Set D to returnAddress
        command.AssemblyCommandList.Add($"@UNIQUERETURN-{_uniqueInteger}");
        command.AssemblyCommandList.Add("D=A");
        // Push D onto the stack
        command.AssemblyCommandList.Add("@SP");
        command.AssemblyCommandList.Add("M=M+1");
        command.AssemblyCommandList.Add("A=M-1");
        command.AssemblyCommandList.Add("M=D");
        
        // PUSH LCL
        command.AssemblyCommandList.Add($"// PUSH LCL");
        // Set D to LCL
        command.AssemblyCommandList.Add("@LCL");
        command.AssemblyCommandList.Add("D=M");
        // Push D onto the stack
        command.AssemblyCommandList.Add("@SP");
        command.AssemblyCommandList.Add("M=M+1");
        command.AssemblyCommandList.Add("A=M-1");
        command.AssemblyCommandList.Add("M=D");
        
        // PUSH ARG
        command.AssemblyCommandList.Add($"// PUSH ARG");
        // Set D to ARG
        command.AssemblyCommandList.Add("@ARG");
        command.AssemblyCommandList.Add("D=M");
        // Push ARG onto the stack
        command.AssemblyCommandList.Add("@SP");
        command.AssemblyCommandList.Add("M=M+1");
        command.AssemblyCommandList.Add("A=M-1");
        command.AssemblyCommandList.Add("M=D");
        
        // PUSH THIS
        command.AssemblyCommandList.Add($"// PUSH THIS");
        // Set D to THIS
        command.AssemblyCommandList.Add("@THIS");
        command.AssemblyCommandList.Add("D=M");
        // Push THIS onto the stack
        command.AssemblyCommandList.Add("@SP");
        command.AssemblyCommandList.Add("M=M+1");
        command.AssemblyCommandList.Add("A=M-1");
        command.AssemblyCommandList.Add("M=D");
        
        // PUSH THAT
        command.AssemblyCommandList.Add($"// PUSH THAT");
        // Set D to THAT
        command.AssemblyCommandList.Add("@THAT");
        command.AssemblyCommandList.Add("D=M");
        // Push THAT onto the stack
        command.AssemblyCommandList.Add("@SP");
        command.AssemblyCommandList.Add("M=M+1");
        command.AssemblyCommandList.Add("A=M-1");
        command.AssemblyCommandList.Add("M=D");
        
        // Reposition ARG
        command.AssemblyCommandList.Add($"// Reposition ARG");
        command.AssemblyCommandList.Add("@SP");
        command.AssemblyCommandList.Add("D=M");
        command.AssemblyCommandList.Add("@5");
        command.AssemblyCommandList.Add("D=D-A");
        command.AssemblyCommandList.Add(!zeroArgFunction ? $"@{command.ArgumentTwo}" : $"@0");
        command.AssemblyCommandList.Add("D=D-A");
        command.AssemblyCommandList.Add("@ARG");
        command.AssemblyCommandList.Add("M=D");
        
        // Reposition LCL
        command.AssemblyCommandList.Add($"// Reposition LCL");
        command.AssemblyCommandList.Add("@SP");
        command.AssemblyCommandList.Add("D=M");
        command.AssemblyCommandList.Add("@LCL");
        command.AssemblyCommandList.Add("M=D");

        // Goto function name
        command.AssemblyCommandList.Add($"// Goto function");
        command.AssemblyCommandList.Add($"@{command.ArgumentOne}");
        command.AssemblyCommandList.Add("0;JMP");

        // Set label for return
        command.AssemblyCommandList.Add($"// Set return label");
        command.AssemblyCommandList.Add($"(UNIQUERETURN-{_uniqueInteger})");

        _uniqueInteger++;
        return command;
    }

    private Command FunctionHandler(Command command)
    {
        command.AssemblyCommandList.Add($"// {command.CommandText} {command.ArgumentOne} {command.ArgumentTwo}");
        // Declare function entry label
        command.AssemblyCommandList.Add($"({command.ArgumentOne})");
        // Push 0's onto the stack according to nvars
        for (var i = 0; i < command.ArgumentTwo; i++)
        {
            // Push 0 onto the stack
            command.AssemblyCommandList.Add("@0");
            command.AssemblyCommandList.Add("D=A");
            command.AssemblyCommandList.Add("@SP");
            command.AssemblyCommandList.Add("M=M+1");
            command.AssemblyCommandList.Add("A=M-1");
            command.AssemblyCommandList.Add("M=D");
        }
        return command;
    }
    
    private Command ReturnHandler(Command command)
    {
        command.AssemblyCommandList.Add($"// {command.CommandText}");

        // Set R13 to endFrame (LCL)
        command.AssemblyCommandList.Add($"// Set R13 to endFrame (LCL)");
        command.AssemblyCommandList.Add("@LCL");
        command.AssemblyCommandList.Add("D=M");
        command.AssemblyCommandList.Add("@R13");
        command.AssemblyCommandList.Add("M=D");
        
        // Set R14 to returnAddress endFrame - 5
        command.AssemblyCommandList.Add($"// Set R14 to returnAddress endFrame - 5");
        command.AssemblyCommandList.Add("@R13");
        command.AssemblyCommandList.Add("D=M");
        command.AssemblyCommandList.Add("@5");
        command.AssemblyCommandList.Add("D=D-A");
        command.AssemblyCommandList.Add("@R14");
        command.AssemblyCommandList.Add("M=D");
        
        // Store the return address value in R15
        command.AssemblyCommandList.Add("@R14");
        command.AssemblyCommandList.Add("A=M");
        command.AssemblyCommandList.Add("D=M");
        command.AssemblyCommandList.Add("@R15");
        command.AssemblyCommandList.Add("M=D");

        // Pop the stack and place the value in the ARG Pointer
        command.AssemblyCommandList.Add($"// Pop the stack and place the value in arg 0");
        command.AssemblyCommandList.Add("@SP");
        command.AssemblyCommandList.Add("M=M-1");
        command.AssemblyCommandList.Add("A=M");
        command.AssemblyCommandList.Add("D=M");
        command.AssemblyCommandList.Add("@ARG");
        command.AssemblyCommandList.Add("A=M");
        command.AssemblyCommandList.Add("M=D");
        
        // Set the stack pointer to ARG + 1 (continues from above)
        command.AssemblyCommandList.Add($"// Set the stack pointer to ARG + 1 (continues from above)");
        command.AssemblyCommandList.Add("@ARG");
        command.AssemblyCommandList.Add("A=M");
        command.AssemblyCommandList.Add("D=A+1");
        command.AssemblyCommandList.Add("@SP");
        command.AssemblyCommandList.Add("M=D");
        
        // Restore THAT from endFrame - 1
        command.AssemblyCommandList.Add($"// Restore THAT from endFrame - 1");
        command.AssemblyCommandList.Add("@R13");
        command.AssemblyCommandList.Add("M=M-1");
        command.AssemblyCommandList.Add("A=M");
        command.AssemblyCommandList.Add("D=M");
        command.AssemblyCommandList.Add("@THAT");
        command.AssemblyCommandList.Add("M=D");
        
        // Restore THIS from endFrame - 2
        command.AssemblyCommandList.Add($"// Restore THIS from endFrame - 2");
        command.AssemblyCommandList.Add("@R13");
        command.AssemblyCommandList.Add("M=M-1");
        command.AssemblyCommandList.Add("A=M");
        command.AssemblyCommandList.Add("D=M");
        command.AssemblyCommandList.Add("@THIS");
        command.AssemblyCommandList.Add("M=D");
        
        // Restore ARG from endFrame - 3
        command.AssemblyCommandList.Add($"// Restore ARG from endFrame - 3");
        command.AssemblyCommandList.Add("@R13");
        command.AssemblyCommandList.Add("M=M-1");
        command.AssemblyCommandList.Add("A=M");
        command.AssemblyCommandList.Add("D=M");
        command.AssemblyCommandList.Add("@ARG");
        command.AssemblyCommandList.Add("M=D");

        // Restore LCL from endFrame - 4
        command.AssemblyCommandList.Add($"// Restore LCL from endFrame - 4");
        command.AssemblyCommandList.Add("@R13");
        command.AssemblyCommandList.Add("M=M-1");
        command.AssemblyCommandList.Add("A=M");
        command.AssemblyCommandList.Add("D=M");
        command.AssemblyCommandList.Add("@LCL");
        command.AssemblyCommandList.Add("M=D");

        // GOTO returnAddress from @R15
        command.AssemblyCommandList.Add($"// GOTO returnAddress (stored in @R15)");
        command.AssemblyCommandList.Add("@R15");
        command.AssemblyCommandList.Add("A=M");
        command.AssemblyCommandList.Add("0;JMP");
        
        return command;
    }
}