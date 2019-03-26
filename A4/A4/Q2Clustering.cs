using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A4
{
    public class Q2Clustering : Processor
    {
        public Q2Clustering(string testDataName) : base(testDataName) { }

        public override string Process(string inStr) =>
            TestTools.Process(inStr, (Func<long, long[][], long, double>)Solve);

        private DisjointSet MyDisjointSet { get; set; }
        private List<(int, int, double)> Edges { get; set; }


        public double Solve(long pointCount, long[][] points, long clusterCount)
        {
            MyDisjointSet = new DisjointSet((int)pointCount);
            Edges = new List<(int, int, double)>();

            for (int first = 0; first < pointCount - 1; first++)
            {
                for (int second = first + 1; second < pointCount; second++)
                {
                    double weight = Math.Sqrt(
                                Math.Pow((points[first][0] - points[second][0]), 2)
                                    + Math.Pow((points[first][1] - points[second][1]), 2));
                    
                    Edges.Add(new ValueTuple<int, int, double>
                                (first, second, weight));
                }
            }

            return Clustring(pointCount,clusterCount);
        }

        private double Clustring(long pointCount, long clusterCount)
        {
            Edges.Sort((x, y) => y.Item3.CompareTo(x.Item3));
            Edges.Reverse();

            long setsCount = 0;
            object result = null;

            foreach (var edge in Edges)
            {
                if (MyDisjointSet.Find(edge.Item1) != MyDisjointSet.Find(edge.Item2))
                {
                    setsCount++;
                    MyDisjointSet.Union(edge.Item1, edge.Item2);
                }
                if (setsCount > pointCount - clusterCount)
                {
                    result = (double)(Math.Round(edge.Item3 * 1_000_000) / 1_000_000);
                    break;
                }
            }

            return (double)result;
        }



        //My first solution for Q2
        //private void Clustering(long pointCount, long[][] points, long clusterCount)
        //{

        //    CurrentCentroids = new(long, long)[clusterCount];
        //    PreviousCentroids = new(long, long)[clusterCount];
        //    List<int>[] classes;

        //    for(int centroid = 0; centroid<clusterCount; centroid++)
        //        CurrentCentroids[centroid] = new ValueTuple<long, long>
        //                             (points[centroid][0], points[centroid][1]);



        //    do
        //    {
        //        classes = new List<int>[clusterCount];
        //        for(int point = 0; point < pointCount; point++)
        //        {
        //            double[] distances = new double[clusterCount];
        //            for(int centroid = 0; centroid < clusterCount; centroid++)
        //                distances[centroid] = Math.Sqrt(
        //                     Math.Pow(points[point][0] - CurrentCentroids[centroid].Item1, 2)
        //                            + Math.Pow((points[point][1] - CurrentCentroids[centroid].Item2), 2)
        //                            );
        //            classes[
        //                Array.IndexOf(distances, distances.Min())].Add(point);


        //            PreviousCentroids = CurrentCentroids;
        //            for(int index = 0; index < clusterCount; index++)
        //            {
        //                long xSum = 0;
        //                long ySum = 0;
        //                int count = classes[index].Count;
        //                for (int i = 0; i < count; i++)
        //                {
        //                    xSum += points[classes[index].ElementAt(i)][0];
        //                    ySum += points[classes[index].ElementAt(i)][1];
        //                }

        //                CurrentCentroids[index] = new ValueTuple<long, long>
        //                                ((long)xSum / count, (long)ySum / count);
        //            }
        //        }

        //    } while (true);

        //}
    }
}
