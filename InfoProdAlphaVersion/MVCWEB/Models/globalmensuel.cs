using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCWEB.Models
{
    public class globalmensuel
    {
        public string Mois { set; get; }
        public double AppelsEmis { set; get; }
        public double CA { set; get; }
        public double Accords { set; get; }
        public double TauxVenteHeure { get; set; }
        public double TauxExploitation { get; set;}
        public double TauxConcrétisation { set; get; }
    }
}