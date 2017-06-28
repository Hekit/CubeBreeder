using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder
{
    class Population
    {
        int size = 0;
        GraphInfo sampleIndividual;
        List<Individual> individuals;
        
        Random rnd = new Random();

        /**
         * Creates new empty population.
         */
        public Population()
        {
            individuals = new List<Individual>();
        }

        /**
        * Makes a deep copy of the population.
        *
        * @return A deep copy of the population. All individuals are cloned using
        * their clone methods.
        */
        public Object Clone()
        {
            return this.MemberwiseClone();
        }

        /**
         * Shuffles the order of the individuals in the population randomly.
         */
        public void Shuffle()
        {
            individuals = (List<Individual>)individuals.OrderBy(item => rnd.Next());
        }

        /**
         * Sets the individual which is cloned to create the random initial population.
         *
         * @param sample The sample individual.
         */
        public void SetSampleIndividual(GraphInfo example)
        {
            this.sampleIndividual = example;
        }

        /**
         * Sets the population size, used only during the random initialization.
         *
         * @param size The population size.
         */
        public void SetPopulationSize(int size)
        {
            this.size = size;
        }

        /**
         * Returns the actual size of the population.
         *
         * @return The number of individuals in the population.
         */
        public int GetPopulationSize()
        {
            return individuals.Count();
        }

        public List<Individual> GetIndividuals()
        {
            return individuals;
        }

        /**
         * Gets the ith individual in the population.
         *
         * @param i The index of the individual which shall be returned.
         * @return The indivudal at index i.
         */
        public Individual Get(int i)
        {
            return individuals[i];
        }

        /**
         * Adds an individual to the population.
         *
         * @param ind The individual which shall be addded.
         */
        public void Add(Individual ind)
        {
            individuals.Add(ind);
        }

        /**
         * Adds all the individuals fromt he population p to the population. Does not
         * clone them.
         *
         * @param p The population from which the individuals shall be added.
         */
        public void AddAll(Population p)
        {
            individuals.AddRange(p.individuals);
        }

        /**
         * Creates random initial population of the specified size. Calls the clone
         * method on the sample individual and the randomInitialization method
         * on the clones created from it.
         */
        public void CreateRandomInitialPopulation()
        {

            individuals = new List<Individual>(size);

            for (int i = 0; i < size; i++)
            {
                //Parallel.For(0, size, i =>
                //{
                Individual n = new Individual(sampleIndividual);

                if (Properties.Settings.Default.File_Initialization && i < size * Properties.Settings.Default.FileInitRatio)
                    n.FileInitialization();
                else
                {
                    n.RandomInitialization(Program.maxColours);
                }
                n.changed = true;
                n.spanner = n.Is_3_Spanner(true);
                individuals.Add(n);
                if (i % (size / 5) == 0) Console.WriteLine((i * 20 / (size / 5)) + " %");
                //});
            }
            Console.WriteLine("100 %");
        }

        public void CreateRandomInitialPopulation2()
        {
            individuals = new List<Individual>(size);

            Parallel.For(0, size, i =>
            {
                Individual n = new Individual(sampleIndividual);
                n.RandomInitialization(Program.maxColours);
                individuals.Add(n);
            });

        }

        /**
         * Removes all the individuals from the population.
         */
        public void Clear()
        {
            individuals.Clear();
        }

        /**
         * Returns all the individuals in the population as a list sorted in descending
         * order og their fitness.
         *
         * @return The sorted list of individuals.
         */
        public List<Individual> GetSortedIndividuals()
        {

            List<Individual> sorted = new List<Individual>();
            sorted.AddRange(individuals);

            sorted.Sort(new FitnessFunctionComparator());

            return sorted;

        }

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