using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder.Operators.Crossovers
{
    class NPointXOver : Operator
    {
        double xOverProb = 0;
        int numberOfPoints = 2;

        RandomNumberGenerator rng = RandomNumberGenerator.GetInstance();

        /**
            * Constructor, sets the probability of crossover
            * 
            * @param prob the probability of crossover
            */
        public NPointXOver(double prob)
        {
            xOverProb = prob;
        }

        public NPointXOver(double prob, int points)
        {
            xOverProb = prob;
            numberOfPoints = points;
        }

        public void Update() { }

        public void Operate(Population parents, Population offspring)
        {
            int size = parents.GetPopulationSize();

            for (int i = 0; i < size / 2; i++)
            {
                Individual p1 = parents.Get(2 * i);
                Individual p2 = parents.Get(2 * i + 1);

                Individual o1 = (Individual)p1.Clone();
                Individual o2 = (Individual)p2.Clone();

                if (rng.NextDouble() < xOverProb)
                {
                    HashSet<int> indices = new HashSet<int>();
                    bool flipper = true;

                    while (indices.Count < numberOfPoints)
                        indices.Add(rng.NextInt(p1.Length()));

                    for (int j = 0; j < p1.Length(); j++)
                    {
                        if (indices.Contains(j)) flipper = flipper == true ? false : true;

                        if (flipper)
                        {
                            o1.SetActivityOnEdge(j, p1.IsActiveOnEdge(j));
                            o2.SetActivityOnEdge(j, p2.IsActiveOnEdge(j));
                        }
                        else
                        {
                            o1.SetActivityOnEdge(j, p2.IsActiveOnEdge(j));
                            o2.SetActivityOnEdge(j, p1.IsActiveOnEdge(j));
                        }
                    }
                    o1.changed = true;
                    o2.changed = true;
                }
                offspring.Add(o1);
                offspring.Add(o2);
            }
        }
    }
}
