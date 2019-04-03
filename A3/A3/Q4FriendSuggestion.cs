using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A3
{
    public class Q4FriendSuggestion : Processor
    {
        public Q4FriendSuggestion(string testDataName) : base(testDataName)
        {
        }

        public List<Node> Graph { get; private set; }
        private List<Node> RGraph { get; set; }

        private List<Node> ForwardProcss { get; set; }
        private List<Node> ReverseProcess { get; set; }

        private PriorityQueue ForwardHeap;
        private PriorityQueue ReverseHeap;

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<long, long, long[][], long, long[][], long[]>)Solve);

        public long[] Solve(long NodeCount, long EdgeCount,
                              long[][] edges, long QueriesCount,
                              long[][] Queries)
        {
            CreateGraph(NodeCount, EdgeCount, edges);

            long[] result = new long[QueriesCount];
            
            for (int i = 0; i < QueriesCount; i++)
                result[i] = BidirectionalDijkstra(Queries[i][0], Queries[i][1], NodeCount);

            return result;

        }

       
        private long BidirectionalDijkstra(long source, long target, long nodeCount)
        {
            if (source == target)
                return 0;

            Setup(nodeCount, source, target);

            while(ForwardHeap.Size>0 && ReverseHeap.Size>0)
            {
                var forward = ForwardHeap.ExtractMin();

                foreach(var neighbor in forward.AdjacentNodes)
                {
                    if(neighbor.Item1.Distance > forward.Distance + neighbor.Item2)
                    {
                        neighbor.Item1.Previous = forward;
                        
                        if (ForwardHeap.Exists[neighbor.Item1.Key - 1])
                            ForwardHeap.ChangePriority(Array.IndexOf(ForwardHeap.NodesList, neighbor.Item1)
                                , forward.Distance + neighbor.Item2);
                        else
                        {
                            neighbor.Item1.Distance = forward.Distance + neighbor.Item2;
                            ForwardHeap.Insert(neighbor.Item1);
                        }
                    }
                }

                ForwardProcss.Add(forward);
                if (ReverseProcess.Exists(v => v.Key == forward.Key))
                    return ShortestPath();

                var reverse = ReverseHeap.ExtractMin();

                foreach(var neighbor in reverse.AdjacentNodes)
                {
                    if(reverse.Distance + neighbor.Item2 < neighbor.Item1.Distance)
                    {
                        neighbor.Item1.Previous = reverse;
                        
                        if (ReverseHeap.Exists[neighbor.Item1.Key - 1])
                            ReverseHeap.ChangePriority(Array.IndexOf(ReverseHeap.NodesList, neighbor.Item1)
                                , reverse.Distance + neighbor.Item2);
                        else
                        {
                            neighbor.Item1.Distance = reverse.Distance + neighbor.Item2;
                            ReverseHeap.Insert(neighbor.Item1);
                        }
                    }
                }

                ReverseProcess.Add(reverse);
                if (ForwardProcss.Exists(v => v.Key == reverse.Key))
                    return ShortestPath();
            }

            return -1;
        }

        
        private long ShortestPath( )
        {
            long distance = long.MaxValue;
            
            foreach (var node in ForwardProcss)
            {
                var reverse = RGraph.ElementAt((int)node.Key - 1);

                if (node.Distance != long.MaxValue && reverse.Distance != long.MaxValue &&
                        distance > node.Distance + reverse.Distance)
                    distance = node.Distance + reverse.Distance;
            }

            foreach(var node in ReverseProcess)
            {
                var forward = Graph.ElementAt((int)node.Key - 1);

                if (node.Distance != long.MaxValue && forward.Distance != long.MaxValue &&
                        distance > node.Distance + forward.Distance)
                    distance = node.Distance + forward.Distance;
            }

            return (distance == long.MaxValue) ? -1 : distance;
        }

        private void CreateGraph(long nodeCount, long edgeCount, long[][] edges)
        {
            Graph = new List<Node>();
            RGraph = new List<Node>();

            for (long i = 1; i <= nodeCount; i++)
            {
                Graph.Add(new Node(i));
                RGraph.Add(new Node(i));
            }

            foreach (var edge in edges)
            {
                // Creates the original Graph.
                ValueTuple<Node, long> originalEdge = new ValueTuple<Node, long>
                                (Graph.ElementAt((int)edge[1] - 1), edge[2]);

                Graph[(int)edge[0] - 1].AdjacentNodes.Add(originalEdge);

                // Creates the reversed Graph.
                ValueTuple<Node, long> reversedEdge = new ValueTuple<Node, long>
                                (RGraph.ElementAt((int)edge[0] - 1), edge[2]);

                RGraph[(int)edge[1] - 1].AdjacentNodes.Add(reversedEdge);

            }
        }

        private void Setup(long nodeCount, long source, long target)
        {
            Graph.ForEach(n => n.Distance = long.MaxValue);
            RGraph.ForEach(n => n.Distance = long.MaxValue);

            ForwardHeap = new PriorityQueue(nodeCount);
            ReverseHeap = new PriorityQueue(nodeCount);

            ForwardProcss = new List<Node>();
            ReverseProcess = new List<Node>();

            Graph[(int)source - 1].Distance = 0;
            RGraph[(int)target - 1].Distance = 0;

            ForwardHeap.Insert(Graph[(int)source - 1]);
            ReverseHeap.Insert(RGraph[(int)target - 1]);
        }
    }

}
