using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
 public class AttendanceHermes_Campagnes
    {
        public int Id { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(10)]
        public string ID_Hermes { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(8)]
        public string Temps_Log { get; set; }


        [Column(TypeName = "VARCHAR")]
        [StringLength(8)]
        public string Temps_Comm { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(255)]
        public string campagne { get; set; }

        public int Customer_Id { get; set; }

        public int laDate { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "Date")]
        public DateTime Date { get; set; }

        public int Annee { get; set; }
        public int Mois { get; set; }
        public int Semaine { get; set; }

     

    
    }
}
