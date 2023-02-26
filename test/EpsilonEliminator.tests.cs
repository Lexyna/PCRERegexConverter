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

}