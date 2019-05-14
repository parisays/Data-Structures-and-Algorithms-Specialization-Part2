using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A8
{
    public class Q2Airlines : Processor
    {
        public Q2Airlines(string testDataName) : base(testDataName)
        {
        }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<long, long, long[][], long[]>)Solve);


        private long[][] Graph { get; set; }
        private long[] Parents { get; set; }
        public virtual long[] Solve(long flightCount, long crewCount, long[][] info)
        {
            Network network = ConstructNetwork(flightCount, crewCount, info);
            FindMatching(network, 0, network.Size() - 1, flightCount);
            return AssignJobs(network, flightCount);
            
        }
        
        private long[] AssignJobs(Network network, long flightCount)
        {
            long[] result = Enumerable.Repeat<long>(-1, (int)flightCount).ToArray();

            for(int i = 0; i < flightCount; i++)
            {
                foreach(int id in network.GetIds(i + 1))
                {
                    Edge e = network.GetEdge(id);
                    if(e.Flow == 1)
                    {
                        result[i] = e.End - flightCount;
                        break;
                    }
                }

            }

            return result;
        }

        private void FindMatching(Network network, int source, int sink, long flightCount)
        {
            int size = network.Size();
            long[] predecessors = new long[size];

            do
            {
                BFS(network, source, sink, size,ref predecessors);

                if(predecessors[sink] != -1)
                {
                    long X = long.MaxValue;

                    // finding minimum X
                    for (long edgeID = predecessors[sink]; edgeID != -1;
                                    edgeID = predecessors[network.GetEdge((int)edgeID).Start])
                        X = Math.Min(X, network.GetEdge((int)edgeID).Capacity);
                    

                    for (long edgeID = predecessors[sink]; edgeID != -1;
                                    edgeID = predecessors[network.GetEdge((int)edgeID).Start])
                        network.AddFlow(X, (int)edgeID);
                    
                }
                

            } while (predecessors[sink] != -1);
            
        }

        private void BFS(Network network, int source, int sink, int size,ref long[] predecessors)
        {
            Queue<long> queue = new Queue<long>();
            predecessors = Enumerable.Repeat<long>(-1, size).ToArray();

            queue.Enqueue(source);

            while(queue.Count > 0)
            {
                long current = queue.Dequeue();

                foreach(int id in network.GetIds(current))
                {
                    Edge e = network.GetEdge(id);
                    
                    if(predecessors[e.End] == -1 && e.Capacity > 0 && e.End != source)
                    {
                        predecessors[e.End] = id;
                        queue.Enqueue(e.End);
                    }
                }
            }

            //return predecessors;

        }

        private Network ConstructNetwork(long flightCount, long crewCount, long[][] adjMatrix)
        {
            Network network = new Network(flightCount + crewCount + 2); // additional source and sink
            
            // Edges from source to flights
            for (int i = 0; i < flightCount; ++i)
                network.AddEdge(0, i + 1, 1);

            // Edges from flights to crews
            for (int i = 0; i < flightCount; i++)
                for (int j = 0; j < crewCount; j++)
                    if (adjMatrix[i][j] == 1)
                        network.AddEdge(i + 1, (int)flightCount + j + 1, 1);

            // Edges from crews to sink
            for (int i = 0; i < crewCount; i++)
                network.AddEdge((int)flightCount + i + 1, (int)flightCount + (int)crewCount + 1, 1);

            return network;
        }
    }
}
