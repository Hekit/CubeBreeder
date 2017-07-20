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
    class CleverRepairEdgeMutation : Operator
    {
        double mutationProbability;

        RandomNumberGenerator rng = RandomNumberGenerator.GetInstance();

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="mutationProbability">mutation probability</param>
        public CleverRepairEdgeMutation(double mutationProbability)
        {
            this.mutationProbability = mutationProbability;
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
                    // get all undetoured edges
                    List<Edge> nondetouredEdges = p1.GetUndetoured();
                    // monitoring repairs
                    Dictionary<Edge, int> repairs = new Dictionary<Edge, int>();
                    int max = 0;
                    int counter = 0;
                    // if there is any nondetoured edge
                    if (nondetouredEdges.Count > 0)
                    {
                        foreach (var edge in nondetouredEdges)
                        {
                            // set activity, check number of detour then and set activity back
                            p1.SetActivityOnEdge(edge.ID, 1);
                            int count = p1.GetUndetoured().Count();
                            repairs.Add(edge, count);
                            if (count > max)
                            {
                                max = count;
                                counter = 1;
                            }
                            else if (count == max) counter++;
                            p1.SetActivityOnEdge(edge.ID, 0);
                        }
                        // find the edge that solves most problems
                        foreach (var edge in nondetouredEdges)
                        {
                            // with probability activate it (might be multiple)
                            if (rng.NextDouble() < 1 / counter 
                                && repairs[edge] == max)
                            {
                                o1.SetActivityOnEdge(edge.ID, 1);
                            }
                        }
                    }
                    o1.changed = true;
                }
                offspring.Add(o1);
            }
        }
    }
}
