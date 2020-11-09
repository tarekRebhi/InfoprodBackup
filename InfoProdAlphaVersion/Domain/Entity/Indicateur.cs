using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class Indicateur
    {
        public int Id { get; set; }
        public int agent { get; set; }

        public int CA { get; set; }  //NOMBRE DE contact argumenté

        public int accord { get; set; } // nombre des accords

        public int CNA { get; set; } // nombre des contacts non argumenté

        public int totAboutis { get; set; }

        public float tempsLog { get; set; }

        public float tempsComm { get; set; }

        public float tempsAtt { get; set; }

        public float tempsACW { get; set; } //temps ACW tems d'appel

        public float tempsPausePerso { get; set; }
        public float tempsPreview { get; set; }

        public float tempsPauseBrief { get; set; }

        public int responsable { get; set; } // a revoir

        public DateTime date { get; set; } // a revoir

        public int semaine { get; set; } // a revoir

        public String mois { get; set; } // a revoir

        public int appelEmis { get; set; }

        public float tvente { get; set; }

        public float tAbs { get; set; }

        public int? employeeId { get; set; }

        public int? customerId { get; set; }
        public string nomCampagne { get; set; }
        [ForeignKey("employeeId")]
        public virtual Employee employee { get; set; }

        //public int? alerteId { get; set; }

        //[ForeignKey("alerteId")]
        //public virtual Alerte alerte { get; set; }

        public int? titreId { get; set; }

        [ForeignKey("titreId")]
        public virtual Titre titre { get; set; }
    }
}
