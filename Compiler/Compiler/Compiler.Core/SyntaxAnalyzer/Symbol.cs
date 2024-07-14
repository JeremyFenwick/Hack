using Compiler.Core.Enums;

namespace Compiler.Core.SyntaxAnalyzer;

public class Symbol
{
    public required string Name { get; init; }
    public required Kind Kind { get; init; }
    public required string Type { get; init; }
    public required int Index { get; init; }
}