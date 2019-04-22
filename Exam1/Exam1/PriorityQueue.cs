using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exam1
{
    public class PriorityQueue
    {
        private long MaxSize { get; set; }
        public int Size { get; set; }
        public Node[] NodesList { get; set; }
        public bool[] Exists { get; set; }


        public PriorityQueue(long maxSize)
        {
            MaxSize = maxSize;
            //Size = 0;
            //NodesList = new Node[maxSize];
            //Exists = Enumerable.Repeat<bool>(false, (int)MaxSize).ToArray();
        }

        
        public void MakeQueue()
        {
            Size = 0;
            NodesList = new Node[MaxSize];
            Exists = Enumerable.Repeat<bool>(false, (int)MaxSize).ToArray();
        }

        public void Insert(Node newNode)
        {
            if(Size < MaxSize)
            {
                Exists[newNode.Key - 1] = true;
                NodesList[Size] = newNode;
                Size++;
                SiftUp(Size - 1);
            }
        }

        public void Remove(int index)
        {
            NodesList[index].Distance = long.MinValue;
            SiftUp(index);
            ExtractMin();
        }

        public Node ExtractMin()
        {
            var result = NodesList[0];
            NodesList[0] = NodesList[Size - 1];
            Exists[result.Key - 1] = false;
            Size--;
            SiftDown(0);
            return result;
        }

        private void SiftUp(int index)
        {
            while(index>0 && Comparer(NodesList[index], NodesList[Parent(index)]))
            {
                // swap nodes in the array
                (NodesList[index], NodesList[Parent(index)]) =
                            (NodesList[Parent(index)], NodesList[index]);
                
                index = Parent(index);
            }
        }

        
        private void SiftDown(int index)
        {
            int swapingIndex = index;
            int leftIndex = LeftChild(index);
            int rightIndex = RightChild(index);

            if (leftIndex < Size && Comparer(NodesList[leftIndex], NodesList[swapingIndex]))
                swapingIndex = leftIndex;

            if (rightIndex < Size && Comparer(NodesList[rightIndex], NodesList[swapingIndex]))
                swapingIndex = rightIndex;
            if(swapingIndex != index)
            {
                // swap Nodes in array
                (NodesList[index], NodesList[swapingIndex]) =
                        (NodesList[swapingIndex], NodesList[index]);
                
                SiftDown(swapingIndex);
            }
        }

        public void ChangePriority(int index, long newPriority)
        {
            long oldPriority = NodesList[index].Distance;
            NodesList[index].Distance = newPriority;

            if (newPriority < oldPriority)
                SiftUp(index);
            else
                SiftDown(index);
        }

        private int Parent(int index) => (index - 1) / 2;
        private int LeftChild(int index) => index * 2 + 1;
        private int RightChild(int index) => index * 2 + 2;

        private bool Comparer(Node node1, Node node2)
        {
            if (node1.Distance == node2.Distance)
                return node1.Key > node2.Key;
            else
                return node1.Distance < node2.Distance;
        }
    }

    
}
