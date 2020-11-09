using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCWEB.Models
{
    public class temps
    {
        public string Agent { get; set; }
        public int IdHermes { get; set; }
        public string Image { get; set; }
        public string groupe { get; set; }
        public string Arrive { get; set; }
        public string Depart { get; set; }
        public string RetardArrive { get; set; }
        public string RetardDepart { get; set; }
        public string CumulRetard { get; set; }
        public string typearrive { get; set; }
        public string typedepart { get; set; }
        public string typecumul { get; set; }
    }
}