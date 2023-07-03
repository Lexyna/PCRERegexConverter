public class AFAToNFAConverter
{

    Automaton afa;

    public Automaton nfa { get; private set; }

    public AFAToNFAConverter(Automaton afa)
    {
        this.afa = afa;
        this.nfa = new Automaton();
        Convert();
    }

    private void Convert()
    {


    }

}