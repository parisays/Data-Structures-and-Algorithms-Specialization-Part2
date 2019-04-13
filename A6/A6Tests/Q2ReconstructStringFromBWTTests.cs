using Microsoft.VisualStudio.TestTools.UnitTesting;
using A6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A6.Tests
{
    [TestClass()]
    public class Q2ReconstructStringFromBWTTests
    {
        [TestMethod()]
        public void Q2ReconstructStringFromBWTTest()
        {
            var q2 = new Q2ReconstructStringFromBWT("TD2");
            Assert.AreEqual("banana$", q2.Solve("annb$aa"));
        }
    }
}