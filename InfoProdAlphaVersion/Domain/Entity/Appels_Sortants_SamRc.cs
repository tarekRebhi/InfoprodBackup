using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity
{
   public class Appels_Sortants_SamRc
    {
        public int Id { get; set; }
        public int? CustomerID { get; set; }
       public DateTime date { get; set; }
       public int ConvDuration { get; set; }
        public String ANI { get; set; }
        public String LastCampaign { get; set; }
        public int  Closed { get; set; }
        public int  NoAgent { get; set; }
        public int  Overflow { get; set; }
        public int  Abandon { get; set; }
        public int LastAgent { get; set; }
        public int semaine { get; set; }
        public String mois{ get; set; }
       public int Annee { get; set; }
       public int Heure { get; set; }
}
}
