public class TerminalToken : Token
{

    public TerminalToken(string symbol) : base(symbol, OP.Terminal)
    {
        if (Entry.verbose)
            Console.WriteLine($"Creating TerminalToken: {symbol}");
    }

}