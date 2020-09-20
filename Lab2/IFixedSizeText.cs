using System;
using System.Collections.Generic;
using System.Text;

namespace CustomGenerics
{
    public interface IFixedSizeText<T>
    {
        int FixedSizeTextLength { get; }
        T GetT(string linea);
        string ToFixedSize();
    }
}
