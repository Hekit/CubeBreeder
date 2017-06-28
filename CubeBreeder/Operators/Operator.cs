using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder.Operators
{
    interface Operator
    {
        void Operate(Population parents, Population offspring);

        void Update();
    }
}
