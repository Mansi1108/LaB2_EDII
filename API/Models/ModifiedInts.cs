using CustomGenerics;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class ModifiedInts : IComparable, IFixedSizeText
    {
        public int Number = -1;
        public int FixedSizeTextLength => 11;

        public ModifiedInts()
        {

        }

        public ModifiedInts(int number)
        {
            Number = number;
        }

        public int CompareTo(object obj)
        {
            return Number.CompareTo(((ModifiedInts)obj).Number);
        }

        public void GetT(string linea)
        {
            Number = Convert.ToInt32(linea.Trim());
        }

        public string ToFixedSize()
        {
            return $"{Number:00000000000;-0000000000}";
        }
    }
}
