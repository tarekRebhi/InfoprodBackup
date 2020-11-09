using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity
{
   public class Temps_SamRc
    {
        public int Id { get; set; }
        public int AgentId { get; set; }
        public DateTime date { get; set; }
        public int tps_prevew { get; set; }
        public int semaine { get; set; }
        public String mois { get; set; }
        public int Annee { get; set; }
        public int tps_log { get; set; }
        public int tps_pret_camp { get; set; }
        public int tps_com { get; set; }
        public int tps_att { get; set; }
        public int tps_acw { get; set; }
        public int tps_autres { get; set; }
        public int Temps_P_Dej { get; set; } 
        public int Temps_P_Brief { get; set; }
        public int Temps_P_Formation { get; set; }
        public int Temps_P_Qualif { get; set; }
        public int Temps_P_Cafe { get; set; }
        public int Temps_Back_OFFICE { get; set; }
        public int Temps_P_phonning { get; set; }
        public int Temps_P_admin { get; set; }
        public int tps_pause_9 { get; set; }
        public int Temps_P_Mission_OP { get; set; }
        public int Temps_P_QAS { get; set; }
        public int Temps_Inter_technique { get; set; }
        public int Temps_Appel_Ip { get; set; }
        public int Temps_P_debrif { get; set; }
        public int att_ape { get; set; }
    }
}
