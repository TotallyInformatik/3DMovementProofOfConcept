using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class GraphCanvas : VisualElement
{
    public List<VertexElement> vertexElements = new List<VertexElement>();
    public List<Edge> edges = new List<Edge>();
    public List<VertexElement> selectedVertices = new List<VertexElement>();
    private Edge _selectedEdge;
    public VertexElement selectedVertex
    {
        get => _selectedVertex;
        set
        {
            if (_selectedVertex != null) _selectedVertex.SetSelected(false);
            _selectedVertex = value;
            if (_selectedVertex != null) _selectedVertex.SetSelected(true);
            OnSelectionChanged?.Invoke();
        }
    }

    private VertexElement _selectedVertex;

    public Edge selectedEdge
    {
        get => _selectedEdge;
        set
        {
            _selectedEdge = value;
            OnSelectionChanged?.Invoke();
            MarkDirtyRepaint();
        }
    }
    public event System.Action OnSelectionChanged;

    public GraphCanvas()
    {
        style.flexGrow = 1;
        generateVisualContent += OnGenerateVisualContent;
        RegisterCallback<MouseDownEvent>(OnCanvasMouseDown);
    }

    public void AddVertex(Vertex vertex)
    {
        var vertexElement = new VertexElement(vertex, ToggleVertexSelection);
        vertexElements.Add(vertexElement);
        Add(vertexElement);
        MarkDirtyRepaint();
    }

    public void RemoveVertex(VertexElement vertexElement)
    {
        edges.RemoveAll(e => e.vertex1 == vertexElement.vertex || e.vertex2 == vertexElement.vertex);
        vertexElements.Remove(vertexElement);
        Remove(vertexElement);
        MarkDirtyRepaint();
    }

    public void AddEdge(Edge edge)
    {
        edges.Add(edge);
        MarkDirtyRepaint();
    }

    public void RemoveEdge(Edge edge)
    {
        edges.Remove(edge);
        MarkDirtyRepaint();
    }

    public void ClearGraph()
    {
        // Remove all vertex elements from the UI
        foreach (var vertex in vertexElements)
        {
            Remove(vertex as VisualElement); // Adjust if Vertex isn’t a VisualElement
        }
        vertexElements.Clear();
        edges.Clear();
        selectedVertex = null;
        selectedEdge = null;
        MarkDirtyRepaint();
    }

    private void ToggleVertexSelection(VertexElement vertexElement)
    {
        if (selectedVertices.Contains(vertexElement))
        {
            selectedVertices.Remove(vertexElement);
            vertexElement.style.backgroundColor = Color.red;
        }
        else
        {
            selectedVertices.Add(vertexElement);
            vertexElement.style.backgroundColor = Color.blue;
        }

        if (selectedVertices.Count == 2)
        {
            var v1 = selectedVertices[0].vertex;
            var v2 = selectedVertices[1].vertex;
            CreateEdge(v1, v2, "New Edge");
            foreach (var v in selectedVertices)
            {
                v.style.backgroundColor = Color.red;
            }
            selectedVertices.Clear();
        }
    }

    public void CreateEdge(Vertex v1, Vertex v2, string name)
    {
        edges.Add(new Edge { vertex1 = v1, vertex2 = v2, name = name });
        MarkDirtyRepaint();
    }

    private void OnCanvasMouseDown(MouseDownEvent evt)
    {
        if (evt.target != this) return; // Ignore clicks on child elements (vertices)

        Vector2 mousePos = evt.localMousePosition;
        bool edgeFound = false;
        foreach (var edge in edges)
        {
            Vector2 p1 = edge.vertex1.position + new Vector2(5f, 5f);
            Vector2 p2 = edge.vertex2.position + new Vector2(5f, 5f);
            if (IsPointNearLine(mousePos, p1, p2, 5f))
            {
                selectedEdge = edge;
                edgeFound = true;
                break;
            }
        }
        if (!edgeFound)
        {
            selectedEdge = null;
            if (selectedVertex != null)
            {
                _selectedVertex.SetSelected(false);
                _selectedVertex = null;
            }
        }
    }

    private bool IsPointNearLine(Vector2 point, Vector2 lineStart, Vector2 lineEnd, float threshold)
    {
        float lineLength = Vector2.Distance(lineStart, lineEnd);
        if (lineLength == 0) return Vector2.Distance(point, lineStart) < threshold;

        float t = Mathf.Clamp01(Vector2.Dot(point - lineStart, lineEnd - lineStart) / (lineLength * lineLength));
        Vector2 projection = lineStart + t * (lineEnd - lineStart);
        return Vector2.Distance(point, projection) < threshold;
    }

    private void OnGenerateVisualContent(MeshGenerationContext mgc)
    {
        var painter = mgc.painter2D;
        painter.lineWidth = 2f;
        Vector2 offset = new Vector2(5f, 5f);

        foreach (var edge in edges)
        {
            Vector2 start = edge.vertex1.position + offset;
            Vector2 end = edge.vertex2.position + offset;

            painter.strokeColor = (edge == selectedEdge) ? Color.green : Color.black;
            painter.BeginPath();
            painter.MoveTo(start);
            painter.LineTo(end);
            painter.Stroke();
        }
    }
}