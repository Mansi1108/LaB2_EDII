using CustomGenerics;
using CustomGenerics.Utilies;
using System;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ClassLibrary1
{
    public class BTree<T> where T : IComparable, IFixedSizeText
    {
        #region Variables
        string FilePath;
        string FileName = "BTree.txt";
        FileStream File;
        int TreeOrder;
        int RootId;
        int NextNodeId;
        int MetadataLength = 18;//Hacer un método para obtener su valor
        #endregion

        public BTree(string path, int order)
        {
            FilePath = path;
            TreeOrder = order;
            File = new FileStream($"{FilePath}/{FileName}", FileMode.OpenOrCreate);

            if (File.Length == 0)
            {
                RootId = 1;
                NextNodeId = 1;
            }
        }

        #region Insert
        public void AddValue(T value)
        {
            Insert(value, RootId);
        }

        private async void Insert(T value, int nodeId)
        {
            int bufferLength = 1024;
            byte[] buffer;
            TreeNode<T> CurrentNode = new TreeNode<T>(default, TreeOrder);
            if (NextNodeId == 0)
            {
                NextNodeId++;
                TreeNode<T> NewNode = new TreeNode<T>(value, TreeOrder);
                using var writer = new StreamWriter(File, Encoding.ASCII);
                await writer.WriteAsync($"{RootId},{NextNodeId},{TreeOrder}\r\n{NewNode.ToFixedSize()}");//Falta agregar método para convertir a un string la metadata.
            }
            else
            {
                buffer = new byte[bufferLength];
                File.Seek((nodeId - 1) * CurrentNode.GetNodeSize() + MetadataLength, SeekOrigin.Begin);
                File.Read(buffer, 0, CurrentNode.GetNodeSize());
                var valueString = ByteGenerator.ConvertToString(buffer);
                CurrentNode.GetT(valueString);
                if (CurrentNode.SubTrees.Count != 0)
                {
                    for (int i = 0; i < CurrentNode.NodeValues.Count; i++)
                    {
                        if (value.CompareTo(CurrentNode.NodeValues[i]) < 0)
                        {
                            if (i == 0)
                            {
                                //Insertar en el árbol de hasta la izquierda
                                Insert(value, CurrentNode.SubTrees[i]);
                                i = CurrentNode.NodeValues.Count;
                            }
                        }
                        else if (value.CompareTo(CurrentNode.NodeValues[i]) > 0)
                        {
                            if (i == CurrentNode.NodeValues.Count - 1)
                            {
                                //Insertar en el árbol de hasta la derecha
                                Insert(value, CurrentNode.SubTrees[i + 1]);
                                i = CurrentNode.NodeValues.Count;
                            }
                            else
                            {
                                if (value.CompareTo(CurrentNode.NodeValues[i + 1]) < 0)
                                {
                                    //Insertar en el i+1
                                    Insert(value, CurrentNode.SubTrees[i + 1]);
                                    i = CurrentNode.NodeValues.Count;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (!CurrentNode.NeedsSeparation())//Validación no está en el lugar correcto, tiene que ser luego de insertar el valor.
                    {
                        CurrentNode.AddValue(value);
                    }
                    //O inserto, o si está lleno, separo.
                }
            }
        }
        #endregion

        private void WriteNewNode(TreeNode<T> node)
        {
            
        }

        #region Delete
        public void DeleteValue(T value)
        {
            Delete(value, RootId);
        }

        private async void Delete(T value, int nodeId)
        {
            TreeNode<T> CurrentNode = new TreeNode<T>(default, TreeOrder);
            byte[] buffer = new byte[1024];
            File.Seek((nodeId - 1) * CurrentNode.GetNodeSize() + MetadataLength, SeekOrigin.Begin);
            File.Read(buffer, 0, CurrentNode.GetNodeSize());
            var valueString = ByteGenerator.ConvertToString(buffer);
            CurrentNode.GetT(valueString);
            if (CurrentNode.SubTrees.Count != 0)
            {
                CurrentNode.RemoveValue(value);
            }
            else
            {
                for (int i = 0; i < CurrentNode.NodeValues.Count; i++)
                {
                    if (value.CompareTo(CurrentNode.NodeValues[i]) < 0)
                    {
                        if (i == 0)
                        {
                            Delete(value, CurrentNode.SubTrees[i]);
                            i = CurrentNode.NodeValues.Count;
                        }
                    }
                    else if (value.CompareTo(CurrentNode.NodeValues[i]) == 0)
                    {
                        //Remover el valor y luego manejar la falta o underflow
                    }
                    else if (value.CompareTo(CurrentNode.NodeValues[i]) > 0)
                    {
                        if (i == CurrentNode.NodeValues.Count - 1)
                        {
                            Delete(value, CurrentNode.SubTrees[i + 1]);
                            i = CurrentNode.NodeValues.Count;
                        }
                        else
                        {
                            if (value.CompareTo(CurrentNode.NodeValues[i + 1]) < 0)
                            {
                                Delete(value, CurrentNode.SubTrees[i + 1]);
                                i = CurrentNode.NodeValues.Count;
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region PrevInsert

        //private void Insert(T value, TreeNode<T> node)
        //{
        //    if (node.SubTrees.Count != 0)
        //    {
        //        for (int i = 0; i < node.NodeValues.Count; i++)
        //        {
        //            if (value.CompareTo(node.NodeValues[i]) < 0)
        //            {
        //                    //Insertar en el árbol de hasta la izquierda
        //                    Insert(value, node.SubTrees[i]);
        //                    i = node.NodeValues.Count;   
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
        //            else
        //            {
        //                i = node.NodeValues.Count;
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
        //            StartIndex = (TreeOrder / 2) + 1; //5/2 = 2+1 =3
        //        }

        //         Pasar los valores y subárboles correspondientes al nuevo nodo
        //        TreeNode<T> NewNode = new TreeNode<T>(TreeOrder);
        //        for (int i = StartIndex; i < node.NodeValues.Count; i++)
        //        {
        //            NewNode.NodeValues.Add(node.NodeValues[i]);
        //            NewNode.SubTrees.Add(node.SubTrees[i]);  
        //            if(i == node.NodeValues.Count - 1)
        //            {
        //                NewNode.SubTrees.Add(node.SubTrees[i + 1]);
        //                node.SubTrees.Remove(i + 1);
        //                node.SubTrees[i] = -1;
        //                node.NodeValues.Remove(node.NodeValues[i]);
        //            }
        //            else
        //            {
        //                node.NodeValues[i] = default(T);
        //                node.SubTrees[i] = -1;
        //            }
        //        }


        //         Si no existe el padre, lo crea y envía el valor medio. De lo contrario,

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
        //             Insertar el nuevo nodo al lado del nodo anterior
        //            for (int i = 0; i < node.Father.SubTrees.Count; i++)
        //            {
        //                if (node.Father.SubTrees[i] == node)
        //                {
        //                    node.Father.SubTrees.Insert(i + 1, NewNode);
        //                }
        //            }
        //            NewNode.Father = node.Father;
        //             Aquí debemos verificar recursivamente si el nodo padre necesita separarse
        //            AddToNode(node.NodeValues[StartIndex - 1], node.Father);
        //             No hay que preocuparse porque quede en la posición correcta porque el .sort() 
        //             se hace cargo de eso
        //        }
        //    }
        //}
        #endregion
    }
}
