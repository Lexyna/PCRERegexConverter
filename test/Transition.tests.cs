using Xunit;

public class TransitionTests
{

    [Fact]
    public void TransitionApply()
    {
        State s1 = new State("s1");
        State s2 = new State("s2");

        Transition t = new Transition(s1, "", s2);
        t.Apply();

        Assert.Single(s1.GetOutgoingTransitions());
        Assert.Single(s2.GetIngoingTransitions());

    }

    [Fact]
    public void TransitionDelete()
    {
        State s1 = new State("s1");
        State s2 = new State("s2");

        Transition t = new Transition(s1, "", s2);
        t.Apply();

        t.Delete();

        Assert.Empty(s1.GetOutgoingTransitions());
        Assert.Empty(s2.GetIngoingTransitions());

    }

}