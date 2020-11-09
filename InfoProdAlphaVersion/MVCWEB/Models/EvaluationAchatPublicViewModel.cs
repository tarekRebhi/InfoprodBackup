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
    public class EvaluationAchatPublicViewModel
    {
        public List<SelectListItem> notes = new List<SelectListItem>();
        public List<SelectListItem> agents = new List<SelectListItem>();
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

        // public List<GrilleEvaluation> evaluations = new List<GrilleEvaluation>();

        public float pourcentageNotes { get; set; }

        public String Url { get; set; }
        public String userName { get; set; }

        public String pseudoNameEmp { get; set; }
        public String empId { get; set; }
        public int? senderId { get; set; }
        public int nbreEvaluations { get; set; }
    }
}