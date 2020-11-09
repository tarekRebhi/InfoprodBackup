using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class Temps
    {
        public int Id { get; set; }
        public int Id_Hermes { get; set; }
        public int Customer_Id { get; set; }
        [DataType(DataType.Date)]
        [Column(TypeName = "Date")]
        public DateTime date { get; set; }

        public int tempsLog { get; set; }
        public int tempscom { get; set; }
        public int tempsAtt { get; set; }
        public int tempsACW { get; set; }
        public int tempsPausePerso { get; set; }

        public int tempsPreview { get; set; }
        public int semaine { get; set; }

        public String mois { get; set; }

        public int Annee { get; set; }

    }
}
