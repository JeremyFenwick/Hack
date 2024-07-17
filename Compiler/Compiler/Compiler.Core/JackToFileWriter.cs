using Compiler.Core.SyntaxAnalyzer;
using Microsoft.Extensions.Logging;

namespace Compiler.Core;

public class JackToFileWriter
{
    private string _fileOrDirectory;
    private bool _fileMode = false;
    private ILogger _logger;
    private bool _vmMode;

    public JackToFileWriter(string fileOrDirectory, ILogger logger, bool vmMode = false)
    {
        _logger = logger;
        _fileOrDirectory = fileOrDirectory;
        _vmMode = vmMode;
        if (fileOrDirectory.EndsWith(".jack"))
        {
            _fileMode = true;
        }
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

    private void HandleSingleFile()
    {
        List<string> rawVmCode;
        string outputFile;
        // Write the code to a .hack file
        if (_vmMode)
        {
            outputFile = $"{Path.GetDirectoryName(_fileOrDirectory)}\\{Path.GetFileNameWithoutExtension(_fileOrDirectory)}.xml";
        }
        else
        {
            outputFile = $"{Path.GetDirectoryName(_fileOrDirectory)}\\{Path.GetFileNameWithoutExtension(_fileOrDirectory)}.vm";
        }
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
        var compilationEngine = new XmlCompilationEngine(tokenizer, _logger);
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
            List<string> rawVmCode;
            string outputFile;
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
            if (_vmMode)
            {
                outputFile = $"{Path.GetDirectoryName(file)}\\{Path.GetFileNameWithoutExtension(file)}.xml";
            }
            else
            {
                outputFile = $"{Path.GetDirectoryName(file)}\\{Path.GetFileNameWithoutExtension(file)}.vm"; 
            }
            var newFs = File.Create(outputFile);
            newFs.Close();
            // Now generate the xml file
            var tokenizer = new Tokenizer(rawVmCode, _logger);
            var compilationEngine = new XmlCompilationEngine(tokenizer, _logger);
            compilationEngine.BeginCompilationRoutine();
        
            using var streamWriter = File.AppendText(outputFile);
            foreach (var line in compilationEngine.CodeLines)
            {
                streamWriter.WriteLine(line);
            }
        }
    }
}