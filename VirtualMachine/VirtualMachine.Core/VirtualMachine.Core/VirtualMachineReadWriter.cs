﻿using System.Globalization;
using System.Net.Mime;

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
        var codeGen = new CommandCodeGenerator("Sys");
        using var streamWriter = File.AppendText(outputFile);
        do
        {
            parser.Advance();
            var currentCommand = parser.CurrentCommand;
            if (currentCommand == null) break;
            currentCommand = codeGen.CommandCodeGen(currentCommand);
            foreach (var assemblyCommand in currentCommand.AssemblyCommandList)
            {
                streamWriter.WriteLine(assemblyCommand);
            }
    
        } while (parser.HasMoreLines);
    }

    private void HandleDirectory()
    {
        var fileList = Directory.GetFiles(_fileOrDirectory);
        if (!File.Exists(_fileOrDirectory + "\\Sys.vm"))
        {
            Console.WriteLine("A file named \"Sys.vm\" is required in the target folder");
            Environment.Exit(1);
        }
        Dictionary<string, List<string>> rawVmCode = [];
        // Write the code to a .hack file
        var outputFile = $"{_fileOrDirectory}\\{new DirectoryInfo(_fileOrDirectory).Name}.asm";
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
        // Add the Sys.vm to the raw code first
        rawVmCode.Add("Sys", File.ReadLines(_fileOrDirectory + "\\Sys.vm").ToList()); 
        // Bootstrap the vm code
        rawVmCode["Sys"].Insert(0,"call Sys.init");
        // Loop through remaining vm files and append their text to the list
        foreach (var file in fileList)
        {
            if (Path.GetExtension(file) != ".vm") continue;
            if (Path.GetFileNameWithoutExtension(file) == "Sys") continue;
            var fileCode = File.ReadLines(file).ToList();
            rawVmCode.Add(Path.GetFileNameWithoutExtension(file),fileCode);
        }

        foreach (var (file, codeList) in rawVmCode)
        {
            var codeGen = new CommandCodeGenerator(file);
            var parser = new Parser(codeList);
            using var streamWriter = File.AppendText(outputFile);
            do
            {
                parser.Advance();
                var currentCommand = parser.CurrentCommand;
                if (currentCommand == null) break;
                currentCommand = codeGen.CommandCodeGen(currentCommand);
                foreach (var assemblyCommand in currentCommand.AssemblyCommandList)
                {
                    streamWriter.WriteLine(assemblyCommand);
                }
            } while (parser.HasMoreLines);
        }
    }
}