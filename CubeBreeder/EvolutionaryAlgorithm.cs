using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CubeBreeder.Selectors;
using CubeBreeder.Fitness;
using CubeBreeder.Operators;
using CubeBreeder.Replacements;

namespace CubeBreeder
{
    /// <summary>
    /// Representation of the evolutionary algorithm
    /// </summary>
    class EvolutionaryAlgorithm
    {
        List<Operator> operators;
        List<Selector> matingSelectors;
        List<Selector> environmentalSelectors;
        double eliteSize = 0.0;
        FitnessEvaluator fitness;
        Replacement replacement;
        int generationNo = 0;

        /// <summary>
        /// Initializes an empty evolutionary algorithm.
        /// </summary>
        public EvolutionaryAlgorithm()
        {
            operators = new List<Operator>();
            matingSelectors = new List<Selector>();
            environmentalSelectors = new List<Selector>();
            replacement = new ChildrenOnlyReplacement();
        }

        public List<Operator> GetOperators() { return operators; }
        public List<Selector> GetMating() { return matingSelectors; }
        public List<Selector> GetEnvironmental() { return environmentalSelectors; }

        /// <summary>
        /// Setter for replacement
        /// </summary>
        /// <param name="replacement">replacement to be used</param>
        public void SetReplacement(Replacement replacement)
        {
            this.replacement = replacement;
        }

        /// <summary>
        /// Setter for elite size
        /// </summary>
        /// <param name="elitePercentage">Elite size - a number between 0 and 1</param>
        public void SetElite(double elitePercentage)
        {
            eliteSize = elitePercentage;
        }

        /// <summary>
        /// Add an operator
        /// </summary>
        /// <param name="o">operator to be added</param>
        public void AddOperator(Operator o)
        {
            operators.Add(o);
        }

        /// <summary>
        /// Add mating selector
        /// </summary>
        /// <param name="s">selector to be added</param>
        public void AddMatingSelector(Selector s)
        {
            matingSelectors.Add(s);
        }

        /// <summary>
        /// Add environmental selector
        /// </summary>
        /// <param name="s">selector to be added</param>
        public void AddEnvironmentalSelector(Selector s)
        {
            environmentalSelectors.Add(s);
        }

        /// <summary>
        /// Set the fitness function
        /// </summary>
        /// <param name="f">fitness function to be used</param>
        public void SetFitnessFunction(FitnessFunction f)
        {
            fitness = new BasicEvaluator(f);
        }

        /// <summary>
        /// Sets the fitness evaluator
        /// </summary>
        /// <param name="fe">fitness evaluator to be used</param>
        public void SetFitnessEvaluator(FitnessEvaluator fe)
        {
            fitness = fe;
        }

        /// <summary>
        /// Executes one iteration of EA
        /// </summary>
        /// <param name="pop">population for this iteration</param>
        public void Evolve(Population pop)
        {
            if (fitness == null)
                throw new Exception("No fitness function defined");

            generationNo++;
            
            // informative output
            if ((!Settings.parallel) && Settings.showGap >= 5 && (generationNo + 1) % (Settings.showGap / 5) == 0)
            {
                Console.Write("|");
            }
            
            fitness.Evaluate(pop, false);
            
            Population parents = pop;

            Population matingPool = new Population();
            
            // mating pool creation - if n selectors, each will select 1 / n of the pool
            if (matingSelectors.Count() > 0)
            {
                int mateSel = matingSelectors.Count;
                int toSelect = parents.GetPopulationSize() / mateSel;
                for (int i = 0; i < matingSelectors.Count; i++)
                {
                    Population sel = new Population();
                    matingSelectors[i].Select(toSelect, parents, sel);
                    matingPool.AddAll((Population)sel.Clone());
                }

                int missing = parents.GetPopulationSize() - matingPool.GetPopulationSize();
                if (missing > 0)
                {
                    Population sel = new Population();
                    matingSelectors[matingSelectors.Count - 1].Select(toSelect, parents, sel);
                    matingPool.AddAll((Population)sel.Clone());
                }
            }
            else
            {
                matingPool = (Population)parents.Clone();
                matingPool.Shuffle();
            }
            
            // operators are executed in the order of specification
            Population offspring = null;
            foreach (Operator o in operators)
            {
                offspring = new Population();
                o.Operate(matingPool, offspring);
                matingPool = offspring;
            }

            // update of operator properties (if desired)
            foreach (Operator o in operators) o.Update();

            fitness.Evaluate(offspring, false);

            // selection process
            Population selected = new Population();

            Population combined = replacement.Replace(parents, offspring);
            
            if (environmentalSelectors.Count < 1)
            {
                selected = (Population)combined.Clone();
                fitness.Evaluate(combined, false);
            }
            else
            {
                List<Individual> sortedOld = parents.GetSortedIndividuals();
                for (int i = 0; i < eliteSize * parents.GetPopulationSize(); i++)
                {
                    selected.Add(sortedOld[i]);
                }

                fitness.Evaluate(combined, false);

                int envSel = environmentalSelectors.Count;
                int toSelect = (parents.GetPopulationSize() - selected.GetPopulationSize()) / envSel;
                for (int i = 0; i < environmentalSelectors.Count; i++)
                {
                    Population sel = new Population();
                    environmentalSelectors[i].Select(toSelect, combined, sel);
                    selected.AddAll((Population)sel.Clone());
                }

                int missing = parents.GetPopulationSize() - selected.GetPopulationSize();
                if (missing > 0)
                {
                    Population sel = new Population();
                    environmentalSelectors[environmentalSelectors.Count - 1].Select(toSelect, combined, sel);
                    selected.AddAll((Population)sel.Clone());
                }
            }
            pop.Clear();
            pop.AddAll(selected);

            fitness.Evaluate(pop, true);
        }
    }
}