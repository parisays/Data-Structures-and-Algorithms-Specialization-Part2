using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A11
{
    public class Q3SchoolBus : Processor
    {
        public Q3SchoolBus(string testDataName) : base(testDataName)
        {
        }

        public override string Process(string inStr)=>
            TestTools.Process(inStr, (Func<long, long[][], Tuple<long, long[]>>)Solve);

        public override Action<string, string> Verifier { get; set; } =
            TestTools.TSPVerifier;

        public virtual Tuple<long, long[]> Solve(long nodeCount, long[][] edges)
        {
            long[,] graph = CreateGraph(nodeCount, edges);
            throw new NotImplementedException();
        }
        

        private long[,] CreateGraph(long nodeCount, long[][] edges)
        {
            long[,] graph = new long[nodeCount, nodeCount];
            foreach(var edge in edges)
            {
                long left = edge[0];
                long right = edge[1];
                long weight = edge[2];

                graph[left - 1, right - 1] = weight;
                graph[right - 1, left - 1] = weight;
            }
            return graph;
        }
    }
}
