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

            List<Tuple<Individual, int>> bestInds = new List<Tuple<Individual,int>>();

            // initialize the array of runs
            Run[] runs = new Run[s.repeats];
            for (int i = 0; i < s.repeats; i++)
            {
                runs[i] = new Run(i);
            }
            var bests = new List<Tuple<Individual,int>>();

            // if parallel run it parallely
            if (Settings.parallel)
            {
                Parallel.ForEach(runs, r =>
                {
                    Individual best = r.RunIt();
                    lock (bestInds)
                    { // lock the list to avoid race conditions
                        bestInds.Add(new Tuple<Individual, int>(best, r.number));
                    }
                });
            }
            // else just run it
            else
            {
                for (int i = 0; i < s.repeats; i++)
                {
                    bests.Add(new Tuple<Individual, int>(runs[i].RunIt(),i));
                }
            }

            // output best individuals
            foreach (var best in bests)
            {
                if (best != null) bestInds.Add(best);
            }

            bestInds.Sort((x, y) => x.Item2.CompareTo(y.Item2));

            Console.WriteLine();
            for (int i = 0; i < bestInds.Count; i++)
            {
                Console.WriteLine("Run " + (i + 1) + ": best objective=" + bestInds[i].Item1.GetObjectiveValue());
                Tools.WriteIndividual(bestInds[i].Item1);
            }
            Console.ReadLine();
        }
    }
}
