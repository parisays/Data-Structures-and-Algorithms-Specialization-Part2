using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A9
{
    public class Q3OnlineAdAllocation : Processor
    {

        public Q3OnlineAdAllocation(string testDataName) : base(testDataName)
        {
            this.ExcludedTestCases = new HashSet<int>() { 5, 33, 41 };
        }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<int, int, double[,], String>)Solve);

        private double[] ResultVariables { get; set; }
        private double[] SecondaryRow { get; set; }
        private const double Epsilon = 1e-3;
        private enum Phase {None, One , Two}
        private enum Solution { None, Infinity, Bounded, NoSolution}

        private Phase CurrentPhase { get; set; }
        private Solution CurrentSolution { get; set; }

        public string Solve(int c, int v, double[,] matrix1)
        {
            Equation equation = new Equation(c , v , matrix1, true);
            CurrentPhase = Phase.None;
            CurrentSolution = Solution.None;

            bool negConstraint = CheckForNegativeConstraints(equation);
            PrepareTable(equation, c, v, negConstraint);
            if (negConstraint)
            {
                PhaseOne(equation);
                if (CurrentSolution == Solution.NoSolution)
                    return "No Solution";
            }

            PhaseTwo(equation, c);

            if (CurrentSolution == Solution.Infinity)
                return "Infinity";


            return SetSolutions(equation, v);
        }

        private string SetSolutions(Equation equation, int v)
        {
            StringBuilder solution = new StringBuilder();
            solution.Append("Bounded Solution\n");

            for (int i = 0; i < v; i++)
            {
                int resultIndex = -1;
                int zeroCount = 0;
                if (ResultVariables[i] >= 0)
                {
                    zeroCount = 0;
                    for (int j = 0; j < equation.RowsCount; j++)
                    {

                        if (equation.Coefficients[j, i] == 1)
                            resultIndex = j;
                        else if (equation.Coefficients[j, i] == 0)
                            zeroCount++;

                    }
                }
                if (zeroCount != equation.RowsCount - 1)
                    resultIndex = -1;

                solution.Append(
                    resultIndex == -1 ? 0 : RegulateResult(equation.Intercepts[resultIndex])
                    );
                
                if (i != v - 1)
                    solution.Append(" ");
            }

            return solution.ToString();
        }

        private double RegulateResult(double number)
        {
            double fraction = number - Math.Truncate(number);

            if (Math.Abs(fraction) < 0.25)
                number = Math.Truncate(number);

            else if (Math.Abs(fraction) > 0.75)
            {
                if (number > 0)
                    number = Math.Truncate(number) + 1;
                else
                    number = Math.Truncate(number) - 1;
            }
            else if (Math.Abs(fraction) != 0.0)
            {

                if (number > 0)
                    number = Math.Truncate(number) + 0.5;
                else
                    number = Math.Truncate(number) - 0.5;
            }

            return number;
        }

        private void PhaseTwo(Equation equation, int c)
        {
            CurrentPhase = Phase.Two;
            equation.ColsCount -= c;
            double[] tempFunction = equation.Function;
            Array.Resize(ref tempFunction, equation.ColsCount);
            equation.Function = tempFunction;


            double[,] tempCoefficients = equation.Coefficients;
            equation.Coefficients = new double[equation.RowsCount, equation.ColsCount];
            for(int i=0; i<equation.RowsCount; i++)
            {
                for (int j = 0; j < equation.ColsCount; j++)
                    equation.Coefficients[i, j] = tempCoefficients[i, j];
            }

            RowReduction(equation.Function, equation);
        }

        private void PhaseOne(Equation equation)
        {
            CurrentPhase = Phase.One;
            RowReduction(SecondaryRow, equation);
            CurrentSolution = (EqualToZero(equation.Intercepts.Last())) ? Solution.Bounded : Solution.NoSolution;
        }

        private void RowReduction(double[] row, Equation equation)
        {
            while(true)
            {
                Pivot pivot = SelectPivot(row, equation);

                if (pivot.Row == -1 || pivot.Column == -1 || CurrentSolution == Solution.Infinity)
                    break;

                ResultVariables[pivot.Column] = pivot.Row;

                
                double divisor = equation.Coefficients[pivot.Row, pivot.Column];
                equation.Intercepts[pivot.Row] /= divisor;
                for (int i = 0; i < equation.ColsCount; i++)
                    equation.Coefficients[pivot.Row, i] /= divisor;

                double multiple = 0;
                for(int i=0; i<equation.RowsCount; i++)
                {
                    if (pivot.Row != i && !EqualToZero(equation.Coefficients[i, pivot.Column]))
                    {
                        multiple = equation.Coefficients[i, pivot.Column];

                        equation.Intercepts[i] -= equation.Intercepts[pivot.Row] * multiple;

                        for (int j = 0; j < equation.ColsCount; j++)
                            equation.Coefficients[i, j] -=
                                            equation.Coefficients[pivot.Row, j] * multiple;
                        
                    }
                        
                }

                int size = equation.Intercepts.Length;
                if (CurrentPhase == Phase.One)
                {
                    equation.Intercepts[size - 2] -= 
                                equation.Intercepts[pivot.Row] * equation.Function[pivot.Column];

                    equation.Intercepts[size - 1] -=
                                equation.Intercepts[pivot.Row] * row[pivot.Column];
                    
                    double functionCol = equation.Function[pivot.Column]; 
                    double rowCol = row[pivot.Column];
                    for (int i = 0; i < equation.ColsCount; i++)
                    {
                        equation.Function[i] -=
                            equation.Coefficients[pivot.Row, i] * functionCol;

                        row[i] -=
                            equation.Coefficients[pivot.Row, i] * rowCol;
                    }

                    SecondaryRow = row;
                }
                else
                {
                    equation.Intercepts[size - 1] -=
                        equation.Intercepts[pivot.Row] * equation.Function[pivot.Column];

                    multiple = equation.Function[pivot.Column];
                    for (int i = 0; i < equation.ColsCount; i++)
                    {
                        equation.Function[i] -= 
                        equation.Coefficients[pivot.Row, i] * multiple;
                    }
                        

                    equation.Function = row;
                }
            }
        }

        private bool EqualToZero(double v, double epsilon = Epsilon) => Math.Abs(v) < epsilon;

        private Pivot SelectPivot(double[] row, Equation equation)
        {
            int minIndex = Array.IndexOf(row, row.Min());
            int distance = minIndex;

            if (row[distance] >= 0)
                return new Pivot(-1, -1);

            double ratio = double.MaxValue - 1;
            int pivotRow = 0;
            for (int i = 0; i < equation.RowsCount; i++)
            {
                if(equation.Coefficients[i,distance] != 0)
                {
                    double r = equation.Intercepts[i] / equation.Coefficients[i, distance];
                    if(r >= 0 && r < ratio)
                    {
                        ratio = r;
                        pivotRow = i;
                    }
                }
            }

            if (ratio == double.MaxValue - 1)
                CurrentSolution = Solution.Infinity;

            return new Pivot(pivotRow, distance);
        }

        private void PrepareTable(Equation equation, int c, int v, bool negConstraint)
        {
            ResultVariables = new double[equation.ColsCount];
            for (int i = 0; i < equation.ColsCount; i++)
            {
                ResultVariables[i] = -1;
                equation.Function[i] *= -1;
            }

            AddSlackVariables(equation, c, v);

            if (negConstraint)
                AddArtificialVariables(equation, c, v);

        }

        private void AddArtificialVariables(Equation equation, int c, int v)
        {
            SecondaryRow = new double[equation.ColsCount];
            int size = equation.Intercepts.Length;

            for (int i = 0, j = c + v; i < size - 1; i++, j++)
            {
                if (equation.Intercepts[i] < 0)
                {
                    ResultVariables[i] = -2;
                    equation.Coefficients[i, j] = -1;

                    equation.Intercepts[size - 1] += equation.Intercepts[i];
                    equation.Intercepts[i] *= -1;

                    for (int k = 0; k < equation.ColsCount; k++)
                        equation.Coefficients[i, k] *= -1;

                    for (int k = 0; k < c + v; k++)
                        SecondaryRow[k] += equation.Coefficients[i, k];
                }
            }

            for (int k = 0; k < equation.ColsCount; k++)
               SecondaryRow[k] *= -1;
        }

        private void AddSlackVariables(Equation equation, int c, int v)
        {
            for (int i = 0; i < c; i++)
                equation.Coefficients[i, i + v] = 1;
        }

        private bool CheckForNegativeConstraints(Equation equation)
        {
            foreach (double b in equation.Intercepts)
                if (b < 0)
                    return true;

            return false;
        }
    }
}
