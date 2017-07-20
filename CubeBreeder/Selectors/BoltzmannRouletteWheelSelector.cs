using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder.Selectors
{
    class BoltzmannRouletteWheelSelector : Selector
    {
        int maxGenerations;
        int currentGeneration;
        int t0 = 100; // [5, 100]
        double alpha = 0.05; // [0,1]
        double temperature;

        RandomNumberGenerator rng = RandomNumberGenerator.GetInstance();

        public BoltzmannRouletteWheelSelector(int max)
        {
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

