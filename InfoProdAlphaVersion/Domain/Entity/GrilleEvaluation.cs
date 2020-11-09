using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
   public class GrilleEvaluation
    {
        public int Id { get; set; }
        public float acceuilPresentation { get; set; }
        public float objetAppel { get; set; }

        public float presentationOffre { get; set; }

        public float gestionObjection { get; set; } //verouillage et conclusion contrat
        public float vCContrat { get; set; }

        public float pCross { get; set; } // proposition cross
        public float discours { get; set; } 
        public float attitude { get; set; }
        public float priseConge { get; set; }
        public float decouverteBesoins { get; set; }

        public float ppOffre { get; set; } //presentation et proposition offre
        public DateTime dateTempEvaluation { get; set; }

        public String type { get; set; }
        public int? employeeId { get; set; }
        [ForeignKey("employeeId")]
        public virtual Employee employee { get; set; }

        public float note { get; set; }

        public String commentaireQualite { get; set; }

        public String commentaireAgent { get; set; }

        public String enregistrementFullName { get; set; }
        public String enregistrementUrl { get; set; }
        public String enregistrementDirectory { get; set; }
        public int? senderId { get; set; }

    }
}
