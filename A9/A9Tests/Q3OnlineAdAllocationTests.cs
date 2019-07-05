using Microsoft.VisualStudio.TestTools.UnitTesting;
using A9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A9.Tests
{
    [TestClass()]
    public class Q3OnlineAdAllocationTests
    {
        
        [TestMethod()]
        public void SolveTest1()
        {
            double[,] matrix = new double[4,4];
            matrix[0, 0] = 1;
            matrix[0, 1] = 1;
            matrix[0, 2] = 1;
            matrix[0, 3] = 600;
            matrix[1, 0] = 1;
            matrix[1, 1] = 3;
            matrix[1, 3] = 600;
            matrix[2, 0] = 2;
            matrix[2, 2] = 1;
            matrix[2, 3] = 900;
            matrix[3, 0] = 60;
            matrix[3, 1] = 90;
            matrix[3, 2] = 300;

            Q3OnlineAdAllocation q3 = new Q3OnlineAdAllocation("TD3");
            q3.Solve(3, 3, matrix);
        }

        [TestMethod()]
        public void SolveTest2()
        {
            double[,] matrix = new double[4, 4];
            matrix[0, 0] = 1;
            matrix[0, 1] = 1;
            matrix[0, 2] = 1;
            matrix[0, 3] = 100;
            matrix[1, 0] = 5;
            matrix[1, 1] = 4;
            matrix[1, 2] = 4;
            matrix[1, 3] = 480;
            matrix[2, 0] = 40;
            matrix[2, 1] = 20;
            matrix[2, 2] = 30;
            matrix[2, 3] = 3200;
            matrix[3, 0] = 70;
            matrix[3, 1] = 210;
            matrix[3, 2] = 140;

            Q3OnlineAdAllocation q3 = new Q3OnlineAdAllocation("TD3");
            q3.Solve(3, 3, matrix);
        }
    }
}