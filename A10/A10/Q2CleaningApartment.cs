using System;
using System.Collections.Generic;
using Combinatorics.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A10
{
    /// <summary>
    /// Determine whether it is possible to
    /// clean an apartment after a party without leaving any traces of the party.
    /// Reduce it to the classic Hamiltonian Path problem,
    /// and then design and implement an efficient algorithm to reduce
    /// it to SAT.
    /// </summary>
    public class Q2CleaningApartment : Processor
    {
        public Q2CleaningApartment(string testDataName) : base(testDataName)
        {
        }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<int, int, long[,], string[]>)Solve);


        private readonly string[] Unsatisfied = { "2 1", "1 0", "-1 0" };

        public string[] Solve(int V, int E, long[,] matrix)
        {
            List<string> clauses = new List<string>
            {
                "0"
            };
            
            int noEdgeVertex = EdgeConstraint(E, V, matrix, ref clauses);

            if (noEdgeVertex == -1)
            {
                VertexConstraints(V, ref clauses);
                PositionConstraints(V, ref clauses);
                clauses[0] = (clauses.Count - 1).ToString() + " " + (V * V).ToString();
            }
            else
                return Unsatisfied;
            
            return clauses.ToArray();
        }
        

        private int EdgeConstraint(int edgeCount,int nodeCount, 
                                long[,] matrix, ref List<string> clauses)
        {
            List<int>[] adjList = CreateAdjList(nodeCount, edgeCount, matrix);

            for(int node = 1; node<=nodeCount; node++)
            {
                if (adjList[node].Count > 0)
                {
                    for (int position = 1; position < nodeCount; position++)
                    {
                        clauses.Add((-1 * Varnum(nodeCount, node, position)).ToString() + " "
                            + adjList[node].Select(n => Varnum(nodeCount, n, position + 1).ToString())
                                .Aggregate((s1, s2) => s1 + " " + s2).ToString()
                                + " 0"
                            );
                    }
                }
                else
                    return node;
            }

            return -1;
        }

        private List<int>[] CreateAdjList(int nodeCount,int edgeCount, long[,] matrix)
        {
            List<int>[] adjList = new List<int>[nodeCount + 1];

            for (int i = 0; i < nodeCount + 1; i++)
                adjList[i] = new List<int>();

            for(int i=0; i<edgeCount; i++)
            {
                adjList[matrix[i, 0]].Add((int)matrix[i, 1]);
                adjList[matrix[i, 1]].Add((int)matrix[i, 0]);
            }
            return adjList;
        }

        private void PositionConstraints(int nodeCount, ref List<string> clauses)
        {
            for(int position = 1; position<=nodeCount; position++)
            {
                List<int> literals = new List<int>();
                for (int node = 1; node <= nodeCount; node++)
                    literals.Add(Varnum(nodeCount, node, position));

                ExactlyOneOf(literals, ref clauses);
            }
        }

        private void VertexConstraints(int nodeCount, ref List<string> clauses)
        {
            for(int node = 1; node<=nodeCount; node++)
            {
                List<int> literals = new List<int>();
                for (int position = 1; position <= nodeCount; position++)
                    literals.Add(Varnum(nodeCount, node, position));

                ExactlyOneOf(literals, ref clauses);
            }
        }

        private void ExactlyOneOf(List<int> literals, ref List<string> clauses)
        {
            clauses.Add(literals.Select(x => x.ToString())
                    .Aggregate((x1, x2) => x1 + " " + x2)
                    .ToString() + " 0"
                        );
            Combinations<int> pairs = new Combinations<int>(literals, 2);

            foreach (List<int> pair in pairs)
                    clauses.Add(pair.Select(x => (x * -1).ToString())
                    .Aggregate((x1, x2) => x1 + " " + x2)
                    .ToString() + " 0"
                        );
        }

        private int Varnum(int nodeCount, int node, int position) 
                    => (nodeCount * (node - 1)) + position;


        public override Action<string, string> Verifier { get; set; } =
            TestTools.SatVerifier;
    }
}
