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
        public int RowsCount { get; set; }
        public int ColsCount { get; set; }
        public double[] Function { get; set; }
        private const double Infinity = 1e+9;

        public Equation() { }


        public Equation(long size, double[,] matrix)
        {
            Coefficients = new double[size, size];
            Intercepts = new double[size];
            RowsCount = (int)size;
            ColsCount = (int)size;

            for (int i = 0; i < size; i++)
            {
                Intercepts[i] = matrix[i, size];

                for (int j = 0; j < size; j++)
                    Coefficients[i, j] = matrix[i, j];
            }
        }
        

        public Equation(long rowsCount,long colsCount, double[,] matrix, bool simplex=false)
        {
            if(!simplex)
            {
                // given inequalities + non negative amounts + infinity
                RowsCount = (int)rowsCount + (int)colsCount;
                ColsCount = (int)colsCount - 1;

                Coefficients = new double[RowsCount, ColsCount];
                Intercepts = new double[RowsCount];

                // given inequalities
                for (int i = 0; i < rowsCount; i++)
                {
                    Intercepts[i] = matrix[i, colsCount - 1];

                    for (int j = 0; j < colsCount - 1; j++)
                        Coefficients[i, j] = matrix[i, j];
                }


                // infinity
                for (int i = 0; i < ColsCount; i++)
                    Coefficients[RowsCount - 1, i] = 1;

                Intercepts[RowsCount - 1] = Infinity;


                // non negative amounts 
                for (long row = rowsCount, col = 0; row < RowsCount - 1 && col < ColsCount; col++, row++)
                {
                    Coefficients[row, col] = -1;
                    Intercepts[row] = 0;
                }
            }
            else
            {
                // number of costraints 
                RowsCount = (int)rowsCount;
                // variables + slack varibles + artificial variables
                ColsCount = (int)(2 * rowsCount + colsCount);

                Coefficients = new double[RowsCount, ColsCount];
                Intercepts = new double[rowsCount + 2];
                Function = new double[ColsCount];

                // given inequalities
                for (int i = 0; i < rowsCount; i++)
                {
                    Intercepts[i] = matrix[i, colsCount];

                    for (int j = 0; j < colsCount; j++)
                        Coefficients[i, j] = matrix[i, j];
                    
                }

                for (int j = 0; j < colsCount; j++)
                    Function[j] = matrix[rowsCount, j];
            }

        }

        public Equation CreateSubEquation(IList<int> set)
        {
            Equation subEq = new Equation();
            subEq.ColsCount = subEq.RowsCount = set.Count;
            subEq.Coefficients = new double[subEq.RowsCount, subEq.ColsCount];
            subEq.Intercepts = new double[subEq.RowsCount];
            

            for(int row =0; row<subEq.RowsCount; row++)
            {
                subEq.Intercepts[row] = this.Intercepts[set[row]];
                for (int col = 0; col < subEq.ColsCount; col++)
                    subEq.Coefficients[row, col] = this.Coefficients[set[row], col];
            }

            return subEq;
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
