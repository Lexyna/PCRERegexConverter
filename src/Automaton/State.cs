using System.Collections;
using System.Collections.Generic;
public class State
{

    public string id { set; get; }
    public string uuid { get; private set; }
    public bool visited = false;
    public bool Simplified = false;
    public bool isEndState { private set; get; }

    //Used for AFA to NFA conversion. A marked state means this state contains a yet unresolved Transition to a lookahead sub automaton
    public Queue<string> marker = new Queue<string>();

    List<Transition> ingoing = new List<Transition>();
    List<Transition> outgoing = new List<Transition>();

    public State(string id, bool isEndState = false)
    {
        this.uuid = System.Guid.NewGuid().ToString();
        this.id = id;
        this.isEndState = isEndState;
    }

    public void SimplifyName(ref int index)
    {
        Simplified = true;
        this.id = "q" + index;
        index++;

        for (int i = 0; i < outgoing.Count; i++)
        {
            if (!outgoing[i].GetOutState().Simplified)
                outgoing[i].GetOutState().SimplifyName(ref index);
        }
    }

    public void SetEndState(bool isEndState)
    {
        this.isEndState = isEndState;
    }

    public void AddIngoingTransition(Transition t)
    {
        ingoing.Add(t);
    }

    public void AddOutgoingTransition(Transition t)
    {
        outgoing.Add(t);
    }

    public void RemoveDeadTransitions()
    {

        for (int i = ingoing.Count - 1; i >= 0; i--)
            if (ingoing[i].delete) ingoing.RemoveAt(i);

        for (int i = outgoing.Count - 1; i >= 0; i--)
            if (outgoing[i].delete) outgoing.RemoveAt(i);

    }

    public List<Transition> GetIngoingTransitions() { return ingoing; }
    public List<Transition> GetOutgoingTransitions() { return outgoing; }

}