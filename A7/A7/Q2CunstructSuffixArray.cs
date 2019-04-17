using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A7
{
    public class Q2CunstructSuffixArray : Processor
    {
        public Q2CunstructSuffixArray(string testDataName) : base(testDataName)
        {
        }

        public override string Process(string inStr) =>
        TestTools.Process(inStr, (Func<String, long[]>)Solve);

        private long[] Solve(string text)
        {
            int length = 1;
            int textSize = text.Length;

            long[] order = SortCharacters(text, textSize);
            int[] @class = ComputeCharClasses(text, textSize, order);

            while(length < textSize)
            {
                order = SortDoubled(textSize, length, order, @class);
                @class = UpdateClasses(order, @class, length, textSize);
                length *= 2;
            }
            
            return order;
        }

        private int[] UpdateClasses(long[] order, int[] @class, int length, int textSize)
        {
            int[] newClass = new int[textSize];
            newClass[order[0]] = 0;

            for(int i=1; i<textSize; i++)
            {
                int current = (int)order[i];
                int previous = (int)order[i - 1];
                int mid = (current + length) % textSize;
                int midPrevious = (previous + length) % textSize;

                if (@class[current] != @class[previous] || @class[mid] != @class[midPrevious])
                    newClass[current] = newClass[previous] + 1;
                else
                    newClass[current] = newClass[previous];
            }

            return newClass;
        }

        private long[] SortDoubled(int textSize, int length, long[] order, int[] @class)
        {
            int[] count = new int[textSize];
            long[] newOrder = new long[textSize];

            for (int i = 0; i < textSize; i++)
                count[@class[i]]++;
            for (int j = 1; j < textSize; j++)
                count[j] = count[j] + count[j - 1];

            for(int i = textSize-1; i>=0; i--)
            {
                int start = ((int)order[i] - length + textSize) 
                                    % textSize;
                
                int cl = @class[start];
                count[cl]--;
                newOrder[count[cl]] = start;
            }

            return newOrder;
            
        }

        private int[] ComputeCharClasses(string text, int textSize, long[] order)
        {
            int[] @class = new int[textSize];
            @class[order[0]] = 0;
            
            for(int i=1; i<textSize; i++)
            {
                if (text[(int)order[i]] != text[(int)order[i - 1]])
                    @class[(int)order[i]] = @class[(int)order[i - 1]] + 1;
                else
                    @class[(int)order[i]] = @class[(int)order[i - 1]];
            }

            return @class;
        }

        private long[] SortCharacters(string text, int textSize)
        {
            long[] order = new long[textSize];

            Dictionary<char, int> count = new Dictionary<char, int>();
            char[] alphabet = text.Distinct().ToArray();
            Array.Sort(alphabet);

            int alphabetCount = alphabet.Count();

            foreach (char c in alphabet)
                count.Add(c, 0);

            for (int i = 0; i < textSize; i++)
                count[text[i]]++;

            
            for (int j = 1; j < alphabetCount; j++)
                count[alphabet[j]] = count[alphabet[j]] + count[alphabet[j - 1]];

            for(int i = textSize - 1; i>=0; i--)
            {
                char current = text[i];
                count[current]--;
                order[count[current]] = i;
            }

            return order;
        }
    }
}
