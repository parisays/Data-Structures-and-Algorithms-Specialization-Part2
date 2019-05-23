using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A9
{
    public class Equation
    {
        public double[,] Coefficients { get; set; }
        public double[] Intercepts { get; set; }
        public int Size { get; set; }

        public Equation(long size, double[,] matrix)
        {
            Coefficients = new double[size,size];
            Intercepts = new double[size];
            Size = (int)size;

            for(int i=0; i<size; i++)
            {
                Intercepts[i] = matrix[i, size];

                for (int j = 0; j < size; j++)
                    Coefficients[i, j] = matrix[i, j];
            }
        }
    }

    public class Pivot
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public Pivot(int row, int col)
        {
            Row = row;
            Column = col;
        }
    }
}
