using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A8
{
    public class Network
    {
        public List<Edge> Edges { get; set; }
        public List<int>[] Graph { get; set; }

        public Network(long nodeCount)
        {
            Edges = new List<Edge>();
            Graph = new List<int>[nodeCount];
            for (int i = 0; i < nodeCount; i++)
                Graph[i] = new List<int>();
        }


        public void AddEdge(int start, int end, long capacity)
        {
            Edge forwardEdge = new Edge(start, end, capacity);
            Edge backwardEdge = new Edge(end, start, 0);

            Graph[start].Add(Edges.Count);
            Edges.Add(forwardEdge);

            Graph[end].Add(Edges.Count);
            Edges.Add(backwardEdge);

        }

        public int Size() => Graph.Length;

        public List<int> GetIds(long start) => Graph[start];

        public Edge GetEdge(int id) => Edges[id];

        public void AddFlow(long flow,int id)
        {
            Edges[id].Flow += flow;
            Edges[id ^ 1].Flow -= flow;
            Edges[id].Capacity -= flow;
            Edges[id ^ 1].Capacity += flow;
        }

    }

    public class Edge
    {
        public long Capacity { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
        public long Flow { get; set; }

        public Edge(int start, int end, long capacity)
        {
            this.Capacity = capacity;
            this.Start = start;
            this.End = end;
            this.Flow = 0;
        }
    }
}
