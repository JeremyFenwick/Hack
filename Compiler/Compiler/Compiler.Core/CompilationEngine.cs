using Compiler.Core.Interfaces;

namespace Compiler.Core;

public class CompilationEngine
{
    private ITokenizer _tokenizer;
    
    public CompilationEngine(ITokenizer tokenizer)
    {
        _tokenizer = tokenizer;
    } 

}