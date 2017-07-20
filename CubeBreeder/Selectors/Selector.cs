using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder.Selectors
{
    /// <summary>
    /// Selector interface
    /// </summary>
    interface Selector
    {
        /// <summary>
        /// Selecting howMany individuals from from to to
        /// </summary>
        /// <param name="howMany">number of individuals to select</param>
        /// <param name="from">old population</param>
        /// <param name="to">new population</param>
        void Select(int howMany, Population from, Population to);
    }
}
