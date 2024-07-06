﻿using Compiler.Core.Enums;

namespace Compiler.Core;

public class Token
{
    public required TokenType TokenType { get; init; }
    public required string TokenValue { get; init; }
}