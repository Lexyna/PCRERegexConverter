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

        bool isAFA = false;

        lexer.GetTokens().ForEach(t =>
        {
            Console.WriteLine("token: {0}: {1}", t.tokenOP, t.symbol);
            if (t.ContainsLookahead() && !isAFA)
                isAFA = true;

            if (t.tokenOP != Token.OP.Class) return;

            ((ClassToken)t).ConvertToGroup();

        });

        ParserSimplifier parser = new ParserSimplifier(lexer.GetTokens());
        parser.Simplify();

        Console.WriteLine("simplified Regex: " + parser.TokenStreamToString());

        List<Token> stream = parser.GetTokens();

        Automaton automaton = new Automaton(stream, true);
        automaton.SetStateName();

        //EpsilonEliminator.RemoveEpsilonFromState(automaton.startStates[0]);

        AutomatonVisualizer nfaVisualizer = new AutomatonVisualizer(automaton.startStates[0]);

        if (isAFA)
        {
            AFAToNFAConverter conv = new AFAToNFAConverter(automaton);

            //SimulateAutomaton(args[1..args.Length], conv.nfa);

            AutomatonVisualizer visualizer = new AutomatonVisualizer(conv.nfa.startStates[0]);
        }
        else
        {
            SimulateAutomaton(args[1..args.Length], automaton);
        }

    }

    private static void SimulateAutomaton(string[] args, Automaton automaton)
    {

        for (int i = 0; i < args.Length; i++)
        {
            Console.WriteLine($"Regex Accepts \"{args[i]}\":{automaton.AcceptsWord(args[i])}\n");
        }

    }

}