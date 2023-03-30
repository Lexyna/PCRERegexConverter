using System;
using System.Collections;
using System.Collections.Generic;
public class ParserSimplifier
{

    List<Token> tokens;

    public ParserSimplifier(List<Token> tokens)
    {
        this.tokens = tokens;
    }

    public List<Token> Simplify()
    {
        Console.WriteLine("-------------- Simplifying tokens -----------------------");

        Console.WriteLine("Original: " + TokenStreamToString());
        Console.WriteLine("Stream: " + PrintTokenStream());

        this.tokens = Groupification();

        Console.WriteLine("Simplified: " + TokenStreamToString());
        Console.WriteLine("Stream: " + PrintTokenStream());

        return tokens;
    }

    /*
        Tales a regexToken stream and converts it to a "Disjunctive normal form"
    */
    private List<Token> Groupification()
    {

        List<Token> dnf_tokens = new List<Token>();

        int i = 0;

        GroupToken mainGroup = new GroupToken("");

        while (i < tokens.Count)
        {
            if (tokens[i].tokenOP == Token.OP.Alternate)
            {
                dnf_tokens.Add(mainGroup);
                dnf_tokens.Add(new AlternateToken());
                mainGroup = new GroupToken("");
                i++;
                continue;
            }

            Token tokenToAdd = tokens[i];
            if (tokenToAdd.tokenOP == Token.OP.Class)
                tokenToAdd = ((ClassToken)tokenToAdd).ConvertToGroup();

            //Implement lookahead 2
            int lookahead = Lookahead(i);

            switch (lookahead)
            {
                case 0:
                    mainGroup.AddToken(tokenToAdd);
                    i++;
                    break;
                case 1:
                    GroupToken gt = new GroupToken("");
                    GroupToken left = new GroupToken("");
                    left.AddToken(tokenToAdd);
                    gt.AddToken(left);
                    //gt.AddToken(tokenToAdd);
                    gt.AddToken(new AlternateToken());
                    //gt.AddToken(new EpsilonToken());
                    gt.AddToken(new GroupToken(""));
                    mainGroup.AddToken(gt);
                    i += 2;
                    break;
                case 2:
                    mainGroup.AddToken(tokenToAdd);
                    mainGroup.AddToken(tokenToAdd);
                    mainGroup.AddToken(new StarToken());
                    i += 2;
                    break;
                case 3:
                    RepetitionToken rep = (RepetitionToken)tokens[i + 1];
                    mainGroup.AddTokenStream(rep.ConvertToTokenStream(tokenToAdd));
                    i += 2;
                    break;
                case 4:
                    mainGroup.AddToken(tokenToAdd);
                    mainGroup.AddToken(new StarToken());
                    mainGroup.AddToken(new NLookaheadToken(tokenToAdd.symbol));
                    i += 3;
                    break;
                case 5:
                    mainGroup.AddToken(tokenToAdd);
                    mainGroup.AddToken(tokenToAdd);
                    mainGroup.AddToken(new StarToken());
                    mainGroup.AddToken(new NLookaheadToken(tokenToAdd.symbol));
                    i += 3;
                    break;
                case 6:
                    RepetitionToken rep_p = (RepetitionToken)tokens[i + 1];
                    mainGroup.AddTokenStream(rep_p.ConvertToTokenStream(tokenToAdd));
                    mainGroup.AddToken(new NLookaheadToken(tokenToAdd.symbol));
                    i += 3;
                    break;
                case 7:
                    mainGroup.AddToken(tokenToAdd);
                    mainGroup.AddToken(new StarToken());
                    i += 2;
                    break;
            }
        }

        dnf_tokens.Add(mainGroup);

        return dnf_tokens;
    }

    /*
        Looks +2 into the tokenStream and returns a value based on the token behavior
        Normal Token: 0
        Optional: 1
        Plus: 2
        Repetition: 3
        Possessive star: 4
        Possessive plus: 5
        Possessive repetition: 6
        Star: 7
    */
    private int Lookahead(int index)
    {
        if (index >= tokens.Count - 1)
            return 0;

        int ret = 0;

        switch (tokens[index + 1].tokenOP)
        {
            case Token.OP.Optional: ret = 1; break;
            case Token.OP.Plus: ret = 2; break;
            case Token.OP.Repetition: ret = 3; break;
            case Token.OP.Star: ret = 7; break;
        }

        if (index + 2 > tokens.Count - 1)
            return ret;

        if (tokens[index + 2].tokenOP == Token.OP.Plus)
        {
            switch (ret)
            {
                case 7: ret = 4; break;
                case 2: ret = 5; break;
                case 3: ret = 6; break;
            }
        }

        if (ret == 7)
            ret = 0;

        return ret;
    }

    public List<Token> GetTokens() { return this.tokens; }

    public string TokenStreamToString()
    {
        string regex = "";

        for (int i = 0; i < tokens.Count; i++)
            regex += tokens[i].ToString();

        return regex;
    }

    public string PrintTokenStream()
    {

        string regex = "";

        for (int i = 0; i < this.tokens.Count; i++)
            regex += " " + this.tokens[i].tokenOP;

        return regex;
    }

}

/*

    Literal -> Literal => Group
    Literal -> Group => Group
    Group -> optional => Group
    .
    .
    .
    Group -> Group -> Group

    Group Group optional Group OR

*/