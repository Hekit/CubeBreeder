using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder.Replacements
{
    /// <summary>
    /// Replacement interface
    /// </summary>
    interface Replacement
    {
        /// <summary>
        /// Replace method
        /// </summary>
        /// <param name="parents">parents</param>
        /// <param name="offspring">offspring</param>
        Population Replace(Population parents, Population offspring);
    }
}
