using Microsoft.SolverFoundation.Solvers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Combinatorics.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TestCommon;

namespace A10
{
    /// <summary>
    /// Reduce the real-world problem about
    /// assigning frequencies to the transmitting towers of the cells in a GSM
    /// network to a problem of proper coloring a graph into 3 colors.
    /// Then design and implement an algorithm to reduce this problem to an
    /// instance of SAT.
    /// </summary>
    public class Q1FrequencyAssignment : Processor
    {
        public Q1FrequencyAssignment(string testDataName) : base(testDataName)
        {
        }

        private readonly int[] Colors = new int[] { 1, 2, 3 };

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<int, int, long[,], string[]>)Solve);

        public string[] Solve(int V, int E, long[,] matrix)
        {
            List<List<int>> allClauses = WriteAllClauses(V, E, matrix);
            return OutputFormula(V, allClauses);
        }

        private string[] OutputFormula(int nodeCount, List<List<int>> allClauses)
        {
            List<string> result = new List<string>();
            StringBuilder builder = new StringBuilder();

            // output the number of clauses in the formula and the number of variables respectively.
            builder.Append(allClauses.Count.ToString() + " " + (nodeCount * Colors.Length).ToString());
            result.Add(builder.ToString());

            foreach (List<int> clause in allClauses)
            {
                clause.Add(0);
                result.Add(clause.Select(x => x.ToString())
                            .Aggregate((x1, x2) => x1 + " " + x2)
                            .ToString());

            }
            return result.ToArray();
        }

        private List<List<int>> WriteAllClauses(int nodeCount, int edgeCount, long[,] matrix)
        {
            List<List<int>> clauses = new List<List<int>>();
            for (int n = 1; n <= nodeCount; n++)
                ExactlyOneOf(n, ref clauses);
            NeighborsConstraint(edgeCount, matrix, ref clauses);

            return clauses;
        }

        private void NeighborsConstraint(int edgeCount, long[,] matrix, ref List<List<int>> clauses)
        {
            for(int row = 0; row < edgeCount; row++)
            {
                List<int> edge = null;
                foreach (int color in Colors)
                {
                    edge = new List<int>
                    {
                        Varnum((int)matrix[row, 0], color) * -1,
                        Varnum((int)matrix[row, 1], color) * -1
                    };
                    clauses.Add(edge);
                }
            }
        }

        private void ExactlyOneOf(int node, ref List<List<int>> clauses)
        {
            List<int> literals = new List<int>();

            foreach (int color in Colors)
                literals.Add(Varnum(node, color));

            clauses.Add(literals);

            Combinations<int> pairs = new Combinations<int>(literals, 2);
            foreach (List<int> pair in pairs)
                clauses.Add(pair.Select(x => x * -1).ToList());
        }

        private int Varnum(int n, int color) => Colors.Length * (n - 1) + color;

        public override Action<string, string> Verifier { get; set; } =
            TestTools.SatVerifier;
    }
}
