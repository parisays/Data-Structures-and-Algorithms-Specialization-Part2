using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A6
{
    public class Q3MatchingAgainCompressedString : Processor
    {
        public Q3MatchingAgainCompressedString(string testDataName) : base(testDataName)
        {
            this.ExcludeTestCaseRangeInclusive(22, 23);
            this.ExcludeTestCaseRangeInclusive(25, 30);
        }

        public override string Process(string inStr) =>
        TestTools.Process(inStr, (Func<String, long, String[], long[]>)Solve);


        private Dictionary<char, int> FirstOccurrence { get; set; }

        private Dictionary<char, int[]> ArrayCount { get; set; }

        private char[] Symbols { get; set; }

        public long[] Solve(string text, long n, String[] patterns)
        {
            Symbols = text.Distinct().ToArray();
            int textSize = text.Length;
            PreProcess(text, textSize);

            long[] result = new long[n];
            for (int i = 0; i < n; i++)
                result[i] = BetterBWMatching(patterns[i], textSize);
            var q2 = new Q2ReconstructStringFromBWT("TD2");
            string original = q2.Solve(text);
            return result;

        }

        private long BetterBWMatching(string pattern, int bwtSize)
        {
            int top = 0;
            int bottom = bwtSize - 1;

            int patternSize = pattern.Length;
            int index = patternSize - 1;

            while (top <= bottom)
            {
                if (index >= 0)
                {
                    char symbol = pattern[index];
                    if (!Symbols.Contains(symbol))
                        return 0;
                    else
                    {
                        top = FirstOccurrence[symbol]
                                    + ArrayCount[symbol][top];

                        bottom = FirstOccurrence[symbol]
                                    + ArrayCount[symbol][bottom + 1] - 1;
                    }

                    index--;
                }

                else
                    return bottom - top + 1;
            }

            return 0;
        }

        private void PreProcess(string bwt, int size)
        {
            FirstOccurrence = new Dictionary<char, int>();
            ArrayCount = new Dictionary<char, int[]>();
            List<char> sortedBWT = bwt.ToList();
            sortedBWT.Sort();

            for (int i = 0; i < Symbols.Length; i++)
                ArrayCount.Add(Symbols[i], new int[size + 1]);

            for (int i = 0; i < size; i++)
            {
                // finding first occurrence of each symbol in bwt

                if (!FirstOccurrence.ContainsKey(sortedBWT[i]))
                    FirstOccurrence.Add(sortedBWT[i], i);

                // computing count of each symbol from 0 to i-th row of bwt

                foreach (char s in Symbols)
                    ArrayCount[s][i + 1] = ArrayCount[s][i];

                ArrayCount[bwt[i]][i + 1]++;

            }

            return;
        }
    }
}
