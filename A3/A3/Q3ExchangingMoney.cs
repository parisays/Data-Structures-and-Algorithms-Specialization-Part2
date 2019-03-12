using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;
namespace A3
{
    public class Q3ExchangingMoney:Processor
    {
        public Q3ExchangingMoney(string testDataName) : base(testDataName) { }

        public List<Node> Graph { get; private set; }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<long, long[][], long, string[]>)Solve);


        public string[] Solve(long nodeCount, long[][] edges, long startNode)
        {
            Graph = new List<Node>();

            for (long i = 1; i <= nodeCount; i++)
                Graph.Add(new Node(i));

            foreach (var edge in edges)
            {
                // Creates the Graph.
                ValueTuple<Node, long> originalEdge = new ValueTuple<Node, long>
                                (Graph.ElementAt((int)edge[1] - 1), edge[2]);

                Graph[(int)edge[0] - 1].AdjacentNodes.Add(originalEdge);

            }

            return BellmanFord(nodeCount, startNode);
        }

        private string[] BellmanFord(long nodeCount, long startNode)
        {
            List<string> result = new List<string>();
            List<Node> inCycle = new List<Node>();

            foreach (var node in Graph)
                node.Distance = long.MaxValue;

            Graph[(int)startNode - 1].Distance = 0;

            for (int i = 1; i < nodeCount; i++)
            {
                foreach (var current in Graph)
                    foreach (var neighbor in current.AdjacentNodes)
                    {
                        if (current.Distance != long.MaxValue &&
                                  neighbor.Item1.Distance > current.Distance + neighbor.Item2)
                        {
                            neighbor.Item1.Distance = current.Distance + neighbor.Item2;
                            neighbor.Item1.Previous = current;
                        }
                    }
            }

            foreach (var current in Graph)
                foreach (var neighbor in current.AdjacentNodes)
                    if (current.Distance != long.MaxValue &&
                                    neighbor.Item1.Distance > current.Distance + neighbor.Item2)
                    {
                        inCycle.Add(neighbor.Item1);
                        neighbor.Item1.Distance = current.Distance + neighbor.Item2;
                        neighbor.Item1.Previous = current;
                    }


            foreach (Node n in inCycle)
            {
                n.Marked = true;
                var prev = n.Previous;
                while (!prev.Marked )
                {
                    prev.Marked = true;
                    prev = prev.Previous;
                }

            }

            
            foreach (Node n in Graph)
            {
                if (n.Key == startNode && !n.Marked)
                    result.Add("0");

                else if (n.Distance == long.MaxValue)
                    result.Add("*");

                else if (n.Marked)
                    result.Add("-");

                else
                {
                    string distance = n.Distance.ToString();
                    var prev = n.Previous;
                    while (prev.Key != startNode)
                    {
                        if (prev.Marked)
                        {
                            distance = "-";
                            break;
                        }

                        prev = prev.Previous;
                    }
                    result.Add(distance);
                }
            }

            return result.ToArray();
        }
    }
}
