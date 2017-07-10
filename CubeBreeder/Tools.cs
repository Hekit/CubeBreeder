using System;
using System.Collections.Generic;
using System.Globalization;
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

        public static int FromBinary(byte[] num)
        {
            int dec = 0;
            for (int i = dimension - 1; i >= 0; i--)
            {
                dec += powers[i] * num[dimension-i-1];
            }
            return dec;
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

        // -1 both out, 0 one and one, 1 both in
        public static int TestSubcube(bool[] fix, byte[] vals, Edge e)
        {
            byte[] v1 = Tools.ToBinary(e.Vertex1);
            byte[] v2 = Tools.ToBinary(e.Vertex2);
            return TestSubcube(fix, vals, v1, v2);
        }

        public static int TestSubcube(bool[] fix, byte[] vals, int x, int y)
        {
            byte[] v1 = Tools.ToBinary(x);
            byte[] v2 = Tools.ToBinary(y);
            return TestSubcube(fix, vals, v1, v2);
        }

        public static int TestSubcube(bool[] fix, byte[] vals, byte[] v1, byte[] v2)
        {
            string status = "bothIn";

            for (int i = 0; i < fix.Length; i++)
            {
                if (fix[i])
                {
                    if (v1[i] != vals[i])
                        if (status == "bothIn") status = "v1out";
                        else if (status == "v2out") return -1;
                    if (v2[i] != vals[i])
                        if (status == "bothIn") status = "v2out";
                        else if (status == "v1out") return -1;
                }
            }
            if (status == "bothIn") return 1;
            else return 0;
        }
    }

    public static class Extensions
    {
        public static bool IsTrue(this byte val) => val != 0;

        public static TItem GetOrElse<TItem>(this IList<TItem> list, int index, TItem defaultValue)
        {
            // other checks omitted
            if (index < 0 || index >= list.Count)
            {
                return defaultValue;
            }
            else return list[index];
        }

        public static int ParseIntOrElse(this IList<string> list, int index, int defaultValue)
        {
            int val;
            if (index < 0 || index >= list.Count || !Int32.TryParse((string)list[index], out val))
            {
                return defaultValue;
            }
            else return val;
        }

        public static double ParseDoubleOrElse(this IList<string> list, int index, double defaultValue)
        {
            double val;
            if (index < 0 || index >= list.Count || 
                !Double.TryParse((string)list[index], NumberStyles.Number, CultureInfo.InvariantCulture, out val))
            {
                return defaultValue;
            }
            else return val;
        }

        /*
        public static int NextInt(this Random rnd, int max)
        {
            return rnd.Next(max);
        }

        public static int NextInt(this Random rnd, int start, int end)
        {
            return rnd.Next(start, end);
        }

        public static double NextDouble(this Random rnd)
        {
            return rnd.NextDouble();
        }

        public static byte NextByte(this Random rnd, int p)
        {
            return (byte)rnd.Next(0, Byte.MaxValue + 1);
        }*/
    }
}
