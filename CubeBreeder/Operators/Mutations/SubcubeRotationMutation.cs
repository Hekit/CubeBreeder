using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder.Operators.Mutations
{
    class SubcubeRotationMutation : Operator
    {
        double mutationProbability;
        int subCubeSize = 3;

        RandomNumberGenerator rng = RandomNumberGenerator.GetInstance();

        public SubcubeRotationMutation(double mutationProbability, int subCube)
        {
            this.mutationProbability = mutationProbability;
            this.subCubeSize = subCube;
        }

        public void Operate(Population parents, Population offspring)
        {
            int size = parents.GetPopulationSize();

            for (int i = 0; i < size; i++)
            {
                Individual p1 = parents.Get(i);
                Individual o1 = (Individual)p1.Clone();

                if (rng.NextDouble() < mutationProbability)
                {
                    // vypocet indexu k fixaci a hodnot pro ne
                    bool[] fix = new bool[o1.GetCubeDimension()];
                    byte[] vals = new byte[fix.Length];

                    for (int j = 0; j < fix.Length; j++)
                    {
                        fix[j] = false;
                    }

                    for (int j = 0; j < subCubeSize; j++)
                    {
                        int val = rng.NextInt(fix.Length);
                        while (fix[val] == true) val = rng.NextInt(fix.Length);
                        fix[val] = true;
                        vals[val] = rng.NextByte(2); // 0 or 1
                    }

                    // vymena podkrychle
                    int testValue;
                    int length = p1.Length();

                    foreach (var e in Program.graph.GetEdges())
                    {
                        //testValue = TestSubcube(fix, vals, j, k);
                        byte parent1Value = p1.IsActiveOnEdge(e.ID);

                        testValue = Tools.TestSubcube(fix, vals, e); // snad to funguje :))

                        if (testValue == -1) // vne
                        {
                        }
                        else if (testValue == 0) // je na hrane
                        {
                        }
                        else // je uvnitr
                        {
                        }
                    }
                }
            }
        }
    }
}
