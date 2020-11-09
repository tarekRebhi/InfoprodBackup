﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
   public class GrilleEvaluationEnqueteAuto
    {
            public int Id { get; set; }
            public float presentation { get; set; }
            public float respectScript { get; set; }
            public float traitementObjections { get; set; }
            public float exploitationOutils { get; set; }
            public float HistorisationOutils { get; set; }         
            public float discours { get; set; }
            public float debit { get; set; }
            public float intonation { get; set; }
            public float ecouteReformulation { get; set; }
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

