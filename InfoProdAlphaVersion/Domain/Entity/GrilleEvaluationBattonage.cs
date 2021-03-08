using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class GrilleEvaluationBattonage
    {
        public int Id { get; set; }
        public float nature { get; set; }
        public float natureRs { get; set; }

        public float typologie { get; set; }
        public float typologieRs { get; set; }
        public float avancement { get; set; }
        public float avancementRs { get; set; }
        public float destination { get; set; }
        public float destinationRs { get; set; }
        public float codeRP { get; set; }
        public float codeRPRs { get; set; }
        public float nomProgramme { get; set; }
        public float nomProgrammeRs { get; set; }
        public float postIT { get; set; }
        public float postITRs { get; set; }
        public float objects { get; set; }
        public float objectsRs { get; set; }
        public float notreClients { get; set; }
        public float notreClientsRs { get; set; }
        public float moa { get; set; }
        public float moaRs { get; set; }
        public float moaArchitectes { get; set; }
        public float moaArchitectesRs { get; set; }
        public float bureauxEtudes { get; set; }
        public float bureauxEtudesRs { get; set; }
        public float bailleurSociaux { get; set; }
        public float bailleurSociauxRs { get; set; }
        public float mairie { get; set; }
        public float mairieRs { get; set; }
        public float dateAccords { get; set; }
        public float dateAccordsRs { get; set; }
        public float datePC { get; set; }
        public float datePCRs { get; set; }
        public float dateConsultation { get; set; }
        public float dateConsultationRs { get; set; }
        public float dateTravaux { get; set; }
        public float dateTravauxRs { get; set; }
        public float dateLivraison { get; set; }
        public float dateLivraisonRs { get; set; }
        public float surfacePlancher { get; set; }
        public float surfacePlancherRs { get; set; }
        public float surfaceTerrain { get; set; }
        public float surfaceTerrainRs { get; set; }
        public float surfaceTypologie { get; set; }
        public float surfaceTypologieRs { get; set; }
        public float nombreBatiment { get; set; }
        public float nombreBatimentRs { get; set; }
        public float nombreEtage { get; set; }
        public float nombreEtageRs { get; set; }
        public float ascenseur { get; set; }
        public float ascenseurRs { get; set; }
        public float nombreAscenseur { get; set; }
        public float nombreAscenseurRs { get; set; }
        public float lieuExecution { get; set; }
        public float lieuExecutionRs { get; set; }

        //Les champs en communs
        public DateTime dateTempEvaluation { get; set; }
        public String type { get; set; }
        public int? employeeId { get; set; }
        [ForeignKey("employeeId")]
        public virtual Employee employee { get; set; }

        public float note { get; set; }

        public float noteRs { get; set; }

        public String commentaireQualite { get; set; }

        public String commentaireAgent { get; set; }

        public String enregistrementFullName { get; set; }
        public String enregistrementUrl { get; set; }
        public String enregistrementDirectory { get; set; }
        public int? senderId { get; set; }

    }
}
