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
        con.Apply();

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
        con.Apply();

        Transition circularTransition = new Transition(end, "", start);
        circularTransition.Apply();

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
        con.Apply();

        Transition midTransition = new Transition(mid, "", end);
        midTransition.Apply();

        Transition circularTransition = new Transition(end, "", start);
        circularTransition.Apply();

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
        con.Apply();

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
        con.Apply();

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
        startTransition.Apply();

        Transition endTransition = new Transition(mid, "b", end);
        endTransition.Apply();

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
        startTransition.Apply();

        Transition endTransition = new Transition(mid, "b", end);
        endTransition.Apply();

        Transition epsilonTransition = new Transition(start, "", end);
        epsilonTransition.Apply();

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
        startTransition.Apply();

        Transition endTransition = new Transition(mid, "b", end);
        endTransition.Apply();

        Transition epsilonTransition = new Transition(mid, "", end);
        epsilonTransition.Apply();

        automaton.AddStartingState(start);
        automaton.AddAcceptingState(end);

        Assert.False(automaton.IsOptional());

    }

}