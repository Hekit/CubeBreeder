using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder.Fitness
{
    interface FitnessEvaluator
    {
        void Evaluate(Population pop, bool count);
    }
}
