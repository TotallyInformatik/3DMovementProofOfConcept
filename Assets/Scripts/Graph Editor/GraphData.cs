using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewGraphData", menuName = "Graph/GraphData")]
public class GraphData : ScriptableObject
{
    public List<VertexData> vertices = new List<VertexData>();
    public List<EdgeData> edges = new List<EdgeData>();
}

[System.Serializable]
public class VertexData
{
    public string name;
    public Vector2 position;
    public string scene;
}

[System.Serializable]
public class EdgeData
{
    public int vertex1Index;
    public int vertex2Index;
    public string name;
    public Biome biome;
    public Difficulty difficulty;
}

public class Vertex // Adjust based on your implementation
{
    public string name;
    public Vector2 position;
    public string scene;
}

public class Edge
{
    public Vertex vertex1;
    public Vertex vertex2;
    public string name;
    public Biome biome;
    public Difficulty difficulty;
    public float Length => Vector2.Distance(vertex1.position, vertex2.position);
}

public enum Biome { Forest, Plains, Desert, Mountain }
public enum Difficulty { Easy, Medium, Hard }