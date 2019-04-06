using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A5
{
    public class Q1ConstructTrie : Processor
    {
        public Q1ConstructTrie(string testDataName) : base(testDataName)
        {
            this.VerifyResultWithoutOrder = true;
        }
        
        private Node Root { get; set; }

        public override string Process(string inStr) =>
        TestTools.Process(inStr, (Func<long, String[], String[]>) Solve);

        public string[] Solve(long n, string[] patterns)
        {
            Root = new Node(0);
            return BuildTrie(n, patterns);
        }

        private string[] BuildTrie(long n, string[] patterns)
        {
            List<string> result = new List<string>();
            int TrieCount = 1;
            foreach(string pattern in patterns)
            {
                var current = Root;
                int patternSize = pattern.Length;
                for(int i=0; i<patternSize; i++)
                {
                    var edge = current.AdjList.Find(x => x.Item2 == pattern[i]);
                    if (CheckExisting(edge))
                    {
                        current = edge.Item1;
                    }
                    else
                    {
                        var newEdge = new ValueTuple<Node, char, int>
                                    (new Node(TrieCount), pattern[i], current.Key);

                        current.AdjList.Add(newEdge);
                        result.Add(MakeString(newEdge));
                        TrieCount++;
                        current = newEdge.Item1;
                    }
                }
            }

            return result.ToArray();
        }

        private string MakeString((Node, char, int) newEdge) 
            =>  newEdge.Item3 + "->" + newEdge.Item1.Key + ":" + newEdge.Item2;
    
        private bool CheckExisting((Node, char, int) edge) => edge.Item1 != null;
    }

    public class Node
    {
        public int Key { get; set; }
        /// <summary>
        /// destination Node, Label, source index
        /// </summary>
        public List<(Node, char, int)> AdjList { get; set; }
        
        public Node(int key)
        {
            Key = key;
            AdjList = new List<(Node, char, int)>();
        }

    }
}
