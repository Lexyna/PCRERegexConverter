using Xunit;

public class AutomatonAcceptTests
{

    [Fact]
    public void AutomatonA()
    {
        Automaton auto = new Automaton();

        State start = new State("");
        State end = new State("");

        Transition aTransition = new Transition(start, "a", end);
        aTransition.Apply();

        auto.AddStartingState(start);
        auto.AddAcceptingState(end);

        auto.SetStateName();

        Assert.True(auto.AcceptsWord("a"));
    }

    [Fact]
    public void AutomatonAB()
    {
        Automaton auto = new Automaton();

        State start = new State("");
        State mid = new State("");
        State end = new State("");

        Transition aTransition = new Transition(start, "a", mid);
        aTransition.Apply();

        Transition bTransition = new Transition(mid, "b", end);
        bTransition.Apply();

        auto.AddStartingState(start);
        auto.AddAcceptingState(end);

        auto.SetStateName();

        Assert.True(auto.AcceptsWord("ab"));
        Assert.False(auto.AcceptsWord("a"));

    }

    [Fact]
    public void AutomatonAStar()
    {
        Automaton auto = new Automaton();

        State start = new State("");

        Transition aTransition = new Transition(start, "a", start);
        aTransition.Apply();

        auto.AddStartingState(start);
        auto.AddAcceptingState(start);

        auto.SetStateName();

        Assert.True(auto.AcceptsWord(""));
        Assert.True(auto.AcceptsWord("a"));
        Assert.True(auto.AcceptsWord("aa"));
        Assert.True(auto.AcceptsWord("aaa"));
        Assert.True(auto.AcceptsWord("aaaa"));

    }

    [Fact]
    public void AutomatonAStarB()
    {
        Automaton auto = new Automaton();

        State start = new State("");
        State end = new State("");

        Transition aTransition = new Transition(start, "a", start);
        aTransition.Apply();

        Transition bTransition = new Transition(start, "b", end);
        bTransition.Apply();

        auto.AddStartingState(start);
        auto.AddAcceptingState(end);

        auto.SetStateName();

        Assert.True(auto.AcceptsWord("b"));
        Assert.True(auto.AcceptsWord("ab"));
        Assert.True(auto.AcceptsWord("aab"));
        Assert.True(auto.AcceptsWord("aaab"));
        Assert.True(auto.AcceptsWord("aaaab"));

        Assert.False(auto.AcceptsWord("a"));

    }

    [Fact]
    public void AutomatonAStarBOptional()
    {
        Automaton auto = new Automaton();

        State start = new State("");
        State end = new State("");

        Transition aTransition = new Transition(start, "a", start);
        aTransition.Apply();

        Transition bTransition = new Transition(start, "b", end);
        bTransition.Apply();

        auto.AddStartingState(start);
        auto.AddAcceptingState(start);
        auto.AddAcceptingState(end);

        auto.SetStateName();

        Assert.True(auto.AcceptsWord("b"));
        Assert.True(auto.AcceptsWord("a"));
        Assert.True(auto.AcceptsWord("aa"));
        Assert.True(auto.AcceptsWord("aaa"));
        Assert.True(auto.AcceptsWord("aaaa"));

        Assert.True(auto.AcceptsWord("b"));
        Assert.True(auto.AcceptsWord("ab"));
        Assert.True(auto.AcceptsWord("aab"));
        Assert.True(auto.AcceptsWord("aaab"));
        Assert.True(auto.AcceptsWord("aaaab"));

    }

    [Fact]
    public void AutomatonAStarBC()
    {
        Automaton auto = new Automaton();

        State start = new State("");
        State mid = new State("");
        State end = new State("");

        Transition aTransition = new Transition(start, "a", start);
        aTransition.Apply();

        Transition bTransition = new Transition(start, "b", mid);
        bTransition.Apply();

        Transition cTransition = new Transition(mid, "c", end);
        cTransition.Apply();

        auto.AddStartingState(start);
        auto.AddAcceptingState(end);

        auto.SetStateName();

        Assert.False(auto.AcceptsWord("b"));
        Assert.False(auto.AcceptsWord("ab"));
        Assert.False(auto.AcceptsWord("aab"));
        Assert.False(auto.AcceptsWord("aaab"));
        Assert.False(auto.AcceptsWord("aaaab"));

        Assert.True(auto.AcceptsWord("bc"));
        Assert.True(auto.AcceptsWord("abc"));
        Assert.True(auto.AcceptsWord("aabc"));
        Assert.True(auto.AcceptsWord("aaabc"));
        Assert.True(auto.AcceptsWord("aaaabc"));

        Assert.False(auto.AcceptsWord("a"));

    }

    [Fact]
    public void AutomatonAStarBOptionalC()
    {
        Automaton auto = new Automaton();

        State start = new State("");
        State mid = new State("");
        State end = new State("");

        Transition aTransition = new Transition(start, "a", start);
        aTransition.Apply();

        Transition bTransition = new Transition(start, "b", mid);
        bTransition.Apply();

        Transition c1 = new Transition(mid, "c", end);
        c1.Apply();

        Transition c2 = new Transition(start, "c", end);
        c2.Apply();

        auto.AddStartingState(start);
        auto.AddAcceptingState(end);

        auto.SetStateName();

        Assert.True(auto.AcceptsWord("bc"));
        Assert.True(auto.AcceptsWord("ac"));
        Assert.True(auto.AcceptsWord("aac"));
        Assert.True(auto.AcceptsWord("aaac"));
        Assert.True(auto.AcceptsWord("aaaac"));

        Assert.True(auto.AcceptsWord("bc"));
        Assert.True(auto.AcceptsWord("abc"));
        Assert.True(auto.AcceptsWord("aabc"));
        Assert.True(auto.AcceptsWord("aaabc"));
        Assert.True(auto.AcceptsWord("aaaabc"));

    }

    [Fact]
    public void AutomatonAStarBOptionalC2()
    {
        //Similar to AutomatonAStarBOptionalC but instead of a direct Transition from 
        //Start to end over c, this automaton has an epsilon Transition from start to mid
        Automaton auto = new Automaton();

        State start = new State("");
        State mid = new State("");
        State end = new State("");

        Transition aTransition = new Transition(start, "a", start);
        aTransition.Apply();

        Transition bTransition = new Transition(start, "b", mid);
        bTransition.Apply();

        Transition c1 = new Transition(mid, "c", end);
        c1.Apply();

        Transition c2 = new Transition(start, "", mid);
        c2.Apply();

        auto.AddStartingState(start);
        auto.AddAcceptingState(end);

        auto.SetStateName();

        Assert.True(auto.AcceptsWord("bc"));
        Assert.True(auto.AcceptsWord("ac"));
        Assert.True(auto.AcceptsWord("aac"));
        Assert.True(auto.AcceptsWord("aaac"));
        Assert.True(auto.AcceptsWord("aaaac"));

        Assert.True(auto.AcceptsWord("bc"));
        Assert.True(auto.AcceptsWord("abc"));
        Assert.True(auto.AcceptsWord("aabc"));
        Assert.True(auto.AcceptsWord("aaabc"));
        Assert.True(auto.AcceptsWord("aaaabc"));

    }

    [Fact]
    public void AutomatonEpsilonA()
    {

        Automaton auto = new Automaton();

        State start = new State("");
        State mid = new State("");
        State end = new State("");

        Transition epsilonTransition = new Transition(start, "", mid);
        epsilonTransition.Apply();

        Transition aTransition = new Transition(mid, "a", end);
        aTransition.Apply();

        auto.AddStartingState(start);
        auto.AddAcceptingState(end);

        auto.SetStateName();

        Assert.True(auto.AcceptsWord("a"));

    }

    [Fact]
    public void AutomatonEpsilonEpsilonA()
    {

        Automaton auto = new Automaton();

        State start = new State("");
        State mid1 = new State("");
        State mid2 = new State("");
        State end = new State("");

        Transition epsilonTransition = new Transition(start, "", mid1);
        epsilonTransition.Apply();

        Transition epsilonTransition2 = new Transition(mid1, "", mid2);
        epsilonTransition2.Apply();

        Transition aTransition = new Transition(mid2, "a", end);
        aTransition.Apply();

        auto.AddStartingState(start);
        auto.AddAcceptingState(end);

        auto.SetStateName();

        Assert.True(auto.AcceptsWord("a"));

    }

    [Fact]
    public void AutomatonABBPlusCOptional()
    {
        //  regex: abb*c?

        Automaton auto = new Automaton();

        State q0 = new State("");
        State q1 = new State("");
        State q2 = new State("");
        State q3 = new State("");

        Transition aTransition = new Transition(q0, "a", q1);
        aTransition.Apply();

        Transition bTransition = new Transition(q1, "b", q2);
        bTransition.Apply();

        Transition bStartTransition = new Transition(q2, "b", q2);
        bStartTransition.Apply();

        Transition cTransition = new Transition(q2, "c", q3);
        cTransition.Apply();

        auto.AddStartingState(q0);
        auto.AddAcceptingState(q2);
        auto.AddAcceptingState(q3);

        auto.SetStateName();

        Assert.True(auto.AcceptsWord("ab"));
        Assert.True(auto.AcceptsWord("abc"));
        Assert.True(auto.AcceptsWord("abbc"));
        Assert.True(auto.AcceptsWord("abbbbc"));

        Assert.False(auto.AcceptsWord("ac"));
    }

    [Fact]
    public void AutomatonAStarBBPlusCOptional()
    {
        //  regex: a*bb*c?

        Automaton auto = new Automaton();

        State q0 = new State("");
        State q1 = new State("");
        State q2 = new State("");
        State q3 = new State("");

        Transition aTransition = new Transition(q0, "a", q1);
        aTransition.Apply();

        Transition epsilonTransition = new Transition(q0, "", q2);
        epsilonTransition.Apply();

        Transition aStarTransition = new Transition(q1, "a", q1);
        aStarTransition.Apply();

        Transition bTransition = new Transition(q1, "b", q2);
        bTransition.Apply();

        Transition bStartTransition = new Transition(q2, "b", q2);
        bStartTransition.Apply();

        Transition cTransition = new Transition(q2, "c", q3);
        cTransition.Apply();

        auto.AddStartingState(q0);
        auto.AddAcceptingState(q2);
        auto.AddAcceptingState(q3);

        auto.SetStateName();

        Assert.True(auto.AcceptsWord("b"));
        Assert.True(auto.AcceptsWord("aab"));
        Assert.True(auto.AcceptsWord("aaab"));
        Assert.True(auto.AcceptsWord("ab"));
        Assert.True(auto.AcceptsWord("abc"));
        Assert.True(auto.AcceptsWord("abbc"));
        Assert.True(auto.AcceptsWord("abbbbc"));

        Assert.False(auto.AcceptsWord("ac"));
    }

}