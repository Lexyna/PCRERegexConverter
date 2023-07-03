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

    private List<State> GetEpsilonHull(State state, HashSet<string> visited)
    {

        if (visited.Contains(state.uuid))
            return new List<State>();
        visited.Add(state.uuid);

        List<State> reachable = new List<State>();

        reachable.Add(state);

        state.GetOutgoingTransitions().ForEach(transition =>
        {
            if (transition.symbol == "")
            {
                reachable.Add(transition.GetOutState());
                reachable.AddRange(GetEpsilonHull(transition.GetOutState(), visited));
            }
        });
        return reachable;
    }

}