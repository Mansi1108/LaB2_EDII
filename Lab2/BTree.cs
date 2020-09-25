﻿using CustomGenerics;
using CustomGenerics.Utilies;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ClassLibrary1
{
    public class BTree<T> where T : IComparable, IFixedSizeText
    {
        #region TreeVariables
        string FilePath;
        string FileName = "BTree.txt";
        FileStream File;
        int TreeOrder;
        int RootId;
        int NextNodeId;
        int MetadataLength = 18;//Hacer un método para obtener su valor
        public List<int> ST;
        public List<T> NV;
        #endregion

        #region SplittingVariables
        T MiddleValue;
        List<int> FatherSubtrees;
        List<T> RightValues;
        List<int> RightSubtreesValues;
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

        #region NodeMoves
        public T LeftMajor(int number)
        {
            T MajorValue = default ;
            TreeNode<T> CurrentNode = new TreeNode<T>(default, TreeOrder);
            byte[] buffer = new byte[1024];
            File.Seek((number - 1) * CurrentNode.GetNodeSize() + MetadataLength, SeekOrigin.Begin);
            File.Read(buffer, 0, CurrentNode.GetNodeSize());
            var valueString = ByteGenerator.ConvertToString(buffer);
            CurrentNode.GetT(valueString);
            if (!(CurrentNode.UnderFlow()))
            {
                for (int i = 0; i < CurrentNode.NodeValues.Count; i++)
                {
                    if (CurrentNode.NodeValues[i].CompareTo(MajorValue) > 0)
                    {
                        MajorValue = CurrentNode.NodeValues[i];
                    }
                }
                return MajorValue;
            }
            return default;
        }

        public T LowerRight(int number)
        {
            T LowerValue = default;
            TreeNode<T> CurrentNode = new TreeNode<T>(default, TreeOrder);
            byte[] buffer = new byte[1024];
            File.Seek((number - 1) * CurrentNode.GetNodeSize() + MetadataLength, SeekOrigin.Begin);
            File.Read(buffer, 0, CurrentNode.GetNodeSize());
            var valueString = ByteGenerator.ConvertToString(buffer);
            CurrentNode.GetT(valueString);
            if (!(CurrentNode.UnderFlow()))
            {
                for (int i = 0; i < CurrentNode.NodeValues.Count; i++)
                {
                    if (CurrentNode.NodeValues[i].CompareTo(default) != 0)
                    {
                        if (CurrentNode.NodeValues[i].CompareTo(LowerValue) < 0)
                        {
                            LowerValue = CurrentNode.NodeValues[i];
                        }
                    }
                }
                return LowerValue;
            }
            return default;
        }
        
        public void TransferValues(int number)
        {
            TreeNode<T> CurrentNode = new TreeNode<T>(default, TreeOrder);
            byte[] buffer = new byte[1024];
            File.Seek((number - 1) * CurrentNode.GetNodeSize() + MetadataLength, SeekOrigin.Begin);
            File.Read(buffer, 0, CurrentNode.GetNodeSize());
            var valueString = ByteGenerator.ConvertToString(buffer);
            CurrentNode.GetT(valueString);

            for (int i = 0; i < CurrentNode.NodeValues.Count; i++)
            {
                if (CurrentNode.NodeValues[i].CompareTo(default) != 0)
                {
                    NV.Add(CurrentNode.NodeValues[i]);
                }
            }
            for (int j = 0; j < CurrentNode.SubTrees.Count; j++)
            {
                if (CurrentNode.SubTrees[j] != -1)
                {
                    ST.Add(CurrentNode.SubTrees[j]);
                }
            }
            CurrentNode.DeathNode(); // :D (mire la referencia si aun no lo ha hecho)

        }

        public void ReceiveValues(int number)
        {
            TreeNode<T> CurrentNode = new TreeNode<T>(default, TreeOrder);
            byte[] buffer = new byte[1024];
            File.Seek((number - 1) * CurrentNode.GetNodeSize() + MetadataLength, SeekOrigin.Begin);
            File.Read(buffer, 0, CurrentNode.GetNodeSize());
            var valueString = ByteGenerator.ConvertToString(buffer);
            CurrentNode.GetT(valueString);

            foreach (var item in NV)
            {
                CurrentNode.NodeValues.Add(item);
            }
            foreach (var item in ST)
            {
                CurrentNode.SubTrees.Add(item);
            }
            NV.Clear();
            ST.Clear();
        }
        #endregion

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
            if (NextNodeId == 1)
            {
                NextNodeId++;
                TreeNode<T> NewNode = new TreeNode<T>(value, TreeOrder, RootId);
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
                            //Insertar en el árbol de hasta la izquierda
                            Insert(value, CurrentNode.SubTrees[i]);
                            i = CurrentNode.NodeValues.Count;
                            
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
                        else
                        {
                            i = CurrentNode.NodeValues.Count;
                        }
                    }
                }
                else
                {
                    CurrentNode.AddValue(value);
                    CurrentNode.NodeValues.Sort(); 
                    if (CurrentNode.NeedsSeparation()) //O inserto, o si está lleno, separo.
                    {
                        int StartIndex;
                        if (TreeOrder % 2 == 0)
                        {
                            StartIndex = (TreeOrder / 2) -1;
                        }
                        else
                        {
                            StartIndex = TreeOrder / 2; 
                        }

                        //Pasar los valores y subárboles correspondientes al nuevo nodo
                        TreeNode<T> NewNode = new TreeNode<T>(TreeOrder);
                        for (int i = StartIndex + 1; i < CurrentNode.NodeValues.Count; i++)
                        {
                            RightValues.Add(CurrentNode.NodeValues[i]);
                            RightSubtreesValues.Add(CurrentNode.SubTrees[i]);
                        }
                        RightSubtreesValues.Add(CurrentNode.SubTrees[CurrentNode.SubTrees.Count - 1]);
                        MiddleValue = CurrentNode.NodeValues[StartIndex];
                        CurrentNode.NodeValues.RemoveRange(StartIndex + 1, CurrentNode.NodeValues.Count - StartIndex);
                        CurrentNode.NodeValues.RemoveRange(StartIndex + 1, CurrentNode.SubTrees.Count - StartIndex);
                        // Sobreescribir el nodo actual
                        File.Seek((nodeId - 1) * CurrentNode.GetNodeSize() + MetadataLength, SeekOrigin.Begin); //Hay que revisar si hay que hacer este segundo seek.
                        using var writer = new StreamWriter(File, Encoding.ASCII);
                        await writer.WriteAsync(CurrentNode.ToFixedSize());//Falta el destructor del nodo para que no quede en memoria.
                        // Crear y escribir el nuevo nodo
                        int NewNodeId = NextNodeId;
                        FatherSubtrees.Add(CurrentNode.Id);
                        FatherSubtrees.Add(NewNodeId);
                        if (CurrentNode.FatherId == -1)
                        {
                            WriteNewRoot();
                        }
                        // Sobreescribir el padre o manejar la separación del nodo padre
                    }

                }
            }
        }
        #endregion

        #region Splitting
        private async void WriteNewNode()
        {
            TreeNode<T> NewNode = new TreeNode<T>(TreeOrder, NextNodeId);
            NextNodeId++;
            foreach (var value in RightValues)
            {
                NewNode.AddValue(value);
            }
            foreach (var subtree in RightSubtreesValues)
            {
                NewNode.SubTrees.Add(subtree);
            }
            // Escribir el nodo
            File.Seek((NewNode.Id - 1) * NewNode.GetNodeSize() + MetadataLength, SeekOrigin.Begin); //Hay que revisar si hay que hacer este segundo seek.
            using var writer = new StreamWriter(File, Encoding.ASCII);
            await writer.WriteAsync(NewNode.ToFixedSize());
        }

        private async void WriteNewRoot()
        {
            TreeNode<T> NewRoot = new TreeNode<T>(MiddleValue, TreeOrder, NextNodeId);
            NextNodeId++;
            foreach (var subtree in FatherSubtrees)
            {
                NewRoot.SubTrees.Add(subtree);
            }
            // Sobreescribir la metadata y escribir el nuevo nodo raíz.
        }
        #endregion

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
                        CurrentNode.RemoveValue(value);
                        T Replacement = LeftMajor(CurrentNode.SubTrees[i]);
                        if (Replacement.CompareTo(default) == 0)
                        {
                            Replacement = LowerRight(CurrentNode.SubTrees[i + 1]);

                            if (Replacement.CompareTo(default) == 0)
                            {
                                TransferValues(CurrentNode.SubTrees[i] + 1);
                                ReceiveValues(CurrentNode.SubTrees[i]);
                                CurrentNode.SubTrees[i + 1] = -1;
                            }
                        }

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
    }
}
