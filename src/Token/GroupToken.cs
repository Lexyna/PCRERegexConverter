using System;
using System.Collections;
using System.Collections.Generic;
public class GroupToken : Token
{

    public GroupToken(string symbol) : base(symbol, OP.Group)
    {
        TokenizeGroup();
    }

    private void TokenizeGroup()
    {
        if (symbol.Length == 0)
            return;

        string regex = symbol.Substring(1, symbol.Length - 2);

        Lexer lexer = new Lexer(regex);
        lexer.Tokenize();

        this.internalTokens = lexer.GetTokens();

        Simplify();
    }

    private void Simplify()
    {
        do
        {
            ParserSimplifier parser = new ParserSimplifier(internalTokens);
            internalTokens = parser.Simplify();
        } while (!isDNF());
    }

    public bool isDNF()
    {
        if (internalTokens.Count == 0)
            return true;

        bool isDNF = true;

        for (int i = 0; i < internalTokens.Count; i++)
        {
            if ((internalTokens[i].tokenOP != OP.Group) &&
                internalTokens[i].tokenOP != OP.Alternate)
            {
                return false;
            }

            if (i % 2 == 0 && internalTokens[i].tokenOP != OP.Group)
                return false;

            if (i % 2 == 1 && internalTokens[i].tokenOP != OP.Alternate)
                return false;

        }

        return isDNF;
    }

    public void AddToken(Token token)
    {
        this.symbol += token.symbol;
        this.internalTokens.Add(token);
    }

    public void AddTokenStream(List<Token> tokens)
    {
        for (int i = 0; i < tokens.Count; i++)
        {
            this.symbol += tokens[i].symbol;
            this.internalTokens.Add(tokens[i]);
        }
    }

    public List<Token> GetTokens() { return internalTokens; }

    public override string ToString()
    {
        if (internalTokens.Count == 1)
            return internalTokens[0].ToString();

        string regex = "(";

        for (int i = 0; i < internalTokens.Count; i++)
        {
            regex += internalTokens[i].ToString();
        }

        regex += ")";
        return regex;
    }

    public Automaton CreateAutomaton(Automaton parentAutomaton = null)
    {

        Automaton automaton = new Automaton();

        State entry = new State(Guid.NewGuid().ToString("N"));

        automaton.AddStartState(entry);

        if (parentAutomaton != null)
            parentAutomaton.endStates.ForEach(s => automaton.AddEndState(s));


        List<State> start = new List<State>();
        start.Add(entry);

        ConvertToAutomaton(start, automaton);

        return automaton;
    }

    public void ConvertToAutomaton(List<State> entry, Automaton automaton)
    {

        //Groups can be in two forks: With Alternation & without alternation
        //Groups with alternation are always in dnf: Group|Group|Group...

        //Convert Groups with alternation:

        if (isDNF() && this.internalTokens.Count > 1)
        {
            ConvertGroupType1(entry, automaton);
            return;
        }

        //Convert Groups without alternation:
        ConvertGroupType2(entry, automaton);

    }


    //Groups with alternation in dnf form
    private void ConvertGroupType1(List<State> entry, Automaton automaton)
    {

        for (int i = 0; i < internalTokens.Count; i++)
        {
            if (i % 2 != 0) continue;

            Automaton innerAutomaton = ((GroupToken)internalTokens[i]).CreateAutomaton();

            entry.ForEach(entry =>
            {
                innerAutomaton.startStates.ForEach(innerEntry =>
                {
                    //Transition epsilonTransition = new Transition(entry, "", innerEntry);
                    //entry.AddOutgoingTransition(epsilonTransition);
                    //innerEntry.AddIngoingTransition(epsilonTransition);

                    List<Transition> transitions = innerEntry.GetOutgoingTransitions();
                    transitions.ForEach(t =>
                    {

                        Transition con = new Transition(entry, t.symbol, t.GetOutState());
                        entry.AddOutgoingTransition(con);
                        t.GetOutState().AddIngoingTransition(con);

                        if (innerEntry.isEndState)
                        {
                            entry.SetEndState(true);
                            innerAutomaton.AddEndState(entry);
                        }

                    });

                });
            });

            //automaton.ResetEndStates();
            innerAutomaton.endStates.ForEach(s => automaton.AddEndState(s));

        }

    }

    //Groups without alternation
    private void ConvertGroupType2(List<State> entry, Automaton automaton)
    {

        AutomatonBuilder builder = new AutomatonBuilder(internalTokens, automaton);
        Automaton innerAutomaton = builder.afa;

        entry.ForEach(parentStart =>
        {
            innerAutomaton.startStates.ForEach(start =>
            {

                //Transition epsilonTransition = new Transition(parentStart, "", start);
                //parentStart.AddOutgoingTransition(epsilonTransition);
                //start.AddIngoingTransition(epsilonTransition);

                List<Transition> transitions = start.GetOutgoingTransitions();
                transitions.ForEach(t =>
                {
                    Transition con = new Transition(parentStart, t.symbol, t.GetOutState());
                    parentStart.AddOutgoingTransition(con);
                    t.GetOutState().AddIngoingTransition(con);

                    if (start.isEndState)
                    {
                        parentStart.SetEndState(true);
                        innerAutomaton.AddEndState(parentStart);
                    }

                });

            });
        });

        automaton.ResetEndStates();
        innerAutomaton.endStates.ForEach(s => automaton.AddEndState(s));

    }

}