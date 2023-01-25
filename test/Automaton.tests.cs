using Xunit;

public class AutomatonTests
{

    [Fact]
    public void EpsilonAutomatonIsOptional()
    {
        Automaton automaton = new Automaton();
        State state = new State("Test");
        state.SetEndState(true);

        automaton.AddStartingState(state);
        Assert.True(automaton.IsOptional());
    }

    [Fact]
    public void EpsilonTransitionIntoEndState()
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

        Assert.True(automaton.IsOptional());

    }

    [Fact]
    public void CircularReference()
    {
        Automaton automaton = new Automaton();
        State start = new State("Start");
        State end = new State("End");
        end.SetEndState(true);

        Transition con = new Transition(start, "", end);
        start.AddOutgoingTransition(con);
        end.AddIngoingTransition(con);

        Transition circularTransition = new Transition(end, "", start);
        end.AddOutgoingTransition(circularTransition);
        start.AddIngoingTransition(circularTransition);

        automaton.AddStartingState(start);
        automaton.AddAcceptingState(end);

        Assert.True(automaton.IsOptional());
    }

    [Fact]
    public void CircularReferenceOverTwoStates()
    {
        Automaton automaton = new Automaton();
        State start = new State("Start");
        State mid = new State("Mid");
        State end = new State("End");
        end.SetEndState(true);

        Transition con = new Transition(start, "", mid);
        start.AddOutgoingTransition(con);
        mid.AddIngoingTransition(con);

        Transition midTransition = new Transition(mid, "", end);
        mid.AddOutgoingTransition(midTransition);
        end.AddIngoingTransition(midTransition);

        Transition circularTransition = new Transition(end, "", start);
        end.AddOutgoingTransition(circularTransition);
        start.AddIngoingTransition(circularTransition);

        automaton.AddStartingState(start);
        automaton.AddAcceptingState(end);

        Assert.True(automaton.IsOptional());
    }

    [Fact]
    public void NoEndState()
    {
        Automaton automaton = new Automaton();
        State state = new State("Test");

        automaton.AddStartingState(state);
        Assert.False(automaton.IsOptional());

    }

    [Fact]
    public void NoEndStateWithTransition()
    {
        Automaton automaton = new Automaton();
        State start = new State("Start");
        State end = new State("End");

        Transition con = new Transition(start, "", end);
        start.AddOutgoingTransition(con);
        end.AddIngoingTransition(con);

        automaton.AddStartingState(start);
        automaton.AddAcceptingState(end);

        Assert.True(automaton.IsOptional());

    }

    [Fact]
    public void AlphaTransition()
    {

        Automaton automaton = new Automaton();
        State start = new State("Start");
        State end = new State("End");

        Transition con = new Transition(start, "a", end);
        start.AddOutgoingTransition(con);
        end.AddIngoingTransition(con);

        automaton.AddStartingState(start);
        automaton.AddAcceptingState(end);

        Assert.False(automaton.IsOptional());

    }

    [Fact]
    public void AlphaBetaTransition()
    {

        Automaton automaton = new Automaton();
        State start = new State("Start");
        State mid = new State("Mid");
        State end = new State("End");

        Transition startTransition = new Transition(start, "a", end);
        start.AddOutgoingTransition(startTransition);
        mid.AddIngoingTransition(startTransition);

        Transition endTransition = new Transition(mid, "b", end);
        mid.AddOutgoingTransition(endTransition);
        end.AddIngoingTransition(endTransition);

        automaton.AddStartingState(start);
        automaton.AddAcceptingState(end);

        Assert.False(automaton.IsOptional());

    }

    [Fact]
    public void AlphaBetaTransitionWithEpsilonToEndStateFromStart()
    {

        Automaton automaton = new Automaton();
        State start = new State("Start");
        State mid = new State("Mid");
        State end = new State("End");

        Transition startTransition = new Transition(start, "a", end);
        start.AddOutgoingTransition(startTransition);
        mid.AddIngoingTransition(startTransition);

        Transition endTransition = new Transition(mid, "b", end);
        mid.AddOutgoingTransition(endTransition);
        end.AddIngoingTransition(endTransition);

        Transition epsilonTransition = new Transition(start, "", end);
        start.AddOutgoingTransition(epsilonTransition);
        end.AddIngoingTransition(epsilonTransition);

        automaton.AddStartingState(start);
        automaton.AddAcceptingState(end);

        Assert.True(automaton.IsOptional());

    }

    [Fact]
    public void AlphaBetaTransitionWithEpsilonToEndStateFromMid()
    {

        Automaton automaton = new Automaton();
        State start = new State("Start");
        State mid = new State("Mid");
        State end = new State("End");

        Transition startTransition = new Transition(start, "a", end);
        start.AddOutgoingTransition(startTransition);
        mid.AddIngoingTransition(startTransition);

        Transition endTransition = new Transition(mid, "b", end);
        mid.AddOutgoingTransition(endTransition);
        end.AddIngoingTransition(endTransition);

        Transition epsilonTransition = new Transition(mid, "", end);
        mid.AddOutgoingTransition(epsilonTransition);
        end.AddIngoingTransition(epsilonTransition);

        automaton.AddStartingState(start);
        automaton.AddAcceptingState(end);

        Assert.False(automaton.IsOptional());

    }

}