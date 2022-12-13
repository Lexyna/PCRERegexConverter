public class Transition
{
    string symbol;

    State inS, outS;

    public Transition(State inS, string symbol, State outS)
    {
        this.inS = inS;
        this.outS = outS;
        this.symbol = symbol;
    }
}