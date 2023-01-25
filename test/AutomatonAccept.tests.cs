using Xunit;

public class AutomatonAcceptTests
{

    [Fact]
    public void AutomatonA()
    {
        //This automaton should accept the word "a"
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
        //This automaton should accept the word "a"
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

}