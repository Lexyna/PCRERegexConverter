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

}