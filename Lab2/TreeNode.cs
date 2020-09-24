using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace CustomGenerics
{
    internal class TreeNode<T> : IFixedSizeText where T : IComparable, IFixedSizeText
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
        public TreeNode(int order)
        {
            Order = order;
            NodeValues = new List<T>(Order - 1);
            SubTrees = new List<int>(Order);
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
            return NodeValues.Count == Order;
        }

        public void AddValue(T value)
        {
            NodeValues.Add(value);
            NodeValues.Sort();
        }

        public string ToFixedSize()
        {
            return $"{Id:00000000000;-0000000000}{FatherId:00000000000;-0000000000}{string.Join("",SubTrees)}{string.Join("",NodeValues)}";
        }

        public void GetT(string linea)
        {
            Id = Convert.ToInt32(linea.Substring(0, 11));
            linea.Remove(0, 11);
            FatherId = Convert.ToInt32(linea.Substring(0, 11));
            linea.Remove(0, 11);
            int Index;
            for (int i = 0; i < Order; i++)
            {
                Index = Convert.ToInt32(linea.Substring(0, 11));
                if (Index != 0)
                {
                    SubTrees.Add(Index);
                }
                linea.Remove(0, 11);
            }
            int TLength = linea.Length / Order - 1;
            T Value = default;
            for (int i = 0; i < Order - 1; i++)
            {
                Value.GetT(linea.Substring(0, TLength));
                if (Value.CompareTo(default) != 0)
                {
                    NodeValues.Add(Value);
                }
                linea.Remove(0, TLength);
            }
        }

        public int GetNodeSize()
        {
            if (NodeValues.Count == 0)
            {
                NodeValues.Add(default);
                int Size = FixedSizeTextLength;
                NodeValues.RemoveRange(0, 1);
                return Size;
            }
            else
            {
                return FixedSizeTextLength;
            }
        }
    }
}
