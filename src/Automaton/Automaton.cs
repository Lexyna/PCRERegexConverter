public class Automaton
{

    private List<State> startStates;
    private List<State> acceptingStates;

    public Automaton()
    {
        startStates = new List<State>();
        acceptingStates = new List<State>();
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

}