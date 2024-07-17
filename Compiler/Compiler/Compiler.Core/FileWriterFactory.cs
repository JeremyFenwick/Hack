using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Compiler.Core;

public static class FileWriterFactory
{
    public static JackToFileWriter CreateFileWriter(string? fileOrDirectory, bool vmMode = false)
    {
        // Get the path from the console if it is empty
        if (string.IsNullOrEmpty(fileOrDirectory))
        {
            Console.WriteLine("Enter a directory or file. You must provide the full path (example C:\\Documents\\Example.jack)");
            do
            {
                var userInput = Console.ReadLine();
                if (string.IsNullOrEmpty(userInput)) continue;
                fileOrDirectory = userInput;
                break;
            } while (true);
        }
        // Check the file extension
        if (Path.GetExtension(fileOrDirectory) != ".jack" && Path.GetExtension(fileOrDirectory) != string.Empty)
        {
            Console.WriteLine("The input file extension must be a jack file (.jack) or directory!");
            Environment.Exit(1);
        }
        // Read the app settings file, you can add secrets and additional files here
        var config = new ConfigurationBuilder()
            .AddJsonFile($"appsettings.json")
            .Build();
        // Create the logger
        using ILoggerFactory factory = LoggerFactory.Create(builder => 
            builder.AddConsole()
                .AddConfiguration(config.GetSection("Logging")));
        var logger = factory.CreateLogger<JackToFileWriter>();
        // Feed the selected string into the xml read writer
        return new JackToFileWriter(fileOrDirectory, logger, vmMode);
    }
}