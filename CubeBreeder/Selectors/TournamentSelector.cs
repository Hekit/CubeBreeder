using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder.Selectors
{
    class TournamentSelector : Selector
    {
        double weakerProb;
        int competitors;

        public TournamentSelector()
        {
            weakerProb = Settings.tourWeakerProb;
            competitors = Settings.competitors;
        }

        RandomNumberGenerator rng = RandomNumberGenerator.GetInstance();

        public void Select(int howMany, Population from, Population to)
        {
            for (int i = 0; i < howMany; i++)
            {
                List<int> players = new List<int>();

                for (int j = 0; j < competitors; j++)
                {
                    players.Add(rng.NextInt(from.GetPopulationSize()));
                }
                
                while (players.Count > 1)
                {
                    if (from.Get(players[0]).GetFitnessValue() > from.Get(players[1]).GetFitnessValue()
                        && rng.NextDouble() > weakerProb)
                        players.Remove(players[1]);
                    else players.Remove(players[0]);                        
                }
                
                to.Add((Individual)from.Get(players[0]).Clone());
            }
        }
    }
}
