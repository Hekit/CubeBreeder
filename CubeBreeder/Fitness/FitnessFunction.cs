using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder.Fitness
{
    interface FitnessFunction
    {
        double Evaluate(Individual ind, bool count);
    }
}
