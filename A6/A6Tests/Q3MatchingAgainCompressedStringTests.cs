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
    public class Q3MatchingAgainCompressedStringTests
    {
        [TestMethod()]
        public void SolveTest()
        {
            var q3 = new Q3MatchingAgainCompressedString("TD3");
            CollectionAssert.AreEqual(new long[] { 3 },
                    q3.Solve("smnpbnnaaaaa$a", 1, new string[] { "ana" }
                ));
        }
    }
}