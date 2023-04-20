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

    [Fact]
    public void ConvertNFAWithShortLookahead()
    {

        string regex = "(?=a)ab";

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

        Assert.False(converter.nfa.AcceptsWord("a"));
        Assert.True(converter.nfa.AcceptsWord("ab"));
    }

    [Fact]
    public void ConvertNFAWithShortLookahead2()
    {

        string regex = "(?=a)abc";

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

        Assert.False(converter.nfa.AcceptsWord("a"));
        Assert.False(converter.nfa.AcceptsWord("ab"));
        Assert.True(converter.nfa.AcceptsWord("abc"));
    }

    [Fact]
    public void ConvertNFAWithShortLookaheadAndStar()
    {

        string regex = "(?=a)a*b";

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

        Assert.False(converter.nfa.AcceptsWord("a"));
        Assert.True(converter.nfa.AcceptsWord("ab"));
        Assert.True(converter.nfa.AcceptsWord("aab"));
    }

    [Fact]
    public void ConvertNFAWithTwoLookaheads()
    {

        string regex = "(?=a)a*(?=d)e";

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

        Assert.False(converter.nfa.AcceptsWord("a"));
        Assert.False(converter.nfa.AcceptsWord("ae"));
        Assert.False(converter.nfa.AcceptsWord("aae"));
    }
}