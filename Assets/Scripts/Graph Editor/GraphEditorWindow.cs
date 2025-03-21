using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class GraphEditorWindow : EditorWindow
{
    private GraphCanvas graphCanvas;
    private VisualElement propertiesPanel;


    [MenuItem("Window/Graph Editor")]
    public static void ShowWindow()
    {
        var window = GetWindow<GraphEditorWindow>("Graph Editor");
        window.Focus();
    }

    private void CreateGUI()
    {
        // Create the graph canvas
        graphCanvas = new GraphCanvas();
        rootVisualElement.Add(graphCanvas);

        // Add a button to create new vertices
        // In GraphEditorWindow.cs, inside CreateGUI method
        graphCanvas = new GraphCanvas();
        rootVisualElement.Add(graphCanvas);

        var loadButton = new Button(LoadGraph)
        {
            text = "L",
            style =
            {
                position = Position.Absolute,
                bottom = 10,
                right = 10,
                width = 50,
                height = 50,
                fontSize = 24,
                backgroundColor = new Color(0.2f, 0.8f, 0.4f), // Green
                unityTextAlign = TextAnchor.MiddleCenter
            }
        };
        rootVisualElement.Add(loadButton);

        // Save Button
        var saveButton = new Button(SaveGraph)
        {
            text = "S",
            style =
            {
                position = Position.Absolute,
                bottom = 10,
                right = 70, // Positioned 60px left of the + button (50px width + 10px gap)
                width = 50,
                height = 50,
                fontSize = 24,
                backgroundColor = new Color(0.8f, 0.4f, 0.2f), // Orange for contrast
                unityTextAlign = TextAnchor.MiddleCenter
            }
        };
        rootVisualElement.Add(saveButton);

        // Add Vertex Button
        var addButton = new Button(() =>
        {
            graphCanvas.AddVertex(new Vertex { position = new Vector2(100, 100), name = "New Vertex", scene = "Default" });
        })
        {
            text = "+",
            style =
            {
                position = Position.Absolute,
                bottom = 10,
                right = 130, // 60px left of save button (50px width + 10px gap)
                width = 50,
                height = 50,
                fontSize = 24,
                backgroundColor = new Color(0.2f, 0.6f, 0.8f),
                unityTextAlign = TextAnchor.MiddleCenter
            }
        };
        rootVisualElement.Add(addButton);

        // Add a properties panel for edge editing
        propertiesPanel = new VisualElement
        {
            style =
            {
                position = Position.Absolute,
                right = 0,
                top = 0,
                width = 250,
                backgroundColor = new Color(0.7f, 0.7f, 0.7f),
                paddingLeft = 10,
                paddingRight = 10,
                paddingTop = 10,
                paddingBottom = 10,
                borderLeftWidth = 1,
                borderLeftColor = new Color(0.5f, 0.5f, 0.5f)
            }
        };
        rootVisualElement.Add(propertiesPanel);
        propertiesPanel.visible = false;

        // Update the edge name field when the selected edge changes
        graphCanvas.OnSelectionChanged += UpdatePropertiesPanel;
    }

    private void OnGUI()
    {
        Event e = Event.current;
        if (e != null && e.type == EventType.KeyDown)
        {
            if (e.keyCode == KeyCode.Backspace)
            {
                if (graphCanvas.selectedVertex != null)
                {
                    graphCanvas.RemoveVertex(graphCanvas.selectedVertex);
                    graphCanvas.selectedVertex = null;
                }
                else if (graphCanvas.selectedEdge != null)
                {
                    graphCanvas.RemoveEdge(graphCanvas.selectedEdge);
                    graphCanvas.selectedEdge = null;
                }
                graphCanvas.MarkDirtyRepaint();
                e.Use(); // Mark event as handled
            }
        }
    }

    private void UpdatePropertiesPanel()
    {
        propertiesPanel.Clear();
        if (graphCanvas.selectedVertex != null)
        {
            propertiesPanel.Add(CreateVertexProperties(graphCanvas.selectedVertex));
        }
        else if (graphCanvas.selectedEdge != null)
        {
            propertiesPanel.Add(CreateEdgeProperties(graphCanvas.selectedEdge));
        }
        propertiesPanel.visible = graphCanvas.selectedVertex != null || graphCanvas.selectedEdge != null;
    }

    private VisualElement CreateVertexProperties(VertexElement vertexElement)
    {
        var container = new VisualElement();

        // Name field
        var nameLabel = new Label("Name") { style = { color = new Color(0.2f, 0.2f, 0.2f), marginBottom = 5 } };
        container.Add(nameLabel);
        var nameField = new TextField { value = vertexElement.vertex.name };
        nameField.style.width = 180; // Wider input
        nameField.style.color = new Color(0.2f, 0.2f, 0.2f); // Darker text
        nameField.style.marginBottom = 5;
        nameField.RegisterValueChangedCallback(evt => vertexElement.vertex.name = evt.newValue);
        container.Add(nameField);

        // Scene field
        var sceneLabel = new Label("Scene") { style = { color = new Color(0.2f, 0.2f, 0.2f), marginBottom = 5 } };
        container.Add(sceneLabel);
        var sceneField = new TextField { value = vertexElement.vertex.scene };
        sceneField.style.width = 180; // Wider input
        sceneField.style.color = new Color(0.2f, 0.2f, 0.2f); // Darker text
        sceneField.style.marginBottom = 5;
        sceneField.RegisterValueChangedCallback(evt => vertexElement.vertex.scene = evt.newValue);
        container.Add(sceneField);

        return container;
    }

    private VisualElement CreateEdgeProperties(Edge edge)
    {
        var container = new VisualElement();

        // Name field
        var nameLabel = new Label("Name") { style = { color = new Color(0.2f, 0.2f, 0.2f), marginBottom = 5 } };
        container.Add(nameLabel);
        var nameField = new TextField { value = edge.name };
        nameField.style.width = 180; // Wider input
        nameField.style.color = new Color(0.2f, 0.2f, 0.2f); // Darker text
        nameField.style.marginBottom = 5;
        nameField.RegisterValueChangedCallback(evt => edge.name = evt.newValue);
        container.Add(nameField);

        // Length (read-only)
        var lengthLabel = new Label($"Length: {edge.Length:F2}") { style = { color = new Color(0.2f, 0.2f, 0.2f), marginBottom = 5 } };
        container.Add(lengthLabel);

        // Biome dropdown
        var biomeLabel = new Label("Biome") { style = { color = new Color(0.2f, 0.2f, 0.2f), marginBottom = 5 } };
        container.Add(biomeLabel);
        var biomeField = new EnumField("", edge.biome);
        biomeField.style.width = 180; // Wider dropdown
        biomeField.style.color = new Color(0.2f, 0.2f, 0.2f); // Darker text
        biomeField.style.marginBottom = 5;
        biomeField.RegisterValueChangedCallback(evt => edge.biome = (Biome)evt.newValue);
        container.Add(biomeField);

        // Difficulty dropdown
        var difficultyLabel = new Label("Difficulty") { style = { color = new Color(0.2f, 0.2f, 0.2f), marginBottom = 5 } };
        container.Add(difficultyLabel);
        var difficultyField = new EnumField("", edge.difficulty);
        difficultyField.style.width = 180; // Wider dropdown
        difficultyField.style.color = new Color(0.2f, 0.2f, 0.2f); // Darker text
        difficultyField.style.marginBottom = 5;
        difficultyField.RegisterValueChangedCallback(evt => edge.difficulty = (Difficulty)evt.newValue);
        container.Add(difficultyField);

        return container;
    }

    private void SaveGraph()
    {
        // Create a new GraphData instance
        var graphData = ScriptableObject.CreateInstance<GraphData>();

        // Map vertices to indices for edge references
        var vertexIndexMap = new Dictionary<Vertex, int>();
        for (int i = 0; i < graphCanvas.vertexElements.Count; i++)
        {
            var vertexElement = graphCanvas.vertexElements[i];
            graphData.vertices.Add(new VertexData
            {
                name = vertexElement.vertex.name,
                position = vertexElement.vertex.position,
                scene = vertexElement.vertex.scene
            });
            vertexIndexMap[vertexElement.vertex] = i;
        }

        // Add edges with vertex indices
        foreach (var edge in graphCanvas.edges)
        {
            graphData.edges.Add(new EdgeData
            {
                vertex1Index = vertexIndexMap[edge.vertex1],
                vertex2Index = vertexIndexMap[edge.vertex2],
                name = edge.name,
                biome = edge.biome,
                difficulty = edge.difficulty
            });
        }

        // Save to asset
        string path = EditorUtility.SaveFilePanelInProject("Save Graph", "NewGraphData", "asset", "Save your graph data");
        if (!string.IsNullOrEmpty(path))
        {
            AssetDatabase.CreateAsset(graphData, path);
            AssetDatabase.SaveAssets();
            Debug.Log("Graph saved to: " + path);
        }
    }

    private void LoadGraph()
    {
        // Open file dialog to select a GraphData asset
        string path = EditorUtility.OpenFilePanel("Load Graph", "Assets", "asset");
        if (string.IsNullOrEmpty(path)) return; // User canceled the dialog

        // Convert absolute path to relative path (e.g., "Assets/...")
        path = "Assets" + path.Substring(Application.dataPath.Length);

        // Load the GraphData asset
        var graphData = AssetDatabase.LoadAssetAtPath<GraphData>(path);
        if (graphData == null)
        {
            Debug.LogError("Failed to load GraphData from: " + path);
            return;
        }

        // Clear the current graph
        graphCanvas.ClearGraph();

        // Map to store vertex indices to Vertex objects
        var vertexMap = new Dictionary<int, Vertex>();

        // Load vertices
        for (int i = 0; i < graphData.vertices.Count; i++)
        {
            var vertexData = graphData.vertices[i];
            var vertex = new Vertex
            {
                name = vertexData.name,
                position = vertexData.position,
                scene = vertexData.scene
            };
            graphCanvas.AddVertex(vertex);
            vertexMap[i] = vertex; // Map index to vertex for edge creation
        }

        // Load edges
        foreach (var edgeData in graphData.edges)
        {
            var vertex1 = vertexMap[edgeData.vertex1Index];
            var vertex2 = vertexMap[edgeData.vertex2Index];
            var edge = new Edge
            {
                vertex1 = vertex1,
                vertex2 = vertex2,
                name = edgeData.name,
                biome = edgeData.biome,
                difficulty = edgeData.difficulty
            };
            graphCanvas.AddEdge(edge);
        }

        Debug.Log("Graph loaded from: " + path);
    }
}