using CustomGenerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class Movies : IComparable, IFixedSizeText
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string ReleaseDate { get; set; }
        public string Director { get; set; }
        public string Genre { get; set; }
        public double ImdbRating { get; set; }
        public int RottenTomatoesRating { get; set; }

        public int FixedSizeTextLength => 205;

        public int CompareTo(object obj)
        {
            return ID.CompareTo(((Movies)obj).ID);
        }

        public string ToFixedSize()
        {
            return $"{string.Format("{0,-100}", Title)},{string.Format("{0,-20}", ReleaseDate)},{string.Format("{0,-50}", Director)}," +
                $"{string.Format("{0,-20}", Genre)},{ImdbRating:000.00},{RottenTomatoesRating:0000}";
        }

        public void GetT(string linea)
        {
            Title = linea.Substring(0, 100).Trim();
            linea = linea.Remove(0, 101);
            ReleaseDate = linea.Substring(0, 20).Trim();
            linea = linea.Remove(0, 21);
            Director = linea.Substring(0, 50).Trim();
            linea = linea.Remove(0, 51);
            Genre = linea.Substring(0, 20).Trim();
            linea = linea.Remove(0, 21);
            ImdbRating = double.Parse(linea.Substring(0, 6));
            linea = linea.Remove(0, 7);
            RottenTomatoesRating = int.Parse(linea.Trim());
            SetID();
        }

        public void SetID()
        {
            ID = $"{Title}-{ReleaseDate.Substring(ReleaseDate.Length-4,4)}";
        }
    }
}
