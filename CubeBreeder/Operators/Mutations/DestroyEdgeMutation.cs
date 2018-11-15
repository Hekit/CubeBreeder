using System;

namespace CubeBreeder.Operators.Mutations
{

    /// <summary>
    /// Destroy Edge Mutation Operator
    /// </summary>
    class DestroyEdgeMutation : Operator
    {
        double mutationProbability;
        double bitFlipProbability;

        Random rng;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mutationProbability">probability of mutation</param>
        /// <param name="bitFlipProbability">probability of mutation per edge</param>
        /// <param name="rnd">random</param>
        public DestroyEdgeMutation(double mutationProbability, double bitFlipProbability, Random rnd)
        {
            this.mutationProbability = mutationProbability;
            this.bitFlipProbability = bitFlipProbability;
            rng = rnd;
        }

        public string ToLog()
        {
            return "Destroy\tprob:\t" + String.Format("{0:f2}", mutationProbability) + "\tbitProb:\t" + String.Format("{0:f2}", bitFlipProbability);
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
                    foreach (var edge in Program.graph.GetEdges())
                    {
                        if (rng.NextDouble() < bitFlipProbability && p1.IsActiveOnEdge(edge.ID) > 0)
                        {
                            o1.SetActivityOnEdge(edge.ID, 0);
                            o1.changed = true;
                        }
                    }
                }
                offspring.Add(o1);
            }
        }
    }
}