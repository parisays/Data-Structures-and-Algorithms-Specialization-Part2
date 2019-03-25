using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A3
{
    public class MinHeap
    {
        public List<Node> Nodes;
        public int Size { get; set; }
        public int MaxSize { get; set; }

        public MinHeap(List<Node> nodes)
        {
            this.Nodes = nodes.OrderBy(n => n.Key).ToList();
            this.MaxSize = nodes.Count;
            this.Size = this.MaxSize;
        }

        
        public Node ExtractMin()
        {
            (Nodes[0], Nodes[Size - 1]) = (Nodes[Size - 1], Nodes[0]);
            Size--;
            SiftDown(0);
            return Nodes.ElementAt(Size);
        }

        public void ChangePriority(int index, long priority)
        {
            long oldPriority = Nodes.ElementAt(index).Distance;
            Nodes[index].Distance = priority;

            if (priority <= oldPriority)
                SiftUp(index);
            else
                SiftDown(index);

        }

        private void SiftDown(int index)
        {
            int minIndex = index;
            int leftIndex = LeftChild(index);
            int rightIndex = RightChild(index);

            if (leftIndex < Size && CompareNodes(
                                        Nodes.ElementAt(leftIndex), Nodes.ElementAt(minIndex)))
                minIndex = leftIndex;

            if (rightIndex < Size && CompareNodes(
                                        Nodes.ElementAt(rightIndex), Nodes.ElementAt(minIndex)))
                minIndex = rightIndex;

            if (minIndex != index)
            {
                (Nodes[index], Nodes[minIndex]) =
                                            (Nodes[minIndex], Nodes[index]);
                SiftDown(minIndex);
            }

        }


        private void SiftUp(int index)
        {
            while (index > 0 && CompareNodes(Nodes.ElementAt(index),
                                                  Nodes.ElementAt(Parent(index))))
            {
                (Nodes[index], Nodes[Parent(index)]) =
                                                (Nodes[Parent(index)], Nodes[index]);
                index = Parent(index);
            }
        }

        private int Parent(int i) => (i - 1) / 2;

        private int RightChild(int index) => 2 * index + 2;

        private int LeftChild(int index) => 2 * index + 1;
        
        private bool CompareNodes(Node node1, Node node2)
        {
            if (node1.Distance == node2.Distance)
                return node1.Key < node2.Key;
            else
                return node1.Distance < node2.Distance;
        }
    }
}
