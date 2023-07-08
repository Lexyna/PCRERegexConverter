public class NLookaheadToken : Token
{

    public NLookaheadToken(string symbol) : base(symbol, OP.NLookahead)
    {
        if (Entry.verbose)
            Console.WriteLine($"Creating NegativeLookaheadToken:{symbol}");
    }

    public override string ToString()
    {
        return "(?!" + symbol + ")";
    }

}