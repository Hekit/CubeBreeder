using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder.Fitness
{
    /// <summary>
    /// General class for evaluating fitness
    /// </summary>
    class BasicEvaluator : FitnessEvaluator
    {
        FitnessFunction fitness;

        public BasicEvaluator(FitnessFunction fitness)
        {
            this.fitness = fitness;
        }

        /// <summary>
        ///  Evaluates the fitness of all individuals in the population
        /// </summary>
        /// <param name="pop">the population</param>
        /// <param name="count">propagating information where spanners are counted</param>
        public void Evaluate(Population pop, bool count)
        {
            double average = 0;
            pop.GetIndividuals().ForEach(x =>
            {
                double fit = fitness.Evaluate(x, count);
                x.SetFitnessValue(fit);
                average += fit;
            });
            // set average of population for console output
            pop.SetAverage(average / pop.GetPopulationSize());
        }
    }
}
