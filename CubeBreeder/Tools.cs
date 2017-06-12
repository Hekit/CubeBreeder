using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder
{
    public class Tools
    {
        private static int[] powers;
        private static int dimension;
        private static bool[] ret;

        private static Tools theInstance = null;

        public Tools(int dim)
        {
            dimension = dim;
            powers = new int[dimension];
            ret = new bool[dimension];
            for (int i = dimension - 1; i >= 0; i--)
            {
                powers[i] = (int)Math.Pow(2, i);
            }
        }

        public static Tools GetInstance(int dim)
        {
            if (theInstance == null)
            {
                theInstance = new Tools(dim);
            }
            return theInstance;
        }

        public static byte[] ToBinary(int num)
        {
            byte[] ret = new byte[dimension];

            for (int i = dimension - 1; i >= 0; i--)
            {
                //ret[i] = (num & (1 << i)) != 0;
                //ret[i] = (byte)(num & (1 << i));
                ret[i] = (byte)((num & (1 << i)) != 0 ? 1 : 0);
            }
            return ret;
        }

        public static bool[] ToBinaryNew(int num)
        {
            //ret = new bool[dimension];
            for (int i = dimension - 1; i >= 0; i--)
            {
                ret[i] = (num & (1 << i)) != 0;
            }
            return ret;
        }

        public static int Distance(int x, int y)
        {
            // http://graphics.stanford.edu/~seander/bithacks.html#CountBitsSetParallel
            int v = x ^ y;
            v = v - ((v >> 1) & 0x55555555);                         // reuse input as temporary
            v = (v & 0x33333333) + ((v >> 2) & 0x33333333);         // temp
            return ((v + (v >> 4) & 0xF0F0F0F) * 0x1010101) >> 24; // count
        }

        public static int DifferenceIndex(int i1, int i2)
        {
            byte[] b1 = ToBinary(i1);
            byte[] b2 = ToBinary(i2);

            for (int i = 0; i < b1.Length; i++)
            {
                if (b1[i] != b2[i]) return i;
            }
            return -1;
        }

        public static int GetPower(int num)
        {
            if (num < dimension) return powers[num];
            else return -1;
        }

        public static bool IsPowerOf2(int n)
        {
            if (powers[dimension - 1] < n && n % 2 == 0) return IsPowerOf2(n / 2);
            else if (powers.Contains(n)) return true;
            else return false;
        }

        public static bool Neighbours(int v1, int v2)
        {
            return IsPowerOf2(Math.Abs(v1 - v2));
        }

        public static void WriteIndividual(Individual ind)
        {
            // Write the string to a file.
            System.IO.StreamWriter file =
                new System.IO.StreamWriter("D:\\Development\\hypercubes\\"
                    + dimension + "_" + ind.GetObjectiveValue() + "__"
                    + DateTime.Now.ToString("yyyy-M-dd_HH-mm-ss") + "__"
                    + ind.GetHashCode() + ".txt");

            file.WriteLine(ind.ToString());

            file.Close();
        }
    }
}
