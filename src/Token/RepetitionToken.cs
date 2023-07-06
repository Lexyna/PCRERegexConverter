using System;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
public class RepetitionToken : Token
{

    static Regex patter = new Regex("\\d+");

    int[] range = new int[] { -1, -1 };

    public RepetitionToken(string symbol) : base(symbol, OP.Repetition)
    {

        MatchCollection matches = patter.Matches(symbol);

        for (int i = 0; i < matches.Count; i++)
        {
            if (i >= 2)
                continue;

            string val = matches[i].Value;

            Int32.TryParse(val, out range[i]);
        }
    }

    public bool IsSimpleToken()
    {

        //Optional
        if (range[0] == 0 && range[1] == 1)
            return true;

        //Plus
        if (range[0] == 1 && range[1] == -1)
            return true;

        //Star
        if (range[0] == 0 && range[1] == -1)
            return true;

        return false;
    }

    public Token.OP ConvertToToken()
    {

        //Optional
        if (range[0] == 0 && range[1] == 1)
            return OP.Optional;

        //Plus
        if (range[0] == 1 && range[1] == -1)
            return OP.Plus;

        //Star
        if (range[0] == 0 && range[1] == -1)
            return OP.Star;

        return OP.Repetition;
    }

    public List<Token> ConvertToTokenStream(Token t)
    {
        List<Token> stream = new List<Token>();

        if (IsSimpleToken())
        {
            switch (ConvertToToken())
            {
                case OP.Optional:
                    stream.Add(t);
                    stream.Add(new AlternateToken());
                    stream.Add(new EpsilonToken());
                    break;
                case OP.Plus:
                    stream.Add(t);
                    stream.Add(t);
                    stream.Add(new StarToken());
                    break;
                case OP.Star:
                    stream.Add(t);
                    stream.Add(new StarToken());
                    break;
            }
            return stream;
        }

        //min max token
        int min = range[0];
        int max = range[1];

        for (int i = 0; i < min; i++)
            stream.Add(t);

        for (int i = 0; i < (max - min); i++)
        {
            GroupToken gt = new GroupToken("");
            GroupToken left = new GroupToken("");
            left.AddToken(t);
            gt.AddToken(left);
            //gt.AddToken(t);
            gt.AddToken(new AlternateToken());
            gt.AddToken(new GroupToken(""));
            //gt.AddToken(new EpsilonToken());
            stream.Add(gt);
        }

        return stream;
    }

}