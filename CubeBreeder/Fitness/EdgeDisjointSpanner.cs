using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder.Fitness
{
    class EdgeDisjointSpanner : FitnessFunction
    {
        int edgeCount = Int32.MaxValue;

        public EdgeDisjointSpanner(int edgeCount)
        {
            this.edgeCount = edgeCount;
        }

        public double Evaluate(Individual ind, bool count)
        {
            int[] fitness = new int[Program.maxColours];
            int[] objective = new int[Program.maxColours];
            double[] spanner = new double[Program.maxColours];

            for (int i = 0; i < fitness.Length; i++)
            {
                fitness[i] = edgeCount - ind.GetActiveEdgeCount((byte)(i + 1));
                spanner[i] = ind.Is_Good_Enough((byte)(i+1));
                //this sets the objective value, can be different from the fitness function
                if (spanner[i] == 1.0) objective[i] = edgeCount - fitness[i];
                else objective[i] = 0;
            }

            double fit = 0;
            int colours = 0;
            for (int i = 0; i < fitness.Length; i++)
            {
                if (fitness[i] < edgeCount)
                {
                    fit += fitness[i] * spanner[i];
                    colours++;
                }
            }
            // proved magickou operaci
            // a ne, ta operace neni prumer
            return fit / colours;
        }
    }
}
