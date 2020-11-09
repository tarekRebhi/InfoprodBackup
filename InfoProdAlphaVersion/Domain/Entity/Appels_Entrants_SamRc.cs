using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity
{
   public class Appels_Entrants_SamRc
    {
        public int Id { get; set; }
        public int CustomerID { get; set; }
        public int Indice { get; set; }
        public DateTime Date { get; set; }
        public int CallType { get; set; }
        public int Duration { get; set; }
        public int WaitDuration { get; set; }
        public int ConvDuration { get; set; }
        public int WrapupDuration { get; set; }
        public string ANI { get; set; }
        public string DNIS { get; set; }
        public string LastCampaign { get; set; }
        public int Closed { get; set; }
        public int NoAgent { get; set; }
        public int Overflow { get; set; }
        public int Abandon { get; set; }
        public int LastQueue { get; set; }
        public int LastAgent { get; set; }
        public string LastTransfer { get; set; }
        public int CallStatusGroup { get; set; }
        public int CallStatusNum { get; set; }
        public int CallStatusDetail { get; set; }
        public int EndByAgent { get; set; }
        public int AgentListen { get; set; }
        public int semaine { get; set; }
        public string mois { get; set; }
        public int Annee { get; set; }
        public int FERIE { get; set; }
        public int Heure { get; set; }

    }
}
