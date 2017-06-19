using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder.Replacements
{
    class ChildrenOnlyReplacement : Replacement
    {
        public Population Replace(Population parents, Population offspring)
        {
            return (Population)offspring.Clone();
        }
    }
}
