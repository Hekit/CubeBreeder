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

        GraphInfo graph;
        bool[] edgeActivity;

        public Individual(Individual daddy)
        {
            this.cubeDimension = daddy.cubeDimension;
            this.vertexCount = daddy.vertexCount;
            this.graph = daddy.graph;
            this.edgeActivity = new bool[daddy.edgeActivity.Length];
        }

        public Individual(GraphInfo graph)
        {
            this.cubeDimension = graph.GetDimension();
            int edgeCount = (int)Math.Pow(2, this.cubeDimension - 1) * this.cubeDimension;
            this.graph = graph;
            this.edgeActivity = new bool[edgeCount];
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

        /**
         * Performs a deep copy of the individual. Resets the fitness value to
         * non-evaluated.
         *
         * @return The deep copy of the individual.
         */
        public Object Clone()
        {
            return MemberwiseClone();
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
            //if (file == null) 
            file = new System.IO.StreamReader(
                "D:\\Development\\hypercubes\\initialization\\" + cubeDimension + "_init.txt");
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
                    ActivateBetweenVertices(v1, v2);
                }
                s = file.ReadLine();
            }
            file.Close();
        }

        public void RandomInitialization()
        {
            RandomNumberGenerator rnd = RandomNumberGenerator.GetInstance();
            int probability = Properties.Settings.Default.P_ActiveProbability - 1;

            for (int i = 0; i < edgeActivity.Length; i++)
            {
                if (rnd.NextInt(0, 100) < probability)
                    edgeActivity[i] = true;
                else
                    edgeActivity[i] = false;
            }
        }

        private void ActivateBetweenVertices(int i1, int i2)
        {
            edgeActivity[graph.GetID(i1, i2)] = true;
        }

        private bool isActiveBetweenVertices(int i1, int i2)
        {
            return edgeActivity[graph.GetID(i1, i2)];
        }

        public int GetActiveEdgeCount()
        {
            return edgeActivity.Count(x => x == true);
        }

        public double Is_3_Spanner(bool counting)
        {
            int detouredCount = 0;
            int nonDetouredCount = 0;
            int activeCount = 0;
            if (IsSpanner() == 1) return 0;

            for (int i = 0; i < edgeActivity.Length; i++)
            {
                if (edgeActivity[i])
                {
                    activeCount++;
                    continue;
                }
                //else if (edge.IsDetoured()) continue;
                else if (IsDetoured(graph.GetEdge(i)))
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
                if (this.GetObjectiveValue() < best && this.GetObjectiveValue() != 0)
                {
                    best = (int)this.GetObjectiveValue();
                    Tools.WriteIndividual(this);
                }
                if (counting) Program.localDetourSpanners++;
            }
            //return 0;
            if (detouredCount + nonDetouredCount == 0) return 1.0;
            //else return (double)detouredCount / (detouredCount + nonDetouredCount + activeCount);
            else return (double)detouredCount / (detouredCount + nonDetouredCount);
        }

        private bool IsDetoured(Edge e)
        {
            foreach (var tuple in e.GetDetours())
            {
                if (edgeActivity[tuple.Item1] && edgeActivity[tuple.Item2] && edgeActivity[tuple.Item3])
                    return true;
            }
            return false;
        }

        public List<Edge> GetUndetoured()
        {
            List<Edge> list = new List<Edge>();

            if (IsSpanner() == 1) return list;

            for (int i = 0; i < edgeActivity.Length; i++)
            {
                if (!edgeActivity[i] && !IsDetoured(graph.GetEdge(i)))
                {
                    list.Add(graph.GetEdge(i));
                }
            }

            return list;
        }

        // returns 1 when the graph is not connected
        // it is then used in the fitness function as a multiplier
        public int IsSpanner()
        {
            if (CountComponents() > 1) return 1;
            else return 0;
        }

        private int CountComponents()
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
                    DFS(i, state);
                }
            }
            return counter;
        }

        private void DFS(int vertexNr, int[] state)
        {
            state[vertexNr] = 1;

            for (int i = 0; i < vertexCount; i++)
            {
                for (int j = 0; j < cubeDimension; j++)
                {
                    Edge e = graph.GetEdge(i, j);
                    if (edgeActivity[e.ID])
                    {
                        int k = e.Vertex1 != vertexNr ? e.Vertex1 : e.Vertex2;
                        if (state[k] == 0) DFS(k, state);
                    }
                }
            }
            state[vertexNr] = 2;
        }


        public override string ToString()
        {
            string str = "Total of " + GetActiveEdgeCount() + " edges\n";
            for (int i = 0; i < edgeActivity.Length; i++)
            {
                if (edgeActivity[i])
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
                for (int j = 0; j < cubeDimension; j++)
                {
                    if (edgeActivity[graph.GetEdge(i,j).ID]) count++;
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
                for (int j = 0; j < cubeDimension; j++)
                {
                    if (edgeActivity[graph.GetEdge(i, j).ID]) count++;
                }
                degrees.Add(count);
            }
            return degrees;
        }

    }
}
