using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A8
{
    public class Q1Evaquating : Processor
    {
        public Q1Evaquating(string testDataName) : base(testDataName)
        {
        }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<long, long, long[][], long>)Solve);

        public virtual long Solve(long nodeCount, long edgeCount, long[][] edges)
        {
            if (edgeCount == 0)
                return 0;

            Network network = CreateNetwork(edges, nodeCount);
            return Maxflow(network, 0, nodeCount - 1);
        }

        private long Maxflow(Network network, int start, long end)
        {
            long flow = 0;
            int size = network.Size();

            while (true)
            {
                var result = BFS(network, start, end, size);

                if (!result.Item1)
                    return flow;

                foreach (int id in result.Item2)
                    network.AddFlow(result.Item3, id);

                flow += result.Item3;
            }

        }
        
        private (bool, List<int>, long) BFS(Network network, int start, long end, int size)
        {
            long X = long.MaxValue;
            bool existsPath = false;
            bool[] marked = new bool[size];
            (long, int)[] parent = new (long, int)[size]; // end : (start, edgeID)
            List<int> path = new List<int>();
            Queue<long> queue = new Queue<long>();
            
            marked[start] = true;
            queue.Enqueue(start);
            
            while (queue.Count > 0)
            {
                long currentStartNode = queue.Dequeue();
                foreach (int id in network.GetIds(currentStartNode))
                {
                    var currentEdge = network.GetEdge(id);
                    if (currentEdge.Capacity > 0 && !marked[currentEdge.End] && currentEdge.End != start)
                    {
                        marked[currentEdge.End] = true;
                        parent[currentEdge.End] = (currentStartNode, id);
                        queue.Enqueue(currentEdge.End);

                        if (currentEdge.End == end)
                        {
                            int idTemp = id;
                            while (true)
                            {
                                path.Add(idTemp); // creating the path from sink to source
                                long currentX = network.GetEdge(idTemp).Capacity;
                                X = Math.Min(X, currentX); // getting minimum X
                                if (currentStartNode == start)
                                    break;

                                idTemp = parent[currentStartNode].Item2;
                                currentStartNode = parent[currentStartNode].Item1;
                            }

                            existsPath = true;
                            return (existsPath, path, X);
                        }
                    }
                }
            }
            return (existsPath, path, X);
        }
        

        public Network CreateNetwork(long[][] edges, long nodeCount)
        {
            Network network = new Network(nodeCount);

            foreach (var line in edges)
                network.AddEdge((int)line[0] - 1, (int)line[1] - 1, line[2]);

            return network;
        }
    }
}
