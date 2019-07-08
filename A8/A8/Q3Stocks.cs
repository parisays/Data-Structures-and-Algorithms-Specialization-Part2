using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A8
{
    public class Q3Stocks : Processor
    {
        public Q3Stocks(string testDataName) : base(testDataName)
        { }

        public override string Process(string inStr) =>
        TestTools.Process(inStr, (Func<long, long, long[][], long>)Solve);

        public virtual long Solve(long stockCount, long pointCount, long[][] matrix)
        {
            Network network = ConstructNetwork(stockCount, pointCount, matrix);
            Maxflow(network, 0, network.Size() - 1);
            return GetOverlaidCharts(network, stockCount);
        }

        private long GetOverlaidCharts(Network network, long stockCount)
        {
            int count = 0;
            for (int i = 1; i <= stockCount; i++)
                foreach(var id in network.GetIds(i))
                    if(network.GetEdge(id).Flow > 0)
                    {
                        count++;
                        break;
                    }

            return stockCount - count;
        }

        private void Maxflow(Network network, int source, int sink)
        {
            long flow = 0;
            int size = network.Size();

            while (true)
            {
                var result = BFS(network, source, sink, size);

                if (result.Item1.Count == 0)
                    return ;

                foreach (int id in result.Item1)
                    network.AddFlow(result.Item2, id);

                flow += result.Item2;
            }
        }

        private (List<int>, long) BFS(Network network, int source, int sink, int size)
        {
            long X = long.MaxValue;
            bool[] marked = new bool[size];
            (long, int)[] parent = new (long, int)[size]; // end : (start, edgeID)
            List<int> path = new List<int>();
            Queue<long> queue = new Queue<long>();

            marked[source] = true;
            queue.Enqueue(source);

            while (queue.Count > 0)
            {
                long currentStartNode = queue.Dequeue();
                foreach (int id in network.GetIds(currentStartNode))
                {
                    var currentEdge = network.GetEdge(id);
                    if (currentEdge.Capacity > 0 && !marked[currentEdge.End])
                    {
                        marked[currentEdge.End] = true;
                        parent[currentEdge.End] = (currentStartNode, id);
                        queue.Enqueue(currentEdge.End);

                        if (currentEdge.End == sink)
                        {
                            int idTemp = id;

                            while (true)
                            {
                                path.Add(idTemp); // creating the path from sink to source
                                X = Math.Min(X, network.GetEdge(idTemp).Capacity); // finding minimum X
                                if (currentStartNode == source)
                                    break;

                                idTemp = parent[currentStartNode].Item2;
                                currentStartNode = parent[currentStartNode].Item1;
                            }

                            return (path, X);
                        }
                    }
                }
            }
            return (path, X);
        }

        private Network ConstructNetwork(long stockCount, long pointCount, long[][] matrix)
        {
            Network network = new Network(2 * stockCount + 2);

            // from source to stocks
            for (int i = 0; i < stockCount; i++)
                network.AddEdge(0, i + 1, 1);

            for (int currentStock = 0; currentStock < stockCount; currentStock++)
            {
                for (int j = 0; j < stockCount; j++)
                {
                    if (currentStock != j)
                    {
                        bool isLess = true;
                        for (int point = 0; point < pointCount; point++)
                            if(matrix[currentStock][point] >= matrix[j][point])
                            {
                                isLess = false;
                                break;
                            }

                        if (isLess)
                            network.AddEdge(currentStock + 1, (int)stockCount + j + 1, 1);
                    }
                }
            }

            // from stocks to sink
            for (int i = (int)stockCount + 1; i <= stockCount * 2; i++)
                network.AddEdge(i, (int)stockCount * 2 + 1, 1);

            return network;
        }
    }
}
