using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A10
{
    public class Q3AdBudgetAllocation : Processor
    {
        public Q3AdBudgetAllocation(string testDataName) 
            : base(testDataName)
        {
            //this.ExcludeTestCaseRangeInclusive(2, 45);
        }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<long, long, long[][], long[], string[]>)Solve);

        public string[] Solve(long eqCount, long varCount, long[][] A, long[] b)
        {
            int clauseCount = 0;
            string firstLine = null;
            List<string> clauses = new List<string>
            {
                firstLine
            };

            for (int row = 0; row < eqCount; row++)
            {
                int nonZeroCo = A[row].Where(x => x != 0).Count();
                int allCombinations = (int)Math.Pow(2, nonZeroCo);

                for (int i = 0; i < allCombinations; i++)
                {
                    var currentCombination = new BitArray(BitConverter.GetBytes(i));

                    long sum = 0;
                    int bitIndex = 0;
                    for (int index = 0; index < varCount; index++)
                        if (A[row][index] != 0 && bitIndex < currentCombination.Length && currentCombination[bitIndex++])
                            sum += A[row][index];

                    if(sum > b[row])
                    {
                        bool isClause = false;
                        StringBuilder currentClause = new StringBuilder();

                        bitIndex = 0;
                        for (int variable = 0; variable < varCount; variable++)
                            if (A[row][variable] != 0)
                            {
                                currentClause.Append((bitIndex < currentCombination.Length && currentCombination[bitIndex++]) ?
                                    (-(variable + 1)).ToString() + " " :
                                    (variable + 1).ToString() + " "
                                    );

                                isClause = true;
                            }

                        if(isClause)
                        {
                            clauseCount++;
                            currentClause.Append("0 \n");
                            clauses.Add(currentClause.ToString());
                        }
                                
                    }
                }
                
            }

            if (clauseCount == 0)
            {
                clauseCount++;
                varCount = 1;
                clauses.Add("1 -1 0");
            }

            firstLine = clauseCount + " " + varCount + " 0 \n";
            clauses[0] = firstLine;

            return clauses.ToArray();
        }

        public override Action<string, string> Verifier { get; set; } =
            TestTools.SatVerifier;
    }
}
