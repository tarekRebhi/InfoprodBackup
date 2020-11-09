using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCWEB.Models
{
    public class ProdOperation
    {
        public string nomOpe { set; get; }
        public double PercentOk { set; get; }
        public double PercentKo { set; get; }
        public double PercentBarrage { set; get; }
        public double PercentSnd { set; get; }
        public double PercentOPTOUT { set; get; }
        public double PercentHorscible { set; get; }
    }
}