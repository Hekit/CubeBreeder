using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder.Fitness
{
    class SpannerFitness : FitnessFunction
    {
        int edgeCount = Int32.MaxValue;

        public SpannerFitness(int edgeCount)
        {
            this.edgeCount = edgeCount;
        }

        public double Evaluate(Individual ind, bool count)
        {
            int fitness = edgeCount - ind.GetActiveEdgeCount();
            double spanner = ind.Is_3_Spanner(count);

            //this sets the objective value, can be different from the fitness function
            if (spanner == 1.0) ind.SetObjectiveValue(edgeCount - fitness);
            else ind.SetObjectiveValue(0);

            // pro pocitani co nejmene hran
            //return fitness;

            // pro pocitani co nejvice hran
            //return edgeCount - fitness;

            // pro pocitani minimalni kostry
            //return fitness - fitness*((CubeIndividual)ind).IsSpanner();

            // pro pocitani 3 spanneru verze 0 1
            //return fitness - fitness * ((CubeIndividual)ind).Is_3_Spanner(count);

            // pro pocitani 3 spanneru
            return fitness * spanner;
        }
    }
}
