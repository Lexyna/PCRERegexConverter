using System.Collections;
using System.Collections.Generic;
public class State
{

    public string id { private set; get; }

    public bool visited = false;
    public bool isEndState { private set; get; }

    //bool isUniversalTransition = false; //For AFA, moves to all connected states

    List<Transition> ingoing = new List<Transition>();
    List<Transition> outgoing = new List<Transition>();

    public State(string id, bool isEndState = false)
    {
        this.id = id;
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

    public List<Transition> GetIngoingTransitions() { return ingoing; }
    public List<Transition> GetOutgoingTransitions() { return outgoing; }

}