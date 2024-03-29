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

    private bool verbose;

    protected List<Token> internalTokens = new List<Token>();

    public Token(string symbol, OP tokenOP, bool verbose = false)
    {
        this.symbol = symbol;
        this.tokenOP = tokenOP;
        this.verbose = verbose;
    }

    public string PrintTokenStream()
    {

        string regex = "";

        for (int i = 0; i < this.internalTokens.Count; i++)
            regex += " " + this.internalTokens[i].tokenOP;

        return regex;
    }

    public override string ToString()
    {
        return symbol;
    }

    public bool ContainsLookahead()
    {
        if (this.tokenOP == OP.Lookahead)
            return true;

        for (int i = 0; i < this.internalTokens.Count; i++)
        {
            switch (this.internalTokens[i].tokenOP)
            {
                case Token.OP.Lookahead: return true;
                case Token.OP.Group:
                    if (this.internalTokens[i].ContainsLookahead())
                        return true;
                    continue;
                default: continue;
            }
        }
        return false;
    }

}