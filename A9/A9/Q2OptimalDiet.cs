using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Combinatorics.Collections;
using TestCommon;

namespace A9
{
    public class Q2OptimalDiet : Processor
    {
        public Q2OptimalDiet(string testDataName) : base(testDataName)
        {
            //this.ExcludeTestCaseRangeInclusive(1, 4);
            //this.ExcludeTestCaseRangeInclusive(6, 30);

        }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<int,int, double[,], String>)Solve);

        private const double Epsilon = 1e-3;
        private const double Infinity = 1e+9;

        public string Solve(int N,int M, double[,] matrix)
        {
            Equation equation = new Equation(N, M + 1, matrix);
            List<double[]> answers = SolveEqualities(equation: equation, restrictions: N, variables: M);

            double[] pleasureIneq = new double[M];
            for (int i = 0; i < M; i++)
                pleasureIneq[i] = matrix[N, i];

            int result = CheckInequalities(equation, answers, pleasureIneq);
            return SetResult(result, M, answers);
                
        }

        private string SetResult(int result, int m, List<double[]> answers)
        {
            if (result == -1)
                return "No Solution";
            else if (result == -2)
                return "Infinity";
            else
            {
                StringBuilder solution = new StringBuilder();
                solution.Append("Bounded Solution\n");

                for(int i=0; i<m; i++)
                {
                    double fraction = answers[result][i] - Math.Truncate(answers[result][i]);

                    if (Math.Abs(fraction) < 0.25)
                        answers[result][i] = Math.Truncate(answers[result][i]);

                    else if (Math.Abs(fraction) > 0.75)
                    {
                        if (answers[result][i] > 0)
                            answers[result][i] = Math.Truncate(answers[result][i]) + 1;
                        else
                            answers[result][i] = Math.Truncate(answers[result][i]) - 1;
                    }
                    else if (Math.Abs(fraction) != 0.0)
                    {

                        if (answers[result][i] > 0)
                            answers[result][i] = Math.Truncate(answers[result][i]) + 0.5;
                        else
                            answers[result][i] = Math.Truncate(answers[result][i]) - 0.5;
                    }

                    solution.Append(answers[result][i].ToString());
                    if (i < m - 1)
                        solution.Append(" ");
                }

                return solution.ToString();
            }
                
            
        }

        private int CheckInequalities(Equation equation, List<double[]> answers, double[] pleasureIneq)
        {
            int size = answers.Count;
            int bestAnswerIndex = -1;
            double maxPleasure = 0.0;

            // check if the current set of answers satisfies all the inequalities
            for(int ans=0; ans<size; ans++)
            {
                bool satisfy = true;
                for(int i=0; i<equation.RowsCount && satisfy; i++)
                {
                    double check = 0;

                    for (int j = 0; j < equation.ColsCount; j++)
                        check += equation.Coefficients[i, j] * answers[ans][j];

                    if (check - equation.Intercepts[i] > Epsilon && check != equation.Intercepts[i])
                        satisfy = false;
                        
                }

                if(satisfy)
                {
                    double p = 0;
                    for (int i = 0; i < equation.ColsCount; i++)
                        p += answers[ans][i] * pleasureIneq[i];

                    if(p >= maxPleasure)
                    {
                        bestAnswerIndex = ans;
                        maxPleasure = p;
                    }

                }
            }



            if(bestAnswerIndex !=-1)
            {
                double sum = 0;
                for (int i = 0; i < equation.ColsCount; i++)
                    sum += answers[bestAnswerIndex][i];

                if (sum + Epsilon >= Infinity)
                    bestAnswerIndex = -2;
            }


            return bestAnswerIndex;
        }

        private List<double[]> SolveEqualities(Equation equation, int restrictions, int variables)
        {
            List<double[]> allOutputs = new List<double[]>();
            Combinations<int> subsets = new Combinations<int>(
                                        Enumerable.Range(0, equation.RowsCount - 1).ToList(), variables
                                            );

            foreach(var set in subsets)
            {
                Equation subEquation = equation.CreateSubEquation(set);
                double[] output = RowReduction(subEquation);
                if (output.Length > 0)
                    allOutputs.Add(output);
            }

            return allOutputs;
        }

        private double[] RowReduction(Equation equation)
        {
            bool[] hasZeroRows = new bool[equation.RowsCount];
            bool[] hasZeroCols = new bool[equation.RowsCount];

            //convert the matrix into an upper triangular matrix
            for (int i = 0; i < equation.RowsCount; i++)
            {
                Pivot pivot = SelectPivot(equation, hasZeroRows, hasZeroCols);
                if (equation.Coefficients[pivot.Row, pivot.Column] == 0)
                    break;

                SwapRows(equation, ref hasZeroRows, pivot);
                ForwardElimination(equation, pivot, ref hasZeroRows, ref hasZeroCols);
            }

            //reduction to row echelon form
            BackSubstitution(equation);

            return equation.Intercepts;
        }
        

        private void BackSubstitution(Equation equation)
        {
            for (int i = equation.RowsCount - 1; i >= 0; i--)
            {
                double value = equation.Intercepts[i];
                for (int j = 0; j < i; j++)
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
            for (int i = pivot.Row + 1; i < equation.RowsCount; i++)
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
            for (int from = p.Row; from < equation.RowsCount; from++)
                if (Math.Abs(equation.Coefficients[from, p.Column]) > Math.Abs(maxValue))
                {
                    maxValue = equation.Coefficients[from, p.Column];
                    p.Row = from;
                }

            return p;
        }
    }
}
