
using Domain.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace MVCWEB.Models
{
    public class EvaluationSCGISIViewModel
    {
        public List<SelectListItem> notes = new List<SelectListItem>();
        public List<SelectListItem> agents = new List<SelectListItem>();
        // En Commun
        public int Id { get; set; }
        //BO
        public string numIntervention { get; set; }
        //BO et en commun avec FO
        public float identificationClient { get; set; }
        public float qualificationDemande { get; set; }
        public float respectProcess { get; set; }
        public float pertinenceReponse { get; set; }
        public float discours { get; set; }
        public float approcheCommerciale { get; set; }

        //FO
        public float accueil { get; set; }
        public float analyseDemande { get; set; }
        public float maitriseOutils { get; set; }
        public float connaissanceProduit { get; set; }
        public float autonomie { get; set; }
        public float priseConge { get; set; }

       

        //Autres Champs en commun
        public DateTime dateTempEvaluation { get; set; }
        public String type { get; set; }
        public int? employeeId { get; set; }
        [ForeignKey("employeeId")]
        public virtual Employee employee { get; set; }

        public float note { get; set; }

        public String commentaireQualite { get; set; }

        public String commentaireAgent { get; set; }

        public String agentName { get; set; }
        public String senderName { get; set; }

        public String enregistrementFullName { get; set; }
        public String enregistrementUrl { get; set; }
        public String enregistrementDirectory { get; set; }

        public List<SelectListItem> employees = new List<SelectListItem>();

      //  public List<GrilleEvaluation> evaluations = new List<GrilleEvaluation>();

        public float pourcentageNotes { get; set; }

        public String Url { get; set; }
        public String userName { get; set; }

        public String pseudoNameEmp { get; set; }
        public String empId { get; set; }
        public int? senderId { get; set; }
        public int nbreEvaluations { get; set; }
    }
}