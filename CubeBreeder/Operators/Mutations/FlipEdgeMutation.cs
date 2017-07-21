using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder.Operators.Mutations
{
    /// <summary>
    /// Flip Edge Mutation Operator
    /// </summary>
    class FlipEdgeMutation : Operator
    {
        double mutationProbability;
        double bitFlipProbability;

        Random rng;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mutationProbability">probability of mutation</param>
        /// <param name="bitFlipProbability">probability of flipping each edge</param>
        public FlipEdgeMutation(double mutationProbability, double bitFlipProbability, Random rnd)
        {
            this.mutationProbability = mutationProbability;
            this.bitFlipProbability = bitFlipProbability;
            rng = rnd;
        }

        public string ToLog()
        {
            return "FlipEdge\tprob:\t" + String.Format("{0:f2}", mutationProbability) + "\tbitProb:\t" + String.Format("{0:f2}", bitFlipProbability);
        }

        /// <summary>
        /// Each generation update method
        /// </summary>
        public void Update()
        {
        }

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
                    int length = p1.Length();

                    byte activity;

                    foreach (Edge edge in Program.graph.GetEdges())
                    {
                        if (rng.NextDouble() < bitFlipProbability)
                        {
                            // swap values
                            activity = p1.IsActiveOnEdge(edge.ID);
                            if (Settings.task != "eds") activity = (byte)(activity > 0 ? 0 : 1);
                            // if doing eds, dont just swap, but choose random colour
                            else activity = rng.NextByte(Settings.maxColours + 1);
                            o1.SetActivityOnEdge(edge.ID, activity);
                            o1.changed = true;
                        }
                    }
                }
                offspring.Add(o1);
            }
        }
    }
}
