using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
public class Entry
{

    [DllImport("kernel32.dll")]
    static extern bool AttachConsole(int dwProcessId);
    private const int ATTACH_PARENT_PROCESS = -1;

    [STAThread]
    public static void Main(string[] args)
    {

        AttachConsole(ATTACH_PARENT_PROCESS);

        if (args.Length < 1)
        {
            Console.WriteLine("no arguments, nothing to do.");
            return;
        }

        Console.WriteLine("Start {0}", args[0]);

        Lexer lexer = new Lexer(args[0]);
        lexer.Tokenize();

        lexer.GetTokens().ForEach(t =>
        {
            Console.WriteLine("token: {0}: {1}", t.tokenOP, t.symbol);

            if (t.tokenOP != Token.OP.Class) return;

            ((ClassToken)t).ConvertToGroup();

        });

        ParserSimplifier parser = new ParserSimplifier(lexer.GetTokens());
        parser.Simplify();

        Console.WriteLine("s: " + parser.TokenStreamToString());

        List<Token> stream = parser.GetTokens();

        Automaton a = new Automaton(stream);
        a.SetStateName();

        EpsilonEliminator.RemoveEpsilonFromState(a.startStates[0]);

        for (int i = 1; i < args.Length; i++)
        {
            Console.WriteLine($"Regex Accepts \"{args[i]}\":{a.AcceptsWord(args[i])}\n");
        }

        AutomatonVisualizer visualizer = new AutomatonVisualizer(a.startStates[0]);

    }

}