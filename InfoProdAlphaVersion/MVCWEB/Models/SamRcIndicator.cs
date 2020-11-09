using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCWEB.Models
{
    public class SamRcIndicator
    {
        //Values of Appels Entrants
        public double AppelsRecus { get; set; }
        public int AppelsPris { get; set; }
        public double AppelsDecroches { get; set; }
        public int AppelsDecrochesinfdix { get; set; }
        public int AppelsAbandonnes { get; set; }
        public int AppelsDebordes { get; set; }
        public int AppelsNoAgent { get; set; }
        public double AppelsPerdus { get; set; }
        public int AppelsAttenteAvantAgent { get; set; }
        public int AppelsAttenteAvantAbandon { get; set; }
        public double ObjectifAppelsPris { get; set; }
        public double AtteinteObjectif { get; set; }
        public double QS { get; set; }
        public double AppelsEmis { get; set; }
        public TimeSpan TempsCommEntrant { get; set; }
        public double TauxCommEntrant { get; set; }
        public TimeSpan TempsCommSortant { get; set; }
        public double TauxCommSortant { get; set; }
        public TimeSpan DMCEntrant { get; set; }
        public TimeSpan DMCSortant { get; set; }

        //Values for tranche horaire
        public TimeSpan DMC { get; set; }
        public TimeSpan DMT { get; set; }
        public double Heure { get; set; }

        public string HeureDebut { get; set; }
        public string HeureFin { get; set; }
        //Values de Présence
        public int JoursTravailles { get; set; }
        public TimeSpan TempsLog { get; set; }
        public double ETP { get; set; }
        //Values de Téléphonie (temps et taux)
        public TimeSpan TempsCommunication { get; set; }
        public TimeSpan TempsAttente { get; set; }
        public TimeSpan TempsACW { get; set; }
        public TimeSpan TempsPauseCafe { get; set; }
        public TimeSpan TempsPauseBrief { get; set; }
        public TimeSpan TempsPauseDej { get; set; }
        public TimeSpan TempsMissionOp { get; set; }
        public TimeSpan TempsFormation { get; set; }
        public TimeSpan TempsQualification { get; set; }
        public TimeSpan TempsPhoning { get; set; }
        public TimeSpan TempsAdmin { get; set; }
        public TimeSpan TempsAppelIP { get; set; }
        public TimeSpan TempsProdBrut { get; set; }
        public TimeSpan TempsProdNet { get; set; }

        public double TauxComm { get; set; }
        public double TauxAttente { get; set; }
        public double TauxACW { get; set; }
        public double TauxPauseCafe { get; set; }
        public double TauxPauseBrief { get; set; }
        public double TauxPauseDej { get; set; }
        public double TauxMissionOP { get; set; }
        public double TauxFormation { get; set; }
        public double TauxQualification { get; set; }
        public double TauxPhoning { get; set; }
        public double TauxTempsAdmin { get; set; }
        public double TauxAppelIP { get; set; }
        public double TauxProdBrut { get; set; }
        public double TauxProdNet { get; set; }
    }
}