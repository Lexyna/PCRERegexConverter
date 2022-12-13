public class LookaheadToken : Token
{

    public LookaheadToken(string symbol) : base(symbol, OP.Lookahead) { }

    public override string ToString()
    {
        return "(?=" + symbol + ")";
    }

}