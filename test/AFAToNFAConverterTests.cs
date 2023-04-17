using Xunit;

public class AFAToNFAConverterTests
{

    [Fact]
    public void ConvertToNFA()
    {
        string regex = "(?=a)a";

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

        AFAToNFAConverter converter = new AFAToNFAConverter(automaton);

    }

    [Fact]
    public void ConvertToNFAWithStar()
    {
        string regex = "(?=a)a*";

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

        AFAToNFAConverter converter = new AFAToNFAConverter(automaton);

        Assert.True(converter.nfa.AcceptsWord("a"));
        Assert.True(converter.nfa.AcceptsWord("aa"));
        Assert.True(converter.nfa.AcceptsWord("aaaa"));

    }

    [Fact]
    public void ConvertNFAWithBaseEndState()
    {
        //the first expression is obv. impossible to satisfy, the second should create a branching pah with
        // a simple accepting sub automaton
        string regex = "(?=a)c|b";

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

        AFAToNFAConverter converter = new AFAToNFAConverter(automaton);

        Assert.True(converter.nfa.AcceptsWord("b"));
    }

}