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

        Automaton automaton = new Automaton(parser.GetTokens(), true);
        automaton.SetStateName();

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

        Automaton automaton = new Automaton(parser.GetTokens(), true);
        automaton.SetStateName();

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

        Automaton automaton = new Automaton(parser.GetTokens(), true);
        automaton.SetStateName();

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

        Automaton automaton = new Automaton(parser.GetTokens(), true);
        automaton.SetStateName();

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

        Automaton automaton = new Automaton(parser.GetTokens(), true);
        automaton.SetStateName();

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

        Automaton automaton = new Automaton(parser.GetTokens(), true);
        automaton.SetStateName();

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

        Automaton automaton = new Automaton(parser.GetTokens(), true);
        automaton.SetStateName();

        AFAToNFAConverter converter = new AFAToNFAConverter(automaton);

        Assert.False(converter.nfa.AcceptsWord("a"));
        Assert.False(converter.nfa.AcceptsWord("ae"));
        Assert.False(converter.nfa.AcceptsWord("aae"));
    }

    [Fact]
    public void ConvertComplexAfa()
    {

        string regex = "(?=a)a*(?=ef*)ef";

        Lexer lexer = new Lexer(regex);
        lexer.Tokenize();
        lexer.GetTokens().ForEach(t =>
        {
            if (t.tokenOP != Token.OP.Class) return;
            ((ClassToken)t).ConvertToGroup();
        });

        ParserSimplifier parser = new ParserSimplifier(lexer.GetTokens());
        parser.Simplify();

        Automaton automaton = new Automaton(parser.GetTokens(), true);
        automaton.SetStateName();

        AFAToNFAConverter converter = new AFAToNFAConverter(automaton);

        Assert.False(converter.nfa.AcceptsWord("a"));
        Assert.False(converter.nfa.AcceptsWord("ae"));

        Assert.True(converter.nfa.AcceptsWord("aaaaaef"));
        Assert.True(converter.nfa.AcceptsWord("aaef"));
    }

    [Fact]
    public void ConvertComplexAfa2()
    {

        string regex = "(?=a)a*(f|c*g)|gh+r";

        Lexer lexer = new Lexer(regex);
        lexer.Tokenize();
        lexer.GetTokens().ForEach(t =>
        {
            if (t.tokenOP != Token.OP.Class) return;
            ((ClassToken)t).ConvertToGroup();
        });

        ParserSimplifier parser = new ParserSimplifier(lexer.GetTokens());
        parser.Simplify();

        Automaton automaton = new Automaton(parser.GetTokens(), true);
        automaton.SetStateName();

        AFAToNFAConverter converter = new AFAToNFAConverter(automaton);

        Assert.False(converter.nfa.AcceptsWord("a"));
        Assert.False(converter.nfa.AcceptsWord("acc"));

        Assert.True(converter.nfa.AcceptsWord("af"));
        Assert.True(converter.nfa.AcceptsWord("aaaaaf"));
        Assert.True(converter.nfa.AcceptsWord("acg"));
        Assert.True(converter.nfa.AcceptsWord("aacg"));
        Assert.True(converter.nfa.AcceptsWord("accg"));
        Assert.True(converter.nfa.AcceptsWord("aaacccg"));
        Assert.True(converter.nfa.AcceptsWord("ghr"));
        Assert.True(converter.nfa.AcceptsWord("ghhhr"));

    }

    [Fact]
    public void ConvertAFAWithStartLookahead()
    {

        string regex = "(?=a+)b+";

        Lexer lexer = new Lexer(regex);
        lexer.Tokenize();
        lexer.GetTokens().ForEach(t =>
        {
            if (t.tokenOP != Token.OP.Class) return;
            ((ClassToken)t).ConvertToGroup();
        });

        ParserSimplifier parser = new ParserSimplifier(lexer.GetTokens());
        parser.Simplify();

        Automaton automaton = new Automaton(parser.GetTokens(), true);
        automaton.SetStateName();

        AFAToNFAConverter converter = new AFAToNFAConverter(automaton);

        Assert.False(converter.nfa.AcceptsWord("a"));
        Assert.False(converter.nfa.AcceptsWord("aa"));

        Assert.False(converter.nfa.AcceptsWord("ab"));
        Assert.False(converter.nfa.AcceptsWord("aabb"));
        Assert.False(converter.nfa.AcceptsWord("b"));
        Assert.False(converter.nfa.AcceptsWord("bb"));
        Assert.False(converter.nfa.AcceptsWord("bbb"));
        Assert.False(converter.nfa.AcceptsWord("aabbb"));
        Assert.False(converter.nfa.AcceptsWord("baba"));
        Assert.False(converter.nfa.AcceptsWord("bba"));

    }

    [Fact]
    public void ConvertAFAWithLookaheadStarGroup()
    {

        string regex = "(?=a)(a|b)*";

        Lexer lexer = new Lexer(regex);
        lexer.Tokenize();
        lexer.GetTokens().ForEach(t =>
        {
            if (t.tokenOP != Token.OP.Class) return;
            ((ClassToken)t).ConvertToGroup();
        });

        ParserSimplifier parser = new ParserSimplifier(lexer.GetTokens());
        parser.Simplify();

        Automaton automaton = new Automaton(parser.GetTokens(), true);
        automaton.SetStateName();

        AFAToNFAConverter converter = new AFAToNFAConverter(automaton);

        Assert.True(converter.nfa.AcceptsWord("a"));
        Assert.True(converter.nfa.AcceptsWord("aa"));

        Assert.True(converter.nfa.AcceptsWord("ab"));
        Assert.True(converter.nfa.AcceptsWord("aabb"));
        Assert.True(converter.nfa.AcceptsWord("aabbabaa"));
        Assert.True(converter.nfa.AcceptsWord("aabbba"));
        Assert.False(converter.nfa.AcceptsWord("b"));
        Assert.False(converter.nfa.AcceptsWord("bba"));
        Assert.False(converter.nfa.AcceptsWord("babb"));
        Assert.False(converter.nfa.AcceptsWord("baba"));
        Assert.False(converter.nfa.AcceptsWord("bba"));

    }


    [Fact]
    public void ConvertAFAWithStartLookaheadInGroupe()
    {

        string regex = "((?=a+))b+";

        Lexer lexer = new Lexer(regex);
        lexer.Tokenize();
        lexer.GetTokens().ForEach(t =>
        {
            if (t.tokenOP != Token.OP.Class) return;
            ((ClassToken)t).ConvertToGroup();
        });

        ParserSimplifier parser = new ParserSimplifier(lexer.GetTokens());
        parser.Simplify();

        Automaton automaton = new Automaton(parser.GetTokens(), true);
        automaton.SetStateName();

        AFAToNFAConverter converter = new AFAToNFAConverter(automaton);

        Assert.False(converter.nfa.AcceptsWord("a"));
        Assert.False(converter.nfa.AcceptsWord("aa"));

        Assert.False(converter.nfa.AcceptsWord("ab"));
        Assert.False(converter.nfa.AcceptsWord("aabb"));
        Assert.False(converter.nfa.AcceptsWord("b"));
        Assert.False(converter.nfa.AcceptsWord("bb"));
        Assert.False(converter.nfa.AcceptsWord("bbb"));
        Assert.False(converter.nfa.AcceptsWord("aabbb"));
        Assert.False(converter.nfa.AcceptsWord("baba"));
        Assert.False(converter.nfa.AcceptsWord("bba"));

    }

    [Fact]
    public void NonAcceptingAFAWithMultipleLookaheads()
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

        Automaton automaton = new Automaton(parser.GetTokens(), true);
        automaton.SetStateName();

        AFAToNFAConverter converter = new AFAToNFAConverter(automaton);

        Assert.False(converter.nfa.AcceptsWord("e"));
        Assert.False(converter.nfa.AcceptsWord("a"));
        Assert.False(converter.nfa.AcceptsWord("ae"));
        Assert.False(converter.nfa.AcceptsWord("aee"));
        Assert.False(converter.nfa.AcceptsWord("aaee"));

    }

    [Fact]
    public void ConvertRepeatingLookahead()
    {

        string regex = "(e((?=ab)(a*b+))*g)";

        Lexer lexer = new Lexer(regex);
        lexer.Tokenize();
        lexer.GetTokens().ForEach(t =>
        {
            if (t.tokenOP != Token.OP.Class) return;
            ((ClassToken)t).ConvertToGroup();
        });

        ParserSimplifier parser = new ParserSimplifier(lexer.GetTokens());
        parser.Simplify();

        Automaton automaton = new Automaton(parser.GetTokens(), true);
        automaton.SetStateName();

        AFAToNFAConverter converter = new AFAToNFAConverter(automaton);

        Assert.True(converter.nfa.AcceptsWord("eg"));
        Assert.True(converter.nfa.AcceptsWord("eabg"));
        Assert.True(converter.nfa.AcceptsWord("eabbg"));
        Assert.True(converter.nfa.AcceptsWord("eabbbbababg"));
        Assert.True(converter.nfa.AcceptsWord("eababg"));
        Assert.False(converter.nfa.AcceptsWord("eabaabbg"));

    }

    [Fact]
    public void ConvertRepeatingLookaheadWithKleeneStar()
    {

        string regex = "(a((?=a)a*)*b)";

        Lexer lexer = new Lexer(regex);
        lexer.Tokenize();
        lexer.GetTokens().ForEach(t =>
        {
            if (t.tokenOP != Token.OP.Class) return;
            ((ClassToken)t).ConvertToGroup();
        });

        ParserSimplifier parser = new ParserSimplifier(lexer.GetTokens());
        parser.Simplify();

        Automaton automaton = new Automaton(parser.GetTokens(), true);
        automaton.SetStateName();

        AFAToNFAConverter converter = new AFAToNFAConverter(automaton);

        Assert.True(converter.nfa.AcceptsWord("ab"));
        Assert.True(converter.nfa.AcceptsWord("aab"));
        Assert.True(converter.nfa.AcceptsWord("aaab"));
        Assert.True(converter.nfa.AcceptsWord("aaaaaaaaab"));
        Assert.False(converter.nfa.AcceptsWord("b"));
        Assert.False(converter.nfa.AcceptsWord("aaaabab"));

    }

    [Fact]
    public void ConvertLookaheadWithEpsilonTransitionInLookahead()
    {

        string regex = "a(?=a(b)+)ab*";

        Lexer lexer = new Lexer(regex);
        lexer.Tokenize();
        lexer.GetTokens().ForEach(t =>
        {
            if (t.tokenOP != Token.OP.Class) return;
            ((ClassToken)t).ConvertToGroup();
        });

        ParserSimplifier parser = new ParserSimplifier(lexer.GetTokens());
        parser.Simplify();

        Automaton automaton = new Automaton(parser.GetTokens(), true);
        automaton.SetStateName();

        AFAToNFAConverter converter = new AFAToNFAConverter(automaton);

        Assert.True(converter.nfa.AcceptsWord("aab"));
        Assert.True(converter.nfa.AcceptsWord("aabbb"));
        Assert.True(converter.nfa.AcceptsWord("aabbbb"));
        Assert.False(converter.nfa.AcceptsWord("ab"));
        Assert.False(converter.nfa.AcceptsWord("ba"));
        Assert.False(converter.nfa.AcceptsWord("baba"));

    }

    public void ConvertRepeatableLookaheadWithRepeatableRegEx()
    {

        string regex = "a(?=a+b+)a*";

        Lexer lexer = new Lexer(regex);
        lexer.Tokenize();
        lexer.GetTokens().ForEach(t =>
        {
            if (t.tokenOP != Token.OP.Class) return;
            ((ClassToken)t).ConvertToGroup();
        });

        ParserSimplifier parser = new ParserSimplifier(lexer.GetTokens());
        parser.Simplify();

        Automaton automaton = new Automaton(parser.GetTokens(), true);
        automaton.SetStateName();

        AFAToNFAConverter converter = new AFAToNFAConverter(automaton);

        Assert.True(converter.nfa.AcceptsWord("aab"));
        Assert.True(converter.nfa.AcceptsWord("aabbb"));
        Assert.True(converter.nfa.AcceptsWord("aabbbb"));
        Assert.False(converter.nfa.AcceptsWord("ab"));
        Assert.False(converter.nfa.AcceptsWord("ba"));
        Assert.False(converter.nfa.AcceptsWord("baba"));

    }

    [Fact]
    public void ConvertComplexLookahead()
    {

        string regex = "a(?=bc(de*|(eh+)f)+)bcehf";

        Lexer lexer = new Lexer(regex);
        lexer.Tokenize();
        lexer.GetTokens().ForEach(t =>
        {
            if (t.tokenOP != Token.OP.Class) return;
            ((ClassToken)t).ConvertToGroup();
        });

        ParserSimplifier parser = new ParserSimplifier(lexer.GetTokens());
        parser.Simplify();

        Automaton automaton = new Automaton(parser.GetTokens(), true);
        automaton.SetStateName();

        AFAToNFAConverter converter = new AFAToNFAConverter(automaton);

        Assert.True(converter.nfa.AcceptsWord("abcehf"));
        Assert.False(converter.nfa.AcceptsWord("abcehhhhhf"));
        Assert.False(converter.nfa.AcceptsWord("aabcehf"));
        Assert.False(converter.nfa.AcceptsWord("abccceehf"));

    }

    [Fact]
    public void ConvertOverlappingLookahead()
    {

        string regex = "(a(?=ba)b)+a";

        Lexer lexer = new Lexer(regex);
        lexer.Tokenize();
        lexer.GetTokens().ForEach(t =>
        {
            if (t.tokenOP != Token.OP.Class) return;
            ((ClassToken)t).ConvertToGroup();
        });

        ParserSimplifier parser = new ParserSimplifier(lexer.GetTokens());
        parser.Simplify();

        Automaton automaton = new Automaton(parser.GetTokens(), true);
        automaton.SetStateName();

        AFAToNFAConverter converter = new AFAToNFAConverter(automaton);

        Assert.True(converter.nfa.AcceptsWord("aba"));
        Assert.True(converter.nfa.AcceptsWord("ababa"));
        Assert.True(converter.nfa.AcceptsWord("abababa"));
        Assert.False(converter.nfa.AcceptsWord("ab"));
        Assert.False(converter.nfa.AcceptsWord("abab"));
        Assert.False(converter.nfa.AcceptsWord("abababb"));

    }

    [Fact]
    public void ConvertOverlappingLookahead2()
    {

        string regex = "(a(?=bab)b)+ab";

        Lexer lexer = new Lexer(regex);
        lexer.Tokenize();
        lexer.GetTokens().ForEach(t =>
        {
            if (t.tokenOP != Token.OP.Class) return;
            ((ClassToken)t).ConvertToGroup();
        });

        ParserSimplifier parser = new ParserSimplifier(lexer.GetTokens());
        parser.Simplify();

        Automaton automaton = new Automaton(parser.GetTokens(), true);
        automaton.SetStateName();

        AFAToNFAConverter converter = new AFAToNFAConverter(automaton);

        Assert.True(converter.nfa.AcceptsWord("abab"));
        Assert.True(converter.nfa.AcceptsWord("ababab"));
        Assert.True(converter.nfa.AcceptsWord("ababababab"));
        Assert.False(converter.nfa.AcceptsWord("aba"));
        Assert.False(converter.nfa.AcceptsWord("ab"));
        Assert.False(converter.nfa.AcceptsWord("ababa"));

    }

    [Fact]
    public void ConvertNestedLookahead()
    {

        string regex = "a(?=a(?=ba)b)aba";

        Lexer lexer = new Lexer(regex);
        lexer.Tokenize();
        lexer.GetTokens().ForEach(t =>
        {
            if (t.tokenOP != Token.OP.Class) return;
            ((ClassToken)t).ConvertToGroup();
        });

        ParserSimplifier parser = new ParserSimplifier(lexer.GetTokens());
        parser.Simplify();

        Automaton automaton = new Automaton(parser.GetTokens(), true);
        automaton.SetStateName();

        AFAToNFAConverter converter = new AFAToNFAConverter(automaton);

        Assert.False(converter.nfa.AcceptsWord("aab"));
        Assert.False(converter.nfa.AcceptsWord("abab"));
        Assert.False(converter.nfa.AcceptsWord("abaa"));
        Assert.True(converter.nfa.AcceptsWord("aaba"));

    }

    [Fact]
    public void ConvertNestedLookahead2()
    {

        string regex = "a(?=a(?=ba)b+)abab*";

        Lexer lexer = new Lexer(regex);
        lexer.Tokenize();
        lexer.GetTokens().ForEach(t =>
        {
            if (t.tokenOP != Token.OP.Class) return;
            ((ClassToken)t).ConvertToGroup();
        });

        ParserSimplifier parser = new ParserSimplifier(lexer.GetTokens());
        parser.Simplify();

        Automaton automaton = new Automaton(parser.GetTokens(), true);
        automaton.SetStateName();

        AFAToNFAConverter converter = new AFAToNFAConverter(automaton);

        Assert.False(converter.nfa.AcceptsWord("aab"));
        Assert.False(converter.nfa.AcceptsWord("abab"));
        Assert.False(converter.nfa.AcceptsWord("abab"));
        Assert.True(converter.nfa.AcceptsWord("aaba"));
        Assert.True(converter.nfa.AcceptsWord("aabab"));
        Assert.True(converter.nfa.AcceptsWord("aababbb"));

    }

    [Fact]
    public void ConvertNestedEpsilonLookahead()
    {

        string regex = "a(?=a(?=bc))ab(?=c)c";

        Lexer lexer = new Lexer(regex);
        lexer.Tokenize();
        lexer.GetTokens().ForEach(t =>
        {
            if (t.tokenOP != Token.OP.Class) return;
            ((ClassToken)t).ConvertToGroup();
        });

        ParserSimplifier parser = new ParserSimplifier(lexer.GetTokens());
        parser.Simplify();

        Automaton automaton = new Automaton(parser.GetTokens(), true);
        automaton.SetStateName();

        AFAToNFAConverter converter = new AFAToNFAConverter(automaton);

        Assert.False(converter.nfa.AcceptsWord("abbc"));
        Assert.False(converter.nfa.AcceptsWord("ac"));
        Assert.False(converter.nfa.AcceptsWord("ab"));
        Assert.False(converter.nfa.AcceptsWord("aabbc"));
        Assert.False(converter.nfa.AcceptsWord("aabcc"));
        Assert.True(converter.nfa.AcceptsWord("aabc"));

    }

    [Fact]
    public void ConvertNestedEpsilonLookaheadRepeating()
    {

        string regex = "a(?=a(?=bc))ab(?=c)c*";

        Lexer lexer = new Lexer(regex);
        lexer.Tokenize();
        lexer.GetTokens().ForEach(t =>
        {
            if (t.tokenOP != Token.OP.Class) return;
            ((ClassToken)t).ConvertToGroup();
        });

        ParserSimplifier parser = new ParserSimplifier(lexer.GetTokens());
        parser.Simplify();

        Automaton automaton = new Automaton(parser.GetTokens(), true);
        automaton.SetStateName();

        AFAToNFAConverter converter = new AFAToNFAConverter(automaton);

        Assert.False(converter.nfa.AcceptsWord("abbc"));
        Assert.False(converter.nfa.AcceptsWord("ac"));
        Assert.False(converter.nfa.AcceptsWord("ab"));
        Assert.False(converter.nfa.AcceptsWord("aabbc"));
        Assert.True(converter.nfa.AcceptsWord("aabc"));
        Assert.True(converter.nfa.AcceptsWord("aabcc"));
        Assert.True(converter.nfa.AcceptsWord("aabcccc"));

    }

    [Fact]
    public void ConvertNestedEpsilonLookaheadRepeating2()
    {

        string regex = "a(?=a(?=bc))ab(?=c)c*d";

        Lexer lexer = new Lexer(regex);
        lexer.Tokenize();
        lexer.GetTokens().ForEach(t =>
        {
            if (t.tokenOP != Token.OP.Class) return;
            ((ClassToken)t).ConvertToGroup();
        });

        ParserSimplifier parser = new ParserSimplifier(lexer.GetTokens());
        parser.Simplify();

        Automaton automaton = new Automaton(parser.GetTokens(), true);
        automaton.SetStateName();

        AFAToNFAConverter converter = new AFAToNFAConverter(automaton);

        Assert.False(converter.nfa.AcceptsWord("abbc"));
        Assert.False(converter.nfa.AcceptsWord("ac"));
        Assert.False(converter.nfa.AcceptsWord("ab"));
        Assert.False(converter.nfa.AcceptsWord("aabbc"));
        Assert.True(converter.nfa.AcceptsWord("aabcd"));
        Assert.True(converter.nfa.AcceptsWord("aabccd"));
        Assert.True(converter.nfa.AcceptsWord("aabccccd"));
        Assert.True(converter.nfa.AcceptsWord("aabccccccd"));

    }
    [Fact]
    public void ConvertNestedLookaheadWithRepeatingOverlap()
    {

        string regex = "(a(?=a(?=bc))ab(?=c)c*)+d";

        Lexer lexer = new Lexer(regex);
        lexer.Tokenize();
        lexer.GetTokens().ForEach(t =>
        {
            if (t.tokenOP != Token.OP.Class) return;
            ((ClassToken)t).ConvertToGroup();
        });

        ParserSimplifier parser = new ParserSimplifier(lexer.GetTokens());
        parser.Simplify();

        Automaton automaton = new Automaton(parser.GetTokens(), true);
        automaton.SetStateName();

        AFAToNFAConverter converter = new AFAToNFAConverter(automaton);

        Assert.False(converter.nfa.AcceptsWord("aabcccc"));
        Assert.False(converter.nfa.AcceptsWord("aabd"));
        Assert.False(converter.nfa.AcceptsWord("aabccccaabd"));
        Assert.False(converter.nfa.AcceptsWord("aabccaabccabccd"));
        Assert.True(converter.nfa.AcceptsWord("aabccccd"));
        Assert.True(converter.nfa.AcceptsWord("aabcccaabcd"));
        Assert.True(converter.nfa.AcceptsWord("aabcd"));
        Assert.True(converter.nfa.AcceptsWord("aabcaabcaabccd"));
        Assert.True(converter.nfa.AcceptsWord("aabccaabcccaabcccd"));

    }

}