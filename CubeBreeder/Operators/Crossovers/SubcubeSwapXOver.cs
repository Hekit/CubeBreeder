using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder.Operators.Crossovers
{
    class SubcubeSwapXOver : Operator
    {
        double xOverProb = 0;
        int subCubeSize = 3;
        RandomNumberGenerator rng = RandomNumberGenerator.GetInstance();

        /**
         * Constructor, sets the probability of crossover
         * 
         * @param prob the probability of crossover
         */
        public SubcubeSwapXOver(double prob)
        {
            xOverProb = prob;
        }

        public SubcubeSwapXOver(double prob, int subcube)
        {
            xOverProb = prob;
            subCubeSize = subcube;
            //subCubeSize = rng.NextInt(1, Properties.Settings.Default.Dimension);
        }

        public void Update()
        {
            if (Settings.changingSubcube > 0)
            {
                if (rng.NextDouble() < Settings.changingSubcube && subCubeSize > 2) subCubeSize--;
                if (rng.NextDouble() < Settings.changingSubcube && subCubeSize < Settings.subCubeMaxSize) subCubeSize++;
            }
        }

        public void Operate(Population parents, Population offspring)
        {
            int size = parents.GetPopulationSize();

            //subCubeSize = rng.NextInt(1, ((CubeIndividual)parents.Get(0)).GetCubeDimension());

            for (int i = 0; i < size / 2; i++)
            //Parallel.For(0, size / 2, i =>
            {
                Individual p1 = parents.Get(2 * i);
                Individual p2 = parents.Get(2 * i + 1);

                Individual o1 = (Individual)p1.Clone();
                Individual o2 = (Individual)p2.Clone();

                if (subCubeSize <= p1.GetCubeDimension() && 
                    rng.NextDouble() < xOverProb)
                {
                    // vypocet indexu k fixaci a hodnot pro ne
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
                    
                    // swapping subcube
                    int testValue;
                    int length = p1.Length();
                    foreach (var e in Program.graph.GetEdges())
                    {
                        byte parent1Value = p1.IsActiveOnEdge(e.ID);
                        byte parent2Value = p2.IsActiveOnEdge(e.ID);

                        if (parent1Value != parent2Value)
                        {
                            testValue = Tools.TestSubcube(fix, vals, e); // snad to funguje :))

                            if (testValue == -1) // vne, proto nechavame
                            {
                                //o1.SetActivityBetweenVertices(j, k, parent1Value);
                                //o2.SetActivityBetweenVertices(j, k, parent2Value);
                            }
                            else if (testValue == 0) // je na hrane, nic
                            {
                                byte value = parent1Value >= 1 ? parent1Value : parent2Value;
                                o1.SetActivityOnEdge(e.ID, value);
                                o2.SetActivityOnEdge(e.ID, value);
                            }
                            else // je uvnitr, proto prohazujem
                            {
                                /*if (subCubeSize == 1)
                                {
                                    e1.Active = e1.Active == true ? false : true;
                                    e2.Active = e2.Active == true ? false : true;
                                }*/
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
            //});
            }
        }

        
    }
}
