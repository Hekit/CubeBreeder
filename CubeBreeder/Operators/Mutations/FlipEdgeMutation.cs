using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder.Operators.Mutations
{
    class FlipEdgeMutation : Operator
    {
        double mutationProbability;
        double bitFlipProbability;

        RandomNumberGenerator rng = RandomNumberGenerator.GetInstance();

        /**
         * Constructor, sets the probabilities.
         * 
         * @param mutationProbability probability of mutating each individual
         * @param bitFlipProbability probability of flipping a bit in the mutated individual
         */

        public FlipEdgeMutation(double mutationProbability, double bitFlipProbability)
        {
            this.mutationProbability = mutationProbability;
            this.bitFlipProbability = bitFlipProbability;
        }

        public void Update()
        {
            //mutationProbability += 0.00001;
        }

        public void Operate(Population parents, Population offspring)
        {

            int size = parents.GetPopulationSize();

            for (int i = 0; i < size; i++)
            //Parallel.For(0, size, i =>
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
                            activity = p1.IsActiveOnEdge(edge.ID);
                            // swap values
                            activity = (byte)(activity > 0 ? 0 : 1);
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
