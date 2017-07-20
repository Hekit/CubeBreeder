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
        public static int localDetourSpanners = 0;
        public static int maxColours = 1;
        public static bool paralell;
        public static int showGap = 1;
        public static int activeProbability;
        public static string inputFolderPath;
        public static double fileUsage = 0;
        public static bool fileInitialization;
        public static int competitors = 2;
        public static double tourWeakerProb = 0.2;
        public static string task;
        public static double changingSubcube;


        private static Settings theInstance = null;

        private Settings()
        {
            InitializeSettings();
            if (System.IO.File.Exists(@"..\..\Resources\config.txt"))
            {
                LoadSettings();
            }
        }

        public EvolutionaryAlgorithm GetEVA(Logger logger)
        {
            if (System.IO.File.Exists(@"..\..\Resources\config.txt"))
            {
                return LoadEVA(logger);
            }
            else
            {
                return InitializeEVA(logger);
            }
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
            // 
            tourWeakerProb = Properties.Settings.Default.TournamentWeakerChance;
            //
            changingSubcube = Properties.Settings.Default.ChangingSubcubeSize;

            paralell = Properties.Settings.Default.Paralell;
            fileInitialization = Properties.Settings.Default.File_Initialization;
            fileUsage = Properties.Settings.Default.FileInitRatio;
            inputFolderPath = @"D:\Development\hypercubes\initialization\";
            showGap = Properties.Settings.Default.ShowGap;

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

        private EvolutionaryAlgorithm InitializeEVA(Logger logger)
        {
            EvolutionaryAlgorithm ea;
            //Set the options for the evolutionary algorithm
            ea = new EvolutionaryAlgorithm();
            // Fitness function
            if (Properties.Settings.Default.Task == "spanner")
            {
                ea.SetFitnessFunction(new SpannerFitness(edgeCount));
                task = "spanner";
            }
            else if (Properties.Settings.Default.Task == "degree")
            {
                ea.SetFitnessFunction(new MaxDegreeFitness(cubeDimension));
                task = "degree";
            }
            else
            {
                ea.SetFitnessFunction(new EdgeDisjointSpanner(cubeDimension));
                task = "eds";
            }
            // Selectors
            //ea.AddMatingSelector(new RouletteWheelSelector());
            ea.AddMatingSelector(new TournamentSelector());
            //ea.AddMatingSelector(new BoltzmannTournamentSelector(maxGen));
            //ea.AddMatingSelector(new BoltzmannRouletteWheelSelector(maxGen));
            // Operators
            ea.AddOperator(new SubcubeSwapXOver(xoverProb, subcubeSize));
            //ea.AddOperator(new NPointXOver(xoverProb, nPoints));
            //ea.AddOperator(new SimpleRepairEdgeMutation(mutProb, mutRepair));
            //ea.AddOperator(new CleverRepairEdgeMutation(mutProb / 100, mutRepair));
            ea.AddOperator(new FlipEdgeMutation(mutProb, mutProbPerBit));
            ea.AddOperator(new SubcubeTranslationMutation(mutProb, 2));
            ea.AddOperator(new SubcubeRotationMutation(mutProb, 2));
            //ea.AddEnvironmentalSelector(new RouletteWheelSelector());
            ea.AddEnvironmentalSelector(new TournamentSelector());
            //ea.AddEnvironmentalSelector(new BoltzmannTournamentSelector(maxGen));
            //ea.AddEnvironmentalSelector(new BoltzmannRouletteWheelSelector(maxGen));

            ea.SetElite(eliteSize);

            return ea;
        }

        private EvolutionaryAlgorithm LoadEVA(Logger logger)
        {
            EvolutionaryAlgorithm ea = new EvolutionaryAlgorithm();
            string[] lines = new string[1];
            try
            {
                lines = System.IO.File.ReadAllLines(@"..\..\Resources\config.txt");
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
                            ea.AddEnvironmentalSelector(new RouletteWheelSelector());
                            logger.Log(Logger.Level.SETTINGS, "Roulette ENV");
                        }
                        if (line.GetOrElse(1, "") == "MAT" || line.GetOrElse(2, "") == "MAT")
                        {
                            ea.AddMatingSelector(new RouletteWheelSelector());
                            logger.Log(Logger.Level.SETTINGS, "Roulette ENV");
                        }
                        if (line.Length < 2)
                        {
                            ea.AddEnvironmentalSelector(new RouletteWheelSelector());
                            ea.AddMatingSelector(new RouletteWheelSelector());
                            logger.Log(Logger.Level.SETTINGS, "Roulette ENV MAT");
                        }
                        break;
                    case "Tournament":
                        // competitors = Properties.Settings.Default.TournamentCompetitors;
                        if (line.GetOrElse(1, "") == "ENV" || line.GetOrElse(2, "") == "ENV")
                        {
                            ea.AddEnvironmentalSelector(new TournamentSelector());
                            logger.Log(Logger.Level.SETTINGS, "Tournament");
                        }
                        if (line.GetOrElse(1, "") == "MAT" || line.GetOrElse(2, "") == "MAT")
                            ea.AddMatingSelector(new TournamentSelector());
                        if (line.Length < 2)
                        {
                            ea.AddEnvironmentalSelector(new TournamentSelector());
                            ea.AddMatingSelector(new TournamentSelector());
                        }
                        break;
                    case "BoltzmanTournament":
                        throw new NotImplementedException();
                        break;
                    case "BoltzmanRouletteWheel":
                        throw new NotImplementedException();
                        break;

                    // operators
                    case "SubcubeSwap":
                        ea.AddOperator(
                            new SubcubeSwapXOver(
                                line.ParseDoubleOrElse(1, Properties.Settings.Default.P_CrossoverProbability),
                                line.ParseIntOrElse(2, Properties.Settings.Default._SubcubeSize)));
                        break;
                    case "N-Point":
                        ea.AddOperator(
                            new NPointXOver(
                                line.ParseDoubleOrElse(1, Properties.Settings.Default.P_CrossoverProbability),
                                line.ParseIntOrElse(2, Properties.Settings.Default.NPoints)));
                        break;
                    case "CleverRepair":
                        ea.AddOperator(
                            new CleverRepairEdgeMutation(
                                line.ParseDoubleOrElse(1, Properties.Settings.Default.P_MutationProbability)));
                        break;
                    case "SimpleRepair":
                        ea.AddOperator(
                            new SimpleRepairEdgeMutation(
                                line.ParseDoubleOrElse(1, Properties.Settings.Default.P_MutationProbability),
                                line.ParseDoubleOrElse(2, Properties.Settings.Default.P_MutationPerEdgeProbability)));
                        break;
                    case "Translation":
                        ea.AddOperator(
                            new SubcubeTranslationMutation(
                                line.ParseDoubleOrElse(1, Properties.Settings.Default.P_MutationProbability),
                                line.ParseIntOrElse(2, Properties.Settings.Default._SubcubeSize)));
                        break;
                    case "Rotation":
                        ea.AddOperator(
                            new SubcubeRotationMutation(
                                line.ParseDoubleOrElse(1, Properties.Settings.Default.P_MutationProbability),
                                line.ParseIntOrElse(2, Properties.Settings.Default._SubcubeSize)));
                        break;
                    case "FlipEdge":
                        ea.AddOperator(
                            new FlipEdgeMutation(
                                line.ParseDoubleOrElse(1, Properties.Settings.Default.P_MutationProbability),
                                line.ParseDoubleOrElse(2, Properties.Settings.Default.P_MutationPerEdgeProbability)));
                        break;

                    // fitness
                    case "Spanner":
                        ea.SetFitnessFunction(new SpannerFitness(edgeCount));
                        break;
                    case "Degree":
                        ea.SetFitnessFunction(new MaxDegreeFitness(cubeDimension));
                        break;
                    case "EdgeDisjoint":
                        ea.SetFitnessFunction(new EdgeDisjointSpanner(cubeDimension));
                        break;
                    default:
                        break;
                }
            }
            return ea;
        }

        private void LoadSettings()
        {
            string[] lines = new string[1];
            try
            {
                lines = System.IO.File.ReadAllLines(@"..\..\Resources\config.txt");
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

                    // initialization
                    case "InputFile":
                        inputFolderPath = line.GetOrElse(1, "D:\\Development\\hypercubes\\initialization\\");
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
                        
                    default:
                        break;
                }
            }
        }
    }
}
