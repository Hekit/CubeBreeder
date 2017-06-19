using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder.Replacements
{
    class MergingReplacement
    {
        public Population Replace(Population parents, Population offspring)
        {
            Population merged = new Population();
            merged.AddAll((Population)parents.Clone());
            merged.AddAll((Population)offspring.Clone());

            return merged;
        }
    }
}
