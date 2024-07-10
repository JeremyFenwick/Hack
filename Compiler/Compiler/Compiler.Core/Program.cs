﻿// Arg1 is the input file (it must be a vm file)

using Compiler.Core;
using Compiler.Core.SyntaxAnalyzer;

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

// Feed the selected string into the xml read writer
var xmlWriter = new JackToXmlWriter(selectedString);
xmlWriter.GenerateXml();