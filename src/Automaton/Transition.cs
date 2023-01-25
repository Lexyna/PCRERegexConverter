public class Transition
{
    public string symbol { private set; get; }

    State inS, outS;

    public bool delete { get; private set; }

    public Transition(State inS, string symbol, State outS)
    {
        this.inS = inS;
        this.outS = outS;
        this.symbol = symbol;
    }

    public State GetOutState() { return outS; }
    public State GetInState() { return inS; }

    public void Apply()
    {
        inS.AddOutgoingTransition(this);
        outS.AddOutgoingTransition(this);
    }
    public void Delete()
    {
        delete = true;
    }

}