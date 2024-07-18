using Compiler.Core.Interfaces;
using Compiler.Core.SyntaxAnalyzer;
using Microsoft.Extensions.Logging;

namespace Compiler.Core;

public class JackToFileWriter
{
    private string _fileOrDirectory;
    private bool _fileMode;
    private ILogger _logger;
    private bool _xmlMode;
    private string _outputFileExtension;
    
    public JackToFileWriter( string fileOrDirectory, ILogger logger, bool xmlMode = false)
    {
        _fileOrDirectory = fileOrDirectory;
        _xmlMode = xmlMode;
        _logger = logger;
        if (fileOrDirectory.EndsWith(".jack"))
        {
            _fileMode = true;
        }
        _outputFileExtension = _xmlMode ? ".xml" : ".vm";
    }
    
    public void GenerateCode()
    {
        if (_fileMode)
        {
            HandleSingleFile();
        }
        else
        { 
            HandleDirectory();
        }
    }

    private ICompilationEngine GenerateCompilationEngine(List<string> rawCode)
    {
        var tokenizer = new Tokenizer(rawCode, _logger);
        if (_xmlMode)
        {
            return new XmlCompilationEngine(tokenizer, _logger);
        }
        else
        {
            return new VmCompilationEngine(tokenizer, _logger);
        }
    }

    private void HandleSingleFile()
    {
        List<string> rawCode;
        
        // Write the code to a .hack file
        var outputFile = $"{Path.GetDirectoryName(_fileOrDirectory)}\\{Path.GetFileNameWithoutExtension(_fileOrDirectory)}{_outputFileExtension}";

        var newFs = File.Create(outputFile);
        newFs.Close();
        // Attempt to load the file
        try
        {
            rawCode = File.ReadLines(_fileOrDirectory).ToList();
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
        // Now generate the xml file
        var compilationEngine = GenerateCompilationEngine(rawCode);
        compilationEngine.BeginCompilationRoutine();
        
        using var streamWriter = File.AppendText(outputFile);
        foreach (var line in compilationEngine.CodeLines)
        {
            streamWriter.WriteLine(line);
        }
    }

    private void HandleDirectory()
    {
        var fileList = Directory.GetFiles(_fileOrDirectory);
        
        foreach (var file in fileList)
        {
            _logger.LogInformation($"Parsing file: {file}");
            List<string> rawCode;
            if (Path.GetExtension(file) != ".jack") continue;
            // Attempt to load the file
            try
            {
                rawCode = File.ReadLines(file).ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            // Create the output file

            var outputFile = $"{Path.GetDirectoryName(file)}\\{Path.GetFileNameWithoutExtension(file)}{_outputFileExtension}"; 
            var newFs = File.Create(outputFile);
            newFs.Close();
            // Now generate the xml file
            var compilationEngine = GenerateCompilationEngine(rawCode);
            compilationEngine.BeginCompilationRoutine();
        
            using var streamWriter = File.AppendText(outputFile);
            foreach (var line in compilationEngine.CodeLines)
            {
                streamWriter.WriteLine(line);
            }
        }
    }
}