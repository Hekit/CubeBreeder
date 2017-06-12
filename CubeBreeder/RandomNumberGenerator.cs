using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder
{
    /**
     * Singleton random number generator.
     */
    public class RandomNumberGenerator
    {
        private static Random rnd;
        private static RandomNumberGenerator theInstance = null;

        private RandomNumberGenerator()
        {
            rnd = new Random();
        }

        /**
         * Returns the only instance of random number generator.
         *
         * @return The random number generator.
         */
        public static RandomNumberGenerator GetInstance()
        {

            if (theInstance == null)
            {
                theInstance = new RandomNumberGenerator();
            }
            return theInstance;

        }

        /**
         * Returns a random integer between 0 and n - 1 (inclusive).
         *
         * @param n The upper limit
         * @return Random integer between 0 and n - 1 drawn from a uniform distribution.
         */
        public int NextInt(int n)
        {
            return rnd.Next(n);
        }

        /**
         * Returns a random integer between start (inclusive) and end (exclusive).
         *
         * @param start The lower limit (inclusive)
         * @param end The upper limit (exclusive)
         * @return Random integer between start and end - 1 drawn from a uniform distribution.
         */
        public int NextInt(int start, int end)
        {
            return rnd.Next(start, end);
        }

        /**
         * @return A random double from the interval [0, 1) drawn from a uniform distribution.
         */
        public double NextDouble()
        {
            return rnd.NextDouble();
        }

        public int NextPower2(int limit)
        {
            return (int)Math.Pow(rnd.Next(limit), 2);
        }

        /**
         * Sets a new seed for the random number generator.
         *
         * @param seed The seed which shall be set.
         */
        public void ReSeed(int seed)
        {
            rnd = new Random(seed);
        }

        public Random getRandom()
        {
            return rnd;
        }

        internal byte NextByte(int p)
        {
            return (byte)rnd.Next(0, Byte.MaxValue + 1);
        }
    }
}
