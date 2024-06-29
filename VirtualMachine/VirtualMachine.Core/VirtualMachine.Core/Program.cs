// Arg1 is the input file (it must be a vm file)

using VirtualMachine.Core;

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
    Console.WriteLine("Enter a directory or file. You must provide the full path (example C:\\Documents\\Example.vm)");
    do
    {
        var userInput = Console.ReadLine();
        if (string.IsNullOrEmpty(userInput)) continue;
        selectedString = userInput;
        break;
    } while (true);
}
// Check the file extension
if (Path.GetExtension(selectedString) != ".vm" && Path.GetExtension(selectedString) != string.Empty)
{
    throw new Exception("The input file extension must be virtual machine (.vm) or directory!");
}

// Feed the selected string into the vm read writer
var vmRw = new VirtualMachineReadWriter(selectedString);
vmRw.GenerateAsmCode();