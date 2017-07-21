using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder.Selectors
{
    /// <summary>
    /// Tournament Selector
    /// </summary>
    class TournamentSelector : Selector
    {
        double weakerProb;
        int competitors;
        Random rng;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="weakerProb">probability that the weaker individual will be selected</param>
        /// <param name="competitors">number of competitors in the tournament</param>
        public TournamentSelector(double weakerProb, int competitors, Random rng)
        {
            this.weakerProb = weakerProb;
            this.competitors = competitors;
            this.rng = rng;
        }

        public string ToLog()
        {
            return "Tournament weaker: " + weakerProb + " competitors: " + competitors;
        }

        /// <summary>
        /// Selecting howMany individuals from from to to
        /// </summary>
        /// <param name="howMany">number of individuals to select</param>
        /// <param name="from">old population</param>
        /// <param name="to">new population</param>
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
