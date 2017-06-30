﻿using System;
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

                    int perm1 = rng.NextInt(fix.Length);
                    while (fix[perm1] == true) perm1 = rng.NextInt(fix.Length);
                    int perm2 = rng.NextInt(fix.Length);
                    while (fix[perm2] == true || perm1 == perm2) perm2 = rng.NextInt(fix.Length);


                    // vymena podkrychle
                    int length = p1.Length();

                    foreach (var e in Program.graph.GetEdges())
                    {
                        byte[] v1 = Tools.ToBinary(e.Vertex1);
                        byte[] v2 = Tools.ToBinary(e.Vertex2);

                        int test = Tools.TestSubcube(fix, vals, v1, v2);

                        if (test == 1)
                        {
                            byte[] sonV1 = v1;
                            byte[] sonV2 = v2;
                            sonV1[perm1] = v1[perm2];
                            sonV1[perm2] = v1[perm1];
                            sonV2[perm1] = v2[perm2];
                            sonV2[perm2] = v2[perm1];

                            int son1 = Tools.FromBinary(sonV1);
                            int son2 = Tools.FromBinary(sonV2);

                            o1.SetActivityBetweenVertices(son1, son2, p1.IsActiveOnEdge(e.ID));
                        }
                    }
                    o1.changed = true;
                }
                offspring.Add(o1);
            }
        }
    }
}