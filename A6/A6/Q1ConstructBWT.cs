using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A6
{
    public class Q1ConstructBWT : Processor
    {
        public Q1ConstructBWT(string testDataName) : base(testDataName)
        {
            //this.ExcludeTestCaseRangeInclusive(11, 50);
        }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<String, String>)Solve);


        private List<string> CyclicRotations { get; set; }

        //private SortedList<string, int> Cycles { get; set; }

        public string Solve(string text)
        {
            int textSize = text.Length;
            CyclicRotations = new List<string>();
            //Cycles = new SortedList<string, int>();
            for (int i = 0; i < textSize; i++)
            {
                StringBuilder newCyclicRotation = new StringBuilder(textSize);
                int index = i;
                for (int j = 0; j < textSize; j++)
                {
                    newCyclicRotation.Insert(j, text[index]);
                    
                    index = (index + 1) % textSize;
                }

                CyclicRotations.Add(newCyclicRotation.ToString());
                //Cycles.Add(newCyclicRotation.ToString(), i);
            }

            return GetBurrowsWheelerTransform(textSize);
        }
        

        private string GetBurrowsWheelerTransform(int textSize)
        {
            CyclicRotations.Sort();
            StringBuilder result = new StringBuilder(textSize);
            for (int i = 0; i < textSize; i++)
                result.Insert(i, CyclicRotations[i][textSize - 1]);
            //result.Insert(i, Cycles.ElementAt(i).Key.Last());


            return result.ToString();
        }
    }
}
