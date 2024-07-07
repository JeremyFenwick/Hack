// See https://aka.ms/new-console-template for more information

using Compiler.Core;
using Compiler.Core.Enums;

// using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
// ILogger logger = factory.CreateLogger("Program");
// logger.LogInformation("Hello World! Logging is {Description}.", "fun");

var test = TokenType.StringConst;
Console.WriteLine($"{test}");