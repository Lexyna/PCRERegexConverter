using Xunit;

public class StateNameTests
{

    [Fact]
    public void EpsilonAutomatonNames()
    {
        Automaton automaton = new Automaton();
        State state = new State("Test");
        state.SetEndState(true);

        automaton.AddStartingState(state);
        automaton.SetStateName();

        Assert.Equal("q0", automaton.startStates[0].id);
    }

    [Fact]
    public void TwoStateAutomaton()
    {
        Automaton automaton = new Automaton();
        State start = new State("Start");
        State end = new State("End");
        end.SetEndState(true);

        Transition con = new Transition(start, "", end);
        con.Apply();

        automaton.AddStartingState(start);
        automaton.AddAcceptingState(end);

        automaton.SetStateName();

        Assert.Equal("q0", automaton.startStates[0].id);
        Assert.Equal("q1", automaton.acceptingStates[0].id);
    }

    [Fact]
    public void ThreeStateForkAutomaton()
    {

        Automaton automaton = new Automaton();
        State start = new State("Start");
        automaton.AddStartingState(start);

        State p1 = new State("p1");
        State p2 = new State("p2");

        Transition leftFork = new Transition(start, "", p1);
        leftFork.Apply();

        Transition rightFork = new Transition(start, "", p2);
        rightFork.Apply();

        automaton.SetStateName();

        Assert.Equal("q0", start.id);
        Assert.Equal("q1", p1.id);
        Assert.Equal("q2", p2.id);

    }

    [Fact]
    public void FourStateForkAutomaton()
    {

        Automaton automaton = new Automaton();
        State start = new State("Start");
        automaton.AddStartingState(start);

        State a1 = new State("a1");
        State a2 = new State("a2");
        State b1 = new State("b1");

        Transition leftFork = new Transition(start, "", a1);
        leftFork.Apply();

        Transition aTransition = new Transition(a1, "", a2);
        aTransition.Apply();

        Transition rightFork = new Transition(start, "", b1);
        rightFork.Apply();

        automaton.SetStateName();

        Assert.Equal("q0", start.id);
        Assert.Equal("q1", a1.id);
        Assert.Equal("q2", a2.id);
        Assert.Equal("q3", b1.id);

    }

}