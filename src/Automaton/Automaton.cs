public class Automaton
{

    public static Automaton buildAutomaton(List<Token> tokenStream)
    {




        return new Automaton();
    }

    public static void BuildTerminal(Automaton automaton, TerminalToken token)
    {

        if (automaton.acceptingStates.Count > 0)
        {
            State newAcceptingState = new State("TerminalState");
            newAcceptingState.SetEndState(true);
            for (int i = automaton.acceptingStates.Count - 1; i >= 0; i--)
            {
                Transition t = new Transition(automaton.acceptingStates[i], token.symbol, newAcceptingState);
                automaton.acceptingStates[i].AddOutgoingTransition(t);
                automaton.acceptingStates[i].SetEndState(false);
                automaton.RemoveAcceptingState(i);
                newAcceptingState.AddIngoingTransition(t);
            }
            return;
        }

        State start = new State("S");
        State newTransitionState = new State("TerminalState");
        newTransitionState.SetEndState(true);

        Transition transition = new Transition(start, token.symbol, newTransitionState);
        start.AddOutgoingTransition(transition);
        automaton.AddStartingState(start);

        newTransitionState.AddIngoingTransition(transition);
        automaton.AddAcceptingState(newTransitionState);
    }

    //Class properties

    public List<State> startStates { get; private set; }
    public List<State> acceptingStates { get; private set; }

    //Reference to the token sequence defining this Automaton
    private List<Token> tokenStream;

    //Child Automaton
    private List<Automaton> innerAutomaton;

    public Automaton()
    {
        startStates = new List<State>();
        acceptingStates = new List<State>();
        tokenStream = new List<Token>();
        innerAutomaton = new List<Automaton>();
    }

    public Automaton(List<Token> tokenStream)
    {
        startStates = new List<State>();
        acceptingStates = new List<State>();
        this.tokenStream = tokenStream;
        innerAutomaton = new List<Automaton>();
        ConvertTokenStream();
    }

    private void ConvertTokenStream()
    {

        if (tokenStream.Count == 0) return;

        for (int i = 0; i < tokenStream.Count; i++)
        {

            switch (tokenStream[i].tokenOP)
            {
                case Token.OP.Terminal: Automaton.BuildTerminal(this, (TerminalToken)tokenStream[i]); break;
                default: Console.WriteLine("Couldn't process  token" + tokenStream[i].tokenOP); break;
            }

        }

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

        startStates.ForEach(s => TraverseStates(s, ref index, ref visited));

    }

    private void TraverseStates(State state, ref int index, ref Dictionary<string, bool> visited)
    {

        if (visited.ContainsKey(state.id)) return;

        state.id = "q" + index;
        index++;

        visited.Add(state.id, true);

        for (int i = 0; i < state.GetOutgoingTransitions().Count; i++)
            TraverseStates(state.GetOutgoingTransitions()[i].GetOutState(), ref index, ref visited);

    }

}