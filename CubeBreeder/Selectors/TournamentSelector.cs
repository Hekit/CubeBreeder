using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder.Selectors
{
    class TournamentSelector : Selector
    {
        RandomNumberGenerator rng = RandomNumberGenerator.GetInstance();

        public void Select(int howMany, Population from, Population to)
        {

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
            }
        }
    }
}
