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
    [Authorize(Roles = "Qualité,Agent Qualité,Agent Qualité_CustomerService,Agent Qualité_Diffusion,Agent,Manager,SuperManager")]

    public class EvaluationController : Controller
    {
        private ReportContext db = new ReportContext();
        // GET: Evaluation
        #region globalVariable
        IEmployeeService serviceEmployee;
        IGrilleEvaluationService service;
        IGroupeEmployeeService serviceGroupeEmp;
        IGroupeService serviceGroupe;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        #endregion
        #region constructor
        public EvaluationController()
        {
            serviceEmployee = new EmployeeService();
            service = new GrilleEvaluationService();
            serviceGroupeEmp = new GroupesEmployeService();
            serviceGroupe = new GroupeService();
        }
        public EvaluationController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
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

        #region getEvaluations
        [Authorize(Roles = "Qualité,Agent Qualité,Agent Qualité_Diffusion")]
        [HttpGet]
        public ActionResult Reab(string enregistrementFullName)
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
            var logins = serviceGroupeEmp.getListEmployeeByGroupe(user.Id);
            var employees = logins.Select(o => o).Distinct().ToList();
            var uspseudoNames = logins.Select(o => o.pseudoName).Distinct().ToList();
            List<SelectListItem> agents = new List<SelectListItem>();
            var a = new EvaluationViewModel();
            foreach (var emp in employees)
            {
                if (!emp.UserName.Equals(user.UserName))
                {
                    agents.Add(new SelectListItem { Text = emp.UserName, Value = emp.UserName });
                }
            }
            a.agents = agents;
            a.enregistrementFullName = enregistrementFullName;
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
        public ActionResult Promo()
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
            var logins = serviceGroupeEmp.getListEmployeeByGroupe(user.Id);
            var employees = logins.Select(o => o).Distinct().ToList();
            var uspseudoNames = logins.Select(o => o.pseudoName).Distinct().ToList(); List<SelectListItem> agents = new List<SelectListItem>();
            var a = new EvaluationViewModel();
            foreach (var emp in employees)
            {
                if (!emp.UserName.Equals(user.UserName))
                {
                    agents.Add(new SelectListItem { Text = emp.UserName, Value = emp.UserName });
                }
            }
            a.agents = agents;
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
        #endregion
        #region postEvaluations
        [Authorize(Roles = "Qualité,AgentQualité,Agent Qualité_Diffusion")]
        [HttpPost, ActionName("Reab")]
        public ActionResult ReabTest(FormCollection form)
        {
            string username = form["typeGenerator"].ToString();
            string acceuil = form["acceuil"].ToString();
            string objetAppel = form["objetAppel"].ToString();
            string presentation = form["presentation"].ToString();
            string objections = form["objections"].ToString();
            string conclusion = form["conclusion"].ToString();
            string cross = form["cross"].ToString();
            string discours = form["discours"].ToString();
            string attitude = form["attitude"].ToString();
            string conge = form["conge"].ToString();
            string dateTest = form["planDate"].ToString();
            float notes = float.Parse(acceuil, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(objetAppel, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(presentation, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(objections, CultureInfo.InvariantCulture.NumberFormat) +
                float.Parse(conclusion, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(cross, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(discours, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(attitude, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(conge, CultureInfo.InvariantCulture.NumberFormat);
            DateTime dateCreationtest = DateTime.Parse(dateTest);
            GrilleEvaluation eval = new GrilleEvaluation();
            Employee emp = serviceEmployee.getByLogin(username);
            eval.acceuilPresentation = float.Parse(acceuil, CultureInfo.InvariantCulture.NumberFormat);
            eval.objetAppel = float.Parse(objetAppel, CultureInfo.InvariantCulture.NumberFormat);
            eval.presentationOffre = float.Parse(presentation, CultureInfo.InvariantCulture.NumberFormat);
            eval.gestionObjection = float.Parse(objections, CultureInfo.InvariantCulture.NumberFormat);
            eval.vCContrat = float.Parse(conclusion, CultureInfo.InvariantCulture.NumberFormat);
            eval.pCross = float.Parse(cross, CultureInfo.InvariantCulture.NumberFormat);
            eval.discours = float.Parse(discours, CultureInfo.InvariantCulture.NumberFormat);
            eval.attitude = float.Parse(attitude, CultureInfo.InvariantCulture.NumberFormat);
            eval.priseConge = float.Parse(conge, CultureInfo.InvariantCulture.NumberFormat);

            eval.dateTempEvaluation = dateCreationtest;
            eval.employeeId = emp.Id;
            eval.note = notes;
            service.Add(eval);
            service.SaveChange();

            return RedirectToAction("Acceuil", "Directory");
        }
        [HttpPost, ActionName("Promo")]
        public ActionResult PromoTest(FormCollection form)
        {
            string username = form["typeGenerator"].ToString();
            string acceuil = form["acceuil"].ToString();
            string objetAppel = form["objetAppel"].ToString();
            string decouverte = form["decouverte"].ToString();
            string presentation = form["presentation"].ToString();
            string objections = form["objections"].ToString();
            string conclusion = form["conclusion"].ToString();
            string cross = form["cross"].ToString();
            string discours = form["discours"].ToString();
            string attitude = form["attitude"].ToString();
            string conge = form["conge"].ToString();
            string dateTest = form["planDate"].ToString();
            //string description = form["description"].ToString();
            float dis = float.Parse(discours, CultureInfo.InvariantCulture.NumberFormat);

            float notes = float.Parse(acceuil, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(objetAppel, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(decouverte, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(presentation, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(objections, CultureInfo.InvariantCulture.NumberFormat) +
                float.Parse(conclusion, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(cross, CultureInfo.InvariantCulture.NumberFormat) + dis + float.Parse(attitude, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(conge, CultureInfo.InvariantCulture.NumberFormat);
            DateTime dateCreationtest = DateTime.Parse(dateTest);
            GrilleEvaluation eval = new GrilleEvaluation();
            Employee emp = serviceEmployee.getByLogin(username);
            eval.acceuilPresentation = float.Parse(acceuil, CultureInfo.InvariantCulture.NumberFormat);
            eval.objetAppel = float.Parse(objetAppel, CultureInfo.InvariantCulture.NumberFormat);
            eval.ppOffre = float.Parse(presentation, CultureInfo.InvariantCulture.NumberFormat);
            eval.gestionObjection = float.Parse(objections, CultureInfo.InvariantCulture.NumberFormat);
            eval.vCContrat = float.Parse(conclusion, CultureInfo.InvariantCulture.NumberFormat);
            eval.pCross = float.Parse(cross, CultureInfo.InvariantCulture.NumberFormat);
            eval.discours = float.Parse(discours, CultureInfo.InvariantCulture.NumberFormat);
            eval.attitude = float.Parse(attitude, CultureInfo.InvariantCulture.NumberFormat);
            eval.priseConge = float.Parse(conge, CultureInfo.InvariantCulture.NumberFormat);
            eval.decouverteBesoins = float.Parse(decouverte, CultureInfo.InvariantCulture.NumberFormat);
            eval.dateTempEvaluation = dateCreationtest;
            eval.employeeId = emp.Id;
            eval.note = notes;
            //eval.commentaireQualite = description;
            service.Add(eval);
            service.SaveChange();
            var a = new EvaluationViewModel();
            return PartialView("ResultPartialView", a);
        }

        public ActionResult PostReab(string username, string acceuil, string objetAppel, string decouverte, string presentation, string objections, string conclusion, string cross, string discours, string attitude, string conge, string dateTest, string description)
        {
            float notes = float.Parse(acceuil, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(objetAppel, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(presentation, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(objections, CultureInfo.InvariantCulture.NumberFormat) +
               float.Parse(conclusion, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(cross, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(discours, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(attitude, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(conge, CultureInfo.InvariantCulture.NumberFormat);
            DateTime dateCreationtest = DateTime.Parse(dateTest);
            GrilleEvaluation eval = new GrilleEvaluation();
            Employee emp = serviceEmployee.getByLogin(username);
            eval.acceuilPresentation = float.Parse(acceuil, CultureInfo.InvariantCulture.NumberFormat);
            eval.objetAppel = float.Parse(objetAppel, CultureInfo.InvariantCulture.NumberFormat);
            eval.presentationOffre = float.Parse(presentation, CultureInfo.InvariantCulture.NumberFormat);
            eval.gestionObjection = float.Parse(objections, CultureInfo.InvariantCulture.NumberFormat);
            eval.vCContrat = float.Parse(conclusion, CultureInfo.InvariantCulture.NumberFormat);
            eval.pCross = float.Parse(cross, CultureInfo.InvariantCulture.NumberFormat);
            eval.discours = float.Parse(discours, CultureInfo.InvariantCulture.NumberFormat);
            eval.attitude = float.Parse(attitude, CultureInfo.InvariantCulture.NumberFormat);
            eval.priseConge = float.Parse(conge, CultureInfo.InvariantCulture.NumberFormat);

            eval.dateTempEvaluation = dateCreationtest;
            eval.employeeId = emp.Id;
            eval.type = "Reab";
            eval.note = notes;
            eval.commentaireQualite = description;
            //service.Add(eval);
            //service.SaveChange();
            var a = new EvaluationViewModel();
            a.note = eval.note;
            a.pourcentageNotes = (notes / 165) * 100;
            a.acceuilPresentation = eval.acceuilPresentation;
            a.objetAppel = eval.objetAppel;
            a.presentationOffre = eval.presentationOffre;
            a.gestionObjection = eval.gestionObjection;
            a.vCContrat = eval.vCContrat;
            a.pCross = eval.pCross;
            a.discours = eval.discours;
            a.attitude = eval.attitude;
            a.priseConge = eval.priseConge;
            a.dateTempEvaluation = eval.dateTempEvaluation;
            a.agentName = username;

            if (Request.IsAjaxRequest())
            {
                //if (Directory.Exists(@utilisateurs) == false)
                //{
                //    return PartialView("DirectoryError", a);

                //}
                //else 

                return PartialView("ResultPartialView", a);


            }
            return RedirectToAction("Acceuil", "Directory");

        }

        public ActionResult PostReab1(string username, string acceuil, string objetAppel, string decouverte, string presentation, string objections, string conclusion, string cross, string discours, string attitude, string conge, string dateTest, string description)
        {
            var userConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            float notes = float.Parse(acceuil, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(objetAppel, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(presentation, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(objections, CultureInfo.InvariantCulture.NumberFormat) +
            float.Parse(conclusion, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(cross, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(discours, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(attitude, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(conge, CultureInfo.InvariantCulture.NumberFormat);
            DateTime dateCreationtest = DateTime.Parse(dateTest);
            GrilleEvaluation eval = new GrilleEvaluation();
            Employee emp = serviceEmployee.getByLogin(username);
            eval.acceuilPresentation = float.Parse(acceuil, CultureInfo.InvariantCulture.NumberFormat);
            eval.objetAppel = float.Parse(objetAppel, CultureInfo.InvariantCulture.NumberFormat);
            eval.presentationOffre = float.Parse(presentation, CultureInfo.InvariantCulture.NumberFormat);
            eval.gestionObjection = float.Parse(objections, CultureInfo.InvariantCulture.NumberFormat);
            eval.vCContrat = float.Parse(conclusion, CultureInfo.InvariantCulture.NumberFormat);
            eval.pCross = float.Parse(cross, CultureInfo.InvariantCulture.NumberFormat);
            eval.discours = float.Parse(discours, CultureInfo.InvariantCulture.NumberFormat);
            eval.attitude = float.Parse(attitude, CultureInfo.InvariantCulture.NumberFormat);
            eval.priseConge = float.Parse(conge, CultureInfo.InvariantCulture.NumberFormat);

            eval.dateTempEvaluation = dateCreationtest;
            eval.employeeId = emp.Id;
            eval.senderId = userConnected.Id;
            eval.type = "Reab";
            eval.note = notes;
            eval.commentaireQualite = description;
            service.Add(eval);
            service.SaveChange();
            var a = new EvaluationViewModel();
            a.note = eval.note;
            a.pourcentageNotes = (notes / 165) * 100;
            a.acceuilPresentation = eval.acceuilPresentation;
            a.objetAppel = eval.objetAppel;
            a.presentationOffre = eval.presentationOffre;
            a.gestionObjection = eval.gestionObjection;
            a.vCContrat = eval.vCContrat;
            a.pCross = eval.pCross;
            a.discours = eval.discours;
            a.attitude = eval.attitude;
            a.priseConge = eval.priseConge;
            a.dateTempEvaluation = eval.dateTempEvaluation;
            a.agentName = username;
            a.senderName = userConnected.UserName;
            try
            {
                SmtpMail oMail = new SmtpMail("TryIt");
                EASendMail.SmtpClient oSmtp = new EASendMail.SmtpClient();

                // l"adresse Email d'emetteur 
                oMail.From = "Sana.BENSALAH@infopro-digital.com";

                //adresse destinataire
                oMail.To = emp.Email;
                // Objet Mail

                oMail.Subject = "Evaluation pour " + username;
                //contenu du mail designed avec HTML
                //oMail.HtmlBody = "<table><tr><td><h3 style='color:red;'>Résultat Evaluation:</h3><div><h5 style='color:#1E88E5;'>Date et temps d'évaluation :</h5><h5>" + dateCreationtest + "</h5></div><h5 style='color:#1E88E5;'>Acceuil / Presentation  :  " + acceuil+ "</h5><h5 style='color:#1E88E5;'>Objet d'appel : " + objetAppel+ "</h5><h5 style='color:#1E88E5;'>Présentation de l'offre / valider la satisfaction client: " + a.presentationOffre + "</h5><h5 style='color:#1E88E5;'>Gestion des objections : " + a.gestionObjection+ "</h5><h5 style='color:#1E88E5;'>Verrouillage et conclusion du contact : " + a.vCContrat+ "</h5><h5 style='color:#1E88E5;'>Proposition Cross : " + a.pCross+ "</h5><h5 style='color:#1E88E5;'>Discours : " + a.discours+ "</h5><h5 style='color:#1E88E5;'>Attitude : " + a.attitude+ "</h5><h5 style='color:#1E88E5;'>votre score est <font clolor ='red;'>" + a.pourcentageNotes + " %</font></h5>";

                oMail.HtmlBody = "<html><body><p>Bonjour,<p><p>Vous avez une évaluation de la part du qualité </p><p>Cordialement.</p></body></html>";

                //oMail.TextBody = "Vous venez d'étre évalué sur vos enregistrements ";

                // Instance du serveur SMTP et commpe parametre son Adresse.
                SmtpServer oServer = new SmtpServer("smtp.info.local");

                // username , password d'émetteur
                oServer.User = "Sana.BENSALAH@infopro-digital.com";
                oServer.Password = "Welcome01";

                // associer ou port  25 ou bien 587.
                oServer.Port = 25;

                // detect TLS connection automatically
                oServer.ConnectType = SmtpConnectType.ConnectSSLAuto;
                oSmtp.SendMail(oServer, oMail);
            }

            catch (SmtpException)
            {
                ViewBag.msg = "mail not sent";
                return RedirectToAction("Promo");
            }
            if (Request.IsAjaxRequest())
            {
                //    //if (Directory.Exists(@utilisateurs) == false)
                //    //{
                //    //    return PartialView("DirectoryError", a);

                //    //}
                //    //else 

                return PartialView("EnvoiMailPartialView", a);

            }
            return RedirectToAction("Acceuil", "Directory");

        }

        public ActionResult PostPromo(string username, string acceuil, string objetAppel, string decouverte, string presentation, string objections, string conclusion, string cross, string discours, string attitude, string conge, string dateTest, string description)
        {
            float notes = float.Parse(acceuil, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(objetAppel, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(decouverte, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(presentation, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(objections, CultureInfo.InvariantCulture.NumberFormat) +
               float.Parse(conclusion, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(cross, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(discours, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(attitude, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(conge, CultureInfo.InvariantCulture.NumberFormat);
            DateTime dateCreationtest = DateTime.Parse(dateTest);
            GrilleEvaluation eval = new GrilleEvaluation();
            Employee emp = serviceEmployee.getByLogin(username);
            eval.acceuilPresentation = float.Parse(acceuil, CultureInfo.InvariantCulture.NumberFormat);
            eval.objetAppel = float.Parse(objetAppel, CultureInfo.InvariantCulture.NumberFormat);
            eval.ppOffre = float.Parse(presentation, CultureInfo.InvariantCulture.NumberFormat);
            eval.gestionObjection = float.Parse(objections, CultureInfo.InvariantCulture.NumberFormat);
            eval.vCContrat = float.Parse(conclusion, CultureInfo.InvariantCulture.NumberFormat);
            eval.pCross = float.Parse(cross, CultureInfo.InvariantCulture.NumberFormat);
            eval.discours = float.Parse(discours, CultureInfo.InvariantCulture.NumberFormat);
            eval.attitude = float.Parse(attitude, CultureInfo.InvariantCulture.NumberFormat);
            eval.priseConge = float.Parse(conge, CultureInfo.InvariantCulture.NumberFormat);
            eval.decouverteBesoins = float.Parse(decouverte, CultureInfo.InvariantCulture.NumberFormat);
            eval.dateTempEvaluation = dateCreationtest;
            eval.employeeId = emp.Id;
            eval.note = notes;
            eval.type = "Promo";
            eval.commentaireQualite = description;
            //service.Add(eval);
            //service.SaveChange();
            var a = new EvaluationViewModel();
            a.note = eval.note;
            a.pourcentageNotes = (notes / 190) * 100;
            a.acceuilPresentation = eval.acceuilPresentation;
            a.decouverteBesoins = eval.decouverteBesoins;
            a.objetAppel = eval.objetAppel;
            a.presentationOffre = eval.presentationOffre;
            a.ppOffre = eval.ppOffre;
            a.gestionObjection = eval.gestionObjection;
            a.vCContrat = eval.vCContrat;
            a.pCross = eval.pCross;
            a.discours = eval.discours;
            a.attitude = eval.attitude;
            a.priseConge = eval.priseConge;
            a.dateTempEvaluation = eval.dateTempEvaluation;
            a.agentName = username;

            if (Request.IsAjaxRequest())
            {
                //if (Directory.Exists(@utilisateurs) == false)
                //{
                //    return PartialView("DirectoryError", a);

                //}
                //else 

                return PartialView("ResultPartialViewPromo", a);


            }
            return RedirectToAction("Acceuil", "Directory");

        }

        public ActionResult PostPromo1(string username, string acceuil, string objetAppel, string decouverte, string presentation, string objections, string conclusion, string cross, string discours, string attitude, string conge, string dateTest, string description)
        {
            var userConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            float notes = float.Parse(acceuil, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(objetAppel, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(decouverte, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(presentation, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(objections, CultureInfo.InvariantCulture.NumberFormat) +
               float.Parse(conclusion, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(cross, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(discours, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(attitude, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(conge, CultureInfo.InvariantCulture.NumberFormat);
            DateTime dateCreationtest = DateTime.Parse(dateTest);
            GrilleEvaluation eval = new GrilleEvaluation();
            Employee emp = serviceEmployee.getByLogin(username);
            eval.acceuilPresentation = float.Parse(acceuil, CultureInfo.InvariantCulture.NumberFormat);
            eval.objetAppel = float.Parse(objetAppel, CultureInfo.InvariantCulture.NumberFormat);
            eval.ppOffre = float.Parse(presentation, CultureInfo.InvariantCulture.NumberFormat);
            eval.gestionObjection = float.Parse(objections, CultureInfo.InvariantCulture.NumberFormat);
            eval.vCContrat = float.Parse(conclusion, CultureInfo.InvariantCulture.NumberFormat);
            eval.pCross = float.Parse(cross, CultureInfo.InvariantCulture.NumberFormat);
            eval.discours = float.Parse(discours, CultureInfo.InvariantCulture.NumberFormat);
            eval.attitude = float.Parse(attitude, CultureInfo.InvariantCulture.NumberFormat);
            eval.priseConge = float.Parse(conge, CultureInfo.InvariantCulture.NumberFormat);
            eval.decouverteBesoins = float.Parse(decouverte, CultureInfo.InvariantCulture.NumberFormat);
            eval.dateTempEvaluation = dateCreationtest;
            eval.employeeId = emp.Id;
            eval.senderId = userConnected.Id;
            eval.note = notes;
            eval.type = "Promo";
            eval.commentaireQualite = description;
            service.Add(eval);
            service.SaveChange();
            var a = new EvaluationViewModel();
            a.note = eval.note;
            a.pourcentageNotes = (notes / 190) * 100;
            a.acceuilPresentation = eval.acceuilPresentation;
            a.decouverteBesoins = eval.decouverteBesoins;
            a.objetAppel = eval.objetAppel;
            a.presentationOffre = eval.presentationOffre;
            a.ppOffre = eval.ppOffre;
            a.gestionObjection = eval.gestionObjection;
            a.vCContrat = eval.vCContrat;
            a.pCross = eval.pCross;
            a.discours = eval.discours;
            a.attitude = eval.attitude;
            a.priseConge = eval.priseConge;
            a.dateTempEvaluation = eval.dateTempEvaluation;
            a.agentName = username;
            a.senderName = userConnected.UserName;
            try
            {
                SmtpMail oMail = new SmtpMail("TryIt");
                EASendMail.SmtpClient oSmtp = new EASendMail.SmtpClient();

                // l"adresse Email d'emetteur 
                oMail.From = "Sana.BENSALAH@infopro-digital.com";

                //adresse destinataire
                oMail.To = emp.Email;
                // Objet Mail

                oMail.Subject = "Evaluation pour " + username;
                //contenu du mail designed avec HTML
                //oMail.HtmlBody = "<table><tr><td><h3 style='color:red;'>Résultat Evaluation:</h3><div><h5 style='color:#1E88E5;'>Date et temps d'évaluation :</h5><h5>" + dateCreationtest + "</h5></div><h5 style='color:#1E88E5;'>Acceuil / Presentation  :  " + acceuil+ "</h5><h5 style='color:#1E88E5;'>Objet d'appel : " + objetAppel+ "</h5><h5 style='color:#1E88E5;'>Présentation de l'offre / valider la satisfaction client: " + a.presentationOffre + "</h5><h5 style='color:#1E88E5;'>Gestion des objections : " + a.gestionObjection+ "</h5><h5 style='color:#1E88E5;'>Verrouillage et conclusion du contact : " + a.vCContrat+ "</h5><h5 style='color:#1E88E5;'>Proposition Cross : " + a.pCross+ "</h5><h5 style='color:#1E88E5;'>Discours : " + a.discours+ "</h5><h5 style='color:#1E88E5;'>Attitude : " + a.attitude+ "</h5><h5 style='color:#1E88E5;'>votre score est <font clolor ='red;'>" + a.pourcentageNotes + " %</font></h5>";

                oMail.HtmlBody = "<html><body><p>Bonjour,<p><p>Vous avez une évaluation de la part du qualité </p><p>Cordialement.</p></body></html>";

                // Set email body
                //oMail.TextBody = "Vous venez d'étre évalué sur vos enregistrements ";

                // Instance du serveur SMTP et commpe parametre son Adresse.
                SmtpServer oServer = new SmtpServer("smtp.info.local");

                // username , password d'émetteur
                oServer.User = "Sana.BENSALAH@infopro-digital.com";
                oServer.Password = "Welcome01";

                // associer ou port  25 ou bien 587.
                oServer.Port = 587;

                // detect TLS connection automatically
                oServer.ConnectType = SmtpConnectType.ConnectSSLAuto;
                oSmtp.SendMail(oServer, oMail);

                ViewBag.msg = "mail sent";
            }
            catch (SmtpException)
            {
                ViewBag.msg = "mail not sent";
                return RedirectToAction("Promo");
            }
            if (Request.IsAjaxRequest())
            {
                //if (Directory.Exists(@utilisateurs) == false)
                //{
                //    return PartialView("DirectoryError", a);

                //}
                //else 

                return PartialView("EnvoiMailPartialViewPromo", a);

            }
            return RedirectToAction("Acceuil", "Directory");

        }
        #endregion
        #region simpleCRUD
        // GET: Evaluation/Details/5
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Evaluation/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Evaluation/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Evaluation/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }



        // GET: Evaluation/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Evaluation/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        #endregion
        #region historique Par Agent
        [Authorize(Roles = "Qualité,Agent Qualité_Diffusion, Manager,SuperManager")]
        //Historique Diffusion
        public ActionResult Historique()
        {
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
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 2))
            {
                ViewBag.role = "Manager";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 6))
            {
                ViewBag.role = "Manager";
            }
            List<Employee> logins = new List<Employee>();
            //var logins = serviceGroupeEmp.getListEmployeeByGroupe(user.Id);
            //var employees = logins.Select(o => o).Distinct().ToList();
            //var ordredByPseudoNames = employees.OrderBy(e => e.UserName).ToList();
            var evaluations = service.GetAll();
            List<string> DiffGroupes = new List<string>(new string[] { "GISI-REAB", "GISI-PROMO", "GMT-REAB", "GMT-PROMO" });
            foreach (var d in DiffGroupes)
            {
                Groupe gr = serviceGroupe.getByNom(d);
                List<Employee> emp = serviceGroupeEmp.getListEmployeeByGroupeId(gr.Id);
                foreach(var e in emp){
                    if (!logins.Exists(l => l.UserName == e.UserName))
                    {
                        logins.Add(e);
                    } }
            }
          //  var us = logins.Select(o => o).Distinct().ToList();
           var employees = logins.OrderBy(a => a.UserName).ToList();
            foreach (var test in employees)
            {
                if (!test.UserName.Equals(user.UserName) && test.Roles.Any(r => r.UserId == test.Id && r.RoleId == 3))
                {
                    evaluation.employees.Add(new SelectListItem { Text = test.UserName, Value = test.UserName });

                }
            }
            evaluation.evaluations = (List<GrilleEvaluation>)evaluations;
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

        public ActionResult GetHisto(string username)
        {
            List<EvaluationViewModel> a = new List<EvaluationViewModel>();
            try
            {
                Employee emp = serviceEmployee.getByLogin(username);

                var historstions = service.GetEvaluationsByEmployee(emp.Id);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationViewModel test = new EvaluationViewModel();
                    test.acceuilPresentation = item.acceuilPresentation;
                    test.objetAppel = item.objetAppel;
                    test.presentationOffre = item.presentationOffre;
                    test.gestionObjection = item.gestionObjection;
                    test.vCContrat = item.vCContrat;
                    test.pCross = item.pCross;
                    test.discours = item.discours;
                    test.attitude = item.attitude;
                    test.priseConge = item.priseConge;
                    test.decouverteBesoins = item.decouverteBesoins;
                    test.ppOffre = item.ppOffre;
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
                    return PartialView("HistoriqueTablePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("Historique");
        }

        public ActionResult GetHistoByDate(string username, string dateDebut, string dateFin)
        {
            List<EvaluationViewModel> a = new List<EvaluationViewModel>();
            try
            {
                Employee emp = serviceEmployee.getByLogin(username);
                DateTime daterecherchedebut = DateTime.Parse(dateDebut);
                DateTime daterecherchefin = DateTime.Parse(dateFin);


                var historstions = service.GetEvaluationsEmployeeBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationViewModel test = new EvaluationViewModel();
                    test.acceuilPresentation = item.acceuilPresentation;
                    test.objetAppel = item.objetAppel;
                    test.presentationOffre = item.presentationOffre;
                    test.gestionObjection = item.gestionObjection;
                    test.vCContrat = item.vCContrat;
                    test.pCross = item.pCross;
                    test.discours = item.discours;
                    test.attitude = item.attitude;
                    test.priseConge = item.priseConge;
                    test.decouverteBesoins = item.decouverteBesoins;
                    test.ppOffre = item.ppOffre;
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
                    return PartialView("HistoriqueTablePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("Historique");
        }
        public JsonResult GetEmployees(string selectedSite)
        {
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            List<SelectListItem> EmplyeesList = new List<SelectListItem>();
            List<Employee> employees = new List<Employee>();
            List<Employee> logins = new List<Employee>();
            if (selectedSite == "DIFFUSION")
            {
                List<string> DiffGroupes = new List<string>(new string[] { "GISI-REAB", "GISI-PROMO","GMT-REAB", "GMT-PROMO" });
                foreach (var d in DiffGroupes)
                {
                    Groupe gr = serviceGroupe.getByNom(d);
                    List<Employee> emp = serviceGroupeEmp.getListEmployeeByGroupeId(gr.Id);
                    logins.AddRange(emp);            
                }
            }
            else
            {
                Groupe gr = serviceGroupe.getByNom(selectedSite);
                 logins = serviceGroupeEmp.getListEmployeeByGroupeId(gr.Id);             
            }
            var us = logins.Select(o => o).Distinct().ToList();
            employees = us.OrderBy(a => a.UserName).ToList();
            foreach (var e in employees)
            {
                if (!e.UserName.Equals(user.UserName) && (e.Roles.Any(r => r.UserId == e.Id && r.RoleId == 3)))
                {
                    EmplyeesList.Add(new SelectListItem { Text = e.UserName, Value = e.UserName });
                }
            }
        
            return Json(new SelectList(EmplyeesList, "Value", "Text"));
        }
        #endregion
        #region historiqueAgent
        [Authorize(Roles = "Agent")]
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
            //string type = form["typeGenerator"].ToString();
            var logins = serviceGroupeEmp.getListEmployeeByGroupe(user.Id);
            var employees = logins.Select(o => o).Distinct().ToList();
            var uspseudoNames = logins.Select(o => o.pseudoName).Distinct().ToList();
            var evaluations = service.GetAll();

            foreach (var test in employees)
            {
                if (!test.UserName.Equals(user.UserName))
                {
                    evaluation.employees.Add(new SelectListItem { Text = test.UserName, Value = test.UserName });

                }
            }
            evaluation.evaluations = (List<GrilleEvaluation>)evaluations;
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
        public ActionResult GetHistoAgent()
        {
            List<EvaluationViewModel> a = new List<EvaluationViewModel>();
            try
            {
                Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

                var historstions = service.GetEvaluationsByEmployee(emp.Id);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationViewModel test = new EvaluationViewModel();
                    test.Id = item.Id;
                    test.acceuilPresentation = item.acceuilPresentation;
                    test.objetAppel = item.objetAppel;
                    test.presentationOffre = item.presentationOffre;
                    test.gestionObjection = item.gestionObjection;
                    test.vCContrat = item.vCContrat;
                    test.pCross = item.pCross;
                    test.discours = item.discours;
                    test.attitude = item.attitude;
                    test.priseConge = item.priseConge;
                    test.decouverteBesoins = item.decouverteBesoins;
                    test.ppOffre = item.ppOffre;
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
                    return PartialView("HistoriqueTableAgentPartialView", a);
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
            List<EvaluationViewModel> a = new List<EvaluationViewModel>();
            try
            {
                Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
                DateTime daterecherchedebut = DateTime.Parse(dateDebut);
                DateTime daterecherchefin = DateTime.Parse(dateFin);
                var historstions = service.GetEvaluationsEmployeeBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationViewModel test = new EvaluationViewModel();
                    test.Id = item.Id;
                    test.acceuilPresentation = item.acceuilPresentation;
                    test.objetAppel = item.objetAppel;
                    test.presentationOffre = item.presentationOffre;
                    test.gestionObjection = item.gestionObjection;
                    test.vCContrat = item.vCContrat;
                    test.pCross = item.pCross;
                    test.discours = item.discours;
                    test.attitude = item.attitude;
                    test.priseConge = item.priseConge;
                    test.decouverteBesoins = item.decouverteBesoins;
                    test.ppOffre = item.ppOffre;
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
                    return PartialView("HistoriqueTableAgentPartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("HistoriqueAgent");
        }
        public ActionResult ReponseEvaluation(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GrilleEvaluation evaluation = db.evaluations.Find(id);
            EvaluationViewModel eval = new EvaluationViewModel();
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
        [HttpPost, ActionName("ReponseEvaluation")]
        [Authorize(Roles = "Agent")]
        public ActionResult ReponseEvaluation(int? id, GrilleEvaluation evaluation)
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

        #region Archive Evaluations du Resp Qualité
        //Archive evaluations par Responsable Qualité
        [Authorize(Roles = "Qualité")]
        public ActionResult ArchiveEvaluationsQualite()
        {
            EvaluationViewModel evaluation = new EvaluationViewModel();
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
           // var logins = serviceGroupeEmp.getListEmployeeByGroupe(user.Id);
            var logins = serviceEmployee.GetAll();
            var employees = logins.Select(o => o).Distinct().ToList();
            var evaluations = service.GetAll();

            foreach (var test in employees)
            {
                if (test.Roles.Any(r => r.UserId == test.Id && r.RoleId == 4 || r.RoleId == 9))
                {
                    evaluation.employees.Add(new SelectListItem { Text = test.UserName, Value = test.UserName });

                }
            }
            evaluation.evaluations = (List<GrilleEvaluation>)evaluations;
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

        public ActionResult GetArchiveEvaluationsQualite(string username)
        {
            List<EvaluationViewModel> a = new List<EvaluationViewModel>();
            try
            {
                Employee emp = serviceEmployee.getByLogin(username);
                var historstions = service.GetEvaluationsBySenderId(emp.Id);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationViewModel test = new EvaluationViewModel();
                    test.acceuilPresentation = item.acceuilPresentation;
                    test.objetAppel = item.objetAppel;
                    test.presentationOffre = item.presentationOffre;
                    test.gestionObjection = item.gestionObjection;
                    test.vCContrat = item.vCContrat;
                    test.pCross = item.pCross;
                    test.discours = item.discours;
                    test.attitude = item.attitude;
                    test.priseConge = item.priseConge;
                    test.decouverteBesoins = item.decouverteBesoins;
                    test.ppOffre = item.ppOffre;
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
                    return PartialView("ArchiveTablePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("ArchiveEvaluationsQualite");
        }

        public ActionResult GetArchiveEvaluationsByDate(string username, string dateDebut, string dateFin)
        {
            List<EvaluationViewModel> a = new List<EvaluationViewModel>();
            try
            {
                Employee emp = serviceEmployee.getByLogin(username);
                DateTime daterecherchedebut = DateTime.Parse(dateDebut);
                DateTime daterecherchefin = DateTime.Parse(dateFin);
                var historstions = service.GetEvaluationsSenderBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationViewModel test = new EvaluationViewModel();
                    test.acceuilPresentation = item.acceuilPresentation;
                    test.objetAppel = item.objetAppel;
                    test.presentationOffre = item.presentationOffre;
                    test.gestionObjection = item.gestionObjection;
                    test.vCContrat = item.vCContrat;
                    test.pCross = item.pCross;
                    test.discours = item.discours;
                    test.attitude = item.attitude;
                    test.priseConge = item.priseConge;
                    test.decouverteBesoins = item.decouverteBesoins;
                    test.ppOffre = item.ppOffre;
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
                    return PartialView("ArchiveTablePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("ArchiveEvaluationsQualite");
        }
        #endregion
        #region evaluationAfterListening Diffusion
        [Authorize(Roles = "Qualité, Agent Qualité,Agent Qualité_Diffusion")]
        public ActionResult Reab2(string enregistrementFullName, string enregistrementUrl, string enregistrementDirectory, string agentName)
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

            //string type = form["typeGenerator"].ToString();
            var logins = serviceGroupeEmp.getListEmployeeByGroupe(user.Id);
            var employees = logins.Select(o => o).Distinct().ToList();
            var uspseudoNames = logins.Select(o => o.pseudoName).Distinct().ToList();
            List<SelectListItem> agents = new List<SelectListItem>();
            var a = new EvaluationViewModel();
            foreach (var emp in employees)
            {
                if (!emp.UserName.Equals(user.UserName))
                {
                    agents.Add(new SelectListItem { Text = emp.UserName, Value = emp.UserName });
                }
            }
            a.agents = agents;
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
        public ActionResult Reab2WithoutAgent(string enregistrementFullName, string enregistrementUrl, string enregistrementDirectory, string siteName)
        {
            List<SelectListItem> agents = new List<SelectListItem>();
            List<Employee> logins = new List<Employee>();
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            var a = new EvaluationViewModel();
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 9))
            {
                ViewBag.role = "Agent Qualité_Diffusion";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 4))
            {
                ViewBag.role = "Qualité";
            }
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
                    agentName = logins[j].pseudoName;
                }
                j++;
            }
            a.agentName = agentName;

            a.enregistrementFullName = enregistrementFullName;
            a.enregistrementUrl = enregistrementUrl;
            a.enregistrementDirectory = enregistrementDirectory;
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
       
        public ActionResult PostReab2(string username, string acceuil, string objetAppel, string decouverte, string presentation, string objections, string conclusion, string cross, string discours, string attitude, string conge, string dateTest, string description, string enregistrement, string enregistrementUrl, string enregistrementDirectory)
        {
            var userConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            float notes = float.Parse(acceuil, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(objetAppel, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(presentation, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(objections, CultureInfo.InvariantCulture.NumberFormat) +
               float.Parse(conclusion, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(cross, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(discours, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(attitude, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(conge, CultureInfo.InvariantCulture.NumberFormat);
            DateTime dateCreationtest = DateTime.Parse(dateTest);
            GrilleEvaluation eval = new GrilleEvaluation();
            Employee emp = serviceEmployee.getByPseudoName(username.ToLower());
            eval.acceuilPresentation = float.Parse(acceuil, CultureInfo.InvariantCulture.NumberFormat);
            eval.objetAppel = float.Parse(objetAppel, CultureInfo.InvariantCulture.NumberFormat);
            eval.presentationOffre = float.Parse(presentation, CultureInfo.InvariantCulture.NumberFormat);
            eval.gestionObjection = float.Parse(objections, CultureInfo.InvariantCulture.NumberFormat);
            eval.vCContrat = float.Parse(conclusion, CultureInfo.InvariantCulture.NumberFormat);
            eval.pCross = float.Parse(cross, CultureInfo.InvariantCulture.NumberFormat);
            eval.discours = float.Parse(discours, CultureInfo.InvariantCulture.NumberFormat);
            eval.attitude = float.Parse(attitude, CultureInfo.InvariantCulture.NumberFormat);
            eval.priseConge = float.Parse(conge, CultureInfo.InvariantCulture.NumberFormat);

            eval.dateTempEvaluation = dateCreationtest;
            eval.employeeId = emp.Id;
            eval.senderId = userConnected.Id;
            eval.type = "Reab";
            eval.note = notes;
            eval.commentaireQualite = description;
            eval.enregistrementFullName = enregistrement;
            eval.enregistrementUrl = enregistrementUrl;
            eval.enregistrementDirectory = enregistrementDirectory;

            service.Add(eval);
            service.SaveChange();
            var a = new EvaluationViewModel();
            a.note = eval.note;
            a.pourcentageNotes = (notes / 165) * 100;
            a.acceuilPresentation = eval.acceuilPresentation;
            a.objetAppel = eval.objetAppel;
            a.presentationOffre = eval.presentationOffre;
            a.gestionObjection = eval.gestionObjection;
            a.vCContrat = eval.vCContrat;
            a.pCross = eval.pCross;
            a.discours = eval.discours;
            a.attitude = eval.attitude;
            a.priseConge = eval.priseConge;
            a.dateTempEvaluation = eval.dateTempEvaluation;
            a.agentName = username;
            a.senderName = userConnected.UserName;
            //try
            //{
            //    SmtpMail oMail = new SmtpMail("TryIt");
            //    EASendMail.SmtpClient oSmtp = new EASendMail.SmtpClient();
           
            //    oMail.Reset();
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
            //    oServer.Password = "sana2019";

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
                //if (Directory.Exists(@utilisateurs) == false)
                //{
                //    return PartialView("DirectoryError", a);

                //}
                //else 

                return PartialView("EnvoiMailPartialView", a);


            }
            return RedirectToAction("Acceuil", "Directory");

        }

        public ActionResult PostReabCalcul2(string username, string acceuil, string objetAppel, string decouverte, string presentation, string objections, string conclusion, string cross, string discours, string attitude, string conge, string dateTest, string description, string enregistrement, string enregistrementUrl, string enregistrementDirectory)
        {
            float notes = float.Parse(acceuil, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(objetAppel, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(presentation, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(objections, CultureInfo.InvariantCulture.NumberFormat) +
               float.Parse(conclusion, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(cross, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(discours, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(attitude, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(conge, CultureInfo.InvariantCulture.NumberFormat);
            DateTime dateCreationtest = DateTime.Parse(dateTest);
            GrilleEvaluation eval = new GrilleEvaluation();
            Employee emp = serviceEmployee.getByPseudoName(username.ToLower());
            eval.acceuilPresentation = float.Parse(acceuil, CultureInfo.InvariantCulture.NumberFormat);
            eval.objetAppel = float.Parse(objetAppel, CultureInfo.InvariantCulture.NumberFormat);
            eval.presentationOffre = float.Parse(presentation, CultureInfo.InvariantCulture.NumberFormat);
            eval.gestionObjection = float.Parse(objections, CultureInfo.InvariantCulture.NumberFormat);
            eval.vCContrat = float.Parse(conclusion, CultureInfo.InvariantCulture.NumberFormat);
            eval.pCross = float.Parse(cross, CultureInfo.InvariantCulture.NumberFormat);
            eval.discours = float.Parse(discours, CultureInfo.InvariantCulture.NumberFormat);
            eval.attitude = float.Parse(attitude, CultureInfo.InvariantCulture.NumberFormat);
            eval.priseConge = float.Parse(conge, CultureInfo.InvariantCulture.NumberFormat);

            eval.dateTempEvaluation = dateCreationtest;
            eval.employeeId = emp.Id;
            eval.type = "Reab";
            eval.note = notes;
            eval.commentaireQualite = description;
            eval.enregistrementFullName = enregistrement;
            eval.enregistrementUrl = enregistrementUrl;
            eval.enregistrementDirectory = enregistrementDirectory;

            // service.Add(eval);
            // service.SaveChange();
            var a = new EvaluationViewModel();
            a.note = eval.note;
            a.pourcentageNotes = (notes / 165) * 100;
            a.acceuilPresentation = eval.acceuilPresentation;
            a.objetAppel = eval.objetAppel;
            a.presentationOffre = eval.presentationOffre;
            a.gestionObjection = eval.gestionObjection;
            a.vCContrat = eval.vCContrat;
            a.pCross = eval.pCross;
            a.discours = eval.discours;
            a.attitude = eval.attitude;
            a.priseConge = eval.priseConge;
            a.dateTempEvaluation = eval.dateTempEvaluation;
            a.agentName = username;
            if (Request.IsAjaxRequest())
            {
                //if (Directory.Exists(@utilisateurs) == false)
                //{
                //    return PartialView("DirectoryError", a);

                //}
                //else 
                return PartialView("ResultPartialView", a);
            }
            return RedirectToAction("Acceuil", "Directory");

        }
        public ActionResult Promo2(string enregistrementFullName)
        {
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

            //string type = form["typeGenerator"].ToString();
            var logins = serviceGroupeEmp.getListEmployeeByGroupe(user.Id);
            var employees = logins.Select(o => o).Distinct().ToList();
            var uspseudoNames = logins.Select(o => o.pseudoName).Distinct().ToList();
            List<SelectListItem> agents = new List<SelectListItem>();
            var a = new EvaluationViewModel();
            foreach (var emp in employees)
            {
                if (!emp.UserName.Equals(user.UserName))
                {
                    agents.Add(new SelectListItem { Text = emp.UserName, Value = emp.UserName });
                }
            }
            a.agents = agents;
            a.enregistrementFullName = enregistrementFullName;
            return View(a);
        }

        public ActionResult PostPromo2(string username, string acceuil, string objetAppel, string decouverte, string presentation, string objections, string conclusion, string cross, string discours, string attitude, string conge, string dateTest, string description, string enregistrement, string enregistrementUrl, string enregistrementDirectory)
        {
            var userConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            float notes = float.Parse(acceuil, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(objetAppel, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(decouverte, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(presentation, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(objections, CultureInfo.InvariantCulture.NumberFormat) +
                  float.Parse(conclusion, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(cross, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(discours, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(attitude, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(conge, CultureInfo.InvariantCulture.NumberFormat);
            DateTime dateCreationtest = DateTime.Parse(dateTest);
            GrilleEvaluation eval = new GrilleEvaluation();
            Employee emp = serviceEmployee.getByPseudoName(username.ToLower());
            eval.acceuilPresentation = float.Parse(acceuil, CultureInfo.InvariantCulture.NumberFormat);
            eval.objetAppel = float.Parse(objetAppel, CultureInfo.InvariantCulture.NumberFormat);
            eval.ppOffre = float.Parse(presentation, CultureInfo.InvariantCulture.NumberFormat);
            eval.gestionObjection = float.Parse(objections, CultureInfo.InvariantCulture.NumberFormat);
            eval.vCContrat = float.Parse(conclusion, CultureInfo.InvariantCulture.NumberFormat);
            eval.pCross = float.Parse(cross, CultureInfo.InvariantCulture.NumberFormat);
            eval.discours = float.Parse(discours, CultureInfo.InvariantCulture.NumberFormat);
            eval.attitude = float.Parse(attitude, CultureInfo.InvariantCulture.NumberFormat);
            eval.priseConge = float.Parse(conge, CultureInfo.InvariantCulture.NumberFormat);
            eval.decouverteBesoins = float.Parse(decouverte, CultureInfo.InvariantCulture.NumberFormat);
            eval.dateTempEvaluation = dateCreationtest;
            eval.employeeId = emp.Id;
            eval.senderId = userConnected.Id;
            eval.note = notes;
            eval.type = "Promo";
            eval.commentaireQualite = description;
            eval.enregistrementFullName = enregistrement;
            eval.enregistrementUrl = enregistrementUrl;
            eval.enregistrementDirectory = enregistrementDirectory;
            service.Add(eval);
            service.SaveChange();
            var a = new EvaluationViewModel();
            a.note = eval.note;
            a.pourcentageNotes = (notes / 190) * 100;
            a.acceuilPresentation = eval.acceuilPresentation;
            a.decouverteBesoins = eval.decouverteBesoins;
            a.objetAppel = eval.objetAppel;
            a.presentationOffre = eval.presentationOffre;
            a.ppOffre = eval.ppOffre;
            a.gestionObjection = eval.gestionObjection;
            a.vCContrat = eval.vCContrat;
            a.pCross = eval.pCross;
            a.discours = eval.discours;
            a.attitude = eval.attitude;
            a.priseConge = eval.priseConge;
            a.dateTempEvaluation = eval.dateTempEvaluation;
            a.agentName = username;
            a.senderName = userConnected.UserName;
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

            //    oMail.HtmlBody = "<html><body><p>Bonjour,<p><p>Vous avez une évaluation de la part du qualité </p><p>Cordialement.</p></body></html>";

            //    // oMail.HtmlBody = "<html><head><head><style>#customers {font-family:'Trebuchet MS',Arial,Helvetica,sans-serif;border-collapse: collapse;width: 100 %;}#customers td, #customers th {border: 1px solid #ddd;padding: 8px;}#customers tr:nth-child(even){background-color: #f2f2f2;}#customers tr:hover {background-color: #ddd;}#customers th {padding-top: 12px;padding-bottom: 12px;text-align: left;background-color: #4CAF50;color: white;}</style></head></head><body><h5 style='color:red;'>Nom Complet de l'enregistrement</h5><a href='#'><h7 style='color:blue;'>" + enregistrement + "</h7></a><br /><br/><table id='customers'><tr><th>Date Evaluation</th><th>Acceuil / Présentation</th><th>Découverte des besoins</th><th>Objet de l'appel</th> <th>Présentation / Proposition de l'offre</th><th>Gestion objections</th><th>Verrouillage et conclusion du contact</th><th>Proposition Cross</th><th>Discours</th><th>Attitude</th><th>Prise de congé</th><th>Score</th></tr><tr><td>" + dateTest + "</td><td>" + acceuil + "</td><td>" + a.decouverteBesoins + "</td><td>" + objetAppel + "</td><td>" + a.ppOffre + "</td><td>" + a.gestionObjection + "</td><td>" + a.vCContrat + "</td><td>" + a.pCross + "</td><td>" + a.discours + "</td><td>" + a.attitude + "</td><td>" + a.priseConge + "</td><td style='color:red;'>" + a.pourcentageNotes + "%" + "</td></tr></table></body></html>";
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
                //if (Directory.Exists(@utilisateurs) == false)
                //{
                //    return PartialView("DirectoryError", a);

                //}
                //else 

                return PartialView("EnvoiMailPartialViewPromo", a);
            }
            return RedirectToAction("Acceuil", "Directory");
        }

        public ActionResult PostPromoCalcul2(string username, string acceuil, string objetAppel, string decouverte, string presentation, string objections, string conclusion, string cross, string discours, string attitude, string conge, string dateTest, string description, string enregistrement, string enregistrementUrl, string enregistrementDirectory)
        {
            float notes = float.Parse(acceuil, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(objetAppel, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(decouverte, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(presentation, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(objections, CultureInfo.InvariantCulture.NumberFormat) +
                  float.Parse(conclusion, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(cross, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(discours, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(attitude, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(conge, CultureInfo.InvariantCulture.NumberFormat);
            DateTime dateCreationtest = DateTime.Parse(dateTest);
            GrilleEvaluation eval = new GrilleEvaluation();
            Employee emp = serviceEmployee.getByPseudoName(username.ToLower());
            eval.acceuilPresentation = float.Parse(acceuil, CultureInfo.InvariantCulture.NumberFormat);
            eval.objetAppel = float.Parse(objetAppel, CultureInfo.InvariantCulture.NumberFormat);
            eval.ppOffre = float.Parse(presentation, CultureInfo.InvariantCulture.NumberFormat);
            eval.gestionObjection = float.Parse(objections, CultureInfo.InvariantCulture.NumberFormat);
            eval.vCContrat = float.Parse(conclusion, CultureInfo.InvariantCulture.NumberFormat);
            eval.pCross = float.Parse(cross, CultureInfo.InvariantCulture.NumberFormat);
            eval.discours = float.Parse(discours, CultureInfo.InvariantCulture.NumberFormat);
            eval.attitude = float.Parse(attitude, CultureInfo.InvariantCulture.NumberFormat);
            eval.priseConge = float.Parse(conge, CultureInfo.InvariantCulture.NumberFormat);
            eval.decouverteBesoins = float.Parse(decouverte, CultureInfo.InvariantCulture.NumberFormat);
            eval.dateTempEvaluation = dateCreationtest;
            eval.employeeId = emp.Id;
            eval.note = notes;
            eval.type = "Promo";
            eval.commentaireQualite = description;
            eval.enregistrementFullName = enregistrement;
            eval.enregistrementUrl = enregistrementUrl;
            eval.enregistrementDirectory = enregistrementDirectory;
            // service.Add(eval);
            // service.SaveChange();
            var a = new EvaluationViewModel();
            a.note = eval.note;
            a.pourcentageNotes = (notes / 190) * 100;
            a.acceuilPresentation = eval.acceuilPresentation;
            a.decouverteBesoins = eval.decouverteBesoins;
            a.objetAppel = eval.objetAppel;
            a.presentationOffre = eval.presentationOffre;
            a.ppOffre = eval.ppOffre;
            a.gestionObjection = eval.gestionObjection;
            a.vCContrat = eval.vCContrat;
            a.pCross = eval.pCross;
            a.discours = eval.discours;
            a.attitude = eval.attitude;
            a.priseConge = eval.priseConge;
            a.dateTempEvaluation = eval.dateTempEvaluation;
            a.agentName = username;
            if (Request.IsAjaxRequest())
            {
                //if (Directory.Exists(@utilisateurs) == false)
                //{
                //    return PartialView("DirectoryError", a);

                //}
                //else 

                return PartialView("ResultPartialViewPromo", a);
            }
            return RedirectToAction("Acceuil", "Directory");
        }

        #endregion


        #region MailTest
        [HttpPost]
        public ActionResult sendMail()
        {
            //    try
            //    {
            //        MailMessage mail = new MailMessage();
            //        SmtpClient SmtpServer = new SmtpClient("smtp.info.local");

            //        mail.From = new MailAddress("Imed.LAKHEL@infopro-digital.com");
            //        mail.To.Add("Imed.LAKHEL@infopro-digital.com");
            //        mail.Subject = "Test Mail";
            //        mail.Body = "This is for testing SMTP mail from IMED";

            //        SmtpServer.Port = 587;
            //        SmtpServer.Credentials = new System.Net.NetworkCredential("Imed.LAKHEL@infopro-digital.com", "naruto1bleach23");
            //        SmtpServer.EnableSsl = true;
            //       // ServicePointManager.ServerCertificateValidationCallback =
            //       // delegate (object s, X509Certificate certificate,
            //       // X509Chain chain, SslPolicyErrors sslPolicyErrors)
            //       //{ return true; };
            //        SmtpServer.Send(mail);
            //        ViewBag.msg = "mail sent";
            //    }
            //    catch (SmtpException a)
            //    {
            //        Console.WriteLine(a);
            //        ViewBag.msg = a.InnerException;
            //        return RedirectToAction("Reab");
            //    }
            //    ViewBag.msg = "aaaaaaaaaaaaa";
            //    return RedirectToAction("Promo");
            //}
            try
            {
                SmtpMail oMail = new SmtpMail("TryIt");
                EASendMail.SmtpClient oSmtp = new EASendMail.SmtpClient();

                // Set sender email address, please change it to yours
                oMail.From = "Imed.LAKHEL@infopro-digital.com";

                // Set recipient email address, please change it to yours
                oMail.To = "Sana.BENSALAH@infopro-digital.com";
                // Set email subject
                oMail.Subject = "test email from imed c#, tls 587 port";

                // Set email body
                oMail.TextBody = "TestMail";

                // Your SMTP server address
                SmtpServer oServer = new SmtpServer("smtp.info.local");

                // User and password for ESMTP authentication, if your server doesn't require
                // User authentication, please remove the following codes.
                oServer.User = "Imed.LAKHEL@infopro-digital.com";
                oServer.Password = "Welcome01";

                // Set 25 or 587 port.
                oServer.Port = 587;

                // detect TLS connection automatically
                oServer.ConnectType = SmtpConnectType.ConnectSSLAuto;
                oSmtp.SendMail(oServer, oMail);

                ViewBag.msg = "mail sent";
            }
            catch (SmtpException)
            {
                ViewBag.msg = "mail not sent";
                return RedirectToAction("Promo");
            }

            return RedirectToAction("Evaluation", "Reab");
        }
        #endregion
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

        #region ArchiveAgentQualité
        [Authorize(Roles = "Agent Qualité,Agent Qualité_Diffusion")]
        public ActionResult ArchiveEvaluationsAgentQualite()
        {
            EvaluationViewModel evaluation = new EvaluationViewModel();

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
        public ActionResult GetArchiveEvaluationsAgentQualite()
        {
            List<EvaluationViewModel> a = new List<EvaluationViewModel>();
            try
            {
                Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

                var historstions = service.GetEvaluationsBySenderId(emp.Id);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationViewModel test = new EvaluationViewModel();
                    test.acceuilPresentation = item.acceuilPresentation;
                    test.objetAppel = item.objetAppel;
                    test.presentationOffre = item.presentationOffre;
                    test.gestionObjection = item.gestionObjection;
                    test.vCContrat = item.vCContrat;
                    test.pCross = item.pCross;
                    test.discours = item.discours;
                    test.attitude = item.attitude;
                    test.priseConge = item.priseConge;
                    test.decouverteBesoins = item.decouverteBesoins;
                    test.ppOffre = item.ppOffre;
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
                    return PartialView("ArchiveTableAgentQualitePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("ArchiveEvaluationsAgentQualite");
        }

        public ActionResult GetArchiveEvaluationsAgentQualiteByDate(string dateDebut, string dateFin)
        {
            List<EvaluationViewModel> a = new List<EvaluationViewModel>();
            try
            {
                Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
                DateTime daterecherchedebut = DateTime.Parse(dateDebut);
                DateTime daterecherchefin = DateTime.Parse(dateFin);
                var historstions = service.GetEvaluationsSenderBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationViewModel test = new EvaluationViewModel();
                    test.acceuilPresentation = item.acceuilPresentation;
                    test.objetAppel = item.objetAppel;
                    test.presentationOffre = item.presentationOffre;
                    test.gestionObjection = item.gestionObjection;
                    test.vCContrat = item.vCContrat;
                    test.pCross = item.pCross;
                    test.discours = item.discours;
                    test.attitude = item.attitude;
                    test.priseConge = item.priseConge;
                    test.decouverteBesoins = item.decouverteBesoins;
                    test.ppOffre = item.ppOffre;
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
                    return PartialView("ArchiveTableAgentQualitePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("ArchiveEvaluationsAgentQualite");
        }
        #endregion

        #region Reportings Diffusion Reab Promo
        [Authorize(Roles = "Qualité,Agent Qualité_Diffusion")]
        //Historique Diffusion
        public ActionResult ReportsReabPromoDiffusion()
        {
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
          
            List<Employee> logins = new List<Employee>();
            var evaluations = service.GetAll();
            List<string> DiffGroupes = new List<string>(new string[] { "GISI-REAB", "GISI-PROMO", "GMT-REAB", "GMT-PROMO" });
            foreach (var d in DiffGroupes)
            {
                Groupe gr = serviceGroupe.getByNom(d);
                List<Employee> emp = serviceGroupeEmp.getListEmployeeByGroupeId(gr.Id);
                foreach (var e in emp)
                {
                    if (!logins.Exists(l => l.UserName == e.UserName))
                    {
                        logins.Add(e);
                    }
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
            evaluation.evaluations = (List<GrilleEvaluation>)evaluations;
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

        public JsonResult GetReportsReabDiffusion(string type, string username, string dateDebut, string dateFin)
        {
            float totAcceuilPresentation = 0, totObjetAppel = 0, totPresentationOffre = 0, totGestionObjection = 0, totVCContrat = 0,
                totPCross = 0, totDiscours = 0, totAttitude = 0,totPriseCongé = 0, totNotes = 0;

            float NbrePresentationOffre = 0, NbreGestionObjection = 0, NbrePCross = 0;

            DateTime daterecherchedebut = DateTime.Parse(dateDebut);
            DateTime daterecherchefin = DateTime.Parse(dateFin);

            var historstions = service.GetEvaluationsReabBetweenTwoDates(type, daterecherchedebut, daterecherchefin);
            if (username != "")
            {
                Employee emp = serviceEmployee.getByLogin(username);
                historstions = service.GetEvaluationsReabEmployeeBetweenTwoDates(type, emp.Id, daterecherchedebut, daterecherchefin);
            }
            float nbreEvaluations = historstions.Count();
            ViewBag.nbreEvaluations = nbreEvaluations;
            foreach (var item in historstions)
            {
                totNotes += item.note;
                totAcceuilPresentation += item.acceuilPresentation;
                totObjetAppel += item.objetAppel;
                
                if (item.presentationOffre >= 0)
                {
                    totPresentationOffre += item.presentationOffre;
                    NbrePresentationOffre += 1;
                }
                if (item.gestionObjection >= 0)
                {
                    totGestionObjection += item.gestionObjection;
                    NbreGestionObjection += 1;
                }
                if (item.pCross >= 0)
                {
                    totPCross += item.pCross;
                    NbrePCross += 1;
                }
               
                totVCContrat += item.vCContrat;
                totDiscours += item.discours;
                totAttitude += item.attitude;
                totPriseCongé += item.priseConge;         
            }

            EvaluationViewModel test = new EvaluationViewModel();

            test.nbreEvaluations = Convert.ToInt32(nbreEvaluations);
            if (NbrePresentationOffre != 0)
            {
                test.presentationOffre = (float)Math.Round((totPresentationOffre / (NbrePresentationOffre * 20)) * 100, 2);
            }
            else { test.presentationOffre = -20; }
            if (NbreGestionObjection != 0)
            {
                test.gestionObjection = (float)Math.Round((totGestionObjection / (NbreGestionObjection * 30)) * 100, 2);
            }
            else { test.gestionObjection = -30; }
            if (NbrePCross != 0)
            {
                test.pCross = (float)Math.Round((totPCross / (NbrePCross * 25)) * 100, 2);
            }
            else { test.pCross = -25; }
           

            if (nbreEvaluations != 0)
            {
                test.note = (float)Math.Round((totNotes / (165 * nbreEvaluations)) * 100, 2);
                test.acceuilPresentation = (float)Math.Round((totAcceuilPresentation / (nbreEvaluations * 10)) * 100, 2);
                test.objetAppel = (float)Math.Round((totObjetAppel / (nbreEvaluations * 15)) * 100, 2);
                test.vCContrat = (float)Math.Round((totVCContrat / (nbreEvaluations * 30)) * 100, 2);     
                test.discours = (float)Math.Round((totDiscours / (nbreEvaluations *15)) * 100, 2);
                test.attitude = (float)Math.Round((totAttitude / (nbreEvaluations * 10)) * 100, 2);
                test.priseConge = (float)Math.Round((totPriseCongé / (nbreEvaluations * 10)) * 100, 2);
            }
            else
            {
                test.note = 0;
                test.acceuilPresentation = 0;
                test.objetAppel = 0;
                test.presentationOffre = 0;
                test.gestionObjection = 0;
                test.pCross = 0;
                test.vCContrat = 0;
                test.discours = 0;
                test.attitude = 0;
                test.priseConge = 0;
            }
            return Json(test, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetReportsPromoDiffusion(string type, string username, string dateDebut, string dateFin)
        {
            float totAcceuilPresentation = 0, totObjetAppel = 0, totPresentationOffre = 0, totGestionObjection = 0, totVCContrat = 0,
                totPCross = 0, totDiscours = 0, totAttitude = 0, totPriseCongé = 0, totDecouverteBesoins = 0, totNotes = 0;

            float NbrePresentationOffre = 0, NbreGestionObjection = 0, NbrePCross = 0, NbreDecouverteBesoins = 0;

            DateTime daterecherchedebut = DateTime.Parse(dateDebut);
            DateTime daterecherchefin = DateTime.Parse(dateFin);

            var historstions = service.GetEvaluationsReabBetweenTwoDates(type, daterecherchedebut, daterecherchefin);
            if (username != "")
            {
                Employee emp = serviceEmployee.getByLogin(username);
                historstions = service.GetEvaluationsReabEmployeeBetweenTwoDates(type, emp.Id, daterecherchedebut, daterecherchefin);
            }
            float nbreEvaluations = historstions.Count();
            ViewBag.nbreEvaluations = nbreEvaluations;
            foreach (var item in historstions)
            {
                totNotes += item.note;
                totAcceuilPresentation += item.acceuilPresentation;
                totObjetAppel += item.objetAppel;

                if (item.presentationOffre >= 0)
                {
                    totPresentationOffre += item.presentationOffre;
                    NbrePresentationOffre += 1;
                }
                if (item.gestionObjection >= 0)
                {
                    totGestionObjection += item.gestionObjection;
                    NbreGestionObjection += 1;
                }
                if (item.pCross >= 0)
                {
                    totPCross += item.pCross;
                    NbrePCross += 1;
                }
                if (item.decouverteBesoins >= 0)
                {
                    totDecouverteBesoins += item.decouverteBesoins;
                    NbreDecouverteBesoins += 1;
                }

                totVCContrat += item.vCContrat;
                totDiscours += item.discours;
                totAttitude += item.attitude;
                totPriseCongé += item.priseConge;
            }

            EvaluationViewModel test = new EvaluationViewModel();

            test.nbreEvaluations = Convert.ToInt32(nbreEvaluations);
            if (NbrePresentationOffre != 0)
            {
                test.presentationOffre = (float)Math.Round((totPresentationOffre / (NbrePresentationOffre * 25)) * 100, 2);
            }
            else { test.presentationOffre = -25; }
            if (NbreGestionObjection != 0)
            {
                test.gestionObjection = (float)Math.Round((totGestionObjection / (NbreGestionObjection * 30)) * 100, 2);
            }
            else { test.gestionObjection = -30; }
            if (NbrePCross != 0)
            {
                test.pCross = (float)Math.Round((totPCross / (NbrePCross * 25)) * 100, 2);
            }
            else { test.pCross = -25; }
            if (NbreDecouverteBesoins != 0)
            {
                test.decouverteBesoins = (float)Math.Round((totDecouverteBesoins / (NbreDecouverteBesoins * 20)) * 100, 2);
            }
            else { test.decouverteBesoins = -20; }

            if (nbreEvaluations != 0)
            {
                test.note = (float)Math.Round((totNotes / (190 * nbreEvaluations)) * 100, 2);
                test.acceuilPresentation = (float)Math.Round((totAcceuilPresentation / (nbreEvaluations * 10)) * 100, 2);
                test.objetAppel = (float)Math.Round((totObjetAppel / (nbreEvaluations * 15)) * 100, 2);
                test.vCContrat = (float)Math.Round((totVCContrat / (nbreEvaluations * 30)) * 100, 2);
                test.discours = (float)Math.Round((totDiscours / (nbreEvaluations * 15)) * 100, 2);
                test.attitude = (float)Math.Round((totAttitude / (nbreEvaluations * 10)) * 100, 2);
                test.priseConge = (float)Math.Round((totPriseCongé / (nbreEvaluations * 10)) * 100, 2);
            }
            else
            {
                test.note = 0;
                test.acceuilPresentation = 0;
                test.objetAppel = 0;
                test.presentationOffre = 0;
                test.gestionObjection = 0;
                test.pCross = 0;
                test.vCContrat = 0;
                test.discours = 0;
                test.attitude = 0;
                test.priseConge = 0;
                test.decouverteBesoins = 0;
            }
            return Json(test, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}