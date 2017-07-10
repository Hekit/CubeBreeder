using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder.Selectors
{
    class BoltzmannSelector : Selector
    {
        RandomNumberGenerator rng = RandomNumberGenerator.GetInstance();
        double temperature = 1;

        public void Select(int howMany, Population from, Population to)
        {
            /*
            // update temperature
            temperature = temperature;

            // calculate average fitness
            double total = 0;
            foreach (var ind in from.GetIndividuals())
            {
                total += Math.Exp(ind.GetFitnessValue() / temperature);
            }
            double average = totalFitness / from.GetPopulationSize();


            Individual indi = new Individual((Individual)null);
            double vzorecek = (Math.Exp(indi.GetFitnessValue() / temperature)
                / Math.Exp(fitnessAverage / temperature));

            int idx = 0;
            for (int i = 0; i < howMany; i++)
            {
                int i1 = rng.NextInt(from.GetPopulationSize());
                int i2 = rng.NextInt(from.GetPopulationSize());

                if ((from.Get(i1).GetFitnessValue() > from.Get(i2).GetFitnessValue()) && rng.NextDouble() < 0.8)
                {
                    to.Add((Individual)from.Get(i1).Clone());
                }
                else
                {
                    to.Add((Individual)from.Get(i2).Clone());
                }
            }*/
        }
    }
}
