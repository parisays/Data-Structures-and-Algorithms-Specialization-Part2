using System;
using System.Collections.Generic;
using TestCommon;

namespace A1
{
    public class Q1MazeExit : Processor
    {
        public Q1MazeExit(string testDataName) : base(testDataName) { }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<long, long[][], long, long, long>)Solve);


        public Dictionary<long, Vertex> Vertices;

        public long Solve(long nodeCount, long[][] edges, long StartNode, long EndNode)
        {
            CreateAdjacencyList(edges, nodeCount);

            if (!Vertices.ContainsKey(StartNode) || !Vertices.ContainsKey(EndNode))
                return 0;

            Explore(StartNode, 1);
            
            return (Vertices[StartNode].CC == Vertices[EndNode].CC) ? 1 : 0;
        }

        private void Explore(long startNode, long cc)
        {
            Vertices[startNode].Visited = true;
            Vertices[startNode].CC = cc;
            foreach(long node in Vertices[startNode].AdjacencyList)
            {
                if (!Vertices[node].Visited)
                    Explore(node, cc);
            }
            
        }

        private void CreateAdjacencyList(long[][] edges, long nodeCount)
        {
            Vertices = new Dictionary<long, Vertex>();

            foreach (var edge in edges)
            {
                if(!Vertices.ContainsKey(edge[0]))
                {
                    Vertices.Add(edge[0], new Vertex(edge[0]));
                }

                if (!Vertices.ContainsKey(edge[1]))
                {
                    Vertices.Add(edge[1], new Vertex(edge[1]));
                }

                Vertices[edge[0]].AdjacencyList.Add(edge[1]);
                Vertices[edge[1]].AdjacencyList.Add(edge[0]);
            }
             
            
        }
    }

    public class Vertex
    {
        public long Key;
        public bool Visited;
        public List<long> AdjacencyList;
        public long CC;
        public long PreVisit;
        public long PostVisit;
        

        public Vertex(long key, bool visited=false)
        {
            this.Key = key;
            this.Visited = visited;
            this.AdjacencyList = new List<long>();
            this.CC = -1;
        }


        public List<Vertex> AdjacenctVertices;
    }
}
