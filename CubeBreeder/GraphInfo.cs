using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder
{
    [Serializable]
    public class GraphInfo
    {
        private Edge[][] graph;
        private int dimension;
        private int vertexCount;
        private int edgeCount;
        private Edge[] edges;
        int edgeID = 0;

        public GraphInfo(int dim)
        {
            dimension = dim;
            edgeCount = (int)Math.Pow(2, dim - 1) * dim;
            vertexCount = (int)Math.Pow(2, dim);

            graph = new Edge[vertexCount][];
            for (int i = 0; i < vertexCount; i++)
            {
                graph[i] = new Edge[dim];
            }
            edges = new Edge[edgeCount];
            CreateFullCube();
        }

        public int GetDimension() { return dimension; }

        public Edge[] GetEdges() { return edges; }

        public int GetID(int i1, int i2)
        {
            foreach (var edge in graph[i1])
            {
                if (((edge.Vertex1 == i1 && edge.Vertex2 == i2)
                    || (edge.Vertex1 == i2 && edge.Vertex2 == i1)))
                    return edge.ID;
            }
            return -1;
        }

        public Edge GetEdge(int ID)
        {
            return edges[ID];
        }

        public Edge[] GetVertex(int vertexIdx)
        {
            return graph[vertexIdx];
        }

        public void ComputeDetours()
        {
            foreach (var edge in edges)
            {
                var edgesFromV1 = graph[edge.Vertex1];
                var edgesToV2 = graph[edge.Vertex2];

                foreach (var e1 in edgesFromV1)
                {
                    foreach (var e3 in edgesToV2)
                    {
                        int from = e1.Vertex1 == edge.Vertex1 ? e1.Vertex2 : e1.Vertex1;
                        int to = e3.Vertex1 == edge.Vertex2 ? e3.Vertex2 : e3.Vertex1;

                        Edge e2 = from < to ? GetEdge(from, to) : GetEdge(to, from);

                        if (e2 != null && e1 != e2 && e2 != e3 && e1 != e3)
                        {
                            edge.AddDetourMe(e1.ID, e2.ID, e3.ID);
                            e1.AddDetour(edge);
                            e2.AddDetour(edge);
                            e3.AddDetour(edge);
                        }
                    }
                }
            }
        }

        private void CreateFullCube()
        {
            for (int i = 0; i < vertexCount; i++)
            {
                for (int j = i + 1; j < vertexCount; j++)
                {
                    if (Tools.Distance(i, j) == 1)
                    {
                        AddEdge(i, j);
                    }
                }
            }
            ComputeDetours();
            SaveFullCube();
        }

        private void AddEdge(int v1, int v2)
        {
            Edge e = new Edge(edgeID);
            edgeID++;
            e.Vertex1 = v1;
            e.Vertex2 = v2;
            int idx = Tools.DifferenceIndex(v1, v2);
            graph[v1][idx] = e;
            graph[v2][idx] = e;
            edges[e.ID] = e;
        }

        public Edge GetEdge(int v1, int v2)
        {
            for (int i = 0; i < dimension; i++)
            {
                if (graph[v1][i].Vertex2 == v2) return graph[v1][i];
            }
            return null;
        }

        public List<Edge> GetEdgesInVertex(int v)
        {
            return graph[v].ToList();
        }

        private void SaveFullCube()
        {
            string filePath = "../../Resources/fullcube_" + dimension;
            WriteToBinaryFile<GraphInfo>(filePath, this);
        }

        private void WriteToBinaryFile<T>(string filePath, T objectToWrite, bool append = false)
        {
            using (Stream stream = File.Open(filePath, append ? FileMode.Append : FileMode.Create))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(stream, objectToWrite);
            }
        }

        public static GraphInfo LoadFullCube(int dimension)
        {
            string filePath = "../../Resources/fullcube_" + dimension;
            return ReadFromBinaryFile<GraphInfo>(filePath);
        }

        private static T ReadFromBinaryFile<T>(string filePath)
        {
            using (Stream stream = File.Open(filePath, FileMode.Open))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                return (T)binaryFormatter.Deserialize(stream);
            }
        }
    }
}
