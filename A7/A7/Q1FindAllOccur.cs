using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A7
{
    public class Q1FindAllOccur : Processor
    {
        public Q1FindAllOccur(string testDataName) : base(testDataName)
        {
			this.VerifyResultWithoutOrder = true;
        }

        public override string Process(string inStr) =>
        TestTools.Process(inStr, (Func<String, String, long[]>)Solve, "\n");

        public long[] Solve(string text, string pattern)
        {
            string searchString = pattern + "$" + text;

            int[] prefixes = ComputePrefixFunction(searchString);
            List<long> result = new List<long>();
            int patternSize = pattern.Length;
            int searchStringSize = searchString.Length;

            for (int i = patternSize + 1; i < searchStringSize; i++)
                if (prefixes[i] == patternSize)
                    result.Add(i - 2 * patternSize);

            if (result.Count == 0)
                result.Add(-1);
            return result.ToArray();

        }

        private int[] ComputePrefixFunction(string searchString)
        {
            int size = searchString.Length;
            int[] prefixes = new int[size];
            int border = 0;

            for(int i = 1; i < size; i++)
            {
                while (border > 0 && searchString[i] != searchString[border])
                    border = prefixes[border - 1];

                if (searchString[i] == searchString[border])
                    border++;
                else
                    border = 0;

                prefixes[i] = border;
            }

            return prefixes;

        }
    }
}
