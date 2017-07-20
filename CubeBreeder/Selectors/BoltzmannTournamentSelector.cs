using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder.Selectors
{
    class BoltzmannTournamentSelector : Selector
    {
        double weakerProb;
        int competitors;
        int maxGenerations;
        int currentGeneration;
        int t0 = 15; // [5, 100]
        double alpha = 0.1; // [0,1]
        double temperature;

        RandomNumberGenerator rng = RandomNumberGenerator.GetInstance();

        public BoltzmannTournamentSelector(int max)
        {
            weakerProb = Settings.tourWeakerProb;
            competitors = Settings.competitors;
            maxGenerations = max;
            currentGeneration = 0;
        }

        public void Select(int howMany, Population from, Population to)
        {
            // update temperature
            currentGeneration++;
            double k = (1 + 100 * (currentGeneration / maxGenerations));
            temperature = t0 * Math.Pow(1 - alpha, k);

            // calculate average fitness
            double average = 0;
            Population pop = new Population();
            foreach (var ind in from.GetIndividuals())
            {
                average += Math.Exp(ind.GetFitnessValue() / temperature);
                Individual boltzInd = (Individual)ind.Clone();
                pop.Add(boltzInd);
            }
            average = average / pop.GetPopulationSize();
            foreach (var ind in pop.GetIndividuals())
            {
                double fitness = ind.GetFitnessValue();
                ind.SetFitnessValue(Math.Exp(fitness / temperature) / average);
            }

            for (int i = 0; i < howMany; i++)
            {
                List<int> players = new List<int>();

                for (int j = 0; j < competitors; j++)
                {
                    players.Add(rng.NextInt(from.GetPopulationSize()));
                }

                while (players.Count > 1)
                {
                    if (pop.Get(players[0]).GetFitnessValue() > pop.Get(players[1]).GetFitnessValue()
                        && rng.NextDouble() > weakerProb)
                        players.Remove(players[1]);
                    else players.Remove(players[0]);
                }

                to.Add((Individual)from.Get(players[0]).Clone());
            }
        }
    }
}
