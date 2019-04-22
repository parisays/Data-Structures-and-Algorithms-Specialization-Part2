using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace Exam1
{
    public class Q1Betweenness : Processor
    {
        public Q1Betweenness(string testDataName) : base(testDataName)
        {
            this.ExcludedTestCases = new HashSet<int>() { 8 };
            this.ExcludeTestCaseRangeInclusive(15, 36);
        }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<long, long[][], long[]>)Solve);


        private List<Node> Graph { get; set; }

        private PriorityQueue MyQueue { get; set; }

        public long[] Solve(long NodeCount, long[][] edges)
        {
            CreateGraphAndQueue(NodeCount, edges);
            
            for(int i=0; i<NodeCount; i++)
            {
                Setup();
                Dijkstra(Graph[i]);
                CheckBetweennessCentrality(Graph[i], NodeCount);
            }

            long[] result = new long[NodeCount];

            for (int i = 0; i < NodeCount; i++)
                result[Graph[i].Key - 1] = Graph[i].BetweennessCentrality;

            return result;
        }

        private void CheckBetweennessCentrality(Node start, long nodeCount)
        {
            for(int i=0; i<nodeCount; i++)
            {
                if(Graph[i].Distance!=long.MaxValue && Graph[i].Key!= start.Key)
                {
                    var prev = Graph[i].Previous;

                    while(prev.Key != start.Key)
                    {
                        prev.BetweennessCentrality = prev.BetweennessCentrality + 1;
                        prev = prev.Previous;
                    }
                }
            }
            
        }

        private void Dijkstra(Node start)
        {
            start.Distance = 0;
            MyQueue.Insert(start);

            while (MyQueue.Size > 0)
            {
                Node current = MyQueue.ExtractMin();

                foreach (var neighbor in current.AdjacentNodes)
                {
                    if (long.MaxValue != current.Distance &&
                                    neighbor.Distance > current.Distance + 1)
                    {
                        neighbor.Previous = current;

                        if (MyQueue.Exists[neighbor.Key - 1])
                            MyQueue.ChangePriority(Array.IndexOf(MyQueue.NodesList, neighbor)
                                , current.Distance + 1);
                        else
                        {
                            neighbor.Distance = current.Distance + 1;
                            MyQueue.Insert(neighbor);
                        }
                    }

                }
            }
            
        }


        private void Setup()
        {
            foreach (Node n in Graph)
            {
                n.Previous = null;
                n.Distance = long.MaxValue;
            }

            MyQueue.MakeQueue();
        }

        private void CreateGraphAndQueue(long nodeCount, long[][] edges)
        {
            Graph = new List<Node>();

            for (long i = 1; i <= nodeCount; i++)
                Graph.Add(new Node(i));

            foreach (var edge in edges)
            {
                
                Graph[(int)edge[0] - 1].AdjacentNodes.Add(Graph.ElementAt((int)edge[1] - 1));

            }

            MyQueue = new PriorityQueue(nodeCount);
        }
    }

    public class Node
    {
        public Node Previous;
        public long Distance;
        public long Key;
        public long BetweennessCentrality;
        public List<Node> AdjacentNodes;

        public Node(long key)
        {
            this.Key = key;
            this.Distance = long.MaxValue;
            this.Previous = null;
            this.BetweennessCentrality = 0;
            this.AdjacentNodes = new List<Node>();
        }

    }
}
