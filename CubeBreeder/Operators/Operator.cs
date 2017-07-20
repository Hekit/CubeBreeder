using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder.Operators
{
    /// <summary>
    /// Operator interface
    /// </summary>
    interface Operator
    {
        /// <summary>
        /// Operator operate method
        /// </summary>
        /// <param name="parents">parents</param>
        /// <param name="offspring">offspring</param>
        void Operate(Population parents, Population offspring);

        /// <summary>
        ///  Each generation update method
        /// </summary>
        void Update();
    }
}
