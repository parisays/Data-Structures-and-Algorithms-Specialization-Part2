using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A4
{
    public class Q3ComputeDistance : Processor
    {
        public Q3ComputeDistance(string testDataName) : base(testDataName) { }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<long,long, long[][], long[][], long, long[][], long[]>)Solve);


        private List<Node> Graph;

        public long[] Solve(long nodeCount, 
                                long edgeCount,
                                    long[][] points,
                                        long[][] edges,
                                            long queriesCount,
                                                long[][] queries)
        {


            Graph = new List<Node>();

            for (int i = 0; i < nodeCount; i++)
                Graph.Add(new Node(i + 1, points[i][0], points[i][1]));

            foreach(var edge in edges)
                Graph[(int)edge[0] - 1].Adj.Add(
                        new ValueTuple<Node, long>(Graph[(int)edge[1] - 1], edge[2])
                            );

            long[] result = new long[queriesCount];

            for (int q = 0; q < queriesCount; q++)
                result[q] = AStar(queries[q][0], queries[q][1]);

            return result;
        }

        private long AStar(long source, long target)
        {
            if (source == target)
                return 0;

            Setup(Graph.Find(n=> n.Key == target));

            Graph[(int)source - 1].Distance = 0;
            Graph[(int)source - 1].Potential = 0;

            MinHeap heap = new MinHeap(Graph);
            
            heap.ChangePriority((int)source - 1, 0);

            while(heap.Size>0)
            {
                var currentPoint = heap.ExtractMin();

                //if (currentPoint.Key == target)
                //    return (currentPoint.Distance == long.MaxValue) ? -1 : currentPoint.Distance;

                if (currentPoint.Distance != long.MaxValue )
                    foreach (var neighbor in currentPoint.Adj)
                    {
                        if (neighbor.Item1.Distance > currentPoint.Distance + neighbor.Item2)
                        {
                            neighbor.Item1.Distance = currentPoint.Distance + neighbor.Item2;
                            neighbor.Item1.Previous = currentPoint;
                            heap.ChangePriority(heap.Nodes.IndexOf(neighbor.Item1),
                                    neighbor.Item1.Distance + neighbor.Item1.Potential
                                        //PotentialCalculator(neighbor.Item1, heap.Nodes.Find(n => n.Key == target))

                                        );
                            
                        }
                    }
            }


            int targetPoint = Graph.IndexOf(Graph.Find(n => n.Key == target));
            return (Graph[targetPoint].Distance == long.MaxValue) ? -1 : Graph[targetPoint].Distance;
            //return -1;
        }

        private void Setup(Node target)
        {
            foreach (var point in Graph)
            {
                point.Distance = long.MaxValue;
                point.DistancePi = long.MaxValue;
                point.Previous = null;
                point.Potential = PotentialCalculator(point, target);
            }

        }

        private double PotentialCalculator(Node point1, Node point2) =>
            Math.Sqrt(
                Math.Pow(point1.X - point2.X, 2) + Math.Pow(point1.Y - point2.Y, 2)
                );

    }

    public class Node
    {
        public long X { get; private set; }
        public long Y { get; private set; }
        public long Key { get; private set; }
        public double DistancePi { get; set; }
        public long Distance { get; set; }
        public Node Previous { get; set; }
        public double Potential { get; set; }

        public List<(Node, long)> Adj { get; set; }

        public Node(long key, long x, long y)
        {
            this.Key = key;
            this.X = x;
            this.Y = y;
            this.Adj = new List<(Node, long)>();
            this.Previous = null;
        }
            
    }
}
