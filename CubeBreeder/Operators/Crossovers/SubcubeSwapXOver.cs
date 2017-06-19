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

        public void Operate(Population parents, Population offspring)
        {

            int size = parents.GetPopulationSize();

            //subCubeSize = rng.NextInt(1, ((CubeIndividual)parents.Get(0)).GetCubeDimension());

            for (int i = 0; i < size / 2; i++)
            //Parallel.For(0, size / 2, i =>
            {
                Individual p1 = parents.Get(2 * i);
                Individual p2 = parents.Get(2 * i + 1);

                Individual o1 = new Individual(p1);
                Individual o2 = new Individual(p2);

                if (rng.NextDouble() < xOverProb)
                {
                    // vypocet indexu k fixaci a hodnot pro ne
                    bool[] fix = new bool[o1.GetCubeDimension()];
                    byte[] vals = new byte[fix.Length];

                    for (int j = 0; j < fix.Length; j++)
                    {
                        fix[j] = false;
                    }

                    for (int j = 0; j < 3; j++)
                    {
                        int val = rng.NextInt(fix.Length);
                        while (fix[val] == true) val = rng.NextInt(fix.Length);
                        fix[val] = true;
                        vals[val] = rng.NextByte(2); // 0 or 1
                    }

                    // vymena podkrychle
                    int testValue;
                    int length = p1.Length();
                    for (int j = 0; j < length; j++)
                    {
                        for (int k = j + 1; k < length; k++)
                        {
                            if (Tools.Distance(j, k) == 1)
                            {
                                //testValue = TestSubcube(fix, vals, j, k);
                                Edge e = Program.graph.GetEdge(j, k);
                                bool parent1Value = p1.IsActiveBetweenVertices(j, k);
                                bool parent2Value = p2.IsActiveBetweenVertices(j, k);

                                if (parent1Value != parent2Value)
                                {
                                    testValue = TestSubcube(fix, vals, e); // snad to funguje :))

                                    if (testValue == -1) // vne, proto nechavame
                                    {
                                        //o1.SetActivityBetweenVertices(j, k, parent1Value);
                                        //o2.SetActivityBetweenVertices(j, k, parent2Value);
                                    }
                                    else if (testValue == 0) // je na hrane, nic
                                    {
                                        bool value = parent1Value == true ? parent1Value : parent2Value;
                                        o1.SetActivityBetweenVertices(j, k, value);
                                        o2.SetActivityBetweenVertices(j, k, value);
                                    }
                                    else // je uvnitr, proto prohazujem
                                    {
                                        /*if (subCubeSize == 1)
                                        {
                                            e1.Active = e1.Active == true ? false : true;
                                            e2.Active = e2.Active == true ? false : true;
                                        }*/
                                        o1.SetActivityBetweenVertices(j, k, parent2Value);
                                        o2.SetActivityBetweenVertices(j, k, parent1Value);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    o1 = p1;
                    o2 = p2;
                }

                offspring.Add(o1);
                offspring.Add(o2);
            //});
            }
        }

        // -1 both out, 0 one and one, 1 both in
        private int TestSubcube(bool[] fix, byte[] vals, Edge e)
        {
            byte[] v1 = Tools.ToBinary(e.Vertex1);
            byte[] v2 = Tools.ToBinary(e.Vertex2);
            return TestSubcube(fix, vals, v1, v2);
        }

        private int TestSubcube(bool[] fix, byte[] vals, int x, int y)
        {
            byte[] v1 = Tools.ToBinary(x);
            byte[] v2 = Tools.ToBinary(y);
            return TestSubcube(fix, vals, v1, v2);
        }

        private int TestSubcube(bool[] fix, byte[] vals,  byte[] v1,  byte[] v2)
        {
            string status = "bothIn";

            for (int i = 0; i < fix.Length; i++)
            {
                if (fix[i])
                {
                    if (v1[i] != vals[i])
                        if (status == "bothIn") status = "v1out";
                        else if (status == "v2out") return -1;
                    if (v2[i] != vals[i])
                        if (status == "bothIn") status = "v2out";
                        else if (status == "v1out") return -1;
                }
            }
            if (status == "bothIn") return 1;
            else return 0;
        }
    }
}
