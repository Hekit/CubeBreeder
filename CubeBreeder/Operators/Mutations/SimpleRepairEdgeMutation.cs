using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder.Operators.Mutations
{
    /// <summary>
    /// Simple Repair Edge Mutation Operator
    /// </summary>
    class SimpleRepairEdgeMutation : Operator
    {
        double mutationProbability;
        double bitFlipProbability;

        RandomNumberGenerator rng = RandomNumberGenerator.GetInstance();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mutationProbability">probability of mutation</param>
        /// <param name="bitFlipProbability">probability of mutation per edge</param>
        public SimpleRepairEdgeMutation(double mutationProbability, double bitFlipProbability)
        {
            this.mutationProbability = mutationProbability;
            this.bitFlipProbability = bitFlipProbability;
        }

        /// <summary>
        /// Each generation update method
        /// </summary>
        public void Update() { }

        /// <summary>
        /// Operator operate method
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
                    // any nondetoured edge might get activated
                    List<Edge> nondetouredEdges = p1.GetUndetoured();
                    if (nondetouredEdges.Count > 0)
                    {
                        foreach (var edge in Program.graph.GetEdges())
                        {
                            if (rng.NextDouble() < bitFlipProbability
                                && nondetouredEdges.Contains(edge))
                            {
                                o1.SetActivityOnEdge(edge.ID, 1);
                                o1.changed = true;
                            }
                        }
                    }
                }
                offspring.Add(o1);
            }
        }
    }
}
