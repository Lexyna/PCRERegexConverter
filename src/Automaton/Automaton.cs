public class Automaton
{

    public List<State> startStates { get; private set; }

    public List<State> endStates { get; private set; }

    public Automaton()
    {
        startStates = new List<State>();
        endStates = new List<State>();
    }

    public void SimplifyName()
    {

        int index = 0;
        startStates[0].SimplifyName(ref index);

    }

    public void ClearEndStates()
    {
        endStates.ForEach(s => s.SetEndState(false));
        endStates.Clear();
    }

    public void ResetEndStates()
    {
        endStates.Clear();
    }

    public void AddStartState(State s) { startStates.Add(s); }

    public void AddEndState(State s)
    {
        if (!s.isEndState) return;
        s.SetEndState(true);
        endStates.Add(s);
    }

    public bool IsOptional()
    {
        for (int i = 0; i < startStates.Count; i++)
            if (startStates[i].isEndState) return true;

        Dictionary<string, bool> visited = new Dictionary<string, bool>();

        for (int i = 0; i < startStates.Count; i++)
        {
            if (TraverseStates(startStates[i], ref visited)) return true;
        }

        return false;
    }

    private bool TraverseStates(State state, ref Dictionary<string, bool> visited)
    {
        if (visited.ContainsKey(state.id)) return false;

        List<Transition> transitions = state.GetOutgoingTransitions();
        for (int j = 0; j < transitions.Count; j++)
        {
            if (transitions[j].symbol != "") continue;

            if (transitions[j].GetOutState().isEndState) return true;

            if (visited.ContainsKey(transitions[j].GetOutState().id)) continue;

            return TraverseStates(transitions[j].GetOutState(), ref visited);

        }

        visited.Add(state.id, false);
        return false;
    }

}