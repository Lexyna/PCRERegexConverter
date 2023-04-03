public class AFAToNFAConverter
{

    Automaton afa;

    public Automaton nfa { get; private set; }

    //true if we currently calculate the power set for a given lookahead sub automaton
    //Prevents calculation of other power sets at the time 
    private bool calculate_power_set = false;

    public AFAToNFAConverter(Automaton afa)
    {
        this.afa = afa;
        this.nfa = new Automaton();
        Convert();
    }

    private void Convert()
    {

        int subStates = afa.GetUniversalTransitionCount();

        if (subStates == 0)
        {
            this.nfa = afa;
            return;
        }

        //List of all already added States
        Dictionary<string, bool> visited = new Dictionary<string, bool>();

        //List of all currently reachable states for the subAutomaton
        List<State> lookaheadSubState = new List<State>();

        State nfa_start_state = new State("q0");
        nfa.AddStartingState(nfa_start_state);
        MapExistentialTransition(nfa_start_state, afa.startStates[0]);

    }

    private void MapExistentialTransition(State nfaState, State afaState)
    {
        //Create the nfa with only existential transitions, ignoring any lookaheads
        for (int i = 0; i < afaState.GetOutgoingTransitions().Count; i++)
        {
            Transition t = afaState.GetOutgoingTransitions()[i];

            if (t.universal) continue;

            //Add all existential transition to nfa
            State new_nfa_state = new State(t.GetOutState().id, t.GetOutState().isEndState);
            Transition new_nfa_transition = new Transition(nfaState, t.symbol, new_nfa_state);
            new_nfa_transition.Apply();

            //For each state, we added, we repeat the process
            MapExistentialTransition(new_nfa_state, t.GetOutState());
        }

    }



}