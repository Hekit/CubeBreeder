
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder.Fitness
{
    /// <summary>
    /// The fitness for edge-disjoint spanning trees
    /// </summary>
    class EdgeDisjointSpanner : FitnessFunction
    {
        int edgeCount = Int32.MaxValue;
        int vertexCount = Int32.MaxValue;

        public EdgeDisjointSpanner(int dim)
        {
            this.edgeCount = dim * Tools.GetPower(dim - 1);
            this.vertexCount = Tools.GetPower(dim);
        }

        public double Evaluate(Individual ind, bool count)
        {
            // "how many edges are missing in that colour
            //int[] fitness = new int[Settings.maxColours];
            // the present edges in that colour iff it is a spanning tree
            //int[] objective = new int[Settings.maxColours];
            // it is a spannning tree (1 or 0)
            double[] spanner = new double[Settings.maxColours];

            for (int i = 0; i < spanner.Length; i++)
            {
                //fitness[i] = Math.Abs(vertexCount - ind.GetActiveEdgeCount((byte)(i + 1)));
                spanner[i] = ind.Is_Good_Enough((byte)(i+1));
            }

            int obj = 0;
            for (int i = 0; i < spanner.Length; i++)
            {
                if (spanner[i] == 1.0) obj += 1;
            }
            ind.SetObjectiveValue(obj);

            double fit = 0;
            int colours = 0;
            for (int i = 0; i < spanner.Length; i++)
            {
                //if (fitness[i] < edgeCount) // if there is any edge of that colour
                /*if (fitness[i] != vertexCount)
                {
                    if (fitness[i] > 0) fit += (1.0 / fitness[i]) * spanner[i];
                    else fit += spanner[i];
                    colours++;
                }*/
                fit += spanner[i];// * spanner[i];
            }
            ind.SetColourCount(colours);
            // perform some magic
            //return (fit ) * (obj + 1);
            //return (fit / colours) * (obj + 1);
            return ( fit ) / Settings.maxColours;
        }
    }
}
