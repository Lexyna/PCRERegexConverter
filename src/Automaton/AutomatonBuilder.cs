public class AutomatonBuilder
{

    List<Token> stream;
    int index = 0;

    Automaton mainAutomaton;

    Automaton auto;

    bool isSubAutomaton = false;

    public AutomatonBuilder(List<Token> stream, Automaton auto)
    {
        this.stream = stream;
        this.mainAutomaton = auto;
        this.auto = auto;
    }

    public void build()
    {
        AppendToken();
        ApplySubEndStates();
    }

    private void ApplySubEndStates()
    {

        if (!isSubAutomaton) return;

        auto.acceptingStates.ForEach(s => mainAutomaton.AddAcceptingState(s));

    }

    public void AppendToken()
    {
        if (index >= stream.Count) return;

        switch (stream[index].tokenOP)
        {
            case Token.OP.Terminal: AppendTerminal(); break;
            case Token.OP.Group: AppendGroup(); break;
            case Token.OP.Alternate: CreateNewAutomaton(); break;
            default: break;
        }

        index++;

        if (index < stream.Count)
            AppendToken();
    }

    public void CreateNewAutomaton()
    {
        ApplySubEndStates();

        Automaton alternateAutomaton = new Automaton();

        auto.startStates.ForEach(s => alternateAutomaton.AddStartingState(s));

        alternateAutomaton.startStates.ForEach(s => s.SetEndState(true));
        alternateAutomaton.startStates.ForEach(s => alternateAutomaton.AddAcceptingState(s));

        this.auto = alternateAutomaton;
        isSubAutomaton = true;
    }

    public void AppendTerminal()
    {
        if (index + 1 >= stream.Count)
        {
            ConcatTerminal((TerminalToken)stream[index]);
            return;
        }

        switch (stream[index + 1].tokenOP)
        {
            case Token.OP.Optional: ConcatOptionalTerminal((TerminalToken)stream[index]); break;
            case Token.OP.Star: ConcatStarTerminal((TerminalToken)stream[index]); break;
            default: ConcatTerminal((TerminalToken)stream[index]); break;
        }

    }

    public void AppendGroup()
    {
        if (index + 1 >= stream.Count)
        {
            ConcatGroup((GroupToken)stream[index]);
            return;
        }

        switch (stream[index + 1].tokenOP)
        {
            case Token.OP.Optional: ConcatOptionalGroup((GroupToken)stream[index]); break;
            case Token.OP.Star: ConcatStarGroup((GroupToken)stream[index]); break;
            default: ConcatGroup((GroupToken)stream[index]); break;
        }

    }

    public void ConcatTerminal(TerminalToken token)
    {

        State nextState = new State("");

        for (int i = 0; i < auto.acceptingStates.Count; i++)
        {
            Transition t = new Transition(auto.acceptingStates[i], token.symbol, nextState);
            t.Apply();
        }

        for (int i = auto.acceptingStates.Count - 1; i >= 0; i--)
        {
            auto.acceptingStates[i].SetEndState(false);
            auto.RemoveAcceptingState(i);
        }

        auto.AddAcceptingState(nextState);
    }

    public void ConcatOptionalTerminal(TerminalToken token)
    {

        State nextState = new State("");

        for (int i = 0; i < auto.acceptingStates.Count; i++)
        {
            Transition t = new Transition(auto.acceptingStates[i], token.symbol, nextState);
            t.Apply();
        }

        auto.AddAcceptingState(nextState);

    }

    public void ConcatStarTerminal(TerminalToken token)
    {

        State nextState = new State("");

        for (int i = 0; i < auto.acceptingStates.Count; i++)
        {
            Transition t = new Transition(auto.acceptingStates[i], token.symbol, nextState);
            t.Apply();
        }

        Transition selfTransition = new Transition(nextState, token.symbol, nextState);
        selfTransition.Apply();

        auto.AddAcceptingState(nextState);

    }

    public void ConcatGroup(GroupToken token)
    {

        Automaton nextAutomaton = new Automaton(token.GetTokens());

        for (int i = 0; i < auto.acceptingStates.Count; i++)
            for (int j = 0; j < nextAutomaton.startStates.Count; j++)
            {
                Transition t = new Transition(auto.acceptingStates[i], "", nextAutomaton.startStates[j]);
                t.Apply();
            }

        for (int i = auto.acceptingStates.Count - 1; i >= 0; i--)
        {
            auto.acceptingStates[i].SetEndState(false);
            auto.RemoveAcceptingState(i);
        }

        nextAutomaton.acceptingStates.ForEach(s => auto.AddAcceptingState(s));
    }

    public void ConcatOptionalGroup(GroupToken token)
    {

        Automaton nextAutomaton = new Automaton(token.GetTokens());

        for (int i = 0; i < auto.acceptingStates.Count; i++)
            for (int j = 0; j < nextAutomaton.startStates.Count; j++)
            {
                Transition t = new Transition(auto.acceptingStates[i], "", nextAutomaton.startStates[j]);
                t.Apply();
            }

        nextAutomaton.acceptingStates.ForEach(s => auto.AddAcceptingState(s));
    }

    public void ConcatStarGroup(GroupToken token)
    {

        Automaton nextAutomaton = new Automaton(token.GetTokens());

        for (int i = 0; i < auto.acceptingStates.Count; i++)
            for (int j = 0; j < nextAutomaton.startStates.Count; j++)
            {
                Transition t = new Transition(auto.acceptingStates[i], "", nextAutomaton.startStates[j]);
                t.Apply();
            }

        for (int i = 0; i < nextAutomaton.acceptingStates.Count; i++)
            for (int j = 0; j < nextAutomaton.startStates.Count; j++)
            {
                Transition selfTransition = new Transition(nextAutomaton.acceptingStates[i], "", nextAutomaton.startStates[j]);
                selfTransition.Apply();
            }

        nextAutomaton.acceptingStates.ForEach(s => auto.AddAcceptingState(s));
    }

}