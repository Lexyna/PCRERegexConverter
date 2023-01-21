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

        return false;
    }

}