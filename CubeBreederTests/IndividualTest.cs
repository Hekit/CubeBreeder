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
            ind.RandomInitialization(1);
            Individual ind1 = new Individual(ind);
            Individual ind2 = (Individual)ind.Clone();
            for (int i = 0; i < ind.Length(); i++)
            {
                Assert.AreEqual(ind.IsActiveOnEdge(i), ind2.IsActiveOnEdge(i), i.ToString());
            }
            ind.SetActivityBetweenVertices(0, 1, 1);
            ind2.SetActivityBetweenVertices(0, 1, 0);
            Assert.AreNotEqual(ind.IsActiveBetweenVertices(0,1), ind2.IsActiveBetweenVertices(0,1), "Two individuals but same object!");
        }
    }
}
