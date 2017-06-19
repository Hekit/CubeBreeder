using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder.Replacements
{
    class PercentageReplacement
    {
        int percentage;
        public PercentageReplacement(int percentage)
        {
            this.percentage = percentage;
        }

        public Population Replace(Population parents, Population offspring)
        {
            int size = offspring.GetPopulationSize();
            List<Individual> kids = ((Population)offspring.Clone()).GetSortedIndividuals();
            List<Individual> folks = ((Population)parents.Clone()).GetSortedIndividuals();
            Population p = new Population();
            p.SetPopulationSize(offspring.GetPopulationSize());
            int idx = 0;
            for (int i = 0; i < size * percentage; i++)
            {
                p.Add(kids[i]);
            }
            for (int i = size * percentage; i < size; i++)
            {
                p.Add(folks[idx]);
                idx++;
            }
            return p;
        }
    }
}
