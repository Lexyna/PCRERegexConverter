public class Automaton
{
    public List<State> startStates { get; private set; }
    public List<State> acceptingStates { get; private set; }

    //Reference to the token sequence defining this Automaton
    private List<Token> tokenStream;

    //stores all States for this NFA, for easy access
    public Dictionary<string, State> states = new Dictionary<string, State>();

    public Automaton()
    {
        startStates = new List<State>();
        acceptingStates = new List<State>();
        tokenStream = new List<Token>();
    }

    public Automaton(List<Token> tokenStream, bool startAutomat = false)
    {
        startStates = new List<State>();
        acceptingStates = new List<State>();
        this.tokenStream = tokenStream;

        State start = new State("");
        start.SetEndState(true);
        AddAcceptingState(start);
        AddStartingState(start);

        AutomatonBuilder builder = new AutomatonBuilder(tokenStream, this);
        builder.build();

        FindAllStates();
    }

    public void AddStartingState(State state)
    {
        startStates.Add(state);
    }

    public void AddAcceptingState(State state)
    {
        state.SetEndState(true);
        acceptingStates.Add(state);
    }

    public void RemoveAcceptingState(int index)
    {
        acceptingStates.RemoveAt(index);
    }

    /**
        Returns if this Automaton is optional. 
        Automatons are optional, if either any start State is an accepting end State, or
        if one can be reach over epsilon Transitions from any start State
*/
    public bool IsOptional()
    {
        for (int i = 0; i < startStates.Count; i++)
            if (startStates[i].isEndState) return true;

        Dictionary<string, bool> visitedList = new Dictionary<string, bool>();

        for (int i = 0; i < startStates.Count; i++)
        {
            bool isOptional = CheckEpsilonTransitions(startStates[i], ref visitedList);
            if (isOptional) return true;
        }

        return false;
    }

    private bool CheckEpsilonTransitions(State state, ref Dictionary<string, bool> visitedList)
    {
        if (visitedList.ContainsKey(state.id)) return false;
        visitedList.Add(state.id, false);

        if (state.isEndState) return true;

        List<Transition> transition = state.GetOutgoingTransitions();

        for (int i = 0; i < transition.Count; i++)
        {
            if (visitedList.ContainsKey(transition[i].GetOutState().id) ||
                !transition[i].symbol.Equals("")) continue;

            return CheckEpsilonTransitions(transition[i].GetOutState(), ref visitedList);

        }

        return false;
    }

    public void SetStateName()
    {

        Dictionary<string, bool> visited = new Dictionary<string, bool>();

        int index = 0;

        startStates.ForEach(s => TraverseStates(s, ref index, ref visited, false));

    }

    private void TraverseStates(State state, ref int index, ref Dictionary<string, bool> visited, bool lookahead)
    {

        if (visited.ContainsKey(state.id)) return;

        if (!lookahead)
            state.id = "q" + index;
        else
            state.id = "L" + index;
        index++;

        visited.Add(state.id, true);

        for (int i = 0; i < state.GetOutgoingTransitions().Count; i++)
            if (!state.GetOutgoingTransitions()[i].universal)
                TraverseStates(state.GetOutgoingTransitions()[i].GetOutState(), ref index, ref visited, lookahead);
            else
                TraverseStates(state.GetOutgoingTransitions()[i].GetOutState(), ref index, ref visited, true);
    }

    public int GetUniversalStateCount()
    {

        int count = 0;

        Dictionary<string, bool> visited = new Dictionary<string, bool>();

        for (int i = 0; i < startStates.Count; i++)
        {
            FindUniversalStates(startStates[i], visited, ref count);
        }

        return count;
    }

    private void FindUniversalStates(State state, Dictionary<string, bool> visited, ref int count)
    {
        visited.Add(state.id, true);

        if (state.isUniversal)
            count++;

        for (int i = 0; i < state.GetOutgoingTransitions().Count; i++)
        {
            if (!visited.ContainsKey(state.GetOutgoingTransitions()[i].GetOutState().id))
                FindUniversalStates(state.GetOutgoingTransitions()[i].GetOutState(), visited, ref count);
        }

    }

    public void FindAllStates()
    {
        for (int i = 0; i < startStates.Count; i++)
        {
            FindAllStatesFromState(startStates[i]);
        }
    }

    private void FindAllStatesFromState(State state)
    {
        if (states.ContainsKey(state.uuid)) return;

        states.Add(state.uuid, state);

        state.GetOutgoingTransitions().ForEach(t => FindAllStatesFromState(t.GetOutState()));
    }

    public bool AcceptsWord(string word)
    {
        char[] sequence = word.ToArray();

        //This list dict tracks all possible position, all state id's need to be unique
        Dictionary<string, State> activeStates = new Dictionary<string, State>();

        for (int i = 0; i < startStates.Count; i++)
            activeStates.Add(startStates[i].id, startStates[i]);

        ResolveEpsilonTransition(ref activeStates);

        AdvanceStates(ref activeStates, sequence);

        return ContainsEndState(ref activeStates);
    }

    private void AdvanceStates(ref Dictionary<string, State> activeStates, char[] sequence)
    {

        for (int i = 0; i < sequence.Length; i++)
        {
            CalculateNextStates(ref activeStates, sequence[i]);
        }

    }

    private void CalculateNextStates(ref Dictionary<string, State> activeStates, char c)
    {

        Dictionary<string, State> nextStates = new Dictionary<string, State>();

        foreach (KeyValuePair<string, State> entry in activeStates)
        {
            State state = entry.Value;

            List<Transition> transitions = state.GetOutgoingTransitions();

            transitions.ForEach(t =>
            {
                if ((t.symbol.Equals(c.ToString())) &&
                    !nextStates.ContainsKey(t.GetOutState().id))
                    nextStates.Add(t.GetOutState().id, t.GetOutState());
            });

        }

        ResolveEpsilonTransition(ref nextStates);

        activeStates = nextStates;

    }

    private void ResolveEpsilonTransition(ref Dictionary<string, State> activeStates)
    {

        Dictionary<string, State> nextStates = new Dictionary<string, State>();

        bool addedNewState = false;

        foreach (KeyValuePair<string, State> entry in activeStates)
        {
            State state = entry.Value;
            nextStates.Add(state.id, state);

            List<Transition> transitions = state.GetOutgoingTransitions();

            for (int j = 0; j < transitions.Count; j++)
            {
                if (transitions[j].symbol != "" ||
                     activeStates.ContainsKey(transitions[j].GetOutState().id))
                    continue;

                nextStates.Add(transitions[j].GetOutState().id, transitions[j].GetOutState());
                addedNewState = true;
            }
        }

        activeStates = nextStates;

        if (addedNewState)
            ResolveEpsilonTransition(ref activeStates);
    }

    private bool ContainsEndState(ref Dictionary<string, State> activeStates)
    {
        bool accepts = false;

        for (int i = 0; i < activeStates.Count; i++)
        {
            KeyValuePair<string, State> entry = activeStates.ElementAt(i);

            if (!entry.Value.isEndState) continue;

            accepts = true;
            break;
        }

        return accepts;
    }

    //Todo: Implement hashing method using id and ingoing/outgoingTransitions

}