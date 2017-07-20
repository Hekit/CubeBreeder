using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeBreeder
{
    /// <summary>
    /// Singleton random number generator
    /// </summary>
    public class RandomNumberGenerator
    {
        private static Random rnd;
        private static Random[] randoms;
        private static RandomNumberGenerator theInstance = null;

        /// <summary>
        /// Constructor
        /// </summary>
        private RandomNumberGenerator()
        {
            rnd = new Random();
            int processorCount = Environment.ProcessorCount;
            randoms = new Random[processorCount];
            for (int i = 0; i < processorCount; i++)
            {
                randoms[i] = new Random();
            }
        }

        /// <summary>
        /// Get the only instance of RNG
        /// </summary>
        /// <returns>returns the instance of RNG</returns>
        public static RandomNumberGenerator GetInstance()
        {
            if (theInstance == null)
            {
                theInstance = new RandomNumberGenerator();
            }
            return theInstance;
        }

        /// <summary>
        /// Get a random integer between 0 and n (exclusive)
        /// </summary>
        /// <param name="n">Exclusive upper bound</param>
        /// <returns>A random integer from [0,n) drawn from uniform distribution</returns>
        public int NextInt(int n)
        {
            return rnd.Next(n);
        }

        /// <summary>
        /// Get a random integer between start (inclusive) and end (exclusive)
        /// </summary>
        /// <param name="start">Inclusive lower bound</param>
        /// <param name="end">Exclusive upper bound</param>
        /// <returns>A random integer from [start,end) drawn from uniform distribution</returns>
        public int NextInt(int start, int end)
        {
            return rnd.Next(start, end);
        }

        /// <summary>
        /// Get a random double between 0 and 1 (exclusive)
        /// </summary>
        /// <returns>A random double from [0,1) drawn from uniform distribution</returns>
        public double NextDouble()
        {
            return rnd.NextDouble();
        }

        /// <summary>
        /// Get a random power of 2
        /// </summary>
        /// <param name="limit">the upper bound on the exponent</param>
        /// <returns>A power of 2</returns>
        public int NextPower2(int limit)
        {
            return (int)Math.Pow(rnd.Next(limit), 2);
        }

        /// <summary>
        /// Sets the new random seed.
        /// </summary>
        /// <param name="seed">random seed</param>
        public void ReSeed(int seed)
        {
            rnd = new Random(seed);
        }

        /// <summary>
        /// Get a random byte between 0 and p (exclusive)
        /// </summary>
        /// <param name="p">Exclusive upper bound</param>
        /// <returns>A random byte from [0,p) drawn from uniform distribution</returns>
        internal byte NextByte(int p)
        {
            return (byte)rnd.Next(0, p);
        }
    }
}
