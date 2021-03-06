﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder.Fitness
{
    /// <summary>
    /// The fitness function for minimizing the maximal degree
    /// </summary>
    class MaxDegreeFitness : FitnessFunction
    {
        int dimension = Int32.MaxValue;

        public MaxDegreeFitness(int dimension)
        {
            this.dimension = dimension;
        }

        public double Evaluate(Individual ind, bool count)
        {
            int maxDegree = ind.GetMaxDegree();
            int fitness = 0;
            double spanner = ind.Is_3_Spanner(count);
            if (spanner == 1.0) ind.SetObjectiveValue(maxDegree);
            else ind.SetObjectiveValue(0);

            foreach (var degree in ind.GetDegrees())
            {
                fitness += (degree * degree);
            }

            return (Math.Pow(2, dimension) / fitness) * (spanner * spanner);
        }
    }
}
