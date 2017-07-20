using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder
{
    /// <summary>
    /// Technical methods class
    /// </summary>
    public class Tools
    {
        private static int[] powers;
        private static int dimension;
        private static bool[] ret;

        private static Tools theInstance = null;

        /// <summary>
        /// Constructor, creates tools for the right dimension
        /// </summary>
        /// <param name="dim">dimension of the problem</param>
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

        /// <summary>
        /// Gets the instance of the Tools class
        /// </summary>
        /// <param name="dim">dimension of the instance we want</param>
        /// <returns>the instance of Tools</returns>
        public static Tools GetInstance(int dim)
        {
            if (theInstance == null)
            {
                theInstance = new Tools(dim);
            }
            return theInstance;
        }

        /// <summary>
        /// Converts a decimal number to array of bits of its binary representaion
        /// </summary>
        /// <param name="num">number to be converted</param>
        /// <returns>array of bits representing a binary number</returns>
        public static byte[] ToBinary(int num)
        {
            byte[] ret = new byte[dimension];

            for (int i = dimension - 1; i >= 0; i--)
            {
                ret[i] = (byte)((num & (1 << i)) != 0 ? 1 : 0);
            }
            return ret;
        }

        /// <summary>
        /// Converts an array of bits that is a binary representaion of a number to decimal
        /// </summary>
        /// <param name="num">number (array of bits) to be converted</param>
        /// <returns>decimal number</returns>
        public static int FromBinary(byte[] num)
        {
            int dec = 0;
            for (int i = dimension - 1; i >= 0; i--)
            {
                dec += powers[i] * num[dimension-i-1];
            }
            return dec;
        }

        /// <summary>
        /// Computes the Hamming distance between x and y
        /// </summary>
        /// <param name="x">vertex x</param>
        /// <param name="y">vertex y</param>
        /// <returns>hamming distance between x and y</returns>
        public static int Distance(int x, int y)
        {
            // http://graphics.stanford.edu/~seander/bithacks.html#CountBitsSetParallel
            int v = x ^ y;
            v = v - ((v >> 1) & 0x55555555);                         // reuse input as temporary
            v = (v & 0x33333333) + ((v >> 2) & 0x33333333);         // temp
            return ((v + (v >> 4) & 0xF0F0F0F) * 0x1010101) >> 24; // count
        }

        /// <summary>
        /// Calculate at which index two vertices differ
        /// </summary>
        /// <param name="i1">vertex i1</param>
        /// <param name="i2">vertex i2</param>
        /// <returns>the index where i1 and i2 differ; or -1 if they dont</returns>
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

        /// <summary>
        /// Gets two to the power of num
        /// </summary>
        /// <param name="num">the base</param>
        /// <returns>2 to the power of num</returns>
        public static int GetPower(int num)
        {
            if (num < dimension) return powers[num];
            else return -1;
        }

        /// <summary>
        /// Checks if a number is a power of two
        /// </summary>
        /// <param name="n">number</param>
        /// <returns>true if n is power of 2</returns>
        public static bool IsPowerOf2(int n)
        {
            if (powers[dimension - 1] < n && n % 2 == 0) return IsPowerOf2(n / 2);
            else if (powers.Contains(n)) return true;
            else return false;
        }

        /// <summary>
        /// Checks if two numbers are neigbouring vertices in the graph
        /// </summary>
        /// <param name="v1">vertex v1</param>
        /// <param name="v2">vertex v2</param>
        /// <returns>true if v1 is a neighbour of v2</returns>
        public static bool Neighbours(int v1, int v2)
        {
            return IsPowerOf2(Math.Abs(v1 - v2));
        }

        /// <summary>
        /// Writes the individual to harddrive
        /// </summary>
        /// <param name="ind">the individual to be written</param>
        public static void WriteIndividual(Individual ind)
        {
            // Write the string to a file.
            System.IO.StreamWriter file =
                new System.IO.StreamWriter(Settings.outputDir
                    + dimension + "_" + ind.GetObjectiveValue() + "__"
                    + DateTime.Now.ToString("yyyy-M-dd_HH-mm-ss") + "__"
                    + ind.GetHashCode() + ".txt");

            file.WriteLine(ind.ToString());

            file.Close();
        }

        /// <summary>
        /// Tests whether e is within the subcube set by fix and vals
        /// </summary>
        /// <param name="fix">indices that are part of the subcube</param>
        /// <param name="vals">values for those indices</param>
        /// <param name="e">edge</param>
        /// <returns>-1 both vertices out, 0 one in and one out, 1 both vertices in</returns>
        // -1 both out, 0 one and one, 1 both in
        public static int TestSubcube(bool[] fix, byte[] vals, Edge e)
        {
            byte[] v1 = Tools.ToBinary(e.Vertex1);
            byte[] v2 = Tools.ToBinary(e.Vertex2);
            return TestSubcube(fix, vals, v1, v2);
        }

        /// <summary>
        /// Tests whether xy is within the subcube set by fix and vals
        /// </summary>
        /// <param name="fix">indices that are part of the subcube</param>
        /// <param name="vals">values for those indices</param>
        /// <param name="x">vertex x</param>
        /// <param name="y">vertex y</param>
        /// <returns>-1 both vertices out, 0 one in and one out, 1 both vertices in</returns>
        public static int TestSubcube(bool[] fix, byte[] vals, int x, int y)
        {
            byte[] v1 = Tools.ToBinary(x);
            byte[] v2 = Tools.ToBinary(y);
            return TestSubcube(fix, vals, v1, v2);
        }

        /// <summary>
        /// Tests whether v1v2 is within the subcube set by fix and vals
        /// </summary>
        /// <param name="fix">indices that are part of the subcube</param>
        /// <param name="vals">values for those indices</param>
        /// <param name="v1">vertex v1</param>
        /// <param name="v2">vertex v2</param>
        /// <returns>-1 both vertices out, 0 one in and one out, 1 both vertices in</returns>
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

    /// <summary>
    /// class for Extension
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Activity of an edge
        /// </summary>
        /// <param name="val">the value of the activity</param>
        /// <returns>true if not zero</returns>
        public static bool IsTrue(this byte val) => val != 0;

#region Parsing
        /// <summary>
        /// Gets an item from the list at index or returns defaultValue
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="list">the list</param>
        /// <param name="index">index</param>
        /// <param name="defaultValue">default value</param>
        /// <returns>returns TItem at index in list or defaultValue</returns>
        public static TItem GetOrElse<TItem>(this IList<TItem> list, int index, TItem defaultValue)
        {
            // other checks omitted
            if (index < 0 || index >= list.Count)
            {
                return defaultValue;
            }
            else return list[index];
        }

        /// <summary>
        /// Parses integer from a list of strings at index or returns defaultValue
        /// </summary>
        /// <param name="list">list</param>
        /// <param name="index">index</param>
        /// <param name="defaultValue">default value</param>
        /// <returns>returns int parsed from list at index or defaultValue</returns>
        public static int ParseIntOrElse(this IList<string> list, int index, int defaultValue)
        {
            if (index < 0 || index >= list.Count || !Int32.TryParse((string)list[index], out int val))
            {
                return defaultValue;
            }
            else return val;
        }

        /// <summary>
        /// Parses double from a list of strings at index or returns defaultValue
        /// </summary>
        /// <param name="list">list</param>
        /// <param name="index">index</param>
        /// <param name="defaultValue">default value</param>
        /// <returns>returns double parsed from list at index or defaultValue</returns>
        public static double ParseDoubleOrElse(this IList<string> list, int index, double defaultValue)
        {
            if (index < 0 || index >= list.Count || 
                !Double.TryParse((string)list[index], NumberStyles.Number, CultureInfo.InvariantCulture, out double val))
            {
                return defaultValue;
            }
            else return val;
        }
#endregion Parsing

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
