using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder.Operators.Crossovers
{
    /// <summary>
    /// N-point Crossover Operator.
    /// </summary>
    class NPointXOver : Operator
    {
        double xOverProb = 0;
        int numberOfPoints = 2;

        Random rng;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="prob">probability of crossover</param>
        public NPointXOver(double prob, Random rnd)
        {
            xOverProb = prob;
            rng = rnd;
        }

        public string ToLog()
        {
            return "Npoint\tprob:\t" + String.Format("{0:f2}", xOverProb) + "\tpoints:\t" + numberOfPoints;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="prob">probability of crossover</param>
        /// <param name="points">number of crossover points</param>
        public NPointXOver(double prob, int points, Random rnd)
        {
            xOverProb = prob;
            numberOfPoints = points;
            rng = rnd;
        }

        /// <summary>
        /// Each generation update method.
        /// </summary>
        public void Update() { }

        /// <summary>
        /// Operator's operate method.
        /// </summary>
        /// <param name="parents">parents</param>
        /// <param name="offspring">offspring</param>
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

                    // select crossover points
                    while (indices.Count < numberOfPoints)
                        indices.Add(rng.NextInt(p1.Length()));

                    // perform the crossover
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
