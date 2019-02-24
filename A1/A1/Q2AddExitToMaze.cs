using System;
using System.Collections.Generic;
using TestCommon;

namespace A1
{
    public class Q2AddExitToMaze : Processor
    {
        public Q2AddExitToMaze(string testDataName) : base(testDataName) { }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<long, long[][], long>)Solve);

        public List<Vertex> Vertices;

        
        public long Solve(long nodeCount, long[][] edges)
        {
            CreateAdjacencyList(edges, nodeCount);

            long result = DFS();
            
            return result;
        }


        private long DFS()
        {
            long cc = 0;
            Stack<Vertex> stack = new Stack<Vertex>();
            foreach(Vertex node in Vertices)
            {
                if(!node.Visited)
                {
                    cc++;
                    stack.Push(node);
                    while(stack.Count>0)
                    {
                        var current = stack.Pop();
                        current.CC = cc;
                        current.Visited = true;
                        foreach (var v in current.AdjacenctVertices)
                            if (!v.Visited)
                                stack.Push(v);
                    }
                }
            }

            return cc;
        }

        


        private void CreateAdjacencyList(long[][] edges, long nodeCount)
        {
            Vertices = new List<Vertex>();
            for (int i = 0; i < nodeCount; i++)
            {
                Vertices.Add(new Vertex(i + 1));
                Vertices[i].AdjacenctVertices = new List<Vertex>();
            }
            foreach (var edge in edges)
            {
                Vertices[(int)edge[0] - 1].AdjacenctVertices.Add(Vertices[(int)edge[1] - 1]);
                Vertices[(int)edge[1] - 1].AdjacenctVertices.Add(Vertices[(int)edge[0] - 1]);
            }

        }
    }
}
