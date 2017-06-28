using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder.Operators.Mutations
{
    class CleverRepairEdgeMutation : Operator
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
        public CleverRepairEdgeMutation(double mutationProbability, double bitFlipProbability)
        {
            this.mutationProbability = mutationProbability;
            this.bitFlipProbability = bitFlipProbability;
        }

        public void Update()
        {
            //mutationProbability -= 0.001;
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
                    List<Edge> nondetouredEdges = p1.GetUndetoured();
                    Dictionary<Edge, int> repairs = new Dictionary<Edge, int>();
                    int max = 0;
                    if (nondetouredEdges.Count > 0)
                    {
                        foreach (var edge in nondetouredEdges)
                        {
                            p1.SetActivityOnEdge(edge.ID, 1);
                            int count = p1.GetUndetoured().Count();
                            repairs.Add(edge, count);
                            if (count > max) max = count;
                            p1.SetActivityOnEdge(edge.ID, 0);
                        }
                        foreach (var edge in nondetouredEdges)
                        {
                            if (/*rng.NextDouble() < bitFlipProbability 
                                &&*/ repairs[edge] == max)
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
