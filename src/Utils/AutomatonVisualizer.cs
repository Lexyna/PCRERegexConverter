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

        CreateEdges(graph, entry);

        Microsoft.Msagl.GraphViewerGdi.GViewer viewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
        graph.Attr.LayerDirection = LayerDirection.LR;
        viewer.Graph = graph;

        graphForm.SuspendLayout();
        viewer.Dock = DockStyle.Fill;
        graphForm.Controls.Add(viewer);
        graphForm.ResumeLayout();
        graphForm.ShowDialog();

    }

    private void CreateEdges(Graph graph, State state)
    {

        state.visited = true;

        foreach (Transition t in state.GetOutgoingTransitions())
        {
            string lable = String.IsNullOrEmpty(t.symbol) ? "ε" : t.symbol;
            graph.AddEdge(state.id, lable, t.GetOutState().id);

            if (!t.GetOutState().visited)
                CreateEdges(graph, t.GetOutState());
        }

        if (!state.isEndState) return;

        Node endNode = graph.FindNode(state.id);
        endNode.Attr.Shape = Shape.DoubleCircle;

    }

}