using System;
using System.Collections;
using System.Collections.Generic;
public class GroupToken : Token
{

    public GroupToken(string symbol) : base(symbol, OP.Group)
    {
        if (Entry.verbose)
            Console.WriteLine($"Creating GroupToken: {symbol}");
        TokenizeGroup();
    }

    private void TokenizeGroup()
    {
        if (symbol.Length == 0)
            return;

        string regex = symbol.Substring(1, symbol.Length - 2);

        Lexer lexer = new Lexer(regex);
        lexer.Tokenize();

        this.internalTokens = lexer.GetTokens();

        Simplify();
    }

    private void Simplify()
    {
        int index = 0;

        do
        {
            if (Entry.verbose)
                Console.WriteLine($"---------------------------------- Simplifying Group [{index}][{symbol}]----------------------------------\n");
            ParserSimplifier parser = new ParserSimplifier(internalTokens);
            internalTokens = parser.Simplify();
            index++;
        } while (!isDNF());
    }

    public bool isDNF()
    {
        if (internalTokens.Count == 0)
            return true;

        bool isDNF = true;

        for (int i = 0; i < internalTokens.Count; i++)
        {
            if ((internalTokens[i].tokenOP != OP.Group) &&
                internalTokens[i].tokenOP != OP.Alternate)
            {
                return false;
            }

            if (i % 2 == 0 && internalTokens[i].tokenOP != OP.Group)
                return false;

            if (i % 2 == 1 && internalTokens[i].tokenOP != OP.Alternate)
                return false;

        }

        return isDNF;
    }

    public void AddToken(Token token)
    {
        this.symbol += token.symbol;
        this.internalTokens.Add(token);
    }

    public void AddTokenStream(List<Token> tokens)
    {
        for (int i = 0; i < tokens.Count; i++)
        {
            this.symbol += tokens[i].symbol;
            this.internalTokens.Add(tokens[i]);
        }
    }

    public List<Token> GetTokens() { return internalTokens; }

    public override string ToString()
    {
        if (internalTokens.Count == 1)
            return internalTokens[0].ToString();

        string regex = "(";

        for (int i = 0; i < internalTokens.Count; i++)
        {
            regex += internalTokens[i].ToString();
        }

        regex += ")";
        return regex;
    }
}