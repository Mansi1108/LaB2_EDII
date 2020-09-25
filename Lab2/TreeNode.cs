using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.IO;
using CustomGenerics.Utilies;

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
        public int FixedSizeTextLength => 10 + 10 + (10 * Order) + (Order - 1) * NodeValues[0].FixedSizeTextLength + 2;
        #endregion

        #region Constructors
        public TreeNode(int order)
        {
            Order = order;
            NodeValues = new List<T>(Order - 1);
            SubTrees = new List<int>(Order);
        }

        public TreeNode(int order, int id)
        {
            Id = id;
            Order = order;
            NodeValues = new List<T>(Order - 1);
            SubTrees = new List<int>(Order);
        }

        public TreeNode(T value, int order, int id)
        {
            Id = id;
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

        public void RemoveValue(T value)
        {
            foreach (var item in NodeValues)
            {
                if (item.CompareTo(value) == 0)
                {
                    NodeValues.Remove(item);
                }
            }
        }

        #region FixedString
        public string ToFixedSize()
        {
            return $"{Id:00000000000;-0000000000}{FatherId:00000000000;-0000000000}{GetSubTreesAndNodeValuesFixedString()}\r\n";
        }

        private string GetSubTreesAndNodeValuesFixedString()
        {
            string FixedString = "";
            int SubtreesLength = Order;
            for (int i = 0; i < SubTrees.Count; i++)
            {
                if (SubTrees[i] != -1)
                {
                    FixedString += $"{SubTrees[i]:00000000000;-0000000000}";
                    SubtreesLength--;
                }
            }

            for (int i = 0; i < Order - SubtreesLength; i++)
            {
                FixedString += new string(' ', 11);
            }

            int NodevaluesLength = Order - 1;
            for (int i = 0; i < NodeValues.Count; i++)
            {
                if (NodeValues[i].CompareTo(default) != 0)
                {
                    FixedString += NodeValues[i].ToFixedSize();
                    NodevaluesLength--;
                }
            }

            T value = default;

            for (int i = 0; i < NodevaluesLength; i++)
            {
                FixedString += new string(' ', value.FixedSizeTextLength);
            }

            return FixedString;
        }
        #endregion

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
            T Value;
            for (int i = 0; i < Order - 1; i++)
            {
                Value = default;
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
        public bool UnderFlow()
        {
            if (NodeValues.Count - 1 < (Order / 2) - 1)
            {
                return true;
            }
            else
            {
            return false;
            }
        }

        public void DeathNode() //Referencia del nombre del método: https://www.youtube.com/watch?v=ATUAmQ1QEKk
        {
            NodeValues.Clear();
            SubTrees.Clear();
            FatherId = -1;
        }

    }
}
