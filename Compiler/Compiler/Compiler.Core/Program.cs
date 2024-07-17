using Compiler.Core;

JackToFileWriter fileWriter;

switch (args.Length)
{
    case 0:
        fileWriter = FileWriterFactory.CreateFileWriter("");
        fileWriter.GenerateCode();
        break;
    case 1:
        fileWriter = FileWriterFactory.CreateFileWriter(args[0]);
        fileWriter.GenerateCode();
        break;
    case 2:
    {
        if (args[0].ToLower() is not "vm")
        {
            Console.WriteLine("If two arguments are provided, the first argument must be 'vm' and the second a file or directory.");
        }
        fileWriter = FileWriterFactory.CreateFileWriter(args[1], true);
        fileWriter.GenerateCode();
        break;
    }
    default:
        Console.WriteLine("You must provide at most two arguments...");
        break;
}
