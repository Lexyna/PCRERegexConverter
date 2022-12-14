using System;
using System.Collections;
using System.Collections.Generic;

public abstract class Token
{
    public enum OP
    {
        Terminal,
        Group,
        Class,
        Repetition,
        Alternate,
        Optional,
        Plus,
        Star,
        Epsilon,
        EOL,
        Lookahead,
        NLookahead,
        Lookbehind
    }

    public string symbol { get; protected set; }
    public OP tokenOP { get; private set; }

    protected List<Token> internalTokens = new List<Token>();

    public Token(string symbol, OP tokenOP)
    {
        this.symbol = symbol;
        this.tokenOP = tokenOP;
    }

    public override string ToString()
    {
        return symbol;
    }

}