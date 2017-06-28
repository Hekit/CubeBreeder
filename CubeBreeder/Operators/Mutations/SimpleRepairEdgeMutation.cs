using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder.Operators.Mutations
{
    class SimpleRepairEdgeMutation : Operator
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
        public SimpleRepairEdgeMutation(double mutationProbability, double bitFlipProbability)
        {
            this.mutationProbability = mutationProbability;
            this.bitFlipProbability = bitFlipProbability;
        }

        public void Update() { }

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
