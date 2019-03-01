using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A2
{
    public class Q2BipartiteGraph : Processor
    {
        public Q2BipartiteGraph(string testDataName) : base(testDataName) { }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<long, long[][], long>)Solve);

        public List<Node> MyNodes;

        public long Solve(long NodeCount, long[][] edges)
        {
            MyNodes = new List<Node>();
            for (long i = 1; i <= NodeCount; i++)
                MyNodes.Add(new Node(i));

            foreach (var edge in edges)
            {
                MyNodes.ElementAt((int)edge[0] - 1).AdjacentNodes.Add(
                                        MyNodes.ElementAt((int)edge[1] - 1));

                MyNodes.ElementAt((int)edge[1] - 1).AdjacentNodes.Add(
                                        MyNodes.ElementAt((int)edge[0] - 1));
            }

            return CheckBipartite((int)NodeCount);
        }

        private long CheckBipartite(int nodeCount)
        {
            Queue<Node> myQueue = new Queue<Node>();
            int cc = 0;

            for (int index = 0; index < nodeCount; index++)
            {
                if (!MyNodes.ElementAt(index).Visited)
                    myQueue.Enqueue(MyNodes.ElementAt(index));

                while (myQueue.Count > 0)
                {
                    Node current = myQueue.Dequeue();
                    if (!current.Visited)
                    {
                        current.Visited = true;

                        if (current.CC == -1)
                        {
                            current.CC = cc;
                            cc = (cc + 1) % 2;
                        }
                        else
                            cc = (current.CC + 1) % 2;

                        foreach (var neighbor in current.AdjacentNodes)
                        {
                            if (current.CC != neighbor.CC && !neighbor.Visited)
                            {
                                neighbor.CC = cc;
                                myQueue.Enqueue(neighbor);
                            }
                            else if (current.CC == neighbor.CC)
                                return 0;
                        }

                    }
                }
            }

            return 1;
        }
    }

}
