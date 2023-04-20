public class LookaheadToken : Token
{

    public LookaheadToken(string symbol) : base(symbol, OP.Lookahead)
    {
        Tokenize();
    }

    public override string ToString()
    {
        return symbol;
    }

    private void Tokenize()
    {

        string subExpression = symbol.Substring(3, symbol.Length - 4);

        Lexer lexer = new Lexer(subExpression);
        lexer.Tokenize();

        ParserSimplifier parser = new ParserSimplifier(lexer.GetTokens());
        internalTokens = parser.Simplify();

    }

    public List<Token> GetToken() { return this.internalTokens; }

}