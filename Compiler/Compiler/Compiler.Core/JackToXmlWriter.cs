using Compiler.Core.SyntaxAnalyzer;
using Microsoft.Extensions.Logging;

namespace Compiler.Core;

public class JackToXmlWriter
{
    private string _fileOrDirectory;
    private bool _fileMode = false;
    private ILogger _logger;

    public JackToXmlWriter(string fileOrDirectory, ILogger logger)
    {
        _logger = logger;
        _fileOrDirectory = fileOrDirectory;
        if (fileOrDirectory.EndsWith(".jack"))
        {
            _fileMode = true;
        }
    }
    public void GenerateXml()
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

    private void HandleSingleFile()
    {
        List<string> rawVmCode;
        // Write the code to a .hack file
        var outputFile = $"{Path.GetDirectoryName(_fileOrDirectory)}\\{Path.GetFileNameWithoutExtension(_fileOrDirectory)}.xml";
        var newFs = File.Create(outputFile);
        newFs.Close();
        // Attempt to load the file
        try
        {
            rawVmCode = File.ReadLines(_fileOrDirectory).ToList();
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
        // Now generate the xml file
        var tokenizer = new Tokenizer(rawVmCode, _logger);
        var compilationEngine = new CompilationEngine(tokenizer, _logger);
        compilationEngine.BeginCompilationRoutine();
        
        using var streamWriter = File.AppendText(outputFile);
        foreach (var line in compilationEngine.XmlLines)
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
            List<string> rawVmCode;
            if (Path.GetExtension(file) != ".jack") continue;
            // Attempt to load the file
            try
            {
                rawVmCode = File.ReadLines(file).ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            // Create the output file
            var outputFile = $"{Path.GetDirectoryName(file)}\\{Path.GetFileNameWithoutExtension(file)}.xml";
            var newFs = File.Create(outputFile);
            newFs.Close();
            // Now generate the xml file
            var tokenizer = new Tokenizer(rawVmCode, _logger);
            var compilationEngine = new CompilationEngine(tokenizer, _logger);
            compilationEngine.BeginCompilationRoutine();
        
            using var streamWriter = File.AppendText(outputFile);
            foreach (var line in compilationEngine.XmlLines)
            {
                streamWriter.WriteLine(line);
            }
        }
    }
}