using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CubeBreeder.Selectors;
using CubeBreeder.Fitness;
using CubeBreeder.Operators.Crossovers;
using CubeBreeder.Operators.Mutations;
using CubeBreeder.Replacements;

namespace CubeBreeder
{
    class Settings
    {
        public int maxGen;
        public int popSize;
        public int vertexCount;
        public int edgeCount;
        public int cubeDimension;
        public double xoverProb;
        public double mutProb;
        public double mutProbPerBit;
        public double mutRepair;
        public int repeats;
        public int subcubeSize;
        public double eliteSize;
        public static int localDetourSpanners = 0;
        public static int maxColours;
        public static bool paralell;
        public static int showGap;
        
        private static Settings theInstance = null;

        private Settings()
        {
            InitializeSettings();
            //InitializeEVA();
            LoadSettings();
        }

        public static Settings GetInstance()
        {
            if (theInstance == null)
            {
                theInstance = new Settings();
            }
            return theInstance;
        }

        private void InitializeSettings()
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

            if (Properties.Settings.Default.Task == "spanner")
            {
                maxColours = 1;
            }
            else if (Properties.Settings.Default.Task == "degree")
            {
                maxColours = 1;
            }
            else maxColours = cubeDimension / 2;
        }

        public EvolutionaryAlgorithm InitializeEVA()
        { 
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
            //ea.AddOperator(new SubcubeRotationMutation(mutProb, 3));
            //ea.AddEnvironmentalSelector(new RouletteWheelSelector());
            ea.AddEnvironmentalSelector(new TournamentSelector());

            ea.SetElite(eliteSize);

            return ea;
        }

        private void LoadSettings()
        {
            popSize = edgeCount * 10;
            showGap = 25;
        }

    }
}
