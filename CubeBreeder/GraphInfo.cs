using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder
{
    /// <summary>
    /// Class for representation of a hypercube graph
    /// </summary>
    [Serializable]
    public class GraphInfo
    {
        private static GraphInfo theInstance = null;

        private Edge[][] graph;
        private int dimension;
        private int vertexCount;
        private int edgeCount;
        private Edge[] edges;
        int edgeID = 0;

        /// <summary>
        /// Get an instance of the graph 
        /// </summary>
        /// <param name="dim">dimension of the graph</param>
        /// <returns>the instance of the graph</returns>
        public static GraphInfo GetInstance(int dim)
        {
            if (theInstance == null)
            {
                try
                {
                    theInstance = LoadFullCube(dim);
                }
                catch
                {
                    theInstance = new GraphInfo(dim);
                }
            }
            return theInstance;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dim">dimension of the graph</param>
        private GraphInfo(int dim)
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

        /// <summary>
        /// Get the dimension of the graph
        /// </summary>
        /// <returns>dimension</returns>
        public int GetDimension() { return dimension; }

        /// <summary>
        /// Get all edges
        /// </summary>
        /// <returns>all edges</returns>
        public Edge[] GetEdges() { return edges; }

        /// <summary>
        /// Get ID of the edge i1i2
        /// </summary>
        /// <param name="i1">vertex i1</param>
        /// <param name="i2">vertex i2</param>
        /// <returns>ID of the edge between i1 and i2</returns>
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

        /// <summary>
        /// Get the edge with ID
        /// </summary>
        /// <param name="ID">ID of the edge</param>
        /// <returns>edge with ID</returns>
        public Edge GetEdge(int ID)
        {
            return edges[ID];
        }

        /// <summary>
        /// Computes all detours for all edges in the hypercube graph
        /// </summary>
        public void ComputeDetours()
        {
            Console.WriteLine("Detours are being computed");
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
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates the whole hypercube graph representation
        /// </summary>
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

        /// <summary>
        /// Adds to the graph the edge v1v2
        /// </summary>
        /// <param name="v1">vertex v1</param>
        /// <param name="v2">vertex v2</param>
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

        /// <summary>
        /// Get an edge between vertices v1 and v2
        /// </summary>
        /// <param name="v1">vertex v1</param>
        /// <param name="v2">vertex v2</param>
        /// <returns>Edge v1v2</returns>
        public Edge GetEdge(int v1, int v2)
        {
            for (int i = 0; i < dimension; i++)
            {
                if (graph[v1][i].Vertex2 == v2) return graph[v1][i];
            }
            return null;
        }

        /// <summary>
        /// Gets all the edges starting in a vertex with ID v
        /// </summary>
        /// <param name="v">ID of a vertex</param>
        /// <returns>array of Edges</returns>
        public Edge[] GetEdgesInVertex(int v)
        {
            return graph[v];
        }

        /// <summary>
        /// Saves the GraphInfo to hard drive.
        /// </summary>
        private void SaveFullCube()
        {
            string filePath = "../../Resources/fullcube_" + dimension;
            WriteToBinaryFile<GraphInfo>(filePath, this);
        }

        /// <summary>
        /// Writes the hypercube graph to a file
        /// </summary>
        /// <typeparam name="T">GraphInfo</typeparam>
        /// <param name="filePath">path to the file</param>
        /// <param name="objectToWrite">GraphInfo</param>
        private void WriteToBinaryFile<T>(string filePath, T objectToWrite)
        {
            using (FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            using (GZipStream gzs = new GZipStream(stream, CompressionMode.Compress))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(gzs, objectToWrite);
            }
        }

        /// <summary>
        /// Loads hypercube structure from harddrive
        /// </summary>
        /// <param name="dimension">dimension of the hypercube</param>
        /// <returns>hypercube graph</returns>
        public static GraphInfo LoadFullCube(int dimension)
        {
            string filePath = "../../Resources/fullcube_" + dimension;
            return ReadFromBinaryFile<GraphInfo>(filePath);
        }

        /// <summary>
        /// Reads the binary file of a saved hypercube graph
        /// </summary>
        /// <typeparam name="T">GraphInfo</typeparam>
        /// <param name="filePath">path to the file</param>
        /// <returns>deserialized GraphInfo</returns>
        private static T ReadFromBinaryFile<T>(string filePath)
        {
            using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (GZipStream gzs = new GZipStream(stream, CompressionMode.Decompress))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                return (T)binaryFormatter.Deserialize(gzs);
            }
        }
    }
}
