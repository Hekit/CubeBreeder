using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder.Fitness
{
    class BasicEvaluator : FitnessEvaluator
    {
        FitnessFunction fitness;

        public BasicEvaluator(FitnessFunction fitness)
        {
            this.fitness = fitness;
        }

        public void Evaluate(Population pop, bool count)
        {
            double average = 0;
            pop.GetIndividuals().ForEach(x =>
            {
                double fit = fitness.Evaluate(x, count);
                x.SetFitnessValue(fit);
                average += fit;
            });
            pop.SetAverage(average / pop.GetPopulationSize());
        }
    }
}
