using VirtualMachine.Core;
using VirtualMachine.Core.Enums;

namespace VirtualMachine.Tests;

public class ControlTests
{
    
    [Test]
    public void TestLabelParse()
    {
        var onlyCommand = new List<string>()
        {
            "label TESTLABEL"
        };
        var parser = new Parser(onlyCommand);
        parser.Advance();
        Assert.That(parser.CurrentCommand.CommandText, Is.EqualTo("label")); 
        Assert.That(parser.CurrentCommand.ArgumentOne, Is.EqualTo("TESTLABEL")); 
        Assert.That(parser.CurrentCommand.CommandType, Is.EqualTo(CommandType.Label));
    }
    
    [Test]
    public void TestLabelCommand()
    {
        var currentCommand = new Command()
        {
            CommandText = "label",
            CommandType = CommandType.Label,
            CommandNumber = 1,
            ArgumentOne = "TESTLABEL",
            AssemblyCommandList = new List<string>()
        };

        currentCommand = CommandCodeGenerator.CommandCodeGen(currentCommand);
        Assert.That(currentCommand.AssemblyCommandList[0], Is.EqualTo("(TESTLABEL)"));
    }
    
    [Test]
    public void TestGotoCommand()
    {
        var currentCommand = new Command()
        {
            CommandText = "goto",
            CommandType = CommandType.Goto,
            CommandNumber = 1,
            ArgumentOne = "TESTLABEL",
            AssemblyCommandList = new List<string>()
        };

        currentCommand = CommandCodeGenerator.CommandCodeGen(currentCommand);
        Assert.That(currentCommand.AssemblyCommandList[0], Is.EqualTo("@TESTLABEL"));
        Assert.That(currentCommand.AssemblyCommandList[1], Is.EqualTo("0;JMP"));
    }
    
    [Test]
    public void TestGotoIfCommand()
    {
        var currentCommand = new Command()
        {
            CommandText = "if-goto",
            CommandType = CommandType.If,
            CommandNumber = 1,
            ArgumentOne = "TESTLABEL",
            AssemblyCommandList = new List<string>()
        };

        currentCommand = CommandCodeGenerator.CommandCodeGen(currentCommand);
        Assert.That(currentCommand.AssemblyCommandList[0], Is.EqualTo("@SP"));
    }
}