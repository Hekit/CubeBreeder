using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder
{
    public class Individual : ICloneable
    {
        double fitnessValue;
        double objectiveValue;

        static int best = Int32.MaxValue;
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

        public Individual(GraphInfo graph)
        {
            this.cubeDimension = graph.GetDimension();
            int edgeCount = (int)Math.Pow(2, this.cubeDimension - 1) * this.cubeDimension;
            this.graph = graph;
            this.edgeActivity = new byte[edgeCount];
            this.vertexCount = (int)Math.Pow(2, this.cubeDimension);
        }

        /**
         * Sets the objective value
         *
         * @param objective The objective value which shall be set.
         */
        public void SetObjectiveValue(double objective)
        {
            this.objectiveValue = objective;
        }

        /**
         * Returns the objective value of the individual.
         *
         * @return The objective value of the individual.
         */
        public double GetObjectiveValue()
        {
            return objectiveValue;
        }

        /**
         * Sets the fitness value of the individual.
         *
         * @param fitness The fitness value of the individual which shall be set.
         */
        public void SetFitnessValue(double fitness)
        {
            fitnessValue = fitness;
        }

        /**
         * Returns the fitness value of the individual.
         *
         * @return The fitness value of the individual.
         */
        public double GetFitnessValue()
        {
            if (fitnessValue == -Double.MaxValue)
                throw new Exception("Fitness value not evaluated");
            return fitnessValue;
        }

        public void SetColourCount(int colours)
        {
            this.colours = colours;
        }


        public int GetColourCount()
        {
            return colours;
        }

        /**
         * Performs a deep copy of the individual. Resets the fitness value to
         * non-evaluated.
         *
         * @return The deep copy of the individual.
         */
        public Object Clone()
        {
            Individual n = new Individual(this);
            for (int i = 0; i < edgeActivity.Length; i++)
            {
                n.edgeActivity[i] = this.edgeActivity[i];
            }
            //changed = true;
            return n;
            //return MemberwiseClone();
        }


        public int Length()
        {
            return edgeActivity.Length;
        }

        public int GetCubeDimension()
        {
            return cubeDimension;
        }

        public static System.IO.StreamReader file = null;

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

        public void SetActivityBetweenVertices(int i1, int i2, byte value)
        {
            edgeActivity[graph.GetID(i1, i2)] = value;
            //changed = true;
        }

        public void SetActivityOnEdge(int edgeID, byte value)
        {
            edgeActivity[edgeID] = value;
            //changed = true;
        }

        public bool IsActiveBetweenVertices(int i1, int i2)
        {
            return edgeActivity[graph.GetID(i1, i2)].IsTrue();
        }

        public byte IsActiveOnEdge(int edgeID)
        {
            return edgeActivity[edgeID];
        }

        public int GetActiveEdgeCount()
        {
            return GetActiveEdgeCount(1);
        }

        public int GetActiveEdgeCount(byte c)
        {
            return edgeActivity.Count(x => x == c);
        }

        public int GetActiveEdgeCountColourblind()
        {
            return edgeActivity.Count(x => x > 0);
        }

        public double Is_Good_Enough(byte colour)
        {
            return IsSpanner(colour);
        }

        public double Is_3_Spanner(bool counting)
        {
            int detouredCount = 0;
            int nonDetouredCount = 0;
            int activeCount = 0;
            //if (IsSpanner(1) == 1) return 0;

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
                    //return 1;
                }
            }
            if (nonDetouredCount == 0)
            {
                /*
                // output of the so-far best achieved individual
                if (this.GetObjectiveValue() < best && this.GetObjectiveValue() != 0)
                {
                    best = (int)this.GetObjectiveValue();
                    Tools.WriteIndividual(this);
                }*/
                if (counting) Program.localDetourSpanners++;
            }
            //return 0;
            if (detouredCount + nonDetouredCount == 0) return 1.0;
            //else return (double)detouredCount / (detouredCount + nonDetouredCount + activeCount);
            else return (double)detouredCount / (detouredCount + nonDetouredCount);
        }

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

        public List<Edge> GetUndetoured()
        {
            return GetUndetoured(1);
        }

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

        // returns 1 when the graph is not connected
        // it is then used in the fitness function as a multiplier
        public double IsSpanner(byte colour)
        {
            return 1.0 / CountComponents(colour);
            if (CountComponents(colour) > 1) return 1;
            else return 0;
        }

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
