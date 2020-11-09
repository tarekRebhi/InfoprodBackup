using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
   public class Inventaire
    {
        public int Id { get; set; }
        //Info RH
        public string Manager { get; set; }
        public string Societe { get; set; }
        public string Matricule { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Mail { get; set; }
        public string Qualification { get; set; }
        public string Service { get; set; }
        public string Adresse { get; set; }
        public string Ville { get; set; }
        public string CIN { get; set; }
        public string Date_Delivrance_CIN { get; set; }
        public string Tel1 { get; set; }
        public string Tel2 { get; set; }

        //Environnement et test Technique
        public string Teletravail { get; set; }
        public string PC { get; set; }
        public string Connexion { get; set; }
        public string Type { get; set; }
        public string Debit { get; set; }
        public string Hermes { get; set; }
        public string Acces_VPN { get; set; }
        public string Acces_TSE { get; set; }

        public string Opérationnel { get; set; }
        public string Commentaire1 { get; set; }

        // Matériel fournie par l'employeur
        public string OrdinateurPro { get; set; }
        public string Modele { get; set; }
        public string SacaDos { get; set; }
        public string Ecran { get; set; }
        public string Adaptateur_HDMI { get; set; }
        public string Peripheriques { get; set; }
        public string Commentaire2 { get; set; }

        //Autre
        public string Contrat_Teletravail { get; set; }
        public string Date_Teletravail { get; set; }
        public string Commentaire3 { get; set; }

        public string SenderInv { get; set; }

    }
}
