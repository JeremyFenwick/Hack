namespace VirtualMachine.Core;

public class VirtualMachineReadWriter
{
    private string _fileOrDirectory;
    private bool _fileMode = false;

    public VirtualMachineReadWriter(string fileOrDirectory)
    {
        _fileOrDirectory = fileOrDirectory;
        if (fileOrDirectory.EndsWith(".vm"))
        {
            _fileMode = true;
        }
    }

    public void GenerateAsmCode()
    {
        if (_fileMode) HandleSingleFile();
    }

    private void HandleSingleFile()
    {
        List<string> rawVmCode;
        // Write the code to a .hack file
        var outputFile = $"{Path.GetDirectoryName(_fileOrDirectory)}\\{Path.GetFileNameWithoutExtension(_fileOrDirectory)}.asm";
        var newFs = File.Create(outputFile);
        newFs.Close();
        // Bootstrap the asm code to set the stack pointer
        var bootstrapWriter = File.AppendText(outputFile);
        bootstrapWriter.WriteLine("// SP = 256");
        bootstrapWriter.WriteLine("@256");
        bootstrapWriter.WriteLine("D=A");
        bootstrapWriter.WriteLine("@SP");
        bootstrapWriter.WriteLine("M=D");
        bootstrapWriter.Close();
        // Attempt to load the file
        try
        {
            rawVmCode = File.ReadLines(_fileOrDirectory).ToList();
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
        // Bootstrap the vm code
        rawVmCode.Insert(0,"call Sys.init");
        // Now generate the assembly file
        var parser = new Parser(rawVmCode);
        if (parser.HasMoreLines == false) {
            System.Environment.Exit(1);
        }
        // 
        using var streamWriter = File.AppendText(outputFile);
        do
        {
            parser.Advance();
            var currentCommand = parser.CurrentCommand;
            if (currentCommand == null) break;
            currentCommand = CommandCodeGenerator.CommandCodeGen(currentCommand);
            foreach (var assemblyCommand in currentCommand.AssemblyCommandList)
            {
                streamWriter.WriteLine(assemblyCommand);
            }
    
        } while (parser.HasMoreLines);
    }
    
}