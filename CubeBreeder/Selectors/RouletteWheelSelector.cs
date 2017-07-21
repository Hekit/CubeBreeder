using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder.Selectors
{
    /// <summary>
    /// Roulette Wheel Selector
    /// </summary>
    class RouletteWheelSelector : Selector
    {
        Random rng;

        public RouletteWheelSelector(Random rnd)
        {
            rng = rnd;
        }

        public string ToLog()
        {
            return "RouletteWheel";
        }

        /// <summary>
        /// Selecting howMany individuals from from to to
        /// </summary>
        /// <param name="howMany">number of individuals to select</param>
        /// <param name="from">old population</param>
        /// <param name="to">new population</param>
        public void Select(int howMany, Population from, Population to)
        {
            double fitnessSum = 0.0;

            for (int i = 0; i < from.GetPopulationSize(); i++)
            {
                fitnessSum += from.Get(i).GetFitnessValue();
            }

            double[] fitnesses = new double[from.GetPopulationSize()];

            for (int i = 0; i < fitnesses.Length; i++)
            {
                fitnesses[i] = from.Get(i).GetFitnessValue() / fitnessSum;
            }

            for (int i = 0; i < howMany; i++)
            {

                double ball = rng.NextDouble();
                double sum = 0;

                for (int j = 0; j < fitnesses.Length; j++)
                {

                    sum += fitnesses[j];
                    if (sum > ball)
                    {
                        to.Add((Individual)from.Get(j).Clone());
                        break;
                    }
                }

            }
        }
    }
}
