using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CubeBreeder;

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

        static Tools tools;
        
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

            GraphInfo graph = new GraphInfo(cubeDimension);

            /////////////////////////////////
            //                             //
            //      Run the algorithm      //
            //                             //
            /////////////////////////////////
            /*
            List<CubeIndividual> bestInds = new List<CubeIndividual>();

            for (int i = 0; i < repeats; i++)
            {
                CubeIndividual best = Run(i);
                if (best != null) bestInds.Add(best);
            }
            for (int i = 0; i < bestInds.Count; i++)
            {
                Console.WriteLine("Run " + (i + 1) + ": best objective=" + bestInds[i].GetObjectiveValue());
                Tools.WriteIndividual(bestInds[i]);
            }*/
        }
    }
}
