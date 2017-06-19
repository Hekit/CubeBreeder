using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CubeBreeder;

namespace CubeBreederTests
{
    [TestClass]
    public class IndividualTest
    {
        static int dimension = 3;
        Tools tools = Tools.GetInstance(dimension);
        GraphInfo gi = new GraphInfo(dimension);

        [TestMethod]
        public void TestMethod1()
        {
        }

        [TestMethod]
        public void TestMethod2()
        {
            Individual ind = new Individual(gi);
        }
    }
}
