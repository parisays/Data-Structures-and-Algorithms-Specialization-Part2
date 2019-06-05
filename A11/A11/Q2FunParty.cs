using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A11
{
    public class Q2FunParty : Processor
    {
        public Q2FunParty(string testDataName) : base(testDataName)
        {
        }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<long,long[],long[][], long>)Solve);


        private Dictionary<int, List<int>> Graph { get; set; }

        private long[] D { get; set; }

        public virtual long Solve(long n, long [] funFactors, long[][] hierarchy)
        {
            CreateGraph(n, hierarchy);
            D = Enumerable.Repeat<long>(-1, (int)n).ToArray();
            
            return PlanParty(vertex: 1, parent: -1, funFactors: funFactors);
        }

        private long PlanParty(int vertex, int parent, long[] funFactors)
        {
            
            if(D[vertex - 1] == -1)
            {
                // the vertex doesn't have children
                if (!Graph.ContainsKey(vertex))
                    D[vertex - 1] = funFactors[vertex - 1];
                else
                {
                    // vertex and its grandchildren
                    long m1 = funFactors[vertex - 1];
                    foreach (int child in Graph[vertex])
                        if (child != parent)
                            foreach (int grandchild in Graph[child])
                                if(grandchild != vertex)
                                    m1 += PlanParty(grandchild, child, funFactors);

                    // children of vertex
                    long m0 = 0;
                    foreach (int child in Graph[vertex])
                        if( child != parent)
                            m0 += PlanParty(child, vertex, funFactors);

                    D[vertex - 1] = Math.Max(m0, m1);
                }
            }

            return D[vertex - 1];
        }

        private void CreateGraph(long n, long[][] hierarchy)
        {
            Graph = new Dictionary<int, List<int>>();
            foreach(var line in hierarchy)
            {
                int parent = (int)line[0];
                int child = (int)line[1];

                if (Graph.ContainsKey(parent))
                    Graph[parent].Add(child);
                else
                    Graph.Add(parent, new List<int>() { child });

                if (Graph.ContainsKey(child))
                    Graph[child].Add(parent);
                else
                    Graph.Add(child, new List<int>() { parent });
            }
        }
    }
}
