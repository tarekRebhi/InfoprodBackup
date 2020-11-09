using Domain.Entity;
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
    [Authorize(Roles = "Qualité,Agent Qualité_CustomerService,Agent_CustomerService,Agent_SAMRC")]
    public class EvaluationServiceClientAutoController : Controller
    {
        private ReportContext db = new ReportContext();
        // GET: Evaluation
        #region globalVariable
        IEmployeeService serviceEmployee;
        IGrilleEvaluationService serviceDiff;
        IEvaluationEnqueteAutoService service;
        IEvaluationFOSAMRCService serviceFOSAMRCS;
        IEvaluationBOSAMRCService serviceBOSAMRC;
        IGroupeEmployeeService serviceGroupeEmp;
        IGroupeService serviceGroupe;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        #endregion
        #region constructor
        public EvaluationServiceClientAutoController()
        {
            serviceEmployee = new EmployeeService();
            serviceDiff = new GrilleEvaluationService();
            service = new EvaluationEnqueteAutoService();
            serviceFOSAMRCS = new EvaluationFOSAMRCService();
            serviceBOSAMRC = new EvaluationBOSAMRCService();
             serviceGroupeEmp = new GroupesEmployeService();
            serviceGroupe = new GroupeService();
        }
        public EvaluationServiceClientAutoController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
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

        #region Evaluation Enquete Auto 
        [Authorize(Roles = "Qualité, Agent Qualité_CustomerService")]

        public ActionResult EnqueteAuto(string enregistrementFullName, string enregistrementUrl, string enregistrementDirectory, string agentName)
        {

            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 8))
            {
                ViewBag.role = "Agent Qualité_CustomerService";
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

        public ActionResult EnqueteAutoWithoutAgent(string enregistrementFullName, string enregistrementUrl, string enregistrementDirectory, string siteName)
        {
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            var a = new EvaluationViewModel();
            List<SelectListItem> agents = new List<SelectListItem>();
            List<Employee> logins = new List<Employee>();
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 8))
            {
                ViewBag.role = "Agent Qualité_CustomerService";
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
        public ActionResult CalculEnqueteAuto(string nomAgent, string planDate, string presentation, string respectScript, string traitementObjections, string exploitationOutils, string HistorisationOutils, string discours, string debit, string intonation, string ecouteReformulation, string priseConge, string commentaire, string enregistrement, string enregistrementUrl, string enregistrementDirectory)
        {
            float total = 71;
            List<string> NEList = new List<string>(new string[] { presentation, respectScript, traitementObjections, exploitationOutils, HistorisationOutils, discours, debit, intonation, ecouteReformulation, priseConge });
            float notes = 0;

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
            var a = new EvaluationEvaluationSCAutoViewModel();
            a.presentation = float.Parse(presentation, CultureInfo.InvariantCulture.NumberFormat);
            a.respectScript = float.Parse(respectScript, CultureInfo.InvariantCulture.NumberFormat);
            a.traitementObjections = float.Parse(traitementObjections, CultureInfo.InvariantCulture.NumberFormat);
            a.exploitationOutils = float.Parse(exploitationOutils, CultureInfo.InvariantCulture.NumberFormat);
            a.HistorisationOutils = float.Parse(HistorisationOutils, CultureInfo.InvariantCulture.NumberFormat);
            a.discours = float.Parse(discours, CultureInfo.InvariantCulture.NumberFormat);
            a.debit = float.Parse(debit, CultureInfo.InvariantCulture.NumberFormat);
            a.intonation = float.Parse(intonation, CultureInfo.InvariantCulture.NumberFormat);
            a.traitementObjections = float.Parse(traitementObjections, CultureInfo.InvariantCulture.NumberFormat);
            a.ecouteReformulation = float.Parse(ecouteReformulation, CultureInfo.InvariantCulture.NumberFormat);
            a.priseConge = float.Parse(priseConge, CultureInfo.InvariantCulture.NumberFormat);
           
            a.dateTempEvaluation = DateTime.Parse(planDate);
            a.agentName = nomAgent;
            a.note = notes;
            a.pourcentageNotes = (notes / total) * 100;
            a.type = "Enquete Auto";

            if (Request.IsAjaxRequest())
            {
                return PartialView("EnqueteAutoResult", a);
            }
            return RedirectToAction("listeSites", "Superviseur");
        }

        public ActionResult SaveEvalEnqueteAuto(string nomAgent, string planDate, string presentation, string respectScript, string traitementObjections, string exploitationOutils, string HistorisationOutils, string discours, string debit, string intonation, string ecouteReformulation, string priseConge, string commentaire, string enregistrement, string enregistrementUrl, string enregistrementDirectory)
        {
            var userConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            Employee emp = serviceEmployee.getByPseudoName(nomAgent.ToLower());
            GrilleEvaluationEnqueteAuto a = new GrilleEvaluationEnqueteAuto();
            float total = 71;
            List<string> NEList = new List<string>(new string[] { presentation, respectScript, traitementObjections, exploitationOutils, HistorisationOutils, discours, debit, intonation, ecouteReformulation, priseConge });
            float notes = 0;

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
            a.presentation = float.Parse(presentation, CultureInfo.InvariantCulture.NumberFormat);
            a.respectScript = float.Parse(respectScript, CultureInfo.InvariantCulture.NumberFormat);
            a.traitementObjections = float.Parse(traitementObjections, CultureInfo.InvariantCulture.NumberFormat);
            a.exploitationOutils = float.Parse(exploitationOutils, CultureInfo.InvariantCulture.NumberFormat);
            a.HistorisationOutils = float.Parse(HistorisationOutils, CultureInfo.InvariantCulture.NumberFormat);
            a.discours = float.Parse(discours, CultureInfo.InvariantCulture.NumberFormat);
            a.debit = float.Parse(debit, CultureInfo.InvariantCulture.NumberFormat);
            a.intonation = float.Parse(intonation, CultureInfo.InvariantCulture.NumberFormat);
            a.traitementObjections = float.Parse(traitementObjections, CultureInfo.InvariantCulture.NumberFormat);
            a.ecouteReformulation = float.Parse(ecouteReformulation, CultureInfo.InvariantCulture.NumberFormat);
            a.priseConge = float.Parse(priseConge, CultureInfo.InvariantCulture.NumberFormat);

            a.dateTempEvaluation = DateTime.Parse(planDate);
            a.employeeId = emp.Id;
            a.senderId = userConnected.Id;
            a.note = (notes / total) * 100;
            a.type = "Enquete Auto";
            a.commentaireQualite = commentaire;
            a.enregistrementFullName = enregistrement;
            a.enregistrementUrl = enregistrementUrl;
            a.enregistrementDirectory = enregistrementDirectory;

            service.Add(a);
            service.SaveChange();
            var eval = new EvaluationEvaluationSCAutoViewModel();
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

        #region Historique Enquete Auto Par Collaborateur
        [Authorize(Roles = "Qualité, Agent Qualité_CustomerService, Manager,SuperManager")]
       
        public ActionResult HistoriqueEnqueteAuto()
        {
            List<Employee> logins = new List<Employee>();
            EvaluationViewModel evaluation = new EvaluationViewModel();
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
                if (!test.UserName.Equals(user.UserName) && test.Roles.Any(r => r.UserId == test.Id && r.RoleId == 3 || r.RoleId == 10 ))
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
        public ActionResult GetHistoEnqueteAuto(string username)
        {
            List<EvaluationEvaluationSCAutoViewModel> a = new List<EvaluationEvaluationSCAutoViewModel>();
            try
            {
                Employee emp = serviceEmployee.getByLogin(username);
                var historstions = service.GetEvaluationsByEmployee(emp.Id);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationEvaluationSCAutoViewModel test = new EvaluationEvaluationSCAutoViewModel();
                    test.Id = item.Id;
                    test.presentation = item.presentation;
                    test.respectScript = item.respectScript;
                    test.traitementObjections = item.traitementObjections;
                    test.exploitationOutils = item.exploitationOutils;
                    test.HistorisationOutils = item.HistorisationOutils;
                    test.discours = item.discours;
                    test.debit = item.debit;
                    test.intonation = item.intonation;
                    test.traitementObjections = item.traitementObjections;
                    test.ecouteReformulation = item.ecouteReformulation;
                    test.priseConge = item.priseConge;
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
                    return PartialView("HistoriqueEnqueteAutoPartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("QRHistorique");
        }

        public ActionResult GetHistoEnqueteAutoByDate(string username, string dateDebut, string dateFin)
        {
            List<EvaluationEvaluationSCAutoViewModel> a = new List<EvaluationEvaluationSCAutoViewModel>();
            try
            {
                Employee emp = serviceEmployee.getByLogin(username);
                DateTime daterecherchedebut = DateTime.Parse(dateDebut);
                DateTime daterecherchefin = DateTime.Parse(dateFin);


                var historstions = service.GetEvaluationsEmployeeBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationEvaluationSCAutoViewModel test = new EvaluationEvaluationSCAutoViewModel();
                    test.Id = item.Id;
                    test.presentation = item.presentation;
                    test.respectScript = item.respectScript;
                    test.traitementObjections = item.traitementObjections;
                    test.exploitationOutils = item.exploitationOutils;
                    test.HistorisationOutils = item.HistorisationOutils;
                    test.discours = item.discours;
                    test.debit = item.debit;
                    test.intonation = item.intonation;
                    test.traitementObjections = item.traitementObjections;
                    test.ecouteReformulation = item.ecouteReformulation;
                    test.priseConge = item.priseConge;
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
                    return PartialView("HistoriqueEnqueteAutoPartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("HistoriqueEnqueteAuto");
        }
        #endregion

        #region Archive Enquete Auto par Resp Qualité
        //Archive evaluations par Responsable Qualité
        [Authorize(Roles = "Qualité")]
        public ActionResult ArchiveEnqueteAuto()
        {
            EvaluationEvaluationSCAutoViewModel evaluation = new EvaluationEvaluationSCAutoViewModel();
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            var logins = serviceEmployee.GetAll();
            var employees = logins.Select(o => o).Distinct().ToList();

            foreach (var test in employees)
            {
                if (test.Roles.Any(r => r.UserId == test.Id && r.RoleId == 4 || r.RoleId == 8))
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

        public ActionResult GetArchiveQualiteEnqueteAuto(string username)
        {
            List<EvaluationEvaluationSCAutoViewModel> a = new List<EvaluationEvaluationSCAutoViewModel>();
            try
            {
                Employee emp = serviceEmployee.getByLogin(username);
                var historstions = service.GetEvaluationsBySenderId(emp.Id);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationEvaluationSCAutoViewModel test = new EvaluationEvaluationSCAutoViewModel();
                    test.Id = item.Id;
                    test.presentation = item.presentation;
                    test.respectScript = item.respectScript;
                    test.traitementObjections = item.traitementObjections;
                    test.exploitationOutils = item.exploitationOutils;
                    test.HistorisationOutils = item.HistorisationOutils;
                    test.discours = item.discours;
                    test.debit = item.debit;
                    test.intonation = item.intonation;
                    test.traitementObjections = item.traitementObjections;
                    test.ecouteReformulation = item.ecouteReformulation;
                    test.priseConge = item.priseConge;
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
                    return PartialView("ArchiveEnqueteAutoQualitePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("ArchiveEnqueteAuto");
        }

        public ActionResult GetArchiveQualiteEnqueteAutoByDate(string username, string dateDebut, string dateFin)
        {
            List<EvaluationEvaluationSCAutoViewModel> a = new List<EvaluationEvaluationSCAutoViewModel>();
            try
            {
                Employee emp = serviceEmployee.getByLogin(username);
                DateTime daterecherchedebut = DateTime.Parse(dateDebut);
                DateTime daterecherchefin = DateTime.Parse(dateFin);
                var historstions = service.GetEvaluationsSenderBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationEvaluationSCAutoViewModel test = new EvaluationEvaluationSCAutoViewModel();
                    test.Id = item.Id;
                    test.presentation = item.presentation;
                    test.respectScript = item.respectScript;
                    test.traitementObjections = item.traitementObjections;
                    test.exploitationOutils = item.exploitationOutils;
                    test.HistorisationOutils = item.HistorisationOutils;
                    test.discours = item.discours;
                    test.debit = item.debit;
                    test.intonation = item.intonation;
                    test.traitementObjections = item.traitementObjections;
                    test.ecouteReformulation = item.ecouteReformulation;
                    test.priseConge = item.priseConge;
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
                    return PartialView("ArchiveEnqueteAutoQualitePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("ArchiveEnqueteAuto");
        }

        #endregion

        #region Archive Enquete Auto  Agent Qualité_CustomerService
        [Authorize(Roles = "Agent Qualité_CustomerService")]
        public ActionResult ArchiveAgentQualiteEnqueteAuto()
        {
            EvaluationEvaluationSCAutoViewModel evaluation = new EvaluationEvaluationSCAutoViewModel();

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
        public ActionResult GetArchiveAgentQualiteEnqueteAuto()
        {
            List<EvaluationEvaluationSCAutoViewModel> a = new List<EvaluationEvaluationSCAutoViewModel>();
            try
            { 
                Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

                var historstions = service.GetEvaluationsBySenderId(emp.Id);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationEvaluationSCAutoViewModel test = new EvaluationEvaluationSCAutoViewModel();
                    test.Id = item.Id;
                    test.presentation = item.presentation;
                    test.respectScript = item.respectScript;
                    test.traitementObjections = item.traitementObjections;
                    test.exploitationOutils = item.exploitationOutils;
                    test.HistorisationOutils = item.HistorisationOutils;
                    test.discours = item.discours;
                    test.debit = item.debit;
                    test.intonation = item.intonation;
                    test.traitementObjections = item.traitementObjections;
                    test.ecouteReformulation = item.ecouteReformulation;
                    test.priseConge = item.priseConge;
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
                    return PartialView("ArchiveEnqueteAutoQualitePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("ArchiveAgentQualiteEnqueteAuto");
        }

        public ActionResult GetArchiveAgentQualiteEnqueteAutoByDate(string dateDebut, string dateFin)
        {
            List<EvaluationEvaluationSCAutoViewModel> a = new List<EvaluationEvaluationSCAutoViewModel>();
            try
            {
                Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
                DateTime daterecherchedebut = DateTime.Parse(dateDebut);
                DateTime daterecherchefin = DateTime.Parse(dateFin);
                var historstions = service.GetEvaluationsSenderBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationEvaluationSCAutoViewModel test = new EvaluationEvaluationSCAutoViewModel();
                    test.Id = item.Id;
                    test.presentation = item.presentation;
                    test.respectScript = item.respectScript;
                    test.traitementObjections = item.traitementObjections;
                    test.exploitationOutils = item.exploitationOutils;
                    test.HistorisationOutils = item.HistorisationOutils;
                    test.discours = item.discours;
                    test.debit = item.debit;
                    test.intonation = item.intonation;
                    test.traitementObjections = item.traitementObjections;
                    test.ecouteReformulation = item.ecouteReformulation;
                    test.priseConge = item.priseConge;
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
                    return PartialView("ArchiveEnqueteAutoQualitePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("ArchiveAgentQualiteEnqueteAuto");
        }
        #endregion

        #region Evaluation SC SAM/RC Auto Front office 
        [Authorize(Roles = "Qualité, Agent Qualité_CustomerService")]

        public ActionResult SAMRCFOAuto(string enregistrementFullName, string enregistrementUrl, string enregistrementDirectory, string agentName)
        {

            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 8))
            {
                ViewBag.role = "Agent Qualité_CustomerService";
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

        public ActionResult SAMRCFOAutoWithoutAgent(string enregistrementFullName, string enregistrementUrl, string enregistrementDirectory, string siteName)
        {
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            var a = new EvaluationViewModel();
            List<SelectListItem> agents = new List<SelectListItem>();
            List<Employee> logins = new List<Employee>();
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 8))
            {
                ViewBag.role = "Agent Qualité_CustomerService";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 4))
            {
                ViewBag.role = "Qualité";
            }

            a.enregistrementFullName = enregistrementFullName;
            a.enregistrementUrl = enregistrementUrl;
            a.enregistrementDirectory = enregistrementDirectory;
       
                Groupe gr = serviceGroupe.getByNom(siteName);
                logins = serviceGroupeEmp.getListEmployeeByGroupeId(gr.Id);
          
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
        public ActionResult CalculSAMRCFOAuto(string nomAgent, string planDate, string accueil, string decouverteAttentes, string utilisationOutils, string miseAttente, string tempsAttente, string pertinenceReponse, string conclusionContact, string discours, string attitude, string historisation, string priseConge, string commentaire, string enregistrement, string enregistrementUrl, string enregistrementDirectory)
        {
            float total = 40;
            List<string> NEList = new List<string>(new string[] { miseAttente, tempsAttente});
            var a = new EvaluationEvaluationSCAutoViewModel();
            float notes = float.Parse(accueil, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(decouverteAttentes, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(utilisationOutils, CultureInfo.InvariantCulture.NumberFormat) +
                float.Parse(pertinenceReponse, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(conclusionContact, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(discours, CultureInfo.InvariantCulture.NumberFormat) +
                float.Parse(attitude, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(historisation, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(priseConge, CultureInfo.InvariantCulture.NumberFormat);

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
            a.decouverteAttentes = float.Parse(decouverteAttentes, CultureInfo.InvariantCulture.NumberFormat);
            a.utilisationOutils = float.Parse(utilisationOutils, CultureInfo.InvariantCulture.NumberFormat);
            a.miseAttente = float.Parse(miseAttente, CultureInfo.InvariantCulture.NumberFormat);
            a.tempsAttente = float.Parse(tempsAttente, CultureInfo.InvariantCulture.NumberFormat);
            a.pertinenceReponse = float.Parse(pertinenceReponse, CultureInfo.InvariantCulture.NumberFormat);
            a.conclusionContact = float.Parse(conclusionContact, CultureInfo.InvariantCulture.NumberFormat);
            a.discours = float.Parse(discours, CultureInfo.InvariantCulture.NumberFormat);
            a.attitude = float.Parse(attitude, CultureInfo.InvariantCulture.NumberFormat);
            a.historisation = float.Parse(historisation, CultureInfo.InvariantCulture.NumberFormat);
            a.priseConge = float.Parse(priseConge, CultureInfo.InvariantCulture.NumberFormat);

            a.dateTempEvaluation = DateTime.Parse(planDate);
            a.agentName = nomAgent;
            a.note = notes;
            a.pourcentageNotes = (notes / total) * 100;
            a.type = "SAM RC FO Auto";

            if (Request.IsAjaxRequest())
            {
                return PartialView("SAMRCFOResult", a);
            }
            return RedirectToAction("listeSites", "Superviseur");
        }

        public ActionResult SaveEvalSAMRCFOAuto(string nomAgent, string planDate, string accueil, string decouverteAttentes, string utilisationOutils, string miseAttente, string tempsAttente, string pertinenceReponse, string conclusionContact, string discours, string attitude, string historisation, string priseConge, string commentaire, string enregistrement, string enregistrementUrl, string enregistrementDirectory)
        {
            var userConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            Employee emp = serviceEmployee.getByPseudoName(nomAgent.ToLower());
            GrilleEvaluationFOSAMRC a = new GrilleEvaluationFOSAMRC();
            float total = 40;
            List<string> NEList = new List<string>(new string[] { miseAttente, tempsAttente });

            float notes = float.Parse(accueil, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(decouverteAttentes, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(utilisationOutils, CultureInfo.InvariantCulture.NumberFormat) + 
                float.Parse(pertinenceReponse, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(conclusionContact, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(discours, CultureInfo.InvariantCulture.NumberFormat) +
                float.Parse(attitude, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(historisation, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(priseConge, CultureInfo.InvariantCulture.NumberFormat);

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
            a.decouverteAttentes = float.Parse(decouverteAttentes, CultureInfo.InvariantCulture.NumberFormat);
            a.utilisationOutils = float.Parse(utilisationOutils, CultureInfo.InvariantCulture.NumberFormat);
            a.miseAttente = float.Parse(miseAttente, CultureInfo.InvariantCulture.NumberFormat);
            a.tempsAttente = float.Parse(tempsAttente, CultureInfo.InvariantCulture.NumberFormat);
            a.pertinenceReponse = float.Parse(pertinenceReponse, CultureInfo.InvariantCulture.NumberFormat);
            a.conclusionContact = float.Parse(conclusionContact, CultureInfo.InvariantCulture.NumberFormat);
            a.discours = float.Parse(discours, CultureInfo.InvariantCulture.NumberFormat);
            a.attitude = float.Parse(attitude, CultureInfo.InvariantCulture.NumberFormat);
            a.historisation = float.Parse(historisation, CultureInfo.InvariantCulture.NumberFormat);
            a.priseConge = float.Parse(priseConge, CultureInfo.InvariantCulture.NumberFormat);

            a.dateTempEvaluation = DateTime.Parse(planDate);
            a.employeeId = emp.Id;
            a.senderId = userConnected.Id;
            a.note = (notes / total) * 100;
            a.type = "SAM RC FO Auto";
            a.commentaireQualite = commentaire;
            a.enregistrementFullName = enregistrement;
            a.enregistrementUrl = enregistrementUrl;
            a.enregistrementDirectory = enregistrementDirectory;

            serviceFOSAMRCS.Add(a);
            serviceFOSAMRCS.SaveChange();
            var eval = new EvaluationEvaluationSCAutoViewModel();
            eval.agentName = nomAgent;
            //Envoi du Mail automatiquement
            string SenderMail = "alerte.infoprod@infopro-digital.com";
            string receiverMail = emp.Email;
            MailAddress to = new MailAddress(receiverMail);
            MailAddress from = new MailAddress(SenderMail);

            MailMessage message = new MailMessage(from, to);
            message.Subject = "Notification Nouvelle Evaluation";
            message.IsBodyHtml = true;
            message.Body = "<html><head></head><body><p>Bonjour,</p><p>Nous vous informons qu'un audit qualité viens d'être enregistré, vous pouvez le consulter sur l’interface INFO-PROD QUALITE </p><p>En attendant le débriefe de l’évaluateur</p><p>Cordialement.</p></body></html>";

            SmtpClient client = new SmtpClient("smtp.info.local", 587)
            {
                UseDefaultCredentials = true,
                // Credentials = new NetworkCredential("alerte.infoprod@infopro-digital.com", "Welcome01"),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = true
            };
            // code in brackets above needed if authentication required 
            try
            {
                client.Send(message);
            }
            catch (SmtpException ex)
            {
                Console.WriteLine(ex.ToString());
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("EnvoiMailResult", eval);
            }
            return RedirectToAction("Acceuil", "Directory");

        }
        #endregion

        #region Evaluation SC SAM/RC Auto Back office 
        [Authorize(Roles = "Qualité, Agent Qualité_CustomerService")]

        public ActionResult SAMRCBOAuto(string enregistrementFullName, string enregistrementUrl, string enregistrementDirectory, string agentName)
        {

            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 8))
            {
                ViewBag.role = "Agent Qualité_CustomerService";
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
            var Id = "5586770313D415B4";
            logins = serviceGroupe.getListEmployeeBySelectedSiteV5(Id);
            var us = logins.Select(o => o).Distinct().ToList();

            var ordredpseudoNames = us.OrderBy(b => b.pseudoName).ToList();

            int i = 0;
            while (i < ordredpseudoNames.Count)
            {
                if (!ordredpseudoNames[i].UserName.Equals(user.UserName) && (ordredpseudoNames[i].Roles.Any(r => r.UserId == ordredpseudoNames[i].Id && r.RoleId == 3 || r.RoleId == 1009)))
                {
                        a.employees.Add(new SelectListItem { Text = ordredpseudoNames[i].pseudoName, Value = ordredpseudoNames[i].pseudoName });
               
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

     
        public ActionResult CalculSAMRCBOAuto(string nomAgent, string planDate, string identificationClient, string qualificationDemandes, string pertinenceReponse, string discours, string numIntervention, string commentaire, string enregistrement, string enregistrementUrl, string enregistrementDirectory)
        {
            float total = 30;
            float notes = float.Parse(identificationClient, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(qualificationDemandes, CultureInfo.InvariantCulture.NumberFormat) 
                + float.Parse(pertinenceReponse, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(discours, CultureInfo.InvariantCulture.NumberFormat);

          
            var a = new EvaluationEvaluationSCAutoViewModel();
            a.identificationClient = float.Parse(identificationClient, CultureInfo.InvariantCulture.NumberFormat);
            a.qualificationDemandes = float.Parse(qualificationDemandes, CultureInfo.InvariantCulture.NumberFormat);
            a.pertinenceReponse = float.Parse(pertinenceReponse, CultureInfo.InvariantCulture.NumberFormat);
            a.discours = float.Parse(discours, CultureInfo.InvariantCulture.NumberFormat);
     
            a.numIntervention = numIntervention;

            a.dateTempEvaluation = DateTime.Parse(planDate);
            a.agentName = nomAgent;
            a.note = notes;
            a.pourcentageNotes = (notes / total) * 100;
            a.type = "SAM RC BO AUTO";

            if (Request.IsAjaxRequest())
            {
                return PartialView("SAMRCBOResult", a);
            }
            return RedirectToAction("listeSites", "Superviseur");
        }

        public ActionResult SaveEvalSAMRCBOAuto(string nomAgent, string planDate, string identificationClient, string qualificationDemandes, string pertinenceReponse, string discours, string numIntervention, string commentaire, string enregistrement, string enregistrementUrl, string enregistrementDirectory)
        {
            var userConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            Employee emp = serviceEmployee.getByPseudoName(nomAgent.ToLower());
            GrilleEvaluationBOSAMRC a = new GrilleEvaluationBOSAMRC();

            float total = 30;
            float notes = float.Parse(identificationClient, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(qualificationDemandes, CultureInfo.InvariantCulture.NumberFormat)
                + float.Parse(pertinenceReponse, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(discours, CultureInfo.InvariantCulture.NumberFormat);

            a.identificationClient = float.Parse(identificationClient, CultureInfo.InvariantCulture.NumberFormat);
            a.qualificationDemandes = float.Parse(qualificationDemandes, CultureInfo.InvariantCulture.NumberFormat);
            a.pertinenceReponse = float.Parse(pertinenceReponse, CultureInfo.InvariantCulture.NumberFormat);
            a.discours = float.Parse(discours, CultureInfo.InvariantCulture.NumberFormat);

            a.numIntervention = numIntervention;               

            a.dateTempEvaluation = DateTime.Parse(planDate);
            a.employeeId = emp.Id;
            a.senderId = userConnected.Id;
            a.note = (notes / total) * 100;
            a.type = "SAM RC BO AUTO";
            a.commentaireQualite = commentaire;
            a.enregistrementFullName = enregistrement;
            a.enregistrementUrl = enregistrementUrl;
            a.enregistrementDirectory = enregistrementDirectory;

            serviceBOSAMRC.Add(a);
            serviceBOSAMRC.SaveChange();
            var eval = new EvaluationEvaluationSCAutoViewModel();
            eval.agentName = nomAgent;
            //Envoi du Mail automatiquement
            string SenderMail = "alerte.infoprod@infopro-digital.com";
            string receiverMail = emp.Email;
            MailAddress to = new MailAddress(receiverMail);
            MailAddress from = new MailAddress(SenderMail);

            MailMessage message = new MailMessage(from, to);
            message.Subject = "Notification Nouvelle Evaluation";
            message.IsBodyHtml = true;
            message.Body = "<html><head></head><body><p>Bonjour,</p><p>Nous vous informons qu'un audit qualité viens d'être enregistré, vous pouvez le consulter sur l’interface INFO-PROD QUALITE </p><p>En attendant le débriefe de l’évaluateur</p><p>Cordialement.</p></body></html>";

            SmtpClient client = new SmtpClient("smtp.info.local", 587)
            {
                UseDefaultCredentials = true,
                // Credentials = new NetworkCredential("alerte.infoprod@infopro-digital.com", "Welcome01"),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = true
            };
            // code in brackets above needed if authentication required 
            try
            {
                client.Send(message);
            }
            catch (SmtpException ex)
            {
                Console.WriteLine(ex.ToString());
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("EnvoiMailResult", eval);
            }
            return RedirectToAction("Acceuil", "Directory");

        }
        #endregion

        #region Historique SAM RC FO Par Collaborateur
        [Authorize(Roles = "Qualité, Agent Qualité_CustomerService,Manager")]

        public ActionResult HistoriqueSAMRCFO()
        {
            List<Employee> logins = new List<Employee>();
            EvaluationViewModel evaluation = new EvaluationViewModel();
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
                if (!test.UserName.Equals(user.UserName) && test.Roles.Any(r => r.UserId == test.Id && r.RoleId == 3 || r.RoleId == 1009))
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
        public ActionResult GetHistoSAMRCFO(string username)
        {
            List<EvaluationEvaluationSCAutoViewModel> a = new List<EvaluationEvaluationSCAutoViewModel>();
            try
            {
                Employee emp = serviceEmployee.getByLogin(username);
                var historstions = serviceFOSAMRCS.GetEvaluationsByEmployee(emp.Id);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationEvaluationSCAutoViewModel test = new EvaluationEvaluationSCAutoViewModel();
                    test.Id = item.Id;
                    test.accueil = item.accueil;
                    test.decouverteAttentes = item.decouverteAttentes;
                    test.utilisationOutils = item.utilisationOutils;
                    test.miseAttente = item.miseAttente;
                    test.tempsAttente = item.tempsAttente;
                    test.pertinenceReponse = item.pertinenceReponse;
                    test.conclusionContact = item.conclusionContact;
                    test.discours = item.discours;
                    test.attitude = item.attitude;
                    test.historisation = item.historisation;
                    test.priseConge = item.priseConge;

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
                    return PartialView("HistoriqueSAMRCFOPartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("HistoriqueSAMRCFO");
        }

        public ActionResult GetHistoSAMRCFOByDate(string username, string dateDebut, string dateFin)
        {
            List<EvaluationEvaluationSCAutoViewModel> a = new List<EvaluationEvaluationSCAutoViewModel>();
            try
            {
                Employee emp = serviceEmployee.getByLogin(username);
                DateTime daterecherchedebut = DateTime.Parse(dateDebut);
                DateTime daterecherchefin = DateTime.Parse(dateFin);


                var historstions = serviceFOSAMRCS.GetEvaluationsEmployeeBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationEvaluationSCAutoViewModel test = new EvaluationEvaluationSCAutoViewModel();
                    test.Id = item.Id;
                    test.accueil = item.accueil;
                    test.decouverteAttentes = item.decouverteAttentes;
                    test.utilisationOutils = item.utilisationOutils;
                    test.miseAttente = item.miseAttente;
                    test.tempsAttente = item.tempsAttente;
                    test.pertinenceReponse = item.pertinenceReponse;
                    test.conclusionContact = item.conclusionContact;
                    test.discours = item.discours;
                    test.attitude = item.attitude;
                    test.historisation = item.historisation;
                    test.priseConge = item.priseConge;

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
                    return PartialView("HistoriqueSAMRCFOPartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("HistoriqueSAMRCFO");
        }
        #endregion

        #region Archive SAM RC FO Auto par Resp Qualité
        //Archive evaluations par Responsable Qualité
        [Authorize(Roles = "Qualité")]
        public ActionResult ArchiveSAMRCFO()
        {
            EvaluationEvaluationSCAutoViewModel evaluation = new EvaluationEvaluationSCAutoViewModel();
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            var logins = serviceEmployee.GetAll();
            var employees = logins.Select(o => o).Distinct().ToList();

            foreach (var test in employees)
            {
                if (test.Roles.Any(r => r.UserId == test.Id && r.RoleId == 4 || r.RoleId == 8))
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

        public ActionResult GetArchiveQualiteSAMRCFO(string username)
        {
            List<EvaluationEvaluationSCAutoViewModel> a = new List<EvaluationEvaluationSCAutoViewModel>();
            try
            {
                Employee emp = serviceEmployee.getByLogin(username);
                var historstions = serviceFOSAMRCS.GetEvaluationsBySenderId(emp.Id);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationEvaluationSCAutoViewModel test = new EvaluationEvaluationSCAutoViewModel();
                    test.Id = item.Id;
                    test.accueil = item.accueil;
                    test.decouverteAttentes = item.decouverteAttentes;
                    test.utilisationOutils = item.utilisationOutils;
                    test.miseAttente = item.miseAttente;
                    test.tempsAttente = item.tempsAttente;
                    test.pertinenceReponse = item.pertinenceReponse;
                    test.conclusionContact = item.conclusionContact;
                    test.discours = item.discours;
                    test.attitude = item.attitude;
                    test.historisation = item.historisation;
                    test.priseConge = item.priseConge;

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
                    return PartialView("ArchiveSAMRCFOQualitePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("ArchiveSAMRCFO");
        }

        public ActionResult GetArchiveQualiteSAMRCFOByDate(string username, string dateDebut, string dateFin)
        {
            List<EvaluationEvaluationSCAutoViewModel> a = new List<EvaluationEvaluationSCAutoViewModel>();
            try
            {
                Employee emp = serviceEmployee.getByLogin(username);
                DateTime daterecherchedebut = DateTime.Parse(dateDebut);
                DateTime daterecherchefin = DateTime.Parse(dateFin);
                var historstions = serviceFOSAMRCS.GetEvaluationsSenderBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationEvaluationSCAutoViewModel test = new EvaluationEvaluationSCAutoViewModel();
                    test.Id = item.Id;
                    test.accueil = item.accueil;
                    test.decouverteAttentes = item.decouverteAttentes;
                    test.utilisationOutils = item.utilisationOutils;
                    test.miseAttente = item.miseAttente;
                    test.tempsAttente = item.tempsAttente;
                    test.pertinenceReponse = item.pertinenceReponse;
                    test.conclusionContact = item.conclusionContact;
                    test.discours = item.discours;
                    test.attitude = item.attitude;
                    test.historisation = item.historisation;
                    test.priseConge = item.priseConge;

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
                    return PartialView("ArchiveSAMRCFOQualitePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("ArchiveSAMRCFO");
        }
        #endregion

        #region Historique SAM RC BO Par Collaborateur
        [Authorize(Roles = "Qualité, Agent Qualité_CustomerService,Manager")]

        public ActionResult HistoriqueSAMRCBO()
        {
            List<Employee> logins = new List<Employee>();
            EvaluationViewModel evaluation = new EvaluationViewModel();
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
                if (!test.UserName.Equals(user.UserName) && test.Roles.Any(r => r.UserId == test.Id && r.RoleId == 3 || r.RoleId == 1009))
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
        public ActionResult GetHistoSAMRCBO(string username)
        {
            List<EvaluationEvaluationSCAutoViewModel> a = new List<EvaluationEvaluationSCAutoViewModel>();
            try
            {
                Employee emp = serviceEmployee.getByLogin(username);
                var historstions = serviceBOSAMRC.GetEvaluationsByEmployee(emp.Id);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationEvaluationSCAutoViewModel test = new EvaluationEvaluationSCAutoViewModel();
                    test.Id = item.Id;
                    test.identificationClient = item.identificationClient;
                    test.qualificationDemandes = item.qualificationDemandes;
                    test.pertinenceReponse = item.pertinenceReponse;
                    test.discours = item.discours;
                    test.numIntervention = item.numIntervention;                   

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
                    return PartialView("HistoriqueSAMRCBOPartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("HistoriqueSAMRCBO");
        }

        public ActionResult GetHistoSAMRCBOByDate(string username, string dateDebut, string dateFin)
        {
            List<EvaluationEvaluationSCAutoViewModel> a = new List<EvaluationEvaluationSCAutoViewModel>();
            try
            {
                Employee emp = serviceEmployee.getByLogin(username);
                DateTime daterecherchedebut = DateTime.Parse(dateDebut);
                DateTime daterecherchefin = DateTime.Parse(dateFin);


                var historstions = serviceBOSAMRC.GetEvaluationsEmployeeBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationEvaluationSCAutoViewModel test = new EvaluationEvaluationSCAutoViewModel();
                    test.Id = item.Id;
                    test.identificationClient = item.identificationClient;
                    test.qualificationDemandes = item.qualificationDemandes;
                    test.pertinenceReponse = item.pertinenceReponse;
                    test.discours = item.discours;
                    test.numIntervention = item.numIntervention;

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
                    return PartialView("HistoriqueSAMRCBOPartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("HistoriqueSAMRCBO");
        }
        #endregion


        #region Archive SAM RC BO Auto par Resp Qualité
        //Archive evaluations par Responsable Qualité
        [Authorize(Roles = "Qualité")]
        public ActionResult ArchiveSAMRCBO()
        {
            EvaluationEvaluationSCAutoViewModel evaluation = new EvaluationEvaluationSCAutoViewModel();
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            var logins = serviceEmployee.GetAll();
            var employees = logins.Select(o => o).Distinct().ToList();

            foreach (var test in employees)
            {
                if (test.Roles.Any(r => r.UserId == test.Id && r.RoleId == 4 || r.RoleId == 8))
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

        public ActionResult GetArchiveQualiteSAMRCBO(string username)
        {
            List<EvaluationEvaluationSCAutoViewModel> a = new List<EvaluationEvaluationSCAutoViewModel>();
            try
            {
                Employee emp = serviceEmployee.getByLogin(username);
                var historstions = serviceBOSAMRC.GetEvaluationsBySenderId(emp.Id);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationEvaluationSCAutoViewModel test = new EvaluationEvaluationSCAutoViewModel();
                    test.Id = item.Id;
                    test.identificationClient = item.identificationClient;
                    test.qualificationDemandes = item.qualificationDemandes;
                    test.pertinenceReponse = item.pertinenceReponse;
                    test.discours = item.discours;
                    test.numIntervention = item.numIntervention;

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
                    return PartialView("ArchiveSAMRCBOQualitePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("ArchiveSAMRCBO");
        }

        public ActionResult GetArchiveQualiteSAMRCBOByDate(string username, string dateDebut, string dateFin)
        {
            List<EvaluationEvaluationSCAutoViewModel> a = new List<EvaluationEvaluationSCAutoViewModel>();
            try
            {
                Employee emp = serviceEmployee.getByLogin(username);
                DateTime daterecherchedebut = DateTime.Parse(dateDebut);
                DateTime daterecherchefin = DateTime.Parse(dateFin);
                var historstions = serviceBOSAMRC.GetEvaluationsSenderBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationEvaluationSCAutoViewModel test = new EvaluationEvaluationSCAutoViewModel();
                    test.Id = item.Id;
                    test.identificationClient = item.identificationClient;
                    test.qualificationDemandes = item.qualificationDemandes;
                    test.pertinenceReponse = item.pertinenceReponse;
                    test.discours = item.discours;
                    test.numIntervention = item.numIntervention;

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
                    return PartialView("ArchiveSAMRCBOQualitePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("ArchiveSAMRCBO");
        }
        #endregion

        #region Edit et delete Enquete Auto
        [Authorize(Roles = "Qualité")]

        public ActionResult EditEnqueteAuto(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GrilleEvaluationEnqueteAuto evaluation = db.GrilleEvaluationEnqueteAutoes.Find(id);
        
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
        [HttpPost, ActionName("EditEnqueteAuto")]
        [Authorize(Roles = "Qualité")]
        public ActionResult EditEnqueteAuto(int? id, GrilleEvaluationEnqueteAuto evaluation)
        {
            float total = 71;
            List<float> NEList = new List<float>(new float[] { evaluation.presentation, evaluation.respectScript,
                evaluation.traitementObjections, evaluation.exploitationOutils ,
                evaluation.HistorisationOutils, evaluation.discours,
                evaluation.debit, evaluation.intonation,
                evaluation.ecouteReformulation, evaluation.priseConge});
            float notes = 0;

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
                return RedirectToAction("HistoriqueEnqueteAuto");
            }
            return View(evaluation);
        }
        [Authorize(Roles = "Qualité")]
        [HttpGet]
        public ActionResult FindEvaluationEnqueteAuto(int? Id)
        {
            GrilleEvaluationEnqueteAuto item = service.getById(Id);

            if (Request.IsAjaxRequest())
            {
                return PartialView("_AlerteDeSuppressionEnqueteAuto", item);
            }

            else
            {
                return View(item);
            }
        }
        [Authorize(Roles = "Qualité")]
        public ActionResult DeleteEnqueteAuto(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GrilleEvaluationEnqueteAuto evaluation = service.getById(id);
            int? empId = evaluation.employeeId;
            service.DeleteEvaluations(id, empId);
            service.SaveChange();
            return RedirectToAction("HistoriqueEnqueteAuto");
        }
        #endregion


        #region Edit et delete SAMRCFO
        [Authorize(Roles = "Qualité")]

        public ActionResult EditSAMRCFO(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GrilleEvaluationFOSAMRC evaluation = db.GrilleEvaluationFOSAMRCs.Find(id);
            EvaluationVecteurPlusViewModel eval = new EvaluationVecteurPlusViewModel();
            eval.dateTempEvaluation = evaluation.dateTempEvaluation;
            eval.commentaireQualite = evaluation.commentaireQualite;

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
        [HttpPost, ActionName("EditSAMRCFO")]
        [Authorize(Roles = "Qualité")]
        public ActionResult EditSAMRCFO(int? id, GrilleEvaluationFOSAMRC evaluation)
        {
            float total = 40;
            List<float> NEList = new List<float>(new float[] { evaluation.miseAttente, evaluation.tempsAttente });
            float notes = evaluation.accueil + evaluation.decouverteAttentes + evaluation.utilisationOutils +
              evaluation.pertinenceReponse + evaluation.conclusionContact + evaluation.discours +
              evaluation.attitude + evaluation.historisation + evaluation.priseConge;

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
                return RedirectToAction("HistoriqueSAMRCFO");
            }
            return View(evaluation);
        }
        [Authorize(Roles = "Qualité")]
        [HttpGet]
        public ActionResult FindEvaluationSAMRCFO(int? Id)
        {
            GrilleEvaluationFOSAMRC item = serviceFOSAMRCS.getById(Id);

            if (Request.IsAjaxRequest())
            {
                return PartialView("_AlerteDeSuppressionSAMRCFO", item);
            }

            else
            {
                return View(item);
            }
        }
        [Authorize(Roles = "Qualité")]
        public ActionResult DeleteSAMRCFO(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GrilleEvaluationFOSAMRC evaluation = serviceFOSAMRCS.getById(id);
            int? empId = evaluation.employeeId;
            serviceFOSAMRCS.DeleteEvaluations(id, empId);
            serviceFOSAMRCS.SaveChange();
            return RedirectToAction("HistoriqueSAMRCFO");
        }

        #endregion

        #region Edit et delete SAMRCBO
        [Authorize(Roles = "Qualité")]

        public ActionResult EditSAMRCBO(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GrilleEvaluationBOSAMRC evaluation = db.GrilleEvaluationBOSAMRCs.Find(id);
            EvaluationVecteurPlusViewModel eval = new EvaluationVecteurPlusViewModel();
            eval.dateTempEvaluation = evaluation.dateTempEvaluation;
            eval.commentaireQualite = evaluation.commentaireQualite;

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
        [HttpPost, ActionName("EditSAMRCBO")]
        [Authorize(Roles = "Qualité")]
        public ActionResult EditSAMRCBO(int? id, GrilleEvaluationBOSAMRC evaluation)
        {
            float total = 30;
            float notes = evaluation.identificationClient + evaluation.qualificationDemandes + evaluation.pertinenceReponse +
              evaluation.discours;

            if (ModelState.IsValid)
            {
                evaluation.note = (notes / total) * 100;
                db.Entry(evaluation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("HistoriqueSAMRCBO");
            }
            return View(evaluation);
        }
        [Authorize(Roles = "Qualité")]
        [HttpGet]
        public ActionResult FindEvaluationSAMRCBO(int? Id)
        {
            GrilleEvaluationBOSAMRC item = serviceBOSAMRC.getById(Id);

            if (Request.IsAjaxRequest())
            {
                return PartialView("_AlerteDeSuppressionSAMRCBO", item);
            }

            else
            {
                return View(item);
            }
        }
        [Authorize(Roles = "Qualité")]
        public ActionResult DeleteSAMRCBO(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GrilleEvaluationBOSAMRC evaluation = serviceBOSAMRC.getById(id);
            int? empId = evaluation.employeeId;
            serviceBOSAMRC.DeleteEvaluations(id, empId);
            serviceBOSAMRC.SaveChange();
            return RedirectToAction("HistoriqueSAMRCBO");
        }

        #endregion

        #region Archive SAMRCFO Agent Qualité_CustomerService
        [Authorize(Roles = "Agent Qualité_CustomerService")]
        public ActionResult ArchiveAgentQualiteSAMRCFO()
        {
            EvaluationEvaluationSCAutoViewModel evaluation = new EvaluationEvaluationSCAutoViewModel();

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
        public ActionResult GetArchiveAgentQualiteSAMRCFO()
        {
            List<EvaluationEvaluationSCAutoViewModel> a = new List<EvaluationEvaluationSCAutoViewModel>();
            try
            {
                Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

                var historstions = serviceFOSAMRCS.GetEvaluationsBySenderId(emp.Id);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationEvaluationSCAutoViewModel test = new EvaluationEvaluationSCAutoViewModel();
                    test.Id = item.Id;
                    test.accueil = item.accueil;
                    test.decouverteAttentes = item.decouverteAttentes;
                    test.utilisationOutils = item.utilisationOutils;
                    test.miseAttente = item.miseAttente;
                    test.tempsAttente = item.tempsAttente;
                    test.pertinenceReponse = item.pertinenceReponse;
                    test.conclusionContact = item.conclusionContact;
                    test.discours = item.discours;
                    test.attitude = item.attitude;
                    test.historisation = item.historisation;
                    test.priseConge = item.priseConge;

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
                    return PartialView("ArchiveSAMRCFOQualitePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("GetArchiveAgentQualiteSAMRCFO");
        }

        public ActionResult GetArchiveAgentQualiteSAMRCFOByDate(string dateDebut, string dateFin)
        {
            List<EvaluationEvaluationSCAutoViewModel> a = new List<EvaluationEvaluationSCAutoViewModel>();
            try
            {
                Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
                DateTime daterecherchedebut = DateTime.Parse(dateDebut);
                DateTime daterecherchefin = DateTime.Parse(dateFin);
                var historstions = serviceFOSAMRCS.GetEvaluationsSenderBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationEvaluationSCAutoViewModel test = new EvaluationEvaluationSCAutoViewModel();
                    test.Id = item.Id;
                    test.accueil = item.accueil;
                    test.decouverteAttentes = item.decouverteAttentes;
                    test.utilisationOutils = item.utilisationOutils;
                    test.miseAttente = item.miseAttente;
                    test.tempsAttente = item.tempsAttente;
                    test.pertinenceReponse = item.pertinenceReponse;
                    test.conclusionContact = item.conclusionContact;
                    test.discours = item.discours;
                    test.attitude = item.attitude;
                    test.historisation = item.historisation;
                    test.priseConge = item.priseConge;

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
                    return PartialView("ArchiveSAMRCFOQualitePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("GetArchiveAgentQualiteSAMRCFO");
        }
        #endregion

        #region Archive SAMRCBO Agent Qualité_CustomerService
        [Authorize(Roles = "Agent Qualité_CustomerService")]
        public ActionResult ArchiveAgentQualiteSAMRCBO()
        {
            EvaluationEvaluationSCAutoViewModel evaluation = new EvaluationEvaluationSCAutoViewModel();

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
        public ActionResult GetArchiveAgentQualiteSAMRCBO()
        {
            List<EvaluationEvaluationSCAutoViewModel> a = new List<EvaluationEvaluationSCAutoViewModel>();
            try
            {
                Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

                var historstions = serviceBOSAMRC.GetEvaluationsBySenderId(emp.Id);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationEvaluationSCAutoViewModel test = new EvaluationEvaluationSCAutoViewModel();
                    test.Id = item.Id;
                    test.identificationClient = item.identificationClient;
                    test.qualificationDemandes = item.qualificationDemandes;
                    test.pertinenceReponse = item.pertinenceReponse;
                    test.discours = item.discours;
                    test.numIntervention = item.numIntervention;

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
                    return PartialView("ArchiveSAMRCBOQualitePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("GetArchiveAgentQualiteSAMRCBO");
        }

        public ActionResult GetArchiveAgentQualiteSAMRCBOByDate(string dateDebut, string dateFin)
        {
            List<EvaluationEvaluationSCAutoViewModel> a = new List<EvaluationEvaluationSCAutoViewModel>();
            try
            {
                Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
                DateTime daterecherchedebut = DateTime.Parse(dateDebut);
                DateTime daterecherchefin = DateTime.Parse(dateFin);
                var historstions = serviceBOSAMRC.GetEvaluationsSenderBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationEvaluationSCAutoViewModel test = new EvaluationEvaluationSCAutoViewModel();
                    test.Id = item.Id;
                    test.identificationClient = item.identificationClient;
                    test.qualificationDemandes = item.qualificationDemandes;
                    test.pertinenceReponse = item.pertinenceReponse;
                    test.discours = item.discours;
                    test.numIntervention = item.numIntervention;

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
                    return PartialView("ArchiveSAMRCBOQualitePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("GetArchiveAgentQualiteSAMRCBO");
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

        #region historiqueAgent Enquete Auto
        [Authorize(Roles = "Agent_CustomerService")]
        public ActionResult HistoriqueAgent()
        {
            EvaluationViewModel evaluation = new EvaluationViewModel();
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (user == null)
            {
                return HttpNotFound();
            }
            if (user.Content != null)
            {
                String strbase64 = Convert.ToBase64String(user.Content);
                String empConnectedImage = "data:" + user.ContentType + ";base64," + strbase64;
                ViewBag.empConnectedImage = empConnectedImage;
            }
            ViewBag.ConnectedUsername = user.UserName;
            ViewBag.pseudoNameEmpConnected = user.pseudoName;

            return View(evaluation);
        }
        public ActionResult GetHistoAgent()
        {
            List<EvaluationEvaluationSCAutoViewModel> a = new List<EvaluationEvaluationSCAutoViewModel>();
            try
            {
                Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

                var historstions = service.GetEvaluationsByEmployee(emp.Id);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationEvaluationSCAutoViewModel test = new EvaluationEvaluationSCAutoViewModel();
                    test.Id = item.Id;
                    test.presentation = item.presentation;
                    test.respectScript = item.respectScript;
                    test.traitementObjections = item.traitementObjections;
                    test.exploitationOutils = item.exploitationOutils;
                    test.HistorisationOutils = item.HistorisationOutils;
                    test.discours = item.discours;
                    test.debit = item.debit;
                    test.intonation = item.intonation;
                    test.ecouteReformulation = item.ecouteReformulation;
                    test.priseConge = item.priseConge;

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

                    a.Add(test);
                }

                if (Request.IsAjaxRequest())
                {
                    return PartialView("HistoriqueTableEnqueteAutoAgentPartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("HistoriqueAgent");
        }

        public ActionResult GetHistoAgentByDate(string dateDebut, string dateFin)
        {
            List<EvaluationEvaluationSCAutoViewModel> a = new List<EvaluationEvaluationSCAutoViewModel>();
            try
            {
                Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
                DateTime daterecherchedebut = DateTime.Parse(dateDebut);
                DateTime daterecherchefin = DateTime.Parse(dateFin);
                var historstions = service.GetEvaluationsEmployeeBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationEvaluationSCAutoViewModel test = new EvaluationEvaluationSCAutoViewModel();
                    test.Id = item.Id;
                    test.presentation = item.presentation;
                    test.respectScript = item.respectScript;
                    test.traitementObjections = item.traitementObjections;
                    test.exploitationOutils = item.exploitationOutils;
                    test.HistorisationOutils = item.HistorisationOutils;
                    test.discours = item.discours;
                    test.debit = item.debit;
                    test.intonation = item.intonation;
                    test.ecouteReformulation = item.ecouteReformulation;
                    test.priseConge = item.priseConge;

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

                    a.Add(test);
                }

                if (Request.IsAjaxRequest())
                {
                    return PartialView("HistoriqueTableEnqueteAutoAgentPartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("HistoriqueAgent");
        }
        public ActionResult ReponseEvaluationEnqueteAuto(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GrilleEvaluationEnqueteAuto evaluation = db.GrilleEvaluationEnqueteAutoes.Find(id);
            EvaluationEvaluationSCAutoViewModel eval = new EvaluationEvaluationSCAutoViewModel();
            eval.dateTempEvaluation = evaluation.dateTempEvaluation;
            eval.commentaireQualite = evaluation.commentaireQualite;

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
        [HttpPost, ActionName("ReponseEvaluationEnqueteAuto")]
        [Authorize(Roles = "Agent_CustomerService")]
        public ActionResult ReponseEvaluationEnqueteAuto(int? id, GrilleEvaluationEnqueteAuto evaluation)
        {
            if (ModelState.IsValid)
            {

                db.Entry(evaluation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("HistoriqueAgent");
            }
            return View(evaluation);
        }
        #endregion

        #region historiqueAgent SAM RC
        [Authorize(Roles = "Agent_SAMRC")]
        public ActionResult HistoriqueAgent_SAMRC()
        {
            EvaluationViewModel evaluation = new EvaluationViewModel();
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (user == null)
            {
                return HttpNotFound();
            }
            if (user.Content != null)
            {
                String strbase64 = Convert.ToBase64String(user.Content);
                String empConnectedImage = "data:" + user.ContentType + ";base64," + strbase64;
                ViewBag.empConnectedImage = empConnectedImage;
            }
            ViewBag.ConnectedUsername = user.UserName;
            ViewBag.pseudoNameEmpConnected = user.pseudoName;

            return View(evaluation);
        }
        public ActionResult GetHistoAgentFOSAMRC()
        {
            List<EvaluationEvaluationSCAutoViewModel> a = new List<EvaluationEvaluationSCAutoViewModel>();
            try
            {
                Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

                var historstions = serviceFOSAMRCS.GetEvaluationsByEmployee(emp.Id);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationEvaluationSCAutoViewModel test = new EvaluationEvaluationSCAutoViewModel();
                    test.Id = item.Id;
                    test.accueil = item.accueil;
                    test.decouverteAttentes = item.decouverteAttentes;
                    test.utilisationOutils = item.utilisationOutils;
                    test.miseAttente = item.miseAttente;
                    test.tempsAttente = item.tempsAttente;
                    test.pertinenceReponse = item.pertinenceReponse;
                    test.conclusionContact = item.conclusionContact;
                    test.discours = item.discours;
                    test.attitude = item.attitude;
                    test.historisation = item.historisation;
                    test.priseConge = item.priseConge;

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

                    a.Add(test);
                }

                if (Request.IsAjaxRequest())
                {
                    return PartialView("HistoriqueTableFOSAMRCAgentPartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("HistoriqueAgent_SAMRC");
        }

        public ActionResult GetHistoAgentBOSAMRC()
        {
            List<EvaluationEvaluationSCAutoViewModel> a = new List<EvaluationEvaluationSCAutoViewModel>();
            try
            {
                Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

                var historstions = serviceBOSAMRC.GetEvaluationsByEmployee(emp.Id);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationEvaluationSCAutoViewModel test = new EvaluationEvaluationSCAutoViewModel();
                    test.Id = item.Id;
                    test.identificationClient = item.identificationClient;
                    test.qualificationDemandes = item.qualificationDemandes;
                    test.pertinenceReponse = item.pertinenceReponse;
                    test.discours = item.discours;               
                    test.numIntervention = item.numIntervention;

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

                    a.Add(test);
                }

                if (Request.IsAjaxRequest())
                {
                    return PartialView("HistoriqueTableBOSAMRCAgentPartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("HistoriqueAgent_SAMRC");
        }
        public ActionResult GetHistoAgentFOSAMRCByDate(string dateDebut, string dateFin)
        {
            List<EvaluationEvaluationSCAutoViewModel> a = new List<EvaluationEvaluationSCAutoViewModel>();
            try
            {
                Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
                DateTime daterecherchedebut = DateTime.Parse(dateDebut);
                DateTime daterecherchefin = DateTime.Parse(dateFin);
                var historstions = serviceFOSAMRCS.GetEvaluationsEmployeeBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationEvaluationSCAutoViewModel test = new EvaluationEvaluationSCAutoViewModel();
                    test.Id = item.Id;
                    test.accueil = item.accueil;
                    test.decouverteAttentes = item.decouverteAttentes;
                    test.utilisationOutils = item.utilisationOutils;
                    test.miseAttente = item.miseAttente;
                    test.tempsAttente = item.tempsAttente;
                    test.pertinenceReponse = item.pertinenceReponse;
                    test.conclusionContact = item.conclusionContact;
                    test.discours = item.discours;
                    test.attitude = item.attitude;
                    test.historisation = item.historisation;
                    test.priseConge = item.priseConge;

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

                    a.Add(test);
                }

                if (Request.IsAjaxRequest())
                {
                    return PartialView("HistoriqueTableFOSAMRCAgentPartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("HistoriqueAgent_SAMRC");
        }

        public ActionResult GetHistoAgentBOSAMRCByDate(string dateDebut, string dateFin)
        {
            List<EvaluationEvaluationSCAutoViewModel> a = new List<EvaluationEvaluationSCAutoViewModel>();
            try
            {
                Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
                DateTime daterecherchedebut = DateTime.Parse(dateDebut);
                DateTime daterecherchefin = DateTime.Parse(dateFin);
                var historstions = serviceBOSAMRC.GetEvaluationsEmployeeBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationEvaluationSCAutoViewModel test = new EvaluationEvaluationSCAutoViewModel();
                    test.Id = item.Id;
                    test.identificationClient = item.identificationClient;
                    test.qualificationDemandes = item.qualificationDemandes;
                    test.pertinenceReponse = item.pertinenceReponse;
                    test.discours = item.discours;
                    test.numIntervention = item.numIntervention;

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

                    a.Add(test);
                }

                if (Request.IsAjaxRequest())
                {
                    return PartialView("HistoriqueTableBOSAMRCAgentPartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("HistoriqueAgent_SAMRC");
        }
        public ActionResult ReponseEvaluationFOSAMRC(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GrilleEvaluationFOSAMRC evaluation = db.GrilleEvaluationFOSAMRCs.Find(id);
            EvaluationEvaluationSCAutoViewModel eval = new EvaluationEvaluationSCAutoViewModel();
            eval.dateTempEvaluation = evaluation.dateTempEvaluation;
            eval.commentaireQualite = evaluation.commentaireQualite;

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
        [HttpPost, ActionName("ReponseEvaluationFOSAMRC")]
        [Authorize(Roles = "Agent_SAMRC")]
        public ActionResult ReponseEvaluationFOSAMRC(int? id, GrilleEvaluationFOSAMRC evaluation)
        {
            if (ModelState.IsValid)
            {

                db.Entry(evaluation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("HistoriqueAgent_SAMRC");
            }
            return View(evaluation);
        }


        public ActionResult ReponseEvaluationBOSAMRC(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GrilleEvaluationBOSAMRC evaluation = db.GrilleEvaluationBOSAMRCs.Find(id);
            EvaluationEvaluationSCAutoViewModel eval = new EvaluationEvaluationSCAutoViewModel();
            eval.dateTempEvaluation = evaluation.dateTempEvaluation;
            eval.commentaireQualite = evaluation.commentaireQualite;

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
        [HttpPost, ActionName("ReponseEvaluationBOSAMRC")]
        [Authorize(Roles = "Agent_SAMRC")]
        public ActionResult ReponseEvaluationBOSAMRC(int? id, GrilleEvaluationBOSAMRC evaluation)
        {
            if (ModelState.IsValid)
            {

                db.Entry(evaluation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("HistoriqueAgent_SAMRC");
            }
            return View(evaluation);
        }
        #endregion

        #region Reports Enquete Auto
        public ActionResult ReportsEnqueteAuto()
        {
            List<Employee> logins = new List<Employee>();
            EvaluationViewModel evaluation = new EvaluationViewModel();
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
                if (!test.UserName.Equals(user.UserName) && test.Roles.Any(r => r.UserId == test.Id && r.RoleId == 10))
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
        public JsonResult GetReportsEnqueteAuto(string username, string dateDebut, string dateFin)
        {
            float totPresentation = 0, totRespectScript = 0, totTraitementObjections = 0, totExploitationOutils = 0, 
                totHistorisationOutils = 0, totDiscours = 0, totDebit = 0, totIntonation = 0, totEcouteReformulation = 0,
                totPriseConge = 0, totNotes = 0;

            float NbrePresentation = 0, NbreRespectScript = 0, NbreTraitementObjections = 0, NbreExploitationOutils = 0,
                NbreHistorisationOutils = 0, NbreDiscours = 0, NbreDebit = 0, NbreIntonation = 0, NbreEcouteReformulation = 0, NbrePriseConge = 0;

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

                if (item.presentation >= 0)
                {
                    totPresentation += item.presentation;
                    NbrePresentation += 1;
                }
                if (item.respectScript >= 0)
                {
                    totRespectScript += item.respectScript;
                    NbreRespectScript += 1;
                }
                if (item.traitementObjections >= 0)
                {

                    totTraitementObjections += item.traitementObjections;
                    NbreTraitementObjections += 1;
                }
                if (item.exploitationOutils >= 0)
                {
                    totExploitationOutils += item.exploitationOutils;
                    NbreExploitationOutils += 1;
                }
                if (item.HistorisationOutils >= 0)
                {
                    totHistorisationOutils += item.HistorisationOutils;
                    NbreHistorisationOutils += 1;
                }
                if (item.discours >= 0)
                {
                    totDiscours += item.discours;
                    NbreDiscours += 1;
                }
                if (item.debit >= 0)
                {
                    totDebit += item.debit;
                    NbreDebit += 1;
                }
                if (item.intonation >= 0)
                {
                    totIntonation += item.intonation;
                    NbreIntonation += 1;
                }

                if (item.ecouteReformulation >= 0)
                {
                    totEcouteReformulation += item.ecouteReformulation;
                    NbreEcouteReformulation += 1;
                }
                if (item.priseConge >= 0)
                {
                    totPriseConge += item.priseConge;
                    NbrePriseConge += 1;
                }

            }

            EvaluationEvaluationSCAutoViewModel test = new EvaluationEvaluationSCAutoViewModel();
                  test.nbreEvaluations = Convert.ToInt32(nbreEvaluations);
            if (NbrePresentation != 0)
            {
                test.presentation = (float)Math.Round((totPresentation / (NbrePresentation * 5)) * 100, 2);
            }
            else { test.presentation = -5; }
            if (NbreRespectScript != 0)
            {
                test.respectScript = (float)Math.Round((totRespectScript / (NbreRespectScript * 15)) * 100, 2);
            }
            else { test.respectScript = -15; }
            if (NbreTraitementObjections != 0)
            {
                test.traitementObjections = (float)Math.Round((totTraitementObjections / (NbreTraitementObjections * 5)) * 100, 2);
            }
            else { test.traitementObjections = -5; }
            if (NbreExploitationOutils != 0)
            {
                test.exploitationOutils = (float)Math.Round((totExploitationOutils / (NbreExploitationOutils * 5)) * 100, 2);
            }
            else { test.exploitationOutils = -5; }
            if (NbreHistorisationOutils != 0)
            {
                test.HistorisationOutils = (float)Math.Round((totHistorisationOutils / (NbreHistorisationOutils * 15)) * 100, 2);
            }
            else { test.HistorisationOutils = -15; }
            if (NbreDiscours != 0)
            {
                test.discours = (float)Math.Round((totDiscours / (NbreDiscours * 10)) * 100, 2);
            }
            else { test.discours = -10; }
            if (NbreDebit != 0)
            {
                test.debit = (float)Math.Round((totDebit / (NbreDebit * 3)) * 100, 2);
            }
            else { test.debit = -3; }
            if (NbreIntonation != 0)
            {
                test.intonation = (float)Math.Round((totIntonation / (NbreIntonation * 3)) * 100, 2);
            }
            else { test.intonation = -3; }

            if (NbreEcouteReformulation != 0)
            {
                test.ecouteReformulation = (float)Math.Round((totEcouteReformulation / (NbreEcouteReformulation * 5)) * 100, 2);
            }
            else { test.ecouteReformulation = -5; }
            if (NbrePriseConge != 0)
            {
                test.priseConge = (float)Math.Round((totPriseConge / (NbrePriseConge * 5)) * 100, 2);
            }
            else { test.priseConge = -5; }
           
            if (nbreEvaluations != 0)
            {
                test.note = (float)Math.Round((totNotes / (nbreEvaluations * 100)) * 100, 2);
            }
            else
            {
                test.note = 0;
            }
            return Json(test, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Reports SAM RC Front Office
        public ActionResult ReportsSAMRCFO()
        {
            List<Employee> logins = new List<Employee>();
            EvaluationViewModel evaluation = new EvaluationViewModel();
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
                if (!test.UserName.Equals(user.UserName) && test.Roles.Any(r => r.UserId == test.Id && r.RoleId == 1009))
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
        public JsonResult GetReportsSAMRCFO(string username, string dateDebut, string dateFin)
        {
            float totAccueil = 0, totDecouverteAttentes = 0, totUtilisationOutils = 0, totMiseAttente = 0,
                totTempsAttente = 0, totPertinenceReponse = 0, totConclusionContact = 0, totDiscours = 0, totAttitude = 0,
                totHistorisation = 0 ,totPriseConge = 0, totNotes = 0;

            float NbreMiseAttente = 0, NbreTempsAttente = 0;
               
            DateTime daterecherchedebut = DateTime.Parse(dateDebut);
            DateTime daterecherchefin = DateTime.Parse(dateFin);

            var historstions = serviceFOSAMRCS.GetEvaluationsBetweenTwoDates(daterecherchedebut, daterecherchefin);
            if (username != "")
            {
                Employee emp = serviceEmployee.getByLogin(username);
                historstions = serviceFOSAMRCS.GetEvaluationsEmployeeBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
            }
            float nbreEvaluations = historstions.Count();
            ViewBag.nbreEvaluations = nbreEvaluations;
            foreach (var item in historstions)
            {
                totNotes += item.note;
                totAccueil += item.accueil;
                totDecouverteAttentes += item.decouverteAttentes;
                totUtilisationOutils += item.utilisationOutils;
                totPertinenceReponse += item.pertinenceReponse;
                totConclusionContact += item.conclusionContact;
                totDiscours += item.discours;
                totAttitude += item.attitude;
                totHistorisation += item.historisation;
                totPriseConge += item.priseConge;

                if (item.miseAttente >= 0)
                {
                    totMiseAttente += item.miseAttente;
                    NbreMiseAttente += 1;
                }
                if (item.tempsAttente >= 0)
                {
                    totTempsAttente += item.tempsAttente;
                    NbreTempsAttente += 1;
                }        

            }

            EvaluationEvaluationSCAutoViewModel test = new EvaluationEvaluationSCAutoViewModel();

            if (NbreMiseAttente != 0)
            {
                test.miseAttente = (float)Math.Round((totMiseAttente / (NbreMiseAttente * 2)) * 100, 2);
            }
            else { test.miseAttente = -2; }
            if (NbreTempsAttente != 0)
            {
                test.tempsAttente = (float)Math.Round((totTempsAttente / (NbreTempsAttente * 2)) * 100, 2);
            }
            else { test.tempsAttente = -2; }
            test.nbreEvaluations = Convert.ToInt32(nbreEvaluations);
            if (nbreEvaluations != 0)
            {
                test.note = (float)Math.Round((totNotes / (nbreEvaluations * 100)) * 100, 2);
                test.accueil = (float)Math.Round((totAccueil / (nbreEvaluations * 2)) * 100, 2);
                test.decouverteAttentes = (float)Math.Round((totDecouverteAttentes / (nbreEvaluations * 4)) * 100, 2);
                test.utilisationOutils = (float)Math.Round((totUtilisationOutils / (nbreEvaluations * 5)) * 100, 2);
                test.pertinenceReponse = (float)Math.Round((totPertinenceReponse / (nbreEvaluations * 6)) * 100, 2);
                test.conclusionContact = (float)Math.Round((totConclusionContact / (nbreEvaluations * 3)) * 100, 2);
                test.discours = (float)Math.Round((totDiscours / (nbreEvaluations * 4)) * 100, 2);
                test.attitude = (float)Math.Round((totAttitude / (nbreEvaluations * 4)) * 100, 2);
                test.historisation = (float)Math.Round((totHistorisation / (nbreEvaluations * 6)) * 100, 2);
                test.priseConge = (float)Math.Round((totPriseConge / (nbreEvaluations * 2)) * 100, 2);
            }
            else
            {
                test.note = 0;
                test.accueil = 0;
                test.decouverteAttentes = 0;
                test.utilisationOutils = 0;
                test.pertinenceReponse = 0;
                test.conclusionContact = 0;
                test.discours = 0;
                test.attitude = 0;
                test.historisation = 0;
                test.priseConge = 0;
            }
            return Json(test, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Reports SAM RC Front Office
        public ActionResult ReportsSAMRCBO()
        {
            List<Employee> logins = new List<Employee>();
            EvaluationEvaluationSCAutoViewModel evaluation = new EvaluationEvaluationSCAutoViewModel();
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
                if (!test.UserName.Equals(user.UserName) && test.Roles.Any(r => r.UserId == test.Id && r.RoleId == 1009))
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
     
        public JsonResult GetReportsSAMRCBO(string username, string dateDebut, string dateFin)
        {
            float totIdentificationClient = 0, totQualificationDemandes = 0, totPertinenceReponse = 0, totDiscours = 0, totNotes = 0;
          
            DateTime daterecherchedebut = DateTime.Parse(dateDebut);
            DateTime daterecherchefin = DateTime.Parse(dateFin);

            var historstions = serviceBOSAMRC.GetEvaluationsBetweenTwoDates(daterecherchedebut, daterecherchefin);
            if (username != "")
            {
                Employee emp = serviceEmployee.getByLogin(username);
                historstions = serviceBOSAMRC.GetEvaluationsEmployeeBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
            }
            float nbreEvaluations = historstions.Count();
            ViewBag.nbreEvaluations = nbreEvaluations;
            foreach (var item in historstions)
            {
                totNotes += item.note;
                totIdentificationClient += item.identificationClient;
                totQualificationDemandes += item.qualificationDemandes;
                totPertinenceReponse += item.pertinenceReponse;
                totDiscours += item.discours;
            }

            EvaluationEvaluationSCAutoViewModel test = new EvaluationEvaluationSCAutoViewModel();
            test.nbreEvaluations = Convert.ToInt32(nbreEvaluations);
            if (nbreEvaluations != 0)
            {
                test.note = (float)Math.Round((totNotes / (nbreEvaluations * 100)) * 100, 2);
                test.identificationClient = (float)Math.Round((totIdentificationClient / (nbreEvaluations * 5)) * 100, 2);
                test.qualificationDemandes = (float)Math.Round((totQualificationDemandes / (nbreEvaluations * 5)) * 100, 2);
                test.pertinenceReponse = (float)Math.Round((totPertinenceReponse / (nbreEvaluations * 15)) * 100, 2);
                test.discours = (float)Math.Round((totDiscours / (nbreEvaluations * 5)) * 100, 2);
            }
            else
            {
                test.note = 0;
                test.identificationClient = 0;
                test.qualificationDemandes = 0;
                test.pertinenceReponse = 0;
                test.discours = 0;
            }
            //if (Request.IsAjaxRequest())
            //{
            //    return PartialView("ReportingSAMRCBOTablePartialView", test);
            //}

            //return RedirectToAction("ReportsSAMRCBO");
            return Json(test, JsonRequestBehavior.AllowGet);
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
    }
}
