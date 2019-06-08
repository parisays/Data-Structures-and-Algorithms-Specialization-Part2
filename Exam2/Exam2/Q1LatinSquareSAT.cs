using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestCommon;
using Combinatorics.Collections;

namespace Exam2
{
    public class Q1LatinSquareSAT : Processor
    {
        public Q1LatinSquareSAT(string testDataName) : base(testDataName)
        {
        }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<int,int?[,],string>)Solve);

        public override Action<string, string> Verifier =>
            TestTools.SatVerifier;


        public string Solve(int dim, int?[,] square)
        {
            StringBuilder builder = new StringBuilder();
            int count = 0;

            count+= CellConstraint(dim, ref builder);
            count+= RowConstraint(dim, ref builder);
            count+= ColumnConstraint(dim, ref builder);
            
            return ConstructFormula(dim, square, ref builder, ref count);
        }

        private string ConstructFormula(int dim, int?[,] square,
                        ref StringBuilder builder,ref int count)
        {
            for(int i=0; i<dim; i++)
                for(int j=0; j<dim; j++)
                {
                    if ( square[i,j].HasValue)
                    {
                        count++;
                        builder.Append(Varnum(i, j, square[i, j].Value, dim).ToString() + "\n");
                    }
                }

            return Math.Pow(dim, 3).ToString() + " " + count.ToString() + "\n" + builder.ToString();
        }
        

        private int ColumnConstraint(int dim, ref StringBuilder builder)
        {
            int count = 0;
            for(int j=0; j<dim; j++)
                for(int k=0; k<dim; k++)
                {
                    List<int> literals = new List<int>();
                    for (int i = 0; i < dim; i++)
                        literals.Add(Varnum(i, j, k, dim));

                    count+=ExactlyOneOf(literals, ref builder);
                }

            return count;
        }

        private int RowConstraint(int dim, ref StringBuilder builder)
        {
            int count = 0;
            for(int i=0; i<dim; i++)
                for(int k=0; k<dim; k++)
                {
                    List<int> literals = new List<int>();
                    for (int j = 0; j < dim; j++)
                        literals.Add(Varnum(i, j, k, dim));

                    count+=ExactlyOneOf(literals, ref builder);
                }

            return count;
        }

        private int CellConstraint(int dim, ref StringBuilder builder)
        {
            int count = 0;
            for(int i=0; i<dim; i++)
                for(int j=0; j<dim; j++)
                {
                    List<int> literals = new List<int>();

                    for (int k = 0; k < dim; k++)
                        literals.Add(Varnum(i, j, k, dim));

                    count+=ExactlyOneOf(literals, ref builder);
                        
                }

            return count;
        }

        private int ExactlyOneOf(List<int> literals, ref StringBuilder builder)
        {
            int count = 1;
            builder.Append(literals.Select(x => x.ToString())
                    .Aggregate((s1, s2) => s1 + " " + s2).ToString() + "\n");

            Combinations<int> pairs = new Combinations<int>(literals, 2);
            
            foreach (List<int> pair in pairs)
            {
                builder.Append(pair.Select(x => (-x).ToString())
                    .Aggregate((s1, s2) => s1 + " " + s2 + "\n").ToString()
                   );
                count++;
            }

            return count;
        }

        private int Varnum(int i, int j, int k, int dim) => (i * (dim * dim)) + (j * dim) + k + 1;
    }
}
