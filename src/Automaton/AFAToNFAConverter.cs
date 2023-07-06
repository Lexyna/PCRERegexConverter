public class AFAToNFAConverter
{

    Automaton afa;

    public Automaton nfa { get; private set; }

    HashSet<string> visited = new HashSet<string>();

    Dictionary<string, List<State>> afa_nfa_link = new Dictionary<string, List<State>>();

    //Contains the the uuid of all universalStates that are either in processing or have been processed already
    HashSet<string> processingStates = new HashSet<string>();

    //Remembers all add State of the NFA
    //Key ist the combined id's of the States sorted, eg. '[1]','[1,5,7]','[23,44,1,7]'
    Dictionary<string, State> nfaStateMemory = new Dictionary<string, State>();
    Dictionary<string, State> afaStateMemory = new Dictionary<string, State>();

    private int index = 0;

    public AFAToNFAConverter(Automaton afa)
    {
        this.afa = afa;
        this.nfa = new Automaton();
        Convert();
        string s = "";
        //this.nfa.SetStateName();
        EpsilonEliminator.RemoveEpsilonFromState(this.nfa.startStates[0]);
    }

    private void Convert()
    {
        State startNFA = new State("s");

        this.nfa.AddStartingState(startNFA);
        StepOneState(startNFA, afa.startStates[0]);
    }

    private void StepOneState(State curr, State afaState, bool allowRepetition = false, bool resolving = false)
    {
        if (visited.Contains(afaState.uuid) && !allowRepetition)
            return;


        if (!allowRepetition)
            visited.Add(afaState.uuid);
        //AutomatonVisualizer visualizer = new AutomatonVisualizer(afa.startStates[0]);

        //Apply link Transitions
        for (int i = 0; i < afaState.GetOutgoingTransitions().Count; i++)
        {
            if (afa_nfa_link.ContainsKey(afaState.GetOutgoingTransitions()[i].GetOutState().id))
                for (int j = 0; j < afa_nfa_link[afaState.GetOutgoingTransitions()[i].GetOutState().id].Count; j++)
                {
                    Transition linkTransition = new Transition(curr, afaState.GetOutgoingTransitions()[i].symbol, afa_nfa_link[afaState.GetOutgoingTransitions()[i].GetOutState().id][j]);
                    linkTransition.Apply();
                }

        }

        if (!afa_nfa_link.ContainsKey(afaState.id))
        {
            afa_nfa_link.Add(afaState.id, new List<State>());
            afa_nfa_link[afaState.id].Add(curr);
        }

        for (int i = 0; i < afaState.GetOutgoingTransitions().Count; i++)
        {

            Transition afaTransition = afaState.GetOutgoingTransitions()[i];

            if (!afaTransition.GetOutState().isUniversal)
            {
                //if (afa_nfa_link.ContainsKey(afaTransition.GetOutState().id))
                //  continue;

                //if (resolving && afaTransition.GetOutState().isUniversal)
                //   continue;

                State newNfaState = new State(index.ToString());
                if (afaTransition.GetOutState().isEndState)
                    newNfaState.SetEndState(true);

                AddStateToNFAMemory(newNfaState);
                index++;

                if (afaTransition.GetInState().uuid == afaTransition.GetOutState().uuid)
                {
                    Transition nfaSelfTransition = new Transition(curr, afaTransition.symbol, curr);
                    nfaSelfTransition.Apply();
                    continue;
                }

                Transition nfaTransition = new Transition(curr, afaTransition.symbol, newNfaState);
                nfaTransition.Apply();
                StepOneState(newNfaState, afaTransition.GetOutState());
                continue;
            }
            else
            {
                State afaUniversalState = afaTransition.GetOutState();

                //Resolve the ComboState Automaton
                State comboState = ResolveUniversalState(afaUniversalState);

                //AutomatonVisualizer visualizer = new AutomatonVisualizer(afa.startStates[0]);

                //DoStep One again
                StepOneState(curr, afaState, true);
            }

        }
    }

    private State ResolveUniversalState(State universalState)
    {
        if (!universalState.isUniversal || universalState.GetOutgoingTransitions().Count != 2)
            throw new Exception("State can't be processed correctly");

        if (!processingStates.Contains(universalState.uuid))
            processingStates.Add(universalState.uuid);

        //First powerState always after the same schemata
        List<State> laLinks = new List<State>();
        List<State> nfaLinks = new List<State>();

        universalState.GetOutgoingTransitions().ForEach(transition =>
        {
            if (transition.universal)
                laLinks.Add(transition.GetOutState());
            else
                nfaLinks.Add(transition.GetOutState());
        });

        State comboState = new State(nfaLinks, laLinks);

        if (comboState.laLinks[0].lookaheadState && comboState.nfaLinks[0].lookaheadState)
            comboState.SetLookaheadState(true);

        AddStateToAFAMemory(comboState);

        ProcessEpsilonHullComboState(comboState);

        universalState.SetUniversal(false);

        //Remove all outgoing transition form the universal State
        for (int j = universalState.GetOutgoingTransitions().Count - 1; j >= 0; j--)
            universalState.GetOutgoingTransitions()[j].Delete();

        //Add new transition to resolved nfa
        Transition resolveBridge = new Transition(universalState, "", comboState);
        resolveBridge.Apply();

        AddStateToAFAMemory(comboState);

        return comboState;
    }

    public void ProcessEpsilonHullComboState(State comboState, bool includeSelf = true)
    {
        if (!comboState.linkedState)
            return;

        HashSet<string> visited = new HashSet<string>();

        List<List<State>> epsilonHull = new List<List<State>>();

        comboState.laLinks.ForEach(link =>
        {
            epsilonHull.Add(GetEpsilonHull(link, visited));
        });

        List<List<State>> nfaEpsilonHull = new List<List<State>>();
        visited.Clear();

        comboState.nfaLinks.ForEach(link =>
        {
            epsilonHull.Add(GetEpsilonHull(link, visited));
        });

        // Create all new States based on the found states

        if (!includeSelf)
            for (int i = epsilonHull.Count - 1; i >= 0; i--)
                for (int j = epsilonHull[i].Count - 1; j >= 0; j--)
                    if (epsilonHull[i][j].uuid == comboState.laLinks[0].uuid)
                        epsilonHull[i].RemoveAt(j);

        List<List<State>> combinableStates = Utils<State>.CrossProduct(epsilonHull);

        List<State> generatedStates = new List<State>();

        //Create all the new reachable states
        for (int i = 0; i < combinableStates.Count; i++)
        {
            List<State> statesToCombine = combinableStates[i];

            List<State> laStates = new List<State>();
            List<State> nfaState = new List<State>();

            for (int j = 0; j < statesToCombine.Count; j++)
            {
                if (statesToCombine[j].lookaheadState)
                    laStates.Add(statesToCombine[j]);
                else
                    nfaState.Add(statesToCombine[j]);
            }

            State newComboState = new State(nfaState, laStates);
            if (comboState.lookaheadState)
                newComboState.SetLookaheadState(true);
            generatedStates.Add(newComboState);

            string stateKey = GetStateKey(newComboState);
            if (afaStateMemory.ContainsKey(stateKey))
                continue;
            Transition epsilonTransition = new Transition(comboState, "", newComboState);
            epsilonTransition.Apply();
            afaStateMemory.Add(stateKey, newComboState);
        }

        //At this point 'generatedStates' only contains States with a lookahead and tail state,
        //States here won't contain any more universal States 
        //Create Intersection
        for (int i = 0; i < generatedStates.Count; i++)
            CreateIntersection(generatedStates[i]);
    }

    private void CreateIntersection(State comboState)
    {
        if (!comboState.linkedState)
            throw new Exception("invalid Format, State must be combined State of Lookahead and tail");

        State nfaState;
        if (!comboState.lookaheadState)
            nfaState = comboState.nfaLinks[0];
        else
            nfaState = comboState.laLinks[0];

        //we only want to process the Combo State, if we have to. Is the lookahead state and endState or already gone, we can just process to add States normally
        bool createIntersection = true;

        if (comboState.laLinks.Count == 0)
            createIntersection = false;
        else if (comboState.laLinks[0].isEndState)
            createIntersection = false;

        //States here will append to themselves, if possible
        if (createIntersection)
        {
            //Create a new State for each reachable state via transition symbols
            State laState;
            if (!comboState.lookaheadState)
                laState = comboState.laLinks[0];
            else if (comboState.laLinks.Count > 1)
                laState = comboState.laLinks[1];
            else
                return;

            //We need to resolve the epsilon hull from laState
            ProcessEpsilonHullComboState(comboState, false);

            for (int i = 0; i < laState.GetOutgoingTransitions().Count; i++)
            {
                for (int j = 0; j < nfaState.GetOutgoingTransitions().Count; j++)
                {
                    Transition laTransition = laState.GetOutgoingTransitions()[i];
                    Transition tailTransition = nfaState.GetOutgoingTransitions()[j];

                    if (laTransition.symbol.Equals(tailTransition.symbol))
                    {

                        List<State> laList = new List<State>();
                        List<State> tailList = new List<State>();

                        laList.Add(laTransition.GetOutState());
                        tailList.Add(tailTransition.GetOutState());

                        State newComboState = new State(tailList, laList);
                        if (comboState.lookaheadState)
                            newComboState.SetLookaheadState(true);

                        if (laTransition.GetOutState().isEndState && tailTransition.GetOutState().isEndState)
                            newComboState.SetEndState(true);

                        if (newComboState.lookaheadState && newComboState.laLinks.Count == 0)
                        {
                            //Apply transition to new comboState with resolved lookahead
                            Transition lookaheadBridge = new Transition(comboState, tailTransition.symbol, newComboState);
                            lookaheadBridge.Apply();

                            //Combo state will contain the rest of the remaining lookahead
                            Transition lookaheadTail = new Transition(newComboState, "", newComboState.nfaLinks[0]);
                            lookaheadTail.Apply();

                            AddStateToAFAMemory(newComboState);
                            continue;
                        }

                        string key = GetStateKey(newComboState);
                        if (afaStateMemory.ContainsKey(key))
                        {
                            Transition newTransitionToExistingState = new Transition(comboState, tailTransition.symbol, afaStateMemory[key]);
                            newTransitionToExistingState.Apply();
                            continue;
                        }

                        Transition newComboTransition = new Transition(comboState, tailTransition.symbol, newComboState);
                        newComboTransition.Apply();
                        AddStateToAFAMemory(newComboState);
                        CreateIntersection(newComboState);
                    }
                }
            }
            return;
        }

        //The Lookahead State is either a EndState or doesn't exists. In this case we can process as normal
        Transition bridge = new Transition(comboState, "", comboState.nfaLinks[0]);
        bridge.Apply();
        //StepOneState(comboState, comboState.nfaLinks[0], false, true);

    }

    private List<State> GetEpsilonHull(State state, HashSet<string> visited)
    {

        if (visited.Contains(state.uuid))
            return new List<State>();
        visited.Add(state.uuid);

        if (state.isUniversal)
        {
            if (!processingStates.Contains(state.uuid))
            {
                State comboState = ResolveUniversalState(state);
            }
            else
            {
                return new List<State>();
            }
        }

        List<State> reachable = new List<State>();

        reachable.Add(state);

        state.GetOutgoingTransitions().ForEach(transition =>
        {
            if (transition.symbol == "")
            {
                reachable.AddRange(GetEpsilonHull(transition.GetOutState(), visited));
            }
        });
        return reachable;
    }

    private void AddStateToNFAMemory(State state)
    {
        string key = GetStateKey(state);
        if (!nfaStateMemory.ContainsKey(key))
            nfaStateMemory.Add(key, state);
    }

    private void AddStateToAFAMemory(State state)
    {
        string key = GetStateKey(state);
        if (!afaStateMemory.ContainsKey(key))
            afaStateMemory.Add(key, state);
    }

    private string GetStateKey(State state)
    {
        if (!state.linkedState)
            return $"[{state.id}]";

        return GetIdentifier(state.laLinks, state.nfaLinks);
    }

    public string GetIdentifier(List<State> lookaheads, List<State> nfas)
    {

        List<State> stateList = new List<State>(lookaheads);
        stateList.AddRange(nfas);

        List<State> sortedList = stateList.OrderBy(s => s.id).ToList();

        string key = "";

        for (int i = 0; i < sortedList.Count; i++)
        {
            key += sortedList[i].id;
            if (i < sortedList.Count - 1)
                key += ",";
        }

        return $"[{key}]";
    }

}