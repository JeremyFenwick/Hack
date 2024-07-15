using Compiler.Core.SyntaxAnalyzer;

namespace Compiler.Core.Interfaces;

public interface ICompilationEngine
{
    public Token CurrentToken { get;}
    public LinkedList<string> CodeLines { get; }
    public void BeginCompilationRoutine();
}