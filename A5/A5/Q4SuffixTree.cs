using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A5
{
    public class Q4SuffixTree : Processor
    {
        public Q4SuffixTree(string testDataName) : base(testDataName)
        {
            this.VerifyResultWithoutOrder = true;
        }

        public override string Process(string inStr) =>
        TestTools.Process(inStr, (Func<String, String[]>)Solve);


        private Suffix Root { get; set; }

        public string[] Solve(string text)
        {
            // write your code here     
            int count = BuildTrie(text);
            return BuildSuffixTree(count);
			throw new NotImplementedException();
        }

        private string[] BuildSuffixTree(int trieCount)
        {
            List<string> result = new List<string>();
            
            throw new NotImplementedException();
        }

        private int BuildTrie(string text)
        {
            Root = new Suffix(-1, 0);
            int textSize = text.Length;
            int trieCount = 1;
            for (int i = 0; i < textSize; i++)
            {
                var current = Root;
                for (int j = i; j < textSize; j++)
                {
                    var point = current.Children.Find(x => text[x.Start] == text[j]);

                    if (point != null)
                    {
                        current = point;
                    }
                    else
                    {
                        if(!current.HasBranch)
                        {
                            current.HasBranch = true;

                            Suffix suffix = new Suffix(current.Start + 1, current.Length - 1);
                            current.Children.Add(suffix);

                            current.Length = 0;

                            if (text[suffix.Start] == text[j])
                            {
                                current = suffix;
                                continue;
                            }
                        }

                        Suffix newSuffix = new Suffix(j, textSize - j);
                        current.Children.Add(newSuffix);
                        trieCount++;
                        break;
                    }
                }
                

            }

            return trieCount;
        }

    }

    public class Suffix
    {
        public int Start { get; set; }
        public int Length { get; set; }
        public bool HasBranch { get; set; }
        public List<Suffix> Children { get; set; }

        public Suffix(int start, int length)
        {
            Start = start;
            Length = length;
            Children = new List<Suffix>();
            HasBranch = false;
        }
    }
}
