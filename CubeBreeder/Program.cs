using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CubeBreeder;
using System.IO;
using System.Diagnostics;
using CubeBreeder.Selectors;
using CubeBreeder.Fitness;
using CubeBreeder.Operators.Crossovers;
using CubeBreeder.Operators.Mutations;
using CubeBreeder.Replacements;

namespace CubeBreeder
{
    class Program
    {
        static int maxGen;
        static int popSize;
        static int vertexCount;
        static int edgeCount;
        static int cubeDimension;
        static double xoverProb;
        static double mutProb;
        static double mutProbPerBit;
        static double mutRepair;
        static int repeats;
        static int subcubeSize;
        static double eliteSize;
        public static int localDetourSpanners = 0;
        public static int maxColours;
        static bool paralell;

        public static int showGap = 25;

        static Tools tools;
        public static GraphInfo graph;

        static void Main(string[] args)
        {
            /////////////////////////////////
            //                             //
            //      Initial settings       //
            //                             //
            /////////////////////////////////
            // Maximal number of generations
            maxGen = Properties.Settings.Default._MaxGenerations;
            // Population size
            popSize = Properties.Settings.Default._PopulationSize;
            // Dimensions
            cubeDimension = Properties.Settings.Default._Dimension;
            edgeCount = (int)Math.Pow(2, cubeDimension - 1) * cubeDimension;
            vertexCount = (int)Math.Pow(2, cubeDimension);
            // Crossover probability
            xoverProb = Properties.Settings.Default.P_CrossoverProbability;
            // Mutation probability
            mutProb = Properties.Settings.Default.P_MutationProbability;
            // Mutation per bit probability
            mutProbPerBit = Properties.Settings.Default.P_MutationPerEdgeProbability;
            // Mutation of repair per edge probability
            mutRepair = Properties.Settings.Default.P_MutationRepairProbability;
            // Number of repeats of the experiment
            repeats = Properties.Settings.Default.Repeats;
            // Size of the subcube in crossover
            subcubeSize = Properties.Settings.Default._SubcubeSize;
            // Size of the elite
            eliteSize = Properties.Settings.Default.EliteSize;

            paralell = Properties.Settings.Default.Paralell;

            tools = Tools.GetInstance(cubeDimension);

            if (Properties.Settings.Default.Task == "spanner")
            {
                maxColours = 1;
            }
            else if (Properties.Settings.Default.Task == "degree")
            {
                maxColours = 1;
            }
            else maxColours = cubeDimension / 2;

            //GraphInfo graph;
            try
            {
                graph = GraphInfo.LoadFullCube(cubeDimension);
            }
            catch
            {
                graph = new GraphInfo(cubeDimension);
            }

            popSize = edgeCount * 10;

            //Console.WriteLine(Environment.ProcessorCount);

            /////////////////////////////////
            //                             //
            //      Run the algorithm      //
            //                             //
            /////////////////////////////////

            Console.WriteLine("Dimension: " + cubeDimension);
            Console.WriteLine("Population: " + (edgeCount * 10));
            Console.WriteLine();

            List<Individual> bestInds = new List<Individual>();

            for (int i = 0; i < repeats; i++)
            //Parallel.For(0, repeats, i =>
            {
                Individual best = Run(i);
                if (best != null) bestInds.Add(best);
            }//);
            for (int i = 0; i < bestInds.Count; i++)
            {
                Console.WriteLine("Run " + (i + 1) + ": best objective=" + bestInds[i].GetObjectiveValue());
                Tools.WriteIndividual(bestInds[i]);
            }
        }

        public static Individual Run(int number)
        {
            Console.WriteLine("Starting run no. " + (number + 1));
            Console.WriteLine("Initializing");
            Stopwatch sw = Stopwatch.StartNew();
            //Set the rng seed
            RandomNumberGenerator.GetInstance().ReSeed(number);

            tools = Tools.GetInstance(cubeDimension);

            //Create new population
            Population pop = new Population();
            pop.SetPopulationSize(popSize);
            pop.SetSampleIndividual(graph);
            pop.CreateRandomInitialPopulation();

            //Set the options for the evolutionary algorithm
            EvolutionaryAlgorithm ea = new EvolutionaryAlgorithm();
            // Fitness function
            if (Properties.Settings.Default.Task == "spanner")
            {
                ea.SetFitnessFunction(new SpannerFitness(edgeCount));
            }
            else if (Properties.Settings.Default.Task == "degree")
            {
                ea.SetFitnessFunction(new MaxDegreeFitness(cubeDimension));
            }
            else throw new NotImplementedException();
            // Selectors
            //ea.AddMatingSelector(new RouletteWheelSelector());
            ea.AddMatingSelector(new TournamentSelector());
            // Operators
            ea.AddOperator(new SubcubeSwapXOver(xoverProb, subcubeSize));
            //ea.AddOperator(new SubcubeSwapXOver(xoverProb, 1));
            //ea.AddOperator(new SimpleRepairEdgeMutation(mutProb, mutRepair));
            ea.AddOperator(new CleverRepairEdgeMutation(mutProb / 100, mutRepair));
            ea.AddOperator(new FlipEdgeMutation(mutProb, mutProbPerBit));
            ea.AddOperator(new SubcubeTranslationMutation(mutProb, 3));
            ea.AddOperator(new SubcubeRotationMutation(mutProb, 3));
            //ea.AddEnvironmentalSelector(new RouletteWheelSelector());
            ea.AddEnvironmentalSelector(new TournamentSelector());

            ea.SetElite(eliteSize);

            Console.WriteLine("Finished in {0:f2} seconds", sw.Elapsed.TotalSeconds);

            //Run the algorithm

            Console.WriteLine("Running");

            try
            {
                for (int i = 0; i < maxGen; i++)
                {
                    sw.Restart();
                    localDetourSpanners = 0;
                    //Make one generation
                    ea.Evolve(pop);
                    List<Individual> sorted = pop.GetSortedIndividuals();
                    //Log the best individual to console.
                    if ((i + 1) % showGap == 0)
                    {
                        int idx = 0;
                        while (idx < popSize && sorted[idx].Is_3_Spanner(false) < 1) idx++;
                        if (idx >= popSize) idx = 0;
                        //if (paralell) Console.Write("Thread " + number + ": ");
                        //if (paralell) Console.Write("Thread " + Environment.CurrentManagedThreadId + ": ");
                        //Console.WriteLine("Generation: " + (i + 1) + " fitness: " + sorted[0].GetFitnessValue());
                        Console.Write("Gen: " + (i + 1));
                        Console.Write(" obj: " + sorted[idx].GetObjectiveValue() + " at " + idx);
                        Console.Write(" fit: {0:f0}", sorted[0].GetFitnessValue());
                        Console.Write(" 3-s: {0:f2} %", (float)(localDetourSpanners * 100.0 / popSize));
                        Console.Write(" med: {0:f0}", sorted[popSize / 2].GetFitnessValue());
                        Console.WriteLine();
                        //if (sorted[0].GetFitnessValue() == edgeCount) i = maxGen; // stopka pro tvoreni plne krychle
                    }

                    for (int j = 0; j < sorted.Count; j++)
                    {
                        if (j < popSize * eliteSize) sorted[j].elite = true;
                        else sorted[j].elite = false;
                    }

                    //Console.WriteLine("{0:f2} s", sw.Elapsed.TotalSeconds);
                    if (Properties.Settings.Default.Extendable && i + 1 == maxGen)
                    {
                        Console.WriteLine("Continue?");
                        int extra = 0;
                        if (Int32.TryParse(Console.ReadLine(), out extra))
                        {
                            maxGen += extra;
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
