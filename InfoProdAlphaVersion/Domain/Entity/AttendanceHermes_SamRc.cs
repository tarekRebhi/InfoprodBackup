
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class AttendanceHermes_SamRc
    {
        public int Id { get; set; }
        [DataType(DataType.Date)]
        [Column(TypeName = "Date")]
        public DateTime Date { get; set; }

        public int Id_Hermes { get; set; }
    
        public DateTime Arrive { get; set; } 
     
        public DateTime Depart { get; set; }
        public int CustomerID { get; set; }
        public int semaine { get; set; }

        public String mois { get; set; }

        public int Annee { get; set; }
    }
}
