using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A4
{
    public class DisjointSet
    {
        private int[] Parent { get; set; }
        private int[] Rank { get; set; }
        public DisjointSet(int count)
        {
            Parent = new int[count];
            Rank = new int[count];

            for (int index = 0; index < count; index++)
                MakeSet(index);

        }
        
        public void MakeSet(int index)
        {
            Parent[index] = index;
            Rank[index] = 0;
        }

        public int Find(int index)
        {
            if (index != Parent[index])
                Parent[index] = Find(Parent[index]);

            return Parent[index];
        }

        public void Union(int first, int second)
        {
            int firstID = Find(first);
            int secondID = Find(second);

            if (firstID == secondID)
                return;

            if (Rank[firstID] > Rank[secondID])
                Parent[secondID] = firstID;
            else
            {
                Parent[firstID] = secondID;
                if (Rank[secondID] == Rank[firstID])
                    Rank[secondID]++;
            }

        }
    }
}
