using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestCommon;

namespace Exam2
{
    public class Q1LatinSquareSAT : Processor
    {
        public Q1LatinSquareSAT(string testDataName) : base(testDataName)
        {}

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<int,int?[,],string>)Solve);

        public override Action<string, string> Verifier =>
            TestTools.SatVerifier;


        public string Solve(int dim, int?[,] square)
        {
            GetClauses(dim, square);
            throw new NotImplementedException();
        }

        private void GetClauses(int dim, int?[,] square)
        {
            List<string> clauses = new List<string>() { "0 0" };
            List<int> variables = new List<int>();
            for (int k = 0; k < dim; k++)
                variables.Add(k);

            CellConstraint(dim, square, ref clauses);
            throw new NotImplementedException();
        }

        private void CellConstraint(int dim, int?[,] square, ref List<string> clauses)
        {
            
            for(int i=0; i<dim; i++)
                for(int j=0; j<dim; j++)
                {

                }
            throw new NotImplementedException();
        }
    }
}
