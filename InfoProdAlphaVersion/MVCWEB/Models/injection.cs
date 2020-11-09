using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCWEB.Models
{
    public class injection
    {
        public string Titre { set; get; }
        public double TotFichesInjectes { set; get; }
        public double TotCA { set; get; }
        public double TotAccords { set; get; }
        public double TauxCA { set; get; }
        public double TauxAccords { set; get; }
        public double Objectif { set; get; }
        public double ReelVSObj { set; get; }
    }
}