using System;
public class Transition
{
    private static List<Transition> universalTransitions = new List<Transition>();

    public static void AddUniversalTransition(Transition transition)
    {
        universalTransitions.Add(transition);
    }

    public static void ResolveUniversalLinks()
    {

        foreach (Transition transition in universalTransitions)
        {
            State startState = transition.GetInState();

            Dictionary<string, Transition> reachable = new Dictionary<string, Transition>();

            EpsilonEliminator.FindReachableStates(startState, reachable);

            foreach (KeyValuePair<string, Transition> entry in reachable)
            {
                Transition reachableTransition = entry.Value;

                if (reachableTransition.symbol != "" && !transition.universalLink.ContainsKey(reachableTransition.uuid))
                {
                    transition.universalLink.Add(reachableTransition.uuid, reachableTransition);
                    if (!reachableTransition.universalLink.ContainsKey(transition.uuid))
                        reachableTransition.universalLink.Add(transition.uuid, transition);
                }
            }

        }

    }

    // Class 

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