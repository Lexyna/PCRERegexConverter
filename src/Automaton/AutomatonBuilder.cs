public class AutomatonBuilder
{

    List<Token> stream;
    int index = 0;

    Automaton main;

    Automaton? last;

    public AutomatonBuilder(List<Token> stream, Automaton main)
    {
        this.stream = stream;
        this.main = main;
    }

    public void build()
    {
        AppendToken(main);
    }

    public void AppendToken(Automaton auto)
    {
        switch (stream[index].tokenOP)
        {
            case Token.OP.Terminal: AppendTerminal(auto); break;
            case Token.OP.Group: AppendGroup(auto); break;
            case Token.OP.Alternate: break;
            default: break;
        }

        index++;

        if (index < stream.Count)
            AppendToken(auto);
    }

    public void AppendTerminal(Automaton auto)
    {
        if (index + 1 >= stream.Count)
        {
            ConcatTerminal(auto, (TerminalToken)stream[index]);
            return;
        }

        switch (stream[index + 1].tokenOP)
        {
            case Token.OP.Optional: ConcatOptionalTerminal(auto, (TerminalToken)stream[index]); break;
            case Token.OP.Star: ConcatStarTerminal(auto, (TerminalToken)stream[index]); break;
            default: ConcatTerminal(auto, (TerminalToken)stream[index]); break;
        }

    }

    public void AppendGroup(Automaton auto)
    {
        if (index + 1 >= stream.Count)
        {
            ConcatGroup(auto, (GroupToken)stream[index]);
            return;
        }

        switch (stream[index + 1].tokenOP)
        {
            case Token.OP.Optional: ConcatOptionalGroup(auto, (GroupToken)stream[index]); break;
            case Token.OP.Star: ConcatStarGroup(auto, (GroupToken)stream[index]); break;
            default: ConcatGroup(auto, (GroupToken)stream[index]); break;
        }

    }

    public void ConcatTerminal(Automaton auto, TerminalToken token)
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

    public void ConcatOptionalTerminal(Automaton auto, TerminalToken token)
    {

        State nextState = new State("");

        for (int i = 0; i < auto.acceptingStates.Count; i++)
        {
            Transition t = new Transition(auto.acceptingStates[i], token.symbol, nextState);
            t.Apply();
        }

        auto.AddAcceptingState(nextState);

    }

    public void ConcatStarTerminal(Automaton auto, TerminalToken token)
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

    public void ConcatGroup(Automaton auto, GroupToken token)
    {

        Automaton nextAutomaton = new Automaton(token.GetTokens());

        for (int i = 0; i < auto.acceptingStates.Count; i++)
            for (int j = 0; j < nextAutomaton.startStates.Count; j++)
            {
                Transition t = new Transition(auto.acceptingStates[i], "", nextAutomaton.startStates[i]);
                t.Apply();
            }

        for (int i = auto.acceptingStates.Count - 1; i >= 0; i--)
        {
            auto.acceptingStates[i].SetEndState(false);
            auto.RemoveAcceptingState(i);
        }

        nextAutomaton.acceptingStates.ForEach(s => auto.AddAcceptingState(s));
    }

    public void ConcatOptionalGroup(Automaton auto, GroupToken token)
    {

        Automaton nextAutomaton = new Automaton(token.GetTokens());

        for (int i = 0; i < auto.acceptingStates.Count; i++)
            for (int j = 0; j < nextAutomaton.startStates.Count; j++)
            {
                Transition t = new Transition(auto.acceptingStates[i], "", nextAutomaton.startStates[i]);
                t.Apply();
            }

        nextAutomaton.acceptingStates.ForEach(s => auto.AddAcceptingState(s));
        last = nextAutomaton;
    }

    public void ConcatStarGroup(Automaton auto, GroupToken token)
    {

        Automaton nextAutomaton = new Automaton(token.GetTokens());

        for (int i = 0; i < auto.acceptingStates.Count; i++)
            for (int j = 0; j < nextAutomaton.startStates.Count; j++)
            {
                Transition t = new Transition(auto.acceptingStates[i], "", nextAutomaton.startStates[i]);
                t.Apply();
            }

        for (int i = 0; i < nextAutomaton.acceptingStates.Count; i++)
            for (int j = 0; j < nextAutomaton.startStates.Count; j++)
            {
                Transition selfTransition = new Transition(nextAutomaton.acceptingStates[i], "", nextAutomaton.startStates[j]);
                selfTransition.Apply();
            }

        nextAutomaton.acceptingStates.ForEach(s => auto.AddAcceptingState(s));
        last = nextAutomaton;
    }

}