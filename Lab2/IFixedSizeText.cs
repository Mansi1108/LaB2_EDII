using System;
using System.Collections.Generic;
using System.Text;

namespace CustomGenerics
{
    public interface IFixedSizeText
    {
        int FixedSizeTextLength { get; }
        void GetT(string linea);
        string ToFixedSize();
    }
}
