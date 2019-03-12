using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;
namespace A3
{
    public class Q2DetectingAnomalies:Processor
    {
        public Q2DetectingAnomalies(string testDataName) : base(testDataName) { }

        public List<Node> Graph { get; private set; }
        private List<Node> RGraph { get; set; }
        private int Clock { get; set; }
        

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<long, long[][], long>)Solve);


        public long Solve(long nodeCount, long[][] edges)
        {
            Graph = new List<Node>();
            RGraph = new List<Node>();
            List<int> markedNodes = new List<int>();

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
            
            markedNodes = GetSCC(nodeCount);

            for (long i = 1; i <= nodeCount; i++)
                Graph.Add(new Node(i));
            
            foreach (var edge in edges)
            {
                // Recreates the original Graph.
                ValueTuple<Node, long> originalEdge = new ValueTuple<Node, long>
                                (Graph.ElementAt((int)edge[1] - 1), edge[2]);

                Graph[(int)edge[0] - 1].AdjacentNodes.Add(originalEdge);
                
            }

            return BellmanFord(markedNodes, nodeCount);
        }



        private long BellmanFord(List<int> markedNodes, long nodeCount)
        {
            for (int index = 0; index < markedNodes.Count; index++)
            {
                foreach (var node in Graph)
                    node.Distance = long.MaxValue;

                Graph[markedNodes[index] - 1].Distance = 0;

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
                                return 1;

            }

            return 0;
        }


        private List<int> GetSCC(long nodeCount)
        {
            List<int> markedNodes = new List<int>();
            DFSOnRGraph(nodeCount);
            
            RGraph = RGraph.OrderBy(v => v.PostVisit).ToList();
            while (Graph.Count > 0)
            {
                var maxPost = RGraph.Last();

                markedNodes.Add((int)maxPost.Key);

                RemovalExploration(Graph.Find(v => v.Key == maxPost.Key));
            }

            return markedNodes;
        }

        private void RemovalExploration(Node v)
        {
            RGraph.Remove(RGraph.Find(n => n.Key == v.Key));
            Graph.Remove(v);

            foreach (var vv in v.AdjacentNodes)
                if (Graph.Contains(vv.Item1))
                    RemovalExploration(vv.Item1);
        }

        private void DFSOnRGraph(long nodeCount)
        {
            Clock = 1;
            for (int node = 0; node < RGraph.Count; node++)
                if (!RGraph[node].Marked)
                    Explore(RGraph[node]);
        }

        private void Explore(Node node)
        {
            node.Marked = true;
            PreVisit(node);
            foreach (var n in node.AdjacentNodes)
                if (!n.Item1.Marked)
                    Explore(n.Item1);
            PostVisit(node);
        }

        private void PostVisit(Node node) => node.PostVisit = Clock++;

        private void PreVisit(Node node) => node.PreVisit = Clock++;

        
    }
}
