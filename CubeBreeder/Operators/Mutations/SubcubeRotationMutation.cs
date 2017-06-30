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

        public void Update() { }

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

                    int direction = rng.NextInt(fix.Length);
                    while (fix[direction] == true) direction = rng.NextInt(fix.Length);

                    // vymena podkrychle
                    int length = p1.Length();

                    foreach (var e in Program.graph.GetEdges())
                    {
                        //testValue = TestSubcube(fix, vals, j, k);
                        byte[] v1 = Tools.ToBinary(e.Vertex1);
                        byte[] v2 = Tools.ToBinary(e.Vertex2);

                        for (int j = 0; j < fix.Length; j++)
                        {
                            if (fix[j] && v1[j] == vals[j] && v2[j] == vals[j])
                            {
                                byte parent1Value = p1.IsActiveOnEdge(e.ID);
                                if (parent1Value.IsTrue())
                                    o1.SetActivityOnEdge(e.ID, 0);
                                else
                                    o1.SetActivityOnEdge(e.ID, 1);
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
