using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class Appel
    {
        public int Id { get; set; }
        public int Id_Hermes { get; set; }
        public int Customer_Id { get; set; }
        [DataType(DataType.Date)]
        [Column(TypeName = "Date")]
        public DateTime date { get; set; }

        public int TotalAppelEmis { get; set; }

        public int TotalAppelAboutis { get; set; }

        public int CA { get; set; }
        public int Accords { get; set; }
        public int CNA { get; set; }
        public String nomCompagne { get; set; }

        public int semaine { get; set; }

        public String mois { get; set; }

        public int Annee { get; set; }
    }
}
