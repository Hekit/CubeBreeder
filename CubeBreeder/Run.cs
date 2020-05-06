using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CubeBreeder.Fitness;

namespace CubeBreeder
{
    /// <summary>
    /// Single GA run class
    /// </summary>
    class Run
    {
        Settings s;
        public int number;
        EvolutionaryAlgorithm ea;
        Logger logger;
        Random rng;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="number">the run identifier; also the random seed</param>
        public Run(int number)
        {
            this.s = Settings.GetInstance();
            this.number = number;
            logger = new Logger(number, s.cubeDimension);
        }

        /// <summary>
        /// GA running method
        /// </summary>
        /// <returns>best computed individual</returns>
        public Individual RunIt()
        {
            if (!Settings.parallel)
            {
                Console.WriteLine("Starting run no. " + (number + 1));
                Console.WriteLine("Initializing run no. " + (number + 1));
            }

            Stopwatch sw = Stopwatch.StartNew();
            //Set the rng seed
            GraphInfo graph = GraphInfo.GetInstance(s.cubeDimension);
            Tools tools = Tools.GetInstance(s.cubeDimension);

            rng = new Random(number);
            logger.Log(s);
            ea = s.GetEVA(logger, rng);
            foreach (var op in ea.GetOperators())
            {
                logger.Log(Logger.Level.SETTINGS, op.ToLog());
            }
            foreach (var se in ea.GetMating())
            {
                logger.Log(Logger.Level.SETTINGS, se.ToLog() + " MAT");
            }
            foreach (var se in ea.GetEnvironmental())
            {
                logger.Log(Logger.Level.SETTINGS, se.ToLog() + " ENV");
            }

            //Create new population
            Population pop = new Population();
            pop.SetPopulationSize(s.popSize);
            pop.SetSampleIndividual(graph);
            pop.CreateRandomInitialPopulation(rng);

            if (!Settings.parallel) Console.WriteLine("Finished in {0:f2} seconds", sw.Elapsed.TotalSeconds);
            logger.Log(Logger.Level.INFO, "Initialization finished in " +
                String.Format("{0:f2}", sw.Elapsed.TotalSeconds) + " seconds");

            //Run the algorithm
            if (!Settings.parallel || number == 0) Console.WriteLine("Running run no. " + (number + 1));
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
                    if ((i + 1) % Settings.showGap == 0 && (!Settings.parallel || number == 0))
                    {
                        int idx = 0;
                        while (idx < s.popSize && sorted[idx].Is_3_Spanner(false) < 1) idx++;
                        if (idx >= s.popSize) idx = 0;
                        Console.Write("Gen: " + (i + 1));
                        Console.Write(" obj: " + sorted[idx].GetObjectiveValue() + " at " + idx);
                        if (Settings.task == "eds") Console.Write(" tc: " + sorted[idx].GetColourCount());
                        if (Settings.task == "spanner") Console.Write(" fit: {0:f0}", sorted[0].GetFitnessValue());
                        if (Settings.task == "eds")
                        {
                            Console.Write(" fit: {0:f3}", sorted[0].GetFitnessValue());
                            Console.Write(" comps: ");
                            for (int j = 0; j < Settings.maxColours; j++)
                            {
                                Console.Write(sorted[0].CountComponents((byte) (j + 1)));
                                Console.Write(" ");
                            }
                        }
                        if (Settings.task == "degree")
                        {
                            Console.Write(" fit: {0:f3}", sorted[0].GetFitnessValue());
                            Console.Write(" degs: ");
                            
                            List<int> degrees = sorted[0].GetDegrees();
                            int[] count = new int[sorted[0].GetCubeDimension()+1];
                            foreach (var deg in degrees)
                            {
                                count[deg]++;
                            }

                            for (int j = 0; j < count.Length; j++)
                            {
                                Console.Write(/*(j+1) + ":" + */count[j] + " ");
                            }
                            Console.Write(" ");
                        }
                        Console.Write(" 3-s: {0:f2} %", (float)(Program.localDetourSpanners * 100.0 / (s.popSize)));
                        Console.Write(" avg: {0:f0}", pop.GetAverage());
                        Console.Write(" med: {0:f0}", sorted[s.popSize / 2].GetFitnessValue());
                        Console.WriteLine();
                    }

                    logger.Log(pop, sorted, i);

                    for (int j = 0; j < sorted.Count; j++)
                    {
                        //Console.WriteLine(sorted[j].ToString());
                        if (j < s.popSize * s.eliteSize) sorted[j].elite = true;
                        else sorted[j].elite = false;
                    }                    
                }
                if (!Settings.parallel) Console.WriteLine();
                Individual bestInd;

                //Console.ReadLine();
                
// #### POSTPROCESSING ####
                bool[] reRun = new bool[pop.GetPopulationSize()];
                for (int j = 0; j < pop.GetPopulationSize(); j++)
                {
                    reRun[j] = true;
                }
                int maxDegree = (int) Math.Floor(1.5 * Math.Sqrt(2 * s.cubeDimension) - 1);
                while (CheckReRun(reRun))
                {
                    var result = Postprocess(pop, maxDegree);
                    pop = result.Item1;
                    reRun = result.Item2;
                }
                for (int j = 0; j < pop.GetPopulationSize(); j++)
                {
                    reRun[j] = true;
                }
                while (CheckReRun(reRun))
                {
                    var result = Postprocess(pop, maxDegree + 1);
                    pop = result.Item1;
                    reRun = result.Item2;
                }
// #### POSTPROCESSING ####
                
                for (int j = 0; j < pop.GetPopulationSize(); j++)
                {
                    if ((pop.GetSortedIndividuals()[j]).Is_3_Spanner(false) == 1)
                    {
                        bestInd = pop.GetSortedIndividuals()[j];
                        logger.Log(bestInd);
                        return bestInd;
                    }
                }
                bestInd = pop.GetSortedIndividuals()[0];
                logger.Log(bestInd);
                return bestInd;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            return null;

        }

        bool CheckReRun(bool[] reRun)
        {
            for (int i = 0; i < reRun.Length; i++)
            {
                if (reRun[i]) return true;
            }
            return false;
        }
        
        Tuple<Population, bool[]> Postprocess(Population pop, int maxdegree)
        {
            //Console.WriteLine("Preprocessing with max degree " + maxdegree);
            int size = pop.GetPopulationSize();
            Population offspring = new Population();
            bool[] found = new bool[size];
            
            for (int i = 0; i < size; i++)
            {
                Individual p = pop.Get(i);
                Individual o = (Individual) p.Clone();
                found[i] = false;

                // get all undetoured edges
                List<Edge> nondetouredEdges = p.GetUndetoured();
                // monitoring repairs
                Dictionary<Edge, int> repairs = new Dictionary<Edge, int>();
                // getting degrees
                List<int> degrees = p.GetDegrees();
                int max = 0;
                int counter = 0;
                // if there is any nondetoured edge
                if (nondetouredEdges.Count > 0)
                {
                    foreach (var edge in nondetouredEdges)
                    {
                        // set activity, check number of detour then and set activity back
                        p.SetActivityOnEdge(edge.ID, 1);
                        if (p.GetMaxDegree() < maxdegree)
                        {
                            int count = p.GetUndetoured().Count();
                            repairs.Add(edge, count);
                            if (count > max)
                            {
                                max = count;
                                counter = 1;
                            }
                            else if (count == max) counter++;
                        }

                        p.SetActivityOnEdge(edge.ID, 0);
                    }
                    if (repairs.Count > 0) found[i] = true;
                    //Console.WriteLine("Repairs " + repairs.Count);
                    // find the edge that solves most problems
                    foreach (var edge in repairs.Keys)
                    {
                        //Console.WriteLine(edge + " " + repairs[edge] + " " + max);
                        if (repairs[edge] == max)
                        {
                            o.SetActivityOnEdge(edge.ID, 1);
                        }
                    }
                }
                o.changed = true;
                offspring.Add(o);
            }
            return new Tuple<Population, bool[]>(offspring, found);
        }
    }
}
