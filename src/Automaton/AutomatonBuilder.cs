public class AutomatonBuilder
{

    List<Token> stream;
    private State entry { set; get; }

    //Reference to the entry of the last automaton 
    private State lastEntry;
    private State curr;

    public Automaton afa { get; private set; }

    public AutomatonBuilder(List<Token> stream, Automaton parentAutomaton = null)
    {
        this.stream = stream;
        afa = new Automaton();

        if (parentAutomaton != null)
            parentAutomaton.endStates.ForEach(s => afa.AddEndState(s));

        entry = new State(Guid.NewGuid().ToString("N"));
        entry.SetEndState(true);
        afa.AddEndState(entry);
        afa.AddStartState(entry);
        lastEntry = entry;
        curr = entry;
        BuildAutomaton();
    }

    private void BuildAutomaton()
    {

        for (int i = 0; i < stream.Count; i++)
        {
            processToken(i);
        }
    }

    private void processToken(int index)
    {
        switch (stream[index].tokenOP)
        {
            case Token.OP.Terminal: buildTerminal((TerminalToken)stream[index], index); break;
            case Token.OP.Star: BuildKleeneStar(); break;
            case Token.OP.Group: BuildGroup((GroupToken)stream[index], index); break;
            case Token.OP.Epsilon: BuildEpsilon(); break;
            case Token.OP.Alternate: break;
            default:
                Console.WriteLine("Couldn't process token:", stream[index].tokenOP.ToString(), " - ", stream[index].symbol);
                break;
        }
    }

    private void buildTerminal(TerminalToken token, int index)
    {

        State endState = new State(Guid.NewGuid().ToString("N"));
        endState.SetEndState(true);

        if (index + 1 == stream.Count || (stream[index + 1].tokenOP != Token.OP.Star))
            afa.ClearEndStates();
        //else
        //  afa.AddEndState(curr);

        Transition t = new Transition(this.curr, token.symbol, endState);

        curr.AddOutgoingTransition(t);
        endState.AddIngoingTransition(t);

        lastEntry = curr;
        curr = endState;
        afa.AddEndState(endState);
    }


    private void BuildKleeneStar()
    {

        Transition epsilonTransition = new Transition(curr, "", lastEntry);
        curr.AddOutgoingTransition(epsilonTransition);
        lastEntry.AddIngoingTransition(epsilonTransition);

        lastEntry.SetEndState(true);
        this.afa.AddEndState(lastEntry);
    }

    private void BuildGroup(GroupToken token, int index)
    {
        if (token.symbol == "()") return;

        Automaton groupAutomaton;

        if (index + 1 == stream.Count || stream[index + 1].tokenOP != Token.OP.Star)
        {
            groupAutomaton = token.CreateAutomaton(this.afa); //this.afa
            if (!groupAutomaton.IsOptional())
                this.afa.ClearEndStates();
        }
        else
            groupAutomaton = token.CreateAutomaton();

        State endState = new State(Guid.NewGuid().ToString("N"));
        endState.SetEndState(true);

        lastEntry = curr;

        groupAutomaton.startStates.ForEach(start =>
        {
            List<Transition> transitions = start.GetOutgoingTransitions();
            transitions.ForEach(t =>
            {
                Transition con = new Transition(curr, t.symbol, t.GetOutState());
                curr.AddOutgoingTransition(con);
                t.GetOutState().AddIngoingTransition(con);
            });

        });

        if (index + 1 < stream.Count)
            groupAutomaton.endStates.ForEach(end =>
            {
                Transition epsilonTransition = new Transition(end, "", endState);
                end.AddOutgoingTransition(epsilonTransition);
                endState.AddIngoingTransition(epsilonTransition);
            });

        groupAutomaton.endStates.ForEach(s => this.afa.AddEndState(s));
        this.afa.AddEndState(endState);

        curr = endState;
    }

    private void BuildEpsilon()
    {
        lastEntry.SetEndState(true);
        afa.AddEndState(lastEntry);
    }

}