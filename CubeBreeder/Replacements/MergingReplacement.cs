using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder.Replacements
{
    /// <summary>
    /// Replacement that combines the parents and the offspring into one population
    /// </summary>
    class MergingReplacement
    {
        /// <summary>
        /// Replace method
        /// </summary>
        /// <param name="parents">parents</param>
        /// <param name="offspring">offspring</param>
        /// <returns>new generation</returns>
        public Population Replace(Population parents, Population offspring)
        {
            Population merged = new Population();
            merged.AddAll((Population)parents.Clone());
            merged.AddAll((Population)offspring.Clone());

            return merged;
        }
    }
}
