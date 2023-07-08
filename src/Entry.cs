using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using CommandLine;
public class Entry
{

    [DllImport("kernel32.dll")]
    static extern bool AttachConsole(int dwProcessId);
    private const int ATTACH_PARENT_PROCESS = -1;

    private static bool isAFA = false;

    public static bool verbose { get; private set; }

    [STAThread]
    public static void Main(string[] args)
    {

        AttachConsole(ATTACH_PARENT_PROCESS);

        Parser.Default.ParseArguments<Options>(args)
                        .WithParsed<Options>(o =>
                        {

                            if (o.verbose)
                                verbose = true;
                            else
                                verbose = false;

                            Console.WriteLine($"Regex: {o.regex}");
                            ParserSimplifier simplifier = SimplifyRegex(o.regex);

                            if (Entry.verbose)
                                Console.WriteLine("---------------------------------- Simplifying final Regex ----------------------------------\n");

                            simplifier.Simplify();

                            Console.WriteLine("Simplified Regex: " + simplifier.TokenStreamToString());

                            List<Token> stream = simplifier.GetTokens();

                            Automaton automaton = new Automaton(stream, true);
                            automaton.SetStateName();

                            if (!isAFA && o.showNFA)
                            {
                                EpsilonEliminator.RemoveEpsilonFromState(automaton.startStates[0]);
                                new AutomatonVisualizer(automaton.startStates[0]);
                                if (o.words.Any())
                                    SimulateAutomaton(o.words, automaton);
                                return;
                            }

                            if (o.showAFA)
                                new AutomatonVisualizer(automaton.startStates[0]);

                            AFAToNFAConverter converter = new AFAToNFAConverter(automaton);

                            if (o.showVerboseAFA)
                                new AutomatonVisualizer(converter.afa.startStates[0]);

                            if (o.showNFA)
                                new AutomatonVisualizer(converter.nfa.startStates[0]);

                            if (o.words.Any())
                                SimulateAutomaton(o.words, converter.nfa);

                        });
    }

    private static ParserSimplifier SimplifyRegex(string regex)
    {
        Lexer lexer = new Lexer(regex);
        lexer.Tokenize();

        lexer.GetTokens().ForEach(t =>
       {
           if (t.ContainsLookahead() && !isAFA)
               isAFA = true;

           if (t.tokenOP != Token.OP.Class) return;

           ((ClassToken)t).ConvertToGroup();

       });

        ParserSimplifier parser = new ParserSimplifier(lexer.GetTokens());
        return parser;

    }

    private static void SimulateAutomaton(IEnumerable<string> words, Automaton automaton)
    {

        Console.WriteLine("---------------------------------- Running words ---------------------------------- \n");

        var enumerator = words.GetEnumerator();

        while (enumerator.MoveNext())
        {
            string word = enumerator.Current;
            Console.WriteLine($"Regex Accepts \"{word}\": {automaton.AcceptsWord(word)}\n");
        }
    }

}