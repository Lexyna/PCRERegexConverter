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

        //TTAC afa = new TTAC(parser.Simplify());
        //afa.CreateAFA();

        //Console.WriteLine("Simplified regex:");

        GroupToken gt = new GroupToken("");

        gt.AddTokenStream(parser.Simplify());

        Automaton afa = gt.CreateAutomaton();
        afa.SimplifyName();

        Console.WriteLine("s: " + parser.TokenStreamToString());

        AutomatonVisualizer visualizer = new AutomatonVisualizer(afa.startStates[0]);

    }

}