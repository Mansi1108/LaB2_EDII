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
        public int FixedSizeTextLength => 12 + 12 + (12 * Order) + (Order - 1) * (NodeValues[0].FixedSizeTextLength + 1) + 2;
        #endregion

        #region Constructors
        public TreeNode(int order)
        {
            FatherId = -1;
            Order = order;
            NodeValues = new List<T>(Order - 1);
            SubTrees = new List<int>(Order);
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
            return $"{Id:00000000000;-0000000000}|{FatherId:00000000000;-0000000000}|{GetSubTreesAndNodeValuesFixedString()}\r\n";
        }

        private string GetSubTreesAndNodeValuesFixedString()
        {
            string FixedString = "";
            int SubtreesLength = Order;
            for (int i = 0; i < SubTrees.Count; i++)
            {
                if (SubTrees[i] != -1)
                {
                    if (i + 1 < SubTrees.Count)
                    {
                        FixedString += $"{SubTrees[i]:00000000000;-0000000000},";
                        SubtreesLength--;
                    }
                    else
                    {
                        FixedString += $"{SubTrees[i]:00000000000;-0000000000}";
                        SubtreesLength--;
                    }
                }
            }

            for (int i = 0; i < SubtreesLength; i++)
            {
                if (i + 1 < SubtreesLength)
                {
                    FixedString += new string(' ', 11)+",";
                }
                else
                {
                    FixedString += new string(' ', 11);
                }
            }

            FixedString += "|";

            int NodeValuesLength = Order - 1;
            for (int i = 0; i < NodeValues.Count; i++)
            {
                if (i + 1 < NodeValuesLength)
                {
                    FixedString += NodeValues[i].ToFixedSize() + "|";
                    NodeValuesLength--;
                }
                else
                {
                    FixedString += NodeValues[i].ToFixedSize();
                    NodeValuesLength--;
                }
            }

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
                if (linea.Substring(0,11).Trim() != "")
                {
                    Index = Convert.ToInt32(linea.Substring(0, 11));
                    if (Index != 0)
                    {
                        SubTrees.Add(Index);
                    }
                }
                linea = linea.Remove(0, 12);
            }
            T Value = new T();
            for (int i = 0; i < Order - 1; i++)
            {
                if (linea.Substring(0,Value.FixedSizeTextLength).Trim() != "")
                {
                    var stringT = linea.Substring(0, Value.FixedSizeTextLength);
                    Value.GetT(stringT);
                    NodeValues.Add(Value);
                    linea = linea.Remove(0, Value.FixedSizeTextLength + 1);
                }
            }
        }

        public int GetNodeSize()
        {
            T value = new T();
            if (NodeValues.Count == 0)
            {
                return 12 + 12 + (12 * Order) + (Order - 1) * (value.FixedSizeTextLength) + 2;
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
