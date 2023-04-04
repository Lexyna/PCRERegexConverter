public class AFAToNFAConverter
{

    Automaton afa;

    public Automaton nfa { get; private set; }

    //If the power set for a lookahead automaton is created, but the Created power set itself contain another lookahead,
    //the endStates can not be assigned without resolving the other lookaheads. Pseudo Mode disables the setting of endStates.
    //private bool pseudoMode = false;

    //keeps track of all 'endStates' that can't be applied since another lookahead isn't resolved yet
    HashSet<string> pseudoEndStates = new HashSet<string>();
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

        //standalone NFA, extracted from subAutomaton lookahead
        Dictionary<string, State> standaloneNFA = new Dictionary<string, State>();
        //list of marked States, string is the uuid key for the standalone subAutomaton, State the entry for the powerSet creation 
        Queue<State> marked = new Queue<State>();

        State nfa_start_state = new State("q0");
        nfa.AddStartingState(nfa_start_state);
        MapExistentialTransition(nfa_start_state, afa.startStates[0], standaloneNFA, marked);

        InitPowerSet(standaloneNFA, marked);
    }

    private void MapExistentialTransition(State nfaState, State afaState, Dictionary<string, State> standaloneNFA, Queue<State> marked)
    {
        //Create the 'sterilized' nfa with only existential transitions, ignoring any lookaheads and without any endStates
        for (int i = 0; i < afaState.GetOutgoingTransitions().Count; i++)
        {
            Transition t = afaState.GetOutgoingTransitions()[i];

            if (t.universal)
            {
                if (!standaloneNFA.ContainsKey(t.GetOutState().uuid))
                    standaloneNFA.Add(t.GetOutState().uuid, t.GetOutState());
                marked.Enqueue(nfaState);
                nfaState.marker.Enqueue(t.GetOutState().uuid);
                continue;
            }

            //Add all existential transition to nfa
            State new_nfa_state = new State("n" + afaState.id, false);
            Transition new_nfa_transition = new Transition(nfaState, t.symbol, new_nfa_state);
            new_nfa_transition.Apply();

            //If this state in the afa is an ednState, the nfa State will be a pseudoEndState
            if (t.GetOutState().isEndState)
                pseudoEndStates.Add(new_nfa_state.uuid);

            //For each state we added, we repeat the process
            MapExistentialTransition(new_nfa_state, t.GetOutState(), standaloneNFA, marked);
        }
    }


    //Handles the  initialization of for the power set creation
    private void InitPowerSet(Dictionary<string, State> standaloneNFA, Queue<State> marked)
    {
        //We want to create the powerSet for each marked State
        while (marked.Count > 0)
        {
            //take the first markedState
            State mState = marked.Dequeue();

            //we now resolve all markers
            while (mState.marker.Count > 0)
            {
                //uuid key of the standaloneNFA
                string uuid = mState.marker.Dequeue();

                HashSet<string> visited = new HashSet<string>();

                //Calculate the powerSet
                CalculatePowerSet(null, mState, standaloneNFA[uuid], visited, false, marked);
            }
        }
    }

    //pseudoMode determines whatever a new potential endStates is an endStates or should be added to the list of pseudoEndStates
    private void CalculatePowerSet(State? prevComboState, State curr, State currLookahead, HashSet<string> visited, bool pseudoMode, Queue<State> marked)
    {
        if (visited.Contains(curr.uuid)) return;
        visited.Add(curr.uuid);

        int currTransition = curr.GetOutgoingTransitions().Count;

        //for each Transition in in the current State
        for (int i = 0; i < currTransition; i++)
        {
            //and each Transition in the lookaheadState
            for (int j = 0; j < currLookahead.GetOutgoingTransitions().Count; j++)
            {

                Transition curr_t = curr.GetOutgoingTransitions()[i];
                Transition lookahead_t = currLookahead.GetOutgoingTransitions()[j];

                //if the Transition symbols don#t line up, ignore
                if (!curr_t.symbol.Equals(lookahead_t.symbol)) continue;

                //otherwise, we can create a new State from curr + lookahead_curr via a Transition over the symbol
                State comboState = new State(curr.id + currLookahead.id);

                //if our current State is marked (count > 0), we need to mark this new State with the same Queue as our curr State, activate pseudoMode
                //and add the new State to the marked list to resolve later
                bool isPseudoMode = curr.marker.Count > 0 ? true : false;
                if (isPseudoMode)
                {
                    comboState.marker = new Queue<string>(curr.marker);
                    marked.Enqueue(comboState);
                }

                //if both curr_next and lookahead_curr_next are endStates, and !pseudoMode, our new State is an endState
                if (curr_t.GetOutState().isEndState && lookahead_t.GetOutState().isEndState)
                    if (pseudoMode)
                        pseudoEndStates.Add(comboState.uuid);
                    else
                        comboState.SetEndState(true);

                //the same is true if curr is a pseudoEndState
                if (pseudoEndStates.Contains(curr_t.GetOutState().uuid) && lookahead_t.GetOutState().isEndState)
                    if (pseudoMode)
                        pseudoEndStates.Add(comboState.uuid);
                    else
                        comboState.SetEndState(true);

                //Now we can create a the new Transition
                //if prevComboState is null, we are at the beginning and need to create a direct connection to the curr State
                if (prevComboState == null)
                {
                    Transition comboTransition = new Transition(curr, curr_t.symbol, comboState);
                    comboTransition.Apply();
                }
                else
                {
                    Transition comboTransition = new Transition(prevComboState, curr_t.symbol, comboState);
                    comboTransition.Apply();
                }

                //we now need to advance to the next States
                CalculatePowerSet(comboState, curr_t.GetOutState(), lookahead_t.GetOutState(), visited, isPseudoMode, marked);
            }
        }

    }

    private bool ContainsUniversalTransition(State state, string? uuid = null)
    {
        for (int i = 0; i < state.GetOutgoingTransitions().Count; i++)
            if (uuid == null && state.GetOutgoingTransitions()[i].universal)
                return true;
            else if (uuid != null)
                if (state.GetOutgoingTransitions()[i].universal && state.GetOutgoingTransitions()[i].uuid != uuid)
                    return true;

        return false;
    }

}