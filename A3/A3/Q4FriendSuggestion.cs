using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A3
{
    public class Q4FriendSuggestion : Processor
    {
        public Q4FriendSuggestion(string testDataName) : base(testDataName) { }

        public List<Node> Graph { get; private set; }
        private List<Node> RGraph { get; set; }

        private List<Node> Processed { get; set; }
        private List<Node> RProcessed { get; set; }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<long, long, long[][], long, long[][], long[]>)Solve);

        public long[] Solve(long NodeCount, long EdgeCount,
                              long[][] edges, long QueriesCount,
                              long[][] Queries)
        {

            Graph = new List<Node>();
            RGraph = new List<Node>();

            for (long i = 1; i <= NodeCount; i++)
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
            
            return BidirectionalDijkstra(Queries, NodeCount);

        }

        private long[] BidirectionalDijkstra(long[][] Queries, long nodeCount)
        {
            List<long> result = new List<long>();
            foreach (var query in Queries)
            {
                if (query[0] == query[1])
                    result.Add(0);

                else
                {
                    for (int i = 0; i < nodeCount; i++)
                    {
                        Graph[i].Distance = long.MaxValue;
                        RGraph[i].Distance = long.MaxValue;
                        Graph[i].Previous = null;
                        RGraph[i].Previous = null;
                    }


                    Processed = new List<Node>();
                    RProcessed = new List<Node>();

                    MinHeap heap = new MinHeap(Graph);
                    MinHeap rHeap = new MinHeap(RGraph);
                    
                    heap.ChangePriority( heap.Nodes.IndexOf(
                            heap.Nodes.Find( v=> v.Key == query[0]))
                            , 0);
                    rHeap.ChangePriority( rHeap.Nodes.IndexOf( 
                            rHeap.Nodes.Find( v=> v.Key == query[1]) )
                            , 0);

                    do
                    {
                        var vertex = heap.ExtractMin();

                        foreach (var neighbor in vertex.AdjacentNodes)
                        {
                            if (vertex.Distance != long.MaxValue
                                        && vertex.Distance + neighbor.Item2 < neighbor.Item1.Distance)
                            {
                                //if (vertex.Key == query[1] && neighbor.Item1.Key == query[0])
                                //{
                                //    result.Add(-1);
                                //    break;
                                //}
                                neighbor.Item1.Previous = vertex;
                                heap.ChangePriority(heap.Nodes.IndexOf(neighbor.Item1),
                                                vertex.Distance + neighbor.Item2);
                            }

                            Processed.Add(neighbor.Item1);

                        }

                        if (RProcessed.Exists(v => v.Key == vertex.Key))
                        {
                            result.Add(ShortestPath(query[0], query[1]));
                            break;
                        }

                        var vertexR = rHeap.ExtractMin();
                        foreach (var neighbor in vertexR.AdjacentNodes)
                        {
                            if (vertexR.Distance != long.MaxValue
                                        && vertexR.Distance + neighbor.Item2 < neighbor.Item1.Distance)
                            {

                                neighbor.Item1.Previous = vertexR;
                                rHeap.ChangePriority(rHeap.Nodes.IndexOf(neighbor.Item1),
                                                vertexR.Distance + neighbor.Item2);
                            }

                            RProcessed.Add(neighbor.Item1);

                        }

                        if (Processed.Exists(v => v.Key == vertexR.Key))
                        {
                            result.Add(ShortestPath(query[0], query[1]));
                            break;
                        }

                        if(heap.Size == 0 || rHeap.Size == 0)
                        {
                            result.Add(-1);
                            break;
                        }

                    } while (true);
                }


            }

            return result.ToArray();
        }

        private long ShortestPath(long source, long target)
        {
            long distance = long.MaxValue;
            Node uBest = null;
            List<Node> allProcessedNodes = new List<Node>();

            allProcessedNodes.AddRange(Processed);
            allProcessedNodes.AddRange(RProcessed);

            foreach (var n in allProcessedNodes)
            {
                var u = Graph.Find(v => v.Key == n.Key);
                var uR = RGraph.Find(v => v.Key == n.Key);

                if (u.Distance != long.MaxValue && uR.Distance != long.MaxValue
                    && u.Distance + uR.Distance < distance)
                {
                    uBest = n;
                    distance = u.Distance + uR.Distance;
                }
            }

            //List<Node> path = new List<Node>();
            //Node last = Graph.Find(n => n.Key == uBest.Key);

            //while(last.Key != source)
            //{
            //    path.Add(last);
            //    last = last.Previous;
            //}

            //path.Reverse();
            //last = RGraph.Find(n => n.Key == uBest.Key);

            //while (last.Key != target)
            //{
            //    last = last.Previous;
            //    path.Add(last);
            //}

            return (distance == long.MaxValue) ? -1: distance;
        }

    }
}
