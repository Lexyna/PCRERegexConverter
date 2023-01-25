using Xunit;

public class AutomatonBuilderTests
{

    [Fact]
    public void TerminalTransition()
    {

        List<Token> stream = new List<Token>();
        stream.Add(new TerminalToken("a"));

        Automaton auto = new Automaton(stream);

        Transition aTransition = auto.acceptingStates[0].GetIngoingTransitions()[0];

        Assert.Equal("a", aTransition.symbol);

    }

}