using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A3
{
    public class Q1MinCost:Processor
    {
        public Q1MinCost(string testDataName) : base(testDataName) { }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<long, long[][], long,long,long>)Solve);

        public List<Node> MyNodes;

        public long Solve(long nodeCount,long [][] edges, long startNode, long endNode)
        {
            if (startNode == endNode)
                return -1;

            MyNodes = new List<Node>();
            for (long i = 1; i <= nodeCount; i++)
                MyNodes.Add(new Node(i));

            foreach (var edge in edges)
            {
                ValueTuple<Node, long> thisEdge = new ValueTuple<Node, long>
                                (MyNodes.ElementAt((int)edge[1] - 1), edge[2]);

                MyNodes[(int)edge[0] - 1].AdjacentNodes.Add(thisEdge);
                
            }
            
            return Dijkstra((int)startNode, endNode);
        }


        private long Dijkstra(int startNode, long endNode)
        {
            MinHeap myHeap = new MinHeap(MyNodes);
            myHeap.ChangePriority(startNode - 1, 0);

            while (myHeap.Size > 0)
            {
                Node current = myHeap.ExtractMin();
                foreach (var neighbor in current.AdjacentNodes)
                {
                    if (long.MaxValue != current.Distance &&
                                    neighbor.Item1.Distance > current.Distance + neighbor.Item2)
                    {
                        neighbor.Item1.Previous = current;
                        myHeap.ChangePriority(myHeap.Nodes.IndexOf(neighbor.Item1),
                                            current.Distance + neighbor.Item2);
                    }

                }
            }

            long result = myHeap.Nodes.Find(n => n.Key == endNode).Distance;
            return (result == long.MaxValue) ? -1 : result;
        }
    }

    public class Node
    {
        public Node Previous;
        public long Distance;
        public long Key;

        public List<(Node, long)> AdjacentNodes;
        public bool Marked;

        public int PreVisit;
        public int PostVisit;

        public Node(long key)
        {
            this.Key = key;
            this.Distance = long.MaxValue;
            this.Previous = null;
            this.AdjacentNodes = new List<(Node,long)>();
            this.Marked = false;
        }
    }
}
