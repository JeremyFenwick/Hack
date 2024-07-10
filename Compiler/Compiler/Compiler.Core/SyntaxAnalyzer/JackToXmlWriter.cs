using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Compiler.Core.SyntaxAnalyzer;

public class JackToXmlWriter
{
    private string _fileOrDirectory;
    private bool _fileMode = false;
    private IConfigurationRoot _config;

    public JackToXmlWriter(string fileOrDirectory)
    {
        _fileOrDirectory = fileOrDirectory;
        if (fileOrDirectory.EndsWith(".jack"))
        {
            _fileMode = true;
        }
        
        // Read the app settings file, you can add secrets and additional files here
        _config = new ConfigurationBuilder()
            .AddJsonFile($"appsettings.json")
            .Build();

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
        // Create the logger
        using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole()
            .AddConfiguration(_config.GetSection("Logging")));
        var logger = factory.CreateLogger<JackToXmlWriter>();
        // Now generate the xml file
        var tokenizer = new Tokenizer(rawVmCode, logger);
        var compilationEngine = new CompilationEngine(tokenizer, logger);
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
        
        using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole()
            .AddConfiguration(_config.GetSection("Logging")));
        var logger = factory.CreateLogger<JackToXmlWriter>();
        
        foreach (var file in fileList)
        {
            logger.LogInformation($"Parsing file: {file}");
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
            var tokenizer = new Tokenizer(rawVmCode, logger);
            var compilationEngine = new CompilationEngine(tokenizer, logger);
            compilationEngine.BeginCompilationRoutine();
        
            using var streamWriter = File.AppendText(outputFile);
            foreach (var line in compilationEngine.XmlLines)
            {
                streamWriter.WriteLine(line);
            }
        }
    }
}