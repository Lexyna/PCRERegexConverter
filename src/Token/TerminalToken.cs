public class TerminalToken : Token
{

    public TerminalToken(string symbol) : base(symbol, OP.Terminal) { }

}