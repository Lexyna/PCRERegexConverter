using Xunit;

public class AutomatonBuilderTests
{

    [Fact]
    public void AutomatonA()
    {

        List<Token> stream = new List<Token>();
        stream.Add(new TerminalToken("a"));

        Automaton auto = new Automaton(stream);
        auto.SetStateName();

        Assert.True(auto.AcceptsWord("a"));
        Assert.False(auto.AcceptsWord(""));

    }

    [Fact]
    public void AutomatonAB()
    {

        List<Token> stream = new List<Token>();
        stream.Add(new TerminalToken("a"));
        stream.Add(new TerminalToken("b"));

        Automaton auto = new Automaton(stream);
        auto.SetStateName();

        Assert.True(auto.AcceptsWord("ab"));
        Assert.False(auto.AcceptsWord(""));
        Assert.False(auto.AcceptsWord("a"));

    }

    [Fact]
    public void AutomatonABC()
    {

        List<Token> stream = new List<Token>();
        stream.Add(new TerminalToken("a"));
        stream.Add(new TerminalToken("b"));
        stream.Add(new TerminalToken("c"));

        Automaton auto = new Automaton(stream);
        auto.SetStateName();

        Assert.True(auto.AcceptsWord("abc"));
        Assert.False(auto.AcceptsWord(""));
        Assert.False(auto.AcceptsWord("a"));
        Assert.False(auto.AcceptsWord("ab"));

    }

    [Fact]
    public void AutomatonAStar()
    {

        List<Token> stream = new List<Token>();
        stream.Add(new TerminalToken("a"));
        stream.Add(new StarToken());

        Automaton auto = new Automaton(stream);
        auto.SetStateName();

        Assert.True(auto.AcceptsWord(""));
        Assert.True(auto.AcceptsWord("a"));
        Assert.True(auto.AcceptsWord("aa"));
        Assert.True(auto.AcceptsWord("aaa"));
        Assert.True(auto.AcceptsWord("aaaa"));

    }

    [Fact]
    public void AutomatonAOptional()
    {

        List<Token> stream = new List<Token>();
        stream.Add(new TerminalToken("a"));
        stream.Add(new OptionalToken());

        Automaton auto = new Automaton(stream);
        auto.SetStateName();

        Assert.True(auto.AcceptsWord("a"));
        Assert.True(auto.AcceptsWord(""));

        Assert.False(auto.AcceptsWord("aa"));

    }

    [Fact]
    public void AutomatonABOptional()
    {

        List<Token> stream = new List<Token>();
        stream.Add(new TerminalToken("a"));
        stream.Add(new TerminalToken("b"));
        stream.Add(new OptionalToken());

        Automaton auto = new Automaton(stream);
        auto.SetStateName();

        Assert.True(auto.AcceptsWord("a"));
        Assert.True(auto.AcceptsWord("ab"));

        Assert.False(auto.AcceptsWord("aa"));
        Assert.False(auto.AcceptsWord("b"));

    }

    [Fact]
    public void AutomatonABStar()
    {

        List<Token> stream = new List<Token>();
        stream.Add(new TerminalToken("a"));
        stream.Add(new TerminalToken("b"));
        stream.Add(new StarToken());

        Automaton auto = new Automaton(stream);
        auto.SetStateName();

        Assert.True(auto.AcceptsWord("a"));
        Assert.True(auto.AcceptsWord("ab"));
        Assert.True(auto.AcceptsWord("abb"));
        Assert.True(auto.AcceptsWord("abbb"));

        Assert.False(auto.AcceptsWord("aa"));
        Assert.False(auto.AcceptsWord("b"));

    }

    [Fact]
    public void AutomatonABStarC()
    {

        List<Token> stream = new List<Token>();
        stream.Add(new TerminalToken("a"));
        stream.Add(new TerminalToken("b"));
        stream.Add(new StarToken());
        stream.Add(new TerminalToken("c"));

        Automaton auto = new Automaton(stream);
        auto.SetStateName();

        Assert.False(auto.AcceptsWord("a"));
        Assert.False(auto.AcceptsWord("ab"));
        Assert.False(auto.AcceptsWord("abb"));
        Assert.False(auto.AcceptsWord("abbb"));

        Assert.True(auto.AcceptsWord("ac"));
        Assert.True(auto.AcceptsWord("abc"));
        Assert.True(auto.AcceptsWord("abbc"));
        Assert.True(auto.AcceptsWord("abbbc"));

    }

    [Fact]
    public void AutomatonAStarBOptional()
    {

        List<Token> stream = new List<Token>();
        stream.Add(new TerminalToken("a"));
        stream.Add(new StarToken());
        stream.Add(new TerminalToken("b"));
        stream.Add(new OptionalToken());

        Automaton auto = new Automaton(stream);
        auto.SetStateName();


        Assert.True(auto.AcceptsWord(""));
        Assert.True(auto.AcceptsWord("a"));
        Assert.True(auto.AcceptsWord("aab"));
        Assert.True(auto.AcceptsWord("aa"));
        Assert.True(auto.AcceptsWord("aaaab"));
    }

    [Fact]
    public void AutomatonAOptionalBStar()
    {

        List<Token> stream = new List<Token>();
        stream.Add(new TerminalToken("a"));
        stream.Add(new OptionalToken());
        stream.Add(new TerminalToken("b"));
        stream.Add(new StarToken());

        Automaton auto = new Automaton(stream);
        auto.SetStateName();


        Assert.True(auto.AcceptsWord(""));
        Assert.True(auto.AcceptsWord("a"));
        Assert.True(auto.AcceptsWord("ab"));
        Assert.True(auto.AcceptsWord("abb"));
        Assert.True(auto.AcceptsWord("bbbb"));
    }

    [Fact]
    public void AutomatonAAlternateB()
    {

        List<Token> stream = new List<Token>();
        stream.Add(new TerminalToken("a"));
        stream.Add(new AlternateToken());
        stream.Add(new TerminalToken("b"));

        Automaton auto = new Automaton(stream);
        auto.SetStateName();

        Assert.True(auto.AcceptsWord("a"));
        Assert.True(auto.AcceptsWord("b"));
    }

    [Fact]
    public void AutomatonAAlternateBOptional()
    {

        List<Token> stream = new List<Token>();
        stream.Add(new TerminalToken("a"));
        stream.Add(new AlternateToken());
        stream.Add(new TerminalToken("b"));
        stream.Add(new OptionalToken());

        Automaton auto = new Automaton(stream);
        auto.SetStateName();

        Assert.True(auto.AcceptsWord(""));
        Assert.True(auto.AcceptsWord("a"));
        Assert.True(auto.AcceptsWord("b"));
    }

    [Fact]
    public void AutomatonAAlternateBOptionalC()
    {

        List<Token> stream = new List<Token>();
        stream.Add(new TerminalToken("a"));
        stream.Add(new AlternateToken());
        stream.Add(new TerminalToken("b"));
        stream.Add(new OptionalToken());
        stream.Add(new TerminalToken("c"));

        Automaton auto = new Automaton(stream);
        auto.SetStateName();

        Assert.True(auto.AcceptsWord("a"));
        Assert.True(auto.AcceptsWord("bc"));
        Assert.True(auto.AcceptsWord("c"));
    }

    [Fact]
    public void AutomatonEmptyGroup()
    {

        //regex: ()a

        List<Token> g1s = new List<Token>();

        GroupToken g1 = new GroupToken("");
        g1.AddTokenStream(g1s);

        List<Token> stream = new List<Token>();
        stream.Add(g1);
        stream.Add(new TerminalToken("a"));

        Automaton auto = new Automaton(stream);
        auto.SetStateName();

        Assert.True(auto.AcceptsWord("a"));
        Assert.False(auto.AcceptsWord(""));
    }

    [Fact]
    public void AutomatonOptionalGroup()
    {

        //regex: (ab)?

        List<Token> g1s = new List<Token>();
        g1s.Add(new TerminalToken("a"));
        g1s.Add(new TerminalToken("b"));

        GroupToken g1 = new GroupToken("");
        g1.AddTokenStream(g1s);

        List<Token> stream = new List<Token>();
        stream.Add(g1);
        stream.Add(new OptionalToken());

        Automaton auto = new Automaton(stream);
        auto.SetStateName();

        Assert.True(auto.AcceptsWord("ab"));
        Assert.True(auto.AcceptsWord(""));
    }

    [Fact]
    public void AutomatonStarGroup()
    {

        //regex: (ab)?

        List<Token> g1s = new List<Token>();
        g1s.Add(new TerminalToken("a"));
        g1s.Add(new TerminalToken("b"));

        GroupToken g1 = new GroupToken("");
        g1.AddTokenStream(g1s);

        List<Token> stream = new List<Token>();
        stream.Add(g1);
        stream.Add(new StarToken());

        Automaton auto = new Automaton(stream);
        auto.SetStateName();

        Assert.True(auto.AcceptsWord(""));
        Assert.True(auto.AcceptsWord("ab"));
        Assert.True(auto.AcceptsWord("abab"));
        Assert.True(auto.AcceptsWord("ababab"));
    }

    [Fact]
    public void AutomatonOptionalGroupC()
    {

        //regex: (ab)?c

        List<Token> g1s = new List<Token>();
        g1s.Add(new TerminalToken("a"));
        g1s.Add(new TerminalToken("b"));

        GroupToken g1 = new GroupToken("");
        g1.AddTokenStream(g1s);

        List<Token> stream = new List<Token>();
        stream.Add(g1);
        stream.Add(new OptionalToken());
        stream.Add(new TerminalToken("c"));

        Automaton auto = new Automaton(stream);
        auto.SetStateName();

        Assert.True(auto.AcceptsWord("abc"));
        Assert.True(auto.AcceptsWord("c"));

        Assert.False(auto.AcceptsWord("ab"));
        Assert.False(auto.AcceptsWord(""));

    }

    [Fact]
    public void AutomatonCOptionalGroup()
    {

        //regex: c(ab)?

        List<Token> g1s = new List<Token>();
        g1s.Add(new TerminalToken("a"));
        g1s.Add(new TerminalToken("b"));

        GroupToken g1 = new GroupToken("");
        g1.AddTokenStream(g1s);

        List<Token> stream = new List<Token>();
        stream.Add(new TerminalToken("c"));
        stream.Add(g1);
        stream.Add(new OptionalToken());

        Automaton auto = new Automaton(stream);
        auto.SetStateName();

        Assert.True(auto.AcceptsWord("cab"));
        Assert.True(auto.AcceptsWord("c"));

        Assert.False(auto.AcceptsWord("ab"));
        Assert.False(auto.AcceptsWord(""));

    }

    [Fact]
    public void AutomatonGroupOptionalGroup()
    {

        //regex: (cd)(ab)?

        List<Token> g1s = new List<Token>();
        g1s.Add(new TerminalToken("c"));
        g1s.Add(new TerminalToken("d"));

        GroupToken g1 = new GroupToken("");
        g1.AddTokenStream(g1s);

        List<Token> g2s = new List<Token>();
        g2s.Add(new TerminalToken("a"));
        g2s.Add(new TerminalToken("b"));

        GroupToken g2 = new GroupToken("");
        g2.AddTokenStream(g2s);

        List<Token> stream = new List<Token>();
        stream.Add(g1);
        stream.Add(g2);
        stream.Add(new OptionalToken());

        Automaton auto = new Automaton(stream);
        auto.SetStateName();

        Assert.True(auto.AcceptsWord("cd"));
        Assert.True(auto.AcceptsWord("cdab"));

        Assert.False(auto.AcceptsWord("ab"));
        Assert.False(auto.AcceptsWord("c"));
        Assert.False(auto.AcceptsWord(""));

    }

    [Fact]
    public void AutomatonGroupABORGroupCD()
    {

        //regex: (ab)|(cd)

        List<Token> g1s = new List<Token>();
        g1s.Add(new TerminalToken("a"));
        g1s.Add(new TerminalToken("b"));

        GroupToken g1 = new GroupToken("");
        g1.AddTokenStream(g1s);

        List<Token> g2s = new List<Token>();
        g2s.Add(new TerminalToken("c"));
        g2s.Add(new TerminalToken("d"));

        GroupToken g2 = new GroupToken("");
        g2.AddTokenStream(g2s);

        List<Token> stream = new List<Token>();
        stream.Add(g1);
        stream.Add(new AlternateToken());
        stream.Add(g2);

        Automaton auto = new Automaton(stream);
        auto.SetStateName();

        Assert.True(auto.AcceptsWord("ab"));
        Assert.True(auto.AcceptsWord("cd"));

        Assert.False(auto.AcceptsWord("a"));
        Assert.False(auto.AcceptsWord("c"));
        Assert.False(auto.AcceptsWord("abcd"));

    }

    [Fact]
    public void AutomatonGroupABORGroupCDOptional()
    {
        //regex: (ab)|(cd)?

        List<Token> g1s = new List<Token>();
        g1s.Add(new TerminalToken("a"));
        g1s.Add(new TerminalToken("b"));

        GroupToken g1 = new GroupToken("");
        g1.AddTokenStream(g1s);

        List<Token> g2s = new List<Token>();
        g2s.Add(new TerminalToken("c"));
        g2s.Add(new TerminalToken("d"));

        GroupToken g2 = new GroupToken("");
        g2.AddTokenStream(g2s);

        List<Token> stream = new List<Token>();
        stream.Add(g1);
        stream.Add(new AlternateToken());
        stream.Add(g2);
        stream.Add(new OptionalToken());

        Automaton auto = new Automaton(stream);
        auto.SetStateName();

        Assert.True(auto.AcceptsWord(""));
        Assert.True(auto.AcceptsWord("ab"));
        Assert.True(auto.AcceptsWord("cd"));

        Assert.False(auto.AcceptsWord("a"));
        Assert.False(auto.AcceptsWord("c"));
        Assert.False(auto.AcceptsWord("abcd"));

    }

    [Fact]
    public void AutomatonGroupDOptionalGroup()
    {

        //regex: (cd)d(ab)?

        List<Token> g1s = new List<Token>();
        g1s.Add(new TerminalToken("c"));
        g1s.Add(new TerminalToken("d"));

        GroupToken g1 = new GroupToken("");
        g1.AddTokenStream(g1s);

        List<Token> g2s = new List<Token>();
        g2s.Add(new TerminalToken("a"));
        g2s.Add(new TerminalToken("b"));

        GroupToken g2 = new GroupToken("");
        g2.AddTokenStream(g2s);

        List<Token> stream = new List<Token>();
        stream.Add(g1);
        stream.Add(new TerminalToken("d"));
        stream.Add(g2);
        stream.Add(new OptionalToken());

        Automaton auto = new Automaton(stream);
        auto.SetStateName();

        Assert.True(auto.AcceptsWord("cdd"));
        Assert.True(auto.AcceptsWord("cddab"));

        Assert.False(auto.AcceptsWord("ab"));
        Assert.False(auto.AcceptsWord("c"));
        Assert.False(auto.AcceptsWord("cd"));
        Assert.False(auto.AcceptsWord("cdab"));
        Assert.False(auto.AcceptsWord(""));

    }

    [Fact]
    public void AutomatonGroupInGroup()
    {

        //regex: ((ab)?c)

        List<Token> abStream = new List<Token>();
        abStream.Add(new TerminalToken("a"));
        abStream.Add(new TerminalToken("b"));

        GroupToken abGroup = new GroupToken("");
        abGroup.AddTokenStream(abStream);

        List<Token> g1s = new List<Token>();
        g1s.Add(abGroup);
        g1s.Add(new OptionalToken());
        g1s.Add(new TerminalToken("c"));

        GroupToken g1 = new GroupToken("");
        g1.AddTokenStream(g1s);

        List<Token> stream = new List<Token>();
        stream.Add(g1);

        Automaton auto = new Automaton(stream);
        auto.SetStateName();

        Assert.True(auto.AcceptsWord("c"));
        Assert.True(auto.AcceptsWord("abc"));

        Assert.False(auto.AcceptsWord("ab"));

    }

    [Fact]
    public void AutomatonGroupInGroupD()
    {

        //regex: ((ab)?c)d

        List<Token> abStream = new List<Token>();
        abStream.Add(new TerminalToken("a"));
        abStream.Add(new TerminalToken("b"));

        GroupToken abGroup = new GroupToken("");
        abGroup.AddTokenStream(abStream);

        List<Token> g1s = new List<Token>();
        g1s.Add(abGroup);
        g1s.Add(new OptionalToken());
        g1s.Add(new TerminalToken("c"));

        GroupToken g1 = new GroupToken("");
        g1.AddTokenStream(g1s);

        List<Token> stream = new List<Token>();
        stream.Add(g1);
        stream.Add(new TerminalToken("d"));

        Automaton auto = new Automaton(stream);
        auto.SetStateName();

        Assert.True(auto.AcceptsWord("cd"));
        Assert.True(auto.AcceptsWord("abcd"));

        Assert.False(auto.AcceptsWord("ab"));
        Assert.False(auto.AcceptsWord("abc"));
        Assert.False(auto.AcceptsWord(""));

    }

}