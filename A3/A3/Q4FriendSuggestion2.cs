using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A3
{
    public class Q4FriendSuggestion2 : Processor
    {
        public Q4FriendSuggestion2(string testDataName) : base(testDataName) { }

        public List<Node> Graph { get; private set; }
        private List<Node> RGraph { get; set; }
        
        private SortedList<(long, long), Node> Forward { get; set; }
        private SortedList<(long, long), Node> Reverse { get; set; }
        

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<long, long, long[][], long, long[][], long[]>)Solve);

        public long[] Solve(long NodeCount, long EdgeCount,
                              long[][] edges, long QueriesCount,
                              long[][] Queries)
        {
            long[] result = new long[QueriesCount];

            CreateGraph(NodeCount, EdgeCount, edges);

            for (int i = 0; i < QueriesCount; i++)
                result[i] = BidirectionalDijkstra(Queries[i][0], Queries[i][1], NodeCount);

            return result;
        }

        private long BidirectionalDijkstra(long source, long target, long nodeCount)
        {
            if (source == target)
                return 0;
            
            List<Node> forwardProcess = new List<Node>();
            List<Node> reverseProcess = new List<Node>();

            Intialize(nodeCount, source, target);

            return MainProcess(forwardProcess, reverseProcess);
        }

        private long MainProcess(List<Node> forwardProcess, List<Node> reverseProcess)
        {
            while (Forward.Count > 0 && Reverse.Count > 0)
            {
                var vertex = Forward.ElementAt(0).Value;
                Forward.RemoveAt(0);

                if (vertex.Distance != long.MaxValue)
                    foreach (var neighbor in vertex.AdjacentNodes)
                    {
                        if (neighbor.Item1.Distance > vertex.Distance + neighbor.Item2)
                        {
                            neighbor.Item1.Previous = vertex;
                            neighbor.Item1.Distance = vertex.Distance + neighbor.Item2;
                            int index = Forward.IndexOfValue(neighbor.Item1);
                            if (index >= 0)
                                Forward.RemoveAt(index);

                            Forward.Add(new ValueTuple<long, long>(neighbor.Item1.Key,
                                    neighbor.Item1.Distance)
                                         , neighbor.Item1);
                        }
                    }

                forwardProcess.Add(vertex);
                if (reverseProcess.Exists(v => v.Key == vertex.Key))
                    return ShortestPath(forwardProcess, reverseProcess);



                vertex = Reverse.ElementAt(0).Value;
                Reverse.RemoveAt(0);

                if (vertex.Distance != long.MaxValue)
                    foreach (var neighbor in vertex.AdjacentNodes)
                    {
                        if (neighbor.Item1.Distance > vertex.Distance + neighbor.Item2)
                        {
                            neighbor.Item1.Previous = vertex;
                            neighbor.Item1.Distance = vertex.Distance + neighbor.Item2;
                            int index = Reverse.IndexOfValue(neighbor.Item1);
                            if (index >= 0)
                                Reverse.RemoveAt(index);
                            Reverse.Add(new ValueTuple<long, long>(neighbor.Item1.Key,
                                    neighbor.Item1.Distance)
                                         , neighbor.Item1);

                        }
                    }

                reverseProcess.Add(vertex);
                if (forwardProcess.Exists(v => v.Key == vertex.Key))
                    return ShortestPath(forwardProcess, reverseProcess);
            }

            return -1;
        }

        private long ShortestPath(List<Node> processed, List<Node> rProcessed)
        {
            long distance = long.MaxValue;
            List<Node> allProcessed = new List<Node>();
            allProcessed.AddRange(processed);
            allProcessed.AddRange(rProcessed);

            foreach (var node in allProcessed)
            {
                var forward = Graph.Find(n => n.Key == node.Key);
                var reverse = RGraph.Find(n => n.Key == node.Key);

                if (forward.Distance != long.MaxValue && reverse.Distance != long.MaxValue &&
                        distance > forward.Distance + reverse.Distance)
                    distance = forward.Distance + reverse.Distance;
            }

            return (distance == long.MaxValue) ? -1 : distance;
        }

        private void Intialize(long nodeCount, long source, long target)
        {
            Forward = new SortedList<(long, long), Node>(new MyComparer());
            Reverse = new SortedList<(long, long), Node>(new MyComparer());

            Graph.ForEach(n => n.Distance = long.MaxValue);
            RGraph.ForEach(n => n.Distance = long.MaxValue);

            Graph[(int)source - 1].Distance = 0;
            Forward.Add(new ValueTuple<long, long>
                    (Graph[(int)source - 1].Key, Graph[(int)source - 1].Distance), Graph[(int)source - 1]);


            RGraph[(int)target - 1].Distance = 0;
            Reverse.Add(new ValueTuple<long, long>
                (RGraph[(int)target - 1].Key, RGraph[(int)target - 1].Distance), RGraph[(int)target - 1]);



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
    }

    internal class MyComparer : IComparer<(long, long)>
    {
        public int Compare((long, long) x, (long, long) y)
        {
            if (x.Item2 == y.Item2)
                return x.Item1.CompareTo(y.Item1);
            else
                return x.Item2.CompareTo(y.Item2);
            
        }
    }

}
