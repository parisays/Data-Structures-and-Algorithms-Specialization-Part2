using System;
using System.Collections.Generic;
using System.Linq;
using TestCommon;
using Microsoft.SolverFoundation.Solvers;

namespace A11
{
    public class Q1CircuitDesign : Processor
    {
        public Q1CircuitDesign(string testDataName) : base(testDataName)
        {
            //this.ExcludeTestCaseRangeInclusive(1, 1);
            //this.ExcludeTestCaseRangeInclusive(1, 9);
        }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<long, long, long[][], Tuple<bool, long[]>>)Solve);

        public override Action<string, string> Verifier =>
            TestTools.SatAssignmentVerifier;

        public virtual Tuple<bool, long[]> Solve(long variableCount, long clauseCount, long[][] cnf)
        {
            Dictionary<int, List<int>> iGraph = new Dictionary<int, List<int>>();
            Dictionary<int, List<int>> iRGraph = new Dictionary<int, List<int>>();

            ConstructImplicationGraph(ref iGraph, ref iRGraph, clauseCount, cnf);

            Dictionary<int, Node> nodes = new Dictionary<int, Node>();
            for (int i = -(int)variableCount; i <= variableCount; i++)
                nodes.Add(i, new Node(i));
            nodes.Remove(0);

            List<List<int>> orderOfCC = GetSCC(nodes, iGraph, iRGraph);
            return CheckSatisfiability(orderOfCC, nodes, variableCount);
        }

        private Tuple<bool, long[]> CheckSatisfiability(List<List<int>> orderOfCC,
            Dictionary<int, Node> nodes,
            long variableCount)
        {
            if (orderOfCC == null)
                return new Tuple<bool, long[]>(false, new long[] { });


            long[] result = new long[variableCount];
            foreach(List<int> cc in orderOfCC)
            {
                foreach(int element in cc)
                    if(result[Math.Abs(element) - 1] == 0)
                    {
                        if (element < 0)
                            result[-element - 1] = -element;
                        else
                            result[element - 1] = -element;

                    }
            }

            return new Tuple<bool, long[]>(true, result);
        }

        private List<List<int>> GetSCC(Dictionary<int, Node> nodes,
                            Dictionary<int, List<int>> iGraph,
                            Dictionary<int, List<int>> iRGraph)
        {
            DFS(ref nodes, ref iRGraph);
            List<List<int>> connectedComponent = new List<List<int>>();
            int cc = 0;
            foreach (Node n in nodes.Select(n => n.Value).OrderByDescending(n => n.PostOrder))
            {
                if (!n.Marked && iGraph.ContainsKey(n.Key))
                {
                    cc++;
                    connectedComponent.Add(new List<int>());
                    if (!Explore(n.Key, ref nodes, ref iGraph, cc, ref connectedComponent))
                        return null;
                }
            }

            connectedComponent.Reverse();
            return connectedComponent;
        }

        private bool Explore(int key, ref Dictionary<int, Node> nodes,
                                ref Dictionary<int, List<int>> iGraph, int cc,
                                ref List<List<int>> connectedComponent)
        {
            if (connectedComponent[cc - 1].Contains(-key))
                return false;

            nodes[key].Marked = true;
            connectedComponent[cc - 1].Add(key);
            foreach (var adj in iGraph[key])
            {
                if (!nodes[adj].Marked)
                {
                    if (iGraph.ContainsKey(adj))
                    {
                        if (!Explore(adj, ref nodes, ref iGraph, cc, ref connectedComponent))
                            return false;

                    }
                    else
                    {
                        if (connectedComponent[cc - 1].Contains(-key))
                            return false;

                        nodes[adj].Marked = true;
                        connectedComponent[cc - 1].Add(adj);
                    }
                }
            }
            return true;
        }

        private void DFS(ref Dictionary<int, Node> nodes, ref Dictionary<int, List<int>> iRGraph)
        {
            int clock = 1;
            foreach (var node in nodes)
                if (iRGraph.ContainsKey(node.Key) && !node.Value.Visited)
                    ExploreReverse(node.Key, ref nodes, ref iRGraph, ref clock);
        }

        private void ExploreReverse(int key,
                        ref Dictionary<int, Node> nodes,
                        ref Dictionary<int, List<int>> iRGraph,
                        ref int clock)
        {
            nodes[key].Visited = true;
            PreVisit(nodes[key], ref clock);

            foreach (var adj in iRGraph[key])
                if (!nodes[adj].Visited)
                {
                    if (iRGraph.ContainsKey(adj))
                        ExploreReverse(adj, ref nodes, ref iRGraph, ref clock);
                    else
                    {
                        nodes[adj].Visited = true;
                        PreVisit(nodes[adj], ref clock);
                        PostVisit(nodes[adj], ref clock);
                    }
                }

            PostVisit(nodes[key], ref clock);

        }

        private void PostVisit(Node node, ref int clock) => node.PostOrder = clock++;

        private void PreVisit(Node node, ref int clock) => node.PreOrder = clock++;

        private void ConstructImplicationGraph(ref Dictionary<int, List<int>> iGraph,
                                    ref Dictionary<int, List<int>> iRGraph,
                                    long clauseCount,
                                    long[][] cnf)
        {
            for (int i = 0; i < clauseCount; i++)
            {
                int left = (int)cnf[i][0];
                int right = (int)cnf[i][1];

                // implication graph
                if (iGraph.ContainsKey(-left))
                    iGraph[-left].Add(right);
                else
                {
                    iGraph.Add(-left, new List<int>() { right });
                }


                if (iGraph.ContainsKey(-right))
                    iGraph[-right].Add(left);
                else
                {
                    iGraph.Add(-right, new List<int>() { left });
                }

                // reverse implication graph
                if (iRGraph.ContainsKey(right))
                    iRGraph[right].Add(-left);
                else
                {
                    iRGraph.Add(right, new List<int>() { -left });
                }


                if (iRGraph.ContainsKey(left))
                    iRGraph[left].Add(-right);
                else
                {
                    iRGraph.Add(left, new List<int>() { -right });
                }
            }
        }
    }
}
