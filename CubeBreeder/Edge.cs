using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder
{
    [Serializable]
    public class Edge
    {
        public int Vertex1 { get; set; }
        public int Vertex2 { get; set; }

        // List of Triples of EdgeIDs that detour this Edge
        List<Tuple<int, int, int>> detourMe;

        public int ID { get; }

        public Edge(int id)
        {
            ID = id;
            detourMe = new List<Tuple<int, int, int>>();
        }

        public void AddDetourMe(int i1, int i2, int i3)
        {
            detourMe.Add(new Tuple<int, int, int>(i1, i2, i3));
        }

        public List<Tuple<int, int, int>> GetDetours()
        {
            return detourMe;
        }
    }
}
