public class AFAToNFAConverter
{

    Automaton afa;

    public Automaton nfa { get; private set; }

    //If the power set for a lookahead automaton is created, but the Created power set itself contain another lookahead,
    //the endStates can not be assigned without resolving the other lookaheads. Pseudo Mode disables the setting of endStates.
    //private bool pseudoMode = false;

    //keeps track of all 'endStates' that can't be applied since another lookahead isn't resolved yet
    HashSet<string> pseudoEndStates = new HashSet<string>();

    //Dictionary of combinedState
    //string - original State uuid in the afa 
    //State - new State in the nfa 
    Dictionary<string, State> bridge = new Dictionary<string, State>();

    //Contains all Transition that got copied from Transitions that are linked to universal transitions
    List<Transition> copyLinks = new List<Transition>();

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

        HashSet<string> visited = new HashSet<string>();

        MapExistentialTransition(nfa_start_state, afa.startStates[0], standaloneNFA, marked, visited, false);

        bridge = new Dictionary<string, State>();
        InitPowerSet(standaloneNFA, marked);

        RemoveAllLinks();

        nfa.SetStateName();
    }

    private void MapExistentialTransition(State sterilizedNfaState, State baseAfaState, Dictionary<string, State> standaloneNFA, Queue<State> marked, HashSet<string> visited, bool pseudoMode)
    {
        if (visited.Contains(baseAfaState.uuid))
            return;

        visited.Add(baseAfaState.uuid);

        //Get all universal Transitions
        List<Transition> universalTransitions = new List<Transition>();

        baseAfaState.GetOutgoingTransitions().ForEach((Transition transition) =>
        {
            if (transition.universal)
                universalTransitions.Add(transition);
        });

        //Iterate over all transitions in the base State
        for (int i = 0; i < baseAfaState.GetOutgoingTransitions().Count; i++)
        {

            Transition transition = baseAfaState.GetOutgoingTransitions()[i];

            //we ignore all universal transitions for now
            if (transition.universal)
                continue;

            bool isPseudoMode = false;
            bool isLinked = false;

            //Otherwise, if this transition is contained in a link set of a universal transition in the baseAfaState, activate pseudoMode
            for (int j = 0; j < universalTransitions.Count; j++)
            {
                if (universalTransitions[j].universalLink.ContainsKey(transition.uuid))
                {
                    isPseudoMode = true;
                    isLinked = true;
                    //Create the standalone NFA (sub automaton) following the universal transition
                    if (!standaloneNFA.ContainsKey(universalTransitions[j].GetOutState().uuid))
                        standaloneNFA.Add(universalTransitions[j].GetOutState().uuid, universalTransitions[j].GetOutState());
                    //mark this state to be processed further
                    if (marked.Contains(sterilizedNfaState))
                        break;

                    marked.Enqueue(sterilizedNfaState);
                    sterilizedNfaState.marker.Enqueue(universalTransitions[j].GetOutState().uuid);
                    break;
                }
            }

            //we add a new self transition and jump to the next 
            if (transition.GetInState().uuid.Equals(transition.GetOutState().uuid))
            {
                Transition newSelfTransition = new Transition(sterilizedNfaState, transition.symbol, sterilizedNfaState);
                newSelfTransition.Apply();
                continue;
            }

            //if we're already in pseudo mode, we continue pseudo mode
            if (pseudoMode)
                isPseudoMode = true;

            if (bridge.ContainsKey(transition.GetOutState().uuid))
            {
                Transition newNfaTransition = new Transition(sterilizedNfaState, transition.symbol, bridge[transition.GetOutState().uuid]);
                newNfaTransition.Apply();

                if (isLinked)
                    copyLinks.Add(newNfaTransition);

            }
            else
            {
                //Create a new nfaState
                State newNfaState = new State("n" + baseAfaState.id, false);
                bridge.Add(transition.GetOutState().uuid, newNfaState);

                Transition newNfaTransition = new Transition(sterilizedNfaState, transition.symbol, newNfaState);
                newNfaTransition.Apply();

                if (isLinked)
                    copyLinks.Add(newNfaTransition);

                //If this state in the afa is an endState, the nfa State will be a pseudoEndState
                if (transition.GetOutState().isEndState && (isPseudoMode || pseudoMode))
                    pseudoEndStates.Add(newNfaState.uuid);
                else if (transition.GetOutState().isEndState)
                    newNfaState.SetEndState(true);

                MapExistentialTransition(newNfaState, transition.GetOutState(), standaloneNFA, marked, visited, isPseudoMode);
            }
        }

        //Create the 'sterilized' nfa with only existential transitions, ignoring any lookaheads and without any endStates
        /*for (int i = 0; i < afaState.GetOutgoingTransitions().Count; i++)
        {
            Transition t = afaState.GetOutgoingTransitions()[i];

            bool isPseudoMode = false;

            if (t.universal)
            {
                isPseudoMode = true;
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
            if (t.GetOutState().isEndState && (isPseudoMode || pseudoMode))
                pseudoEndStates.Add(new_nfa_state.uuid);
            else if (t.GetOutState().isEndState)
                new_nfa_state.SetEndState(true);

            //For each state we added, we repeat the process
            MapExistentialTransition(new_nfa_state, t.GetOutState(), standaloneNFA, marked, visited, isPseudoMode);
        }*/
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
                bridge = new Dictionary<string, State>();

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
            bool lookaheadAccepts = false;

            //and each Transition in the lookaheadState
            for (int j = 0; j < currLookahead.GetOutgoingTransitions().Count; j++)
            {

                Transition curr_t = curr.GetOutgoingTransitions()[i];
                Transition lookahead_t = currLookahead.GetOutgoingTransitions()[j];

                //if the Transition symbols don't line up, ignore
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

                //if the lookahead state accepts, we can add all transitions of the base nfa without creating the power set with the afa
                if (lookahead_t.GetOutState().isEndState || lookaheadAccepts)
                {
                    lookaheadAccepts = true;
                    //TODO: Implement function to add all te subgraph starting from curr
                    //additional note; we can have to add transitions back to any state x and combo state containing base x. However, we do not(!) nee to iterate over them (should be contained in the visited map)
                    HashSet<string> visitedBaseStates = new HashSet<string>();

                    AddAllBaseTransitions(comboState, curr_t.GetOutState(), visitedBaseStates, false);
                    break;
                }

                //we now need to advance to the next States
                if (!lookaheadAccepts)
                    CalculatePowerSet(comboState, curr_t.GetOutState(), lookahead_t.GetOutState(), visited, isPseudoMode, marked);
            }
        }

    }

    private void AddAllBaseTransitions(State startState, State rest, HashSet<string> visited, bool pseudoMode)
    {
        //Now we can attach each curr state onto out startState in a similar way to MapExistentialTransition.
        if (visited.Contains(startState.uuid))
            return;
        visited.Add(startState.uuid);

        //Iterate over all outgoing rest Transitions
        int transitionCount = rest.GetOutgoingTransitions().Count;
        for (int i = 0; i < transitionCount; i++)
        {
            Transition transition = rest.GetOutgoingTransitions()[i];

            //we apply all self transitions first
            if (transition.GetInState().uuid.Equals(transition.GetOutState().uuid))
            {
                Transition newSelfTransition = new Transition(startState, transition.symbol, startState);
                newSelfTransition.Apply();
                continue;
            }


            if (bridge.ContainsKey(transition.GetOutState().uuid))
            {
                Transition newNfaTransition = new Transition(startState, transition.symbol, bridge[transition.GetOutState().uuid]);
                newNfaTransition.Apply();
            }
            else
            {
                //Create a new comboState
                State comboState = new State("b" + startState.id + transition.GetOutState().id);
                bridge.Add(transition.GetOutState().uuid, comboState);

                bool isPseudoMode = pseudoMode ? pseudoMode : false;
                if (transition.GetInState().marker.Count > 0)
                    isPseudoMode = true;

                //If this is either an endState or  pseudoEnd state, the new States need to be an endState as well
                if ((transition.GetOutState().isEndState || pseudoEndStates.Contains(transition.GetOutState().uuid)) && !isPseudoMode)
                    comboState.SetEndState(true);
                else if ((transition.GetOutState().isEndState || pseudoEndStates.Contains(transition.GetOutState().uuid)) &&
                isPseudoMode) // if the state is still marked, we have to add it to our pseudoEndStates list
                    pseudoEndStates.Add(comboState.uuid);

                Transition baseTransition = new Transition(startState, transition.symbol, comboState);
                baseTransition.Apply();

                AddAllBaseTransitions(comboState, transition.GetOutState(), visited, isPseudoMode);
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

    //Using the list of all universal transitions, this function will remove every link in the final nfa
    private void RemoveAllLinks()
    {
        copyLinks.ForEach(transitions => transitions.Delete());
    }

}