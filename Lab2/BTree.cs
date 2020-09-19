using CustomGenerics;
using System;

namespace ClassLibrary1
{
    public class BTree<T> where T : IComparable
    {
        private int TreeOrder;
        private TreeNode<T> Root;

        public BTree(int order)
        {
            TreeOrder = order;
        }

        public void AddValue(T value)
        {
            if (Root != null)
            {
                Root = new TreeNode<T>(value, TreeOrder);
            }
            else
            {
                Insert(value, Root);
            }
        }

        private void Insert(T value, TreeNode<T> node)
        {
            if (node.SubTrees.Count != 0)
            {
                for (int i = 0; i < node.NodeValues.Count; i++)
                {
                    if (value.CompareTo(node.NodeValues[i]) < 0)
                    {
                        if (i == 0)
                        {
                            //Insertar en el árbol de hasta la izquierda
                            Insert(value, node.SubTrees[i]);
                            i = node.NodeValues.Count;
                        }
                    }
                    else if (value.CompareTo(node.NodeValues[i]) > 0)
                    {
                        if (i == node.NodeValues.Count - 1)
                        {
                            //Insertar en el árbol de hasta la derecha
                            Insert(value, node.SubTrees[i + 1]);
                            i = node.NodeValues.Count;
                        }
                        else
                        {
                            if (value.CompareTo(node.NodeValues[i + 1]) < 0)
                            {
                                //Insertar en el i+1
                                Insert(value, node.SubTrees[i + 1]);
                                i = node.NodeValues.Count;
                            }
                        }
                    }
                }
            }
            else
            {
                AddToNode(value, node);
            }
        }

        private void AddToNode(T value, TreeNode<T> node)
        {
            node.NodeValues.Add(value);
            node.NodeValues.Sort();
            if (node.NeedsSeparation())
            {
                int StartIndex;
                if (TreeOrder % 2 == 0)
                {
                    StartIndex = TreeOrder / 2;
                }
                else
                {
                    StartIndex = TreeOrder / 2 + 1;
                }

                // Pasar los valores y subárboles correspondientes al nuevo nodo
                TreeNode<T> NewNode = new TreeNode<T>();
                for (int i = StartIndex; i < node.NodeValues.Count; i++)
                {
                    NewNode.NodeValues.Add(node.NodeValues[i]);
                    NewNode.SubTrees.Add(node.SubTrees[i]);
                    node.NodeValues.RemoveAt(i);
                    node.SubTrees.RemoveAt(i);
                }

                // Si no existe el padre, lo crea y envía el valor medio. De lo contrario,
                // 
                if (node.Father == null)
                {
                    node.Father = new TreeNode<T>(node.NodeValues[StartIndex - 1], TreeOrder);
                    node.Father.SubTrees.Add(node);
                    node.Father.SubTrees.Add(NewNode);
                    Root = node.Father;
                    NewNode.Father = node.Father;
                }
                else
                {
                    node.Father.NodeValues.Add(node.NodeValues[StartIndex - 1]);
                    // Insertar el nuevo nodo al lado del nodo anterior
                    for (int i = 0; i < node.Father.SubTrees.Count; i++)
                    {
                        if (node.Father.SubTrees[i] == node)
                        {
                            node.Father.SubTrees.Insert(i + 1, NewNode);
                        }
                    }
                    NewNode.Father = node.Father;
                    // Aquí debemos verificar recursivamente si el nodo padre necesita separarse
                    AddToNode(node.NodeValues[StartIndex - 1], node.Father);
                    // No hay que preocuparse porque quede en la posición correcta porque el .sort() 
                    // se hace cargo de eso
                }
            }
        }
    }
}
