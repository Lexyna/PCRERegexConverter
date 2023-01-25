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
        start.AddOutgoingTransition(con);
        end.AddIngoingTransition(con);

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
        start.AddOutgoingTransition(leftFork);
        p1.AddIngoingTransition(leftFork);

        Transition rightFork = new Transition(start, "", p2);
        start.AddOutgoingTransition(rightFork);
        p2.AddIngoingTransition(rightFork);

        automaton.SetStateName();

        Assert.Equal("q0", start.id);
        Assert.Equal("q1", p1.id);
        Assert.Equal("q2", p2.id);

    }

}