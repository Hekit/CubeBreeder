using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder
{
    class Run
    {
        Settings s = Settings.GetInstance();
        int number;
        EvolutionaryAlgorithm ea;

        public Run(int number)
        {
            this.number = number;
        }

        public Individual RunIt()
        { 
            Console.WriteLine("Starting run no. " + (number + 1));
            Console.WriteLine("Initializing run no. " + (number + 1));
            Stopwatch sw = Stopwatch.StartNew();
            //Set the rng seed
            GraphInfo graph = GraphInfo.GetInstance(s.cubeDimension);
            Tools tools = Tools.GetInstance(s.cubeDimension);

            ea = s.InitializeEVA();

            //Create new population
            Population pop = new Population();
            pop.SetPopulationSize(s.popSize);
            pop.SetSampleIndividual(graph);
            pop.CreateRandomInitialPopulation();

            if (!Settings.paralell) Console.WriteLine("Finished in {0:f2} seconds", sw.Elapsed.TotalSeconds);

            //Run the algorithm

            Console.WriteLine("Running run no. " + (number + 1));

            try
            {
                for (int i = 0; i < s.maxGen; i++)
                {
                    sw.Restart();
                    Program.localDetourSpanners = 0;
                    //Make one generation
                    ea.Evolve(pop);
                    List<Individual> sorted = pop.GetSortedIndividuals();
                    //Log the best individual to console.
                    if ((i + 1) % Settings.showGap == 0 && !Settings.paralell)
                    {
                        int idx = 0;
                        while (idx < s.popSize && sorted[idx].Is_3_Spanner(false) < 1) idx++;
                        if (idx >= s.popSize) idx = 0;
                        Console.Write("Gen: " + (i + 1));
                        Console.Write(" obj: " + sorted[idx].GetObjectiveValue() + " at " + idx);
                        Console.Write(" fit: {0:f0}", sorted[0].GetFitnessValue());
                        Console.Write(" 3-s: {0:f2} %", (float)(Program.localDetourSpanners * 100.0 / s.popSize));
                        Console.Write(" med: {0:f0}", sorted[s.popSize / 2].GetFitnessValue());
                        Console.WriteLine();
                    }

                    for (int j = 0; j < sorted.Count; j++)
                    {
                        if (j < s.popSize * s.eliteSize) sorted[j].elite = true;
                        else sorted[j].elite = false;
                    }

                    //Console.WriteLine("{0:f2} s", sw.Elapsed.TotalSeconds);
                    if (Properties.Settings.Default.Extendable && i + 1 == s.maxGen)
                    {
                        Console.WriteLine("Continue?");
                        int extra = 0;
                        if (Int32.TryParse(Console.ReadLine(), out extra))
                        {
                            s.maxGen += extra;
                        }
                    }
                }
                Console.WriteLine();
                Individual bestInd;
                for (int j = 0; j < pop.GetPopulationSize(); j++)
                {
                    if ((pop.GetSortedIndividuals()[j]).Is_3_Spanner(false) == 1)
                        return bestInd = pop.GetSortedIndividuals()[j];
                }
                return bestInd = pop.GetSortedIndividuals()[0];
                //return bestInd;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            return null;

        }
    }
}
