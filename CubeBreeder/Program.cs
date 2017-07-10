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
        public static GraphInfo graph;
        public static int localDetourSpanners = 0;

        static void Main(string[] args)
        {
            Settings s = Settings.GetInstance();
            Tools t = Tools.GetInstance(s.cubeDimension);
            graph = GraphInfo.GetInstance(s.cubeDimension);

            //Console.WriteLine(Environment.ProcessorCount);

            /////////////////////////////////
            //                             //
            //      Run the algorithm      //
            //                             //
            /////////////////////////////////

            Console.WriteLine("Dimension: " + s.cubeDimension);
            //Console.WriteLine("Population: " + (s.edgeCount * 10));
            Console.WriteLine();

            List<Individual> bestInds = new List<Individual>();

            Run[] runs = new Run[s.repeats];
            for (int i = 0; i < s.repeats; i++)
            {
                runs[i] = new Run(i);
            }
            var bests = new List<Individual>();

            //List<Task<Individual>> tasks = new List<Task<Individual>>();

            if (Settings.paralell)
            {
                
                Parallel.ForEach(runs, r =>
                {
                    Individual best = r.RunIt();
                    lock (bestInds)
                    { // lock the list to avoid race conditions
                        bestInds.Add(best);
                    }
                });

                /*
                for (int i = 0; i < s.repeats; i++)
                {
                    Task<Individual> task = new Task<Individual>(() => runs[i].RunIt());
                    tasks.Add(task);
                    task.RunSynchronously();
                }                
                Task.WaitAll(tasks.ToArray());*/
                /*
                foreach (var task in tasks)
                {
                    bests.Add(task.Result);
                }
                */
                /*
                Parallel.For(0, s.repeats, i =>
                {
                    bests[i] = runs[i].RunIt();
                });*/
            }
            else
            {
                for (int i = 0; i < s.repeats; i++)
                {
                    RandomNumberGenerator.GetInstance().ReSeed(i);
                    bests.Add(runs[i].RunIt());
                }
            }

            foreach (var best in bests)
            {
                if (best != null) bestInds.Add(best);
            }

            for (int i = 0; i < bestInds.Count; i++)
            {
                Console.WriteLine("Run " + (i + 1) + ": best objective=" + bestInds[i].GetObjectiveValue());
                Tools.WriteIndividual(bestInds[i]);
            }
        }
    }
}
