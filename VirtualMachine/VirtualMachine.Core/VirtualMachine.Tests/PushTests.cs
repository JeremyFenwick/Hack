using NuGet.Frameworks;
using VirtualMachine.Core;
using VirtualMachine.Core.Enums;

namespace VirtualMachine.Tests;

public class PushTests
{
    [Test]
    public void TestPushConstantCommand()
    {
        var currentCommand = new Command()
        {
            CommandText = "push",
            CommandType = CommandType.Push,
            CommandNumber = 1,
            ArgumentOne = "constant",
            ArgumentTwo = 11,
            AssemblyCommandList = new List<string>()
        };

        currentCommand = CommandCodeGenerator.CommandCodeGen(currentCommand);
        Assert.That(currentCommand.AssemblyCommandList[1], Is.EqualTo("@11"));
    }
    
    [Test]
    public void TestPushLocalCommand()
    {
        var currentCommand = new Command()
        {
            CommandText = "push",
            CommandType = CommandType.Push,
            CommandNumber = 1,
            ArgumentOne = "local",
            ArgumentTwo = 2,
            AssemblyCommandList = new List<string>()
        };

        currentCommand = CommandCodeGenerator.CommandCodeGen(currentCommand);
        Assert.That(currentCommand.AssemblyCommandList[3], Is.EqualTo("@LCL"));
    }
    
    [Test]
    public void TestPushTempCommand()
    {
        var currentCommand = new Command()
        {
            CommandText = "push",
            CommandType = CommandType.Push,
            CommandNumber = 1,
            ArgumentOne = "temp",
            ArgumentTwo = 2,
            AssemblyCommandList = new List<string>()
        };

        currentCommand = CommandCodeGenerator.CommandCodeGen(currentCommand);
        Assert.That(currentCommand.AssemblyCommandList[1], Is.EqualTo("@7"));
    }
    
    [Test]
    public void TestPushStaticCommand()
    {
        var currentCommand = new Command()
        {
            CommandText = "push",
            CommandType = CommandType.Push,
            CommandNumber = 1,
            ArgumentOne = "static",
            ArgumentTwo = 3,
            AssemblyCommandList = new List<string>()
        };

        currentCommand = CommandCodeGenerator.CommandCodeGen(currentCommand);
        Assert.That(currentCommand.AssemblyCommandList[1], Is.EqualTo("@static3"));
    }
    
    [Test]
    public void TestPushPointerCommand()
    {
        var currentCommand = new Command()
        {
            CommandText = "push",
            CommandType = CommandType.Push,
            CommandNumber = 1,
            ArgumentOne = "pointer",
            ArgumentTwo = 1,
            AssemblyCommandList = new List<string>()
        };

        currentCommand = CommandCodeGenerator.CommandCodeGen(currentCommand);
        Assert.That(currentCommand.AssemblyCommandList[1], Is.EqualTo("@THAT"));
    }
}