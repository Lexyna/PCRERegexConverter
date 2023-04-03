using Xunit;

public class AFATests
{

    [Fact]
    public void IsAFA()
    {

        string regex = "a(?=a)";

        Lexer lexer = new Lexer(regex);
        lexer.Tokenize();
        lexer.GetTokens().ForEach(t =>
        {
            if (t.tokenOP != Token.OP.Class) return;
            ((ClassToken)t).ConvertToGroup();
        });

        ParserSimplifier parser = new ParserSimplifier(lexer.GetTokens());
        parser.Simplify();

        Automaton automaton = new Automaton(parser.GetTokens());

        automaton.SetStateName();
        Assert.True(automaton.IsAFA());
    }

    [Fact]
    public void LookaheadAsFirstToken()
    {

        string regex = "(?=b)a";

        Lexer lexer = new Lexer(regex);
        lexer.Tokenize();
        lexer.GetTokens().ForEach(t =>
        {
            if (t.tokenOP != Token.OP.Class) return;
            ((ClassToken)t).ConvertToGroup();
        });

        ParserSimplifier parser = new ParserSimplifier(lexer.GetTokens());
        parser.Simplify();

        Automaton automaton = new Automaton(parser.GetTokens());
        automaton.SetStateName();

        EpsilonEliminator.RemoveEpsilonFromState(automaton.startStates[0]);
        Assert.True(automaton.IsAFA());

    }

}