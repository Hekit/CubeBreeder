using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder.Selectors
{
    /// <summary>
    /// Boltzmann Roulette Wheel Selector
    /// </summary>
    class BoltzmannRouletteWheelSelector : Selector
    {
        int maxGenerations;
        int currentGeneration;
        double temperature;
        // temperature settings
        int t0 = 100; // [5, 100]
        double alpha = 0.05; // [0,1]

        RandomNumberGenerator rng = RandomNumberGenerator.GetInstance();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="max">maximal number of generations</param>
        public BoltzmannRouletteWheelSelector(int max)
        {
            maxGenerations = max;
            currentGeneration = 0;
        }

        /// <summary>
        /// Selecting howMany individuals from from to to
        /// </summary>
        /// <param name="howMany">number of individuals to select</param>
        /// <param name="from">old population</param>
        /// <param name="to">new population</param>
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

            double fitnessSum = 0.0;

            foreach (var ind in pop.GetIndividuals())
            {
                double fitness = ind.GetFitnessValue();
                double newFitness = Math.Exp(fitness / temperature) / average;
                ind.SetFitnessValue(newFitness);
                fitnessSum += newFitness;
            }

            double[] fitnesses = new double[pop.GetPopulationSize()];

            for (int i = 0; i < fitnesses.Length; i++)
            {
                fitnesses[i] = pop.Get(i).GetFitnessValue() / fitnessSum;
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

