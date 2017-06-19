using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder.Replacements
{
    interface Replacement
    {
        Population Replace(Population parents, Population offspring);
    }
}
