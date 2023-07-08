using CommandLine;

public class Options
{

    [Option('r', "regex", Required = true, HelpText = "Regex to convert.")]
    public string regex { get; set; }

    [Option('v', "verbose", Required = false, Default = false, HelpText = "Shows verbose output for regex transformation")]
    public bool verbose { get; set; }

    [Option("show_nfa", Required = false, HelpText = "Shows the resulting epsilon free nfa without")]
    public bool showNFA { get; set; }

    [Option("show_afa", Required = false, HelpText = "Shows the resulting afa")]
    public bool showAFA { get; set; }

    [Option("show_verbose_nfa", Required = false, HelpText = "Shows the resulting free nfa from afa conversion, including epsilon Transitions")]
    public bool showVerboseAFA { get; set; }

    [Option("parse_words", Required = false, HelpText = "Runs the given words through the nfa and outputs if a given word is accepted or not")]
    public IEnumerable<string> words { get; set; }
}