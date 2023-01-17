using System;
using System.Collections;
using System.Collections.Generic;

//TokenToAutomatonConverter
public class TTAC
{
    List<Token> tokens;
    State start;

    public TTAC(List<Token> tokens)
    {
        this.tokens = tokens;
        start = new State("Start");

        if (!ContainsLookahead())
        {
            Print();
            return;
        }

    }

    public void CreateAFA()
    {
        AddAlternativeTransition(start, this.tokens);
    }

    /*
        Create a subAutomaton that represents this groups automaton.
    */
    private State CreateTail(State baseState, List<Token> tokens)
    {

        State curr = baseState;

        for (int i = 0; i < tokens.Count; i++)
        {
            Token token = tokens[i];

            int la = 0;

            if (i + 1 < tokens.Count)
                la = Lookahead(tokens[i + 1]);

            switch (token.tokenOP)
            {
                case Token.OP.Terminal:
                    int x1 = AddTerminalState(ref curr, token, la);
                    i += x1;
                    break;
                case Token.OP.Group:
                    int x2 = AddGroupState(ref curr, token, la);
                    i += x2;
                    break;

                default:
                    throw new Exception("Unhandled token Exception"); // Error, shouldn't happen
            }
        }

        return curr;

    }

    private int AddGroupState(ref State curr, Token token, int la)
    {

        GroupToken groupToken = ((GroupToken)token);

        switch (la)
        {
            case 0:
                {
                    List<Token> groupTokenStream = groupToken.GetTokens();
                    if (!groupToken.isDNF())
                    {
                        curr = CreateTail(curr, groupTokenStream);
                        return 0;
                    }

                    State endState = AddAlternateTailGroup(curr, groupTokenStream);
                    curr = endState;

                    return 0;
                }
            case 1:
                {
                    List<Token> groupTokenStream = groupToken.GetTokens();
                    if (!groupToken.isDNF())
                    {
                        State temp = curr;
                        State groupEnd = CreateTail(curr, groupTokenStream);
                        Transition t1 = new Transition(temp, "", groupEnd);
                        temp.AddOutgoingTransition(t1);
                        groupEnd.AddIngoingTransition(t1);
                        curr = groupEnd;
                        return 0;
                    }

                    State endState = AddAlternateTailGroup(curr, groupTokenStream);
                    Transition t2 = new Transition(curr, "", endState);
                    curr.AddOutgoingTransition(t2);
                    endState.AddIngoingTransition(t2);
                    curr = endState;

                    return 1;
                }
            case 2:
                {
                    List<Token> groupTokenStream = groupToken.GetTokens();
                    if (!groupToken.isDNF())
                    {
                        State temp = curr;
                        State groupEnd = CreateTail(curr, groupTokenStream);
                        Transition t1 = new Transition(temp, "", groupEnd);
                        Transition t2 = new Transition(groupEnd, "", temp);
                        temp.AddOutgoingTransition(t1);
                        groupEnd.AddIngoingTransition(t1);
                        groupEnd.AddOutgoingTransition(t2);
                        temp.AddIngoingTransition(t2);
                        curr = groupEnd;
                        return 0;
                    }

                    State endState = AddAlternateTailGroup(curr, groupTokenStream);
                    Transition t3 = new Transition(curr, "", endState);
                    Transition t4 = new Transition(endState, "", curr);
                    curr.AddOutgoingTransition(t3);
                    endState.AddIngoingTransition(t3);
                    endState.AddOutgoingTransition(t4);
                    curr.AddIngoingTransition(t4);
                    curr = endState;

                    return 1;
                }
            default: { return 0; }
        }
    }

    private State AddAlternateTailGroup(State baseState, List<Token> tokens)
    {
        //tokens are always in dnf

        State endState = new State("e");

        for (int i = 0; i < tokens.Count; i++)
        {
            if (i % 2 != 0) continue;

            State newState = new State("q" + i);
            Transition t = new Transition(baseState, "", newState); //Epsilon transition
            baseState.AddOutgoingTransition(t);
            newState.AddIngoingTransition(t);

            GroupToken gt = ((GroupToken)tokens[i]);
            State curr = ProcessTailGroup(newState, gt);

            Transition endTransition = new Transition(curr, "", endState);
            curr.AddOutgoingTransition(endTransition);
            endState.AddIngoingTransition(endTransition);

        }

        return endState;
    }

    private State ProcessTailGroup(State baseState, GroupToken group)
    {
        State curr;

        if (group.isDNF())
        {
            curr = AddAlternateTailGroup(baseState, group.GetTokens());
            return curr;
        }

        curr = CreateTail(baseState, group.GetTokens());
        return curr;
    }

    private int AddTerminalState(ref State curr, Token token, int la)
    {

        switch (la)
        {
            case 0:
                {
                    State terminalState = new State("Ts");
                    Transition t = new Transition(curr, token.symbol, terminalState);
                    curr.AddOutgoingTransition(t);
                    terminalState.AddIngoingTransition(t);
                    curr = terminalState;
                    return 0;
                }
            case 1:
                {
                    State terminalState = new State("Ts");
                    Transition t1 = new Transition(curr, token.symbol, terminalState);
                    Transition t2 = new Transition(curr, "", terminalState);
                    curr.AddOutgoingTransition(t1);
                    curr.AddOutgoingTransition(t2);
                    terminalState.AddIngoingTransition(t1);
                    terminalState.AddIngoingTransition(t2);
                    curr = terminalState;
                    return 1;
                }
            case 2:
                {
                    State terminalState = new State("Ts");
                    Transition epsilon = new Transition(curr, "", terminalState);
                    Transition self = new Transition(terminalState, token.symbol, terminalState);
                    curr.AddOutgoingTransition(epsilon);
                    terminalState.AddOutgoingTransition(self);
                    terminalState.AddIngoingTransition(epsilon);
                    terminalState.AddIngoingTransition(self);
                    curr = terminalState;
                    return 1;
                }
            default: { return 0; }
        }

    }

    /*
        Returns value based on the next tokeN:
        nonAffecting: 0
        Optional: 1
        Star: 2
    */
    private int Lookahead(Token t)
    {
        switch (t.tokenOP)
        {
            case Token.OP.Optional: return 1;
            case Token.OP.Star: return 2;
            default: return 0;
        }
    }

    private void ProcessGroup(State baseState, GroupToken group)
    {
        //TODO: Differentiate between dnf in group and end group
        if (group.isDNF())
        {
            AddAlternativeTransition(baseState, group.GetTokens());
            return;
        }

        CreateTail(baseState, group.GetTokens());
    }

    private void AddAlternativeTransition(State baseState, List<Token> tokens)
    {
        // Tokens are in disjunct normal form
        // Group | Group | Group ...

        for (int i = 0; i < tokens.Count; i++)
        {
            if (i % 2 != 0) continue;

            State newState = new State("q" + i);
            Transition t = new Transition(baseState, "", newState); //Epsilon transition
            baseState.AddOutgoingTransition(t);
            newState.AddIngoingTransition(t);

            GroupToken gT = ((GroupToken)tokens[i]);
            ProcessGroup(newState, gT);

        }

    }

    private bool ContainsLookahead()
    {
        for (int i = 0; i < tokens.Count; i++)
            if (tokens[i].tokenOP == Token.OP.Lookahead ||
                tokens[i].tokenOP == Token.OP.NLookahead)
                return true;
        return false;
    }

    private void Print()
    {
        string regex = "";

        for (int i = 0; i < tokens.Count; i++)
            regex += tokens[i].ToString();

        Console.WriteLine(regex);

    }

}