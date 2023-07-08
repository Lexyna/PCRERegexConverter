using System;
public class Transition
{
    public string symbol { private set; get; }

    public string uuid { get; private set; }

    State inS, outS;

    public bool delete { get; private set; }

    public bool universal { get; private set; }

    //links universal and existential Transitions together
    //list of all linked universal/existential Transitions
    public Dictionary<string, Transition> universalLink = new Dictionary<string, Transition>();

    public Transition(State inS, string symbol, State outS, bool universal = false)
    {
        this.inS = inS;
        this.outS = outS;
        this.symbol = symbol;
        this.uuid = System.Guid.NewGuid().ToString();
        this.universal = universal;
    }

    public State GetOutState() { return outS; }
    public State GetInState() { return inS; }

    public void SetUniversal(bool uni)
    {
        this.universal = uni;
    }

    public void OverwriteInState(State state)
    {
        delete = true;
        inS.RemoveDeadTransitions();
        this.inS = state;
        delete = false;
        inS.AddOutgoingTransition(this);

    }

    public void Apply()
    {
        //checked for duplicate Transition
        string uuid_in = inS.uuid;
        string uuid_out = outS.uuid;

        for (int i = 0; i < inS.GetOutgoingTransitions().Count; i++)
        {
            Transition possibleDuplicate = inS.GetOutgoingTransitions()[i];
            if (possibleDuplicate.inS.uuid == uuid_in && possibleDuplicate.outS.uuid == uuid_out && possibleDuplicate.symbol == this.symbol)
                return;
        }

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