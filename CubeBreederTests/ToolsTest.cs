using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CubeBreeder;

namespace CubeBreederTests
{
    /// <summary>
    /// Souhrnný popis pro ToolsTest
    /// </summary>
    [TestClass]
    public class ToolsTest
    {
        Tools tools = Tools.GetInstance(3);

        [TestMethod]
        public void Distance()
        {
            Tools tools = Tools.GetInstance(3);

            for (int i = 0; i < 500000; i++)
            {
                Assert.AreEqual(1, Tools.Distance(0, 1), "0, 1");
                Assert.AreEqual(1, Tools.Distance(0, 2), "0, 2");
                Assert.AreEqual(2, Tools.Distance(1, 2), "1, 2");
                Assert.AreEqual(3, Tools.Distance(0, 7), "0, 7");
                Assert.AreEqual(3, Tools.Distance(3, 4), "3, 4");
            }
        }

        /*
        [TestMethod]
        public void DistanceNew()
        {
            Tools tools = Tools.GetInstance(3);

            for (int i = 0; i < 500000; i++)
            {
                Assert.AreEqual(1, Tools.DistanceNew(0, 1), "0, 1");
                Assert.AreEqual(1, Tools.DistanceNew(0, 2), "0, 2");
                Assert.AreEqual(2, Tools.DistanceNew(1, 2), "1, 2");
                Assert.AreEqual(3, Tools.DistanceNew(0, 7), "0, 7");
                Assert.AreEqual(3, Tools.DistanceNew(3, 4), "3, 4");
            }
        }*/

        [TestMethod]
        public void ToBinary()
        {
            Tools tools = Tools.GetInstance(3);

            for (int k = 0; k < 50000; k++)
            {
                byte[] e4 = { 0, 0, 1 };
                byte[] a4 = Tools.ToBinary(4);

                for (int i = 0; i < 3; i++)
                {
                    Assert.AreEqual(e4[i], a4[i], "Index " + i);
                }

                byte[] e6 = { 0, 1, 1 };
                byte[] a6 = Tools.ToBinary(6);

                for (int i = 0; i < 3; i++)
                {
                    Assert.AreEqual(e6[i], a6[i], "Index " + i);
                }
            }
        }

        [TestMethod]
        public void ToBinaryNew()
        {
            Tools tools = Tools.GetInstance(3);

            for (int k = 0; k < 50000; k++)
            {
                bool[] e4 = { false, false, true };
                bool[] a4 = Tools.ToBinaryNew(4);

                for (int i = 0; i < 3; i++)
                {
                    Assert.AreEqual(e4[i], a4[i], "Index " + i);
                }

                bool[] e6 = { false, true, true };
                bool[] a6 = Tools.ToBinaryNew(6);

                for (int i = 0; i < 3; i++)
                {
                    Assert.AreEqual(e6[i], a6[i], "Index " + i);
                }
            }
        }

        [TestMethod]
        public void FromBinary()
        {
            Tools tools = Tools.GetInstance(3);

            byte[] b1 = { 0, 0, 1 };
            byte[] b2 = { 1, 0, 1 };
            byte[] b3 = { 0, 1, 1 };

            Assert.AreEqual(1, Tools.FromBinary(b1), "1");
            Assert.AreEqual(5, Tools.FromBinary(b2), "5");
            Assert.AreEqual(3, Tools.FromBinary(b3), "3");
        }

        [TestMethod]
        public void DifferenceIndex()
        {
            Assert.AreEqual(0, Tools.DifferenceIndex(0, 1));
            Assert.AreEqual(1, Tools.DifferenceIndex(0, 2));
            Assert.AreEqual(2, Tools.DifferenceIndex(0, 4));
        }

        [TestMethod]
        public void GetPower()
        {
            Assert.AreEqual(-1, Tools.GetPower(3));
            Assert.AreEqual(4, Tools.GetPower(2));
            Assert.AreEqual(2, Tools.GetPower(1));
            Assert.AreEqual(1, Tools.GetPower(0));
        }

        [TestMethod]
        public void IsPowerOf2()
        {
            Assert.AreEqual(false, Tools.IsPowerOf2(3), "3");
            Assert.AreEqual(true, Tools.IsPowerOf2(1024), "1024");
            Assert.AreEqual(true, Tools.IsPowerOf2(4), "4");
            Assert.AreEqual(false, Tools.IsPowerOf2(6), "6");
        }
    }
}
