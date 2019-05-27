using System;
using TestCommon;

namespace A9
{
    public class Q1InferEnergyValues : Processor
    {
        public Q1InferEnergyValues(string testDataName) : base(testDataName)
        {
        }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<long, double[,], double[]>)Solve);

        public double[] Solve(long MATRIX_SIZE, double[,] matrix)
        {
            Equation equation = new Equation(MATRIX_SIZE, matrix);
            double[] result = RowReduction(equation);
            return result;
        }

        private double[] RowReduction(Equation equation)
        {
            bool[] hasZeroRows = new bool[equation.RowsCount];
            bool[] hasZeroCols = new bool[equation.RowsCount];

            //convert the matrix into an upper triangular matrix
            for (int i=0; i<equation.RowsCount; i++)
            {
                Pivot pivot = SelectPivot(equation, hasZeroRows, hasZeroCols);
                SwapRows(equation, ref hasZeroRows, pivot);
                ForwardElimination(equation, pivot, ref hasZeroRows, ref hasZeroCols);
            }

            //reduction to row echelon form
            BackSubstitution(equation);

            return StandardizePrecision(equation);
        }

        private double[] StandardizePrecision(Equation equation)
        {
            for(int i=0; i<equation.RowsCount; i++)
            {
                double fraction = equation.Intercepts[i] - Math.Truncate(equation.Intercepts[i]);

                if (Math.Abs(fraction) < 0.25)
                    equation.Intercepts[i] = Math.Truncate(equation.Intercepts[i]);

                else if (Math.Abs(fraction) > 0.75)
                {
                    if(equation.Intercepts[i] > 0)
                        equation.Intercepts[i] = Math.Truncate(equation.Intercepts[i]) + 1;
                    else
                        equation.Intercepts[i] = Math.Truncate(equation.Intercepts[i]) - 1;
                }
                else if(Math.Abs(fraction) != 0.0)
                {

                    if (equation.Intercepts[i] > 0)
                        equation.Intercepts[i] = Math.Truncate(equation.Intercepts[i]) + 0.5;
                    else
                        equation.Intercepts[i] = Math.Truncate(equation.Intercepts[i]) - 0.5;
                }
            }
            return equation.Intercepts;
        }

        private void BackSubstitution(Equation equation)
        {
            for(int i = equation.RowsCount - 1; i>= 0; i--)
            {
                double value = equation.Intercepts[i];
                for(int j=0; j<i; j++)
                {
                    equation.Intercepts[j] -= (value * equation.Coefficients[j, i]);
                    equation.Coefficients[j, i] = 0;
                }
            }
        }

        private void ForwardElimination(Equation equation, Pivot pivot, 
                                                ref bool[] hasZeroRows, ref bool[] hasZeroCols)
        {
            //Scaling
            double divisor = equation.Coefficients[pivot.Row, pivot.Column];

            for (int i = pivot.Column; i < equation.RowsCount; i++)
                equation.Coefficients[pivot.Row, i] /= divisor;
            equation.Intercepts[pivot.Row] /= divisor;


            //Adding or Subtracting
            for(int i = pivot.Row + 1; i<equation.RowsCount; i++)
            {
                double multiple = equation.Coefficients[i, pivot.Column];

                equation.Intercepts[i] -= (multiple * equation.Intercepts[pivot.Row]);

                for (int j = pivot.Column; j < equation.RowsCount; j++)
                    equation.Coefficients[i, j] -= (multiple * equation.Coefficients[pivot.Row, j]);
            }

            
            //Mark the used pivot
            hasZeroRows[pivot.Row] = true;
            hasZeroCols[pivot.Column] = true;
        }

        private void SwapRows(Equation equation, ref bool[] hasZeroRows, Pivot pivot)
        {
            for (int i = 0; i < equation.RowsCount; i++)
                (equation.Coefficients[pivot.Row, i], equation.Coefficients[pivot.Column, i]) =
                                (equation.Coefficients[pivot.Column, i], equation.Coefficients[pivot.Row, i]);

            (equation.Intercepts[pivot.Row], equation.Intercepts[pivot.Column]) =
                                (equation.Intercepts[pivot.Column], equation.Intercepts[pivot.Row]);

            (hasZeroRows[pivot.Row], hasZeroRows[pivot.Column]) = (hasZeroRows[pivot.Column], hasZeroRows[pivot.Row]);

            pivot.Row = pivot.Column;
        }

        private Pivot SelectPivot(Equation equation, bool[] hasZeroRows, bool[] hasZeroCols)
        {
            Pivot p = new Pivot(0, 0);
            while (hasZeroRows[p.Row])
                p.Row += 1;
            while (hasZeroCols[p.Column])
                p.Column += 1;

            double maxValue = 0;
            for(int from = p.Row; from< equation.RowsCount; from++)
                if(Math.Abs(equation.Coefficients[from, p.Column])> Math.Abs(maxValue))
                {
                    maxValue = equation.Coefficients[from, p.Column];
                    p.Row = from;
                }

            return p;
        }
    }
}
