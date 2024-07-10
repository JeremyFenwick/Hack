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
        throw new NotImplementedException();
        
        // var fileList = Directory.GetFiles(_fileOrDirectory);
        // if (!File.Exists(_fileOrDirectory + "\\Sys.vm"))
        // {
        //     Console.WriteLine("A file named \"Sys.vm\" is required in the target folder");
        //     Environment.Exit(1);
        // }
        // Dictionary<string, List<string>> rawVmCode = [];
        // // Write the code to a .hack file
        // var outputFile = $"{_fileOrDirectory}\\{new DirectoryInfo(_fileOrDirectory).Name}.asm";
        // var newFs = File.Create(outputFile);
        // newFs.Close();
        // // Add the Sys.vm to the raw code first
        // rawVmCode.Add("Sys", File.ReadLines(_fileOrDirectory + "\\Sys.vm").ToList()); 
        // // Bootstrap the vm code
        // rawVmCode["Sys"].Insert(0,"call Sys.init");
        // // Loop through remaining vm files and append their text to the list
        // foreach (var file in fileList)
        // {
        //     if (Path.GetExtension(file) != ".vm") continue;
        //     if (Path.GetFileNameWithoutExtension(file) == "Sys") continue;
        //     var fileCode = File.ReadLines(file).ToList();
        //     rawVmCode.Add(Path.GetFileNameWithoutExtension(file),fileCode);
        // }
        //
        // foreach (var (file, codeList) in rawVmCode)
        // {
        //     var codeGen = new CommandCodeGenerator(file);
        //     var parser = new Parser(codeList);
        //     using var streamWriter = File.AppendText(outputFile);
        //     do
        //     {
        //         parser.Advance();
        //         var currentCommand = parser.CurrentCommand;
        //         if (currentCommand == null) break;
        //         currentCommand = codeGen.CommandCodeGen(currentCommand);
        //         foreach (var assemblyCommand in currentCommand.AssemblyCommandList)
        //         {
        //             streamWriter.WriteLine(assemblyCommand);
        //         }
        //     } while (parser.HasMoreLines);
        // }
    }
}