using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder
{
    /// <summary>
    /// Entry point to the application
    /// </summary>
    class Program
    {
        public static GraphInfo graph;
        public static int localDetourSpanners = 0;

        /// <summary>
        /// Main method
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // initialize settings, tools and the hypoercube graph
            Settings s = Settings.GetInstance();
            Tools t = Tools.GetInstance(s.cubeDimension);
            graph = GraphInfo.GetInstance(s.cubeDimension);

            /////////////////////////////////
            //                             //
            //      Run the algorithm      //
            //                             //
            /////////////////////////////////

            // information output
            Console.WriteLine("Dimension: " + s.cubeDimension);
            Console.WriteLine();

            List<Individual> bestInds = new List<Individual>();

            // initialize the array of runs
            Run[] runs = new Run[s.repeats];
            for (int i = 0; i < s.repeats; i++)
            {
                runs[i] = new Run(i);
            }
            var bests = new List<Individual>();

            // if parallel run it parallely
            if (Settings.parallel)
            {
                Parallel.ForEach(runs, r =>
                {
                    Individual best = r.RunIt();
                    lock (bestInds)
                    { // lock the list to avoid race conditions
                        bestInds.Add(best);
                    }
                });
            }
            // else just run it
            else
            {
                for (int i = 0; i < s.repeats; i++)
                {
                    RandomNumberGenerator.GetInstance().ReSeed(i);
                    bests.Add(runs[i].RunIt());
                }
            }

            // output best individuals
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
