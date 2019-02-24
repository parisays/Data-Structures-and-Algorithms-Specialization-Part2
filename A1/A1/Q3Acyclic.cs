using System;
using System.Collections.Generic;
using TestCommon;

namespace A1
{
    public class Q3Acyclic : Processor
    {
        public Q3Acyclic(string testDataName) : base(testDataName) { }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<long, long[][], long>)Solve);

        public Dictionary<long, Vertex> Vertices;
        

        public long Solve(long nodeCount, long[][] edges)
        {
            CreateAdjacencyList(edges, nodeCount);
            return IsCyclic();
        }

        private long IsCyclic()
        {
            foreach(long node in Vertices.Keys)
            {
                if (!Vertices[node].Visited)
                    if (Explore(node,node))
                        return 1;
            }

            return 0;
        }

        private bool Explore(long start ,long node)
        {
            Vertices[node].Visited = true;
            bool result = false;
            foreach(long n in Vertices[node].AdjacencyList)
            {
                if (n == start)
                    return true;
                else if (!Vertices[n].Visited)
                    result = Explore(start,n);

                if (result)
                    break;
            }

            return result;
        }

        private void CreateAdjacencyList(long[][] edges, long nodeCount)
        {
            Vertices = new Dictionary<long, Vertex>();
            
            for (int i = 1; i <= nodeCount; i++)
            {
                Vertices.Add(i, new Vertex(i));
            }

            foreach (var edge in edges)
                    Vertices[edge[0]].AdjacencyList.Add(edge[1]);
        }
    }
}