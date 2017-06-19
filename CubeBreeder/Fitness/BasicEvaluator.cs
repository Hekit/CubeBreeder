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
            pop.GetIndividuals().ForEach(x =>
            {
                x.SetFitnessValue(fitness.Evaluate(x, count));
            });
        }
    }
}
