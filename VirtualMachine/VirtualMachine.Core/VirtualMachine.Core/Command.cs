using VirtualMachine.Core.Enums;

namespace VirtualMachine.Core;

public class Command
{
    public required string CommandText;
    public required CommandType CommandType;
    public required int CommandNumber;
    public string? ArgumentOne;
    public int? ArgumentTwo;
    public required List<string> AssemblyCommandList;
}