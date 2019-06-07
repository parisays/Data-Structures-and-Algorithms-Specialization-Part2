using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A11
{
    public class Node
    {
        public int PreOrder { get; set; }
        public int PostOrder { get; set; }
        public int Key { get; set; }
        public bool Marked { get; set; }
        public bool Visited { get; set; }
        public int Component { get; set; }
        public Node(int key)
        {
            Key = key;
            PostOrder = 0;
            PreOrder = 0;
            Marked = false;
            Visited = false;
            Component = 0;
        }
    }
}
