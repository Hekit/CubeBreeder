using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder.Selectors
{
    class RouletteWheelSelector : Selector
    {
        RandomNumberGenerator rng = RandomNumberGenerator.GetInstance();

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
