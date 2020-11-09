using System.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace MVCWEB.Models
{
    public class DirectoryViewModel
    {
        public string Id { get; set; }
        public string value { get; set; }
        public string pseudoName { get; set; }
        public String enregistrementName { get; set; }
        public int IdHermes { get; set; }
        public List<SelectListItem> utilisateurs = new List<SelectListItem>();
        public List<SelectListItem> utilisateursPseudonames = new List<SelectListItem>();
        public List<SelectListItem> topDirectorieDirectories = new List<SelectListItem>();
        public List<SelectListItem> IdHermeses = new List<SelectListItem>();


        public List<String> files = new List<String>();
        public List<String> filesTests = new List<String>();
        public List<FileInfo> filesTestsInfo = new List<FileInfo>();
        public  List<FileSystemInfo> filesSysTestsInfo = new List<FileSystemInfo>();

        public String directoryName { get; set; }
        public int minutes { get; set; }
        public int secondes { get; set; }

        public int nbresEnrefgistrements { get; set; }
        public String enregistrementFullName { get; set; }

        public String Url { get; set; }
        public String userName { get; set; }

        public String pseudoNameEmp { get; set; }
        public String empId { get; set; }
        public String indice { get; set; }
        public string roleName { get; set; }
        public string roleKey { get; set; }
        public List<SelectListItem> compagneEnreg = new List<SelectListItem>();
        public List<SelectListItem> listeSites = new List<SelectListItem>();
        public List<SelectListItem> listeSitesV5 = new List<SelectListItem>();
        public List<SelectListItem> qualifications = new List<SelectListItem>();
        public float NombreEnregistrements { get; set; }
        public double NombreEnregistrementsTotal { get; set; }
        public String agentName { get; set; }
        public List<accueil> compagneEnregV5 = new List<accueil>();
    }

}