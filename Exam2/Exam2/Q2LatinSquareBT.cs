using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestCommon;
using Combinatorics.Collections;

namespace Exam2
{
    public class Q2LatinSquareBT : Processor
    {
        public Q2LatinSquareBT(string testDataName) : base(testDataName)
        {
            this.ExcludedTestCases = new HashSet<int>() { 16 };
            this.ExcludeTestCaseRangeInclusive(20, 54);
        }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<int, int?[,], string>)Solve);

        public string Solve(int dim, int?[,] square)
        {
            List<List<int>> clauses = GetAllClauses(dim, square);
            return SATSolver(clauses, clauses);
        }

        private string SATSolver(List<List<int>> clauses1, List<List<int>> clauses2)
        {
            if (clauses1.Exists(x => x.Count == 0))
                return "UNSATISFIABLE";
            if (clauses1.Count == 0)
                return "SATISFIABLE";

            var l = clauses2.Find(x => x.Count == 1);

            if (l == null)
                l = clauses2[0];

            int unassigned = l.First();
            
            // set to 1
            for (int i = 0; i < clauses2.Count && i >= 0 && clauses2.Count > 0; i++)
            {
                if (clauses2[i].Contains(unassigned))
                    clauses2.RemoveAt(i--);

                else if (clauses2[i].Contains(-unassigned))
                    clauses2[i].Remove(-unassigned);
            }

            if (SATSolver(clauses2, clauses2) == "SATISFIABLE")
                return "SATISFIABLE";
            

            // set to 0
            for (int i = 0; i < clauses1.Count && i >= 0 && clauses1.Count > 0; i++)
            {
                if (clauses1[i].Contains(unassigned))
                    clauses1[i].Remove(unassigned);

                else if (clauses1[i].Contains(-unassigned))
                    clauses1.RemoveAt(i--);
            }

            if (SATSolver(clauses1, clauses1) == "SATISFIABLE")
                return "SATISFIABLE";


            return "UNSATISFIABLE";
        }

        private List<List<int>> GetAllClauses(int dim, int?[,] square)
        {
            List<List<int>> clauses = new List<List<int>>();

            CellConstraint(dim, ref clauses);
            RowConstraint(dim, ref clauses);
            ColumnConstraint(dim, ref clauses);

            for (int i = 0; i < dim; i++)
                for (int j = 0; j < dim; j++)
                {
                    if (square[i, j].HasValue)
                    {
                        List<int> single = new List<int>
                        {
                            Varnum(i, j, square[i, j].Value, dim)
                        };
                        clauses.Add(single);
                    }
                }

            return clauses;
        }

        private void ColumnConstraint(int dim, ref List<List<int>> clauses)
        {
            for (int j = 0; j < dim; j++)
                for (int k = 0; k < dim; k++)
                {
                    List<int> literals = new List<int>();
                    for (int i = 0; i < dim; i++)
                        literals.Add(Varnum(i, j, k, dim));

                    ExactlyOneOf(literals, ref clauses);
                }
        }

        private void RowConstraint(int dim, ref List<List<int>> clauses)
        {
            for (int i = 0; i < dim; i++)
                for (int k = 0; k < dim; k++)
                {
                    List<int> literals = new List<int>();
                    for (int j = 0; j < dim; j++)
                        literals.Add(Varnum(i, j, k, dim));

                    ExactlyOneOf(literals, ref clauses);
                }
        }

        private void CellConstraint(int dim, ref List<List<int>> clauses)
        {
            for (int i = 0; i < dim; i++)
                for (int j = 0; j < dim; j++)
                {
                    List<int> literals = new List<int>();

                    for (int k = 0; k < dim; k++)
                        literals.Add(Varnum(i, j, k, dim));

                    ExactlyOneOf(literals, ref clauses);

                }
        }

        private int Varnum(int i, int j, int k, int dim) => (i * (dim * dim)) + (j * dim) + k + 1;

        private void ExactlyOneOf(List<int> literals, ref List<List<int>> clauses)
        {
            clauses.Add(literals);

            Combinations<int> pairs = new Combinations<int>(literals, 2);

            foreach (List<int> pair in pairs)
                clauses.Add(pair.Select(x => -x).ToList());

        }
    }
}
