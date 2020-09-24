﻿using CustomGenerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class Movies : IComparable, IFixedSizeText
    {
        public string Director { get; set; }
        public double ImdbRating { get; set; }
        public string Genre { get; set; }
        public string ReleaseDate { get; set; }
        public int RottenTomatoesRating { get; set; }
        public string Title { get; set; }

        public int FixedSizeTextLength => throw new NotImplementedException();

        public int CompareTo(object obj)
        {
            if (Title.CompareTo(((Movies)obj).Title) == 0)
            {
                if (ReleaseDate.CompareTo(((Movies)obj).ReleaseDate) == 0)
                {
                    return Director.CompareTo(((Movies)obj).Director);
                }
                else
                {
                    return ReleaseDate.CompareTo(((Movies)obj).ReleaseDate);
                }
            }
            else
            {
                return Title.CompareTo(((Movies)obj).Title);
            }
        }

        public void GetT(string linea)
        {
            throw new NotImplementedException();
        }

        public string ToFixedSize()
        {
            throw new NotImplementedException();
        }
    }
}
