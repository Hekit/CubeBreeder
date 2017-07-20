using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder
{
    /// <summary>
    /// Class representing an edge.
    /// </summary>
    [Serializable]
    public class Edge
    {
        public int Vertex1 { get; set; }
        public int Vertex2 { get; set; }

        /// <summary>
        /// List of Triples of EdgeIDs that detour this Edge
        /// </summary>
        List<Tuple<int, int, int>> detourMe;

        /// <summary>
        /// Unique ID (and index) of the Edge
        /// </summary>
        public int ID { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">the future id of the edge</param>
        public Edge(int id)
        {
            ID = id;
            detourMe = new List<Tuple<int, int, int>>();
        }

        /// <summary>
        /// Adds a triple of EdgeIDs that detour this edge.
        /// </summary>
        /// <param name="i1">first edge of the detour</param>
        /// <param name="i2">second edge of the detour</param>
        /// <param name="i3">third edge of the detour</param>
        public void AddDetourMe(int i1, int i2, int i3)
        {
            detourMe.Add(new Tuple<int, int, int>(i1, i2, i3));
        }

        /// <summary>
        /// Get all the detours of this edge
        /// </summary>
        /// <returns>Returns a list of Triples of EdgeIDs of Edges that detour this edge</returns>
        public List<Tuple<int, int, int>> GetDetours()
        {
            return detourMe;
        }
    }
}
