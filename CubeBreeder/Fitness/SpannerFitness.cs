using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder.Fitness
{
    /// <summary>
    /// The fitness for 3-spanner search.
    /// </summary>
    class SpannerFitness : FitnessFunction
    {
        int edgeCount = Int32.MaxValue;

        public SpannerFitness(int edgeCount)
        {
            this.edgeCount = edgeCount;
        }

        public double Evaluate(Individual ind, bool count)
        {
            if (!ind.changed)
            {
                if (count && ind.spanner == 1)
                    Program.localDetourSpanners++; 
                return ind.GetFitnessValue();
            }
            else ind.changed = false;
            
            int fitness = edgeCount - ind.GetActiveEdgeCount();
            ind.spanner = ind.Is_3_Spanner(count);

            //this sets the objective value, can be different from the fitness function
            if (ind.spanner == 1.0) ind.SetObjectiveValue(edgeCount - fitness);
            else ind.SetObjectiveValue(0);

            return fitness * (ind.spanner * ind.spanner);
        }
    }
}
