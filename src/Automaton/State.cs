using System.Collections;
using System.Collections.Generic;
public class State
{

    public string id { set; get; }
    public string uuid { get; private set; }
    public bool Simplified = false;
    public bool isEndState { private set; get; }

    public bool isUniversal = false;

    public List<State> laLinks { private set; get; }
    public List<State> nfaLinks { private set; get; }
    public bool linkedState { private set; get; }

    public bool lookaheadState { private set; get; }

    List<Transition> ingoing = new List<Transition>();
    List<Transition> outgoing = new List<Transition>();

    public State(string id, bool isEndState = false, bool isLookaheadState = false)
    {
        this.uuid = System.Guid.NewGuid().ToString();
        this.id = id;
        this.isEndState = isEndState;
        this.lookaheadState = isLookaheadState;
    }

    public State(List<State> nfaLinks, List<State> laLinks, bool isEndState = false)
    {
        for (int i = laLinks.Count - 1; i >= 0; i--)
        {
            if (laLinks[i].isEndState)
                laLinks.RemoveAt(i);
        }

        this.nfaLinks = nfaLinks;
        this.laLinks = laLinks;
        this.uuid = System.Guid.NewGuid().ToString();
        String id = "[";
        this.nfaLinks.ForEach(state => id += state.id);
        this.laLinks.ForEach(state => id += state.id);
        this.id = id + "]";
        this.isEndState = isEndState;
        this.linkedState = true;
        this.lookaheadState = false;
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

    public void SetUniversal(bool universal)
    {
        this.isUniversal = universal;
    }

    public void SetLookaheadState(bool lookaheadState)
    {
        this.lookaheadState = lookaheadState;
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

    public override bool Equals(object? obj)
    {
        var recommendationDTO = obj as State;

        if (recommendationDTO == null)
            return false;
        return this.uuid == recommendationDTO.uuid;
    }

}