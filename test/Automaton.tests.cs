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

}