using Xunit;

public class EpsilonEliminatorTests
{
    [Fact]
    public void RemoveEpsilonTransition()
    {

        State s1 = new State("s1");
        State s2 = new State("s2");
        State s3 = new State("s3");

        Transition epsilonTrans = new Transition(s1, "", s2);
        epsilonTrans.Apply();

        Transition endTrans = new Transition(s2, "a", s3);
        endTrans.Apply();

        EpsilonEliminator.RemoveEpsilonFromState(s1);

        Assert.Single(s1.GetOutgoingTransitions());
        Assert.Empty(s2.GetIngoingTransitions());
        Assert.Equal("a", s1.GetOutgoingTransitions()[0].symbol);
    }

    [Fact]
    public void RemoveSelfEpsilonTransition()
    {

        State s1 = new State("s1");

        Transition selfEpsilon = new Transition(s1, "", s1);
        selfEpsilon.Apply();

        EpsilonEliminator.RemoveEpsilonFromState(s1);

        Assert.Empty(s1.GetIngoingTransitions());
        Assert.Empty(s1.GetOutgoingTransitions());
    }

    [Fact]
    public void RemoveEpsilonTransitionAsEndSate()
    {

        State s1 = new State("s1");
        State s2 = new State("s2");
        State s3 = new State("s3");

        s2.SetEndState(true);

        Transition epsilonTrans = new Transition(s1, "", s2);
        epsilonTrans.Apply();

        Transition endTrans = new Transition(s2, "a", s3);
        endTrans.Apply();

        EpsilonEliminator.RemoveEpsilonFromState(s1);

        Assert.Single(s1.GetOutgoingTransitions());
        Assert.Empty(s2.GetIngoingTransitions());
        Assert.Equal("a", s1.GetOutgoingTransitions()[0].symbol);
        Assert.True(s1.isEndState);
    }

    [Fact]
    public void RemoveEpsilonTransitionAcceptsWord()
    {

        List<Token> tokenStream = new List<Token>();
        tokenStream.Add(new TerminalToken("a"));
        tokenStream.Add(new EpsilonToken());
        tokenStream.Add(new TerminalToken("b"));

        Automaton a = new Automaton(tokenStream);

        Assert.True(a.AcceptsWord("ab"));

        EpsilonEliminator.RemoveEpsilonFromState(a.startStates[0]);

        Assert.True(a.AcceptsWord("ab"));
    }

    [Fact]
    public void RemoveTwoPointEpsilonTransition()
    {

        State s1 = new State("s1");
        State s2 = new State("s2");

        Transition t1 = new Transition(s1, "", s2);
        t1.Apply();

        Transition t2 = new Transition(s2, "", s1);
        t2.Apply();

        EpsilonEliminator.RemoveEpsilonFromState(s1);

        Assert.Empty(s1.GetOutgoingTransitions());
    }

    [Fact]
    public void RemoveThreePointEpsilonTransition()
    {

        State s1 = new State("s1");
        State s2 = new State("s2");
        State s3 = new State("s3");

        Transition t1 = new Transition(s1, "", s2);
        t1.Apply();

        Transition t2 = new Transition(s2, "", s3);
        t2.Apply();

        Transition t3 = new Transition(s3, "", s1);
        t3.Apply();

        EpsilonEliminator.RemoveEpsilonFromState(s1);

        Assert.Empty(s1.GetOutgoingTransitions());
    }

    [Fact]
    public void RemoveThreePointTwoWayEpsilonTransition()
    {

        State s1 = new State("s1");
        State s2 = new State("s2");
        State s3 = new State("s3");

        Transition t1 = new Transition(s1, "", s2);
        t1.Apply();

        Transition t2 = new Transition(s2, "", s3);
        t2.Apply();

        Transition t3 = new Transition(s3, "", s2);
        t3.Apply();

        EpsilonEliminator.RemoveEpsilonFromState(s1);

        Assert.Empty(s1.GetOutgoingTransitions());
    }

    [Fact]
    public void RemoveEpsilonFromAStarAutomaton()
    {

        List<Token> tokenStream = new List<Token>();
        tokenStream.Add(new TerminalToken("a"));
        tokenStream.Add(new StarToken());

        Automaton a = new Automaton(tokenStream);
        a.SetStateName();

        EpsilonEliminator.RemoveEpsilonFromState(a.startStates[0]);

        Assert.True(a.AcceptsWord(""));
        Assert.True(a.AcceptsWord("a"));
        Assert.True(a.AcceptsWord("aaa"));

    }

    [Fact]
    public void NonEpsilonSelfTransition()
    {

        State s1 = new State("s1");

        Transition t = new Transition(s1, "a", s1);
        t.Apply();

        EpsilonEliminator.RemoveEpsilonFromState(s1);

        Assert.Single(s1.GetIngoingTransitions());
        Assert.Single(s1.GetOutgoingTransitions());
    }

    [Fact]
    public void NonEpsilonTransition()
    {

        State s1 = new State("s1");
        State s2 = new State("s2");

        Transition t1 = new Transition(s1, "a", s2);
        t1.Apply();

        Transition t2 = new Transition(s2, "b", s2);
        t2.Apply();

        EpsilonEliminator.RemoveEpsilonFromState(s1);

        Assert.Single(s1.GetOutgoingTransitions());
    }

    [Fact]
    public void EliminateEpsilonTransitionToEndState()
    {

        State s1 = new State("s1");
        State s2 = new State("s2");
        State s3 = new State("s3");

        s3.SetEndState(true);

        Transition t1 = new Transition(s1, "", s2);
        t1.Apply();

        Transition t2 = new Transition(s2, "", s3);
        t2.Apply();

        EpsilonEliminator.RemoveEpsilonFromState(s1);

        Assert.Empty(s1.GetOutgoingTransitions());
        Assert.True(s1.isEndState);
    }

    [Fact]
    public void EliminateEpsilonForRegex1()
    {
        //Regex a(ab*)*

        const string regex = "a(ab*)*";

        Lexer lexer = new Lexer(regex);
        lexer.Tokenize();

        lexer.GetTokens().ForEach(t =>
        {
            if (t.tokenOP != Token.OP.Class) return;
            ((ClassToken)t).ConvertToGroup();
        });

        ParserSimplifier parser = new ParserSimplifier(lexer.GetTokens());
        parser.Simplify();

        Automaton automaton = new Automaton(parser.GetTokens());
        automaton.SetStateName();

        Assert.True(automaton.AcceptsWord("a"));
        Assert.True(automaton.AcceptsWord("aab"));
        Assert.True(automaton.AcceptsWord("aabab"));
        Assert.True(automaton.AcceptsWord("aababab"));
        Assert.True(automaton.AcceptsWord("aaaaaaaa"));

        EpsilonEliminator.RemoveEpsilonFromState(automaton.startStates[0]);

        Assert.True(automaton.AcceptsWord("a"));
        Assert.True(automaton.AcceptsWord("aab"));
        Assert.True(automaton.AcceptsWord("aabab"));
        Assert.True(automaton.AcceptsWord("aababab"));
        Assert.True(automaton.AcceptsWord("aaaaaaaa"));

    }

    [Fact]
    public void EliminateEpsilonForRegex2()
    {
        //Regex a(ab*)+

        const string regex = "a(ab*)+";

        Lexer lexer = new Lexer(regex);
        lexer.Tokenize();

        lexer.GetTokens().ForEach(t =>
        {
            if (t.tokenOP != Token.OP.Class) return;
            ((ClassToken)t).ConvertToGroup();
        });

        ParserSimplifier parser = new ParserSimplifier(lexer.GetTokens());
        parser.Simplify();

        Automaton automaton = new Automaton(parser.GetTokens());
        automaton.SetStateName();

        Assert.True(automaton.AcceptsWord("aabbbbbb"));
        Assert.True(automaton.AcceptsWord("aababab"));
        Assert.True(automaton.AcceptsWord("aabaaabbbbbaaabb"));
        Assert.True(automaton.AcceptsWord("aab"));
        Assert.True(automaton.AcceptsWord("aabab"));
        Assert.True(automaton.AcceptsWord("aababab"));
        Assert.True(automaton.AcceptsWord("aaaaaaaa"));

        EpsilonEliminator.RemoveEpsilonFromState(automaton.startStates[0]);

        Assert.True(automaton.AcceptsWord("aabbbbbb"));
        Assert.True(automaton.AcceptsWord("aababab"));
        Assert.True(automaton.AcceptsWord("aabaaabbbbbaaabb"));
        Assert.True(automaton.AcceptsWord("aab"));
        Assert.True(automaton.AcceptsWord("aabab"));
        Assert.True(automaton.AcceptsWord("aababab"));
        Assert.True(automaton.AcceptsWord("aaaaaaaa"));

    }

    [Fact]
    public void EliminateEpsilonForRegex3()
    {
        //Regex a(ab*c)+

        const string regex = "a(ab*c)+";

        Lexer lexer = new Lexer(regex);
        lexer.Tokenize();

        lexer.GetTokens().ForEach(t =>
        {
            if (t.tokenOP != Token.OP.Class) return;
            ((ClassToken)t).ConvertToGroup();
        });

        ParserSimplifier parser = new ParserSimplifier(lexer.GetTokens());
        parser.Simplify();

        Automaton automaton = new Automaton(parser.GetTokens());
        automaton.SetStateName();

        Assert.True(automaton.AcceptsWord("aac"));
        Assert.True(automaton.AcceptsWord("aabc"));
        Assert.True(automaton.AcceptsWord("aabbbbbc"));
        Assert.True(automaton.AcceptsWord("aabcabc"));
        Assert.True(automaton.AcceptsWord("aabbcabbbbcac"));

        EpsilonEliminator.RemoveEpsilonFromState(automaton.startStates[0]);

        Assert.True(automaton.AcceptsWord("aac"));
        Assert.True(automaton.AcceptsWord("aabc"));
        Assert.True(automaton.AcceptsWord("aabbbbbc"));
        Assert.True(automaton.AcceptsWord("aabcabc"));
        Assert.True(automaton.AcceptsWord("aabbcabbbbcac"));

    }

    [Fact]
    public void EliminateEpsilonForRegex4()
    {
        //Regex a(a?b*c)+

        const string regex = "a(a?b*c)+";

        Lexer lexer = new Lexer(regex);
        lexer.Tokenize();

        lexer.GetTokens().ForEach(t =>
        {
            if (t.tokenOP != Token.OP.Class) return;
            ((ClassToken)t).ConvertToGroup();
        });

        ParserSimplifier parser = new ParserSimplifier(lexer.GetTokens());
        parser.Simplify();

        Automaton automaton = new Automaton(parser.GetTokens());
        automaton.SetStateName();

        Assert.True(automaton.AcceptsWord("aac"));
        Assert.True(automaton.AcceptsWord("aabc"));
        Assert.True(automaton.AcceptsWord("aabbbbbc"));
        Assert.True(automaton.AcceptsWord("aabcabc"));
        Assert.True(automaton.AcceptsWord("aabbcabbbbcac"));

        Assert.True(automaton.AcceptsWord("ac"));
        Assert.True(automaton.AcceptsWord("aabc"));
        Assert.True(automaton.AcceptsWord("aabcbc"));
        Assert.True(automaton.AcceptsWord("abcbcbcbbc"));

        EpsilonEliminator.RemoveEpsilonFromState(automaton.startStates[0]);

        Assert.True(automaton.AcceptsWord("aac"));
        Assert.True(automaton.AcceptsWord("aabc"));
        Assert.True(automaton.AcceptsWord("aabbbbbc"));
        Assert.True(automaton.AcceptsWord("aabcabc"));
        Assert.True(automaton.AcceptsWord("aabbcabbbbcac"));

        Assert.True(automaton.AcceptsWord("ac"));
        Assert.True(automaton.AcceptsWord("aabc"));
        Assert.True(automaton.AcceptsWord("aabcbc"));
        Assert.True(automaton.AcceptsWord("abcbcbcbbc"));

    }

}