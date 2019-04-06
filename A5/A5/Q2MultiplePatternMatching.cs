using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCommon;

namespace A5
{
    public class Q2MultiplePatternMatching : Processor
    {
        public Q2MultiplePatternMatching(string testDataName) : base(testDataName)
        {
        }

        private Point Root { get; set; }

        public override string Process(string inStr) =>
        TestTools.Process(inStr, (Func<String, long, String[], long[]>)Solve);

        public long[] Solve(string text, long n, string[] patterns)
        {
            //BuildTrie(text);
            //long[] result = SearchForPatterns(n, patterns);
            //return result;

            BuildPatternsTrie(patterns);
            return SearchForIndxes(text);
        }

        private long[] SearchForIndxes(string text)
        {
            List<long> result = new List<long>();
            int textSize = text.Length;
            for(int i=0; i<textSize; i++)
            {
                var current = Root;
                for(int j=i; j<textSize; j++)
                {
                    var point = current.Children.Find(x => x.Label == text[j]);
                    if (point == null)
                        break;
                    else
                    {
                        if (point.Children.Exists(c => c.Label == '$'))
                            result.Add(i);

                        current = point;
                    }
                }
            }

            if (result.Count == 0)
                result.Add(-1);

            else
            {
                result.Sort();
                result = result.Distinct().ToList();
            }

            return result.ToArray();
        }

        private void BuildPatternsTrie(string[] patterns)
        {
            Root = new Point('\0');
            foreach(string pattern in patterns)
            {
                var current = Root;

                int patternSize = pattern.Length;
                for (int i=0; i < patternSize; i++)
                {
                    var point = current.Children.Find(x => x.Label == pattern[i]);

                    if (point != null)
                    {
                        current = point;
                    }
                    else
                    {
                        Point newPoint = new Point(pattern[i]);
                        current.Children.Add(newPoint);
                        current = newPoint;
                    }
                }

                current.Children.Add(new Point('$'));
            }

        }

        //private long[] SearchForPatterns(long n, string[] patterns)
        //{
        //    List<long> result = new List<long>();

        //    foreach (string pattern in patterns)
        //    {
        //        var current = Root;
        //        int patternSize = pattern.Length;
        //        bool match = false;
        //        for (int i = 0; i < patternSize; i++)
        //        {
        //            Point point = null;
        //            foreach(var child in current.Children)
        //            {
        //                if(child.Label == pattern[i])
        //                {
        //                    point = child;
        //                    break;
        //                }
        //            }

        //            //var point = current.Children.Find(x => x.Label == pattern[i]);

        //            if (point == null)
        //            {
        //                match = false;
        //                break;
        //            }
        //            else
        //            {
        //                current = point;
        //                match = true;
        //            }

        //        }

        //        if (match)
        //            result.AddRange(FindSources(current));

        //    }

        //    if (result.Count == 0)
        //        result.Add(-1);

        //    else
        //    {

        //        result.Sort();
        //        result = result.Distinct().ToList();
        //    }

        //    return result.ToArray();
        //}

        //private List<long> FindSources(Point current)
        //{
        //    List<long> result = new List<long>();
        //    foreach (var child in current.Children)
        //    {
        //        if (child.Label == '$')
        //            result.Add(child.Source);
        //        else
        //            result.AddRange(FindSources(child));
        //    }

        //    return result;
        //}

        //private void BuildTrie(string text)
        //{
        //    Root = new Point('\0', 0);
        //    //int TrieCount = 1;
        //    int textSize = text.Length;

        //    for (int i = 0; i < textSize; i++)
        //    {
        //        var current = Root;
        //        for (int j = i; j < textSize; j++)
        //        {
        //            var point = current.Children.Find(x => x.Label == text[j]);

        //            if (CheckExisting(point))
        //            {
        //                current = point;
        //            }
        //            else
        //            {
        //                Point newPoint = new Point(text[j], i);
        //                current.Children.Add(newPoint);
        //                //TrieCount++;
        //                current = newPoint;
        //            }
        //        }

        //        current.Children.Add( new Point('$', i) );

        //    }

        //}


        //private bool CheckExisting(Point edge) => edge != null;
    }

    public class Point
    {
        public char Label { get; set; }
        public List<Point> Children { get; set; }
        
        public Point(char label)
        {
            Label = label;
            Children = new List<Point>();
        }
    }
}
