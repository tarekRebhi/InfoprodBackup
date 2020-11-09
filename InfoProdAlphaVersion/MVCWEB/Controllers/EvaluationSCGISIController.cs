using Domain.Entity;
using EASendMail;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using MVCWEB.Models;
using Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using System.IO;
using NAudio.Wave;
using System.Data;
using Data;
using System.Data.Entity;

namespace MVCWEB.Controllers
{
    [Authorize(Roles = "Qualité,Agent Qualité_Diffusion")]
    public class EvaluationSCGISIController : Controller
    {
        private ReportContext db = new ReportContext();

        #region globalVariable
        IEmployeeService serviceEmployee;
        IGrilleEvaluationService serviceDiff;
        IEvaluationFOSCGISIService service;
        IEvaluationBOSCGISIService serviceBO;
        IGroupeEmployeeService serviceGroupeEmp;
        IGroupeService serviceGroupe;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        #endregion
        #region constructor
        public EvaluationSCGISIController()
        {
            serviceEmployee = new EmployeeService();
            serviceDiff = new GrilleEvaluationService();
            service = new EvaluationFOSCGISIService();
            serviceBO = new EvaluationBOSCGISIService();
            serviceGroupeEmp = new GroupesEmployeService();
            serviceGroupe = new GroupeService();
        }
        public EvaluationSCGISIController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            RoleManager = roleManager;
        }
        #endregion
        #region ID
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }
        #endregion

        #region Front Office SC GISI 
        public ActionResult FrontOfficeSCGISI(string enregistrementFullName, string enregistrementUrl, string enregistrementDirectory, string agentName)
        {

            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 9))
            {
                ViewBag.role = "Agent Qualité_Diffusion";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 4))
            {
                ViewBag.role = "Qualité";
            }
            var a = new EvaluationViewModel();

            a.enregistrementFullName = enregistrementFullName;
            a.enregistrementUrl = enregistrementUrl;
            a.enregistrementDirectory = enregistrementDirectory;
            a.agentName = agentName;
            Employee employee = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            a.empId = "" + employee.Id;

            a.userName = employee.UserName;
            a.pseudoNameEmp = employee.pseudoName;
            if (employee.Content != null)
            {
                String strbase64 = Convert.ToBase64String(employee.Content);
                String Url = "data:" + employee.ContentType + ";base64," + strbase64;
                ViewBag.url = Url;
                a.Url = Url;
            }
            return View(a);
        }

        public ActionResult CalculFOSCGISI(string nomAgent, string planDate, string accueil, string analyseDemande, string maitriseOutils, string connaissanceProduit, string respectProcess, string pertinenceReponse, string autonomie, string approcheCommerciale, string discours, string priseConge,string identificationClient, string qualificationDemande, string commentaire, string enregistrement, string enregistrementUrl, string enregistrementDirectory)
        {
            float total = 47;
            List<string> NEList = new List<string>(new string[] { connaissanceProduit, approcheCommerciale });
           
            float notes = float.Parse(accueil, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(analyseDemande, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(maitriseOutils, CultureInfo.InvariantCulture.NumberFormat) +
            float.Parse(respectProcess, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(pertinenceReponse, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(autonomie, CultureInfo.InvariantCulture.NumberFormat) +
            float.Parse(discours, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(priseConge, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(identificationClient, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(qualificationDemande, CultureInfo.InvariantCulture.NumberFormat);

            foreach (var i in NEList)
            {
                if (float.Parse(i, CultureInfo.InvariantCulture.NumberFormat) < 0)
                {
                    total += float.Parse(i, CultureInfo.InvariantCulture.NumberFormat);
                }
                else
                {
                    notes += float.Parse(i, CultureInfo.InvariantCulture.NumberFormat);
                }
            }
            var a = new EvaluationSCGISIViewModel();
            a.accueil = float.Parse(accueil, CultureInfo.InvariantCulture.NumberFormat);
            a.analyseDemande = float.Parse(analyseDemande, CultureInfo.InvariantCulture.NumberFormat);
            a.maitriseOutils = float.Parse(maitriseOutils, CultureInfo.InvariantCulture.NumberFormat);
            a.connaissanceProduit = float.Parse(connaissanceProduit, CultureInfo.InvariantCulture.NumberFormat);
            a.respectProcess = float.Parse(respectProcess, CultureInfo.InvariantCulture.NumberFormat);
            a.pertinenceReponse = float.Parse(pertinenceReponse, CultureInfo.InvariantCulture.NumberFormat);
            a.autonomie = float.Parse(autonomie, CultureInfo.InvariantCulture.NumberFormat);
            a.approcheCommerciale = float.Parse(approcheCommerciale, CultureInfo.InvariantCulture.NumberFormat);
            a.discours = float.Parse(discours, CultureInfo.InvariantCulture.NumberFormat);
            a.priseConge = float.Parse(priseConge, CultureInfo.InvariantCulture.NumberFormat);
            a.identificationClient = float.Parse(identificationClient, CultureInfo.InvariantCulture.NumberFormat);
            a.qualificationDemande = float.Parse(qualificationDemande, CultureInfo.InvariantCulture.NumberFormat);

            a.dateTempEvaluation = DateTime.Parse(planDate);
            a.agentName = nomAgent;
            a.note = notes;
            a.pourcentageNotes = (notes / total) * 100;
            a.type = "FO SC GISI";

            if (Request.IsAjaxRequest())
            {
                return PartialView("FrontOfficeResult", a);
            }
            return RedirectToAction("listeSites", "Superviseur");
        }

        public ActionResult SaveEvalFOSCGISI(string nomAgent, string planDate, string accueil, string analyseDemande, string maitriseOutils, string connaissanceProduit, string respectProcess, string pertinenceReponse, string autonomie, string approcheCommerciale, string discours, string priseConge, string identificationClient, string qualificationDemande, string commentaire, string enregistrement, string enregistrementUrl, string enregistrementDirectory)
        {
            var userConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            Employee emp = serviceEmployee.getByPseudoName(nomAgent.ToLower());
            GrilleEvaluationFO a = new GrilleEvaluationFO();
            float total = 47;
            List<string> NEList = new List<string>(new string[] { connaissanceProduit, approcheCommerciale });

            float notes = float.Parse(accueil, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(analyseDemande, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(maitriseOutils, CultureInfo.InvariantCulture.NumberFormat) +
             float.Parse(respectProcess, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(pertinenceReponse, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(autonomie, CultureInfo.InvariantCulture.NumberFormat) +
             float.Parse(discours, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(priseConge, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(identificationClient, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(qualificationDemande, CultureInfo.InvariantCulture.NumberFormat);

            foreach (var i in NEList)
            {
                if (float.Parse(i, CultureInfo.InvariantCulture.NumberFormat) < 0)
                {
                    total += float.Parse(i, CultureInfo.InvariantCulture.NumberFormat);
                }
                else
                {
                    notes += float.Parse(i, CultureInfo.InvariantCulture.NumberFormat);
                }
            }
            a.accueil = float.Parse(accueil, CultureInfo.InvariantCulture.NumberFormat);
            a.analyseDemande = float.Parse(analyseDemande, CultureInfo.InvariantCulture.NumberFormat);
            a.maitriseOutils = float.Parse(maitriseOutils, CultureInfo.InvariantCulture.NumberFormat);
            a.connaissanceProduit = float.Parse(connaissanceProduit, CultureInfo.InvariantCulture.NumberFormat);
            a.respectProcess = float.Parse(respectProcess, CultureInfo.InvariantCulture.NumberFormat);
            a.pertinenceReponse = float.Parse(pertinenceReponse, CultureInfo.InvariantCulture.NumberFormat);
            a.autonomie = float.Parse(autonomie, CultureInfo.InvariantCulture.NumberFormat);
            a.approcheCommerciale = float.Parse(approcheCommerciale, CultureInfo.InvariantCulture.NumberFormat);
            a.discours = float.Parse(discours, CultureInfo.InvariantCulture.NumberFormat);
            a.priseConge = float.Parse(priseConge, CultureInfo.InvariantCulture.NumberFormat);
            a.identificationClient = float.Parse(identificationClient, CultureInfo.InvariantCulture.NumberFormat);
            a.qualificationDemande = float.Parse(qualificationDemande, CultureInfo.InvariantCulture.NumberFormat);

            a.dateTempEvaluation = DateTime.Parse(planDate);
            a.employeeId = emp.Id;
            a.senderId = userConnected.Id;
            a.note = (notes / total) * 100;
            a.type = "FO SC GISI";
            a.commentaireQualite = commentaire;
            a.enregistrementFullName = enregistrement;
            a.enregistrementUrl = enregistrementUrl;
            a.enregistrementDirectory = enregistrementDirectory;

            service.Add(a);
            service.SaveChange();
            var eval = new EvaluationSCGISIViewModel();
            eval.agentName = nomAgent;
            //try
            //{
            //    SmtpMail oMail = new SmtpMail("TryIt");
            //    EASendMail.SmtpClient oSmtp = new EASendMail.SmtpClient();

            //    // l"adresse Email d'emetteur 
            //    oMail.From = "Sana.BENSALAH@infopro-digital.com";

            //    //adresse destinataire
            //    oMail.To = emp.Email;
            //    // Objet Mail

            //    oMail.Subject = "Evaluation pour " + username;
            //    //contenu du mail designed avec HTML
            //    //oMail.HtmlBody = "<table><tr><td><h3 style='color:red;'>Résultat Evaluation:</h3><div><h5 style='color:#1E88E5;'>Date et temps d'évaluation :</h5><h5>" + dateCreationtest + "</h5></div><h5 style='color:#1E88E5;'>Acceuil / Presentation  :  " + acceuil+ "</h5><h5 style='color:#1E88E5;'>Objet d'appel : " + objetAppel+ "</h5><h5 style='color:#1E88E5;'>Présentation de l'offre / valider la satisfaction client: " + a.presentationOffre + "</h5><h5 style='color:#1E88E5;'>Gestion des objections : " + a.gestionObjection+ "</h5><h5 style='color:#1E88E5;'>Verrouillage et conclusion du contact : " + a.vCContrat+ "</h5><h5 style='color:#1E88E5;'>Proposition Cross : " + a.pCross+ "</h5><h5 style='color:#1E88E5;'>Discours : " + a.discours+ "</h5><h5 style='color:#1E88E5;'>Attitude : " + a.attitude+ "</h5><h5 style='color:#1E88E5;'>votre score est <font clolor ='red;'>" + a.pourcentageNotes + " %</font></h5>";

            //    oMail.HtmlBody = "<html><head><head><style>#customers {font-family:'Trebuchet MS',Arial,Helvetica,sans-serif;border-collapse: collapse;width: 100 %;}#customers td, #customers th {border: 1px solid #ddd;padding: 8px;}#customers tr:nth-child(even){background-color: #f2f2f2;}#customers tr:hover {background-color: #ddd;}#customers th {padding-top: 12px;padding-bottom: 12px;text-align: left;background-color: #4CAF50;color: white;}</style></head></head><body><p>Bonjour,<p><p>Vous avez une évaluation de la part du qualité </p><p>Cordialement.</p></body></html>";
            //    // oMail.HtmlBody = "<html><head><head><style>#customers {font-family:'Trebuchet MS',Arial,Helvetica,sans-serif;border-collapse: collapse;width: 100 %;}#customers td, #customers th {border: 1px solid #ddd;padding: 8px;}#customers tr:nth-child(even){background-color: #f2f2f2;}#customers tr:hover {background-color: #ddd;}#customers th {padding-top: 12px;padding-bottom: 12px;text-align: left;background-color: #4CAF50;color: white;}</style></head></head><body><h5 style='color:red;'>Nom Complet de l'enregistrement</h5><a href='#'><h7 style='color:blue;'>" + enregistrement + "</h7></a><br /><br/><table id='customers'><tr><th>Date Evaluation</th><th>Acceuil / Présentation</th><th>Objet d'appel</th><th>Présentation de l'offre / valider la satisfaction client</th> <th>Gestion objections</th><th>Verrouillage et conclusion du contact</th><th>Proposition Cross</th><th>Discours</th><th>Attitude</th><th>Prise de congé</th><th>Score</th></tr><tr><td>" + dateTest + "</td><td>" + acceuil + "</td><td>" + objetAppel + "</td><td>" + a.presentationOffre + "</td><td>" + a.gestionObjection + "</td><td>" + a.vCContrat + "</td><td>" + a.pCross + "</td><td>" + a.discours + "</td><td>" + a.attitude + "</td><td>" + a.priseConge + "</td><td style='color:red;'>" + a.pourcentageNotes + "%" + "</td></tr></table></body></html>";
            //    // Set email body
            //    //oMail.TextBody = "Vous venez d'étre évalué sur vos enregistrements ";

            //    // Instance du serveur SMTP et commpe parametre son Adresse.
            //    SmtpServer oServer = new SmtpServer("smtp.info.local");

            //    // username , password d'émetteur
            //    oServer.User = "Sana.BENSALAH@infopro-digital.com";
            //    oServer.Password = "Welcome01";

            //    // associer ou port  25 ou bien 587.
            //    oServer.Port = 587;

            //    // detect TLS connection automatically
            //    oServer.ConnectType = SmtpConnectType.ConnectSSLAuto;
            //    oSmtp.SendMail(oServer, oMail);

            //    ViewBag.msg = "mail sent";
            //}
            //catch (SmtpException)
            //{
            //    ViewBag.msg = "mail not sent";
            //    return RedirectToAction("Promo");
            //}
            if (Request.IsAjaxRequest())
            {
                return PartialView("EnvoiMailResult", eval);
            }
            return RedirectToAction("Acceuil", "Directory");

        }

        public ActionResult FrontOfficeSCGISIWithoutAgent(string enregistrementFullName, string enregistrementUrl, string enregistrementDirectory, string siteName)
        {
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            var a = new EvaluationViewModel();
            List<SelectListItem> agents = new List<SelectListItem>();
            List<Employee> logins = new List<Employee>();
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 9))
            {
                ViewBag.role = "Agent Qualité_Diffusion";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 4))
            {
                ViewBag.role = "Qualité";
            }

            a.enregistrementFullName = enregistrementFullName;
            a.enregistrementUrl = enregistrementUrl;
            a.enregistrementDirectory = enregistrementDirectory;

            if (siteName == "DIFFUSION")
            {
                List<string> listdiffusiongr = new List<string>(new string[] { "GISI-REAB", "GISI-PROMO", "GMT-REAB", "GMT-PROMO" });
                foreach (var g in listdiffusiongr)
                {
                    Groupe grr = serviceGroupe.getByNom(g);

                    var loginsdiffusion = serviceGroupeEmp.getListEmployeeByGroupeId(grr.Id);
                    logins.AddRange(loginsdiffusion);
                }
            }
            else
            {
                Groupe gr = serviceGroupe.getByNom(siteName);
                logins = serviceGroupeEmp.getListEmployeeByGroupeId(gr.Id);
            }
            var us = logins.Select(o => o).Distinct().ToList();
            var ordredpseudoNames = us.OrderBy(u => u.pseudoName).ToList();
            int j = 0;
            string agentName = "";
            while (j < logins.Count)
            {
                string tv = "TV." + logins[j].IdHermes;
                if (enregistrementFullName.Contains(tv))
                {
                    agentName = logins[j].UserName;
                }
                j++;
            }
            a.agentName = agentName;
            a.empId = "" + user.Id;
            a.userName = user.UserName;
            a.pseudoNameEmp = user.pseudoName;
            if (user.Content != null)
            {
                String strbase64 = Convert.ToBase64String(user.Content);
                String Url = "data:" + user.ContentType + ";base64," + strbase64;
                ViewBag.url = Url;
                a.Url = Url;
            }
            return View(a);
        }
        #endregion


        #region Back Office SC GISI 
        public ActionResult BackOfficeSCGISI(string enregistrementFullName, string enregistrementUrl, string enregistrementDirectory, string agentName)
        {

            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 9))
            {
                ViewBag.role = "Agent Qualité_Diffusion";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 4))
            {
                ViewBag.role = "Qualité";
            }
            var a = new EvaluationViewModel();

            a.enregistrementFullName = enregistrementFullName;
            a.enregistrementUrl = enregistrementUrl;
            a.enregistrementDirectory = enregistrementDirectory;
            a.agentName = agentName;
            List<Employee> logins = new List<Employee>();
            //if (Id == "DIFFUSION" && Version == "V4")
            //{
            //    List<string> listdiffusiongr = new List<string>(new string[] { "GISI-REAB", "GISI-PROMO", "GMT-REAB", "GMT-PROMO" });
            //    foreach (var g in listdiffusiongr)
            //    {
            //        Groupe grr = serviceGroupe.getByNom(g);

            //        var loginsdiffusion = serviceGroupeEmp.getListEmployeeByGroupeId(grr.Id);
            //        logins.AddRange(loginsdiffusion);
            //    }
            //}
            //else if (Id != "DIFFUSION" && Version == "V4")
            //{
            //    logins = serviceGroupe.getListEmployeeBySelectedSite(Id);
            //}
            //else
            //{


            //}
            //5586770313D415B4 = customer service
            var Id = "5586770313D415B4";
            logins = serviceGroupe.getListEmployeeBySelectedSiteV5(Id);
            var us = logins.Select(o => o).Distinct().ToList();

            var ordredpseudoNames = us.OrderBy(b => b.pseudoName).ToList();

            int i = 0;
            while (i < ordredpseudoNames.Count)
            {
                if (!ordredpseudoNames[i].UserName.Equals(user.UserName) && (ordredpseudoNames[i].Roles.Any(r => r.UserId == ordredpseudoNames[i].Id && r.RoleId == 3)))
                {
                    //Employee emp = serviceEmployee.getById(ordredpseudoNames[i].Id);
                    //var rolemp = serviceEmployee.getListRoles(emp).FirstOrDefault();
                    //if ( rolemp.Name== Id)
                    //{
                   // directoryModel.utilisateurs.Add(new SelectListItem { Text = ordredpseudoNames[i].UserName, Value = ordredpseudoNames[i].UserName });
                    a.employees.Add(new SelectListItem { Text = ordredpseudoNames[i].pseudoName, Value = ordredpseudoNames[i].pseudoName });
                    //}
                }
                i++;

            }
            Employee employee = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            a.empId = "" + employee.Id;

            a.userName = employee.UserName;
            a.pseudoNameEmp = employee.pseudoName;
            if (employee.Content != null)
            {
                String strbase64 = Convert.ToBase64String(employee.Content);
                String Url = "data:" + employee.ContentType + ";base64," + strbase64;
                ViewBag.url = Url;
                a.Url = Url;
            }
            return View(a);
        }

        public ActionResult BackOfficeSCGISIWithoutAgent(string enregistrementFullName, string enregistrementUrl, string enregistrementDirectory, string siteName)
        {
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            var a = new EvaluationViewModel();
            List<SelectListItem> agents = new List<SelectListItem>();
            List<Employee> logins = new List<Employee>();
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 9))
            {
                ViewBag.role = "Agent Qualité_Diffusion";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 4))
            {
                ViewBag.role = "Qualité";
            }

            a.enregistrementFullName = enregistrementFullName;
            a.enregistrementUrl = enregistrementUrl;
            a.enregistrementDirectory = enregistrementDirectory;

            if (siteName == "DIFFUSION")
            {
                List<string> listdiffusiongr = new List<string>(new string[] { "GISI-REAB", "GISI-PROMO", "GMT-REAB", "GMT-PROMO" });
                foreach (var g in listdiffusiongr)
                {
                    Groupe grr = serviceGroupe.getByNom(g);

                    var loginsdiffusion = serviceGroupeEmp.getListEmployeeByGroupeId(grr.Id);
                    logins.AddRange(loginsdiffusion);
                }
            }
            else
            {
                Groupe gr = serviceGroupe.getByNom(siteName);
                logins = serviceGroupeEmp.getListEmployeeByGroupeId(gr.Id);
            }
            var us = logins.Select(o => o).Distinct().ToList();
            var ordredpseudoNames = us.OrderBy(u => u.pseudoName).ToList();
            int j = 0;
            string agentName = "";
            while (j < logins.Count)
            {
                string tv = "TV." + logins[j].IdHermes;
                if (enregistrementFullName.Contains(tv))
                {
                    agentName = logins[j].UserName;
                }
                j++;
            }
            a.agentName = agentName;
            a.empId = "" + user.Id;
            a.userName = user.UserName;
            a.pseudoNameEmp = user.pseudoName;
            if (user.Content != null)
            {
                String strbase64 = Convert.ToBase64String(user.Content);
                String Url = "data:" + user.ContentType + ";base64," + strbase64;
                ViewBag.url = Url;
                a.Url = Url;
            }
            return View(a);
        }
        public ActionResult CalculBOSCGISI(string nomAgent, string planDate,string numIntervention, string identificationClient, string qualificationDemande, string respectProcess, string pertinenceReponse, string discours, string approcheCommerciale, string commentaire, string enregistrement, string enregistrementUrl, string enregistrementDirectory)
        {
            float total = 31;
            List<string> NEList = new List<string>(new string[] { approcheCommerciale });

            float notes = float.Parse(respectProcess, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(pertinenceReponse, CultureInfo.InvariantCulture.NumberFormat) +
            float.Parse(discours, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(identificationClient, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(qualificationDemande, CultureInfo.InvariantCulture.NumberFormat);

            foreach (var i in NEList)
            {
                if (float.Parse(i, CultureInfo.InvariantCulture.NumberFormat) < 0)
                {
                    total += float.Parse(i, CultureInfo.InvariantCulture.NumberFormat);
                }
                else
                {
                    notes += float.Parse(i, CultureInfo.InvariantCulture.NumberFormat);
                }
            }
            var a = new EvaluationSCGISIViewModel();
            a.identificationClient = float.Parse(identificationClient, CultureInfo.InvariantCulture.NumberFormat);
            a.qualificationDemande = float.Parse(qualificationDemande, CultureInfo.InvariantCulture.NumberFormat);
            a.respectProcess = float.Parse(respectProcess, CultureInfo.InvariantCulture.NumberFormat);
            a.pertinenceReponse = float.Parse(pertinenceReponse, CultureInfo.InvariantCulture.NumberFormat);  
            a.discours = float.Parse(discours, CultureInfo.InvariantCulture.NumberFormat);
            a.approcheCommerciale = float.Parse(approcheCommerciale, CultureInfo.InvariantCulture.NumberFormat);

            a.dateTempEvaluation = DateTime.Parse(planDate);
            a.agentName = nomAgent;
            a.numIntervention = numIntervention;
            a.note = notes;
            a.pourcentageNotes = (notes / total) * 100;
            a.type = "BO SC GISI";

            if (Request.IsAjaxRequest())
            {
                return PartialView("BackOfficeResult", a);
            }
            return RedirectToAction("listeSites", "Superviseur");
        }

        public ActionResult SaveEvalBOSCGISI(string nomAgent, string planDate, string numIntervention, string identificationClient, string qualificationDemande, string respectProcess, string pertinenceReponse, string discours, string approcheCommerciale, string commentaire, string enregistrement, string enregistrementUrl, string enregistrementDirectory)
        {
            var userConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            Employee emp = serviceEmployee.getByPseudoName(nomAgent.ToLower());
            GrilleEvaluationBO a = new GrilleEvaluationBO();
            float total = 31;
            List<string> NEList = new List<string>(new string[] { approcheCommerciale });

            float notes = float.Parse(respectProcess, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(pertinenceReponse, CultureInfo.InvariantCulture.NumberFormat) +
            float.Parse(discours, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(identificationClient, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(qualificationDemande, CultureInfo.InvariantCulture.NumberFormat);

            foreach (var i in NEList)
            {
                if (float.Parse(i, CultureInfo.InvariantCulture.NumberFormat) < 0)
                {
                    total += float.Parse(i, CultureInfo.InvariantCulture.NumberFormat);
                }
                else
                {
                    notes += float.Parse(i, CultureInfo.InvariantCulture.NumberFormat);
                }
            }
            a.identificationClient = float.Parse(identificationClient, CultureInfo.InvariantCulture.NumberFormat);
            a.qualificationDemande = float.Parse(qualificationDemande, CultureInfo.InvariantCulture.NumberFormat);
            a.respectProcess = float.Parse(respectProcess, CultureInfo.InvariantCulture.NumberFormat);
            a.pertinenceReponse = float.Parse(pertinenceReponse, CultureInfo.InvariantCulture.NumberFormat);
            a.discours = float.Parse(discours, CultureInfo.InvariantCulture.NumberFormat);
            a.approcheCommerciale = float.Parse(approcheCommerciale, CultureInfo.InvariantCulture.NumberFormat);

            a.dateTempEvaluation = DateTime.Parse(planDate);
            a.employeeId = emp.Id;
            a.numIntervention = numIntervention;
            a.senderId = userConnected.Id;
            a.note = (notes / total) * 100;
            a.type = "BO SC GISI";
            a.commentaireQualite = commentaire;
            a.enregistrementFullName = enregistrement;
            a.enregistrementUrl = enregistrementUrl;
            a.enregistrementDirectory = enregistrementDirectory;

            serviceBO.Add(a);
            serviceBO.SaveChange();
            var eval = new EvaluationSCGISIViewModel();
            eval.agentName = nomAgent;
            //try
            //{
            //    SmtpMail oMail = new SmtpMail("TryIt");
            //    EASendMail.SmtpClient oSmtp = new EASendMail.SmtpClient();

            //    // l"adresse Email d'emetteur 
            //    oMail.From = "Sana.BENSALAH@infopro-digital.com";

            //    //adresse destinataire
            //    oMail.To = emp.Email;
            //    // Objet Mail

            //    oMail.Subject = "Evaluation pour " + username;
            //    //contenu du mail designed avec HTML
            //    //oMail.HtmlBody = "<table><tr><td><h3 style='color:red;'>Résultat Evaluation:</h3><div><h5 style='color:#1E88E5;'>Date et temps d'évaluation :</h5><h5>" + dateCreationtest + "</h5></div><h5 style='color:#1E88E5;'>Acceuil / Presentation  :  " + acceuil+ "</h5><h5 style='color:#1E88E5;'>Objet d'appel : " + objetAppel+ "</h5><h5 style='color:#1E88E5;'>Présentation de l'offre / valider la satisfaction client: " + a.presentationOffre + "</h5><h5 style='color:#1E88E5;'>Gestion des objections : " + a.gestionObjection+ "</h5><h5 style='color:#1E88E5;'>Verrouillage et conclusion du contact : " + a.vCContrat+ "</h5><h5 style='color:#1E88E5;'>Proposition Cross : " + a.pCross+ "</h5><h5 style='color:#1E88E5;'>Discours : " + a.discours+ "</h5><h5 style='color:#1E88E5;'>Attitude : " + a.attitude+ "</h5><h5 style='color:#1E88E5;'>votre score est <font clolor ='red;'>" + a.pourcentageNotes + " %</font></h5>";

            //    oMail.HtmlBody = "<html><head><head><style>#customers {font-family:'Trebuchet MS',Arial,Helvetica,sans-serif;border-collapse: collapse;width: 100 %;}#customers td, #customers th {border: 1px solid #ddd;padding: 8px;}#customers tr:nth-child(even){background-color: #f2f2f2;}#customers tr:hover {background-color: #ddd;}#customers th {padding-top: 12px;padding-bottom: 12px;text-align: left;background-color: #4CAF50;color: white;}</style></head></head><body><p>Bonjour,<p><p>Vous avez une évaluation de la part du qualité </p><p>Cordialement.</p></body></html>";
            //    // oMail.HtmlBody = "<html><head><head><style>#customers {font-family:'Trebuchet MS',Arial,Helvetica,sans-serif;border-collapse: collapse;width: 100 %;}#customers td, #customers th {border: 1px solid #ddd;padding: 8px;}#customers tr:nth-child(even){background-color: #f2f2f2;}#customers tr:hover {background-color: #ddd;}#customers th {padding-top: 12px;padding-bottom: 12px;text-align: left;background-color: #4CAF50;color: white;}</style></head></head><body><h5 style='color:red;'>Nom Complet de l'enregistrement</h5><a href='#'><h7 style='color:blue;'>" + enregistrement + "</h7></a><br /><br/><table id='customers'><tr><th>Date Evaluation</th><th>Acceuil / Présentation</th><th>Objet d'appel</th><th>Présentation de l'offre / valider la satisfaction client</th> <th>Gestion objections</th><th>Verrouillage et conclusion du contact</th><th>Proposition Cross</th><th>Discours</th><th>Attitude</th><th>Prise de congé</th><th>Score</th></tr><tr><td>" + dateTest + "</td><td>" + acceuil + "</td><td>" + objetAppel + "</td><td>" + a.presentationOffre + "</td><td>" + a.gestionObjection + "</td><td>" + a.vCContrat + "</td><td>" + a.pCross + "</td><td>" + a.discours + "</td><td>" + a.attitude + "</td><td>" + a.priseConge + "</td><td style='color:red;'>" + a.pourcentageNotes + "%" + "</td></tr></table></body></html>";
            //    // Set email body
            //    //oMail.TextBody = "Vous venez d'étre évalué sur vos enregistrements ";

            //    // Instance du serveur SMTP et commpe parametre son Adresse.
            //    SmtpServer oServer = new SmtpServer("smtp.info.local");

            //    // username , password d'émetteur
            //    oServer.User = "Sana.BENSALAH@infopro-digital.com";
            //    oServer.Password = "Welcome01";

            //    // associer ou port  25 ou bien 587.
            //    oServer.Port = 587;

            //    // detect TLS connection automatically
            //    oServer.ConnectType = SmtpConnectType.ConnectSSLAuto;
            //    oSmtp.SendMail(oServer, oMail);

            //    ViewBag.msg = "mail sent";
            //}
            //catch (SmtpException)
            //{
            //    ViewBag.msg = "mail not sent";
            //    return RedirectToAction("Promo");
            //}
            if (Request.IsAjaxRequest())
            {
                return PartialView("EnvoiMailResult", eval);
            }
            return RedirectToAction("Acceuil", "Directory");

        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        #region Historique Qualité Front/Back Office SC GISI
        public ActionResult HistoriqueFOSCGISI()
        {
            List<Employee> logins = new List<Employee>();
            EvaluationViewModel evaluation = new EvaluationViewModel();
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 8))
            {
                ViewBag.role = "Agent Qualité_CustomerService";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 9))
            {
                ViewBag.role = "Agent Qualité_Diffusion";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 4))
            {
                ViewBag.role = "Qualité";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 2))
            {
                ViewBag.role = "Manager";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 6))
            {
                ViewBag.role = "Manager";
            }


            string d = "CUSTOMER SERVICE";
            Groupe gr = serviceGroupe.getByNom(d);
            List<Employee> emp = serviceGroupeEmp.getListEmployeeByGroupeId(gr.Id);
            foreach (var e in emp)
            {
                if (!logins.Exists(l => l.UserName == e.UserName))
                {
                    logins.Add(e);
                }
            }
            var employees = logins.OrderBy(a => a.UserName).ToList();
            foreach (var test in employees)
            {
                if (!test.UserName.Equals(user.UserName) && test.Roles.Any(r => r.UserId == test.Id && r.RoleId == 3))
                {
                    evaluation.employees.Add(new SelectListItem { Text = test.UserName, Value = test.UserName });

                }
            }

            Employee employee = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            evaluation.empId = "" + employee.Id;

            evaluation.userName = employee.UserName;
            evaluation.pseudoNameEmp = employee.pseudoName;
            if (employee.Content != null)
            {
                String strbase64 = Convert.ToBase64String(employee.Content);
                String Url = "data:" + employee.ContentType + ";base64," + strbase64;
                ViewBag.url = Url;
                evaluation.Url = Url;

            }
            return View(evaluation);
        }

        public ActionResult GetHistoFO(string username)
        {
            List<EvaluationSCGISIViewModel> a = new List<EvaluationSCGISIViewModel>();
            try
            {
                Employee emp = serviceEmployee.getByLogin(username);
                var historstions = service.GetEvaluationsByEmployee(emp.Id);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationSCGISIViewModel test = new EvaluationSCGISIViewModel();
                    test.Id = item.Id;
                    test.accueil = item.accueil;
                    test.analyseDemande = item.analyseDemande;
                    test.maitriseOutils = item.maitriseOutils;
                    test.connaissanceProduit = item.connaissanceProduit;
                    test.respectProcess = item.respectProcess;
                    test.pertinenceReponse = item.pertinenceReponse;
                    test.autonomie = item.autonomie;
                    test.approcheCommerciale = item.approcheCommerciale;
                    test.discours = item.discours;
                    test.priseConge = item.priseConge;
                    test.identificationClient = item.identificationClient;
                    test.qualificationDemande = item.qualificationDemande;
                    test.dateTempEvaluation = item.dateTempEvaluation;
                    test.type = item.type;
                    test.note = item.note;
                    test.commentaireQualite = item.commentaireQualite;
                    test.commentaireAgent = item.commentaireAgent;
                    test.enregistrementFullName = item.enregistrementFullName;
                    test.enregistrementUrl = item.enregistrementUrl;
                    test.enregistrementDirectory = item.enregistrementDirectory;

                    if (item.employeeId != null)
                    {
                        test.employeeId = item.employeeId;
                        Employee reciever = serviceEmployee.getById(item.employeeId);
                        test.agentName = reciever.UserName;
                    }
                    if (item.senderId != null)
                    {
                        test.senderId = item.senderId;
                        Employee sender = serviceEmployee.getById(item.senderId);
                        test.senderName = sender.UserName;
                    }
                    a.Add(test);
                }

                if (Request.IsAjaxRequest())
                {
                    return PartialView("HistoriqueFOPartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("HistoriqueFOGISI");
        }

        public ActionResult GetHistoFOByDate(string username, string dateDebut, string dateFin)
        {
            List<EvaluationSCGISIViewModel> a = new List<EvaluationSCGISIViewModel>();
            try
            {
                Employee emp = serviceEmployee.getByLogin(username);
                DateTime daterecherchedebut = DateTime.Parse(dateDebut);
                DateTime daterecherchefin = DateTime.Parse(dateFin);


                var historstions = service.GetEvaluationsEmployeeBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationSCGISIViewModel test = new EvaluationSCGISIViewModel();
                    test.Id = item.Id;
                    test.accueil = item.accueil;
                    test.analyseDemande = item.analyseDemande;
                    test.maitriseOutils = item.maitriseOutils;
                    test.connaissanceProduit = item.connaissanceProduit;
                    test.respectProcess = item.respectProcess;
                    test.pertinenceReponse = item.pertinenceReponse;
                    test.autonomie = item.autonomie;
                    test.approcheCommerciale = item.approcheCommerciale;
                    test.discours = item.discours;
                    test.priseConge = item.priseConge;
                    test.identificationClient = item.identificationClient;
                    test.qualificationDemande = item.qualificationDemande;
                    test.dateTempEvaluation = item.dateTempEvaluation;
                    test.type = item.type;
                    test.note = item.note;
                    test.commentaireQualite = item.commentaireQualite;
                    test.commentaireAgent = item.commentaireAgent;
                    test.enregistrementFullName = item.enregistrementFullName;
                    test.enregistrementUrl = item.enregistrementUrl;
                    test.enregistrementDirectory = item.enregistrementDirectory;

                    if (item.employeeId != null)
                    {
                        test.employeeId = item.employeeId;
                        Employee reciever = serviceEmployee.getById(item.employeeId);
                        test.agentName = reciever.UserName;
                    }
                    if (item.senderId != null)
                    {
                        test.senderId = item.senderId;
                        Employee sender = serviceEmployee.getById(item.senderId);
                        test.senderName = sender.UserName;
                    }
                    a.Add(test);
                }

                if (Request.IsAjaxRequest())
                {
                    return PartialView("HistoriqueFOPartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("HistoriqueFO");
        }
     
        public ActionResult HistoriqueBOSCGISI()
        {
            List<Employee> logins = new List<Employee>();
            EvaluationSCGISIViewModel evaluation = new EvaluationSCGISIViewModel();
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 8))
            {
                ViewBag.role = "Agent Qualité_CustomerService";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 4))
            {
                ViewBag.role = "Qualité";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 2))
            {
                ViewBag.role = "Manager";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 6))
            {
                ViewBag.role = "Manager";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 9))
            {
                ViewBag.role = "Agent Qualité_Diffusion";
            }

            string d = "CUSTOMER SERVICE";
            Groupe gr = serviceGroupe.getByNom(d);
            List<Employee> emp = serviceGroupeEmp.getListEmployeeByGroupeId(gr.Id);
            foreach (var e in emp)
            {
                if (!logins.Exists(l => l.UserName == e.UserName))
                {
                    logins.Add(e);
                }
            }
            var employees = logins.OrderBy(a => a.UserName).ToList();
            foreach (var test in employees)
            {
                if (!test.UserName.Equals(user.UserName) && test.Roles.Any(r => r.UserId == test.Id && r.RoleId == 3))
                {
                    evaluation.employees.Add(new SelectListItem { Text = test.UserName, Value = test.UserName });

                }
            }

            Employee employee = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            evaluation.empId = "" + employee.Id;

            evaluation.userName = employee.UserName;
            evaluation.pseudoNameEmp = employee.pseudoName;
            if (employee.Content != null)
            {
                String strbase64 = Convert.ToBase64String(employee.Content);
                String Url = "data:" + employee.ContentType + ";base64," + strbase64;
                ViewBag.url = Url;
                evaluation.Url = Url;

            }
            return View(evaluation);
        }

        public ActionResult GetHistoBO(string username)
        {
            List<EvaluationSCGISIViewModel> a = new List<EvaluationSCGISIViewModel>();
            try
            {
                Employee emp = serviceEmployee.getByLogin(username);
                var historstions = serviceBO.GetEvaluationsByEmployee(emp.Id);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationSCGISIViewModel test = new EvaluationSCGISIViewModel();
                    test.Id = item.Id;
                    test.identificationClient = item.identificationClient;
                    test.qualificationDemande = item.qualificationDemande;
                    test.respectProcess = item.respectProcess;
                    test.pertinenceReponse = item.pertinenceReponse;
                    test.discours = item.discours;
                    test.approcheCommerciale = item.approcheCommerciale;                    
                    test.dateTempEvaluation = item.dateTempEvaluation;
                    test.numIntervention = item.numIntervention;
                    test.type = item.type;
                    test.note = item.note;
                    test.commentaireQualite = item.commentaireQualite;
                    test.commentaireAgent = item.commentaireAgent;
                    test.enregistrementFullName = item.enregistrementFullName;
                    test.enregistrementUrl = item.enregistrementUrl;
                    test.enregistrementDirectory = item.enregistrementDirectory;

                    if (item.employeeId != null)
                    {
                        test.employeeId = item.employeeId;
                        Employee reciever = serviceEmployee.getById(item.employeeId);
                        test.agentName = reciever.UserName;
                    }
                    if (item.senderId != null)
                    {
                        test.senderId = item.senderId;
                        Employee sender = serviceEmployee.getById(item.senderId);
                        test.senderName = sender.UserName;
                    }
                    a.Add(test);
                }

                if (Request.IsAjaxRequest())
                {
                    return PartialView("HistoriqueBOPartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("HistoriqueBOGISI");
        }

        public ActionResult GetHistoBOByDate(string username, string dateDebut, string dateFin)
        {
            List<EvaluationSCGISIViewModel> a = new List<EvaluationSCGISIViewModel>();
            try
            {
                Employee emp = serviceEmployee.getByLogin(username);
                DateTime daterecherchedebut = DateTime.Parse(dateDebut);
                DateTime daterecherchefin = DateTime.Parse(dateFin);


                var historstions = serviceBO.GetEvaluationsEmployeeBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationSCGISIViewModel test = new EvaluationSCGISIViewModel();
                    test.Id = item.Id;
                    test.identificationClient = item.identificationClient;
                    test.qualificationDemande = item.qualificationDemande;
                    test.respectProcess = item.respectProcess;
                    test.pertinenceReponse = item.pertinenceReponse;
                    test.discours = item.discours;
                    test.dateTempEvaluation = item.dateTempEvaluation;
                    test.numIntervention = item.numIntervention;
                    test.type = item.type;
                    test.note = item.note;
                    test.commentaireQualite = item.commentaireQualite;
                    test.commentaireAgent = item.commentaireAgent;
                    test.enregistrementFullName = item.enregistrementFullName;
                    test.enregistrementUrl = item.enregistrementUrl;
                    test.enregistrementDirectory = item.enregistrementDirectory;

                    if (item.employeeId != null)
                    {
                        test.employeeId = item.employeeId;
                        Employee reciever = serviceEmployee.getById(item.employeeId);
                        test.agentName = reciever.UserName;
                    }
                    if (item.senderId != null)
                    {
                        test.senderId = item.senderId;
                        Employee sender = serviceEmployee.getById(item.senderId);
                        test.senderName = sender.UserName;
                    }
                    a.Add(test);
                }

                if (Request.IsAjaxRequest())
                {
                    return PartialView("HistoriqueBOPartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("HistoriqueBO");
        }
        #endregion


        #region Archive Front/Back Office SC GISI
        //Archive evaluations par Responsable Qualité
        [Authorize(Roles = "Qualité")]
        public ActionResult ArchiveFOSCGISI()
        {
            EvaluationSCGISIViewModel evaluation = new EvaluationSCGISIViewModel();
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            var logins = serviceEmployee.GetAll();
            var employees = logins.Select(o => o).Distinct().ToList();

            foreach (var test in employees)
            {
                if (test.Roles.Any(r => r.UserId == test.Id && r.RoleId == 4 || r.RoleId == 8 || r.RoleId == 9))
                {
                    evaluation.employees.Add(new SelectListItem { Text = test.UserName, Value = test.UserName });

                }
            }
            Employee employee = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            evaluation.empId = "" + employee.Id;

            evaluation.userName = employee.UserName;
            evaluation.pseudoNameEmp = employee.pseudoName;
            if (employee.Content != null)
            {
                String strbase64 = Convert.ToBase64String(employee.Content);
                String Url = "data:" + employee.ContentType + ";base64," + strbase64;
                ViewBag.url = Url;
                evaluation.Url = Url;

            }
            return View(evaluation);
        }

        public ActionResult GetArchiveQualiteFO(string username)
        {
            List<EvaluationSCGISIViewModel> a = new List<EvaluationSCGISIViewModel>();
            try
            {
                Employee emp = serviceEmployee.getByLogin(username);
                var historstions = service.GetEvaluationsBySenderId(emp.Id);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationSCGISIViewModel test = new EvaluationSCGISIViewModel();
                    test.Id = item.Id;
                    test.accueil = item.accueil;
                    test.analyseDemande = item.analyseDemande;
                    test.maitriseOutils = item.maitriseOutils;
                    test.connaissanceProduit = item.connaissanceProduit;
                    test.respectProcess = item.respectProcess;
                    test.pertinenceReponse = item.pertinenceReponse;
                    test.autonomie = item.autonomie;
                    test.approcheCommerciale = item.approcheCommerciale;
                    test.discours = item.discours;
                    test.priseConge = item.priseConge;
                    test.identificationClient = item.identificationClient;
                    test.qualificationDemande = item.qualificationDemande;
                    test.dateTempEvaluation = item.dateTempEvaluation;
                    test.type = item.type;
                    test.note = item.note;
                    test.commentaireQualite = item.commentaireQualite;
                    test.commentaireAgent = item.commentaireAgent;
                    test.enregistrementFullName = item.enregistrementFullName;
                    test.enregistrementUrl = item.enregistrementUrl;
                    test.enregistrementDirectory = item.enregistrementDirectory;

                    if (item.employeeId != null)
                    {
                        test.employeeId = item.employeeId;
                        Employee reciever = serviceEmployee.getById(item.employeeId);
                        test.agentName = reciever.UserName;
                    }

                    a.Add(test);
                }

                if (Request.IsAjaxRequest())
                {
                    return PartialView("ArchiveFOQualitePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("ArchiveFOSCGISI");
        }

       
        public ActionResult GetArchiveQualiteFOByDate(string username, string dateDebut, string dateFin)
        {
            List<EvaluationSCGISIViewModel> a = new List<EvaluationSCGISIViewModel>();
            try
            {
                Employee emp = serviceEmployee.getByLogin(username);
                DateTime daterecherchedebut = DateTime.Parse(dateDebut);
                DateTime daterecherchefin = DateTime.Parse(dateFin);
                var historstions = service.GetEvaluationsSenderBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationSCGISIViewModel test = new EvaluationSCGISIViewModel();
                    test.Id = item.Id;
                    test.accueil = item.accueil;
                    test.analyseDemande = item.analyseDemande;
                    test.maitriseOutils = item.maitriseOutils;
                    test.connaissanceProduit = item.connaissanceProduit;
                    test.respectProcess = item.respectProcess;
                    test.pertinenceReponse = item.pertinenceReponse;
                    test.autonomie = item.autonomie;
                    test.approcheCommerciale = item.approcheCommerciale;
                    test.discours = item.discours;
                    test.priseConge = item.priseConge;
                    test.identificationClient = item.identificationClient;
                    test.qualificationDemande = item.qualificationDemande;
                    test.dateTempEvaluation = item.dateTempEvaluation;
                    test.type = item.type;
                    test.note = item.note;
                    test.commentaireQualite = item.commentaireQualite;
                    test.commentaireAgent = item.commentaireAgent;
                    test.enregistrementFullName = item.enregistrementFullName;
                    test.enregistrementUrl = item.enregistrementUrl;
                    test.enregistrementDirectory = item.enregistrementDirectory;

                    if (item.employeeId != null)
                    {
                        test.employeeId = item.employeeId;
                        Employee reciever = serviceEmployee.getById(item.employeeId);
                        test.agentName = reciever.UserName;
                    }

                    a.Add(test);
                }

                if (Request.IsAjaxRequest())
                {
                    return PartialView("ArchiveFOQualitePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("ArchiveFOSCGISI");
        }

        public ActionResult ArchiveBOSCGISI()
        {
            EvaluationSCGISIViewModel evaluation = new EvaluationSCGISIViewModel();
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            var logins = serviceEmployee.GetAll();
            var employees = logins.Select(o => o).Distinct().ToList();

            foreach (var test in employees)
            {
                if (test.Roles.Any(r => r.UserId == test.Id && r.RoleId == 4 || r.RoleId == 8 || r.RoleId == 9))
                {
                    evaluation.employees.Add(new SelectListItem { Text = test.UserName, Value = test.UserName });

                }
            }
            Employee employee = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            evaluation.empId = "" + employee.Id;

            evaluation.userName = employee.UserName;
            evaluation.pseudoNameEmp = employee.pseudoName;
            if (employee.Content != null)
            {
                String strbase64 = Convert.ToBase64String(employee.Content);
                String Url = "data:" + employee.ContentType + ";base64," + strbase64;
                ViewBag.url = Url;
                evaluation.Url = Url;

            }
            return View(evaluation);
        }

        public ActionResult GetArchiveQualiteBO(string username)
        {
            List<EvaluationSCGISIViewModel> a = new List<EvaluationSCGISIViewModel>();
            try
            {
                Employee emp = serviceEmployee.getByLogin(username);
                var historstions = serviceBO.GetEvaluationsBySenderId(emp.Id);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationSCGISIViewModel test = new EvaluationSCGISIViewModel();
                    test.Id = item.Id;
                    test.identificationClient = item.identificationClient;
                    test.qualificationDemande = item.qualificationDemande;
                    test.respectProcess = item.respectProcess;
                    test.pertinenceReponse = item.pertinenceReponse;
                    test.discours = item.discours;
                    test.approcheCommerciale = item.approcheCommerciale;                  
                    test.dateTempEvaluation = item.dateTempEvaluation;
                    test.numIntervention = item.numIntervention;
                    test.type = item.type;
                    test.note = item.note;
                    test.commentaireQualite = item.commentaireQualite;
                    test.commentaireAgent = item.commentaireAgent;
                    test.enregistrementFullName = item.enregistrementFullName;
                    test.enregistrementUrl = item.enregistrementUrl;
                    test.enregistrementDirectory = item.enregistrementDirectory;

                    if (item.employeeId != null)
                    {
                        test.employeeId = item.employeeId;
                        Employee reciever = serviceEmployee.getById(item.employeeId);
                        test.agentName = reciever.UserName;
                    }

                    a.Add(test);
                }

                if (Request.IsAjaxRequest())
                {
                    return PartialView("ArchiveBOQualitePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("ArchiveFOSCGISI");
        }


        public ActionResult GetArchiveQualiteBOByDate(string username, string dateDebut, string dateFin)
        {
            List<EvaluationSCGISIViewModel> a = new List<EvaluationSCGISIViewModel>();
            try
            {
                Employee emp = serviceEmployee.getByLogin(username);
                DateTime daterecherchedebut = DateTime.Parse(dateDebut);
                DateTime daterecherchefin = DateTime.Parse(dateFin);
                var historstions = serviceBO.GetEvaluationsSenderBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationSCGISIViewModel test = new EvaluationSCGISIViewModel();
                    test.Id = item.Id;
                    test.identificationClient = item.identificationClient;
                    test.qualificationDemande = item.qualificationDemande;
                    test.respectProcess = item.respectProcess;
                    test.pertinenceReponse = item.pertinenceReponse;
                    test.discours = item.discours;
                    test.approcheCommerciale = item.approcheCommerciale;
                    test.dateTempEvaluation = item.dateTempEvaluation;
                    test.numIntervention = item.numIntervention;
                    test.type = item.type;
                    test.note = item.note;
                    test.commentaireQualite = item.commentaireQualite;
                    test.commentaireAgent = item.commentaireAgent;
                    test.enregistrementFullName = item.enregistrementFullName;
                    test.enregistrementUrl = item.enregistrementUrl;
                    test.enregistrementDirectory = item.enregistrementDirectory;

                    if (item.employeeId != null)
                    {
                        test.employeeId = item.employeeId;
                        Employee reciever = serviceEmployee.getById(item.employeeId);
                        test.agentName = reciever.UserName;
                    }

                    a.Add(test);
                }

                if (Request.IsAjaxRequest())
                {
                    return PartialView("ArchiveBOQualitePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("ArchiveBOSCGISI");
        }
        #endregion


        #region Archive Front/Back Agent Qualité_CustomerService

        [Authorize(Roles = "Agent Qualité_Diffusion")]
        public ActionResult ArchiveAgentQualiteFOSCGISI()
        {
            EvaluationSCGISIViewModel evaluation = new EvaluationSCGISIViewModel();

            Employee employee = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            evaluation.empId = "" + employee.Id;

            evaluation.userName = employee.UserName;
            evaluation.pseudoNameEmp = employee.pseudoName;
            if (employee.Content != null)
            {
                String strbase64 = Convert.ToBase64String(employee.Content);
                String Url = "data:" + employee.ContentType + ";base64," + strbase64;
                ViewBag.url = Url;
                evaluation.Url = Url;

            }
            return View(evaluation);
        }
        public ActionResult GetArchiveAgentQualiteFO()
        {
            List<EvaluationSCGISIViewModel> a = new List<EvaluationSCGISIViewModel>();
            try
            {
                Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

                var historstions = service.GetEvaluationsBySenderId(emp.Id);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationSCGISIViewModel test = new EvaluationSCGISIViewModel();
                    test.accueil = item.accueil;
                    test.analyseDemande = item.analyseDemande;
                    test.maitriseOutils = item.maitriseOutils;
                    test.connaissanceProduit = item.connaissanceProduit;
                    test.respectProcess = item.respectProcess;
                    test.pertinenceReponse = item.pertinenceReponse;
                    test.autonomie = item.autonomie;
                    test.approcheCommerciale = item.approcheCommerciale;
                    test.discours = item.discours;
                    test.priseConge = item.priseConge;
                    test.identificationClient = item.identificationClient;
                    test.qualificationDemande = item.qualificationDemande;
                    test.dateTempEvaluation = item.dateTempEvaluation;
                    test.type = item.type;
                    test.note = item.note;
                    test.commentaireQualite = item.commentaireQualite;
                    test.commentaireAgent = item.commentaireAgent;
                    test.enregistrementFullName = item.enregistrementFullName;
                    test.enregistrementUrl = item.enregistrementUrl;
                    test.enregistrementDirectory = item.enregistrementDirectory;
                    if (item.senderId != null)
                    {
                        test.senderId = item.senderId;
                        Employee sender = serviceEmployee.getById(item.senderId);
                        test.senderName = sender.UserName;
                    }
                    if (item.employeeId != null)
                    {
                        test.employeeId = item.employeeId;
                        Employee receiver = serviceEmployee.getById(item.employeeId);
                        test.agentName = receiver.UserName;
                    }

                    a.Add(test);
                }

                if (Request.IsAjaxRequest())
                {
                    return PartialView("ArchiveFOQualitePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("ArchiveAgentQualiteFOSCGISI");
        }

        public ActionResult GetArchiveAgentQualiteFOByDate(string dateDebut, string dateFin)
        {
            List<EvaluationSCGISIViewModel> a = new List<EvaluationSCGISIViewModel>();
            try
            {
                Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
                DateTime daterecherchedebut = DateTime.Parse(dateDebut);
                DateTime daterecherchefin = DateTime.Parse(dateFin);
                var historstions = service.GetEvaluationsSenderBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationSCGISIViewModel test = new EvaluationSCGISIViewModel();
                    test.accueil = item.accueil;
                    test.analyseDemande = item.analyseDemande;
                    test.maitriseOutils = item.maitriseOutils;
                    test.connaissanceProduit = item.connaissanceProduit;
                    test.respectProcess = item.respectProcess;
                    test.pertinenceReponse = item.pertinenceReponse;
                    test.autonomie = item.autonomie;
                    test.approcheCommerciale = item.approcheCommerciale;
                    test.discours = item.discours;
                    test.priseConge = item.priseConge;
                    test.identificationClient = item.identificationClient;
                    test.qualificationDemande = item.qualificationDemande;
                    test.dateTempEvaluation = item.dateTempEvaluation;
                    test.type = item.type;
                    test.note = item.note;
                    test.commentaireQualite = item.commentaireQualite;
                    test.commentaireAgent = item.commentaireAgent;
                    test.enregistrementFullName = item.enregistrementFullName;
                    test.enregistrementUrl = item.enregistrementUrl;
                    test.enregistrementDirectory = item.enregistrementDirectory;

                    if (item.senderId != null)
                    {
                        test.senderId = item.senderId;
                        Employee sender = serviceEmployee.getById(item.senderId);
                        test.senderName = sender.UserName;
                    }
                    if (item.employeeId != null)
                    {
                        test.employeeId = item.employeeId;
                        Employee receiver = serviceEmployee.getById(item.employeeId);
                        test.agentName = receiver.UserName;
                    }
                    a.Add(test);
                }

                if (Request.IsAjaxRequest())
                {
                    return PartialView("ArchiveFOQualitePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("ArchiveAgentQualiteFOSCGISI");
        }

        public ActionResult ArchiveAgentQualiteBOSCGISI()
        {
            EvaluationSCGISIViewModel evaluation = new EvaluationSCGISIViewModel();

            Employee employee = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            evaluation.empId = "" + employee.Id;

            evaluation.userName = employee.UserName;
            evaluation.pseudoNameEmp = employee.pseudoName;
            if (employee.Content != null)
            {
                String strbase64 = Convert.ToBase64String(employee.Content);
                String Url = "data:" + employee.ContentType + ";base64," + strbase64;
                ViewBag.url = Url;
                evaluation.Url = Url;

            }
            return View(evaluation);
        }
        public ActionResult GetArchiveAgentQualiteBO()
        {
            List<EvaluationSCGISIViewModel> a = new List<EvaluationSCGISIViewModel>();
            try
            {
                Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

                var historstions = serviceBO.GetEvaluationsBySenderId(emp.Id);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationSCGISIViewModel test = new EvaluationSCGISIViewModel();
                    test.identificationClient = item.identificationClient;
                    test.qualificationDemande = item.qualificationDemande;
                    test.respectProcess = item.respectProcess;
                    test.pertinenceReponse = item.pertinenceReponse;
                    test.discours = item.discours;
                    test.approcheCommerciale = item.approcheCommerciale;                 
                    test.dateTempEvaluation = item.dateTempEvaluation;
                    test.numIntervention = item.numIntervention;
                    test.type = item.type;
                    test.note = item.note;
                    test.commentaireQualite = item.commentaireQualite;
                    test.commentaireAgent = item.commentaireAgent;
                    test.enregistrementFullName = item.enregistrementFullName;
                    test.enregistrementUrl = item.enregistrementUrl;
                    test.enregistrementDirectory = item.enregistrementDirectory;
                    if (item.senderId != null)
                    {
                        test.senderId = item.senderId;
                        Employee sender = serviceEmployee.getById(item.senderId);
                        test.senderName = sender.UserName;
                    }
                    if (item.employeeId != null)
                    {
                        test.employeeId = item.employeeId;
                        Employee receiver = serviceEmployee.getById(item.employeeId);
                        test.agentName = receiver.UserName;
                    }

                    a.Add(test);
                }

                if (Request.IsAjaxRequest())
                {
                    return PartialView("ArchiveBOQualitePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("ArchiveAgentQualiteBOSCGISI");
        }

        public ActionResult GetArchiveAgentQualiteBOByDate(string dateDebut, string dateFin)
        {
            List<EvaluationSCGISIViewModel> a = new List<EvaluationSCGISIViewModel>();
            try
            {
                Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
                DateTime daterecherchedebut = DateTime.Parse(dateDebut);
                DateTime daterecherchefin = DateTime.Parse(dateFin);
                var historstions = serviceBO.GetEvaluationsSenderBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationSCGISIViewModel test = new EvaluationSCGISIViewModel();
                    test.identificationClient = item.identificationClient;
                    test.qualificationDemande = item.qualificationDemande;
                    test.respectProcess = item.respectProcess;
                    test.pertinenceReponse = item.pertinenceReponse;
                    test.discours = item.discours;
                    test.approcheCommerciale = item.approcheCommerciale;             
                    test.dateTempEvaluation = item.dateTempEvaluation;
                    test.numIntervention = item.numIntervention;
                    test.type = item.type;
                    test.note = item.note;
                    test.commentaireQualite = item.commentaireQualite;
                    test.commentaireAgent = item.commentaireAgent;
                    test.enregistrementFullName = item.enregistrementFullName;
                    test.enregistrementUrl = item.enregistrementUrl;
                    test.enregistrementDirectory = item.enregistrementDirectory;

                    if (item.senderId != null)
                    {
                        test.senderId = item.senderId;
                        Employee sender = serviceEmployee.getById(item.senderId);
                        test.senderName = sender.UserName;
                    }
                    if (item.employeeId != null)
                    {
                        test.employeeId = item.employeeId;
                        Employee receiver = serviceEmployee.getById(item.employeeId);
                        test.agentName = receiver.UserName;
                    }
                    a.Add(test);
                }

                if (Request.IsAjaxRequest())
                {
                    return PartialView("ArchiveBOQualitePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("ArchiveAgentQualiteBOSCGISI");
        }
        #endregion

        #region Edit et delete FO SCGISI
        [Authorize(Roles = "Qualité")]

        public ActionResult EditFOSCGISI(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GrilleEvaluationFO evaluation = db.GrilleEvaluationFOSCGISI.Find(id);
           
            var emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

            if (emp.Content != null)
            {
                String strbase64 = Convert.ToBase64String(emp.Content);
                String empConnectedImage = "data:" + emp.ContentType + ";base64," + strbase64;
                ViewBag.empConnectedImage = empConnectedImage;
            }
            ViewBag.nameEmpConnected = emp.UserName;
            ViewBag.pseudoNameEmpConnected = emp.pseudoName;
            return View(evaluation);
        }
        // POST: Evaluation/Edit/5
        [HttpPost, ActionName("EditFOSCGISI")]
        [Authorize(Roles = "Qualité")]
        public ActionResult EditFOSCGISI(int? id, GrilleEvaluationFO evaluation)
        {
            float total = 47;
            List<float> NEList = new List<float>(new float[] { evaluation.connaissanceProduit, evaluation.approcheCommerciale});

            float notes = evaluation.accueil + evaluation.analyseDemande + evaluation.maitriseOutils +
              evaluation.respectProcess + evaluation.pertinenceReponse + evaluation.autonomie +
             evaluation.discours + evaluation.priseConge + evaluation.identificationClient + evaluation.qualificationDemande;

            foreach (var i in NEList)
            {
                if (i < 0)
                {
                    total += i;
                }
                else
                {
                    notes += i;
                }
            }
            if (ModelState.IsValid)
            {
                evaluation.note = (notes / total) * 100;
                db.Entry(evaluation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("HistoriqueFOSCGISI");
            }
            return View(evaluation);
        }
        [Authorize(Roles = "Qualité")]
        [HttpGet]
        public ActionResult FindEvaluationFOSCGISI(int? Id)
        {
            GrilleEvaluationFO item = service.getById(Id);

            if (Request.IsAjaxRequest())
            {
                return PartialView("_AlerteDeSuppressionFOSCGIGI", item);
            }

            else
            {
                return View(item);
            }
        }
        [Authorize(Roles = "Qualité")]
        public ActionResult DeleteFOSCGISI(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GrilleEvaluationFO evaluation = service.getById(id);
            int? empId = evaluation.employeeId;
           service.DeleteEvaluations(id, empId);
            service.SaveChange();
            return RedirectToAction("HistoriqueFOSCGISI");
        }
        #endregion

        #region Edit et delete BO SCGISI
        [Authorize(Roles = "Qualité")]

        public ActionResult EditBOSCGISI(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GrilleEvaluationBO evaluation = db.GrilleEvaluationBOSCGISI.Find(id);

            var emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

            if (emp.Content != null)
            {
                String strbase64 = Convert.ToBase64String(emp.Content);
                String empConnectedImage = "data:" + emp.ContentType + ";base64," + strbase64;
                ViewBag.empConnectedImage = empConnectedImage;
            }
            ViewBag.nameEmpConnected = emp.UserName;
            ViewBag.pseudoNameEmpConnected = emp.pseudoName;
            return View(evaluation);
        }
        // POST: Evaluation/Edit/5
        [HttpPost, ActionName("EditBOSCGISI")]
        [Authorize(Roles = "Qualité")]
        public ActionResult EditBOSCGISI(int? id, GrilleEvaluationBO evaluation)
        {
            float total = 31;
            List<float> NEList = new List<float>(new float[] {evaluation.approcheCommerciale });

            float notes = evaluation.respectProcess + evaluation.pertinenceReponse + evaluation.discours +
             evaluation.identificationClient + evaluation.qualificationDemande;

            foreach (var i in NEList)
            {
                if (i < 0)
                {
                    total += i;
                }
                else
                {
                    notes += i;
                }
            }
            if (ModelState.IsValid)
            {
                evaluation.note = (notes / total) * 100;
                db.Entry(evaluation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("HistoriqueBOSCGISI");
            }
            return View(evaluation);
        }
        [Authorize(Roles = "Qualité")]
        [HttpGet]
        public ActionResult FindEvaluationBOSCGISI(int? Id)
        {
            GrilleEvaluationBO item = serviceBO.getById(Id);

            if (Request.IsAjaxRequest())
            {
                return PartialView("_AlerteDeSuppressionBOSCGIGI", item);
            }

            else
            {
                return View(item);
            }
        }
        [Authorize(Roles = "Qualité")]
        public ActionResult DeleteBOSCGISI(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GrilleEvaluationBO evaluation = serviceBO.getById(id);
            int? empId = evaluation.employeeId;
            serviceBO.DeleteEvaluations(id, empId);
            serviceBO.SaveChange();
            return RedirectToAction("HistoriqueBOSCGISI");
        }
        #endregion

        #region Reports Front Office SC GISI 
        public ActionResult ReportsFOSCGISI()
        {
            List<Employee> logins = new List<Employee>();
            EvaluationViewModel evaluation = new EvaluationViewModel();
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 9))
            {
                ViewBag.role = "Agent Qualité_Diffusion";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 4))
            {
                ViewBag.role = "Qualité";
            }
           
            string d = "CUSTOMER SERVICE";
            Groupe gr = serviceGroupe.getByNom(d);
            List<Employee> emp = serviceGroupeEmp.getListEmployeeByGroupeId(gr.Id);
            foreach (var e in emp)
            {
                if (!logins.Exists(l => l.UserName == e.UserName))
                {
                    logins.Add(e);
                }
            }
            var employees = logins.OrderBy(a => a.UserName).ToList();
            foreach (var test in employees)
            {
                if (!test.UserName.Equals(user.UserName) && test.Roles.Any(r => r.UserId == test.Id && r.RoleId == 3))
                {
                    evaluation.employees.Add(new SelectListItem { Text = test.UserName, Value = test.UserName });

                }
            }

            Employee employee = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            evaluation.empId = "" + employee.Id;

            evaluation.userName = employee.UserName;
            evaluation.pseudoNameEmp = employee.pseudoName;
            if (employee.Content != null)
            {
                String strbase64 = Convert.ToBase64String(employee.Content);
                String Url = "data:" + employee.ContentType + ";base64," + strbase64;
                ViewBag.url = Url;
                evaluation.Url = Url;

            }
            return View(evaluation);
        }
        public JsonResult GetReportsFOSCGISI(string username, string dateDebut, string dateFin)
        {
            float totAccueil = 0, totAnalyseDemande = 0, totMaitriseOutils = 0, totConnaissanceProduit = 0, totRespectProcess = 0,
                totPertinenceReponse = 0, totAutonomie = 0, totApprocheCommerciale = 0, totIdentificationClient = 0, totQualificationDemande = 0,
                totDiscours = 0, totPriseCongé = 0, totNotes = 0;

            float  NbreConnaissanceProduit = 0, NbreApprocheCommerciale = 0;

            DateTime daterecherchedebut = DateTime.Parse(dateDebut);
            DateTime daterecherchefin = DateTime.Parse(dateFin);

            var historstions = service.GetEvaluationsBetweenTwoDates(daterecherchedebut, daterecherchefin);
            if (username != "")
            {
                Employee emp = serviceEmployee.getByLogin(username);
                historstions = service.GetEvaluationsEmployeeBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
            }
            float nbreEvaluations = historstions.Count();
            ViewBag.nbreEvaluations = nbreEvaluations;
            foreach (var item in historstions)
            {
                totNotes += item.note;
                totAccueil += item.accueil;
                totAnalyseDemande += item.analyseDemande;
                totMaitriseOutils += item.maitriseOutils;
                if (item.connaissanceProduit >= 0)
                {
                    totConnaissanceProduit += item.connaissanceProduit;
                    NbreConnaissanceProduit += 1;
                }
                if (item.approcheCommerciale >= 0)
                {
                    totApprocheCommerciale += item.approcheCommerciale;
                    NbreApprocheCommerciale += 1;
                }
                totRespectProcess += item.respectProcess;
                totPertinenceReponse += item.pertinenceReponse;
                totAutonomie += item.autonomie;
                totDiscours += item.discours;
                totPriseCongé += item.priseConge;
                totIdentificationClient += item.identificationClient;
                totQualificationDemande += item.qualificationDemande;              
            }

            EvaluationSCGISIViewModel test = new EvaluationSCGISIViewModel();

            test.nbreEvaluations = Convert.ToInt32(nbreEvaluations);
            if (NbreConnaissanceProduit != 0)
            {
                test.connaissanceProduit = (float)Math.Round((totConnaissanceProduit / (NbreConnaissanceProduit * 3)) * 100, 2);
            }
            else { test.connaissanceProduit = -3; }
            if (NbreApprocheCommerciale != 0)
            {
                test.approcheCommerciale = (float)Math.Round((totApprocheCommerciale / (NbreApprocheCommerciale * 4)) * 100, 2);
            }
            else { test.approcheCommerciale = -4; }
           

            if (nbreEvaluations != 0)
            {
                test.note = (float)Math.Round((totNotes / (nbreEvaluations * 100)) * 100, 2);
                test.accueil = (float)Math.Round((totAccueil / (nbreEvaluations * 2)) * 100, 2);
                test.analyseDemande = (float)Math.Round((totAnalyseDemande / (nbreEvaluations * 3)) * 100, 2);
                test.maitriseOutils = (float)Math.Round((totMaitriseOutils / (nbreEvaluations * 4)) * 100, 2);
                test.respectProcess = (float)Math.Round((totRespectProcess / (nbreEvaluations * 6)) * 100, 2);
                test.pertinenceReponse = (float)Math.Round((totPertinenceReponse / (nbreEvaluations * 6)) * 100, 2);
                test.autonomie = (float)Math.Round((totAutonomie / (nbreEvaluations * 3)) * 100, 2);
                test.discours = (float)Math.Round((totDiscours / (nbreEvaluations * 4)) * 100, 2);               
                test.priseConge = (float)Math.Round((totPriseCongé / (nbreEvaluations * 2)) * 100, 2);
                test.identificationClient = (float)Math.Round((totIdentificationClient / (nbreEvaluations * 5)) * 100, 2);
                test.qualificationDemande = (float)Math.Round((totQualificationDemande / (nbreEvaluations * 5)) * 100, 2);
            }
            else
            {
                test.note = 0;
                test.accueil = 0;
                test.analyseDemande = 0;
                test.maitriseOutils = 0;
                test.connaissanceProduit = 0;
                test.respectProcess = 0;
                test.pertinenceReponse = 0;
                test.autonomie = 0;
                test.approcheCommerciale = 0;
                test.discours = 0;
                test.priseConge = 0;
                test.identificationClient = 0;
                test.qualificationDemande = 0;
            }
            return Json(test, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Reports Back Office SC GISI 
        public ActionResult ReportsBOSCGISI()
        {
            List<Employee> logins = new List<Employee>();
            EvaluationViewModel evaluation = new EvaluationViewModel();
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 9))
            {
                ViewBag.role = "Agent Qualité_Diffusion";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 4))
            {
                ViewBag.role = "Qualité";
            }

            string d = "CUSTOMER SERVICE";
            Groupe gr = serviceGroupe.getByNom(d);
            List<Employee> emp = serviceGroupeEmp.getListEmployeeByGroupeId(gr.Id);
            foreach (var e in emp)
            {
                if (!logins.Exists(l => l.UserName == e.UserName))
                {
                    logins.Add(e);
                }
            }
            var employees = logins.OrderBy(a => a.UserName).ToList();
            foreach (var test in employees)
            {
                if (!test.UserName.Equals(user.UserName) && test.Roles.Any(r => r.UserId == test.Id && r.RoleId == 3))
                {
                    evaluation.employees.Add(new SelectListItem { Text = test.UserName, Value = test.UserName });

                }
            }

            Employee employee = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            evaluation.empId = "" + employee.Id;

            evaluation.userName = employee.UserName;
            evaluation.pseudoNameEmp = employee.pseudoName;
            if (employee.Content != null)
            {
                String strbase64 = Convert.ToBase64String(employee.Content);
                String Url = "data:" + employee.ContentType + ";base64," + strbase64;
                ViewBag.url = Url;
                evaluation.Url = Url;

            }
            return View(evaluation);
        }
        public JsonResult GetReportsBOSCGISI(string username, string dateDebut, string dateFin)
        {
            float totIdentificationClient = 0, totQualificationDemande = 0, totRespectProcess = 0,
                totPertinenceReponse = 0, totApprocheCommerciale = 0, totDiscours = 0, totNotes = 0;

            float NbreApprocheCommerciale = 0;

            DateTime daterecherchedebut = DateTime.Parse(dateDebut);
            DateTime daterecherchefin = DateTime.Parse(dateFin);

            var historstions = serviceBO.GetEvaluationsBetweenTwoDates(daterecherchedebut, daterecherchefin);
            if (username != "")
            {
                Employee emp = serviceEmployee.getByLogin(username);
                historstions = serviceBO.GetEvaluationsEmployeeBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
            }
            float nbreEvaluations = historstions.Count();
            ViewBag.nbreEvaluations = nbreEvaluations;
            foreach (var item in historstions)
            {
                totNotes += item.note;
              
                if (item.approcheCommerciale >= 0)
                {
                    totApprocheCommerciale += item.approcheCommerciale;
                    NbreApprocheCommerciale += 1;
                }
                totIdentificationClient += item.identificationClient;
                totQualificationDemande += item.qualificationDemande;
                totRespectProcess += item.respectProcess;
                totPertinenceReponse += item.pertinenceReponse;
                totDiscours += item.discours;
          
            }

            EvaluationSCGISIViewModel test = new EvaluationSCGISIViewModel();

            test.nbreEvaluations = Convert.ToInt32(nbreEvaluations);
        
            if (NbreApprocheCommerciale != 0)
            {
                test.approcheCommerciale = (float)Math.Round((totApprocheCommerciale / (NbreApprocheCommerciale * 4)) * 100, 2);
            }
            else { test.approcheCommerciale = -4; }


            if (nbreEvaluations != 0)
            {
                test.note = (float)Math.Round((totNotes / (nbreEvaluations * 100)) * 100, 2);
                test.identificationClient = (float)Math.Round((totIdentificationClient / (nbreEvaluations * 5)) * 100, 2);
                test.qualificationDemande = (float)Math.Round((totQualificationDemande / (nbreEvaluations * 5)) * 100, 2);
                test.respectProcess = (float)Math.Round((totRespectProcess / (nbreEvaluations * 6)) * 100, 2);
                test.pertinenceReponse = (float)Math.Round((totPertinenceReponse / (nbreEvaluations * 6)) * 100, 2);
                test.discours = (float)Math.Round((totDiscours / (nbreEvaluations * 5)) * 100, 2);
              
            }
            else
            {
                test.note = 0;
                test.identificationClient = 0;
                test.qualificationDemande = 0;
                test.respectProcess = 0;
                test.pertinenceReponse = 0;
                test.approcheCommerciale = 0;
                test.discours = 0;
            
            }
            return Json(test, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Ecouter et Details
        [HttpGet]
        public ActionResult FindCustomEnregistrement(String name, String fullName, String DirectoryName)
        {
            var b = new DirectoryViewModel();

            try
            {

                if (Request.IsAjaxRequest())
                {

                    b.pseudoName = name;
                    b.enregistrementName = fullName;
                    b.directoryName = DirectoryName;
                    var fileInfo = new FileInfo(@name);

                    System.IO.File.Copy(fullName, Server.MapPath("~/Files/") + name, true);
                    // player2 = new WaveOutEvent();
                    return PartialView("CustomEnregistrementEval", b);
                }
            }
            catch (Exception)
            {

                return PartialView("CustomEnregistrementEval", b);
            }
            return RedirectToAction("Index");

        }

        public ActionResult FindComments(String CommentQualite, String CommentAgent)
        {
            var b = new EvaluationViewModel();

            try
            {

                if (Request.IsAjaxRequest())
                {

                    b.commentaireQualite = CommentQualite;
                    b.commentaireAgent = CommentAgent;

                    return PartialView("CommentsEvaluation", b);
                }
            }
            catch (Exception)
            {

                return PartialView("CommentsEvaluation", b);
            }
            return RedirectToAction("Historique");
        }
        #endregion
     
    }
}
