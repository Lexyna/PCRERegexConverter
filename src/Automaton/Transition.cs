using System;
public class Transition
{
    public string symbol { private set; get; }

    public string uuid { get; private set; }

    State inS, outS;

    public bool delete { get; private set; }

    public Transition(State inS, string symbol, State outS)
    {
        this.inS = inS;
        this.outS = outS;
        this.symbol = symbol;
        this.uuid = System.Guid.NewGuid().ToString();
    }

    public State GetOutState() { return outS; }
    public State GetInState() { return inS; }

    public void Apply()
    {
        inS.AddOutgoingTransition(this);
        outS.AddIngoingTransition(this);
    }
    public void Delete()
    {
        delete = true;
        inS.RemoveDeadTransitions();
        outS.RemoveDeadTransitions();
    }

}