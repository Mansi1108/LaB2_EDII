using System;
using System.Collections.Generic;
using System.Text;

namespace CustomGenerics
{
    class TreeNode<T> where T : IComparable
    {
        public int Order;
        public List<T> NodeValues;
        public TreeNode<T> Father;
        public List<TreeNode<T>> SubTrees;

        public TreeNode()
        {

        }

        public TreeNode(T value, int order)
        {
            NodeValues.Add(value);
            this.Order = order;
        }

        public bool NeedsSeparation()
        {
            return NodeValues.Count == Order;//Verify if this function is needed
        }

        public void OrderNode()
        {
            NodeValues.Sort();
        }
    }
}
