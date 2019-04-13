using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A6
{
    public class Q2ReconstructStringFromBWT : Processor
    {
        public Q2ReconstructStringFromBWT(string testDataName) : base(testDataName)
        {
        }

        public override string Process(string inStr) =>
        TestTools.Process(inStr, (Func<String, String>)Solve);

        public string Solve(string bwt)
        {
            int textSize = bwt.Length;
            List<int> idx = new List<int>(textSize);

            for (int i = 0; i < textSize; i++)
                idx.Add(i);

            idx = idx.OrderBy(i => bwt[i]).ToList();

            int[] idxsOfIdxs = new int[textSize];
            for (int i = 0; i < textSize; i++)
                idxsOfIdxs[idx[i]] = i;

            var result = ReconstructString(bwt, idx, idxsOfIdxs, textSize);
            return result;
        }

        
        private string ReconstructString(string bwt, List<int> idxs, int[] idxsOfIdxs, int textSize)
        {
            int current = idxs[0];
            char[] originalText = new char[textSize];
            for (int i = textSize - 2; i >= 0; i--)
            {
                int index = idxsOfIdxs[current];
                originalText[i] = bwt[index];
                current = index;
            }

            originalText[textSize - 1] = '$';
            var result = CreateString(originalText, textSize);
            return result;
        }

        private string CreateString(char[] originalText, int size)
        {
            StringBuilder result = new StringBuilder(size);
            foreach (char c in originalText)
                result.Append(c);

            return result.ToString();
        }
    }
}
