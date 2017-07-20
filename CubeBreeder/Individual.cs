using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder
{
    /// <summary>
    /// Representation of an individual
    /// </summary>
    public class Individual : ICloneable
    {
        double fitnessValue;
        double objectiveValue;

        int cubeDimension = 0;
        int vertexCount;
        public static long knownSize = 0;
        public bool elite = false;
        public bool changed = false;
        public double spanner = 0;
        int colours = 0;

        GraphInfo graph;
        byte[] edgeActivity;

        RandomNumberGenerator rnd = RandomNumberGenerator.GetInstance();

        public static StreamReader file = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="daddy">individual whose properties but not edgeActivity i taken</param>
        public Individual(Individual daddy)
        {
            this.cubeDimension = daddy.cubeDimension;
            this.vertexCount = daddy.vertexCount;
            this.graph = daddy.graph;
            this.changed = daddy.changed;
            this.spanner = daddy.spanner;
            this.fitnessValue = daddy.fitnessValue;
            this.objectiveValue = daddy.objectiveValue;
            this.edgeActivity = new byte[daddy.edgeActivity.Length];
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="graph">graph to indicate dimension of the individual</param>
        public Individual(GraphInfo graph)
        {
            this.cubeDimension = graph.GetDimension();
            int edgeCount = (int)Math.Pow(2, this.cubeDimension - 1) * this.cubeDimension;
            this.graph = graph;
            this.edgeActivity = new byte[edgeCount];
            this.vertexCount = (int)Math.Pow(2, this.cubeDimension);
        }

        /// <summary>
        /// Set the objective value of the individual
        /// </summary>
        /// <param name="objective">objective value to be set</param>
        public void SetObjectiveValue(double objective)
        {
            this.objectiveValue = objective;
        }

        /// <summary>
        /// Get the objective value of the individual
        /// </summary>
        /// <returns>objective value</returns>
        public double GetObjectiveValue()
        {
            return objectiveValue;
        }

        /// <summary>
        /// Set the fitness value of the individual
        /// </summary>
        /// <param name="fitness">fitness value to be set</param>
        public void SetFitnessValue(double fitness)
        {
            fitnessValue = fitness;
        }

        /// <summary>
        /// Get the fitness value of the individual
        /// </summary>
        /// <returns>fitness value</returns>
        public double GetFitnessValue()
        {
            if (fitnessValue == -Double.MaxValue)
                throw new Exception("Fitness value not evaluated");
            return fitnessValue;
        }

        /// <summary>
        /// Set the number of colours
        /// </summary>
        /// <param name="colours">number of colours</param>
        public void SetColourCount(int colours)
        {
            this.colours = colours;
        }

        /// <summary>
        /// Count the colours used
        /// </summary>
        /// <returns>colours used</returns>
        public int GetColourCount()
        {
            return colours;
        }

        /// <summary>
        /// Clone the individual (deep copy)
        /// </summary>
        /// <returns>new individual</returns>
        public Object Clone()
        {
            Individual n = new Individual(this);
            for (int i = 0; i < edgeActivity.Length; i++)
            {
                n.edgeActivity[i] = this.edgeActivity[i];
            }
            return n;
        }

        /// <summary>
        /// Get the length of the individual (= number of edges)
        /// </summary>
        /// <returns>length</returns>
        public int Length()
        {
            return edgeActivity.Length;
        }

        /// <summary>
        /// Get the dimension of the hypercube
        /// </summary>
        /// <returns>dimension</returns>
        public int GetCubeDimension()
        {
            return cubeDimension;
        }

        /// <summary>
        /// Initialize the individual according to a file 
        /// </summary>
        public void FileInitialization()
        {
            try
            {
                file = new System.IO.StreamReader(Settings.inputFolderPath + cubeDimension + "_init.txt");
                file.ReadLine();
                string s = file.ReadLine();
                int v1 = -1;
                int v2 = -1;
                while (s != null && s.Length > 0)
                {
                    v1 = Int32.Parse(s.Substring(0, s.IndexOf("\t")));
                    s = s.Substring(s.IndexOf("\t") + 1);
                    v2 = Int32.Parse(s.Substring(0, s.IndexOf("\t")));
                    for (int i = 0; i < edgeActivity.Length; i++)
                    {
                        SetActivityBetweenVertices(v1, v2, 1);
                    }
                    s = file.ReadLine();
                }
                file.Close();
            }
            catch
            {
                RandomInitialization(1);
            }
        }

        /// <summary>
        /// Initialize the individual randomly
        /// </summary>
        /// <param name="maxColours">maximal number of colours to be used</param>
        public void RandomInitialization(int maxColours)
        {
            int probability = Settings.activeProbability;

            for (int i = 0; i < edgeActivity.Length; i++)
            {
                if (rnd.NextInt(0, 100) < probability)
                {
                    edgeActivity[i] = (byte)(rnd.NextInt(1,maxColours+1));
                }
                else
                    edgeActivity[i] = 0;
            }
        }

        /// <summary>
        /// Set activity of edge i1i2
        /// </summary>
        /// <param name="i1">vertex i1</param>
        /// <param name="i2">vertex i2</param>
        /// <param name="value">value to be set</param>
        public void SetActivityBetweenVertices(int i1, int i2, byte value)
        {
            edgeActivity[graph.GetID(i1, i2)] = value;
        }

        /// <summary>
        /// Set the activity of edge edgeID
        /// </summary>
        /// <param name="edgeID">ID</param>
        /// <param name="value">value to be set</param>
        public void SetActivityOnEdge(int edgeID, byte value)
        {
            edgeActivity[edgeID] = value;
        }

        /// <summary>
        /// Chech if edge i1i2 is active
        /// </summary>
        /// <param name="i1">vertex i1</param>
        /// <param name="i2">vertex i2</param>
        /// <returns>true if active</returns>
        public bool IsActiveBetweenVertices(int i1, int i2)
        {
            return edgeActivity[graph.GetID(i1, i2)].IsTrue();
        }

        /// <summary>
        /// Check if edge with edgeID is active
        /// </summary>
        /// <param name="edgeID">ID</param>
        /// <returns>non-zero if active</returns>
        public byte IsActiveOnEdge(int edgeID)
        {
            return edgeActivity[edgeID];
        }

        /// <summary>
        /// Get active edge count for colour 1 only
        /// </summary>
        /// <returns>number of edges with colour 1</returns>
        public int GetActiveEdgeCount()
        {
            return GetActiveEdgeCount(1);
        }

        /// <summary>
        /// Get count of active edges with colour c
        /// </summary>
        /// <param name="c">colour</param>
        /// <returns>count of active edges with colour c</returns>
        public int GetActiveEdgeCount(byte c)
        {
            return edgeActivity.Count(x => x == c);
        }

        /// <summary>
        /// Get count of active edges regardless colour
        /// </summary>
        /// <returns>count of active edges</returns>
        public int GetActiveEdgeCountColourblind()
        {
            return edgeActivity.Count(x => x > 0);
        }

        /// <summary>
        /// Check for some property; part of EDS fitness
        /// </summary>
        /// <param name="colour">colour checked</param>
        /// <returns>double representing percentage of quality</returns>
        public double Is_Good_Enough(byte colour)
        {
            return IsSpanner(colour);
        }

        /// <summary>
        /// Check whether the individual is a 3-spanner
        /// </summary>
        /// <param name="counting">counting spanners for output</param>
        /// <returns>ratio of detoured to (detoured and nondetoured)</returns>
        public double Is_3_Spanner(bool counting)
        {
            int detouredCount = 0;
            int nonDetouredCount = 0;
            int activeCount = 0;
            // if really strict, uncomment next line
            //if (IsNotSpanner(1)) return 0;

            for (int i = 0; i < edgeActivity.Length; i++)
            {
                if (edgeActivity[i].IsTrue())
                {
                    activeCount++;
                    continue;
                }
                else if (IsDetoured(graph.GetEdge(i), 1))
                {
                    detouredCount++;
                    continue;
                }
                else
                {
                    nonDetouredCount++;
                }
            }
            if (nonDetouredCount == 0)
            {
                // uncomment the following to
                // output each so-far best achieved individual
                /*if (this.GetObjectiveValue() < best && this.GetObjectiveValue() != 0)
                {
                    best = (int)this.GetObjectiveValue();
                    Tools.WriteIndividual(this);
                }*/
                if (counting) Program.localDetourSpanners++;
            }
            if (detouredCount + nonDetouredCount == 0) return 1.0;
            else return (double)detouredCount / (detouredCount + nonDetouredCount);
        }

        /// <summary>
        /// Checks if edge e is detoured using colour
        /// </summary>
        /// <param name="e">edge to check</param>
        /// <param name="colour">checked colour</param>
        /// <returns>true if detoured</returns>
        private bool IsDetoured(Edge e, byte colour)
        {
            foreach (var tuple in e.GetDetours())
            {
                if (edgeActivity[tuple.Item1].IsTrue() && edgeActivity[tuple.Item2].IsTrue() 
                    && edgeActivity[tuple.Item3].IsTrue()
                    && edgeActivity[tuple.Item1] == colour 
                    && edgeActivity[tuple.Item2] == colour
                    && edgeActivity[tuple.Item3] == colour)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Gets all undetoured edges
        /// </summary>
        /// <returns>list of Edges</returns>
        public List<Edge> GetUndetoured()
        {
            return GetUndetoured(1);
        }

        /// <summary>
        /// Gets all undetoured edges of colour
        /// </summary>
        /// <param name="colour">colour checked</param>
        /// <returns>list of Edges</returns>
        public List<Edge> GetUndetoured(byte colour)
        {
            List<Edge> list = new List<Edge>();

            if (IsSpanner(colour) == 1) return list;

            for (int i = 0; i < edgeActivity.Length; i++)
            {
                if (!edgeActivity[i].IsTrue() && !IsDetoured(graph.GetEdge(i), colour))
                {
                    list.Add(graph.GetEdge(i));
                }
            }

            return list;
        }

        /// <summary>
        /// Checks if the individual is a spanner
        /// </summary>
        /// <param name="colour">colour checked</param>
        /// <returns>1 if a spanner</returns>
        public double IsSpanner(byte colour)
        {
            return 1.0 / CountComponents(colour);
            // for other option uncomment
            /*if (CountComponents(colour) > 1) return 1;
            else return 0;*/
        }

        /// <summary>
        /// Count the components of edges with colour
        /// </summary>
        /// <param name="colour">colour being used</param>
        /// <returns>number of components</returns>
        private int CountComponents(byte colour)
        {
            //Stavy jednotlivych uzlu
            int[] state = new int[vertexCount];

            for (int i = 0; i < vertexCount; i++) state[i] = 0;
            int counter = 0;
            //zajisti pruchod vsemi komponentami souvislosti
            for (int i = 0; i < vertexCount; i++)
            {
                if (state[i] == 0)
                {
                    counter++;
                    DFS(i, state, colour);
                }
            }
            return counter;
        }

        /// <summary>
        /// Check if the individual represents a spanning tree
        /// </summary>
        /// <param name="colour">colour being checked</param>
        /// <returns>true if a spanning tree</returns>
        private bool IsSpanningTree(byte colour)
        {
            //Stavy jednotlivych uzlu
            int[] state = new int[vertexCount];

            for (int i = 0; i < vertexCount; i++) state[i] = 0;

            DFS(0, state, colour);

            for (int i = 0; i < vertexCount; i++)
            {
                if (state[i] == 0)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Depth First Search for colouring components
        /// </summary>
        /// <param name="vertexNr">starting vertex</param>
        /// <param name="state">current states of vertices</param>
        /// <param name="colour">colour used</param>
        private void DFS(int vertexNr, int[] state, byte colour)
        {
            state[vertexNr] = 1;

            foreach (var e in graph.GetEdgesInVertex(vertexNr))
            {
                if (edgeActivity[e.ID].IsTrue() && edgeActivity[e.ID] == colour)
                {
                    int k = e.Vertex1 != vertexNr ? e.Vertex1 : e.Vertex2;
                    if (state[k] == 0) DFS(k, state, colour);
                }
            }
            state[vertexNr] = 2;
        }

        /// <summary>
        /// Convert the individual to string
        /// </summary>
        /// <returns>String representation of the individual</returns>
        public override string ToString()
        {
            string str;
            if (Settings.task != "eds")
                str = "Total of " + GetActiveEdgeCount() + " edges "
                + "with max degree " + GetMaxDegree() + "\n";
            else str = "Total of " + GetActiveEdgeCountColourblind() + " edges in " 
                    + objectiveValue + " edge-disjoint spanners\n";
            for (int i = 0; i < edgeActivity.Length; i++)
            {
                if (edgeActivity[i].IsTrue())
                {
                    Edge edge = graph.GetEdge(i);
                    str += edge.Vertex1 + "\t" + edge.Vertex2 + "\t";
                    foreach (var j in Tools.ToBinary(edge.Vertex1).Reverse())
                    {
                        str += j;
                    }
                    str += "\t";
                    foreach (var j in Tools.ToBinary(edge.Vertex2).Reverse())
                    {
                        str += j;
                    }
                    if (Settings.task == "eds") str += "\t" + edgeActivity[i];
                    str += "\n";
                }
            }
            return str;
        }

        /// <summary>
        /// Get the maximal degree in the individual
        /// </summary>
        /// <returns>maximal degree</returns>
        public int GetMaxDegree()
        {
            int maxDegree = Int32.MinValue;
            int vertexCount = (int)Math.Pow(2, cubeDimension);

            for (int i = 0; i < vertexCount; i++)
            {
                int count = 0;
                foreach (var edge in graph.GetEdgesInVertex(i))
                {
                    if (edgeActivity[edge.ID].IsTrue()) count++;
                }
                if (maxDegree < count) maxDegree = count;
            }
            return maxDegree;
        }

        /// <summary>
        /// Get the list of all degrees of vertices in the individual
        /// </summary>
        /// <returns>list of degrees</returns>
        public List<int> GetDegrees()
        {
            int vertexCount = (int)Math.Pow(2, cubeDimension);
            List<int> degrees = new List<int>();
            int count = 0;
            for (int i = 0; i < vertexCount; i++)
            {
                count = 0;
                foreach (var edge in graph.GetEdgesInVertex(i))
                {
                    if (edgeActivity[edge.ID].IsTrue()) count++;
                }
                degrees.Add(count);
            }
            return degrees;
        }

    }
}
