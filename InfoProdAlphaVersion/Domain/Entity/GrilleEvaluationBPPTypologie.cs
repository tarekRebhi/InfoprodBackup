using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class GrilleEvaluationBPPTypologie
    {
        public int Id { get; set; }
        public float nombreLogements { get; set; }
        public float nombreLogementsRs { get; set; }
        public float nombreSociaux { get; set; }
        public float nombreSociauxRs { get; set; }
        public float typeCommerce { get; set; }
        public float typeCommerceRs { get; set; }
        public float typeIndustrie { get; set; }
        public float typeIndustrieRs { get; set; }
        public float typeEtablissementSante { get; set; }
        public float typeEtablissementSanteRs { get; set; }
        public float typeAcceuil { get; set; }
        public float typeAcceuilRs { get; set; }
        public float capaciteAcceuilSante { get; set; }
        public float capaciteAcceuilSanteRs { get; set; }
        public float typeEtablissementHR { get; set; }
        public float typeEtablissementHRRs { get; set; }
        public float nombreEtoile { get; set; }
        public float nombreEtoileRs { get; set; }
        public float typeEnseignement { get; set; }
        public float typeEnseignementRs { get; set; }
        public float capaciteAcceuilEnseignement { get; set; }
        public float capaciteAcceuilEnseignementRs { get; set; }
        public float typeLoisirs { get; set; }
        public float typeLoisirsRs { get; set; }
        public float frigorifique { get; set; }
        public float frigorifiqueRs { get; set; }
        public float plateformeLogistique { get; set; }
        public float plateformeLogistiqueRs { get; set; }


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
