using VirtualMachine.Core;
using VirtualMachine.Core.Enums;

namespace VirtualMachine.Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TestSingleCommand()
    {
        var singleCommand = new List<string>()
        {
            "push constant 2"
        };
        var parser = new Parser(singleCommand);
        parser.Advance();
        Assert.That(parser.CurrentCommand.CommandType, Is.EqualTo(CommandType.Push));
    }
    
    [Test]
    public void TestCommandsWithComments()
    {
        var singleCommand = new List<string>()
        {
            "// First comment",
            "push constant 2 //  Second Comment",
            "// Third comment",
            "pop local 1",
            "// fourth comment"
        };
        var parser = new Parser(singleCommand);
        parser.Advance();
        parser.Advance();
        var noMoreLines = parser.Advance();
        Assert.That(parser.HasMoreLines, Is.EqualTo(noMoreLines));
    }
    
    [Test]
    public void TestRetrievingMultipleCommands()
    {
        var singleCommand = new List<string>()
        {
            "// First comment",
            "push constant 2 //  Second Comment",
            "// Third comment",
            "pop local 1",
            "// fourth comment",
            "push push 8"
        };
        var parser = new Parser(singleCommand);
        
        parser.Advance();
        var firstCommand = parser.CurrentCommand;
        Assert.That(firstCommand.CommandType, Is.EqualTo(CommandType.Push));

        parser.Advance();
        var secondCommand = parser.CurrentCommand;
        Assert.That(secondCommand.CommandType, Is.EqualTo(CommandType.Pop));
        
        parser.Advance();
        var thirdCommand = parser.CurrentCommand;
        Assert.That(thirdCommand.ArgumentTwo, Is.EqualTo(8));
        Assert.That(parser.HasMoreLines, Is.EqualTo(false));
    }
}