﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder.Fitness
{
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
            int[] fitness = new int[Settings.maxColours];
            // the present edges in that colour iff it is a spanning tree
            int[] objective = new int[Settings.maxColours];
            // it is a spannning tree (1 or 0)
            double[] spanner = new double[Settings.maxColours];

            for (int i = 0; i < fitness.Length; i++)
            {
                fitness[i] = Math.Abs(vertexCount - ind.GetActiveEdgeCount((byte)(i + 1)));
                spanner[i] = ind.Is_Good_Enough((byte)(i+1));
                //this sets the objective value, can be different from the fitness function
                /*if (spanner[i] == 1.0) objective[i] = edgeCount - fitness[i];
                else objective[i] = 0;*/
            }

            int obj = 0;
            for (int i = 0; i < spanner.Length; i++)
            {
                if (spanner[i] == 1.0) obj += 1;
            }
            ind.SetObjectiveValue(obj);

            double fit = 0;
            int colours = 0;
            for (int i = 0; i < fitness.Length; i++)
            {
                //if (fitness[i] < edgeCount) // if there is any edge of that colour
                if (fitness[i] != vertexCount)
                {
                    if (fitness[i] > 0) fit += (1.0 / fitness[i]) * spanner[i];
                    else fit += spanner[i];
                    colours++;
                }
            }
            ind.SetColourCount(colours);
            // proved magickou operaci
            // a ne, ta operace neni prumer
            return (fit ) * (obj + 1);
            //return (fit / colours) * (obj + 1);
        }
    }
}
