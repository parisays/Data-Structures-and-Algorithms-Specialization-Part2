using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TestCommon;

namespace A1
{
    public class Q4OrderOfCourse: Processor
    {
        public Q4OrderOfCourse(string testDataName) : base(testDataName) { }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<long, long[][], long[]>)Solve);


        public Dictionary<long, Vertex> Vertices;

        private long Clock;

        public long[] Solve(long nodeCount, long[][] edges)
        {
            CreateAdjacencyList(edges, nodeCount);
            DFS();
            long[] result 
                = Vertices.Values.OrderBy(v => v.PostVisit).Select(v=> v.Key).Reverse().ToArray();
            return result;
        }


        private void DFS()
        {
            Clock = 1;
            foreach (long node in Vertices.Keys)
               if (!Vertices[node].Visited)
                  Explore(node);
            
        }

        private void Explore(long node)
        {
            Vertices[node].Visited = true;
            PreVisit(node);
            foreach (long n in Vertices[node].AdjacencyList)
                if (!Vertices[n].Visited)
                    Explore(n);
            PostVisit(node);
        }

        private void PostVisit(long node) => Vertices[node].PostVisit = Clock++;

        private void PreVisit(long node) => Vertices[node].PreVisit = Clock++;

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



        public override Action<string, string> Verifier { get; set; } = TopSortVerifier;

        /// <summary>
        /// کد شما با متد زیر راست آزمایی میشود
        /// این کد نباید تغییر کند
        /// داده آزمایشی فقط یک جواب درست است
        /// تنها جواب درست نیست
        /// </summary>
        public static void TopSortVerifier(string inFileName, string strResult)
        {
            long[] topOrder = strResult.Split(TestTools.IgnoreChars)
                .Select(x => long.Parse(x)).ToArray();

            long count;
            long[][] edges;
            TestTools.ParseGraph(File.ReadAllText(inFileName), out count, out edges);

            // Build an array for looking up the position of each node in topological order
            // for example if topological order is 2 3 4 1, topOrderPositions[2] = 0, 
            // because 2 is first in topological order.
            long[] topOrderPositions = new long[count];
            for (int i = 0; i < topOrder.Length; i++)
                topOrderPositions[topOrder[i] - 1] = i;
            // Top Order nodes is 1 based (not zero based).

            // Make sure all direct depedencies (edges) of the graph are met:
            //   For all directed edges u -> v, u appears before v in the list
            foreach (var edge in edges)
                if (topOrderPositions[edge[0] - 1] >= topOrderPositions[edge[1] - 1])
                    throw new InvalidDataException(
                        $"{Path.GetFileName(inFileName)}: " +
                        $"Edge dependency violoation: {edge[0]}->{edge[1]}");

        }
    }
}
