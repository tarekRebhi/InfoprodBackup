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
    public class EvaluationVecteurPlusViewModel
    {
        public List<SelectListItem> notes = new List<SelectListItem>();
        public List<SelectListItem> agents = new List<SelectListItem>();
        // En Commun
        public int Id { get; set; }
        public float presentationIdentification { get; set; }
        public float objetAppel { get; set; }
        public float mutualisation { get; set; }    
        public float coordonneesAttr { get; set; }
        public float traitementObjections { get; set; }
        public float transcriptionsInformations { get; set; }
        public float envoiMail { get; set; }
        public float discours { get; set; }
        public float attitude { get; set; }
        public float priseConge { get; set; }
        //QR
        public float marcheRenouvlable { get; set; }
        public float dureeMarche { get; set; }
        public float montantMarche { get; set; }
        public float responsableMarche { get; set; }
        public float mailContact { get; set; }
        //KLMO
        public float maitriseOeuvre { get; set; }
        public float avisAttr { get; set; }
        //BPP
        public float validationNature { get; set; }
        public float calendrier { get; set; }
        public float envergure { get; set; }
        public float complétudeQuestions { get; set; }
        public float confirmationIntervenants { get; set; }
        public float enchainementQuestions { get; set; }
        public float reformulation { get; set; }
        public float codesAppropriés { get; set; }
        public float noteClient { get; set; }
        //Battonage
        public float nature { get; set; }
        public float typologie { get; set; }
        public float avancement { get; set; }
        public float destination { get; set; }
        public float codeRP { get; set; }
        public float nomProgramme { get; set; }
        public float postIT { get; set; }
        public float objects { get; set; }
        public float notreClients { get; set; }
        public float moa { get; set; }
        public float moaArchitectes { get; set; }
        public float bureauxEtudes { get; set; }
        public float bailleurSociaux { get; set; }
        public float mairie { get; set; }
        public float dateAccords { get; set; }
        public float datePC { get; set; }
        public float dateConsultation { get; set; }
        public float dateTravaux { get; set; }
        public float dateLivraison { get; set; }
        public float surfacePlancher { get; set; }
        public float surfaceTerrain { get; set; }
        public float surfaceTypologie { get; set; }
        public float nombreBatiment { get; set; }
        public float nombreEtage { get; set; }
        public float ascenseur { get; set; }
        public float nombreAscenseur { get; set; }
        public float lieuExecution { get; set; }
        // PRV
        public float presentation { get; set; }
        public float validationDisponibilite { get; set; }
        public float questionnement { get; set; }
        public float propositionPRV { get; set; }
        public float objections { get; set; }
        public float validation { get; set; }
        public float recapitulatif { get; set; }

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