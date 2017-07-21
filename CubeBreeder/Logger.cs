using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder
{
    /// <summary>
    /// Logger class
    /// </summary>
    class Logger
    {
        int id;
        string dateTimeFormat;
        string fileName;

        /// <summary>
        /// Logging levels
        /// </summary>
        public enum Level
        {
            NONE,
            DEBUG,
            GENERATION,
            SETTINGS,
            EXCEPTION,
            INFO
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">run id</param>
        /// <param name="dim">dimension</param>
        public Logger(int id, int dim)
        {
            this.fileName = Settings.outputDir;
            this.id = id;
            dateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";
            fileName = fileName + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + "__Dim-" + dim + "_Run-" + id + ".log";

            // Log file header line
            string logHeader = fileName + " is created.";
            if (!File.Exists(fileName))
            {
                Console.WriteLine(logHeader);
            }
        }

        /// <summary>
        /// Logs information about current generation
        /// </summary>
        /// <param name="pop">population in the generation</param>
        /// <param name="sorted">sorted individuals</param>
        /// <param name="gen">number of generation</param>
        public void Log(Population pop, List<Individual> sorted, int gen)
        {
            int idx = 0;
            //while (idx < pop.GetPopulationSize() && sorted[idx].Is_3_Spanner(false) < 1) idx++;
            Log(Level.NONE, "");
            Log(Level.GENERATION, "Generation:\t" + (gen + 1).ToString());
            Log(Level.GENERATION, "Objective:\t" + sorted[idx].GetObjectiveValue());
            Log(Level.GENERATION, "Best valid:\t" + idx);
            Log(Level.GENERATION, "Fitness:\t\t" + String.Format("{0:f1}", sorted[0].GetFitnessValue()));
            Log(Level.GENERATION, "3-spanner:\t" + String.Format("{0:f2}", 
                (float)(Program.localDetourSpanners * 100.0 / pop.GetPopulationSize())) + "%");
            Log(Level.GENERATION, "Average:\t\t" + String.Format("{0:f0}", pop.GetAverage()));
            Log(Level.GENERATION, "Median:\t\t" + String.Format("{0:f0}", 
                sorted[pop.GetPopulationSize() / 2].GetFitnessValue()));
        }

        /// <summary>
        /// Logs settings of the program
        /// </summary>
        /// <param name="s"></param>
        public void Log(Settings s)
        {
            Log(Level.SETTINGS, "Hypercube Dimension:\t" + s.cubeDimension);
            Log(Level.SETTINGS, "Population size:\t\t" + s.popSize);
            Log(Level.SETTINGS, "Elite size:\t\t\t" + String.Format("{0:f2}", s.eliteSize));
            Log(Level.SETTINGS, "Maximum generations:\t" + s.maxGen);
            Log(Level.SETTINGS, "Mutation probability:\t" + String.Format("{0:f2}", s.mutProb));
            Log(Level.SETTINGS, "Mutation Per Item:\t\t" + String.Format("{0:f2}", s.mutProbPerBit));
            Log(Level.SETTINGS, "Repair probability:\t\t" + String.Format("{0:f2}", s.mutRepair));
            Log(Level.SETTINGS, "Crossover probability:\t" + String.Format("{0:f2}", s.xoverProb));
            Log(Level.SETTINGS, "Subcube size:\t\t" + s.subcubeSize);
            Log(Level.SETTINGS, "N in N-points:\t\t" + s.nPoints);
            //Log(Level.SETTINGS, "\t" + s);
        }

        /// <summary>
        /// Logs individual ind
        /// </summary>
        /// <param name="ind">individual to be logged</param>
        public void Log(Individual ind)
        {
            Log(Level.INFO, "Best individual has objective " + ind.GetFitnessValue());
        }

        /// <summary>
        /// Logs a string text at level level
        /// </summary>
        /// <param name="level">logging level</param>
        /// <param name="text">string to be logged</param>
        public void Log(Level level, string text)
        {
            string pretext = DateTime.Now.ToString(dateTimeFormat) + " [";
            switch (level)
            {
                case Level.NONE: pretext += "NONE] \t"; break;
                case Level.INFO: pretext += "INFO] \t"; break;
                case Level.DEBUG: pretext += "DEBUG] \t"; break;
                case Level.GENERATION: pretext += "GENERATION] \t"; break;
                case Level.SETTINGS: pretext += "SETTINGS] \t"; break;
                case Level.EXCEPTION: pretext += "EXCEPTION] \t"; break;
                default: pretext = ""; break;
            }
            WriteLine(pretext + text);
        }

        /// <summary>
        /// Writes text to the logfile
        /// </summary>
        /// <param name="text">text to be logged</param>
        private void WriteLine(string text)
        {
            try
            {
                using (StreamWriter Writer = new StreamWriter(fileName, true))
                {
                    if (text != "" ) Writer.WriteLine(text);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Logger error: " + e.Message);
            }
        }
    }
}
