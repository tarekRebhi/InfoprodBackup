using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Domain.Entity
{
   public class GrilleEvaluationAchatPublic
    {
        public int Id { get; set; }
        public float accueil { get; set; }
        public float identificatioClient { get; set; }
        public float decouverteDemande { get; set; }
        public float maitrisePlateforme { get; set; }
        public float miseAttente { get; set; }
        public float tempsAttente { get; set; }
        public float qualiteService { get; set; }
        public float discours { get; set; }
        public float conclusionContact { get; set; }
        public float attitude { get; set; }
        public float qualification { get; set; }
        public float priseConge { get; set; }

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
