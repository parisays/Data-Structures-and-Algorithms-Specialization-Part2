using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A6
{
    public class Q4ConstructSuffixArray : Processor
    {
        public Q4ConstructSuffixArray(string testDataName) : base(testDataName)
        {
        }

        public override string Process(string inStr) => 
            TestTools.Process(inStr, (Func<String, long[]>)Solve);

        public long[] Solve(string text)
        {
            SortedList<string, long> suffixes = new SortedList<string, long>();
            int size = text.Length;

            for (int i = 0; i < size; i++)
                suffixes.Add(text.Substring(i), i);

            return suffixes.Values.ToArray();
        }
    }
}
