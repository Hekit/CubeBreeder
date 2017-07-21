using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder.Operators.Crossovers
{
    /// <summary>
    /// Subcube Swap Crossover Operator
    /// </summary>
    class SubcubeSwapXOver : Operator
    {
        double xOverProb = 0;
        int subCubeSize = 3;

        Random rng;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="prob">probability of crossover</param>
        public SubcubeSwapXOver(double prob, Random rnd)
        {
            xOverProb = prob;
            rng = rnd;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="prob">probability of crossover</param>
        /// <param name="subcube">size of subcube</param>
        public SubcubeSwapXOver(double prob, int subcube, Random rnd)
        {
            xOverProb = prob;
            subCubeSize = subcube;
            rng = rnd;
        }

        public string ToLog()
        {
            return "SubcubeSwap pc: " + xOverProb + " subcube: " + subCubeSize;
        }

        /// <summary>
        /// Each generation update method.
        /// </summary>
        public void Update()
        {
            if (Settings.changingSubcube > 0)
            {
                // note that the size might change up and down (=not at all) in the same generation
                if (rng.NextDouble() < Settings.changingSubcube && subCubeSize > 2) subCubeSize--;
                if (rng.NextDouble() < Settings.changingSubcube && subCubeSize < Settings.subCubeMaxSize) subCubeSize++;
            }
        }

        /// <summary>
        /// The operator operate method.
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

                if (subCubeSize <= p1.GetCubeDimension() && 
                    rng.NextDouble() < xOverProb)
                {
                    // choosing the indices to be fixed and the respective values
                    bool[] fix = new bool[p1.GetCubeDimension()];
                    byte[] vals = new byte[fix.Length];

                    for (int j = 0; j < fix.Length; j++)
                    {
                        fix[j] = false;
                    }

                    for (int j = 0; j < p1.GetCubeDimension() - subCubeSize; j++)
                    {
                        int val = rng.NextInt(fix.Length);
                        while (fix[val] == true)
                        {
                            val = rng.NextInt(fix.Length);
                        }
                        fix[val] = true;
                        vals[val] = rng.NextByte(2); // 0 or 1
                    }
                    
                    // swapping subcubes
                    int testValue;
                    int length = p1.Length();
                    foreach (var e in Program.graph.GetEdges())
                    {
                        byte parent1Value = p1.IsActiveOnEdge(e.ID);
                        byte parent2Value = p2.IsActiveOnEdge(e.ID);

                        if (parent1Value != parent2Value)
                        {
                            testValue = Tools.TestSubcube(fix, vals, e);

                            if (testValue == -1)
                            {
                                // both outside, therefore nothing happens
                            }
                            else if (testValue == 0) 
                            {
                                // on the edge
                                byte value = parent1Value >= 1 ? parent1Value : parent2Value;
                                o1.SetActivityOnEdge(e.ID, value);
                                o2.SetActivityOnEdge(e.ID, value);
                            }
                            else 
                            {
                                // inside, therefore we swap
                                o1.SetActivityOnEdge(e.ID, parent2Value);
                                o2.SetActivityOnEdge(e.ID, parent1Value);
                            }
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
