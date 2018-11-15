using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder.Operators.Mutations
{
    /// <summary>
    /// Clever Repair Edge Mutation Operator
    /// </summary>
    class CleverDestroyEdgeMutation : Operator
    {
        double mutationProbability;

        Random rng;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="mutationProbability">mutation probability</param>
        public CleverDestroyEdgeMutation(double mutationProbability, Random rnd)
        {
            this.mutationProbability = mutationProbability;
            rng = rnd;
        }

        public string ToLog()
        {
            return "CleverDestroy\tprob:\t" + String.Format("{0:f2}", mutationProbability);
        }

        /// <summary>
        /// Each generation update method
        /// </summary>
        public void Update()
        {
        }

        /// <summary>
        /// The operator operate method
        /// </summary>
        /// <param name="parents">parents</param>
        /// <param name="offspring">offspring</param>
        public void Operate(Population parents, Population offspring)
        {
            int size = parents.GetPopulationSize();

            for (int i = 0; i < size; i++)
            {
                Individual p1 = parents.Get(i);
                Individual o1 = (Individual)p1.Clone();

                if (rng.NextDouble() < mutationProbability)
                {
                    List<Edge> deleteableEdges = new List<Edge>();
                    Dictionary<Edge, int> destroyedDetours = new Dictionary<Edge, int>();
                    int min = Int32.MaxValue;
                    int counter = 0;
                    // if there is any nondetoured edge
                
                    foreach (var edge in Program.graph.GetEdges())
                    {
                        if (p1.IsActiveOnEdge(edge.ID) > 0)
                        {
                            p1.SetActivityOnEdge(edge.ID, 0);
                            int count = p1.GetUndetoured().Count();
                            destroyedDetours.Add(edge, count);
                            p1.SetActivityOnEdge(edge.ID, 1);
                            deleteableEdges.Add(edge);
                            if (count < min)
                            {
                                min = count;
                                counter = 1;
                            }
                            else if (count == min) counter++;
                        }
                    }
                    
                    // find the edge that creates least problems
                    foreach (var edge in deleteableEdges)
                    {
                        // with probability activate it (might be multiple)
                        if (rng.NextDouble() < (1.0 / counter) && destroyedDetours[edge] == min)
                        {
                            o1.SetActivityOnEdge(edge.ID, 0);
                        }
                    }
                    o1.changed = true;
                }
                offspring.Add(o1);
            }
        }
    }
}
