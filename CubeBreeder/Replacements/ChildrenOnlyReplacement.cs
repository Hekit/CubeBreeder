using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder.Replacements
{
    /// <summary>
    /// Replacement that keeps only children
    /// </summary>
    class ChildrenOnlyReplacement : Replacement
    {
        /// <summary>
        /// Replace method
        /// </summary>
        /// <param name="parents">parents</param>
        /// <param name="offspring">offspring</param>
        /// <returns>new generation</returns>
        public Population Replace(Population parents, Population offspring)
        {
            return (Population)offspring.Clone();
        }
    }
}
