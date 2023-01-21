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

        CreateEdges(graph, entry, true);

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

    private void CreateEdges(Graph graph, State state, bool start)
    {

        state.visited = true;

        foreach (Transition t in state.GetOutgoingTransitions())
        {
            string label = String.IsNullOrEmpty(t.symbol) ? "ε" : t.symbol;
            graph.AddEdge(state.id, label, t.GetOutState().id);

            Node left = graph.FindNode(state.id);
            Node right = graph.FindNode(t.GetOutState().id);

            left.Attr.Shape = Shape.Circle;
            right.Attr.Shape = Shape.Circle;

            if (!t.GetOutState().visited)
                CreateEdges(graph, t.GetOutState(), false);
        }

        if (!state.isEndState) return;

        Node endNode = graph.FindNode(state.id);
        endNode.Attr.Shape = Shape.DoubleCircle;
        endNode.Attr.FillColor = Microsoft.Msagl.Drawing.Color.Red;

    }

}