using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.IO;
using CustomGenerics.Utilies;

namespace CustomGenerics
{
    internal class TreeNode<T> : IFixedSizeText where T : IComparable, IFixedSizeText, new()
    {
        #region Variables
        public int Order;
        public int Id;
        public int FatherId;
        public List<int> SubTrees;
        public List<T> NodeValues;
        public int FixedSizeTextLength => 12 + 12 + (12 * Order) + (Order - 1) * (NodeValues[0].FixedSizeTextLength + 1) + 1;
        #endregion

        #region Constructors
        public TreeNode(int order)
        {
            FatherId = -1;
            Order = order;
            NodeValues = new List<T>(Order - 1);
            SubTrees = new List<int>(Order);
        }
        public TreeNode()
        {
        }

        public TreeNode(int order, int id)
        {
            FatherId = -1;
            Id = id;
            Order = order;
            NodeValues = new List<T>(Order - 1);
            SubTrees = new List<int>(Order);
        }

        public TreeNode(T value, int order, int id)
        {
            FatherId = -1;
            Id = id;
            Order = order;
            NodeValues = new List<T>(Order - 1);
            FillWithNull();
            NodeValues.Add(value);
        }
        #endregion

        private void FillWithNull()
        {
            SubTrees = new List<int>(Order);
            for (int i = 0; i < Order; i++)
            {
                SubTrees.Add(-1);
            }
        }

        public bool AllSubtreesNull()
        {
            foreach (var item in SubTrees)
            {
                if (item != -1)
                {
                    return false;
                }
            }
            return true;
        }

        public void SetSubtreesNull(int StartIndex)
        {
            for (int i = StartIndex; i < SubTrees.Count; i++)
            {
                SubTrees[i] = -1;
            }
        }

        public bool NeedsSeparation()
        {
            return NodeValues.Count == Order;
        }

        public void AddValue(T value)
        {
            NodeValues.Add(value);
            NodeValues.Sort();
        }

        public void AddSubTree(int subtree)
        {
            for (int i = 0; i < SubTrees.Count; i++)
            {
                if (SubTrees[i] == -1)
                {
                    SubTrees[i] = subtree;
                    i = SubTrees.Count;
                }
            }
        }

        public bool RemoveValue(T value)
        {
            foreach (var item in NodeValues)
            {
                if (item.CompareTo(value) == 0)
                {
                    NodeValues.Remove(item);
                    return true;
                }
            }
            return false;
        }

        #region FixedString
        public string ToFixedSize()
        {
            return $"{Id:00000000000;-0000000000}|{FatherId:00000000000;-0000000000}|{GetSubTreesAndNodeValuesFixedString()}\r\n";
        }

        private string GetSubTreesAndNodeValuesFixedString()
        {
            string FixedString = "";
            for (int i = 0; i < Order; i++)
            {
                if (i < SubTrees.Count)
                {
                    if (i + 1 < Order)
                    {
                        FixedString += $"{SubTrees[i]:00000000000;-0000000000},";
                    }
                    else
                    {
                        FixedString += $"{SubTrees[i]:00000000000;-0000000000}";
                    }
                }
                else
                {
                    if (i + 1 < Order)
                    {
                        FixedString += $"{-1:00000000000;-0000000000},";
                    }
                    else
                    {
                        FixedString += $"{-1:00000000000;-0000000000}";
                    }
                }
            }

            FixedString += "|";

            int NodeValuesLength = Order - 1;
            for (int i = 0; i < NodeValues.Count; i++)
            {
                if (i + 1 < NodeValuesLength)
                {
                    FixedString += NodeValues[i].ToFixedSize() + "|";
                }
                else
                {
                    FixedString += NodeValues[i].ToFixedSize();
                }
            }
            NodeValuesLength -= NodeValues.Count;

            int Tlength = NodeValues[0].FixedSizeTextLength;
            for (int i = 0; i < NodeValuesLength; i++)
            {
                if (i + 1 < NodeValuesLength)
                {
                    FixedString += new string(' ', Tlength) + "|";
                }
                else
                {
                    FixedString += new string(' ', Tlength);
                }
            }

            return FixedString;
        }
        #endregion

        public void GetT(string linea)
        {
            Id = Convert.ToInt32(linea.Substring(0, 11));
            linea = linea.Remove(0, 12);
            FatherId = Convert.ToInt32(linea.Substring(0, 11));
            linea = linea.Remove(0, 12);
            int Index;
            for (int i = 0; i < Order; i++)
            {
                if (linea.Substring(0, 11).Trim() != "")
                {
                    Index = Convert.ToInt32(linea.Substring(0, 11));
                    if (Index != 0)
                    {
                        SubTrees.Add(Index);
                    }
                }
                else
                {
                    SubTrees.Add(-1);
                }
                linea = linea.Remove(0, 12);
            }
            T Value = new T();
            for (int i = 0; i < Order - 1; i++)//4
            {
                if (linea.Substring(0, Value.FixedSizeTextLength).Trim() != "")
                {
                    var stringT = linea.Substring(0, Value.FixedSizeTextLength);
                    Value.GetT(stringT);
                    NodeValues.Add(Value);
                    Value = new T();
                }
                linea = linea.Remove(0, Value.FixedSizeTextLength + 1);
            }
        }

        public int GetNodeSize()
        {
            T value = new T();
            if (NodeValues.Count == 0)
            {
                return 12 + 12 + (12 * Order) + (Order - 1) * (value.FixedSizeTextLength + 1) + 1;
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
