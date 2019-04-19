using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A7
{
    public class Q3PatternMatchingSuffixArray : Processor
    {
        public Q3PatternMatchingSuffixArray(string testDataName) : base(testDataName)
        {
            this.VerifyResultWithoutOrder = true;
            this.ExcludedTestCases = new HashSet<int>() { 14, 16, 32 };
        }

        public override string Process(string inStr) =>
        TestTools.Process(inStr, (Func<String, long, string[], long[]>)Solve, "\n");

        private long[] Solve(string text, long n, string[] patterns)
        {
            Q2CunstructSuffixArray utility = new Q2CunstructSuffixArray("TD3");
            long[] suffixArray = utility.Solve(text + "$").Skip(1).ToArray();

            int textSize = text.Length;
            List<long> result = new List<long>();

            foreach (string pattern in patterns)
                result.AddRange(FindOccurrences(pattern, text, textSize, suffixArray));

            if (result.Count == 0)
                result.Add(-1);

            return result.ToArray();
        }

        private List<long> FindOccurrences(string pattern, string text, int textSize, long[] suffixArray)
        {
            int minIndex = 0;
            int maxIndex = textSize;
            int patternSize = pattern.Length;
            List<long> result = new List<long>();

            while(minIndex < maxIndex)
            {
                int middle = (minIndex + maxIndex) / 2;
                int length = (int)Math.Min(patternSize, textSize - suffixArray[middle]);
                string suffix = text.Substring((int)suffixArray[middle],
                           length);

                if (pattern.CompareTo(suffix) > 0)
                    minIndex = middle + 1;
                else
                    maxIndex = middle;
            }

            int startIndex = minIndex;

            
            maxIndex = textSize;
            while (minIndex < maxIndex)
            {
                int middle = (minIndex + maxIndex) / 2;
                string suffix = text.Substring((int)suffixArray[middle],
                             (int)Math.Min(patternSize, textSize - suffixArray[middle]));


                if (pattern.CompareTo(suffix) < 0)
                    maxIndex = middle;
                else
                    minIndex = middle + 1;
            }

            int endIndex = maxIndex;


            if (startIndex <= endIndex)
                for (int i = startIndex; i < endIndex; i++)
                    result.Add(suffixArray[i]);

            return result;
        }
    }
}
