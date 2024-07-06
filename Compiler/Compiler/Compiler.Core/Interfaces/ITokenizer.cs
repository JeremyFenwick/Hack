using Compiler.Core.Enums;

namespace Compiler.Core.Interfaces;

public interface ITokenizer
{
    public bool HasMoreTokens { get; }
    public Token Token { get; }
    public string CurrentLine { get; }
    public bool Advance();
}