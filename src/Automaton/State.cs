using System.Collections;
using System.Collections.Generic;
public class State
{

    string id;

    //bool isUniversalTransition = false; //For AFA, moves to all connected states

    List<Transition> ingoing = new List<Transition>();
    List<Transition> outgoing = new List<Transition>();

    public State(string id)
    {
        this.id = id;
    }

    public void AddIngoingTransition(Transition t)
    {
        ingoing.Add(t);
    }

    public void AddOutgoingTransition(Transition t)
    {
        outgoing.Add(t);
    }

}