using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class GrilleEvaluationFO
    {
        public int Id { get; set; }
        public float accueil { get; set; }
        public float analyseDemande { get; set; }
        public float maitriseOutils { get; set; }
        public float connaissanceProduit { get; set; }
        public float respectProcess { get; set; }
        public float pertinenceReponse { get; set; }
        public float autonomie { get; set; }
        public float approcheCommerciale { get; set; }
        public float discours { get; set; }
        public float priseConge { get; set; }
        public float identificationClient { get; set; }
        public float qualificationDemande { get; set; }
        //Les champs en communs
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
