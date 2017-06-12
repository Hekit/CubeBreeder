using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder
{
    class GraphInfo
    {
        private List<int>[] graph;

        public GraphInfo(int dim)
        {

        }

        public List<int> EdgesFrom(int i)
        {
            return graph[i];
        }
            

    }
}
