public class NLookaheadToken : Token
{

    public NLookaheadToken(string symbol) : base(symbol, OP.NLookahead) { }

    public override string ToString()
    {
        return "(?!" + symbol + ")";
    }

}