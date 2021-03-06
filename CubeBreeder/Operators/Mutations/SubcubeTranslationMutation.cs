﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder.Operators.Mutations
{
    /// <summary>
    /// Subcube Translation Mutation
    /// </summary>
    class SubcubeTranslationMutation : Operator
    {
        double mutationProbability;
        int subCubeSize = 3;

        Random rng;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mutationProbability">probability of mutation</param>
        /// <param name="subCube">size of subcube</param>
        public SubcubeTranslationMutation(double mutationProbability, int subCube, Random rnd)
        {
            this.mutationProbability = mutationProbability;
            this.subCubeSize = subCube;
            rng = rnd;
        }

        public string ToLog()
        {
            return "SubcubeTranslation\tprob:\t" + String.Format("{0:f2}", mutationProbability) + "\tsubcube:\t" + subCubeSize;
        }

        /// <summary>
        /// Each generation update method
        /// </summary>
        public void Update()
        {
            // note that the size might change up and down (=not at all) in the same generation
            if (Settings.changingSubcube > 0)
            {
                if (rng.NextDouble() < Settings.changingSubcube && subCubeSize > 2) subCubeSize--;
                if (rng.NextDouble() < Settings.changingSubcube && subCubeSize < Settings.subCubeMaxSize) subCubeSize++;
            }
        }

        /// <summary>
        /// Operator operate method
        /// </summary>
        /// <param name="parents">parents</param>
        /// <param name="offspring">offsrping</param>
        public void Operate(Population parents, Population offspring)
        {
            int size = parents.GetPopulationSize();

            for (int i = 0; i < size; i++)
            {
                Individual p1 = parents.Get(i);
                Individual o1 = (Individual)p1.Clone();

                if (subCubeSize <= o1.GetCubeDimension() &&
                    rng.NextDouble() < mutationProbability)
                {
                    // find indices to be fixed and their values
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

                    // subcube swap
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
                            sonV1[direction] = v1[direction].IsTrue() ? (byte)0 : (byte)1;
                            sonV2[direction] = v2[direction].IsTrue() ? (byte)0 : (byte)1;

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
