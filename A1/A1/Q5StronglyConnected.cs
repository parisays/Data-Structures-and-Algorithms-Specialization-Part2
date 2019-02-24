using System;
using System.Collections.Generic;
using System.Linq;
using TestCommon;

namespace A1
{
    public class Q5StronglyConnected: Processor
    {
        public Q5StronglyConnected(string testDataName) : base(testDataName) { }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<long, long[][], long>)Solve);

        private List<Vertex> GRVertices;
        public List<Vertex> Vertices;
        private long Clock;

        public long Solve(long nodeCount, long[][] edges)
        {
            CreateAdjacencyList(edges, nodeCount);
            DFS();
            return ComputeSCCs();
        }

        private long ComputeSCCs()
        {
            long numberOfSCC = 0;
            GRVertices = GRVertices.OrderBy(v => v.PostVisit).ToList();
            while (Vertices.Count>0)
            {
                var maxPost = GRVertices.Last();
                RemovalExploration(Vertices.Find(v => v.Key == maxPost.Key));
                numberOfSCC++;
            }
            return numberOfSCC;
        }

        private void RemovalExploration(Vertex v)
        {
            GRVertices.Remove(GRVertices.Find(n => n.Key == v.Key));
            Vertices.Remove(v);
            foreach (Vertex vv in v.AdjacenctVertices)
                if (Vertices.Contains(vv))
                    RemovalExploration(vv);
        }

        private void DFS()
        {
            Clock = 1;
            for(int node = 0; node<GRVertices.Count; node++)
                if (!GRVertices[node].Visited)
                    Explore(GRVertices[node]);
        }

        private void Explore(Vertex node)
        {
            node.Visited = true;
            PreVisit(node);
            foreach (var n in node.AdjacenctVertices)
                if (!n.Visited)
                    Explore(n);
            PostVisit(node);
        }

        private void PostVisit(Vertex node) => node.PostVisit = Clock++;

        private void PreVisit(Vertex node) => node.PreVisit = Clock++;

        private void CreateAdjacencyList(long[][] edges, long nodeCount)
        {
            GRVertices = new List<Vertex>();
            Vertices = new List<Vertex>();
            for (int i = 0; i < nodeCount; i++)
            {
                GRVertices.Add(new Vertex(i));
                GRVertices[i].AdjacenctVertices = new List<Vertex>();

                Vertices.Add(new Vertex(i));
                Vertices[i].AdjacenctVertices = new List<Vertex>();
            }

            foreach (var edge in edges)
            {
                Vertices[(int)edge[0] - 1].AdjacenctVertices.Add(Vertices.ElementAt((int)edge[1] - 1));
                GRVertices[(int)edge[1] - 1].AdjacenctVertices.Add(GRVertices.ElementAt((int)edge[0] - 1));
            }

        }
    }
}
