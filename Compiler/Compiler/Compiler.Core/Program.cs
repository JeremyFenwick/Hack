// Arg1 is the input file (it must be a vm file)

using Compiler.Core;
using Compiler.Core.SyntaxAnalyzer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

string selectedString;
try
{
    selectedString = args[0];
}
catch
{
    selectedString = "";
}
// Get the path from the console
if (string.IsNullOrEmpty(selectedString))
{
    Console.WriteLine("Enter a directory or file. You must provide the full path (example C:\\Documents\\Example.jack)");
    do
    {
        var userInput = Console.ReadLine();
        if (string.IsNullOrEmpty(userInput)) continue;
        selectedString = userInput;
        break;
    } while (true);
}
// Check the file extension
if (Path.GetExtension(selectedString) != ".jack" && Path.GetExtension(selectedString) != string.Empty)
{
    Console.WriteLine("The input file extension must be a jack file (.jack) or directory!");
    Environment.Exit(1);
}
// Read the app settings file, you can add secrets and additional files here
var config = new ConfigurationBuilder()
    .AddJsonFile($"appsettings.json")
    .Build();
// Create the logger
using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole()
    .AddConfiguration(config.GetSection("Logging")));
var logger = factory.CreateLogger<JackToXmlWriter>();
// Feed the selected string into the xml read writer
var xmlWriter = new JackToXmlWriter(selectedString, logger);
xmlWriter.GenerateXml();