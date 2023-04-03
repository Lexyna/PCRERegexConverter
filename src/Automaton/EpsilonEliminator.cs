public static class EpsilonEliminator
{

    private static Dictionary<string, bool> visited = new Dictionary<string, bool>();

    public static void RemoveEpsilonFromState(State state, bool init = true)
    {
        if (init)
            visited = new Dictionary<string, bool>();

        if (!visited.ContainsKey(state.id))
            visited.Add(state.id, true);

        int epsilonIndex = HasEpsilonTransition(state);

        if (epsilonIndex == -1)
        {
            foreach (Transition t in state.GetOutgoingTransitions())
                if (t.GetOutState().id != state.id && !visited.ContainsKey(t.GetOutState().id))
                    RemoveEpsilonFromState(t.GetOutState(), false);
            return;
        }

        if (state.id == state.GetOutgoingTransitions()[epsilonIndex].GetOutState().id)
        {
            state.GetOutgoingTransitions()[epsilonIndex].Delete();
            return;
        }

        RemoveEpsilonTransitionToNode(state, state.GetOutgoingTransitions()[epsilonIndex].GetOutState());
        state.GetOutgoingTransitions()[epsilonIndex].Delete();

        RemoveEpsilonFromState(state, false);
    }

    private static int HasEpsilonTransition(State s)
    {
        for (int i = 0; i < s.GetOutgoingTransitions().Count; i++)
            if (s.GetOutgoingTransitions()[i].symbol == "" && !s.GetOutgoingTransitions()[i].universal)
                return i;
        return -1;
    }

    private static void RemoveEpsilonTransitionToNode(State v1, State v2)
    {

        Dictionary<string, Transition> reachable = new Dictionary<string, Transition>();

        FindReachableStates(v2, reachable);

        foreach (KeyValuePair<string, Transition> entry in reachable)
        {
            Transition t = entry.Value;

            if (t.symbol != "")
            {

                Transition newTransition = new Transition(v1, t.symbol, t.GetOutState());
                newTransition.Apply();

            }
            else
            {

                if (t.GetOutState().isEndState)
                    v1.SetEndState(true);

            }
        }

        for (int i = 0; i < v2.GetOutgoingTransitions().Count; i++)
            if (v2.GetOutgoingTransitions()[i].universal)
            {
                Transition lookaheadTransition = new Transition(v1, "", v2.GetOutgoingTransitions()[i].GetOutState(), true);
                lookaheadTransition.Apply();
            }


        if (v2.isEndState)
            v1.SetEndState(true);

    }

    private static void FindReachableStates(State s, Dictionary<string, Transition> reachable)
    {

        for (int i = 0; i < s.GetOutgoingTransitions().Count; i++)
        {
            if (reachable.ContainsKey(s.GetOutgoingTransitions()[i].uuid)
            || s.GetOutgoingTransitions()[i].universal) continue;

            reachable.Add(s.GetOutgoingTransitions()[i].uuid, s.GetOutgoingTransitions()[i]);

            if (s.GetOutgoingTransitions()[i].symbol == "")
                FindReachableStates(s.GetOutgoingTransitions()[i].GetOutState(), reachable);
        }

    }

}