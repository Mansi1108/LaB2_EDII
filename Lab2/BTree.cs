using CustomGenerics;
using CustomGenerics.Utilies;
using System;
using System.IO;
using System.Text;

namespace ClassLibrary1
{
    public class BTree<T> where T : IComparable, IFixedSizeText
    {
        string FilePath;
        string FileName = "BTree.txt";
        FileStream File;
        int TreeOrder;
        int RootId;
        int NodeQty;
        int MetadataLength = 18;

        public BTree(string path, int order)
        {
            FilePath = path;
            TreeOrder = order;
            File = new FileStream($"{FilePath}/{FileName}", FileMode.OpenOrCreate);

            if (File.Length == 0)
            {
                RootId = 1;
                NodeQty = 0;
            }
        }

        public void AddValue(T value)
        {
            Insert(value, RootId);
        }

        private async void Insert(T value, int nodeId)
        {
            int bufferLength = 1024;
            byte[] buffer;
            TreeNode<T> CurrentNode = new TreeNode<T>(default, TreeOrder);
            if (NodeQty == 0)
            {
                TreeNode<T> NewNode = new TreeNode<T>(value, TreeOrder);
                using var writer = new StreamWriter(File, Encoding.ASCII);
                await writer.WriteAsync($"{RootId},{NodeQty},{TreeOrder}\r\n{NewNode.ToFixedSize()}");
            }
            else
            {
                buffer = new byte[bufferLength];
                File.Seek((RootId - 1) * value.FixedSizeTextLength, SeekOrigin.Begin);
                File.Read(buffer, 0, value.FixedSizeTextLength);
                var valueString = ByteGenerator.ConvertToString(buffer);
                CurrentNode.GetT(valueString);
                // Esto devuelve un T :<
                //raiz = new T();
            }
        }

        private void WriteNewNode(TreeNode<T> node)
        {
            
        }

        #region PrevInsert



        //private void Insert(T value, TreeNode<T> node)
        //{
        //    if (node.SubTrees.Count != 0)
        //    {
        //        for (int i = 0; i < node.NodeValues.Count; i++)
        //        {
        //            if (value.CompareTo(node.NodeValues[i]) < 0)
        //            {
        //                if (i == 0)
        //                {
        //                    //Insertar en el árbol de hasta la izquierda
        //                    Insert(value, node.SubTrees[i]);
        //                    i = node.NodeValues.Count;
        //                }
        //            }
        //            else if (value.CompareTo(node.NodeValues[i]) > 0)
        //            {
        //                if (i == node.NodeValues.Count - 1)
        //                {
        //                    //Insertar en el árbol de hasta la derecha
        //                    Insert(value, node.SubTrees[i + 1]);
        //                    i = node.NodeValues.Count;
        //                }
        //                else
        //                {
        //                    if (value.CompareTo(node.NodeValues[i + 1]) < 0)
        //                    {
        //                        //Insertar en el i+1
        //                        Insert(value, node.SubTrees[i + 1]);
        //                        i = node.NodeValues.Count;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        AddToNode(value, node);
        //    }
        //}

        //private void AddToNode(T value, TreeNode<T> node)
        //{
        //    node.NodeValues.Add(value);
        //    node.NodeValues.Sort();
        //    if (node.NeedsSeparation())
        //    {
        //        int StartIndex;
        //        if (TreeOrder % 2 == 0)
        //        {
        //            StartIndex = TreeOrder / 2;
        //        }
        //        else
        //        {
        //            StartIndex = TreeOrder / 2 + 1;
        //        }

        //        // Pasar los valores y subárboles correspondientes al nuevo nodo
        //        TreeNode<T> NewNode = new TreeNode<T>();
        //        for (int i = StartIndex; i < node.NodeValues.Count; i++)
        //        {
        //            NewNode.NodeValues.Add(node.NodeValues[i]);
        //            NewNode.SubTrees.Add(node.SubTrees[i]);
        //            node.NodeValues.RemoveAt(i);
        //            node.SubTrees.RemoveAt(i);
        //        }

        //        // Si no existe el padre, lo crea y envía el valor medio. De lo contrario,
        //        // 
        //        if (node.Father == null)
        //        {
        //            node.Father = new TreeNode<T>(node.NodeValues[StartIndex - 1], TreeOrder);
        //            node.Father.SubTrees.Add(node);
        //            node.Father.SubTrees.Add(NewNode);
        //            Root = node.Father;
        //            NewNode.Father = node.Father;
        //        }
        //        else
        //        {
        //            node.Father.NodeValues.Add(node.NodeValues[StartIndex - 1]);
        //            // Insertar el nuevo nodo al lado del nodo anterior
        //            for (int i = 0; i < node.Father.SubTrees.Count; i++)
        //            {
        //                if (node.Father.SubTrees[i] == node)
        //                {
        //                    node.Father.SubTrees.Insert(i + 1, NewNode);
        //                }
        //            }
        //            NewNode.Father = node.Father;
        //            // Aquí debemos verificar recursivamente si el nodo padre necesita separarse
        //            AddToNode(node.NodeValues[StartIndex - 1], node.Father);
        //            // No hay que preocuparse porque quede en la posición correcta porque el .sort() 
        //            // se hace cargo de eso
        //        }
        //    }
        //}
        #endregion
    }
}
