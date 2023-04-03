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

        //List of all universal transitions
        List<Transition> universalTransitions = new List<Transition>();
        //List all new states, index corresponds to universalTransitions Transitions
        List<State> universalEntryState = new List<State>();

        State nfa_start_state = new State("q0");
        nfa.AddStartingState(nfa_start_state);
        MapExistentialTransition(nfa_start_state, afa.startStates[0], universalTransitions, universalEntryState);

        for (int i = 0; i < universalTransitions.Count; i++)
        {
            InitPowerSet(universalTransitions[i], universalEntryState[i]);
        }

    }

    private void MapExistentialTransition(State nfaState, State afaState, List<Transition> universalTransitions, List<State> universalEntryState)
    {
        //Create the nfa with only existential transitions, ignoring any lookaheads
        for (int i = 0; i < afaState.GetOutgoingTransitions().Count; i++)
        {
            Transition t = afaState.GetOutgoingTransitions()[i];

            State new_nfa_state = new State(t.GetOutState().id, false);

            if (t.universal)
            {
                universalTransitions.Add(t);
                universalEntryState.Add(new_nfa_state);
                continue;
            }

            //Add all existential transition to nfa
            Transition new_nfa_transition = new Transition(nfaState, t.symbol, new_nfa_state);
            new_nfa_transition.Apply();

            //For each state we added, we repeat the process
            MapExistentialTransition(new_nfa_state, t.GetOutState(), universalTransitions, universalEntryState);
        }
    }


    //This function inits the power set calculation for the subAutomaton Connected via the given transition at the Entry state
    private void InitPowerSet(Transition universalTransition, State entry)
    {
        //Keeps track of all already created states
        Dictionary<string, bool> visited = new Dictionary<string, bool>();



    }

    //
    private void CreatePowerSet(State curr, List<State> possible)
    {

    }

}