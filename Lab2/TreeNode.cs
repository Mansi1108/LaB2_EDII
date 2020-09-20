using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace CustomGenerics
{
    internal class TreeNode<T> : IFixedSizeText<T> where T : IComparable, IFixedSizeText<T>
    {
        #region Variables
        public int Order;
        public int Id;
        public int FatherId;
        public List<int> SubTrees;
        public List<T> NodeValues;
        public int FixedSizeTextLength => 10 + 10 + (10 * Order) + (Order - 1) * NodeValues[0].FixedSizeTextLength;
        #endregion

        #region Constructors
        public TreeNode()
        {

        }

        public TreeNode(T value, int order)
        {
            Order = order;
            NodeValues = new List<T>(Order - 1);
            SubTrees = new List<int>(Order);
            NodeValues.Add(value);
        }
        #endregion

        public bool NeedsSeparation()
        {
            return NodeValues.Count == Order;//Verify if this function is needed
        }

        public void OrderNode()
        {
            NodeValues.Sort();
        }

        public string ToFixedSize()
        {
            return $"{Id:00000000000;-0000000000}{FatherId:00000000000;-0000000000}{string.Join("",SubTrees)}{string.Join("",NodeValues)}";
        }

        public T GetT(string linea)
        {
            throw new NotImplementedException();
        }
    }
}
