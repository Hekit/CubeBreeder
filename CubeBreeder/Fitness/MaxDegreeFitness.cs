using System;
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
        int edgeCount = Int32.MaxValue;
        private double[] exps;

        public MaxDegreeFitness(int dimension, int edgeCount)
        {
            this.dimension = dimension;
            this.edgeCount = edgeCount;
            this.exps = new double[dimension+1];
            for (int i = 0; i < exps.Length; i++)
            {
                //exps[i] = Math.Pow(i*2, 2);
                exps[i] = Math.Exp(i);
            }
        }

        public double Evaluate(Individual ind, bool count)
        {
            int maxDegree = ind.GetMaxDegree();
            double fitness = 1;
            int threshold = 2;
            
            double spanner = ind.Is_3_Spanner(count);
            //if (spanner == 1.0) ind.SetObjectiveValue(maxDegree);
            if (ind.spanner == 1.0) ind.SetObjectiveValue(ind.GetActiveEdgeCount());
            else ind.SetObjectiveValue(0);

            foreach (var degree in ind.GetDegrees())
            {
                //fitness += (degree * degree);
                //fitness += exps[(dimension - degree)];
                //fitness += exps[degree];
                //if (degree > threshold) fitness += (degree - threshold);
                if (degree > threshold) fitness += exps[degree - threshold];
            }
            int missing = edgeCount - ind.GetActiveEdgeCount();

            //return fitness * Math.Pow(spanner,2);
            //return ((Math.Pow(2, dimension) * edgeCount) / fitness) * Math.Pow(spanner,3);//(Math.Pow(2, dimension) / fitness); //* (spanner * spanner);
            //double xyz =  (1 / fitness) * Math.Pow(spanner,2) * missing;
            return (1 / fitness) * Math.Pow(spanner, 2);
            //Console.WriteLine(xyz);
            //return xyz;
        }
    }
}
