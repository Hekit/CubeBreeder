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
using System.Globalization;

namespace CubeBreeder
{
    /// <summary>
    /// Settings of the program
    /// </summary>
    class Settings
    {
        public int maxGen;
        public int popSize;
        public int vertexCount;
        public int edgeCount;
        public int cubeDimension;
        public static int subCubeMaxSize;
        public double xoverProb;
        public double mutProb;
        public double mutProbPerBit;
        public double mutRepair;
        public int repeats;
        public int subcubeSize;
        public int nPoints;
        public double eliteSize;
        public string logFolder;
        public static int localDetourSpanners = 0;
        public static int maxColours = 1;
        public static bool parallel;
        public static int showGap = 1;
        public static int activeProbability;
        public static string inputFolderPath;
        public static double fileUsage = 0;
        public static bool fileInitialization;
        public static int competitors = 2;
        public static double tourWeakerProb = 0.2;
        public static string task;
        public static double changingSubcube;
        public static string outputDir = ".";

        private static Settings theInstance = null;

        /// <summary>
        /// Constructor
        /// </summary>
        private Settings()
        {
            // initialize from Settings
            InitializeSettings();
            // if a config file is provided, overrun the initialization
            if (System.IO.File.Exists(@".\config.txt"))
            {
                LoadSettings();
            }
        }

        /// <summary>
        /// Setup of the EA
        /// </summary>
        /// <param name="logger">Logger to log the configuration</param>
        /// <returns></returns>
        public EvolutionaryAlgorithm GetEVA(Logger logger, Random rng)
        {
            // initialize from file
            if (System.IO.File.Exists(@".\config.txt"))
            {
                return LoadEVA(logger, rng);
            }
            // or from default
            else
            {
                return InitializeEVA(logger, rng);
            }
        }

        /// <summary>
        /// Get instance of settings
        /// </summary>
        /// <returns>return instence of settings</returns>
        public static Settings GetInstance()
        {
            if (theInstance == null)
            {
                theInstance = new Settings();
            }
            return theInstance;
        }

        /// <summary>
        /// Initialize all the basic settings
        /// </summary>
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
            // Number of edges
            edgeCount = (int)Math.Pow(2, cubeDimension - 1) * cubeDimension;
            // Number of vertices
            vertexCount = (int)Math.Pow(2, cubeDimension);
            // Maximu size of a subcube
            subCubeMaxSize = cubeDimension - 1;
            // Initialization probability
            activeProbability = Properties.Settings.Default.P_ActiveProbability;
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
            // Number of points in n-point Xover
            nPoints = Properties.Settings.Default.NPoints;
            // Size of the elite
            eliteSize = Properties.Settings.Default.EliteSize;
            // Count of competitors for tournament
            competitors = Properties.Settings.Default.TournamentCompetitors;
            // Probability of a weaker individual winning a tournament fight
            tourWeakerProb = Properties.Settings.Default.TournamentWeakerChance;
            // Probability of changing the size of a subcube
            changingSubcube = Properties.Settings.Default.ChangingSubcubeSize;
            // Is program running in parallel?
            parallel = Properties.Settings.Default.Parallel;
            // Initialize from file
            fileInitialization = Properties.Settings.Default.File_Initialization;
            // How much of the population should be initialized from file
            fileUsage = Properties.Settings.Default.FileInitRatio;
            // Input folder for initialization
            inputFolderPath = @".";
            // Gap for displaying info on console during runtime
            showGap = Properties.Settings.Default.ShowGap;


            // Set the task
            if (Properties.Settings.Default.Task == "spanner")
            {
                maxColours = 1;
                task = "spanner";
            }
            else if (Properties.Settings.Default.Task == "degree")
            {
                maxColours = 1;
                task = "degree";
            }
            else
            {
                maxColours = cubeDimension / 2;
                task = "eds";
            }
        }

        /// <summary>
        /// Default EA settings
        /// </summary>
        /// <param name="logger">logger to log the settings</param>
        /// <returns>configuration of EA</returns>
        private EvolutionaryAlgorithm InitializeEVA(Logger logger, Random rng)
        {
            EvolutionaryAlgorithm ea;
            //Set the options for the evolutionary algorithm
            ea = new EvolutionaryAlgorithm();
            // Fitness function
            if (Properties.Settings.Default.Task == "spanner")
            {
                ea.SetFitnessFunction(new SpannerFitness(edgeCount));
                logger.Log(Logger.Level.SETTINGS, "Spanner");
                task = "spanner";
            }
            else if (Properties.Settings.Default.Task == "degree")
            {
                ea.SetFitnessFunction(new MaxDegreeFitness(cubeDimension));
                logger.Log(Logger.Level.SETTINGS, "MaxDegree");
                task = "degree";
            }
            else
            {
                ea.SetFitnessFunction(new EdgeDisjointSpanner(cubeDimension));
                logger.Log(Logger.Level.SETTINGS, "EDS");
                task = "eds";
            }
            // Selectors
            //ea.AddMatingSelector(new RouletteWheelSelector());
            ea.AddMatingSelector(new TournamentSelector(tourWeakerProb, competitors, rng));
            //ea.AddMatingSelector(new BoltzmannTournamentSelector(maxGen));
            //ea.AddMatingSelector(new BoltzmannRouletteWheelSelector(maxGen));
            // Operators
            ea.AddOperator(new SubcubeSwapXOver(xoverProb, subcubeSize, rng));
            //ea.AddOperator(new NPointXOver(xoverProb, nPoints));
            //ea.AddOperator(new SimpleRepairEdgeMutation(mutProb, mutRepair));
            //ea.AddOperator(new CleverRepairEdgeMutation(mutProb / 100, mutRepair));
            ea.AddOperator(new FlipEdgeMutation(mutProb, mutProbPerBit, rng));
            ea.AddOperator(new SubcubeTranslationMutation(mutProb, 2, rng));
            ea.AddOperator(new SubcubeRotationMutation(mutProb, 2, rng));
            //ea.AddEnvironmentalSelector(new RouletteWheelSelector());
            ea.AddEnvironmentalSelector(new TournamentSelector(tourWeakerProb, competitors, rng));
            //ea.AddEnvironmentalSelector(new BoltzmannTournamentSelector(maxGen));
            //ea.AddEnvironmentalSelector(new BoltzmannRouletteWheelSelector(maxGen));

            ea.SetElite(eliteSize);

            return ea;
        }

        /// <summary>
        /// Loads EA settings from a config file
        /// </summary>
        /// <param name="logger">logger to log the configuration</param>
        /// <returns>configuration of EA</returns>
        private EvolutionaryAlgorithm LoadEVA(Logger logger, Random rnd)
        {
            EvolutionaryAlgorithm ea = new EvolutionaryAlgorithm();
            string[] lines = new string[1];
            try
            {
                lines = System.IO.File.ReadAllLines(@".\config.txt");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

            for (int i = 0; i < lines.Length; i++)
            {
                string thisLine = lines[i];
                // commentary
                if (thisLine.Length < 1 || thisLine[0] == '#')
                    continue;
                var line = thisLine.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                switch (line[0])
                {
                    // selection
                    case "RouletteWheel":
                        if (line.GetOrElse(1, "") == "ENV" || line.GetOrElse(2, "") == "ENV")
                        {
                            ea.AddEnvironmentalSelector(new RouletteWheelSelector(rnd));
                        }
                        if (line.GetOrElse(1, "") == "MAT" || line.GetOrElse(2, "") == "MAT")
                        {
                            ea.AddMatingSelector(new RouletteWheelSelector(rnd));
                        }
                        if (line.Length < 2)
                        {
                            ea.AddEnvironmentalSelector(new RouletteWheelSelector(rnd));
                            ea.AddMatingSelector(new RouletteWheelSelector(rnd));
                        }
                        break;
                    case "Tournament":
                        if (line.GetOrElse(2, "") == "ENV" || line.GetOrElse(3, "") == "ENV")
                        {
                            ea.AddEnvironmentalSelector(new TournamentSelector(
                                tourWeakerProb,
                                line.ParseIntOrElse(1, Properties.Settings.Default.TournamentCompetitors),
                                rnd));
                        }
                        if (line.GetOrElse(2, "") == "MAT" || line.GetOrElse(3, "") == "MAT")
                            ea.AddMatingSelector(new TournamentSelector(
                                tourWeakerProb,
                                line.ParseIntOrElse(1, Properties.Settings.Default.TournamentCompetitors),
                                rnd));
                        if (line.Length < 3)
                        {
                            ea.AddEnvironmentalSelector(new TournamentSelector(
                                tourWeakerProb,
                                line.ParseIntOrElse(1, Properties.Settings.Default.TournamentCompetitors),
                                rnd));
                            ea.AddMatingSelector(new TournamentSelector(
                                tourWeakerProb,
                                line.ParseIntOrElse(1, Properties.Settings.Default.TournamentCompetitors),
                                rnd));
                        }
                        break;
                    case "BoltzmannTournament":
                        if (line.GetOrElse(2, "") == "ENV" || line.GetOrElse(3, "") == "ENV")
                        {
                            ea.AddEnvironmentalSelector(new BoltzmannTournamentSelector(maxGen,
                                tourWeakerProb,
                                line.ParseIntOrElse(1, Properties.Settings.Default.TournamentCompetitors),
                                rnd));
                        }
                        if (line.GetOrElse(2, "") == "MAT" || line.GetOrElse(3, "") == "MAT")
                            ea.AddMatingSelector(new BoltzmannTournamentSelector(maxGen,
                                tourWeakerProb,
                                line.ParseIntOrElse(1, Properties.Settings.Default.TournamentCompetitors),
                                rnd));
                        if (line.Length < 3)
                        {
                            ea.AddEnvironmentalSelector(new BoltzmannTournamentSelector(maxGen,
                                tourWeakerProb,
                                line.ParseIntOrElse(1, Properties.Settings.Default.TournamentCompetitors),
                                rnd));
                            ea.AddMatingSelector(new BoltzmannTournamentSelector(maxGen,
                                tourWeakerProb,
                                line.ParseIntOrElse(1, Properties.Settings.Default.TournamentCompetitors),
                                rnd));
                        }
                        break;
                    case "BoltzmannRouletteWheel":
                        if (line.GetOrElse(1, "") == "ENV" || line.GetOrElse(2, "") == "ENV")
                        {
                            ea.AddEnvironmentalSelector(new BoltzmannRouletteWheelSelector(maxGen, rnd));
                        }
                        if (line.GetOrElse(1, "") == "MAT" || line.GetOrElse(2, "") == "MAT")
                        {
                            ea.AddMatingSelector(new BoltzmannRouletteWheelSelector(maxGen, rnd));
                        }
                        if (line.Length < 2)
                        {
                            ea.AddEnvironmentalSelector(new BoltzmannRouletteWheelSelector(maxGen, rnd));
                            ea.AddMatingSelector(new BoltzmannRouletteWheelSelector(maxGen, rnd));
                        }
                        break;

                    // operators
                    case "SubcubeSwap":
                        ea.AddOperator(
                            new SubcubeSwapXOver(
                                line.ParseDoubleOrElse(1, Properties.Settings.Default.P_CrossoverProbability),
                                line.ParseIntOrElse(2, Properties.Settings.Default._SubcubeSize),
                                rnd));
                        break;
                    case "N-Point":
                        ea.AddOperator(
                            new NPointXOver(
                                line.ParseDoubleOrElse(1, Properties.Settings.Default.P_CrossoverProbability),
                                line.ParseIntOrElse(2, Properties.Settings.Default.NPoints),
                                rnd));
                        break;
                    case "CleverRepair":
                        ea.AddOperator(
                            new CleverRepairEdgeMutation(
                                line.ParseDoubleOrElse(1, Properties.Settings.Default.P_MutationProbability),
                                rnd));
                        break;
                    case "SimpleRepair":
                        ea.AddOperator(
                            new SimpleRepairEdgeMutation(
                                line.ParseDoubleOrElse(1, Properties.Settings.Default.P_MutationProbability),
                                line.ParseDoubleOrElse(2, Properties.Settings.Default.P_MutationPerEdgeProbability),
                                rnd));
                        break;
                    case "Translation":
                        ea.AddOperator(
                            new SubcubeTranslationMutation(
                                line.ParseDoubleOrElse(1, Properties.Settings.Default.P_MutationProbability),
                                line.ParseIntOrElse(2, Properties.Settings.Default._SubcubeSize),
                                rnd));
                        break;
                    case "Rotation":
                        ea.AddOperator(
                            new SubcubeRotationMutation(
                                line.ParseDoubleOrElse(1, Properties.Settings.Default.P_MutationProbability),
                                line.ParseIntOrElse(2, Properties.Settings.Default._SubcubeSize),
                                rnd));
                        break;
                    case "FlipEdge":
                        ea.AddOperator(
                            new FlipEdgeMutation(
                                line.ParseDoubleOrElse(1, Properties.Settings.Default.P_MutationProbability),
                                line.ParseDoubleOrElse(2, Properties.Settings.Default.P_MutationPerEdgeProbability),
                                rnd));
                        break;

                    // fitness
                    case "Spanner":
                        ea.SetFitnessFunction(new SpannerFitness(edgeCount));
                        logger.Log(Logger.Level.SETTINGS, "Spanner");
                        break;
                    case "Degree":
                        ea.SetFitnessFunction(new MaxDegreeFitness(cubeDimension));
                        logger.Log(Logger.Level.SETTINGS, "MaxDegree");
                        break;
                    case "EdgeDisjoint":
                        ea.SetFitnessFunction(new EdgeDisjointSpanner(cubeDimension));
                        logger.Log(Logger.Level.SETTINGS, "EDS");
                        break;
                    default:
                        break;
                }
            }
            return ea;
        }

        /// <summary>
        /// Load general configuration from a config file
        /// </summary>
        private void LoadSettings()
        {
            string[] lines = new string[1];
            try
            {
                lines = System.IO.File.ReadAllLines(@".\config.txt");
            }
            catch  (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

            for (int i = 0; i < lines.Length; i++)
            {
                string thisLine = lines[i];
                // commentary
                if (thisLine.Length < 1 || thisLine[0] == '#')
                    continue;
                var line = thisLine.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                switch (line[0])
                {
                    // settings
                    case "PopulationSize":
                        popSize = line.ParseIntOrElse(1, Properties.Settings.Default._PopulationSize);
                        break;
                    case "Dimension":
                        cubeDimension = line.ParseIntOrElse(1, Properties.Settings.Default._Dimension);
                        edgeCount = (int)Math.Pow(2, cubeDimension - 1) * cubeDimension;
                        vertexCount = (int)Math.Pow(2, cubeDimension);
                        subCubeMaxSize = cubeDimension - 1;
                        break;
                    case "Generations":
                        maxGen = line.ParseIntOrElse(1, Properties.Settings.Default._MaxGenerations);
                        break;
                    case "EliteSize":
                        eliteSize = line.ParseDoubleOrElse(1, Properties.Settings.Default.EliteSize);
                        break;
                    case "OutputRate":
                        showGap = line.ParseIntOrElse(1, Properties.Settings.Default.ShowGap);
                        break;
                    case "Repeats":
                        repeats = line.ParseIntOrElse(1, Properties.Settings.Default.Repeats);
                        break;
                    case "Parallel":
                        parallel = true;
                        break;
                    case "OutputDir":
                        outputDir = line.GetOrElse(1, ".");
                        break;

                    // initialization
                    case "InputFile":
                        inputFolderPath = line.GetOrElse(1, ".");
                        if (fileUsage > 0) fileInitialization = true;   
                        break;
                    case "Percentage":
                        activeProbability = line.ParseIntOrElse(1, Properties.Settings.Default.P_ActiveProbability);
                        break;
                    case "FileUsage":
                        fileUsage = line.ParseDoubleOrElse(1, Properties.Settings.Default.FileInitRatio);
                        if (fileUsage > 0) fileInitialization = true;
                        break;

                    // probability
                    case "Mutation":
                        mutProb = line.ParseDoubleOrElse(1, Properties.Settings.Default.P_MutationProbability);
                        break;
                    case "Crossover":
                        xoverProb = line.ParseDoubleOrElse(1, Properties.Settings.Default.P_CrossoverProbability);
                        break;
                    case "MutationPerItem":
                        mutProbPerBit = line.ParseDoubleOrElse(1, Properties.Settings.Default.P_MutationPerEdgeProbability);
                        break;
                    case "TournamentWeaker":
                        tourWeakerProb = line.ParseDoubleOrElse(1, Properties.Settings.Default.TournamentWeakerChance);
                        break;
                    case "ChangingSubcube":
                        changingSubcube = line.ParseDoubleOrElse(1, Properties.Settings.Default.ChangingSubcubeSize);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
