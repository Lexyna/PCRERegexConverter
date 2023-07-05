using Microsoft.Msagl.Drawing;

public class AutomatonVisualizer
{

    private State entry;

    public AutomatonVisualizer(State entry)
    {
        this.entry = entry;
        ShowGraph();
    }

    private void ShowGraph()
    {

        Form graphForm = new Form();

        Graph graph = new Graph("RegEx Automaton");

        HashSet<string> visited = new HashSet<string>();

        CreateEdges(graph, entry, visited);

        Node startNode = graph.FindNode(entry.id);
        startNode.Attr.FillColor = Microsoft.Msagl.Drawing.Color.Green;

        Microsoft.Msagl.GraphViewerGdi.GViewer viewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
        graph.Attr.LayerDirection = LayerDirection.LR;
        viewer.Graph = graph;

        graphForm.SuspendLayout();
        viewer.Dock = DockStyle.Fill;
        graphForm.Controls.Add(viewer);
        graphForm.ResumeLayout();
        graphForm.ShowDialog();

    }

    private void CreateEdges(Graph graph, State state, HashSet<string> visited)
    {
        if (visited.Contains(state.uuid))
            return;
        visited.Add(state.uuid);

        graph.AddNode(state.id);

        foreach (Transition t in state.GetOutgoingTransitions())
        {
            string label = String.IsNullOrEmpty(t.symbol) ? "ε" : t.symbol;

            graph.AddEdge(state.id, label, t.GetOutState().id);

            if (!visited.Contains(t.GetOutState().uuid))
                CreateEdges(graph, t.GetOutState(), visited);
        }

        Node node = graph.FindNode(state.id);
        if (!state.isUniversal)
            node.Attr.Shape = Shape.Circle;
        else
            node.Attr.Shape = Shape.Box;

        if (!state.isEndState) return;

        node.Attr.Shape = Shape.DoubleCircle;
        node.Attr.FillColor = Microsoft.Msagl.Drawing.Color.Red;

    }

}