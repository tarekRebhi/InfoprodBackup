using System;
using System.Net;
using System.Net.Mail;
using Domain.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using MVCWEB.Models;
using Services;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
    [Authorize(Roles = "Qualité, Agent Qualité_AchatPublic")]
    public class EvaluationAchatPublicController : Controller
    {
        private ReportContext db = new ReportContext();
        #region globalVariable
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        IEmployeeService serviceEmployee;
        IEvaluationAchatPublicService serviceAchatPublic;
        IGroupeEmployeeService serviceGroupeEmp;
        IGroupeService serviceGroupe;
        #endregion

        #region constructor
        public EvaluationAchatPublicController()
        {
            serviceEmployee = new EmployeeService();
            serviceAchatPublic = new EvaluationAchatPublicService();
            serviceGroupeEmp = new GroupesEmployeService();
            serviceGroupe = new GroupeService();
        }
        public EvaluationAchatPublicController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
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

        public ActionResult AchatPublic(string enregistrementFullName, string enregistrementUrl, string enregistrementDirectory, string agentName)
        {

            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 2009))
            {
                ViewBag.role = "Agent Qualité_AchatPublic";
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

        public ActionResult AchatPublicWithoutAgent(string enregistrementFullName, string enregistrementUrl, string enregistrementDirectory, string siteName)
        {
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            var a = new EvaluationViewModel();
            List<SelectListItem> agents = new List<SelectListItem>();
            List<Employee> logins = new List<Employee>();
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 2009))
            {
                ViewBag.role = "Agent Qualité_AchatPublic";
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
                string d = logins[j].pseudoName.Replace(" ", "-").Replace("é", "Ã©").ToLower();
                string dd = logins[j].pseudoName.Replace(" ", "--").Replace("é", "Ã©").ToLower();
                string ddd = logins[j].pseudoName.Replace(" ", ".").Replace("é", "Ã©").ToLower();
                string tv = "TV." + logins[j].IdHermes;
                // if (enregistrementFullName.ToLower().Contains(d) || enregistrementFullName.ToLower().Contains(dd) || enregistrementFullName.ToLower().Contains(ddd))
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
        public ActionResult CalculAchatPublic(string nomAgent, string planDate, string accueil, string identificatioClient, string decouverteDemande, string maitrisePlateforme, string miseAttente, string tempsAttente, string qualiteService, string discours, string conclusionContact, string attitude, string qualification, string priseConge, string commentaire, string enregistrement, string enregistrementUrl, string enregistrementDirectory)
        {
            float total = 42;
            List<string> NEList = new List<string>(new string[] { miseAttente, tempsAttente });
            float notes = float.Parse(accueil, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(identificatioClient, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(decouverteDemande, CultureInfo.InvariantCulture.NumberFormat) +
              float.Parse(maitrisePlateforme, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(qualiteService, CultureInfo.InvariantCulture.NumberFormat)+
              float.Parse(discours, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(conclusionContact, CultureInfo.InvariantCulture.NumberFormat)+
              float.Parse(attitude, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(qualification, CultureInfo.InvariantCulture.NumberFormat)+
              float.Parse(priseConge, CultureInfo.InvariantCulture.NumberFormat);

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
            var a = new EvaluationAchatPublicViewModel();
            a.accueil = float.Parse(accueil, CultureInfo.InvariantCulture.NumberFormat);
            a.identificatioClient = float.Parse(identificatioClient, CultureInfo.InvariantCulture.NumberFormat);
            a.decouverteDemande = float.Parse(decouverteDemande, CultureInfo.InvariantCulture.NumberFormat);
            a.maitrisePlateforme = float.Parse(maitrisePlateforme, CultureInfo.InvariantCulture.NumberFormat);
            a.miseAttente = float.Parse(miseAttente, CultureInfo.InvariantCulture.NumberFormat);
            a.tempsAttente = float.Parse(tempsAttente, CultureInfo.InvariantCulture.NumberFormat);
            a.qualiteService = float.Parse(qualiteService, CultureInfo.InvariantCulture.NumberFormat);
            a.discours = float.Parse(discours, CultureInfo.InvariantCulture.NumberFormat);
            a.conclusionContact = float.Parse(conclusionContact, CultureInfo.InvariantCulture.NumberFormat);
            a.attitude = float.Parse(attitude, CultureInfo.InvariantCulture.NumberFormat);
            a.qualification = float.Parse(qualification, CultureInfo.InvariantCulture.NumberFormat);
           a.priseConge = float.Parse(priseConge, CultureInfo.InvariantCulture.NumberFormat);

            a.dateTempEvaluation = DateTime.Parse(planDate);
            a.agentName = nomAgent;
            a.note = notes;
            a.pourcentageNotes = (notes / total) * 100;
            a.type = "Achat Public";

            if (Request.IsAjaxRequest())
            {
                return PartialView("AchatPublicResultPartialView", a);
            }
            return RedirectToAction("Acceuil", "Directory");
        }

        public ActionResult SaveEvalAchatPublic(string nomAgent, string planDate, string accueil, string identificatioClient, string decouverteDemande, string maitrisePlateforme, string miseAttente, string tempsAttente, string qualiteService, string discours, string conclusionContact, string attitude, string qualification, string priseConge, string commentaire, string enregistrement, string enregistrementUrl, string enregistrementDirectory)
        {
            var userConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            Employee emp = serviceEmployee.getByPseudoName(nomAgent.ToLower());
            GrilleEvaluationAchatPublic a = new GrilleEvaluationAchatPublic();
            float total = 42;
            List<string> NEList = new List<string>(new string[] { miseAttente, tempsAttente });
            float notes = float.Parse(accueil, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(identificatioClient, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(decouverteDemande, CultureInfo.InvariantCulture.NumberFormat) +
              float.Parse(maitrisePlateforme, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(qualiteService, CultureInfo.InvariantCulture.NumberFormat) +
              float.Parse(discours, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(conclusionContact, CultureInfo.InvariantCulture.NumberFormat) +
              float.Parse(attitude, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(qualification, CultureInfo.InvariantCulture.NumberFormat) +
              float.Parse(priseConge, CultureInfo.InvariantCulture.NumberFormat);

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
            a.identificatioClient = float.Parse(identificatioClient, CultureInfo.InvariantCulture.NumberFormat);
            a.decouverteDemande = float.Parse(decouverteDemande, CultureInfo.InvariantCulture.NumberFormat);
            a.maitrisePlateforme = float.Parse(maitrisePlateforme, CultureInfo.InvariantCulture.NumberFormat);
            a.miseAttente = float.Parse(miseAttente, CultureInfo.InvariantCulture.NumberFormat);
            a.tempsAttente = float.Parse(tempsAttente, CultureInfo.InvariantCulture.NumberFormat);
            a.qualiteService = float.Parse(qualiteService, CultureInfo.InvariantCulture.NumberFormat);
            a.discours = float.Parse(discours, CultureInfo.InvariantCulture.NumberFormat);
            a.conclusionContact = float.Parse(conclusionContact, CultureInfo.InvariantCulture.NumberFormat);
            a.attitude = float.Parse(attitude, CultureInfo.InvariantCulture.NumberFormat);
            a.qualification = float.Parse(qualification, CultureInfo.InvariantCulture.NumberFormat);
           a.priseConge = float.Parse(priseConge, CultureInfo.InvariantCulture.NumberFormat);
            a.dateTempEvaluation = DateTime.Parse(planDate);
            a.employeeId = emp.Id;
            a.senderId = userConnected.Id;
            a.note = (notes / total) * 100;
            a.type = "Achat Public";
            a.commentaireQualite = commentaire;
            a.enregistrementFullName = enregistrement;
            a.enregistrementUrl = enregistrementUrl;
            a.enregistrementDirectory = enregistrementDirectory;

            serviceAchatPublic.Add(a);
            serviceAchatPublic.SaveChange();
            var eval = new EvaluationAchatPublicViewModel();
            eval.agentName = nomAgent;
            //Send Email
            //string SenderMail = "alerte.infoprod@infopro-digital.com";
            //string receiverMail = emp.Email;
            //MailAddress to = new MailAddress(receiverMail);
            //MailAddress from = new MailAddress(SenderMail);

            //MailMessage message = new MailMessage(from, to);
            //message.Subject = "Notification Nouvelle Evaluation";
            //message.IsBodyHtml = true;
            //message.Body = "<html><head></head><body><p>Bonjour,</p><p>Nous vous informons qu'un audit qualité viens d'être enregistré, vous pouvez le consulter sur l’interface INFO-PROD QUALITE </p><p>En attendant le débriefe de l’évaluateur</p><p>Cordialement.</p></body></html>";

            //SmtpClient client = new SmtpClient("smtp.info.local", 587)
            //{
            //    UseDefaultCredentials = true,
            //    // Credentials = new NetworkCredential("alerte.infoprod@infopro-digital.com", "Welcome01"),
            //    DeliveryMethod = SmtpDeliveryMethod.Network,
            //    EnableSsl = true
            //};
            //// code in brackets above needed if authentication required 
            //try
            //{
            //    client.Send(message);
            //}
            //catch (SmtpException ex)
            //{
            //    Console.WriteLine(ex.ToString());
            //}
            if (Request.IsAjaxRequest())
            {
                return PartialView("EnvoiMailPartialView", eval);
            }
            return RedirectToAction("Acceuil", "Directory");

        }

        #region Historique Achat Public (Par Collaborateur)
        public ActionResult HistoriqueAchatPublic()
        {
            List<Employee> logins = new List<Employee>();
        EvaluationViewModel evaluation = new EvaluationViewModel();
        var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 2009))
            {
                ViewBag.role = "Agent Qualité_AchatPublic";
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
                if (!test.UserName.Equals(user.UserName) && test.Roles.Any(r => r.UserId == test.Id && r.RoleId == 2010))
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
        public ActionResult GetHistoAchatPublic(string username)
{
    List<EvaluationAchatPublicViewModel> a = new List<EvaluationAchatPublicViewModel>();
    try
    {
        Employee emp = serviceEmployee.getByLogin(username);
        var historstions = serviceAchatPublic.GetEvaluationsByEmployee(emp.Id);
        ViewBag.nbreEvaluations = historstions.Count();
        foreach (var item in historstions)
        {
                    EvaluationAchatPublicViewModel test = new EvaluationAchatPublicViewModel();
            test.Id = item.Id;
            test.accueil = item.accueil;
            test.identificatioClient = item.identificatioClient;
            test.decouverteDemande = item.decouverteDemande;
            test.maitrisePlateforme = item.maitrisePlateforme;
            test.miseAttente = item.miseAttente;
            test.tempsAttente = item.tempsAttente;
            test.qualiteService = item.qualiteService;
            test.discours = item.discours;
            test.conclusionContact = item.conclusionContact;
            test.attitude = item.attitude;
            test.qualification = item.qualification;
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
            return PartialView("HistoriqueAchatPublicTablePartialView", a);
        }
    }
    catch (NullReferenceException e)
    {
        Console.WriteLine(e.Data);
        return PartialView("SelectError");
    }

    return RedirectToAction("HistoriqueAchatPublic");
}
public ActionResult GetHistoAchatPublicByDate(string username, string dateDebut, string dateFin)
{
    List<EvaluationAchatPublicViewModel> a = new List<EvaluationAchatPublicViewModel>();
    try
    {
        Employee emp = serviceEmployee.getByLogin(username);
        DateTime daterecherchedebut = DateTime.Parse(dateDebut);
        DateTime daterecherchefin = DateTime.Parse(dateFin);


        var historstions = serviceAchatPublic.GetEvaluationsEmployeeBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
        ViewBag.nbreEvaluations = historstions.Count();
        foreach (var item in historstions)
        {
                    EvaluationAchatPublicViewModel test = new EvaluationAchatPublicViewModel();
            test.Id = item.Id;
                    test.accueil = item.accueil;
                    test.identificatioClient = item.identificatioClient;
                    test.decouverteDemande = item.decouverteDemande;
                    test.maitrisePlateforme = item.maitrisePlateforme;
                    test.miseAttente = item.miseAttente;
                    test.tempsAttente = item.tempsAttente;
                    test.qualiteService = item.qualiteService;
                    test.discours = item.discours;
                    test.conclusionContact = item.conclusionContact;
                    test.attitude = item.attitude;
                    test.qualification = item.qualification;
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
            return PartialView("HistoriqueAchatPublicTablePartialView", a);
        }
    }
    catch (NullReferenceException e)
    {
        Console.WriteLine(e.Data);
        return PartialView("SelectError");
    }

    return RedirectToAction("QRHistorique");
}
        #endregion




        #region Ecouter et Details
        [HttpGet]
        public ActionResult FindCustomEnregistrement(String name, String fullName, String DirectoryName)
        {
            var b = new DirectoryViewModel();
            Random rnd = new Random();
            int iss = rnd.Next(1, 10000);

            try
            {

                if (Request.IsAjaxRequest())
                {
                    string fullName2 = fullName.Replace(" ", "+");
                    string name2 = name.Replace("+", "");
                    b.pseudoName = name;
                    var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
                    b.userName = user.UserName;
                    b.enregistrementName = fullName;
                    b.directoryName = DirectoryName;
                    var fileInfo = new FileInfo(@name2);
                    System.IO.File.Copy(fullName, Server.MapPath("~/Files/") + name2, true);

                    return PartialView("CustomEnregistrement", b);
                }
            }
            catch (Exception)
            {

                return PartialView("CustomEnregistrement", b);
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
            return RedirectToAction("Index");
        }
        #endregion

        #region Archive Achat Public (Par Agent Qualité)
        public ActionResult ArchiveAchatPublic()
        {
            EvaluationAchatPublicViewModel evaluation = new EvaluationAchatPublicViewModel();
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            var logins = serviceEmployee.GetAll();
            var employees = logins.Select(o => o).Distinct().ToList();

            foreach (var test in employees)
            {
                if (test.Roles.Any(r => r.UserId == test.Id && r.RoleId == 4 || r.RoleId == 2009))
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

        public ActionResult GetAchatPublicArchiveEvaluationsQualite(string username)
        {
            List<EvaluationAchatPublicViewModel> a = new List<EvaluationAchatPublicViewModel>();
            try
            {
                Employee emp = serviceEmployee.getByLogin(username);
                var historstions = serviceAchatPublic.GetEvaluationsBySenderId(emp.Id);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationAchatPublicViewModel test = new EvaluationAchatPublicViewModel();
                    test.Id = item.Id;
                    test.accueil = item.accueil;
                    test.identificatioClient = item.identificatioClient;
                    test.decouverteDemande = item.decouverteDemande;
                    test.maitrisePlateforme = item.maitrisePlateforme;
                    test.miseAttente = item.miseAttente;
                    test.tempsAttente = item.tempsAttente;
                    test.qualiteService = item.qualiteService;
                    test.discours = item.discours;
                    test.conclusionContact = item.conclusionContact;
                    test.attitude = item.attitude;
                    test.qualification = item.qualification;
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
                    return PartialView("ArchiveAchatPublicTablePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("ArchiveAchatPublic");
        }

        public ActionResult GetArchiveAchatPublicEvaluationsByDate(string username, string dateDebut, string dateFin)
        {
            List<EvaluationAchatPublicViewModel> a = new List<EvaluationAchatPublicViewModel>();
            try
            {
                Employee emp = serviceEmployee.getByLogin(username);
                DateTime daterecherchedebut = DateTime.Parse(dateDebut);
                DateTime daterecherchefin = DateTime.Parse(dateFin);
                var historstions = serviceAchatPublic.GetEvaluationsSenderBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationAchatPublicViewModel test = new EvaluationAchatPublicViewModel();
                    test.Id = item.Id;
                    test.accueil = item.accueil;
                    test.identificatioClient = item.identificatioClient;
                    test.decouverteDemande = item.decouverteDemande;
                    test.maitrisePlateforme = item.maitrisePlateforme;
                    test.miseAttente = item.miseAttente;
                    test.tempsAttente = item.tempsAttente;
                    test.qualiteService = item.qualiteService;
                    test.discours = item.discours;
                    test.conclusionContact = item.conclusionContact;
                    test.attitude = item.attitude;
                    test.qualification = item.qualification;
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
                    return PartialView("ArchiveAchatPublicTablePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("ArchiveAchatPublic");
        }
        #endregion

        #region Archive Achat Public Agent Qualité
        [Authorize(Roles = "Agent Qualité_AchatPublic")]
        public ActionResult ArchiveAgentQualiteAchatPublic()
        {
            EvaluationVecteurPlusViewModel evaluation = new EvaluationVecteurPlusViewModel();

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
        public ActionResult GetArchiveAchatPublicAgentQualite()
        {
            List<EvaluationAchatPublicViewModel> a = new List<EvaluationAchatPublicViewModel>();
            try
            {
                Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

                var historstions = serviceAchatPublic.GetEvaluationsBySenderId(emp.Id);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationAchatPublicViewModel test = new EvaluationAchatPublicViewModel();
                    test.Id = item.Id;
                    test.accueil = item.accueil;
                    test.identificatioClient = item.identificatioClient;
                    test.decouverteDemande = item.decouverteDemande;
                    test.maitrisePlateforme = item.maitrisePlateforme;
                    test.miseAttente = item.miseAttente;
                    test.tempsAttente = item.tempsAttente;
                    test.qualiteService = item.qualiteService;
                    test.discours = item.discours;
                    test.conclusionContact = item.conclusionContact;
                    test.attitude = item.attitude;
                    test.qualification = item.qualification;
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
                    return PartialView("ArchiveAchatPublicTablePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("ArchiveAgentQualiteAchatPublic");
        }

        public ActionResult GetArchiveAchatPublicAgentQualiteByDate(string dateDebut, string dateFin)
        {
            List<EvaluationAchatPublicViewModel> a = new List<EvaluationAchatPublicViewModel>();
            try
            {
                Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
                DateTime daterecherchedebut = DateTime.Parse(dateDebut);
                DateTime daterecherchefin = DateTime.Parse(dateFin);
                var historstions = serviceAchatPublic.GetEvaluationsSenderBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationAchatPublicViewModel test = new EvaluationAchatPublicViewModel();
                    test.Id = item.Id;
                    test.accueil = item.accueil;
                    test.identificatioClient = item.identificatioClient;
                    test.decouverteDemande = item.decouverteDemande;
                    test.maitrisePlateforme = item.maitrisePlateforme;
                    test.miseAttente = item.miseAttente;
                    test.tempsAttente = item.tempsAttente;
                    test.qualiteService = item.qualiteService;
                    test.discours = item.discours;
                    test.conclusionContact = item.conclusionContact;
                    test.attitude = item.attitude;
                    test.qualification = item.qualification;
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
                    return PartialView("ArchiveAchatPublicTablePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("ArchiveAgentQualiteAchatPublic");
        }
        #endregion


        #region Edit et delete Achat Public
        [Authorize(Roles = "Qualité")]
        public ActionResult EditAchatPublic(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GrilleEvaluationAchatPublic evaluation = db.GrilleEvaluationAchatPublics.Find(id);
            EvaluationAchatPublicViewModel eval = new EvaluationAchatPublicViewModel();
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
        [HttpPost, ActionName("EditAchatPublic")]
        [Authorize(Roles = "Qualité")]
        public ActionResult EditAchatPublic(int? id, GrilleEvaluationAchatPublic evaluation)
        {
            float total = 42;
            List<float> NEList = new List<float>(new float[] { evaluation.miseAttente, evaluation.tempsAttente });
            float notes = evaluation.accueil + evaluation.identificatioClient + evaluation.decouverteDemande +evaluation.maitrisePlateforme+
                evaluation.qualiteService + evaluation.discours + evaluation.conclusionContact +
              evaluation.attitude + evaluation.qualification+ evaluation.priseConge;

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
                return RedirectToAction("HistoriqueAchatPublic");
            }
            return View(evaluation);
        }
        [Authorize(Roles = "Qualité")]
        [HttpGet]
        public ActionResult FindEvaluation(int? Id)
        {
            GrilleEvaluationAchatPublic item = serviceAchatPublic.getById(Id);

            if (Request.IsAjaxRequest())
            {
                return PartialView("_AlerteDeSuppressionAchatPublic", item);
            }

            else
            {
                return View(item);
            }
        }
        [Authorize(Roles = "Qualité")]
        public ActionResult DeleteAchatPublic(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GrilleEvaluationAchatPublic evaluation = serviceAchatPublic.getById(id);
            int? empId = evaluation.employeeId;
            serviceAchatPublic.DeleteEvaluations(id, empId);
            serviceAchatPublic.SaveChange();
            return RedirectToAction("HistoriqueAchatPublic");
        }
        #endregion

        #region Reportings Achat Public
        public ActionResult ReportsAchatPublic()
        {
            List<Employee> logins = new List<Employee>();
            EvaluationViewModel evaluation = new EvaluationViewModel();
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 2009))
            {
                ViewBag.role = "Agent Qualité_AchatPublic";
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
                if (!test.UserName.Equals(user.UserName) && test.Roles.Any(r => r.UserId == test.Id && r.RoleId == 2010))
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

        public JsonResult GetReportsAchatPublic(string username, string dateDebut, string dateFin)
        {
            float totAccueil = 0, totIdentificatioClient = 0, totDecouverteDemande = 0, totMaitrisePlateforme = 0, totMiseAttente = 0,
                totTempsAttente = 0, totQualiteService = 0, totDiscours = 0, totConclusionContact = 0, totAttitude = 0, totQualification = 0,
                totPriseCongé = 0, totNotes = 0;

            float NbreMiseAttente = 0, NbreTempsAttente = 0;

            DateTime daterecherchedebut = DateTime.Parse(dateDebut);
            DateTime daterecherchefin = DateTime.Parse(dateFin);

            var historstions = serviceAchatPublic.GetEvaluationsBetweenTwoDates(daterecherchedebut, daterecherchefin);
            if (username != "")
            {
                Employee emp = serviceEmployee.getByLogin(username);
                historstions = serviceAchatPublic.GetEvaluationsEmployeeBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
            }
            float nbreEvaluations = historstions.Count();
            ViewBag.nbreEvaluations = nbreEvaluations;
            foreach (var item in historstions)
            {
                totNotes += item.note;
                totAccueil += item.accueil;
                totIdentificatioClient += item.identificatioClient;
                totDecouverteDemande += item.decouverteDemande;
                totMaitrisePlateforme += item.maitrisePlateforme;

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
                totQualiteService += item.qualiteService;
                totDiscours += item.discours;
                totConclusionContact += item.conclusionContact;
                totAttitude += item.attitude;
                totQualification += item.qualification;
                totPriseCongé += item.priseConge;
            }

            EvaluationAchatPublicViewModel test = new EvaluationAchatPublicViewModel();

            test.nbreEvaluations = Convert.ToInt32(nbreEvaluations);
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
           

            if (nbreEvaluations != 0)
            {
                test.note = (float)Math.Round((totNotes / (nbreEvaluations * 100)) * 100, 2);
                test.accueil = (float)Math.Round((totAccueil / (nbreEvaluations * 2)) * 100, 2);
                test.identificatioClient = (float)Math.Round((totIdentificatioClient / (nbreEvaluations * 2)) * 100, 2);
                test.decouverteDemande = (float)Math.Round((totDecouverteDemande / (nbreEvaluations * 4)) * 100, 2);
                test.maitrisePlateforme = (float)Math.Round((totMaitrisePlateforme / (nbreEvaluations * 5)) * 100, 2);
                test.qualiteService = (float)Math.Round((totQualiteService / (nbreEvaluations * 8)) * 100, 2);
                test.discours = (float)Math.Round((totDiscours / (nbreEvaluations * 4)) * 100, 2);
                test.conclusionContact = (float)Math.Round((totConclusionContact / (nbreEvaluations * 3)) * 100, 2);
                test.attitude = (float)Math.Round((totAttitude / (nbreEvaluations * 6)) * 100, 2);
                test.qualification = (float)Math.Round((totQualification / (nbreEvaluations * 2)) * 100, 2);
                test.priseConge = (float)Math.Round((totPriseCongé / (nbreEvaluations * 2)) * 100, 2);
            }
            else
            {
                test.note = 0;
                test.accueil = 0;
                test.identificatioClient = 0;
                test.decouverteDemande = 0;
                test.maitrisePlateforme = 0;
                test.qualiteService = 0;
                test.discours = 0;
                test.conclusionContact = 0;
                test.attitude = 0;
                test.qualification = 0;
                test.priseConge = 0;
            }
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
