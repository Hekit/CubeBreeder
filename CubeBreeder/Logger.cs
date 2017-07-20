using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder
{
    class Logger
    {
        string path;
        FileStream log;
        DateTime start;
        int id;
        string dateTimeFormat;
        string fileName;

        public enum Level
        {
            NONE,
            DEBUG,
            GENERATION,
            SETTINGS,
            EXCEPTION,
            INFO
        }

        public Logger(int id, int dim)
        {
            this.id = id;
            dateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff";
            fileName = "D:\\Development\\hypercubes\\" +
                DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + "__Dim-" + dim + "_Run-" + id + ".log";

            // Log file header line
            string logHeader = fileName + " is created.";
            if (!File.Exists(fileName))
            {
                Console.WriteLine(logHeader);
            }
        }

        public void Log(Population pop, List<Individual> sorted, int gen)
        {
            int idx = 0;
            //while (idx < pop.GetPopulationSize() && sorted[idx].Is_3_Spanner(false) < 1) idx++;
            Log(Level.NONE, "");
            Log(Level.GENERATION, "Generation:\t" + (gen + 1).ToString());
            Log(Level.GENERATION, "Objective:\t" + sorted[idx].GetObjectiveValue());
            Log(Level.GENERATION, "Best valid:\t" + idx);
            Log(Level.GENERATION, "Fitness:\t" + String.Format("{0:f1}", sorted[0].GetFitnessValue()));
            Log(Level.GENERATION, "3-spanner:\t" + String.Format("{0:f2}", 
                (float)(Program.localDetourSpanners * 100.0 / pop.GetPopulationSize())) + "%");
            Log(Level.GENERATION, "Average:\t" + String.Format("{0:f0}", pop.GetAverage()));
            Log(Level.GENERATION, "Median:\t\t" + String.Format("{0:f0}", 
                sorted[pop.GetPopulationSize() / 2].GetFitnessValue()));
        }

        public void Log(Settings s)
        {
            Log(Level.SETTINGS, "Hypercube Dimension:\t" + s.cubeDimension);
            Log(Level.SETTINGS, "Population size:\t" + s.popSize);
            Log(Level.SETTINGS, "Elite size:\t\t" + String.Format("{0:f2}", s.eliteSize));
            Log(Level.SETTINGS, "Maximum generations:\t" + String.Format("{0:f2}", s.maxGen));
            Log(Level.SETTINGS, "Mutation probability:\t" + String.Format("{0:f2}", s.mutProb));
            Log(Level.SETTINGS, "Mutation Per Item:\t" + String.Format("{0:f2}", s.mutProbPerBit));
            Log(Level.SETTINGS, "Repair probability:\t" + String.Format("{0:f2}", s.mutRepair));
            Log(Level.SETTINGS, "Crossover probability:\t" + String.Format("{0:f2}", s.xoverProb));
            Log(Level.SETTINGS, "Subcube size:\t" + s.subcubeSize);
            Log(Level.SETTINGS, "N in N-points:\t" + s.nPoints);
            //Log(Level.SETTINGS, "\t" + s);
        }

        public void Log(Individual ind)
        {
            Log(Level.INFO, "Best individual has objective " + ind.GetFitnessValue());
        }

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
