using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder
{
    /// <summary>
    /// Population class
    /// </summary>
    class Population
    {
        int size = 0;
        GraphInfo sampleIndividual;
        List<Individual> individuals;
        double average;

        Random rnd = new Random();

        /// <summary>
        /// Creates new empty population
        /// </summary>
        public Population()
        {
            individuals = new List<Individual>();
        }

        /// <summary>
        /// Clones the population
        /// </summary>
        /// <returns></returns>
        public Object Clone()
        {
            return this.MemberwiseClone();
        }

        /// <summary>
        /// Randomly shuffles the order of individuals in the population
        /// </summary>
        public void Shuffle()
        {
            individuals = (List<Individual>)individuals.OrderBy(item => rnd.Next());
        }

        #region Get-Set
        /// <summary>
        /// Sets the structure of the individuals (i.e. the hypercube graph)
        /// </summary>
        /// <param name="example">hypercube graph</param>
        public void SetSampleIndividual(GraphInfo example)
        {
            this.sampleIndividual = example;
        }

        /// <summary>
        /// Sets the population size
        /// </summary>
        /// <param name="size"></param>
        public void SetPopulationSize(int size)
        {
            this.size = size;
        }

        /// <summary>
        /// Returns the size of the population
        /// </summary>
        /// <returns>size of the population</returns>
        public int GetPopulationSize()
        {
            return individuals.Count();
        }

        /// <summary>
        /// Method to get all the individuals from the population
        /// </summary>
        /// <returns>list of individuals</returns>
        public List<Individual> GetIndividuals()
        {
            return individuals;
        }

        /// <summary>
        /// Gets the individuals sorted descendingly according to their fitness
        /// </summary>
        /// <returns>sorted list of individuals</returns>
        public List<Individual> GetSortedIndividuals()
        {

            List<Individual> sorted = new List<Individual>();
            sorted.AddRange(individuals);

            sorted.Sort(new FitnessFunctionComparator());

            return sorted;

        }

        /// <summary>
        /// Sets the population average
        /// </summary>
        /// <param name="average">population average</param>
        public void SetAverage(double average)
        {
            this.average = average;
        }

        /// <summary>
        /// Gets the population average
        /// </summary>
        /// <returns>population average</returns>
        public double GetAverage()
        {
            return average;
        }

        /// <summary>
        /// Gets the individual at index i
        /// </summary>
        /// <param name="i">the index of the individual that is returned</param>
        /// <returns>the individual at index i</returns>
        public Individual Get(int i)
        {
            return individuals[i];
        }
        #endregion Get-Set

        /// <summary>
        /// Adds an individual to the population
        /// </summary>
        /// <param name="ind">the individual to be added</param>
        public void Add(Individual ind)
        {
            individuals.Add(ind);
        }

        /// <summary>
        /// Adds the individuals from the population p to this population (not cloning)
        /// </summary>
        /// <param name="p">population whose individuals are added</param>
        public void AddAll(Population p)
        {
            individuals.AddRange(p.individuals);
        }

        /// <summary>
        /// Creates random initial population
        /// </summary>
        public void CreateRandomInitialPopulation()
        {
            if (!Settings.paralell) Console.Write("Population Initialization ");
            individuals = new List<Individual>(size);

            for (int i = 0; i < size; i++)
            {
                Individual n = new Individual(sampleIndividual);

                if (Properties.Settings.Default.File_Initialization && i < size * Settings.fileUsage)
                    // if we are using file, use it
                    n.FileInitialization();
                else
                {
                    // else initialize randomly
                    n.RandomInitialization(Settings.maxColours);
                }
                n.changed = true;
                n.spanner = n.Is_3_Spanner(true);
                individuals.Add(n);
                if (i % (size / 5) == 0)
                    if (!Settings.paralell) Console.Write("|");
            }
            if (!Settings.paralell) Console.WriteLine(" Done");
        }

        /// <summary>
        /// Removes all the individuals from the population
        /// </summary>
        public void Clear()
        {
            individuals.Clear();
        }

        /// <summary>
        /// Method for comparing two individuals (based on their fitness)
        /// </summary>
        class FitnessFunctionComparator : IComparer<Individual>
        {

            public int Compare(Individual o1, Individual o2)
            {
                if (o1.GetFitnessValue() > o2.GetFitnessValue())
                    return -1;
                if (o1.GetFitnessValue() == o2.GetFitnessValue())
                    return 0;
                return 1;
            }

        }
    }

}