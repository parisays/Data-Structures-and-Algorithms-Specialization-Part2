using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A4
{
    public class Q1BuildingRoads : Processor
    {
        public Q1BuildingRoads(string testDataName) : base(testDataName) { }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<long, long[][], double>)Solve);

        private DisjointSet MyDisjointSet { get; set; }
        private List<(int, int, double)> Edges { get; set; }

        public double Solve(long pointCount, long[][] points)
        {
            MyDisjointSet = new DisjointSet((int)pointCount);
            Edges = new List<(int, int, double)>();
            
            for(int first = 0; first < pointCount - 1; first++)
            {
                for(int second = first + 1; second < pointCount; second++)
                {
                    double weight = Math.Sqrt(
                                Math.Pow((points[first][0] - points[second][0]), 2)
                                    + Math.Pow((points[first][1] - points[second][1]), 2));

                    ValueTuple<int, int, double>edge = new ValueTuple<int, int, double>
                                (first, second, weight);

                    Edges.Add(edge);
                }
            }

            return Kruskal();
                
        }

        private double Kruskal()
        {
            List<double> minST = new List<double>();
            //Edges = Edges.OrderBy(e => e.Item3).ToList();
            Edges.Sort((x, y) => y.Item3.CompareTo(x.Item3));
            Edges.Reverse();

            foreach (var edge in Edges)
            {
                if( MyDisjointSet.Find(edge.Item1) != MyDisjointSet.Find(edge.Item2) )
                {
                    minST.Add(edge.Item3);
                    MyDisjointSet.Union(edge.Item1, edge.Item2);
                }
            }

            return (double)Math.Round(((minST.Sum() * 1_000_000)))/ 1_000_000;
        }
    }

    //public class Point
    //{
    //    public long X;
    //    public long Y;
    //    public Point Parent;

    //    public Point(long x, long y)
    //    {
    //        this.X = x;
    //        this.Y = y;
    //        this.Parent = this;
    //    }
    //}
}
