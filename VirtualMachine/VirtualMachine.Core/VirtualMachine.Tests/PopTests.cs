using VirtualMachine.Core;
using VirtualMachine.Core.Enums;

namespace VirtualMachine.Tests;

public class PopTests
{
    [Test]
    public void TestPopLocalCommand()
    {
        var currentCommand = new Command()
        {
            CommandText = "pop",
            CommandType = CommandType.Pop,
            CommandNumber = 1,
            ArgumentOne = "local",
            ArgumentTwo = 1,
            AssemblyCommandList = new List<string>()
        };

        currentCommand = CommandCodeGenerator.CommandCodeGen(currentCommand);
        Assert.That(currentCommand.AssemblyCommandList[1], Is.EqualTo("@LCL"));
    }
    
    [Test]
    public void TestPopTempCommand()
    {
        var currentCommand = new Command()
        {
            CommandText = "pop",
            CommandType = CommandType.Pop,
            CommandNumber = 1,
            ArgumentOne = "temp",
            ArgumentTwo = 3,
            AssemblyCommandList = new List<string>()
        };

        currentCommand = CommandCodeGenerator.CommandCodeGen(currentCommand);
        Assert.That(currentCommand.AssemblyCommandList[5], Is.EqualTo("@8"));
    }
}