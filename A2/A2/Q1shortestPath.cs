using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A2
{
    public class Q1ShortestPath : Processor
    {
        public Q1ShortestPath(string testDataName) : base(testDataName) { }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<long,long[][], long, long, long>)Solve);

        public List<Node> MyNodes;

        public long Solve(long NodeCount, long[][] edges, long StartNode,  long EndNode)
        {
            MyNodes = new List<Node>();
            for(long i=1; i<=NodeCount; i++)
               MyNodes.Add(new Node(i));
            
            foreach(var edge in edges)
            {
                MyNodes.ElementAt((int)edge[0] - 1).AdjacentNodes.Add(
                    MyNodes.ElementAt((int)edge[1] - 1));

                MyNodes.ElementAt((int)edge[1] - 1).AdjacentNodes.Add(
                    MyNodes.ElementAt((int)edge[0] - 1));
            }

            //MyNodes.ElementAt((int)StartNode - 1).Distance = 0;

            
            return Dijkstra((int)StartNode, EndNode);
            
        }

        private long Dijkstra(int startNode, long endNode)
        {
            MinHeap myHeap = new MinHeap(MyNodes);
            myHeap.ChangePriority(startNode - 1, 0);

            while(myHeap.Size > 0)
            {
                Node current = myHeap.ExtractMin();
                foreach(var neighbor in current.AdjacentNodes)
                {
                    if ( neighbor.Distance!=current.Distance && neighbor.Distance > current.Distance + 1)
                    {
                        //neighbor.Distance = current.Distance + 1;
                        neighbor.Previous = current;
                        myHeap.ChangePriority(myHeap.Nodes.IndexOf(neighbor), current.Distance + 1);
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
        public List<Node> AdjacentNodes;
        public int CC;
        public bool Visited;

        public Node(long key)
        {
            this.Key = key;
            this.Distance = long.MaxValue;
            this.Previous = null;
            this.AdjacentNodes = new List<Node>();
            this.CC = -1;
            this.Visited = false;
        }
    }
}
