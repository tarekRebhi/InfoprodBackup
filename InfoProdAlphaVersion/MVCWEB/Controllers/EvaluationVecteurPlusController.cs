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
        [Authorize(Roles = "Qualité,Agent Qualité_PRV,Agent Qualité,Agent,Agent_QR,Agent_KLMO,Agent_PRV")]

        public class EvaluationVecteurPlusController : Controller
        {
            private ReportContext db = new ReportContext();
            // GET: Evaluation
            #region globalVariable
            IEmployeeService serviceEmployee;
            IGrilleEvaluationService serviceDiff;
            IEvaluationQRService service;
            IEvaluationKLMOService KLMOservice;
            IEvaluationBPPService BPPservice;
            IEvaluationBppTypologieService BPPTypologieService;
            IEvaluationBattonageService Battonageservice;
            IEvaluationPRVService PRVservice;
            IGroupeEmployeeService serviceGroupeEmp;
            IGroupeService serviceGroupe;
            private ApplicationSignInManager _signInManager;
            private ApplicationUserManager _userManager;
            private ApplicationRoleManager _roleManager;
            #endregion
            #region constructor
            public EvaluationVecteurPlusController()
            {
                serviceEmployee = new EmployeeService();
                serviceDiff = new GrilleEvaluationService();
                service = new EvaluationQRService();
                KLMOservice = new EvaluationKLMOService();
                BPPservice = new EvaluationBPPService();
                BPPTypologieService = new EvaluationBPPTypologieService();
                Battonageservice = new EvaluationBattonageService();
                PRVservice = new EvaluationPRVService();
                serviceGroupeEmp = new GroupesEmployeService();
                serviceGroupe = new GroupeService();
        }
            public EvaluationVecteurPlusController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
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

        #region Evaluation Vecteur Plus QR
        [Authorize(Roles = "Qualité, Agent Qualité")]

        public ActionResult QRVecteurPlus(string enregistrementFullName, string enregistrementUrl, string enregistrementDirectory, string agentName)
        {

            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 5))
            {
                ViewBag.role = "Agent Qualité";
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
        public ActionResult QRVecteurPlusWithoutAgent(string enregistrementFullName, string enregistrementUrl, string enregistrementDirectory, string siteName)
        {
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            var a = new EvaluationViewModel();
            List<SelectListItem> agents = new List<SelectListItem>();
            List<Employee> logins = new List<Employee>();
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 5))
            {
                ViewBag.role = "Agent Qualité";
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
        public ActionResult SaveEvalQRVecteurPlus(string nomAgent, string planDate, string presentation, string objetAppel, string mutualisation, string marcheRenouvlable, string dureeMarche, string coordonneesAttr, string montantMarche, string responsableMarche, string mailContact, string traitementObjections, string transcriptionsInformations, string envoiMail, string discours, string attitude, string priseConge, string commentaire, string enregistrement, string enregistrementUrl, string enregistrementDirectory)
        {
            var userConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            Employee emp = serviceEmployee.getByPseudoName(nomAgent.ToLower());
            GrilleEvaluationQR a = new GrilleEvaluationQR();
            float total = 67;
            List<string> NEList = new List<string>(new string[] { mutualisation, marcheRenouvlable, dureeMarche, coordonneesAttr, montantMarche, responsableMarche, mailContact, traitementObjections, transcriptionsInformations, envoiMail });
            float notes = float.Parse(presentation, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(objetAppel, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(discours, CultureInfo.InvariantCulture.NumberFormat) +
              float.Parse(attitude, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(priseConge, CultureInfo.InvariantCulture.NumberFormat);

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
            a.presentationIdentification = float.Parse(presentation, CultureInfo.InvariantCulture.NumberFormat);
            a.objetAppel = float.Parse(objetAppel, CultureInfo.InvariantCulture.NumberFormat);
            a.mutualisation = float.Parse(mutualisation, CultureInfo.InvariantCulture.NumberFormat);
            a.marcheRenouvlable = float.Parse(marcheRenouvlable, CultureInfo.InvariantCulture.NumberFormat);
            a.dureeMarche = float.Parse(dureeMarche, CultureInfo.InvariantCulture.NumberFormat);
            a.coordonneesAttr = float.Parse(coordonneesAttr, CultureInfo.InvariantCulture.NumberFormat);
            a.montantMarche = float.Parse(montantMarche, CultureInfo.InvariantCulture.NumberFormat);
            a.responsableMarche = float.Parse(responsableMarche, CultureInfo.InvariantCulture.NumberFormat);
            a.mailContact = float.Parse(mailContact, CultureInfo.InvariantCulture.NumberFormat);           
            a.traitementObjections = float.Parse(traitementObjections, CultureInfo.InvariantCulture.NumberFormat);
            a.transcriptionsInformations = float.Parse(transcriptionsInformations, CultureInfo.InvariantCulture.NumberFormat);
            a.envoiMail = float.Parse(envoiMail, CultureInfo.InvariantCulture.NumberFormat);
            a.discours = float.Parse(discours, CultureInfo.InvariantCulture.NumberFormat);
            a.attitude = float.Parse(attitude, CultureInfo.InvariantCulture.NumberFormat);
            a.priseConge = float.Parse(priseConge, CultureInfo.InvariantCulture.NumberFormat);
            a.dateTempEvaluation = DateTime.Parse(planDate);
            a.employeeId = emp.Id;
            a.senderId = userConnected.Id;
            a.note = (notes / total) * 100;
            a.type = "QR Vecteur Plus";
            a.commentaireQualite = commentaire;
            a.enregistrementFullName = enregistrement;
            a.enregistrementUrl = enregistrementUrl;
            a.enregistrementDirectory = enregistrementDirectory;

            service.Add(a);
            service.SaveChange();
            var eval = new EvaluationVecteurPlusViewModel();
            eval.agentName = nomAgent;
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
                return PartialView("EnvoiMailPartialView", eval);
            }
            return RedirectToAction("Acceuil", "Directory");

        }

        public ActionResult CalculQRVecteurPlus(string nomAgent, string planDate, string presentation, string objetAppel, string mutualisation, string marcheRenouvlable, string dureeMarche, string coordonneesAttr, string montantMarche, string responsableMarche, string mailContact, string traitementObjections, string transcriptionsInformations, string envoiMail, string discours, string attitude, string priseConge, string commentaire, string enregistrement, string enregistrementUrl, string enregistrementDirectory)
        {
            float total = 67;
            List<string> NEList = new List<string>(new string[] { mutualisation, marcheRenouvlable, dureeMarche, coordonneesAttr, montantMarche, responsableMarche, mailContact, traitementObjections, transcriptionsInformations, envoiMail });
            float notes = float.Parse(presentation, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(objetAppel, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(discours, CultureInfo.InvariantCulture.NumberFormat) +
              float.Parse(attitude, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(priseConge, CultureInfo.InvariantCulture.NumberFormat);

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
            var a = new EvaluationVecteurPlusViewModel();          
            a.presentationIdentification = float.Parse(presentation, CultureInfo.InvariantCulture.NumberFormat);
            a.objetAppel = float.Parse(objetAppel, CultureInfo.InvariantCulture.NumberFormat);
            a.mutualisation = float.Parse(mutualisation, CultureInfo.InvariantCulture.NumberFormat);
            a.marcheRenouvlable = float.Parse(marcheRenouvlable, CultureInfo.InvariantCulture.NumberFormat);
            a.dureeMarche = float.Parse(dureeMarche, CultureInfo.InvariantCulture.NumberFormat);
            a.coordonneesAttr = float.Parse(coordonneesAttr, CultureInfo.InvariantCulture.NumberFormat);
            a.montantMarche = float.Parse(montantMarche, CultureInfo.InvariantCulture.NumberFormat);
            a.responsableMarche = float.Parse(responsableMarche, CultureInfo.InvariantCulture.NumberFormat);
            a.mailContact = float.Parse(mailContact, CultureInfo.InvariantCulture.NumberFormat);
            a.traitementObjections = float.Parse(traitementObjections, CultureInfo.InvariantCulture.NumberFormat);
            a.transcriptionsInformations = float.Parse(transcriptionsInformations, CultureInfo.InvariantCulture.NumberFormat);
            a.envoiMail = float.Parse(envoiMail, CultureInfo.InvariantCulture.NumberFormat);
            a.discours = float.Parse(discours, CultureInfo.InvariantCulture.NumberFormat);
            a.attitude = float.Parse(attitude, CultureInfo.InvariantCulture.NumberFormat);
            a.priseConge = float.Parse(priseConge, CultureInfo.InvariantCulture.NumberFormat);

            a.dateTempEvaluation = DateTime.Parse(planDate);
            a.agentName = nomAgent;
            a.note = notes;
            a.pourcentageNotes = (notes / total) * 100;
            a.type = "QR Vecteur Plus";

            if (Request.IsAjaxRequest())
            {
                return PartialView("QRResultPartialView", a);
            }
            return RedirectToAction("Acceuil", "Directory");

        }
        #endregion

        #region Evaluation KLMO Vecteur Plus
        public ActionResult KLMOVecteurPlus(string enregistrementFullName, string enregistrementUrl, string enregistrementDirectory, string agentName)
        {

            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 5))
            {
                ViewBag.role = "Agent Qualité";
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
        public ActionResult KLMOVecteurPlusWithoutAgent(string enregistrementFullName, string enregistrementUrl, string enregistrementDirectory, string siteName)
        {
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            var a = new EvaluationViewModel();
            List<SelectListItem> agents = new List<SelectListItem>();
            List<Employee> logins = new List<Employee>();
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 5))
            {
                ViewBag.role = "Agent Qualité";
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
                string d = logins[j].pseudoName.Replace(" ", "-").Replace("é", "Ã©"); ;
                string dd = logins[j].pseudoName.Replace(" ", "--").Replace("é", "Ã©"); 
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
        public ActionResult SaveEvalKLMOVecteurPlus(string nomAgent, string planDate, string presentation, string objetAppel, string mutualisation, string maitriseOeuvre, string coordonneesAttr, string avisAttr, string traitementObjections, string transcriptionsInformations, string envoiMail, string discours, string attitude, string priseConge, string commentaire, string enregistrement, string enregistrementUrl, string enregistrementDirectory)
        {
            var userConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            Employee emp = serviceEmployee.getByPseudoName(nomAgent.ToLower());
            GrilleEvaluationKLMO a = new GrilleEvaluationKLMO();
            float total = 52;
            List<string> NEList = new List<string>(new string[] { mutualisation, coordonneesAttr, maitriseOeuvre, avisAttr, traitementObjections, transcriptionsInformations, envoiMail });
            float notes = float.Parse(presentation, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(objetAppel, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(discours, CultureInfo.InvariantCulture.NumberFormat) +
              float.Parse(attitude, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(priseConge, CultureInfo.InvariantCulture.NumberFormat);

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
            a.presentationIdentification = float.Parse(presentation, CultureInfo.InvariantCulture.NumberFormat);
            a.objetAppel = float.Parse(objetAppel, CultureInfo.InvariantCulture.NumberFormat);
            a.mutualisation = float.Parse(mutualisation, CultureInfo.InvariantCulture.NumberFormat);
            a.maitriseOeuvre = float.Parse(maitriseOeuvre, CultureInfo.InvariantCulture.NumberFormat);
            a.coordonneesAttr = float.Parse(coordonneesAttr, CultureInfo.InvariantCulture.NumberFormat);
            a.avisAttr = float.Parse(avisAttr, CultureInfo.InvariantCulture.NumberFormat);
            a.traitementObjections = float.Parse(traitementObjections, CultureInfo.InvariantCulture.NumberFormat);
            a.transcriptionsInformations = float.Parse(transcriptionsInformations, CultureInfo.InvariantCulture.NumberFormat);
            a.envoiMail = float.Parse(envoiMail, CultureInfo.InvariantCulture.NumberFormat);
            a.discours = float.Parse(discours, CultureInfo.InvariantCulture.NumberFormat);
            a.attitude = float.Parse(attitude, CultureInfo.InvariantCulture.NumberFormat);
            a.priseConge = float.Parse(priseConge, CultureInfo.InvariantCulture.NumberFormat);

            a.dateTempEvaluation = DateTime.Parse(planDate);
            a.employeeId = emp.Id;
            a.senderId = userConnected.Id;
            a.note = (notes / total) * 100;
            a.type = "KLMO Vecteur Plus";
            a.commentaireQualite = commentaire;
            a.enregistrementFullName = enregistrement;
            a.enregistrementUrl = enregistrementUrl;
            a.enregistrementDirectory = enregistrementDirectory;

            KLMOservice.Add(a);
            KLMOservice.SaveChange();
            var eval = new EvaluationVecteurPlusViewModel();
            eval.agentName = nomAgent;

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
                return PartialView("EnvoiMailPartialView", eval);
            }
            return RedirectToAction("Acceuil", "Directory");

        }

        public ActionResult CalculKLMOVecteurPlus(string nomAgent, string planDate, string presentation, string objetAppel, string mutualisation, string maitriseOeuvre, string coordonneesAttr, string avisAttr, string traitementObjections, string transcriptionsInformations, string envoiMail, string discours, string attitude, string priseConge, string commentaire, string enregistrement, string enregistrementUrl, string enregistrementDirectory)
        {
            float total = 52;
            List<string> NEList = new List<string>(new string[] { mutualisation, coordonneesAttr, maitriseOeuvre, avisAttr, traitementObjections, transcriptionsInformations, envoiMail });
            float notes = float.Parse(presentation, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(objetAppel, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(discours, CultureInfo.InvariantCulture.NumberFormat) +
              float.Parse(attitude, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(priseConge, CultureInfo.InvariantCulture.NumberFormat);

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
            var a = new EvaluationVecteurPlusViewModel();
            a.presentationIdentification = float.Parse(presentation, CultureInfo.InvariantCulture.NumberFormat);
            a.objetAppel = float.Parse(objetAppel, CultureInfo.InvariantCulture.NumberFormat);
            a.mutualisation = float.Parse(mutualisation, CultureInfo.InvariantCulture.NumberFormat);
            a.maitriseOeuvre = float.Parse(maitriseOeuvre, CultureInfo.InvariantCulture.NumberFormat);
            a.coordonneesAttr = float.Parse(coordonneesAttr, CultureInfo.InvariantCulture.NumberFormat);
            a.avisAttr = float.Parse(avisAttr, CultureInfo.InvariantCulture.NumberFormat);
            a.traitementObjections = float.Parse(traitementObjections, CultureInfo.InvariantCulture.NumberFormat);
            a.transcriptionsInformations = float.Parse(transcriptionsInformations, CultureInfo.InvariantCulture.NumberFormat);
            a.envoiMail = float.Parse(envoiMail, CultureInfo.InvariantCulture.NumberFormat);
            a.discours = float.Parse(discours, CultureInfo.InvariantCulture.NumberFormat);
            a.attitude = float.Parse(attitude, CultureInfo.InvariantCulture.NumberFormat);
            a.priseConge = float.Parse(priseConge, CultureInfo.InvariantCulture.NumberFormat);

            a.dateTempEvaluation = DateTime.Parse(planDate);
            a.agentName = nomAgent;
            a.note = notes;
            a.pourcentageNotes = (notes / total) * 100;
            a.type = "KLMO Vecteur Plus";

            if (Request.IsAjaxRequest())
            {
                return PartialView("KLMOResultPartialView", a);
            }
            return RedirectToAction("Acceuil", "Directory");

        }
        #endregion

        #region Evaluation BPP Vecteur Plus
        public ActionResult BPPVecteurPlus(string enregistrementFullName, string enregistrementUrl, string enregistrementDirectory, string agentName)
        {

            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 5))
            {
                ViewBag.role = "Agent Qualité";
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
        public ActionResult BPPVecteurPlusWithoutAgent(string enregistrementFullName, string enregistrementUrl, string enregistrementDirectory, string siteName)
        {
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            var a = new EvaluationViewModel();
            List<SelectListItem> agents = new List<SelectListItem>();
            List<Employee> logins = new List<Employee>();
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 5))
            {
                ViewBag.role = "Agent Qualité";
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
                string d = logins[j].pseudoName.Replace(" ", "-").Replace("é", "Ã©"); ;
                string dd = logins[j].pseudoName.Replace(" ", "--").Replace("é", "Ã©"); 
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
      
        public ActionResult SaveEvalBPPVecteurPlus(string nomAgent, string planDate, string presentation, string objetAppel, string validationNature, string calendrier, string envergure, string complétudeQuestions, string confirmationIntervenants, string enchainementQuestions, string traitementObjections, string reformulation, string transcriptionsInformations, string codesAppropriés, string noteClient, string discours, string attitude, string priseConge, string commentaire, string enregistrement, string enregistrementUrl, string enregistrementDirectory)
        {
            var userConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            Employee emp = serviceEmployee.getByPseudoName(nomAgent.ToLower());
            GrilleEvaluationBPP a = new GrilleEvaluationBPP();
            float total = 72;
            List<string> NEList = new List<string>(new string[] { validationNature, calendrier, envergure, complétudeQuestions, confirmationIntervenants, enchainementQuestions, traitementObjections, reformulation, transcriptionsInformations, codesAppropriés, noteClient });
            float notes = float.Parse(presentation, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(objetAppel, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(discours, CultureInfo.InvariantCulture.NumberFormat) +
              float.Parse(attitude, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(priseConge, CultureInfo.InvariantCulture.NumberFormat);

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
            a.presentationIdentification = float.Parse(presentation, CultureInfo.InvariantCulture.NumberFormat);
            a.objetAppel = float.Parse(objetAppel, CultureInfo.InvariantCulture.NumberFormat);
            a.validationNature = float.Parse(validationNature, CultureInfo.InvariantCulture.NumberFormat);
            a.calendrier = float.Parse(calendrier, CultureInfo.InvariantCulture.NumberFormat);
            a.envergure = float.Parse(envergure, CultureInfo.InvariantCulture.NumberFormat);
            a.complétudeQuestions = float.Parse(complétudeQuestions, CultureInfo.InvariantCulture.NumberFormat);
            a.confirmationIntervenants = float.Parse(confirmationIntervenants, CultureInfo.InvariantCulture.NumberFormat);
            a.enchainementQuestions = float.Parse(enchainementQuestions, CultureInfo.InvariantCulture.NumberFormat);
            a.traitementObjections = float.Parse(traitementObjections, CultureInfo.InvariantCulture.NumberFormat);
            a.reformulation = float.Parse(reformulation, CultureInfo.InvariantCulture.NumberFormat);
            a.transcriptionsInformations = float.Parse(transcriptionsInformations, CultureInfo.InvariantCulture.NumberFormat);
            a.codesAppropriés = float.Parse(codesAppropriés, CultureInfo.InvariantCulture.NumberFormat);
            a.noteClient = float.Parse(noteClient, CultureInfo.InvariantCulture.NumberFormat);
            a.discours = float.Parse(discours, CultureInfo.InvariantCulture.NumberFormat);
            a.attitude = float.Parse(attitude, CultureInfo.InvariantCulture.NumberFormat);
            a.priseConge = float.Parse(priseConge, CultureInfo.InvariantCulture.NumberFormat);

            a.dateTempEvaluation = DateTime.Parse(planDate);
            a.employeeId = emp.Id;
            a.senderId = userConnected.Id;
            a.note = (notes / total) * 100;
            a.type = "BPP Vecteur Plus";
            a.commentaireQualite = commentaire;
            a.enregistrementFullName = enregistrement;
            a.enregistrementUrl = enregistrementUrl;
            a.enregistrementDirectory = enregistrementDirectory;

            BPPservice.Add(a);
            BPPservice.SaveChange();
            var eval = new EvaluationVecteurPlusViewModel();
            eval.agentName = nomAgent;
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
                return PartialView("EnvoiMailPartialView", eval);
            }
            return RedirectToAction("Acceuil", "Directory");
        }

        public ActionResult CalculBPPVecteurPlus(string nomAgent, string planDate, string presentation, string objetAppel, string validationNature, string calendrier, string envergure, string complétudeQuestions, string confirmationIntervenants, string enchainementQuestions, string traitementObjections, string reformulation, string transcriptionsInformations, string codesAppropriés, string noteClient, string discours, string attitude, string priseConge, string commentaire, string enregistrement, string enregistrementUrl, string enregistrementDirectory)
        {
            float total = 72;
            List<string> NEList = new List<string>(new string[] { validationNature, calendrier, envergure, complétudeQuestions, confirmationIntervenants, enchainementQuestions, traitementObjections, reformulation, transcriptionsInformations, codesAppropriés, noteClient});
            float notes = float.Parse(presentation, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(objetAppel, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(discours, CultureInfo.InvariantCulture.NumberFormat) +
              float.Parse(attitude, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(priseConge, CultureInfo.InvariantCulture.NumberFormat);

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
            var a = new EvaluationVecteurPlusViewModel();
            a.presentationIdentification = float.Parse(presentation, CultureInfo.InvariantCulture.NumberFormat);
            a.objetAppel = float.Parse(objetAppel, CultureInfo.InvariantCulture.NumberFormat);
            a.validationNature = float.Parse(validationNature, CultureInfo.InvariantCulture.NumberFormat);
            a.calendrier = float.Parse(calendrier, CultureInfo.InvariantCulture.NumberFormat);
            a.envergure = float.Parse(envergure, CultureInfo.InvariantCulture.NumberFormat);
            a.complétudeQuestions = float.Parse(complétudeQuestions, CultureInfo.InvariantCulture.NumberFormat);
            a.confirmationIntervenants = float.Parse(confirmationIntervenants, CultureInfo.InvariantCulture.NumberFormat);
            a.enchainementQuestions = float.Parse(enchainementQuestions, CultureInfo.InvariantCulture.NumberFormat);
            a.traitementObjections = float.Parse(traitementObjections, CultureInfo.InvariantCulture.NumberFormat);
            a.reformulation = float.Parse(reformulation, CultureInfo.InvariantCulture.NumberFormat);
            a.transcriptionsInformations = float.Parse(transcriptionsInformations, CultureInfo.InvariantCulture.NumberFormat);
            a.codesAppropriés = float.Parse(codesAppropriés, CultureInfo.InvariantCulture.NumberFormat);
            a.noteClient = float.Parse(noteClient, CultureInfo.InvariantCulture.NumberFormat);
            a.discours = float.Parse(discours, CultureInfo.InvariantCulture.NumberFormat);
            a.attitude = float.Parse(attitude, CultureInfo.InvariantCulture.NumberFormat);
            a.priseConge = float.Parse(priseConge, CultureInfo.InvariantCulture.NumberFormat);

            a.dateTempEvaluation = DateTime.Parse(planDate);
            a.agentName = nomAgent;
            a.note = notes;
            a.pourcentageNotes = (notes / total) * 100;
            a.type = "BPP Vecteur Plus";

            if (Request.IsAjaxRequest())
            {
                return PartialView("BPPResultPartialView", a);
            }
            return RedirectToAction("Acceuil", "Directory");

        }
        #endregion



        #region Evaluation BPP Typologie Vecteur Plus
        public ActionResult BPPTypologieVecteurPlus(string enregistrementFullName, string enregistrementUrl, string enregistrementDirectory, string agentName)
        {

            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 5))
            {
                ViewBag.role = "Agent Qualité";
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
        public ActionResult BPPTypologieVecteurPlusWithoutAgent(string enregistrementFullName, string enregistrementUrl, string enregistrementDirectory, string siteName)
        {
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            var a = new EvaluationViewModel();
            List<SelectListItem> agents = new List<SelectListItem>();
            List<Employee> logins = new List<Employee>();
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 5))
            {
                ViewBag.role = "Agent Qualité";
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
                string d = logins[j].pseudoName.Replace(" ", "-").Replace("é", "Ã©"); ;
                string dd = logins[j].pseudoName.Replace(" ", "--").Replace("é", "Ã©");
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

        public ActionResult SaveEvalBPPTypologieVecteurPlus(string nomAgent, string planDate, string nombreLogement, string nombreSociaux, string typeCommerce , string typeIndustrie , string typeEtablissementSante , string typeAcceuil , string capaciteAcceuilSante , string typeEtablissementHR , string nombreEtoile , string typeEnseignement , string capaciteAcceuilEnseignement , string typeLoisirs , string frigorifique, string plateformeLogistique, string nombreLogementRs, string nombreSociauxRs, string typeCommerceRs, string typeIndustrieRs, string typeEtablissementSanteRs, string typeAcceuilRs, string capaciteAcceuilSanteRs, string typeEtablissementHRRs, string nombreEtoileRs, string typeEnseignementRs, string capaciteAcceuilEnseignementRs, string typeLoisirsRs, string frigorifiqueRs, string plateformeLogistiqueRs, string commentaire, string enregistrement, string enregistrementUrl, string enregistrementDirectory)
        {
            var userConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            Employee emp = serviceEmployee.getByPseudoName(nomAgent.ToLower());
            GrilleEvaluationBPPTypologie a = new GrilleEvaluationBPPTypologie();
            float total = 14;
            float totalRs = 14;
            List<string> NEList = new List<string>(new string[] { nombreLogement, nombreSociaux, typeCommerce, typeIndustrie, typeEtablissementSante, typeAcceuil, capaciteAcceuilSante, typeEtablissementHR, nombreEtoile, typeEnseignement, capaciteAcceuilEnseignement, typeLoisirs , frigorifique , plateformeLogistique });
            List<string> NEListRs = new List<string>(new string[] { nombreLogementRs, nombreSociauxRs, typeCommerceRs, typeIndustrieRs, typeEtablissementSanteRs, typeAcceuilRs, capaciteAcceuilSanteRs, typeEtablissementHRRs, nombreEtoileRs, typeEnseignementRs, capaciteAcceuilEnseignementRs, typeLoisirsRs, frigorifiqueRs, plateformeLogistiqueRs });
            float notes=0;            
            float notesRs = 0;

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
            foreach (var i in NEListRs)
            {
                if (float.Parse(i, CultureInfo.InvariantCulture.NumberFormat) < 0)
                {
                    totalRs += float.Parse(i, CultureInfo.InvariantCulture.NumberFormat);
                }
                else
                {
                    notesRs += float.Parse(i, CultureInfo.InvariantCulture.NumberFormat);
                }
            }
            a.nombreLogements = float.Parse(nombreLogement, CultureInfo.InvariantCulture.NumberFormat);
            a.nombreSociaux = float.Parse(nombreSociaux, CultureInfo.InvariantCulture.NumberFormat);
            a.typeCommerce = float.Parse(typeCommerce, CultureInfo.InvariantCulture.NumberFormat);
            a.typeIndustrie = float.Parse(typeIndustrie, CultureInfo.InvariantCulture.NumberFormat);
            a.typeEtablissementSante = float.Parse(typeEtablissementSante, CultureInfo.InvariantCulture.NumberFormat);
            a.typeAcceuil = float.Parse(typeAcceuil, CultureInfo.InvariantCulture.NumberFormat);
            a.capaciteAcceuilSante = float.Parse(capaciteAcceuilSante, CultureInfo.InvariantCulture.NumberFormat);
            a.typeEtablissementHR = float.Parse(typeEtablissementHR, CultureInfo.InvariantCulture.NumberFormat);
            a.nombreEtoile = float.Parse(nombreEtoile, CultureInfo.InvariantCulture.NumberFormat);
            a.typeEnseignement = float.Parse(typeEnseignement, CultureInfo.InvariantCulture.NumberFormat);
            a.capaciteAcceuilEnseignement = float.Parse(capaciteAcceuilEnseignement, CultureInfo.InvariantCulture.NumberFormat);
            a.typeLoisirs = float.Parse(typeLoisirs, CultureInfo.InvariantCulture.NumberFormat);
            a.frigorifique = float.Parse(frigorifique, CultureInfo.InvariantCulture.NumberFormat);
            a.plateformeLogistique = float.Parse(plateformeLogistique, CultureInfo.InvariantCulture.NumberFormat);
            //
            a.nombreLogementsRs = float.Parse(nombreLogementRs, CultureInfo.InvariantCulture.NumberFormat);
            a.nombreSociauxRs = float.Parse(nombreSociauxRs, CultureInfo.InvariantCulture.NumberFormat);
            a.typeCommerceRs = float.Parse(typeCommerceRs, CultureInfo.InvariantCulture.NumberFormat);
            a.typeIndustrieRs = float.Parse(typeIndustrieRs, CultureInfo.InvariantCulture.NumberFormat);
            a.typeEtablissementSanteRs = float.Parse(typeEtablissementSanteRs, CultureInfo.InvariantCulture.NumberFormat);
            a.typeAcceuilRs = float.Parse(typeAcceuilRs, CultureInfo.InvariantCulture.NumberFormat);
            a.capaciteAcceuilSanteRs = float.Parse(capaciteAcceuilSanteRs, CultureInfo.InvariantCulture.NumberFormat);
            a.typeEtablissementHRRs = float.Parse(typeEtablissementHRRs, CultureInfo.InvariantCulture.NumberFormat);
            a.nombreEtoileRs = float.Parse(nombreEtoileRs, CultureInfo.InvariantCulture.NumberFormat);
            a.typeEnseignementRs = float.Parse(typeEnseignementRs, CultureInfo.InvariantCulture.NumberFormat);
            a.capaciteAcceuilEnseignementRs = float.Parse(capaciteAcceuilEnseignementRs, CultureInfo.InvariantCulture.NumberFormat);
            a.typeLoisirsRs = float.Parse(typeLoisirsRs, CultureInfo.InvariantCulture.NumberFormat);
            a.frigorifiqueRs = float.Parse(frigorifiqueRs, CultureInfo.InvariantCulture.NumberFormat);
            a.plateformeLogistiqueRs = float.Parse(plateformeLogistiqueRs, CultureInfo.InvariantCulture.NumberFormat);

            a.dateTempEvaluation = DateTime.Parse(planDate);
            a.employeeId = emp.Id;
            a.senderId = userConnected.Id;
            a.note = (notes / total) * 100;
            a.noteRs = (notesRs / total) * 100;
            a.type = "BPP Typologie Vecteur Plus";

            a.commentaireQualite = commentaire;
            a.enregistrementFullName = enregistrement;
            a.enregistrementUrl = enregistrementUrl;
            a.enregistrementDirectory = enregistrementDirectory;

            BPPTypologieService.Add(a);
            BPPTypologieService.SaveChange();

            var eval = new EvaluationVecteurPlusViewModel();
            eval.agentName = nomAgent;

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
                return PartialView("EnvoiMailPartialView", eval);
            }
            return RedirectToAction("Acceuil", "Directory");
        }

        public ActionResult CalculBPPTypologieVecteurPlus(string nomAgent, string planDate, string nombreLogement, string nombreSociaux, string typeCommerce, string typeIndustrie, string typeEtablissementSante, string typeAcceuil, string capaciteAcceuilSante, string typeEtablissementHR, string nombreEtoile, string typeEnseignement, string capaciteAcceuilEnseignement, string typeLoisirs, string frigorifique, string plateformeLogistique, 
            string nombreLogementRs, string nombreSociauxRs, string typeCommerceRs, string typeIndustrieRs, string typeEtablissementSanteRs, string typeAcceuilRs, string capaciteAcceuilSanteRs, string typeEtablissementHRRs, string nombreEtoileRs, string typeEnseignementRs, string capaciteAcceuilEnseignementRs, string typeLoisirsRs, string frigorifiqueRs, string plateformeLogistiqueRs, string commentaire, string enregistrement, string enregistrementUrl, string enregistrementDirectory)
        {
            float total = 14;
            float totalRs = 14;
            List<string> NEList = new List<string>(new string[] { nombreLogement, nombreSociaux, typeCommerce, typeIndustrie, typeEtablissementSante, typeAcceuil, capaciteAcceuilSante, typeEtablissementHR, nombreEtoile, typeEnseignement, capaciteAcceuilEnseignement, typeLoisirs, frigorifique, plateformeLogistique });
            List<string> NEListRs = new List<string>(new string[] { nombreLogementRs, nombreSociauxRs, typeCommerceRs, typeIndustrieRs, typeEtablissementSanteRs, typeAcceuilRs, capaciteAcceuilSanteRs, typeEtablissementHRRs, nombreEtoileRs, typeEnseignementRs, capaciteAcceuilEnseignementRs, typeLoisirsRs, frigorifiqueRs, plateformeLogistiqueRs });
            float notes = 0;
            float notesRs = 0;

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
            foreach (var i in NEListRs)
            {
                if (float.Parse(i, CultureInfo.InvariantCulture.NumberFormat) < 0)
                {
                    totalRs += float.Parse(i, CultureInfo.InvariantCulture.NumberFormat);
                }
                else
                {
                    notesRs += float.Parse(i, CultureInfo.InvariantCulture.NumberFormat);
                }
            }
            
            var a = new EvaluationVecteurPlusViewModel();
            a.nombreLogements = float.Parse(nombreLogement, CultureInfo.InvariantCulture.NumberFormat);
            a.nombreSociaux = float.Parse(nombreSociaux, CultureInfo.InvariantCulture.NumberFormat);
            a.typeCommerce = float.Parse(typeCommerce, CultureInfo.InvariantCulture.NumberFormat);
            a.typeIndustrie = float.Parse(typeIndustrie, CultureInfo.InvariantCulture.NumberFormat);
            a.typeEtablissementSante = float.Parse(typeEtablissementSante, CultureInfo.InvariantCulture.NumberFormat);
            a.typeAcceuil = float.Parse(typeAcceuil, CultureInfo.InvariantCulture.NumberFormat);
            a.capaciteAcceuilSante = float.Parse(capaciteAcceuilSante, CultureInfo.InvariantCulture.NumberFormat);
            a.typeEtablissementHR = float.Parse(typeEtablissementHR, CultureInfo.InvariantCulture.NumberFormat);
            a.nombreEtoile = float.Parse(nombreEtoile, CultureInfo.InvariantCulture.NumberFormat);
            a.typeEnseignement = float.Parse(typeEnseignement, CultureInfo.InvariantCulture.NumberFormat);
            a.capaciteAcceuilEnseignement = float.Parse(capaciteAcceuilEnseignement, CultureInfo.InvariantCulture.NumberFormat);
            a.typeLoisirs = float.Parse(typeLoisirs, CultureInfo.InvariantCulture.NumberFormat);
            a.frigorifique = float.Parse(frigorifique, CultureInfo.InvariantCulture.NumberFormat);
            a.plateformeLogistique = float.Parse(plateformeLogistique, CultureInfo.InvariantCulture.NumberFormat);
            //
            a.nombreLogementsRs = float.Parse(nombreLogementRs, CultureInfo.InvariantCulture.NumberFormat);
            a.nombreSociauxRs = float.Parse(nombreSociauxRs, CultureInfo.InvariantCulture.NumberFormat);
            a.typeCommerceRs = float.Parse(typeCommerceRs, CultureInfo.InvariantCulture.NumberFormat);
            a.typeIndustrieRs = float.Parse(typeIndustrieRs, CultureInfo.InvariantCulture.NumberFormat);
            a.typeEtablissementSanteRs = float.Parse(typeEtablissementSanteRs, CultureInfo.InvariantCulture.NumberFormat);
            a.typeAcceuilRs = float.Parse(typeAcceuilRs, CultureInfo.InvariantCulture.NumberFormat);
            a.capaciteAcceuilSanteRs = float.Parse(capaciteAcceuilSanteRs, CultureInfo.InvariantCulture.NumberFormat);
            a.typeEtablissementHRRs = float.Parse(typeEtablissementHRRs, CultureInfo.InvariantCulture.NumberFormat);
            a.nombreEtoileRs = float.Parse(nombreEtoileRs, CultureInfo.InvariantCulture.NumberFormat);
            a.typeEnseignementRs = float.Parse(typeEnseignementRs, CultureInfo.InvariantCulture.NumberFormat);
            a.capaciteAcceuilEnseignementRs = float.Parse(capaciteAcceuilEnseignementRs, CultureInfo.InvariantCulture.NumberFormat);
            a.typeLoisirsRs = float.Parse(typeLoisirsRs, CultureInfo.InvariantCulture.NumberFormat);
            a.frigorifiqueRs = float.Parse(frigorifiqueRs, CultureInfo.InvariantCulture.NumberFormat);
            a.plateformeLogistiqueRs = float.Parse(plateformeLogistiqueRs, CultureInfo.InvariantCulture.NumberFormat);

            a.dateTempEvaluation = DateTime.Parse(planDate);
            a.agentName = nomAgent;
            a.note = notes;
            a.noteRs = notesRs;
            a.pourcentageNotes = (notes / total) * 100;
            a.pourcentageNotesRs = (notesRs / total) * 100;
            a.type = "BPP Typologie Vecteur Plus";

            if (Request.IsAjaxRequest())
            {
                return PartialView("BPPTypologieResultPartialView", a);
            }
            return RedirectToAction("Acceuil", "Directory");

        }


        //typologie historique

        public ActionResult TypologieHistorique()
        {
            List<Employee> logins = new List<Employee>();
            EvaluationViewModel evaluation = new EvaluationViewModel();
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 5))
            {
                ViewBag.role = "Agent Qualité";
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


            string d = "COMONLINE";
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
        public ActionResult GetHistoTypologieVecteurPlus(string username)
        {
            List<EvaluationVecteurPlusViewModel> a = new List<EvaluationVecteurPlusViewModel>();
            try
            {
                Employee emp = serviceEmployee.getByLogin(username);
                var historstionsBPP = BPPTypologieService.GetEvaluationsByEmployee(emp.Id);
                ViewBag.nbreEvaluations = historstionsBPP.Count();
                foreach (var item in historstionsBPP)
                {
                    EvaluationVecteurPlusViewModel test = new EvaluationVecteurPlusViewModel();
                    test.Id = item.Id;
                    //
                    test.nombreLogements = item.nombreLogements;
                    test.nombreSociaux = item.nombreSociaux;
                    test.typeCommerce = item.typeCommerce;
                    test.typeIndustrie = item.typeIndustrie;
                    test.typeEtablissementSante = item.typeEtablissementSante;
                    test.typeAcceuil = item.typeAcceuil;
                    test.capaciteAcceuilSante = item.capaciteAcceuilSante;
                    test.typeEtablissementHR = item.typeEtablissementHR;
                    test.nombreEtoile = item.nombreEtoile;
                    test.typeEnseignement = item.typeEnseignement;
                    test.capaciteAcceuilEnseignement = item.capaciteAcceuilEnseignement;
                    test.typeLoisirs = item.typeLoisirs;
                    test.frigorifique = item.frigorifique;
                    test.plateformeLogistique = item.plateformeLogistique;

                    //
                    test.nombreLogementsRs = item.nombreLogementsRs;
                    test.nombreSociauxRs = item.nombreSociauxRs;
                    test.typeCommerceRs = item.typeCommerceRs;
                    test.typeIndustrieRs = item.typeIndustrieRs;
                    test.typeEtablissementSanteRs = item.typeEtablissementSanteRs;
                    test.typeAcceuilRs = item.typeAcceuilRs;
                    test.capaciteAcceuilSanteRs = item.capaciteAcceuilSanteRs;
                    test.typeEtablissementHRRs = item.typeEtablissementHRRs;
                    test.nombreEtoileRs = item.nombreEtoileRs;
                    test.typeEnseignementRs = item.typeEnseignementRs;
                    test.capaciteAcceuilEnseignementRs = item.capaciteAcceuilEnseignementRs;
                    test.typeLoisirsRs = item.typeLoisirsRs;
                    test.frigorifiqueRs = item.frigorifiqueRs;
                    test.plateformeLogistiqueRs = item.plateformeLogistiqueRs;
                    //

                    test.dateTempEvaluation = item.dateTempEvaluation;
                    test.type = item.type;
                    test.note = item.note;
                    test.noteRs = item.noteRs;
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
                    return PartialView("HistoriqueTypologieTablePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("TypologieHistorique");
        }
        public ActionResult GetHistoTypologieByDate(string username, string dateDebut, string dateFin)
        {
            List<EvaluationVecteurPlusViewModel> a = new List<EvaluationVecteurPlusViewModel>();
            try
            {
                Employee emp = serviceEmployee.getByLogin(username);
                DateTime daterecherchedebut = DateTime.Parse(dateDebut);
                DateTime daterecherchefin = DateTime.Parse(dateFin);

                var historstions = BPPTypologieService.GetEvaluationsEmployeeBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationVecteurPlusViewModel test = new EvaluationVecteurPlusViewModel();
                    test.Id = item.Id;
                    test.type = item.type;

                    test.nombreLogements = item.nombreLogements;
                    test.nombreSociaux = item.nombreSociaux;
                    test.typeCommerce = item.typeCommerce;
                    test.typeIndustrie = item.typeIndustrie;
                    test.typeEtablissementSante = item.typeEtablissementSante;
                    test.typeAcceuil = item.typeAcceuil;
                    test.capaciteAcceuilSante = item.capaciteAcceuilSante;
                    test.typeEtablissementHR = item.typeEtablissementHR;
                    test.nombreEtoile = item.nombreEtoile;
                    test.typeEnseignement = item.typeEnseignement;
                    test.capaciteAcceuilEnseignement = item.capaciteAcceuilEnseignement;
                    test.typeLoisirs = item.typeLoisirs;
                    test.frigorifique = item.frigorifique;
                    test.plateformeLogistique = item.plateformeLogistique;

                    //
                    test.nombreLogementsRs = item.nombreLogementsRs;
                    test.nombreSociauxRs = item.nombreSociauxRs;
                    test.typeCommerceRs = item.typeCommerceRs;
                    test.typeIndustrieRs = item.typeIndustrieRs;
                    test.typeEtablissementSanteRs = item.typeEtablissementSanteRs;
                    test.typeAcceuilRs = item.typeAcceuilRs;
                    test.capaciteAcceuilSanteRs = item.capaciteAcceuilSanteRs;
                    test.typeEtablissementHRRs = item.typeEtablissementHRRs;
                    test.nombreEtoileRs = item.nombreEtoileRs;
                    test.typeEnseignementRs = item.typeEnseignementRs;
                    test.capaciteAcceuilEnseignementRs = item.capaciteAcceuilEnseignementRs;
                    test.typeLoisirsRs = item.typeLoisirsRs;
                    test.frigorifiqueRs = item.frigorifiqueRs;
                    test.plateformeLogistiqueRs = item.plateformeLogistiqueRs;
                    //

                    test.note = item.note;
                    test.noteRs = item.noteRs;

                    test.dateTempEvaluation = item.dateTempEvaluation;
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
                    return PartialView("HistoriqueTypologieTablePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("TypologieHistorique");
        }

        //Archive Typologie Qualité
        public ActionResult ArchiveTypologieEvaluationsQualite()
        {
            EvaluationVecteurPlusViewModel evaluation = new EvaluationVecteurPlusViewModel();
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            var logins = serviceEmployee.GetAll();
            var employees = logins.Select(o => o).Distinct().ToList();

            foreach (var test in employees)
            {
                if (test.Roles.Any(r => r.UserId == test.Id && r.RoleId == 4 || r.RoleId == 5))
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

        public ActionResult GetTypologieArchiveEvaluationsQualite(string username)
        {
            List<EvaluationVecteurPlusViewModel> a = new List<EvaluationVecteurPlusViewModel>();
            try
            {
                Employee emp = serviceEmployee.getByLogin(username);
                var historstions = BPPTypologieService.GetEvaluationsBySenderId(emp.Id);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationVecteurPlusViewModel test = new EvaluationVecteurPlusViewModel();
                    test.Id = item.Id;

                    test.nombreLogements = item.nombreLogements;
                    test.nombreSociaux = item.nombreSociaux;
                    test.typeCommerce = item.typeCommerce;
                    test.typeIndustrie = item.typeIndustrie;
                    test.typeEtablissementSante = item.typeEtablissementSante;
                    test.typeAcceuil = item.typeAcceuil;
                    test.capaciteAcceuilSante = item.capaciteAcceuilSante;
                    test.typeEtablissementHR = item.typeEtablissementHR;
                    test.nombreEtoile = item.nombreEtoile;
                    test.typeEnseignement = item.typeEnseignement;
                    test.capaciteAcceuilEnseignement = item.capaciteAcceuilEnseignement;
                    test.typeLoisirs = item.typeLoisirs;
                    test.frigorifique = item.frigorifique;
                    test.plateformeLogistique = item.plateformeLogistique;

                    //
                    test.nombreLogementsRs = item.nombreLogementsRs;
                    test.nombreSociauxRs = item.nombreSociauxRs;
                    test.typeCommerceRs = item.typeCommerceRs;
                    test.typeIndustrieRs = item.typeIndustrieRs;
                    test.typeEtablissementSanteRs = item.typeEtablissementSanteRs;
                    test.typeAcceuilRs = item.typeAcceuilRs;
                    test.capaciteAcceuilSanteRs = item.capaciteAcceuilSanteRs;
                    test.typeEtablissementHRRs = item.typeEtablissementHRRs;
                    test.nombreEtoileRs = item.nombreEtoileRs;
                    test.typeEnseignementRs = item.typeEnseignementRs;
                    test.capaciteAcceuilEnseignementRs = item.capaciteAcceuilEnseignementRs;
                    test.typeLoisirsRs = item.typeLoisirsRs;
                    test.frigorifiqueRs = item.frigorifiqueRs;
                    test.plateformeLogistiqueRs = item.plateformeLogistiqueRs;
                    //
                                    
                    test.note = item.note;
                    test.noteRs = item.noteRs;

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
                    return PartialView("ArchiveTypologieTablePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("ArchiveTypologieEvaluationsQualite");
        }
                            
        public ActionResult GetArchiveTypologieEvaluationsByDate(string username, string dateDebut, string dateFin)
        {
            List<EvaluationVecteurPlusViewModel> a = new List<EvaluationVecteurPlusViewModel>();
            try
            {
                Employee emp = serviceEmployee.getByLogin(username);
                DateTime daterecherchedebut = DateTime.Parse(dateDebut);
                DateTime daterecherchefin = DateTime.Parse(dateFin);
                var historstions = BPPTypologieService.GetEvaluationsSenderBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationVecteurPlusViewModel test = new EvaluationVecteurPlusViewModel();
                    test.Id = item.Id;

                    test.nombreLogements = item.nombreLogements;
                    test.nombreSociaux = item.nombreSociaux;
                    test.typeCommerce = item.typeCommerce;
                    test.typeIndustrie = item.typeIndustrie;
                    test.typeEtablissementSante = item.typeEtablissementSante;
                    test.typeAcceuil = item.typeAcceuil;
                    test.capaciteAcceuilSante = item.capaciteAcceuilSante;
                    test.typeEtablissementHR = item.typeEtablissementHR;
                    test.nombreEtoile = item.nombreEtoile;
                    test.typeEnseignement = item.typeEnseignement;
                    test.capaciteAcceuilEnseignement = item.capaciteAcceuilEnseignement;
                    test.typeLoisirs = item.typeLoisirs;
                    test.frigorifique = item.frigorifique;
                    test.plateformeLogistique = item.plateformeLogistique;

                    //
                    test.nombreLogementsRs = item.nombreLogementsRs;
                    test.nombreSociauxRs = item.nombreSociauxRs;
                    test.typeCommerceRs = item.typeCommerceRs;
                    test.typeIndustrieRs = item.typeIndustrieRs;
                    test.typeEtablissementSanteRs = item.typeEtablissementSanteRs;
                    test.typeAcceuilRs = item.typeAcceuilRs;
                    test.capaciteAcceuilSanteRs = item.capaciteAcceuilSanteRs;
                    test.typeEtablissementHRRs = item.typeEtablissementHRRs;
                    test.nombreEtoileRs = item.nombreEtoileRs;
                    test.typeEnseignementRs = item.typeEnseignementRs;
                    test.capaciteAcceuilEnseignementRs = item.capaciteAcceuilEnseignementRs;
                    test.typeLoisirsRs = item.typeLoisirsRs;
                    test.frigorifiqueRs = item.frigorifiqueRs;
                    test.plateformeLogistiqueRs = item.plateformeLogistiqueRs;
                    //
                    //                    
                    test.note = item.note;
                    test.noteRs = item.noteRs;
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
                    return PartialView("ArchiveTypologieTablePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("ArchiveTypologieEvaluationsQualite");
        }

        //Archive Typologie par Agent Qualité
        public ActionResult ArchiveTypologieEvaluationsAgentQualite()
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
                           
        public ActionResult GetArchiveTypologieEvaluationsAgentQualite()
        {
            List<EvaluationVecteurPlusViewModel> a = new List<EvaluationVecteurPlusViewModel>();
            try
            {
                Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

                var historstions = BPPTypologieService.GetEvaluationsBySenderId(emp.Id);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationVecteurPlusViewModel test = new EvaluationVecteurPlusViewModel();
                    test.Id = item.Id;
                    test.nombreLogements = item.nombreLogements;
                    test.nombreSociaux = item.nombreSociaux;
                    test.typeCommerce = item.typeCommerce;
                    test.typeIndustrie = item.typeIndustrie;
                    test.typeEtablissementSante = item.typeEtablissementSante;
                    test.typeAcceuil = item.typeAcceuil;
                    test.capaciteAcceuilSante = item.capaciteAcceuilSante;
                    test.typeEtablissementHR = item.typeEtablissementHR;
                    test.nombreEtoile = item.nombreEtoile;
                    test.typeEnseignement = item.typeEnseignement;
                    test.capaciteAcceuilEnseignement = item.capaciteAcceuilEnseignement;
                    test.typeLoisirs = item.typeLoisirs;
                    test.frigorifique = item.frigorifique;
                    test.plateformeLogistique = item.plateformeLogistique;

                    //
                    test.nombreLogementsRs = item.nombreLogementsRs;
                    test.nombreSociauxRs = item.nombreSociauxRs;
                    test.typeCommerceRs = item.typeCommerceRs;
                    test.typeIndustrieRs = item.typeIndustrieRs;
                    test.typeEtablissementSanteRs = item.typeEtablissementSanteRs;
                    test.typeAcceuilRs = item.typeAcceuilRs;
                    test.capaciteAcceuilSanteRs = item.capaciteAcceuilSanteRs;
                    test.typeEtablissementHRRs = item.typeEtablissementHRRs;
                    test.nombreEtoileRs = item.nombreEtoileRs;
                    test.typeEnseignementRs = item.typeEnseignementRs;
                    test.capaciteAcceuilEnseignementRs = item.capaciteAcceuilEnseignementRs;
                    test.typeLoisirsRs = item.typeLoisirsRs;
                    test.frigorifiqueRs = item.frigorifiqueRs;
                    test.plateformeLogistiqueRs = item.plateformeLogistiqueRs;
                    //
                    test.note = item.note;
                    test.noteRs = item.noteRs;


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
                    return PartialView("ArchiveTypologieTablePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("ArchiveTypologieEvaluationsAgentQualite");
        }
                            
        public ActionResult GetArchiveTypologieEvaluationsAgentQualiteByDate(string dateDebut, string dateFin)
        {
            List<EvaluationVecteurPlusViewModel> a = new List<EvaluationVecteurPlusViewModel>();
            try
            {
                Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
                DateTime daterecherchedebut = DateTime.Parse(dateDebut);
                DateTime daterecherchefin = DateTime.Parse(dateFin);
                var historstions = BPPTypologieService.GetEvaluationsSenderBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationVecteurPlusViewModel test = new EvaluationVecteurPlusViewModel();
                    test.Id = item.Id;
                    //
                    test.nombreLogements = item.nombreLogements;
                    test.nombreSociaux = item.nombreSociaux;
                    test.typeCommerce = item.typeCommerce;
                    test.typeIndustrie = item.typeIndustrie;
                    test.typeEtablissementSante = item.typeEtablissementSante;
                    test.typeAcceuil = item.typeAcceuil;
                    test.capaciteAcceuilSante = item.capaciteAcceuilSante;
                    test.typeEtablissementHR = item.typeEtablissementHR;
                    test.nombreEtoile = item.nombreEtoile;
                    test.typeEnseignement = item.typeEnseignement;
                    test.capaciteAcceuilEnseignement = item.capaciteAcceuilEnseignement;
                    test.typeLoisirs = item.typeLoisirs;
                    test.frigorifique = item.frigorifique;
                    test.plateformeLogistique = item.plateformeLogistique;

                    //
                    test.nombreLogementsRs = item.nombreLogementsRs;
                    test.nombreSociauxRs = item.nombreSociauxRs;
                    test.typeCommerceRs = item.typeCommerceRs;
                    test.typeIndustrieRs = item.typeIndustrieRs;
                    test.typeEtablissementSanteRs = item.typeEtablissementSanteRs;
                    test.typeAcceuilRs = item.typeAcceuilRs;
                    test.capaciteAcceuilSanteRs = item.capaciteAcceuilSanteRs;
                    test.typeEtablissementHRRs = item.typeEtablissementHRRs;
                    test.nombreEtoileRs = item.nombreEtoileRs;
                    test.typeEnseignementRs = item.typeEnseignementRs;
                    test.capaciteAcceuilEnseignementRs = item.capaciteAcceuilEnseignementRs;
                    test.typeLoisirsRs = item.typeLoisirsRs;
                    test.frigorifiqueRs = item.frigorifiqueRs;
                    test.plateformeLogistiqueRs = item.plateformeLogistiqueRs;
                    //
                    test.note = item.note;
                    test.noteRs = item.noteRs;

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
                    return PartialView("ArchiveTypologieTablePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("ArchiveTypologieEvaluationsAgentQualite");
        }
   



        #endregion


        #region Historique Vecteur Plus Par Collaborateur
        [Authorize(Roles = "Qualité, Agent Qualité, Manager,SuperManager")]
        //Historique QR
        public ActionResult QRHistorique()
        {
            List<Employee> logins = new List<Employee>();
            EvaluationViewModel evaluation = new EvaluationViewModel();
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 5))
            {
                ViewBag.role = "Agent Qualité";
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
   

            string d = "COMONLINE";
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
                if (!test.UserName.Equals(user.UserName) && test.Roles.Any(r => r.UserId == test.Id && r.RoleId == 1010))
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
        public ActionResult GetHistoQRVecteurPlus(string username)
        {
            List<EvaluationVecteurPlusViewModel> a = new List<EvaluationVecteurPlusViewModel>();
            try
            {
                Employee emp = serviceEmployee.getByLogin(username);
                    var historstionsQR = service.GetEvaluationsByEmployee(emp.Id);
                    ViewBag.nbreEvaluations = historstionsQR.Count();
                    foreach (var item in historstionsQR)
                    {
                    EvaluationVecteurPlusViewModel test = new EvaluationVecteurPlusViewModel();
                    test.Id = item.Id;
                    test.presentationIdentification = item.presentationIdentification;
                    test.objetAppel = item.objetAppel;
                    test.mutualisation = item.mutualisation;
                    test.coordonneesAttr = item.coordonneesAttr;                   
                    test.marcheRenouvlable = item.marcheRenouvlable;
                    test.dureeMarche = item.dureeMarche;
                    test.montantMarche = item.montantMarche;
                    test.responsableMarche = item.responsableMarche;
                    test.mailContact = item.mailContact;
                    test.traitementObjections = item.traitementObjections;
                    test.transcriptionsInformations = item.transcriptionsInformations;
                    test.envoiMail = item.envoiMail;
                    test.discours = item.discours;
                    test.attitude = item.attitude;
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
                        return PartialView("HistoriqueQRTablePartialView", a);
                    }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("QRHistorique");
        }
        public ActionResult GetHistoQRByDate(string username, string dateDebut, string dateFin)
        {
            List<EvaluationVecteurPlusViewModel> a = new List<EvaluationVecteurPlusViewModel>();
            try
            {
                Employee emp = serviceEmployee.getByLogin(username);
                DateTime daterecherchedebut = DateTime.Parse(dateDebut);
                DateTime daterecherchefin = DateTime.Parse(dateFin);


                var historstionsQR = service.GetEvaluationsEmployeeBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
                ViewBag.nbreEvaluations = historstionsQR.Count();
                foreach (var item in historstionsQR)
                {
                    EvaluationVecteurPlusViewModel test = new EvaluationVecteurPlusViewModel();
                    test.Id = item.Id;
                    test.presentationIdentification = item.presentationIdentification;
                    test.objetAppel = item.objetAppel;
                    test.mutualisation = item.mutualisation;
                    test.coordonneesAttr = item.coordonneesAttr;
                    test.marcheRenouvlable = item.marcheRenouvlable;
                    test.dureeMarche = item.dureeMarche;
                    test.montantMarche = item.montantMarche;
                    test.responsableMarche = item.responsableMarche;
                    test.mailContact = item.mailContact;
                    test.traitementObjections = item.traitementObjections;
                    test.transcriptionsInformations = item.transcriptionsInformations;
                    test.envoiMail = item.envoiMail;
                    test.discours = item.discours;
                    test.attitude = item.attitude;
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
                    return PartialView("HistoriqueQRTablePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("QRHistorique");
        }
        //KLMO Historique
        public ActionResult KLMOHistorique()
        {
            List<Employee> logins = new List<Employee>();
            EvaluationViewModel evaluation = new EvaluationViewModel();
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 5))
            {
                ViewBag.role = "Agent Qualité";
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


            string d = "COMONLINE";
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
                if (!test.UserName.Equals(user.UserName) && test.Roles.Any(r => r.UserId == test.Id && r.RoleId == 1011))
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
        public ActionResult GetHistoKLMOVecteurPlus( string username)
        {
            List<EvaluationVecteurPlusViewModel> a = new List<EvaluationVecteurPlusViewModel>();
            try
            {
                Employee emp = serviceEmployee.getByLogin(username);
                var historstionsKLMO = KLMOservice.GetEvaluationsByEmployee(emp.Id);
                ViewBag.nbreEvaluations = historstionsKLMO.Count();
                foreach (var item in historstionsKLMO)
                {
                    EvaluationVecteurPlusViewModel test = new EvaluationVecteurPlusViewModel();
                    test.Id = item.Id;
                    test.presentationIdentification = item.presentationIdentification;
                    test.objetAppel = item.objetAppel;
                    test.mutualisation = item.mutualisation;
                    test.maitriseOeuvre = item.maitriseOeuvre;
                    test.coordonneesAttr = item.coordonneesAttr;                  
                    test.avisAttr = item.avisAttr;
                    test.traitementObjections = item.traitementObjections;
                    test.transcriptionsInformations = item.transcriptionsInformations;
                    test.envoiMail = item.envoiMail;
                    test.discours = item.discours;
                    test.attitude = item.attitude;
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
                    return PartialView("HistoriqueKLMOTablePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("KLMOHistorique");
        }
        public ActionResult GetHistoKLMOByDate(string username, string dateDebut, string dateFin)
        {
            List<EvaluationVecteurPlusViewModel> a = new List<EvaluationVecteurPlusViewModel>();
            try
            {
                Employee emp = serviceEmployee.getByLogin(username);
                DateTime daterecherchedebut = DateTime.Parse(dateDebut);
                DateTime daterecherchefin = DateTime.Parse(dateFin);


                var historstionsKLMO = KLMOservice.GetEvaluationsEmployeeBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
                ViewBag.nbreEvaluations = historstionsKLMO.Count();
                foreach (var item in historstionsKLMO)
                {
                    EvaluationVecteurPlusViewModel test = new EvaluationVecteurPlusViewModel();
                    test.Id = item.Id;
                    test.presentationIdentification = item.presentationIdentification;
                    test.objetAppel = item.objetAppel;
                    test.mutualisation = item.mutualisation;
                    test.maitriseOeuvre = item.maitriseOeuvre;
                    test.coordonneesAttr = item.coordonneesAttr;
                    test.avisAttr = item.avisAttr;
                    test.traitementObjections = item.traitementObjections;
                    test.transcriptionsInformations = item.transcriptionsInformations;
                    test.envoiMail = item.envoiMail;
                    test.discours = item.discours;
                    test.attitude = item.attitude;
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
                    return PartialView("HistoriqueKLMOTablePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("KLMOHistorique");
        }
        //BPP Historique
        public ActionResult BPPHistorique()
        {
            List<Employee> logins = new List<Employee>();
            EvaluationViewModel evaluation = new EvaluationViewModel();
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 5))
            {
                ViewBag.role = "Agent Qualité";
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


            string d = "COMONLINE";
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
        public ActionResult GetHistoBPPVecteurPlus(string username)
        {
            List<EvaluationVecteurPlusViewModel> a = new List<EvaluationVecteurPlusViewModel>();
            try
            {
                Employee emp = serviceEmployee.getByLogin(username);
                var historstionsBPP = BPPservice.GetEvaluationsByEmployee(emp.Id);
                ViewBag.nbreEvaluations = historstionsBPP.Count();
                foreach (var item in historstionsBPP)
                {
                    EvaluationVecteurPlusViewModel test = new EvaluationVecteurPlusViewModel();
                    test.Id = item.Id;
                    test.presentationIdentification = item.presentationIdentification;
                    test.objetAppel = item.objetAppel;
                    test.validationNature = item.validationNature;
                    test.calendrier = item.calendrier;
                    test.envergure = item.envergure;
                    test.complétudeQuestions = item.complétudeQuestions;
                    test.confirmationIntervenants = item.confirmationIntervenants;
                    test.enchainementQuestions = item.enchainementQuestions;
                    test.traitementObjections = item.traitementObjections;
                    test.reformulation = item.reformulation;
                    test.transcriptionsInformations = item.transcriptionsInformations;
                    test.codesAppropriés = item.codesAppropriés;
                    test.noteClient = item.noteClient;
                    test.discours = item.discours;
                    test.attitude = item.attitude;
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
                    return PartialView("HistoriqueBPPTablePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("BPPHistorique");
        }
        public ActionResult GetHistoBPPByDate(string username, string dateDebut, string dateFin)
        {
            List<EvaluationVecteurPlusViewModel> a = new List<EvaluationVecteurPlusViewModel>();
            try
            {
                Employee emp = serviceEmployee.getByLogin(username);
                DateTime daterecherchedebut = DateTime.Parse(dateDebut);
                DateTime daterecherchefin = DateTime.Parse(dateFin);

                var historstionsBPP = BPPservice.GetEvaluationsEmployeeBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
                ViewBag.nbreEvaluations = historstionsBPP.Count();
                foreach (var item in historstionsBPP)
                {
                    EvaluationVecteurPlusViewModel test = new EvaluationVecteurPlusViewModel();
                    test.Id = item.Id;
                    test.presentationIdentification = item.presentationIdentification;
                    test.objetAppel = item.objetAppel;
                    test.validationNature = item.validationNature;
                    test.calendrier = item.calendrier;
                    test.envergure = item.envergure;
                    test.complétudeQuestions = item.complétudeQuestions;
                    test.confirmationIntervenants = item.confirmationIntervenants;
                    test.enchainementQuestions = item.enchainementQuestions;
                    test.traitementObjections = item.traitementObjections;
                    test.reformulation = item.reformulation;
                    test.transcriptionsInformations = item.transcriptionsInformations;
                    test.codesAppropriés = item.codesAppropriés;
                    test.noteClient = item.noteClient;
                    test.discours = item.discours;
                    test.attitude = item.attitude;
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
                    return PartialView("HistoriqueBPPTablePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("BPPHistorique");
        }
        #endregion

        #region Archive Vecteur Plus Evaluations du Resp Qualité
        //Archive evaluations par Responsable Qualité
        [Authorize(Roles = "Qualité")]
        public ActionResult ArchiveQREvaluationsQualite()
        {
            EvaluationVecteurPlusViewModel evaluation = new EvaluationVecteurPlusViewModel();
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            var logins = serviceEmployee.GetAll();
            var employees = logins.Select(o => o).Distinct().ToList();

            foreach (var test in employees)
            {
                if (test.Roles.Any(r => r.UserId == test.Id && r.RoleId == 4 || r.RoleId == 5))
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

        public ActionResult GetQRArchiveEvaluationsQualite(string username)
        {
            List<EvaluationVecteurPlusViewModel> a = new List<EvaluationVecteurPlusViewModel>();
            try
            {
                Employee emp = serviceEmployee.getByLogin(username);
                var historstions = service.GetEvaluationsBySenderId(emp.Id);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationVecteurPlusViewModel test = new EvaluationVecteurPlusViewModel();
                    test.Id = item.Id;
                    test.presentationIdentification = item.presentationIdentification;
                    test.objetAppel = item.objetAppel;
                    test.mutualisation = item.mutualisation;
                    test.coordonneesAttr = item.coordonneesAttr;
                    test.marcheRenouvlable = item.marcheRenouvlable;
                    test.dureeMarche = item.dureeMarche;
                    test.montantMarche = item.montantMarche;
                    test.responsableMarche = item.responsableMarche;
                    test.mailContact = item.mailContact;
                    test.traitementObjections = item.traitementObjections;
                    test.transcriptionsInformations = item.transcriptionsInformations;
                    test.envoiMail = item.envoiMail;
                    test.discours = item.discours;
                    test.attitude = item.attitude;
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
                    return PartialView("ArchiveQRTablePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("ArchiveQREvaluationsQualite");
        }

        public ActionResult GetArchiveQREvaluationsByDate(string username, string dateDebut, string dateFin)
        {
            List<EvaluationVecteurPlusViewModel> a = new List<EvaluationVecteurPlusViewModel>();
            try
            {
                Employee emp = serviceEmployee.getByLogin(username);
                DateTime daterecherchedebut = DateTime.Parse(dateDebut);
                DateTime daterecherchefin = DateTime.Parse(dateFin);
                var historstions = service.GetEvaluationsSenderBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationVecteurPlusViewModel test = new EvaluationVecteurPlusViewModel();
                    test.Id = item.Id;
                    test.presentationIdentification = item.presentationIdentification;
                    test.objetAppel = item.objetAppel;
                    test.mutualisation = item.mutualisation;
                    test.coordonneesAttr = item.coordonneesAttr;
                    test.marcheRenouvlable = item.marcheRenouvlable;
                    test.dureeMarche = item.dureeMarche;
                    test.montantMarche = item.montantMarche;
                    test.responsableMarche = item.responsableMarche;
                    test.mailContact = item.mailContact;
                    test.traitementObjections = item.traitementObjections;
                    test.transcriptionsInformations = item.transcriptionsInformations;
                    test.envoiMail = item.envoiMail;
                    test.discours = item.discours;
                    test.attitude = item.attitude;
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
                    return PartialView("ArchiveQRTablePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("ArchiveQREvaluationsQualite");
        }
        //KLMO Archive Par Qualité
        public ActionResult ArchiveKLMOEvaluationsQualite()
        {
            EvaluationVecteurPlusViewModel evaluation = new EvaluationVecteurPlusViewModel();
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            var logins = serviceEmployee.GetAll();
            var employees = logins.Select(o => o).Distinct().ToList();

            foreach (var test in employees)
            {
                if (test.Roles.Any(r => r.UserId == test.Id && r.RoleId == 4 || r.RoleId == 5))
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

        public ActionResult GetKLMOArchiveEvaluationsQualite(string username)
        {
            List<EvaluationVecteurPlusViewModel> a = new List<EvaluationVecteurPlusViewModel>();
            try
            {
                Employee emp = serviceEmployee.getByLogin(username);
                var historstions = KLMOservice.GetEvaluationsBySenderId(emp.Id);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationVecteurPlusViewModel test = new EvaluationVecteurPlusViewModel();
                    test.Id = item.Id;
                    test.presentationIdentification = item.presentationIdentification;
                    test.objetAppel = item.objetAppel;
                    test.mutualisation = item.mutualisation;
                    test.maitriseOeuvre = item.maitriseOeuvre;
                    test.coordonneesAttr = item.coordonneesAttr;
                    test.avisAttr = item.avisAttr;
                    test.traitementObjections = item.traitementObjections;
                    test.transcriptionsInformations = item.transcriptionsInformations;
                    test.envoiMail = item.envoiMail;
                    test.discours = item.discours;
                    test.attitude = item.attitude;
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
                    return PartialView("ArchiveKLMOTablePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("ArchiveKLMOEvaluationsQualite");
        }

        public ActionResult GetArchiveKLMOEvaluationsByDate(string username, string dateDebut, string dateFin)
        {
            List<EvaluationVecteurPlusViewModel> a = new List<EvaluationVecteurPlusViewModel>();
            try
            {
                Employee emp = serviceEmployee.getByLogin(username);
                DateTime daterecherchedebut = DateTime.Parse(dateDebut);
                DateTime daterecherchefin = DateTime.Parse(dateFin);
                var historstions = KLMOservice.GetEvaluationsSenderBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationVecteurPlusViewModel test = new EvaluationVecteurPlusViewModel();
                    test.Id = item.Id;
                    test.presentationIdentification = item.presentationIdentification;
                    test.objetAppel = item.objetAppel;
                    test.mutualisation = item.mutualisation;
                    test.maitriseOeuvre = item.maitriseOeuvre;
                    test.coordonneesAttr = item.coordonneesAttr;
                    test.avisAttr = item.avisAttr;
                    test.traitementObjections = item.traitementObjections;
                    test.transcriptionsInformations = item.transcriptionsInformations;
                    test.envoiMail = item.envoiMail;
                    test.discours = item.discours;
                    test.attitude = item.attitude;
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
                    return PartialView("ArchiveKLMOTablePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("ArchiveKLMOEvaluationsQualite");
        }
        //BPP Archive Par Qualité
        public ActionResult ArchiveBPPEvaluationsQualite()
        {
            EvaluationVecteurPlusViewModel evaluation = new EvaluationVecteurPlusViewModel();
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            var logins = serviceEmployee.GetAll();
            var employees = logins.Select(o => o).Distinct().ToList();

            foreach (var test in employees)
            {
                if (test.Roles.Any(r => r.UserId == test.Id && r.RoleId == 4 || r.RoleId == 5))
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

        public ActionResult GetBPPArchiveEvaluationsQualite(string username)
        {
            List<EvaluationVecteurPlusViewModel> a = new List<EvaluationVecteurPlusViewModel>();
            try
            {
                Employee emp = serviceEmployee.getByLogin(username);
                var historstions = BPPservice.GetEvaluationsBySenderId(emp.Id);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationVecteurPlusViewModel test = new EvaluationVecteurPlusViewModel();
                    test.Id = item.Id;
                    test.presentationIdentification = item.presentationIdentification;
                    test.objetAppel = item.objetAppel;
                    test.validationNature = item.validationNature;
                    test.calendrier = item.calendrier;
                    test.envergure = item.envergure;
                    test.complétudeQuestions = item.complétudeQuestions;
                    test.confirmationIntervenants = item.confirmationIntervenants;
                    test.enchainementQuestions = item.enchainementQuestions;
                    test.traitementObjections = item.traitementObjections;
                    test.reformulation = item.reformulation;
                    test.transcriptionsInformations = item.transcriptionsInformations;
                    test.codesAppropriés = item.codesAppropriés;
                    test.noteClient = item.noteClient;
                    test.discours = item.discours;
                    test.attitude = item.attitude;
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
                    return PartialView("ArchiveBPPTablePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("ArchiveBPPEvaluationsQualite");
        }

        public ActionResult GetArchiveBPPEvaluationsByDate(string username, string dateDebut, string dateFin)
        {
            List<EvaluationVecteurPlusViewModel> a = new List<EvaluationVecteurPlusViewModel>();
            try
            {
                Employee emp = serviceEmployee.getByLogin(username);
                DateTime daterecherchedebut = DateTime.Parse(dateDebut);
                DateTime daterecherchefin = DateTime.Parse(dateFin);
                var historstions = BPPservice.GetEvaluationsSenderBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationVecteurPlusViewModel test = new EvaluationVecteurPlusViewModel();
                    test.Id = item.Id;
                    test.presentationIdentification = item.presentationIdentification;
                    test.objetAppel = item.objetAppel;
                    test.validationNature = item.validationNature;
                    test.calendrier = item.calendrier;
                    test.envergure = item.envergure;
                    test.complétudeQuestions = item.complétudeQuestions;
                    test.confirmationIntervenants = item.confirmationIntervenants;
                    test.enchainementQuestions = item.enchainementQuestions;
                    test.traitementObjections = item.traitementObjections;
                    test.reformulation = item.reformulation;
                    test.transcriptionsInformations = item.transcriptionsInformations;
                    test.codesAppropriés = item.codesAppropriés;
                    test.noteClient = item.noteClient;
                    test.discours = item.discours;
                    test.attitude = item.attitude;
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
                    return PartialView("ArchiveBPPTablePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("ArchiveBPPEvaluationsQualite");
        }

        #endregion

        #region Archive Vecteur Plus Agent Qualité
        [Authorize(Roles = "Agent Qualité")]
        public ActionResult ArchiveQREvaluationsAgentQualite()
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
        public ActionResult GetArchiveQREvaluationsAgentQualite()
        {
            List<EvaluationVecteurPlusViewModel> a = new List<EvaluationVecteurPlusViewModel>();
            try
            {
                Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

                var historstions = service.GetEvaluationsBySenderId(emp.Id);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationVecteurPlusViewModel test = new EvaluationVecteurPlusViewModel();
                    test.Id = item.Id;
                    test.presentationIdentification = item.presentationIdentification;
                    test.objetAppel = item.objetAppel;
                    test.mutualisation = item.mutualisation;
                    test.coordonneesAttr = item.coordonneesAttr;
                    test.marcheRenouvlable = item.marcheRenouvlable;
                    test.dureeMarche = item.dureeMarche;
                    test.montantMarche = item.montantMarche;
                    test.responsableMarche = item.responsableMarche;
                    test.mailContact = item.mailContact;
                    test.traitementObjections = item.traitementObjections;
                    test.transcriptionsInformations = item.transcriptionsInformations;
                    test.envoiMail = item.envoiMail;
                    test.discours = item.discours;
                    test.attitude = item.attitude;
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
                    return PartialView("ArchiveQRTablePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("ArchiveQREvaluationsAgentQualite");
        }

        public ActionResult GetArchiveQREvaluationsAgentQualiteByDate(string dateDebut, string dateFin)
        {
            List<EvaluationVecteurPlusViewModel> a = new List<EvaluationVecteurPlusViewModel>();
            try
            {
                Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
                DateTime daterecherchedebut = DateTime.Parse(dateDebut);
                DateTime daterecherchefin = DateTime.Parse(dateFin);
                var historstions = service.GetEvaluationsSenderBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationVecteurPlusViewModel test = new EvaluationVecteurPlusViewModel();
                    test.Id = item.Id;
                    test.presentationIdentification = item.presentationIdentification;
                    test.objetAppel = item.objetAppel;
                    test.mutualisation = item.mutualisation;
                    test.coordonneesAttr = item.coordonneesAttr;
                    test.marcheRenouvlable = item.marcheRenouvlable;
                    test.dureeMarche = item.dureeMarche;
                    test.montantMarche = item.montantMarche;
                    test.responsableMarche = item.responsableMarche;
                    test.mailContact = item.mailContact;
                    test.traitementObjections = item.traitementObjections;
                    test.transcriptionsInformations = item.transcriptionsInformations;
                    test.envoiMail = item.envoiMail;
                    test.discours = item.discours;
                    test.attitude = item.attitude;
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
                    return PartialView("ArchiveQRTablePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("ArchiveQREvaluationsAgentQualite");
        }
        //Archive KLMO par Agent Qualité
        public ActionResult ArchiveKLMOEvaluationsAgentQualite()
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
        public ActionResult GetArchiveKLMOEvaluationsAgentQualite()
        {
            List<EvaluationVecteurPlusViewModel> a = new List<EvaluationVecteurPlusViewModel>();
            try
            {
                Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

                var historstions = KLMOservice.GetEvaluationsBySenderId(emp.Id);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationVecteurPlusViewModel test = new EvaluationVecteurPlusViewModel();
                    test.Id = item.Id;
                    test.presentationIdentification = item.presentationIdentification;
                    test.objetAppel = item.objetAppel;
                    test.mutualisation = item.mutualisation;
                    test.maitriseOeuvre = item.maitriseOeuvre;
                    test.coordonneesAttr = item.coordonneesAttr;
                    test.avisAttr = item.avisAttr;
                    test.traitementObjections = item.traitementObjections;
                    test.transcriptionsInformations = item.transcriptionsInformations;
                    test.envoiMail = item.envoiMail;
                    test.discours = item.discours;
                    test.attitude = item.attitude;
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
                    return PartialView("ArchiveKLMOTablePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("ArchiveKLMOEvaluationsAgentQualite");
        }

        public ActionResult GetArchiveKLMOEvaluationsAgentQualiteByDate(string dateDebut, string dateFin)
        {
            List<EvaluationVecteurPlusViewModel> a = new List<EvaluationVecteurPlusViewModel>();
            try
            {
                Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
                DateTime daterecherchedebut = DateTime.Parse(dateDebut);
                DateTime daterecherchefin = DateTime.Parse(dateFin);
                var historstions = KLMOservice.GetEvaluationsSenderBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationVecteurPlusViewModel test = new EvaluationVecteurPlusViewModel();
                    test.Id = item.Id;
                    test.presentationIdentification = item.presentationIdentification;
                    test.objetAppel = item.objetAppel;
                    test.mutualisation = item.mutualisation;
                    test.maitriseOeuvre = item.maitriseOeuvre;
                    test.coordonneesAttr = item.coordonneesAttr;
                    test.avisAttr = item.avisAttr;
                    test.traitementObjections = item.traitementObjections;
                    test.transcriptionsInformations = item.transcriptionsInformations;
                    test.envoiMail = item.envoiMail;
                    test.discours = item.discours;
                    test.attitude = item.attitude;
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
                    return PartialView("ArchiveKLMOTablePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("ArchiveKLMOEvaluationsAgentQualite");
        }
        //Archive BPP par Agent Qualité
        public ActionResult ArchiveBPPEvaluationsAgentQualite()
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
        public ActionResult GetArchiveBPPEvaluationsAgentQualite()
        {
            List<EvaluationVecteurPlusViewModel> a = new List<EvaluationVecteurPlusViewModel>();
            try
            {
                Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

                var historstions = BPPservice.GetEvaluationsBySenderId(emp.Id);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationVecteurPlusViewModel test = new EvaluationVecteurPlusViewModel();
                    test.presentationIdentification = item.presentationIdentification;
                    test.objetAppel = item.objetAppel;
                    test.validationNature = item.validationNature;
                    test.calendrier = item.calendrier;
                    test.envergure = item.envergure;
                    test.complétudeQuestions = item.complétudeQuestions;
                    test.confirmationIntervenants = item.confirmationIntervenants;
                    test.enchainementQuestions = item.enchainementQuestions;
                    test.traitementObjections = item.traitementObjections;
                    test.reformulation = item.reformulation;
                    test.transcriptionsInformations = item.transcriptionsInformations;
                    test.codesAppropriés = item.codesAppropriés;
                    test.noteClient = item.noteClient;
                    test.discours = item.discours;
                    test.attitude = item.attitude;
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
                    return PartialView("ArchiveBPPTablePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("ArchiveBPPEvaluationsAgentQualite");
        }

        public ActionResult GetArchiveBPPEvaluationsAgentQualiteByDate(string dateDebut, string dateFin)
        {
            List<EvaluationVecteurPlusViewModel> a = new List<EvaluationVecteurPlusViewModel>();
            try
            {
                Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
                DateTime daterecherchedebut = DateTime.Parse(dateDebut);
                DateTime daterecherchefin = DateTime.Parse(dateFin);
                var historstions = BPPservice.GetEvaluationsSenderBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationVecteurPlusViewModel test = new EvaluationVecteurPlusViewModel();
                    test.presentationIdentification = item.presentationIdentification;
                    test.objetAppel = item.objetAppel;
                    test.validationNature = item.validationNature;
                    test.calendrier = item.calendrier;
                    test.envergure = item.envergure;
                    test.complétudeQuestions = item.complétudeQuestions;
                    test.confirmationIntervenants = item.confirmationIntervenants;
                    test.enchainementQuestions = item.enchainementQuestions;
                    test.traitementObjections = item.traitementObjections;
                    test.reformulation = item.reformulation;
                    test.transcriptionsInformations = item.transcriptionsInformations;
                    test.codesAppropriés = item.codesAppropriés;
                    test.noteClient = item.noteClient;
                    test.discours = item.discours;
                    test.attitude = item.attitude;
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
                    return PartialView("ArchiveBPPTablePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("ArchiveTypologieEvaluationsAgentQualite");
        }
        #endregion

        #region historiqueAgent BPP
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
        
            return View(evaluation);
        }
        public ActionResult GetHistoAgent()
        {
            List<EvaluationVecteurPlusViewModel> a = new List<EvaluationVecteurPlusViewModel>();
            try
            {
                Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

                var historstions = BPPservice.GetEvaluationsByEmployee(emp.Id);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationVecteurPlusViewModel test = new EvaluationVecteurPlusViewModel();
                    test.Id = item.Id;
                    test.presentationIdentification = item.presentationIdentification;
                    test.objetAppel = item.objetAppel;
                    test.validationNature = item.validationNature;
                    test.calendrier = item.calendrier;
                    test.envergure = item.envergure;
                    test.complétudeQuestions = item.complétudeQuestions;
                    test.confirmationIntervenants = item.confirmationIntervenants;
                    test.enchainementQuestions = item.enchainementQuestions;
                    test.traitementObjections = item.traitementObjections;
                    test.reformulation = item.reformulation;
                    test.transcriptionsInformations = item.transcriptionsInformations;
                    test.codesAppropriés = item.codesAppropriés;
                    test.noteClient = item.noteClient;
                    test.discours = item.discours;
                    test.attitude = item.attitude;
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
                    return PartialView("HistoriqueTableBPPAgentPartialView", a);
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
            List<EvaluationVecteurPlusViewModel> a = new List<EvaluationVecteurPlusViewModel>();
            try
            {
                Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
                DateTime daterecherchedebut = DateTime.Parse(dateDebut);
                DateTime daterecherchefin = DateTime.Parse(dateFin);
                var historstions = BPPservice.GetEvaluationsEmployeeBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationVecteurPlusViewModel test = new EvaluationVecteurPlusViewModel();
                    test.Id = item.Id;
                    test.presentationIdentification = item.presentationIdentification;
                    test.objetAppel = item.objetAppel;
                    test.validationNature = item.validationNature;
                    test.calendrier = item.calendrier;
                    test.envergure = item.envergure;
                    test.complétudeQuestions = item.complétudeQuestions;
                    test.confirmationIntervenants = item.confirmationIntervenants;
                    test.enchainementQuestions = item.enchainementQuestions;
                    test.traitementObjections = item.traitementObjections;
                    test.reformulation = item.reformulation;
                    test.transcriptionsInformations = item.transcriptionsInformations;
                    test.codesAppropriés = item.codesAppropriés;
                    test.noteClient = item.noteClient;
                    test.discours = item.discours;
                    test.attitude = item.attitude;
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
                    return PartialView("HistoriqueTableBPPAgentPartialView", a);
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
            GrilleEvaluationBPP evaluation = db.GrilleEvaluationBPPs.Find(id);
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
        [HttpPost, ActionName("ReponseEvaluation")]
        [Authorize(Roles = "Agent")]
        public ActionResult ReponseEvaluation(int? id, GrilleEvaluationBPP evaluation)
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

        #region historiqueAgent QR
        [Authorize(Roles = "Agent_QR")]
        public ActionResult HistoriqueAgent_QR()
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
        public ActionResult GetHistoAgent_QR()
        {
            List<EvaluationVecteurPlusViewModel> a = new List<EvaluationVecteurPlusViewModel>();
            try
            {
                Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

                var historstions = service.GetEvaluationsByEmployee(emp.Id);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationVecteurPlusViewModel test = new EvaluationVecteurPlusViewModel();
                    test.Id = item.Id;
                    test.presentationIdentification = item.presentationIdentification;
                    test.objetAppel = item.objetAppel;
                    test.mutualisation = item.mutualisation;
                    test.coordonneesAttr = item.coordonneesAttr;
                    test.marcheRenouvlable = item.marcheRenouvlable;
                    test.dureeMarche = item.dureeMarche;
                    test.montantMarche = item.montantMarche;
                    test.responsableMarche = item.responsableMarche;
                    test.mailContact = item.mailContact;
                    test.traitementObjections = item.traitementObjections;
                    test.transcriptionsInformations = item.transcriptionsInformations;
                    test.envoiMail = item.envoiMail;
                    test.discours = item.discours;
                    test.attitude = item.attitude;
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
                    return PartialView("HistoriqueTableQRAgentPartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("HistoriqueAgent_QR");
        }

        public ActionResult GetHistoAgent_QRByDate(string dateDebut, string dateFin)
        {
            List<EvaluationVecteurPlusViewModel> a = new List<EvaluationVecteurPlusViewModel>();
            try
            {
                Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
                DateTime daterecherchedebut = DateTime.Parse(dateDebut);
                DateTime daterecherchefin = DateTime.Parse(dateFin);
                var historstions = service.GetEvaluationsEmployeeBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationVecteurPlusViewModel test = new EvaluationVecteurPlusViewModel();
                    test.Id = item.Id;
                    test.presentationIdentification = item.presentationIdentification;
                    test.objetAppel = item.objetAppel;
                    test.mutualisation = item.mutualisation;
                    test.coordonneesAttr = item.coordonneesAttr;
                    test.marcheRenouvlable = item.marcheRenouvlable;
                    test.dureeMarche = item.dureeMarche;
                    test.montantMarche = item.montantMarche;
                    test.responsableMarche = item.responsableMarche;
                    test.mailContact = item.mailContact;
                    test.traitementObjections = item.traitementObjections;
                    test.transcriptionsInformations = item.transcriptionsInformations;
                    test.envoiMail = item.envoiMail;
                    test.discours = item.discours;
                    test.attitude = item.attitude;
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
                    return PartialView("HistoriqueTableQRAgentPartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("HistoriqueAgent_QR");
        }
        public ActionResult ReponseEvaluationQR(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GrilleEvaluationQR evaluation = db.GrilleEvaluationQRs.Find(id);
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
        [HttpPost, ActionName("ReponseEvaluationQR")]
        [Authorize(Roles = "Agent_QR")]
        public ActionResult ReponseEvaluationQR(int? id, GrilleEvaluationQR evaluation)
        {
            if (ModelState.IsValid)
            {

                db.Entry(evaluation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("HistoriqueAgent_QR");
            }
            return View(evaluation);
        }
        #endregion

        #region historiqueAgent KLMO
        [Authorize(Roles = "Agent_KLMO")]
        public ActionResult HistoriqueAgent_KLMO()
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
        public ActionResult GetHistoAgent_KLMO()
        {
            List<EvaluationVecteurPlusViewModel> a = new List<EvaluationVecteurPlusViewModel>();
            try
            {
                Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

                var historstions = KLMOservice.GetEvaluationsByEmployee(emp.Id);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationVecteurPlusViewModel test = new EvaluationVecteurPlusViewModel();
                    test.Id = item.Id;
                    test.presentationIdentification = item.presentationIdentification;
                    test.objetAppel = item.objetAppel;
                    test.mutualisation = item.mutualisation;
                    test.maitriseOeuvre = item.maitriseOeuvre;
                    test.coordonneesAttr = item.coordonneesAttr;
                    test.avisAttr = item.avisAttr;
                    test.traitementObjections = item.traitementObjections;
                    test.transcriptionsInformations = item.transcriptionsInformations;
                    test.envoiMail = item.envoiMail;
                    test.discours = item.discours;
                    test.attitude = item.attitude;
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
                    return PartialView("HistoriqueTableKLMOAgentPartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("HistoriqueAgent_KLMO");
        }

        public ActionResult GetHistoAgent_KLMOByDate(string dateDebut, string dateFin)
        {
            List<EvaluationVecteurPlusViewModel> a = new List<EvaluationVecteurPlusViewModel>();
            try
            {
                Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
                DateTime daterecherchedebut = DateTime.Parse(dateDebut);
                DateTime daterecherchefin = DateTime.Parse(dateFin);
                var historstions = KLMOservice.GetEvaluationsEmployeeBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationVecteurPlusViewModel test = new EvaluationVecteurPlusViewModel();
                    test.Id = item.Id;
                    test.presentationIdentification = item.presentationIdentification;
                    test.objetAppel = item.objetAppel;
                    test.mutualisation = item.mutualisation;
                    test.maitriseOeuvre = item.maitriseOeuvre;
                    test.coordonneesAttr = item.coordonneesAttr;
                    test.avisAttr = item.avisAttr;
                    test.traitementObjections = item.traitementObjections;
                    test.transcriptionsInformations = item.transcriptionsInformations;
                    test.envoiMail = item.envoiMail;
                    test.discours = item.discours;
                    test.attitude = item.attitude;
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
                    return PartialView("HistoriqueTableKLMOAgentPartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("HistoriqueAgent_KLMO");
        }
        public ActionResult ReponseEvaluationKLMO(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GrilleEvaluationKLMO evaluation = db.GrilleEvaluationKLMOes.Find(id);
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
        [HttpPost, ActionName("ReponseEvaluationKLMO")]
        [Authorize(Roles = "Agent_KLMO")]
        public ActionResult ReponseEvaluationKLMO(int? id, GrilleEvaluationKLMO evaluation)
        {
            if (ModelState.IsValid)
            {

                db.Entry(evaluation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("HistoriqueAgent_KLMO");
            }
            return View(evaluation);
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
            return RedirectToAction("Index");
        }
        #endregion

        #region Edit et delete BPP
        [Authorize(Roles = "Qualité")]

        public ActionResult EditBPP(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GrilleEvaluationBPP evaluation = db.GrilleEvaluationBPPs.Find(id);
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
        [HttpPost, ActionName("EditBPP")]
        [Authorize(Roles = "Qualité")]
        public ActionResult EditBPP(int? id, GrilleEvaluationBPP evaluation)
        {
            float total = 72;
            List<float> NEList = new List<float>(new float[] { evaluation.validationNature, evaluation.calendrier, evaluation.envergure, evaluation.complétudeQuestions, evaluation.confirmationIntervenants, evaluation.enchainementQuestions, evaluation.traitementObjections, evaluation.reformulation, evaluation.transcriptionsInformations, evaluation.codesAppropriés, evaluation.noteClient });
            float notes = evaluation.presentationIdentification + evaluation.objetAppel + evaluation.discours +
              evaluation.attitude + evaluation.priseConge;

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
                return RedirectToAction("BPPHistorique");
            }
            return View(evaluation);
        }
        [Authorize(Roles = "Qualité")]
        [HttpGet]
        public ActionResult FindEvaluation(int? Id)
        {
            GrilleEvaluationBPP item = BPPservice.getById(Id);

            if (Request.IsAjaxRequest())
            {
                return PartialView("_AlerteDeSuppression", item);
            }

            else
            {
                return View(item);
            }
        }
        [Authorize(Roles = "Qualité")]
        public ActionResult DeleteBPP(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GrilleEvaluationBPP evaluation = BPPservice.getById(id);
            int? empId = evaluation.employeeId;
            BPPservice.DeleteEvaluations(id, empId);
            BPPservice.SaveChange();
            return RedirectToAction("BPPHistorique");
        }

        #endregion

        #region Edit et delete QR
        [Authorize(Roles = "Qualité")]

        public ActionResult EditQR(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GrilleEvaluationQR evaluation = db.GrilleEvaluationQRs.Find(id);
            
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
        [HttpPost, ActionName("EditQR")]
        [Authorize(Roles = "Qualité")]
        public ActionResult EditQR(int? id, GrilleEvaluationQR evaluation)
        {
            float total = 67;
            List<float> NEList = new List<float>(new float[] { evaluation.mutualisation, evaluation.marcheRenouvlable,
                evaluation.dureeMarche, evaluation.coordonneesAttr,
                evaluation.montantMarche, evaluation.responsableMarche,
                evaluation.mailContact, evaluation.traitementObjections, evaluation.transcriptionsInformations,
                evaluation.envoiMail});

            float notes = evaluation.presentationIdentification + evaluation.objetAppel + evaluation.discours +
              evaluation.attitude + evaluation.priseConge;

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
                return RedirectToAction("QRHistorique");
            }
            return View(evaluation);
        }
        [Authorize(Roles = "Qualité")]
        [HttpGet]
        public ActionResult FindEvaluationQR(int? Id)
        {
            GrilleEvaluationQR item = service.getById(Id);

            if (Request.IsAjaxRequest())
            {
                return PartialView("_AlerteDeSuppressionQR", item);
            }

            else
            {
                return View(item);
            }
        }
        [Authorize(Roles = "Qualité")]
        public ActionResult DeleteQR(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GrilleEvaluationQR evaluation = service.getById(id);
            int? empId = evaluation.employeeId;
            service.DeleteEvaluations(id, empId);
            service.SaveChange();
            return RedirectToAction("QRHistorique");
        }
        #endregion

        #region Edit et delete KLMO
        [Authorize(Roles = "Qualité")]

        public ActionResult EditKLMO(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GrilleEvaluationKLMO evaluation = db.GrilleEvaluationKLMOes.Find(id);

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
        [HttpPost, ActionName("EditKLMO")]
        [Authorize(Roles = "Qualité")]
        public ActionResult EditKLMO(int? id, GrilleEvaluationKLMO evaluation)
        {
            float total = 52;
            List<float> NEList = new List<float>(new float[] { evaluation.mutualisation,evaluation.coordonneesAttr,
                evaluation.maitriseOeuvre, evaluation.avisAttr, evaluation.traitementObjections,
                evaluation.transcriptionsInformations, evaluation.envoiMail});

            float notes = evaluation.presentationIdentification + evaluation.objetAppel + evaluation.discours +
              evaluation.attitude + evaluation.priseConge;

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
                return RedirectToAction("KLMOHistorique");
            }
            return View(evaluation);
        }
        [Authorize(Roles = "Qualité")]
        [HttpGet]
        public ActionResult FindEvaluationKLMO(int? Id)
        {
            GrilleEvaluationKLMO item = KLMOservice.getById(Id);

            if (Request.IsAjaxRequest())
            {
                return PartialView("_AlerteDeSuppressionKLMO", item);
            }

            else
            {
                return View(item);
            }
        }
        [Authorize(Roles = "Qualité")]
        public ActionResult DeleteKLMO(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GrilleEvaluationKLMO evaluation = KLMOservice.getById(id);
            int? empId = evaluation.employeeId;
            KLMOservice.DeleteEvaluations(id, empId);
            KLMOservice.SaveChange();
            return RedirectToAction("KLMOHistorique");
        }
        #endregion

        #region Evaluation Battonage Vecteur Plus
        public ActionResult BattonageVecteurPlus(string enregistrementFullName, string enregistrementUrl, string enregistrementDirectory, string agentName)
        {

            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 5))
            {
                ViewBag.role = "Agent Qualité";
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
        public ActionResult BattonageVecteurPlusWithoutAgent(string enregistrementFullName, string enregistrementUrl, string enregistrementDirectory, string siteName)
        {
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            var a = new EvaluationViewModel();
            List<SelectListItem> agents = new List<SelectListItem>();
            List<Employee> logins = new List<Employee>();
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 5))
            {
                ViewBag.role = "Agent Qualité";
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
                string d = logins[j].pseudoName.Replace(" ", "-").Replace("é", "Ã©"); ;
                string dd = logins[j].pseudoName.Replace(" ", "--").Replace("é", "Ã©");
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

        public ActionResult CalculBattonageVecteurPlus(string nomAgent, string planDate, string type, string nature, string typologie, string avancement, string destination, string codeRP,string nomProgramme,string postIT, string objects, string notreClients, string moa, string moaArchitectes, string bureauxEtudes, string bailleurSociaux,string mairie, string dateAccords, string datePC, string dateConsultation, string dateTravaux, string dateLivraison, string surfacePlancher, string surfaceTerrain, string surfaceTypologie, string nombreBatiment, string nombreEtage, string ascenseur, string nombreAscenseur, string lieuExecution, string commentaire, string enregistrement, string enregistrementUrl, string enregistrementDirectory,
           string natureRs, string typologieRs, string avancementRs, string destinationRs, string codeRPRs, string nomProgrammeRs, string postITRs, string objectsRs, string notreClientsRs, string moaRs, string moaArchitectesRs, string bureauxEtudesRs, string bailleurSociauxRs, string mairieRs, string dateAccordsRs, string datePCRs, string dateConsultationRs, string dateTravauxRs, string dateLivraisonRs, string surfacePlancherRs, string surfaceTerrainRs, string surfaceTypologieRs, string nombreBatimentRs, string nombreEtageRs, string ascenseurRs, string nombreAscenseurRs, string lieuExecutionRs)
        {
            float total = 27;
            float totalRs = 27;
            List<string> NEList = new List<string>(new string[] { nature, typologie, avancement, destination, codeRP, nomProgramme, postIT, objects, notreClients, moa, moaArchitectes, bureauxEtudes, bailleurSociaux, mairie, dateAccords, datePC, dateConsultation, dateTravaux, dateLivraison, surfacePlancher, surfaceTerrain, surfaceTypologie, nombreBatiment, nombreEtage, ascenseur, nombreAscenseur, lieuExecution });
            List<string> NEListRs = new List<string>(new string[] { natureRs, typologieRs, avancementRs, destinationRs, codeRPRs, nomProgrammeRs, postITRs, objectsRs, notreClientsRs, moaRs, moaArchitectesRs, bureauxEtudesRs, bailleurSociauxRs, mairieRs, dateAccordsRs, datePCRs, dateConsultationRs, dateTravauxRs, dateLivraisonRs, surfacePlancherRs, surfaceTerrainRs, surfaceTypologieRs, nombreBatimentRs, nombreEtageRs, ascenseurRs, nombreAscenseurRs, lieuExecutionRs });

            float notes = 0;
            float notesRs = 0;
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
            foreach (var i in NEListRs)
            {
                if (float.Parse(i, CultureInfo.InvariantCulture.NumberFormat) < 0)
                {
                    totalRs += float.Parse(i, CultureInfo.InvariantCulture.NumberFormat);
                }
                else
                {                    
                    notesRs += float.Parse(i, CultureInfo.InvariantCulture.NumberFormat);
                }
            }
            var a = new EvaluationVecteurPlusViewModel();
            a.type = type;
            a.nature = float.Parse(nature, CultureInfo.InvariantCulture.NumberFormat);
            a.typologie = float.Parse(typologie, CultureInfo.InvariantCulture.NumberFormat);
            a.avancement = float.Parse(avancement, CultureInfo.InvariantCulture.NumberFormat);
            a.destination = float.Parse(destination, CultureInfo.InvariantCulture.NumberFormat);
            a.codeRP = float.Parse(codeRP, CultureInfo.InvariantCulture.NumberFormat);
            a.nomProgramme = float.Parse(nomProgramme, CultureInfo.InvariantCulture.NumberFormat);
            a.postIT = float.Parse(postIT, CultureInfo.InvariantCulture.NumberFormat);
            a.objects = float.Parse(objects, CultureInfo.InvariantCulture.NumberFormat);
            a.notreClients = float.Parse(notreClients, CultureInfo.InvariantCulture.NumberFormat);
            a.moa = float.Parse(moa, CultureInfo.InvariantCulture.NumberFormat);
            a.moaArchitectes = float.Parse(moaArchitectes, CultureInfo.InvariantCulture.NumberFormat);
            a.bureauxEtudes = float.Parse(bureauxEtudes, CultureInfo.InvariantCulture.NumberFormat);
            a.bailleurSociaux = float.Parse(bailleurSociaux, CultureInfo.InvariantCulture.NumberFormat);
            a.mairie = float.Parse(mairie, CultureInfo.InvariantCulture.NumberFormat);
            a.dateAccords = float.Parse(dateAccords, CultureInfo.InvariantCulture.NumberFormat);
            a.datePC = float.Parse(datePC, CultureInfo.InvariantCulture.NumberFormat);
            a.dateConsultation = float.Parse(dateConsultation, CultureInfo.InvariantCulture.NumberFormat);
            a.dateTravaux = float.Parse(dateTravaux, CultureInfo.InvariantCulture.NumberFormat);
            a.dateLivraison = float.Parse(dateLivraison, CultureInfo.InvariantCulture.NumberFormat);
            a.surfacePlancher = float.Parse(surfacePlancher, CultureInfo.InvariantCulture.NumberFormat);
            a.surfaceTerrain = float.Parse(surfaceTerrain, CultureInfo.InvariantCulture.NumberFormat);
            a.surfaceTypologie = float.Parse(surfaceTypologie, CultureInfo.InvariantCulture.NumberFormat);
            a.nombreBatiment = float.Parse(nombreBatiment, CultureInfo.InvariantCulture.NumberFormat);
            a.nombreEtage = float.Parse(nombreEtage, CultureInfo.InvariantCulture.NumberFormat);
            a.ascenseur = float.Parse(ascenseur, CultureInfo.InvariantCulture.NumberFormat);
            a.nombreAscenseur = float.Parse(nombreAscenseur, CultureInfo.InvariantCulture.NumberFormat);
            a.lieuExecution = float.Parse(lieuExecution, CultureInfo.InvariantCulture.NumberFormat);
            //
            a.natureRs = float.Parse(natureRs, CultureInfo.InvariantCulture.NumberFormat);
            a.typologieRs = float.Parse(typologieRs, CultureInfo.InvariantCulture.NumberFormat);
            a.avancementRs = float.Parse(avancementRs, CultureInfo.InvariantCulture.NumberFormat);
            a.destinationRs = float.Parse(destinationRs, CultureInfo.InvariantCulture.NumberFormat);
            a.codeRPRs = float.Parse(codeRPRs, CultureInfo.InvariantCulture.NumberFormat);
            a.nomProgrammeRs = float.Parse(nomProgrammeRs, CultureInfo.InvariantCulture.NumberFormat);
            a.postITRs = float.Parse(postITRs, CultureInfo.InvariantCulture.NumberFormat);
            a.objectsRs = float.Parse(objectsRs, CultureInfo.InvariantCulture.NumberFormat);
            a.notreClientsRs = float.Parse(notreClientsRs, CultureInfo.InvariantCulture.NumberFormat);
            a.moaRs = float.Parse(moaRs, CultureInfo.InvariantCulture.NumberFormat);
            a.moaArchitectesRs = float.Parse(moaArchitectesRs, CultureInfo.InvariantCulture.NumberFormat);
            a.bureauxEtudesRs = float.Parse(bureauxEtudesRs, CultureInfo.InvariantCulture.NumberFormat);
            a.bailleurSociauxRs = float.Parse(bailleurSociauxRs, CultureInfo.InvariantCulture.NumberFormat);
            a.mairieRs = float.Parse(mairieRs, CultureInfo.InvariantCulture.NumberFormat);
            a.dateAccordsRs = float.Parse(dateAccordsRs, CultureInfo.InvariantCulture.NumberFormat);
            a.datePCRs = float.Parse(datePCRs, CultureInfo.InvariantCulture.NumberFormat);
            a.dateConsultationRs = float.Parse(dateConsultationRs, CultureInfo.InvariantCulture.NumberFormat);
            a.dateTravauxRs = float.Parse(dateTravauxRs, CultureInfo.InvariantCulture.NumberFormat);
            a.dateLivraisonRs = float.Parse(dateLivraisonRs, CultureInfo.InvariantCulture.NumberFormat);
            a.surfacePlancherRs = float.Parse(surfacePlancherRs, CultureInfo.InvariantCulture.NumberFormat);
            a.surfaceTerrainRs = float.Parse(surfaceTerrainRs, CultureInfo.InvariantCulture.NumberFormat);
            a.surfaceTypologieRs = float.Parse(surfaceTypologieRs, CultureInfo.InvariantCulture.NumberFormat);
            a.nombreBatimentRs = float.Parse(nombreBatimentRs, CultureInfo.InvariantCulture.NumberFormat);
            a.nombreEtageRs = float.Parse(nombreEtageRs, CultureInfo.InvariantCulture.NumberFormat);
            a.ascenseurRs = float.Parse(ascenseurRs, CultureInfo.InvariantCulture.NumberFormat);
            a.nombreAscenseurRs = float.Parse(nombreAscenseurRs, CultureInfo.InvariantCulture.NumberFormat);
            a.lieuExecutionRs = float.Parse(lieuExecutionRs, CultureInfo.InvariantCulture.NumberFormat);

            a.dateTempEvaluation = DateTime.Parse(planDate);
            a.agentName = nomAgent;
            a.note = notes;
            a.noteRs = notesRs;
            a.pourcentageNotes = (notes / total) * 100;
            a.pourcentageNotesRs = (notesRs / totalRs) * 100;

            if (Request.IsAjaxRequest())
            {
                return PartialView("BattonageResultPartialView", a);
            }
            return RedirectToAction("Acceuil", "Directory");
        }
        public ActionResult SaveEvalBattonageVecteurPlus(string nomAgent, string planDate, string type, string nature, string typologie, string avancement, string destination, string codeRP, string nomProgramme, string postIT, string objects, string notreClients, string moa, string moaArchitectes, string bureauxEtudes, string bailleurSociaux,string mairie, string dateAccords, string datePC, string dateConsultation, string dateTravaux, string dateLivraison, string surfacePlancher, string surfaceTerrain, string surfaceTypologie, string nombreBatiment, string nombreEtage, string ascenseur, string nombreAscenseur, string lieuExecution, string commentaire, string enregistrement, string enregistrementUrl, string enregistrementDirectory,
            string natureRs, string typologieRs, string avancementRs, string destinationRs, string codeRPRs, string nomProgrammeRs, string postITRs, string objectsRs, string notreClientsRs, string moaRs, string moaArchitectesRs, string bureauxEtudesRs, string bailleurSociauxRs, string mairieRs, string dateAccordsRs, string datePCRs, string dateConsultationRs, string dateTravauxRs, string dateLivraisonRs, string surfacePlancherRs, string surfaceTerrainRs, string surfaceTypologieRs, string nombreBatimentRs, string nombreEtageRs, string ascenseurRs, string nombreAscenseurRs, string lieuExecutionRs)
        {
            var userConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            Employee emp = serviceEmployee.getByPseudoName(nomAgent.ToLower());
            GrilleEvaluationBattonage a = new GrilleEvaluationBattonage();
            float total = 27;
            float totalRs = 27;
            List<string> NEList = new List<string>(new string[] { nature, typologie, avancement, destination, codeRP, nomProgramme, postIT, objects, notreClients, moa, moaArchitectes, bureauxEtudes, bailleurSociaux, mairie, dateAccords, datePC, dateConsultation, dateTravaux, dateLivraison, surfacePlancher, surfaceTerrain, surfaceTypologie, nombreBatiment, nombreEtage, ascenseur, nombreAscenseur, lieuExecution });
            List<string> NEListRs = new List<string>(new string[] { natureRs, typologieRs, avancementRs, destinationRs, codeRPRs, nomProgrammeRs, postITRs, objectsRs, notreClientsRs, moaRs, moaArchitectesRs, bureauxEtudesRs, bailleurSociauxRs, mairieRs, dateAccordsRs, datePCRs, dateConsultationRs, dateTravauxRs, dateLivraisonRs, surfacePlancherRs, surfaceTerrainRs, surfaceTypologieRs, nombreBatimentRs, nombreEtageRs, ascenseurRs, nombreAscenseurRs, lieuExecutionRs });
            float notes = 0;
            float notesRs = 0;
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
            foreach (var i in NEListRs)
            {
                if (float.Parse(i, CultureInfo.InvariantCulture.NumberFormat) < 0)
                {
                    totalRs += float.Parse(i, CultureInfo.InvariantCulture.NumberFormat);
                }
                else
                {
                    notesRs += float.Parse(i, CultureInfo.InvariantCulture.NumberFormat);
                }
            }
            a.type = type;
            a.nature = float.Parse(nature, CultureInfo.InvariantCulture.NumberFormat);
            a.typologie = float.Parse(typologie, CultureInfo.InvariantCulture.NumberFormat);
            a.avancement = float.Parse(avancement, CultureInfo.InvariantCulture.NumberFormat);
            a.destination = float.Parse(destination, CultureInfo.InvariantCulture.NumberFormat);
            a.codeRP = float.Parse(codeRP, CultureInfo.InvariantCulture.NumberFormat);
            a.nomProgramme = float.Parse(nomProgramme, CultureInfo.InvariantCulture.NumberFormat);
            a.postIT = float.Parse(postIT, CultureInfo.InvariantCulture.NumberFormat);
            a.objects = float.Parse(objects, CultureInfo.InvariantCulture.NumberFormat);
            a.notreClients = float.Parse(notreClients, CultureInfo.InvariantCulture.NumberFormat);
            a.moa = float.Parse(moa, CultureInfo.InvariantCulture.NumberFormat);
            a.moaArchitectes = float.Parse(moaArchitectes, CultureInfo.InvariantCulture.NumberFormat);
            a.bureauxEtudes = float.Parse(bureauxEtudes, CultureInfo.InvariantCulture.NumberFormat);
            a.bailleurSociaux = float.Parse(bailleurSociaux, CultureInfo.InvariantCulture.NumberFormat);
            a.mairie = float.Parse(mairie, CultureInfo.InvariantCulture.NumberFormat);
            a.dateAccords = float.Parse(dateAccords, CultureInfo.InvariantCulture.NumberFormat);
            a.datePC = float.Parse(datePC, CultureInfo.InvariantCulture.NumberFormat);
            a.dateConsultation = float.Parse(dateConsultation, CultureInfo.InvariantCulture.NumberFormat);
            a.dateTravaux = float.Parse(dateTravaux, CultureInfo.InvariantCulture.NumberFormat);
            a.dateLivraison = float.Parse(dateLivraison, CultureInfo.InvariantCulture.NumberFormat);
            a.surfacePlancher = float.Parse(surfacePlancher, CultureInfo.InvariantCulture.NumberFormat);
            a.surfaceTerrain = float.Parse(surfaceTerrain, CultureInfo.InvariantCulture.NumberFormat);
            a.surfaceTypologie = float.Parse(surfaceTypologie, CultureInfo.InvariantCulture.NumberFormat);
            a.nombreBatiment = float.Parse(nombreBatiment, CultureInfo.InvariantCulture.NumberFormat);
            a.nombreEtage = float.Parse(nombreEtage, CultureInfo.InvariantCulture.NumberFormat);
            a.ascenseur = float.Parse(ascenseur, CultureInfo.InvariantCulture.NumberFormat);
            a.nombreAscenseur = float.Parse(nombreAscenseur, CultureInfo.InvariantCulture.NumberFormat);
            a.lieuExecution = float.Parse(lieuExecution, CultureInfo.InvariantCulture.NumberFormat);

            a.natureRs = float.Parse(natureRs, CultureInfo.InvariantCulture.NumberFormat);
            a.typologieRs = float.Parse(typologieRs, CultureInfo.InvariantCulture.NumberFormat);
            a.avancementRs = float.Parse(avancementRs, CultureInfo.InvariantCulture.NumberFormat);
            a.destinationRs = float.Parse(destinationRs, CultureInfo.InvariantCulture.NumberFormat);
            a.codeRPRs = float.Parse(codeRPRs, CultureInfo.InvariantCulture.NumberFormat);
            a.nomProgrammeRs = float.Parse(nomProgrammeRs, CultureInfo.InvariantCulture.NumberFormat);
            a.postITRs = float.Parse(postITRs, CultureInfo.InvariantCulture.NumberFormat);
            a.objectsRs = float.Parse(objectsRs, CultureInfo.InvariantCulture.NumberFormat);
            a.notreClientsRs = float.Parse(notreClientsRs, CultureInfo.InvariantCulture.NumberFormat);
            a.moaRs = float.Parse(moaRs, CultureInfo.InvariantCulture.NumberFormat);
            a.moaArchitectesRs = float.Parse(moaArchitectesRs, CultureInfo.InvariantCulture.NumberFormat);
            a.bureauxEtudesRs = float.Parse(bureauxEtudesRs, CultureInfo.InvariantCulture.NumberFormat);
            a.bailleurSociauxRs = float.Parse(bailleurSociauxRs, CultureInfo.InvariantCulture.NumberFormat);
            a.mairieRs = float.Parse(mairieRs, CultureInfo.InvariantCulture.NumberFormat);
            a.dateAccordsRs = float.Parse(dateAccordsRs, CultureInfo.InvariantCulture.NumberFormat);
            a.datePCRs = float.Parse(datePCRs, CultureInfo.InvariantCulture.NumberFormat);
            a.dateConsultationRs = float.Parse(dateConsultationRs, CultureInfo.InvariantCulture.NumberFormat);
            a.dateTravauxRs = float.Parse(dateTravauxRs, CultureInfo.InvariantCulture.NumberFormat);
            a.dateLivraisonRs = float.Parse(dateLivraisonRs, CultureInfo.InvariantCulture.NumberFormat);
            a.surfacePlancherRs = float.Parse(surfacePlancherRs, CultureInfo.InvariantCulture.NumberFormat);
            a.surfaceTerrainRs = float.Parse(surfaceTerrainRs, CultureInfo.InvariantCulture.NumberFormat);
            a.surfaceTypologieRs = float.Parse(surfaceTypologieRs, CultureInfo.InvariantCulture.NumberFormat);
            a.nombreBatimentRs = float.Parse(nombreBatimentRs, CultureInfo.InvariantCulture.NumberFormat);
            a.nombreEtageRs = float.Parse(nombreEtageRs, CultureInfo.InvariantCulture.NumberFormat);
            a.ascenseurRs = float.Parse(ascenseurRs, CultureInfo.InvariantCulture.NumberFormat);
            a.nombreAscenseurRs = float.Parse(nombreAscenseurRs, CultureInfo.InvariantCulture.NumberFormat);
            a.lieuExecutionRs = float.Parse(lieuExecutionRs, CultureInfo.InvariantCulture.NumberFormat);

            a.dateTempEvaluation = DateTime.Parse(planDate);
            a.employeeId = emp.Id;
            a.senderId = userConnected.Id;
            a.note = (notes / total) * 100;
            a.noteRs = (notesRs / totalRs) * 100;
            a.commentaireQualite = commentaire;
            a.enregistrementFullName = enregistrement;
            a.enregistrementUrl = enregistrementUrl;
            a.enregistrementDirectory = enregistrementDirectory;

            Battonageservice.Add(a);
            Battonageservice.SaveChange();
            var eval = new EvaluationVecteurPlusViewModel();
            eval.agentName = nomAgent;
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
                return PartialView("EnvoiMailPartialView", eval);
            }
            return RedirectToAction("Acceuil", "Directory");
        }
        
        //Battonage Historique
        public ActionResult BattonageHistorique()
        {
            List<Employee> logins = new List<Employee>();
            EvaluationViewModel evaluation = new EvaluationViewModel();
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 5))
            {
                ViewBag.role = "Agent Qualité";
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


            string d = "COMONLINE";
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
        public ActionResult GetHistoBattonageVecteurPlus(string username)
        {
            List<EvaluationVecteurPlusViewModel> a = new List<EvaluationVecteurPlusViewModel>();
            try
            {
                Employee emp = serviceEmployee.getByLogin(username);
                var historstionsBPP = Battonageservice.GetEvaluationsByEmployee(emp.Id);
                ViewBag.nbreEvaluations = historstionsBPP.Count();
                foreach (var item in historstionsBPP)
                {
                    EvaluationVecteurPlusViewModel test = new EvaluationVecteurPlusViewModel();
                    test.Id = item.Id;
                    test.nature = item.nature;
                    test.typologie = item.typologie;
                    test.avancement = item.avancement;
                    test.destination = item.destination;
                    test.codeRP = item.codeRP;
                    test.nomProgramme = item.nomProgramme;
                    test.postIT = item.postIT;
                    test.objects = item.objects;
                    test.notreClients = item.notreClients;
                    test.moa = item.moa;
                    test.moaArchitectes = item.moaArchitectes;
                    test.bureauxEtudes = item.bureauxEtudes;
                    test.bailleurSociaux = item.bailleurSociaux;
                    test.mairie = item.mairie;
                    test.dateAccords = item.dateAccords;
                    test.datePC = item.datePC;
                    test.dateConsultation = item.dateConsultation;
                    test.dateTravaux = item.dateTravaux;
                    test.dateLivraison = item.dateLivraison;
                    test.surfacePlancher = item.surfacePlancher;
                    test.surfaceTerrain = item.surfaceTerrain;
                    test.surfaceTypologie = item.surfaceTypologie;
                    test.nombreBatiment = item.nombreBatiment;
                    test.nombreEtage = item.nombreEtage;
                    test.ascenseur = item.ascenseur;
                    test.nombreAscenseur = item.nombreAscenseur;
                    test.lieuExecution = item.lieuExecution;

                    //
                    test.natureRs = item.natureRs;
                    test.typologieRs = item.typologieRs;
                    test.avancementRs = item.avancementRs;
                    test.destinationRs = item.destinationRs;
                    test.codeRPRs = item.codeRPRs;
                    test.nomProgrammeRs = item.nomProgrammeRs;
                    test.postITRs = item.postITRs;
                    test.objectsRs = item.objectsRs;
                    test.notreClientsRs = item.notreClientsRs;
                    test.moaRs = item.moaRs;
                    test.moaArchitectesRs = item.moaArchitectesRs;
                    test.bureauxEtudesRs = item.bureauxEtudesRs;
                    test.bailleurSociauxRs = item.bailleurSociauxRs;
                    test.mairieRs = item.mairieRs;
                    test.dateAccordsRs = item.dateAccordsRs;
                    test.datePCRs = item.datePCRs;
                    test.dateConsultationRs = item.dateConsultationRs;
                    test.dateTravauxRs = item.dateTravauxRs;
                    test.dateLivraisonRs = item.dateLivraisonRs;
                    test.surfacePlancherRs = item.surfacePlancherRs;
                    test.surfaceTerrainRs = item.surfaceTerrainRs;
                    test.surfaceTypologieRs = item.surfaceTypologieRs;
                    test.nombreBatimentRs = item.nombreBatimentRs;
                    test.nombreEtageRs = item.nombreEtageRs;
                    test.ascenseurRs = item.ascenseurRs;
                    test.nombreAscenseurRs = item.nombreAscenseurRs;
                    test.lieuExecutionRs = item.lieuExecutionRs;

                    test.dateTempEvaluation = item.dateTempEvaluation;
                    test.type = item.type;
                    test.note = item.note;
                    test.noteRs = item.noteRs;
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
                    return PartialView("HistoriqueBattonageTablePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("BattonageHistorique");
        }
        public ActionResult GetHistoBattonageByDate(string username, string dateDebut, string dateFin)
        {
            List<EvaluationVecteurPlusViewModel> a = new List<EvaluationVecteurPlusViewModel>();
            try
            {
                Employee emp = serviceEmployee.getByLogin(username);
                DateTime daterecherchedebut = DateTime.Parse(dateDebut);
                DateTime daterecherchefin = DateTime.Parse(dateFin);

                var historstions = Battonageservice.GetEvaluationsEmployeeBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationVecteurPlusViewModel test = new EvaluationVecteurPlusViewModel();
                    test.Id = item.Id;
                    test.type = item.type;

                    test.nature = item.nature;
                    test.typologie = item.typologie;
                    test.avancement = item.avancement;
                    test.destination = item.destination;
                    test.codeRP = item.codeRP;
                    test.nomProgramme = item.nomProgramme;
                    test.postIT = item.postIT;
                    test.objects = item.objects;
                    test.notreClients = item.notreClients;
                    test.moa = item.moa;
                    test.moaArchitectes = item.moaArchitectes;
                    test.bureauxEtudes = item.bureauxEtudes;
                    test.bailleurSociaux = item.bailleurSociaux;
                    test.mairie = item.mairie;
                    test.dateAccords = item.dateAccords;
                    test.datePC = item.datePC;
                    test.dateConsultation = item.dateConsultation;
                    test.dateTravaux = item.dateTravaux;
                    test.dateLivraison = item.dateLivraison;
                    test.surfacePlancher = item.surfacePlancher;
                    test.surfaceTerrain = item.surfaceTerrain;
                    test.surfaceTypologie = item.surfaceTypologie;
                    test.nombreBatiment = item.nombreBatiment;
                    test.nombreEtage = item.nombreEtage;
                    test.ascenseur = item.ascenseur;
                    test.nombreAscenseur = item.nombreAscenseur;
                    test.lieuExecution = item.lieuExecution;

                    //
                    test.natureRs = item.natureRs;
                    test.typologieRs = item.typologieRs;
                    test.avancementRs = item.avancementRs;
                    test.destinationRs = item.destinationRs;
                    test.codeRPRs = item.codeRPRs;
                    test.nomProgrammeRs = item.nomProgrammeRs;
                    test.postITRs = item.postITRs;
                    test.objectsRs = item.objectsRs;
                    test.notreClientsRs = item.notreClientsRs;
                    test.moaRs = item.moaRs;
                    test.moaArchitectesRs = item.moaArchitectesRs;
                    test.bureauxEtudesRs = item.bureauxEtudesRs;
                    test.bailleurSociauxRs = item.bailleurSociauxRs;
                    test.mairieRs = item.mairieRs;
                    test.dateAccordsRs = item.dateAccordsRs;
                    test.datePCRs = item.datePCRs;
                    test.dateConsultationRs = item.dateConsultationRs;
                    test.dateTravauxRs = item.dateTravauxRs;
                    test.dateLivraisonRs = item.dateLivraisonRs;
                    test.surfacePlancherRs = item.surfacePlancherRs;
                    test.surfaceTerrainRs = item.surfaceTerrainRs;
                    test.surfaceTypologieRs = item.surfaceTypologieRs;
                    test.nombreBatimentRs = item.nombreBatimentRs;
                    test.nombreEtageRs = item.nombreEtageRs;
                    test.ascenseurRs = item.ascenseurRs;
                    test.nombreAscenseurRs = item.nombreAscenseurRs;
                    test.lieuExecutionRs = item.lieuExecutionRs;
                    //

                    test.note = item.note;
                    test.noteRs = item.noteRs;

                    test.dateTempEvaluation = item.dateTempEvaluation;
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
                    return PartialView("HistoriqueBattonageTablePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("BattonageHistorique");
        }

        //Battonage Archive Par Qualité
        public ActionResult ArchiveBattonageEvaluationsQualite()
        {
            EvaluationVecteurPlusViewModel evaluation = new EvaluationVecteurPlusViewModel();
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            var logins = serviceEmployee.GetAll();
            var employees = logins.Select(o => o).Distinct().ToList();

            foreach (var test in employees)
            {
                if (test.Roles.Any(r => r.UserId == test.Id && r.RoleId == 4 || r.RoleId == 5))
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

        public ActionResult GetBattonageArchiveEvaluationsQualite(string username)
        {
            List<EvaluationVecteurPlusViewModel> a = new List<EvaluationVecteurPlusViewModel>();
            try
            {
                Employee emp = serviceEmployee.getByLogin(username);
                var historstions = Battonageservice.GetEvaluationsBySenderId(emp.Id);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationVecteurPlusViewModel test = new EvaluationVecteurPlusViewModel();
                    test.Id = item.Id;

                    test.nature = item.nature;
                    test.typologie = item.typologie;
                    test.avancement = item.avancement;
                    test.destination = item.destination;
                    test.codeRP = item.codeRP;
                    test.nomProgramme = item.nomProgramme;
                    test.postIT = item.postIT;
                    test.objects = item.objects;
                    test.notreClients = item.notreClients;
                    test.moa = item.moa;
                    test.moaArchitectes = item.moaArchitectes;
                    test.bureauxEtudes = item.bureauxEtudes;
                    test.bailleurSociaux = item.bailleurSociaux;
                    test.mairie = item.mairie;
                    test.dateAccords = item.dateAccords;
                    test.datePC = item.datePC;
                    test.dateConsultation = item.dateConsultation;
                    test.dateTravaux = item.dateTravaux;
                    test.dateLivraison = item.dateLivraison;
                    test.surfacePlancher = item.surfacePlancher;
                    test.surfaceTerrain = item.surfaceTerrain;
                    test.surfaceTypologie = item.surfaceTypologie;
                    test.nombreBatiment = item.nombreBatiment;
                    test.nombreEtage = item.nombreEtage;
                    test.ascenseur = item.ascenseur;
                    test.nombreAscenseur = item.nombreAscenseur;
                    test.lieuExecution = item.lieuExecution;

                    //
                    test.natureRs = item.natureRs;
                    test.typologieRs = item.typologieRs;
                    test.avancementRs = item.avancementRs;
                    test.destinationRs = item.destinationRs;
                    test.codeRPRs = item.codeRPRs;
                    test.nomProgrammeRs = item.nomProgrammeRs;
                    test.postITRs = item.postITRs;
                    test.objectsRs = item.objectsRs;
                    test.notreClientsRs = item.notreClientsRs;
                    test.moaRs = item.moaRs;
                    test.moaArchitectesRs = item.moaArchitectesRs;
                    test.bureauxEtudesRs = item.bureauxEtudesRs;
                    test.bailleurSociauxRs = item.bailleurSociauxRs;
                    test.mairieRs = item.mairieRs;
                    test.dateAccordsRs = item.dateAccordsRs;
                    test.datePCRs = item.datePCRs;
                    test.dateConsultationRs = item.dateConsultationRs;
                    test.dateTravauxRs = item.dateTravauxRs;
                    test.dateLivraisonRs = item.dateLivraisonRs;
                    test.surfacePlancherRs = item.surfacePlancherRs;
                    test.surfaceTerrainRs = item.surfaceTerrainRs;
                    test.surfaceTypologieRs = item.surfaceTypologieRs;
                    test.nombreBatimentRs = item.nombreBatimentRs;
                    test.nombreEtageRs = item.nombreEtageRs;
                    test.ascenseurRs = item.ascenseurRs;
                    test.nombreAscenseurRs = item.nombreAscenseurRs;
                    test.lieuExecutionRs = item.lieuExecutionRs;
                    //

                    test.dateTempEvaluation = item.dateTempEvaluation;
                    test.type = item.type;
                    test.note = item.note;
                    test.noteRs = item.noteRs;

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
                    return PartialView("ArchiveBattonageTablePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("ArchiveBattonageEvaluationsQualite");
        }

        public ActionResult GetArchiveBattonageEvaluationsByDate(string username, string dateDebut, string dateFin)
        {
            List<EvaluationVecteurPlusViewModel> a = new List<EvaluationVecteurPlusViewModel>();
            try
            {
                Employee emp = serviceEmployee.getByLogin(username);
                DateTime daterecherchedebut = DateTime.Parse(dateDebut);
                DateTime daterecherchefin = DateTime.Parse(dateFin);
                var historstions = Battonageservice.GetEvaluationsSenderBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationVecteurPlusViewModel test = new EvaluationVecteurPlusViewModel();
                    test.Id = item.Id;

                    test.nature = item.nature;
                    test.typologie = item.typologie;
                    test.avancement = item.avancement;
                    test.destination = item.destination;
                    test.codeRP = item.codeRP;
                    test.nomProgramme = item.nomProgramme;
                    test.postIT = item.postIT;
                    test.objects = item.objects;
                    test.notreClients = item.notreClients;
                    test.moa = item.moa;
                    test.moaArchitectes = item.moaArchitectes;
                    test.bureauxEtudes = item.bureauxEtudes;
                    test.bailleurSociaux = item.bailleurSociaux;
                    test.mairie = item.mairie;
                    test.dateAccords = item.dateAccords;
                    test.datePC = item.datePC;
                    test.dateConsultation = item.dateConsultation;
                    test.dateTravaux = item.dateTravaux;
                    test.dateLivraison = item.dateLivraison;
                    test.surfacePlancher = item.surfacePlancher;
                    test.surfaceTerrain = item.surfaceTerrain;
                    test.surfaceTypologie = item.surfaceTypologie;
                    test.nombreBatiment = item.nombreBatiment;
                    test.nombreEtage = item.nombreEtage;
                    test.ascenseur = item.ascenseur;
                    test.nombreAscenseur = item.nombreAscenseur;
                    test.lieuExecution = item.lieuExecution;
                    //
                    test.natureRs = item.natureRs;
                    test.typologieRs = item.typologieRs;
                    test.avancementRs = item.avancementRs;
                    test.destinationRs = item.destinationRs;
                    test.codeRPRs = item.codeRPRs;
                    test.nomProgrammeRs = item.nomProgrammeRs;
                    test.postITRs = item.postITRs;
                    test.objectsRs = item.objectsRs;
                    test.notreClientsRs = item.notreClientsRs;
                    test.moaRs = item.moaRs;
                    test.moaArchitectesRs = item.moaArchitectesRs;
                    test.bureauxEtudesRs = item.bureauxEtudesRs;
                    test.bailleurSociauxRs = item.bailleurSociauxRs;
                    test.mairieRs = item.mairieRs;
                    test.dateAccordsRs = item.dateAccordsRs;
                    test.datePCRs = item.datePCRs;
                    test.dateConsultationRs = item.dateConsultationRs;
                    test.dateTravauxRs = item.dateTravauxRs;
                    test.dateLivraisonRs = item.dateLivraisonRs;
                    test.surfacePlancherRs = item.surfacePlancherRs;
                    test.surfaceTerrainRs = item.surfaceTerrainRs;
                    test.surfaceTypologieRs = item.surfaceTypologieRs;
                    test.nombreBatimentRs = item.nombreBatimentRs;
                    test.nombreEtageRs = item.nombreEtageRs;
                    test.ascenseurRs = item.ascenseurRs;
                    test.nombreAscenseurRs = item.nombreAscenseurRs;
                    test.lieuExecutionRs = item.lieuExecutionRs;
                    //
                    test.dateTempEvaluation = item.dateTempEvaluation;
                    test.type = item.type;
                    test.note = item.note;
                    test.noteRs = item.noteRs;
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
                    return PartialView("ArchiveBattonageTablePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("ArchiveBattonageEvaluationsQualite");
        }

        //Archive Battonage par Agent Qualité
        public ActionResult ArchiveBattonageEvaluationsAgentQualite()
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
        public ActionResult GetArchiveBattonageEvaluationsAgentQualite()
        {
            List<EvaluationVecteurPlusViewModel> a = new List<EvaluationVecteurPlusViewModel>();
            try
            {
                Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

                var historstions = Battonageservice.GetEvaluationsBySenderId(emp.Id);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationVecteurPlusViewModel test = new EvaluationVecteurPlusViewModel();
                    test.Id = item.Id;
                    test.nature = item.nature;
                    test.typologie = item.typologie;
                    test.avancement = item.avancement;
                    test.destination = item.destination;
                    test.codeRP = item.codeRP;
                    test.nomProgramme = item.nomProgramme;
                    test.postIT = item.postIT;
                    test.objects = item.objects;
                    test.notreClients = item.notreClients;
                    test.moa = item.moa;
                    test.moaArchitectes = item.moaArchitectes;
                    test.bureauxEtudes = item.bureauxEtudes;
                    test.bailleurSociaux = item.bailleurSociaux;
                    test.mairie = item.mairie;
                    test.dateAccords = item.dateAccords;
                    test.datePC = item.datePC;
                    test.dateConsultation = item.dateConsultation;
                    test.dateTravaux = item.dateTravaux;
                    test.dateLivraison = item.dateLivraison;
                    test.surfacePlancher = item.surfacePlancher;
                    test.surfaceTerrain = item.surfaceTerrain;
                    test.surfaceTypologie = item.surfaceTypologie;
                    test.nombreBatiment = item.nombreBatiment;
                    test.nombreEtage = item.nombreEtage;
                    test.ascenseur = item.ascenseur;
                    test.nombreAscenseur = item.nombreAscenseur;
                    test.lieuExecution = item.lieuExecution;
                    test.dateTempEvaluation = item.dateTempEvaluation;
                    test.type = item.type;
                    test.note = item.note;                    
                    //
                    test.natureRs = item.natureRs;
                    test.typologieRs = item.typologieRs;
                    test.avancementRs = item.avancementRs;
                    test.destinationRs = item.destinationRs;
                    test.codeRPRs = item.codeRPRs;
                    test.nomProgrammeRs = item.nomProgrammeRs;
                    test.postITRs = item.postITRs;
                    test.objectsRs = item.objectsRs;
                    test.notreClientsRs = item.notreClientsRs;
                    test.moaRs = item.moaRs;
                    test.moaArchitectesRs = item.moaArchitectesRs;
                    test.bureauxEtudesRs = item.bureauxEtudesRs;
                    test.bailleurSociauxRs = item.bailleurSociauxRs;
                    test.mairieRs = item.mairieRs;
                    test.dateAccordsRs = item.dateAccordsRs;
                    test.datePCRs = item.datePCRs;
                    test.dateConsultationRs = item.dateConsultationRs;
                    test.dateTravauxRs = item.dateTravauxRs;
                    test.dateLivraisonRs = item.dateLivraisonRs;
                    test.surfacePlancherRs = item.surfacePlancherRs;
                    test.surfaceTerrainRs = item.surfaceTerrainRs;
                    test.surfaceTypologieRs = item.surfaceTypologieRs;
                    test.nombreBatimentRs = item.nombreBatimentRs;
                    test.nombreEtageRs = item.nombreEtageRs;
                    test.ascenseurRs = item.ascenseurRs;
                    test.nombreAscenseurRs = item.nombreAscenseurRs;
                    test.lieuExecutionRs = item.lieuExecutionRs;
                    test.noteRs = item.noteRs;


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
                    return PartialView("ArchiveBattonageTablePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("ArchiveBPPEvaluationsAgentQualite");
        }

        public ActionResult GetArchiveBattonageEvaluationsAgentQualiteByDate(string dateDebut, string dateFin)
        {
            List<EvaluationVecteurPlusViewModel> a = new List<EvaluationVecteurPlusViewModel>();
            try
            {
                Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
                DateTime daterecherchedebut = DateTime.Parse(dateDebut);
                DateTime daterecherchefin = DateTime.Parse(dateFin);
                var historstions = Battonageservice.GetEvaluationsSenderBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationVecteurPlusViewModel test = new EvaluationVecteurPlusViewModel();
                    test.Id = item.Id;
                    //
                    test.nature = item.nature;
                    test.typologie = item.typologie;
                    test.avancement = item.avancement;
                    test.destination = item.destination;
                    test.codeRP = item.codeRP;
                    test.nomProgramme = item.nomProgramme;
                    test.postIT = item.postIT;
                    test.objects = item.objects;
                    test.notreClients = item.notreClients;
                    test.moa = item.moa;
                    test.moaArchitectes = item.moaArchitectes;
                    test.bureauxEtudes = item.bureauxEtudes;
                    test.bailleurSociaux = item.bailleurSociaux;
                    test.mairie = item.mairie;
                    test.dateAccords = item.dateAccords;
                    test.datePC = item.datePC;
                    test.dateConsultation = item.dateConsultation;
                    test.dateTravaux = item.dateTravaux;
                    test.dateLivraison = item.dateLivraison;
                    test.surfacePlancher = item.surfacePlancher;
                    test.surfaceTerrain = item.surfaceTerrain;
                    test.surfaceTypologie = item.surfaceTypologie;
                    test.nombreBatiment = item.nombreBatiment;
                    test.nombreEtage = item.nombreEtage;
                    test.ascenseur = item.ascenseur;
                    test.nombreAscenseur = item.nombreAscenseur;
                    test.lieuExecution = item.lieuExecution;
                    test.dateTempEvaluation = item.dateTempEvaluation;
                    test.type = item.type;
                    test.note = item.note;
                    //
                    test.natureRs = item.natureRs;
                    test.typologieRs = item.typologieRs;
                    test.avancementRs = item.avancementRs;
                    test.destinationRs = item.destinationRs;
                    test.codeRPRs = item.codeRPRs;
                    test.nomProgrammeRs = item.nomProgrammeRs;
                    test.postITRs = item.postITRs;
                    test.objectsRs = item.objectsRs;
                    test.notreClientsRs = item.notreClientsRs;
                    test.moaRs = item.moaRs;
                    test.moaArchitectesRs = item.moaArchitectesRs;
                    test.bureauxEtudesRs = item.bureauxEtudesRs;
                    test.bailleurSociauxRs = item.bailleurSociauxRs;
                    test.mairieRs = item.mairieRs;
                    test.dateAccordsRs = item.dateAccordsRs;
                    test.datePCRs = item.datePCRs;
                    test.dateConsultationRs = item.dateConsultationRs;
                    test.dateTravauxRs = item.dateTravauxRs;
                    test.dateLivraisonRs = item.dateLivraisonRs;
                    test.surfacePlancherRs = item.surfacePlancherRs;
                    test.surfaceTerrainRs = item.surfaceTerrainRs;
                    test.surfaceTypologieRs = item.surfaceTypologieRs;
                    test.nombreBatimentRs = item.nombreBatimentRs;
                    test.nombreEtageRs = item.nombreEtageRs;
                    test.ascenseurRs = item.ascenseurRs;
                    test.nombreAscenseurRs = item.nombreAscenseurRs;
                    test.lieuExecutionRs = item.lieuExecutionRs;               
                    test.noteRs = item.noteRs;

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
                    return PartialView("ArchiveBattonageTablePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("ArchiveBattonageEvaluationsAgentQualite");
        }
        #endregion


        #region Edit et delete Battonage
        [Authorize(Roles = "Qualité")]

        public ActionResult EditBattonage(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GrilleEvaluationBattonage evaluation = db.GrilleEvaluationBattonages.Find(id);
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
        [HttpPost, ActionName("EditBattonage")]
        [Authorize(Roles = "Qualité")]
        public ActionResult EditBattonage(int? id, GrilleEvaluationBattonage evaluation)
        {
            float total = 27;
            float totalRs = 27;
            List<float> NEList = new List<float>(new float[] { evaluation.nature, evaluation.typologie, evaluation.avancement, evaluation.destination, evaluation.codeRP, evaluation.nomProgramme,evaluation.postIT, evaluation.objects, evaluation.notreClients, evaluation.moa, evaluation.moaArchitectes, evaluation.bureauxEtudes, evaluation.bailleurSociaux, evaluation.mairie, evaluation.dateAccords, evaluation.datePC, evaluation.dateConsultation, evaluation.dateTravaux, evaluation.dateLivraison, evaluation.surfacePlancher, evaluation.surfaceTerrain, evaluation.surfaceTypologie, evaluation.nombreBatiment, evaluation.nombreEtage, evaluation.ascenseur, evaluation.nombreAscenseur, evaluation.lieuExecution });
            List<float> NEListRs = new List<float>(new float[] { evaluation.natureRs, evaluation.typologieRs, evaluation.avancementRs, evaluation.destinationRs, evaluation.codeRPRs, evaluation.nomProgrammeRs, evaluation.postITRs, evaluation.objectsRs, evaluation.notreClientsRs, evaluation.moaRs, evaluation.moaArchitectesRs, evaluation.bureauxEtudesRs, evaluation.bailleurSociauxRs, evaluation.mairieRs, evaluation.dateAccordsRs, evaluation.datePCRs, evaluation.dateConsultationRs, evaluation.dateTravauxRs, evaluation.dateLivraisonRs, evaluation.surfacePlancherRs, evaluation.surfaceTerrainRs, evaluation.surfaceTypologieRs, evaluation.nombreBatimentRs, evaluation.nombreEtageRs, evaluation.ascenseurRs, evaluation.nombreAscenseurRs, evaluation.lieuExecutionRs });
            float notes = 0;
            float notesRs = 0;

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
            foreach (var i in NEListRs)
            {
                if (i < 0)
                {
                    totalRs += i;
                }
                else
                {
                    notesRs += i;
                }
            }
            if (ModelState.IsValid)
            {
                evaluation.note = (notes / total) * 100;
                evaluation.noteRs = (notesRs / totalRs) * 100;
                db.Entry(evaluation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("BattonageHistorique");
            }
            return View(evaluation);
        }
        [Authorize(Roles = "Qualité")]
        [HttpGet]
        public ActionResult FindEvaluationBattonage(int? Id)
        {
            GrilleEvaluationBattonage item = Battonageservice.getById(Id);

            if (Request.IsAjaxRequest())
            {
                return PartialView("_AlerteDeSuppressionBattonage", item);
            }

            else
            {
                return View(item);
            }
        }
        [Authorize(Roles = "Qualité")]
        public ActionResult DeleteBattonage(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GrilleEvaluationBattonage evaluation = Battonageservice.getById(id);
            int? empId = evaluation.employeeId;
            Battonageservice.DeleteEvaluations(id, empId);
            Battonageservice.SaveChange();
            return RedirectToAction("BattonageHistorique");
        }

        #endregion

        #region Reportings QR
        public ActionResult ReportsQR()
        {
            List<Employee> logins = new List<Employee>();
            EvaluationViewModel evaluation = new EvaluationViewModel();
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 5))
            {
                ViewBag.role = "Agent Qualité";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 4))
            {
                ViewBag.role = "Qualité";
            }
           
            string d = "COMONLINE";
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
                if (!test.UserName.Equals(user.UserName) && test.Roles.Any(r => r.UserId == test.Id && r.RoleId == 1010))
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

        public JsonResult GetReportsQR(string username, string dateDebut, string dateFin)
        {
            float totPresentation = 0, totObjetAppel = 0, totMutualisation = 0, totCoordonneesAttr = 0, totmarcheRenouvlable = 0,
                totdureeMarche = 0, totMontantMarche = 0, totResponsableMarche = 0, totMailContact = 0, totTraitementObjections = 0,
                totTranscriptionsInformations = 0, totEnvoiMail = 0, totDiscours = 0, totAttitude = 0, totPriseCongé = 0, totNotes= 0;

            float NbreMutuatisation=0, NbreCoordonneesAttr=0, NbremarcheRenouvlable=0, NbredureeMarche=0, NbreMontantMarche=0,
                NbreResponsableMarche=0, NbreMailContact=0, NbreTraitementObjections=0, NbreTranscriptionsInformations=0, NbreEnvoiMail=0;
         
                DateTime daterecherchedebut = DateTime.Parse(dateDebut);
                DateTime daterecherchefin = DateTime.Parse(dateFin);

                var historstionsQR = service.GetEvaluationsBetweenTwoDates(daterecherchedebut, daterecherchefin);
                if (username != "")
                {
                    Employee emp = serviceEmployee.getByLogin(username);
                    historstionsQR = service.GetEvaluationsEmployeeBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
                }
                float nbreEvaluations = historstionsQR.Count();
                ViewBag.nbreEvaluations = nbreEvaluations;
                foreach (var item in historstionsQR)
                {
                totNotes += item.note;
                    totPresentation += item.presentationIdentification;
                    totObjetAppel += item.objetAppel;
                    if(item.mutualisation >= 0) {
                        totMutualisation += item.mutualisation;
                        NbreMutuatisation += 1;
                    }
                    if (item.coordonneesAttr >= 0)
                    {
                        totCoordonneesAttr += item.coordonneesAttr;
                        NbreCoordonneesAttr += 1; 
                    }
                    if (item.marcheRenouvlable >= 0)
                    {
                        totmarcheRenouvlable += item.marcheRenouvlable;
                        NbremarcheRenouvlable += 1;
                    }
                    if (item.dureeMarche >= 0)
                    {
                        totdureeMarche += item.dureeMarche;
                        NbredureeMarche += 1;
                    }
                    if (item.montantMarche >= 0)
                    {
                        totMontantMarche += item.montantMarche;
                        NbreMontantMarche += 1;
                    }
                    if (item.responsableMarche >= 0)
                    {
                        totResponsableMarche += item.responsableMarche;
                        NbreResponsableMarche += 1;
                    }
                    if (item.mailContact >= 0)
                    {
                        totMailContact += item.mailContact;
                        NbreMailContact += 1;
                    }
                    if (item.traitementObjections >= 0)
                    {
                        totTraitementObjections += item.traitementObjections;
                        NbreTraitementObjections += 1;
                    }
                    if (item.transcriptionsInformations >= 0)
                    {
                        totTranscriptionsInformations += item.transcriptionsInformations;
                        NbreTranscriptionsInformations += 1;
                    }
                    if (item.envoiMail >= 0) {
                        totEnvoiMail += item.envoiMail;
                        NbreEnvoiMail += 1;
                    }
                    totDiscours += item.discours;
                    totAttitude += item.attitude;
                    totPriseCongé += item.priseConge;                 
                }

                EvaluationVecteurPlusViewModel test = new EvaluationVecteurPlusViewModel();

            test.nbreEvaluations = Convert.ToInt32(nbreEvaluations);
            if ( NbreMutuatisation != 0)
            {
                test.mutualisation = (float)Math.Round((totMutualisation / (NbreMutuatisation * 5)) * 100, 2);
            }
            else { test.mutualisation = -5; }
            if (NbreCoordonneesAttr != 0)
            {
                test.coordonneesAttr = (float)Math.Round((totCoordonneesAttr / (NbreCoordonneesAttr * 5)) * 100, 2);
            }
            else { test.coordonneesAttr = -5; }
            if (NbremarcheRenouvlable != 0)
            {
                test.marcheRenouvlable = (float)Math.Round((totmarcheRenouvlable / (NbremarcheRenouvlable * 5)) * 100, 2);
            }
            else { test.marcheRenouvlable = -5; }
            if (NbredureeMarche != 0)
            {
                test.dureeMarche = (float)Math.Round((totdureeMarche / (NbredureeMarche * 5)) * 100, 2);
            }
            else { test.dureeMarche = -5; }
            if (NbreMontantMarche != 0)
            {
                test.montantMarche = (float)Math.Round((totMontantMarche / (NbreMontantMarche * 5)) * 100, 2);
            }
            else { test.montantMarche = -5; }
            if (NbreResponsableMarche != 0)
            {
                test.responsableMarche = (float)Math.Round((totResponsableMarche / (NbreResponsableMarche * 5)) * 100, 2);
            }
            else { test.responsableMarche = -5; }
            if (NbreMailContact != 0 )
            {
                test.mailContact = (float)Math.Round((totMailContact / (NbreMailContact * 5)) * 100, 2);
            }
            else { test.mailContact = -5; }
            if (NbreTraitementObjections != 0)
            {
                test.traitementObjections = (float)Math.Round((totTraitementObjections / (NbreTraitementObjections * 4)) * 100, 2);
            }
            else { test.traitementObjections = -4; }
            if (NbreTranscriptionsInformations != 0)
            {
                test.transcriptionsInformations = (float)Math.Round((totTranscriptionsInformations / (NbreTranscriptionsInformations * 5)) * 100, 2);
            }
            else { test.transcriptionsInformations = -5; }
            if (NbreEnvoiMail != 0)
            {
                test.envoiMail = (float)Math.Round((totEnvoiMail / (NbreEnvoiMail * 4)) * 100, 2);
            }
           else { test.envoiMail = -4; }

            if (nbreEvaluations != 0)
            {
                test.note = (float)Math.Round((totNotes / (nbreEvaluations * 100)) * 100, 2);
                test.presentationIdentification = (float)Math.Round((totPresentation / (nbreEvaluations * 2)) * 100, 2);
                test.objetAppel = (float)Math.Round((totObjetAppel / (nbreEvaluations * 4)) * 100, 2);
                test.discours = (float)Math.Round((totDiscours / (nbreEvaluations * 6)) * 100, 2);
                test.attitude = (float)Math.Round((totAttitude / (nbreEvaluations * 5)) * 100, 2);
                test.priseConge = (float)Math.Round((totPriseCongé / (nbreEvaluations * 2)) * 100, 2);
            }
            else {
                test.note = 0;
                test.presentationIdentification = 0;
                test.objetAppel = 0;
                test.discours = 0;
                test.attitude = 0;
                test.priseConge = 0;
            }
            return Json(test, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Reportings KLMO
        public ActionResult ReportsKLMO()
        {
            List<Employee> logins = new List<Employee>();
            EvaluationViewModel evaluation = new EvaluationViewModel();
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 5))
            {
                ViewBag.role = "Agent Qualité";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 4))
            {
                ViewBag.role = "Qualité";
            }

            string d = "COMONLINE";
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
                if (!test.UserName.Equals(user.UserName) && test.Roles.Any(r => r.UserId == test.Id && r.RoleId == 1011))
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

        public JsonResult GetReportsKLMO(string username, string dateDebut, string dateFin)
        {
            float totPresentation = 0, totObjetAppel = 0, totMutualisation = 0, totMaitriseOeuvre = 0, totCoordonneesAttr = 0,
                totAvisAttr = 0, totTraitementObjections = 0,totTranscriptionsInformations = 0, totEnvoiMail = 0, 
                totDiscours = 0, totAttitude = 0, totPriseCongé = 0, totNotes = 0;

            float NbreMutuatisation = 0, NbreMaitriseOeuvre = 0, NbreCoordonneesAttr = 0, NbreAvisAttr = 0,
                NbreTraitementObjections = 0, NbreTranscriptionsInformations = 0, NbreEnvoiMail = 0;

            DateTime daterecherchedebut = DateTime.Parse(dateDebut);
            DateTime daterecherchefin = DateTime.Parse(dateFin);

            var historstionsKLMO = KLMOservice.GetEvaluationsBetweenTwoDates(daterecherchedebut, daterecherchefin);
            if (username != "")
            {
                Employee emp = serviceEmployee.getByLogin(username);
                historstionsKLMO = KLMOservice.GetEvaluationsEmployeeBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
            }
            float nbreEvaluations = historstionsKLMO.Count();
            ViewBag.nbreEvaluations = nbreEvaluations;
            foreach (var item in historstionsKLMO)
            {
                totNotes += item.note;
                totPresentation += item.presentationIdentification;
                totObjetAppel += item.objetAppel;
                if (item.mutualisation >= 0)
                {
                    totMutualisation += item.mutualisation;
                    NbreMutuatisation += 1;
                }
                if (item.maitriseOeuvre >= 0)
                {
                    totMaitriseOeuvre += item.maitriseOeuvre;
                    NbreMaitriseOeuvre += 1;
                }
                if (item.coordonneesAttr >= 0)
                {
                    totCoordonneesAttr += item.coordonneesAttr;
                    NbreCoordonneesAttr += 1;
                }
                if (item.avisAttr >= 0)
                {
                    totAvisAttr += item.avisAttr;
                    NbreAvisAttr += 1;
                }
                if (item.traitementObjections >= 0)
                {
                    totTraitementObjections += item.traitementObjections;
                    NbreTraitementObjections += 1;
                }
                if (item.transcriptionsInformations >= 0)
                {
                    totTranscriptionsInformations += item.transcriptionsInformations;
                    NbreTranscriptionsInformations += 1;
                }
                if (item.envoiMail >= 0)
                {
                    totEnvoiMail += item.envoiMail;
                    NbreEnvoiMail += 1;
                }
                totDiscours += item.discours;
                totAttitude += item.attitude;
                totPriseCongé += item.priseConge;
            }

            EvaluationVecteurPlusViewModel test = new EvaluationVecteurPlusViewModel();
            test.nbreEvaluations = Convert.ToInt32(nbreEvaluations);
            if (NbreMutuatisation != 0)
            {
                test.mutualisation = (float)Math.Round((totMutualisation / (NbreMutuatisation * 5)) * 100, 2);
            }
            else { test.mutualisation = -5; }
            if (NbreMaitriseOeuvre != 0)
            {
                test.maitriseOeuvre = (float)Math.Round((totMaitriseOeuvre / (NbreMaitriseOeuvre * 5)) * 100, 2);
            }
            else { test.maitriseOeuvre = -5; }
            if (NbreCoordonneesAttr != 0)
            {
                test.coordonneesAttr = (float)Math.Round((totCoordonneesAttr / (NbreCoordonneesAttr * 5)) * 100, 2);
            }
            else { test.coordonneesAttr = -5; }
          
            if (NbreAvisAttr != 0)
            {
                test.avisAttr = (float)Math.Round((totAvisAttr / (NbreAvisAttr * 5)) * 100, 2);
            }
            else { test.avisAttr = -5; }
            if (NbreTraitementObjections != 0)
            {
                test.traitementObjections = (float)Math.Round((totTraitementObjections / (NbreTraitementObjections * 4)) * 100, 2);
            }
            else { test.traitementObjections = -4; }
            if (NbreTranscriptionsInformations != 0)
            {
                test.transcriptionsInformations = (float)Math.Round((totTranscriptionsInformations / (NbreTranscriptionsInformations * 5)) * 100, 2);
            }
            else { test.transcriptionsInformations = -5; }
            if (NbreEnvoiMail != 0)
            {
                test.envoiMail = (float)Math.Round((totEnvoiMail / (NbreEnvoiMail * 4)) * 100, 2);
            }
            else { test.envoiMail = -4; }

            if (nbreEvaluations != 0)
            {
                test.note = (float)Math.Round((totNotes / (nbreEvaluations * 100)) * 100, 2);
                test.presentationIdentification = (float)Math.Round((totPresentation / (nbreEvaluations * 2)) * 100, 2);
                test.objetAppel = (float)Math.Round((totObjetAppel / (nbreEvaluations * 4)) * 100, 2);
                test.discours = (float)Math.Round((totDiscours / (nbreEvaluations * 6)) * 100, 2);
                test.attitude = (float)Math.Round((totAttitude / (nbreEvaluations * 5)) * 100, 2);
                test.priseConge = (float)Math.Round((totPriseCongé / (nbreEvaluations * 2)) * 100, 2);
            }
            else
            {
                test.note = 0;
                test.presentationIdentification = 0;
                test.objetAppel = 0;
                test.discours = 0;
                test.attitude = 0;
                test.priseConge = 0;
            }
            return Json(test, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Reportings BPP
        public ActionResult ReportsBPP()
        {
            List<Employee> logins = new List<Employee>();
            EvaluationViewModel evaluation = new EvaluationViewModel();
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 5))
            {
                ViewBag.role = "Agent Qualité";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 4))
            {
                ViewBag.role = "Qualité";
            }

            string d = "COMONLINE";
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

        public JsonResult GetReportsBPP(string username, string dateDebut, string dateFin)
        {
            float totPresentation = 0, totObjetAppel = 0, totValidationNature = 0, totCalendrier = 0, totEnvergure = 0,
                totComplétudeQuestions = 0, totConfirmationIntervenants = 0, totEnchainementQuestions = 0, totTraitementObjections = 0, totReformulation = 0,
                totTranscriptionsInformations = 0, totCodesAppropriés = 0, totNoteClient = 0, totDiscours = 0, totAttitude = 0, totPriseCongé = 0, totNotes = 0;

            float NbreValidationNature = 0, NbreCalendrier = 0, NbreEnvergure = 0, NbreComplétudeQuestions = 0, NbreConfirmationIntervenants = 0,
                NbreEnchainementQuestions = 0, NbreTraitementObjections = 0, NbreReformulation = 0,
                NbreTranscriptionsInformations = 0, NbreCodesAppropriés = 0, NbreNoteClient = 0;

            DateTime daterecherchedebut = DateTime.Parse(dateDebut);
            DateTime daterecherchefin = DateTime.Parse(dateFin);

            var historstionsBPP = BPPservice.GetEvaluationsBetweenTwoDates(daterecherchedebut, daterecherchefin);
            if (username != "")
            {
                Employee emp = serviceEmployee.getByLogin(username);
                historstionsBPP = BPPservice.GetEvaluationsEmployeeBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
            }
            float nbreEvaluations = historstionsBPP.Count();
            ViewBag.nbreEvaluations = nbreEvaluations;
            foreach (var item in historstionsBPP)
            {
                totNotes += item.note;
                totPresentation += item.presentationIdentification;
                totObjetAppel += item.objetAppel;
                if (item.validationNature >= 0)
                {
                    totValidationNature += item.validationNature;
                    NbreValidationNature += 1;
                }
                if (item.calendrier >= 0)
                {
                    totCalendrier += item.calendrier;
                    NbreCalendrier += 1;
                }
                if (item.envergure >= 0)
                {
                    totEnvergure += item.envergure;
                    NbreEnvergure += 1;
                }
                if (item.complétudeQuestions >= 0)
                {
                    totComplétudeQuestions += item.complétudeQuestions;
                    NbreComplétudeQuestions += 1;
                }
                if (item.confirmationIntervenants >= 0)
                {
                    totConfirmationIntervenants += item.confirmationIntervenants;
                    NbreConfirmationIntervenants += 1;
                }
                if (item.enchainementQuestions >= 0)
                {
                    totEnchainementQuestions += item.enchainementQuestions;
                    NbreEnchainementQuestions += 1;
                }
                if (item.traitementObjections >= 0)
                {
                    totTraitementObjections += item.traitementObjections;
                    NbreTraitementObjections += 1;
                }
                if (item.reformulation >= 0)
                {
                    totReformulation += item.reformulation;
                    NbreReformulation += 1;
                }
              
                if (item.transcriptionsInformations >= 0)
                {
                    totTranscriptionsInformations += item.transcriptionsInformations;
                    NbreTranscriptionsInformations += 1;
                }
                if (item.codesAppropriés >= 0)
                {
                    totCodesAppropriés += item.codesAppropriés;
                    NbreCodesAppropriés += 1;
                }
                if (item.noteClient >= 0)
                {
                    totNoteClient += item.noteClient;
                    NbreNoteClient += 1;
                }
                totDiscours += item.discours;
                totAttitude += item.attitude;
                totPriseCongé += item.priseConge;
            }

            EvaluationVecteurPlusViewModel test = new EvaluationVecteurPlusViewModel();
            test.nbreEvaluations = Convert.ToInt32(nbreEvaluations);
            if (NbreValidationNature != 0)
            {
                test.validationNature = (float)Math.Round((totValidationNature / (NbreValidationNature * 5)) * 100, 2);
            }
            else { test.validationNature = -5; }
            if (NbreCalendrier != 0)
            {
                test.calendrier = (float)Math.Round((totCalendrier / (NbreCalendrier * 5)) * 100, 2);
            }
            else { test.calendrier = -5; }
            if (NbreEnvergure != 0)
            {
                test.envergure = (float)Math.Round((totEnvergure / (NbreEnvergure * 5)) * 100, 2);
            }
            else { test.envergure = -5; }
            if (NbreComplétudeQuestions != 0)
            {
                test.complétudeQuestions = (float)Math.Round((totComplétudeQuestions / (NbreComplétudeQuestions * 5)) * 100, 2);
            }
            else { test.complétudeQuestions = -5; }
            if (NbreConfirmationIntervenants != 0)
            {
                test.confirmationIntervenants = (float)Math.Round((totConfirmationIntervenants / (NbreConfirmationIntervenants * 5)) * 100, 2);
            }
            else { test.confirmationIntervenants = -5; }
            if (NbreEnchainementQuestions != 0)
            {
                test.enchainementQuestions = (float)Math.Round((totEnchainementQuestions / (NbreEnchainementQuestions * 5)) * 100, 2);
            }
            else { test.enchainementQuestions = -5; }
            if (NbreTraitementObjections != 0)
            {
                test.traitementObjections = (float)Math.Round((totTraitementObjections / (NbreTraitementObjections * 4)) * 100, 2);
            }
            else { test.traitementObjections = -4; }
            if (NbreReformulation != 0)
            {
                test.reformulation = (float)Math.Round((totReformulation / (NbreReformulation * 4)) * 100, 2);
            }
            else { test.reformulation = -4; }
           
            if (NbreTranscriptionsInformations != 0)
            {
                test.transcriptionsInformations = (float)Math.Round((totTranscriptionsInformations / (NbreTranscriptionsInformations * 5)) * 100, 2);
            }
            else { test.transcriptionsInformations = -5; }
            if (NbreCodesAppropriés != 0)
            {
                test.codesAppropriés = (float)Math.Round((totCodesAppropriés / (NbreCodesAppropriés * 5)) * 100, 2);
            }
            else { test.codesAppropriés = -5; }
            if (NbreNoteClient != 0)
            {
                test.noteClient = (float)Math.Round((totNoteClient / (NbreNoteClient * 5)) * 100, 2);
            }
            else { test.noteClient = -5; }

            if (nbreEvaluations != 0)
            {
                test.note = (float)Math.Round((totNotes / (nbreEvaluations * 100)) * 100, 2);
                test.presentationIdentification = (float)Math.Round((totPresentation / (nbreEvaluations * 2)) * 100, 2);
                test.objetAppel = (float)Math.Round((totObjetAppel / (nbreEvaluations * 4)) * 100, 2);
                test.discours = (float)Math.Round((totDiscours / (nbreEvaluations * 6)) * 100, 2);
                test.attitude = (float)Math.Round((totAttitude / (nbreEvaluations * 5)) * 100, 2);
                test.priseConge = (float)Math.Round((totPriseCongé / (nbreEvaluations * 2)) * 100, 2);
            }
            else
            {
                test.note = 0;
                test.presentationIdentification = 0;
                test.objetAppel = 0;
                test.discours = 0;
                test.attitude = 0;
                test.priseConge = 0;
            }
            return Json(test, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Reportings Battonage
        public ActionResult ReportsBattonage()
        {
            List<Employee> logins = new List<Employee>();
            EvaluationViewModel evaluation = new EvaluationViewModel();
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 5))
            {
                ViewBag.role = "Agent Qualité";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 4))
            {
                ViewBag.role = "Qualité";
            }

            string d = "COMONLINE";
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

        public JsonResult GetReportsBattonage(string username, string dateDebut, string dateFin)
        {
            float totNature = 0, totTypologie = 0, totAvancement = 0, totDestination = 0, totCodeRP = 0, totNomProgramme = 0, totPostIT = 0,
               totObject = 0, totNoteClients = 0, totMoa = 0, totMoaArchitectes = 0, totBureauxEtudes = 0, totBailleurSociaux = 0, totMairie = 0, totDateAccords = 0,
                totDatePC = 0, totDateConsultation = 0, totDateTravaux = 0, totDateLivraison = 0, totSurfacePlancher = 0, totSurfaceTerrain = 0,
                totSurfaceTypologie = 0, totNombreBatiment = 0, totNombreEtage = 0, totAscenseur = 0, totNombreAscenseur = 0, totLieuExecution = 0, totNotes = 0
                ,totNatureRs=0, totTypologieRs = 0, totAvancementRs = 0, totDestinationRs = 0, totCodeRPRs = 0, totNomProgrammeRs = 0, totPostITRs = 0,
               totObjectRs = 0, totNoteClientsRs = 0, totMoaRs = 0, totMoaArchitectesRs = 0, totBureauxEtudesRs = 0, totBailleurSociauxRs = 0, totMairieRs = 0, totDateAccordsRs = 0,
                totDatePCRs = 0, totDateConsultationRs = 0, totDateTravauxRs = 0, totDateLivraisonRs = 0, totSurfacePlancherRs = 0, totSurfaceTerrainRs = 0,
                totSurfaceTypologieRs = 0, totNombreBatimentRs = 0, totNombreEtageRs = 0, totAscenseurRs = 0, totNombreAscenseurRs = 0, totLieuExecutionRs = 0, totNotesRs = 0;

            float NbreNature = 0, NbreTypologie = 0, NbreAvancement = 0, NbreDestination = 0, NbreCodeRP = 0, NbreNomProgramme = 0, NbrePostIT = 0,
                 NbreObject = 0, NbreNoteClients = 0, NbreMoa = 0, NbreMoaArchitectes = 0, NbreBureauxEtudes = 0, NbreBailleurSociaux = 0, NbreMairie = 0, NbreDateAccords = 0,
                NbreDatePC = 0, NbreDateConsultation = 0, NbreDateTravaux = 0, NbreDateLivraison = 0, NbreSurfacePlancher = 0, NbreSurfaceTerrain = 0,
                NbreSurfaceTypologie = 0, NbreNombreBatiment = 0, NbreNombreEtage = 0, NbreAscenseur = 0, NbreNombreAscenseur = 0, NbreLieuExecution = 0
                , NbreNatureRs = 0, NbreTypologieRs = 0, NbreAvancementRs = 0, NbreDestinationRs = 0, NbreCodeRPRs = 0, NbreNomProgrammeRs = 0, NbrePostITRs = 0,
                 NbreObjectRs = 0, NbreNoteClientsRs = 0, NbreMoaRs = 0, NbreMoaArchitectesRs = 0, NbreBureauxEtudesRs = 0, NbreBailleurSociauxRs = 0, NbreMairieRs = 0, NbreDateAccordsRs = 0,
                NbreDatePCRs = 0, NbreDateConsultationRs = 0, NbreDateTravauxRs = 0, NbreDateLivraisonRs = 0, NbreSurfacePlancherRs = 0, NbreSurfaceTerrainRs = 0,
                NbreSurfaceTypologieRs = 0, NbreNombreBatimentRs = 0, NbreNombreEtageRs = 0, NbreAscenseurRs = 0, NbreNombreAscenseurRs = 0, NbreLieuExecutionRs = 0;

            DateTime daterecherchedebut = DateTime.Parse(dateDebut);
            DateTime daterecherchefin = DateTime.Parse(dateFin);

            var historstionsBattonage = Battonageservice.GetEvaluationsBetweenTwoDates(daterecherchedebut, daterecherchefin);
            if (username != "")
            {
                Employee emp = serviceEmployee.getByLogin(username);
                historstionsBattonage = Battonageservice.GetEvaluationsEmployeeBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
            }
            float nbreEvaluations = historstionsBattonage.Count();
            ViewBag.nbreEvaluations = nbreEvaluations;
            foreach (var item in historstionsBattonage)
            {
                totNotes += item.note;
                totNotesRs += item.noteRs;

                if (item.nature >= 0)
                {
                    totNature += item.nature;
                    NbreNature += 1;
                }

                if (item.typologie >= 0)
                {
                    totTypologie += item.typologie;
                    NbreTypologie += 1;
                }

                if (item.avancement >= 0)
                {
                    totAvancement += item.avancement;
                    NbreAvancement += 1;
                }

                if (item.destination >= 0)
                {
                    totDestination += item.destination;
                    NbreDestination += 1;
                }

                if (item.codeRP >= 0)
                {
                    totCodeRP += item.codeRP;
                    NbreCodeRP += 1;
                }
                if (item.nomProgramme >= 0)
                {
                    totNomProgramme += item.nomProgramme;
                    NbreNomProgramme += 1;
                }
                if (item.postIT >= 0)
                {
                    totPostIT += item.postIT;
                    NbrePostIT += 1;
                }
                if (item.objects >= 0)
                {
                    totObject += item.objects;
                    NbreObject += 1;
                }
                if (item.notreClients >= 0)
                {
                    totNoteClients += item.notreClients;
                    NbreNoteClients += 1;
                }
                if (item.moa >= 0)
                {
                    totMoa += item.moa;
                    NbreMoa += 1;
                }
                if (item.moaArchitectes >= 0)
                {
                    totMoaArchitectes += item.moaArchitectes;
                    NbreMoaArchitectes += 1;
                }
                if (item.bureauxEtudes >= 0)
                {
                    totBureauxEtudes += item.bureauxEtudes;
                    NbreBureauxEtudes += 1;
                }

                if (item.bailleurSociaux >= 0)
                {
                    totBailleurSociaux += item.bailleurSociaux;
                    NbreBailleurSociaux += 1;
                }
                if (item.mairie >= 0)
                {
                    totMairie += item.mairie;
                    NbreMairie += 1;
                }
                if (item.dateAccords >= 0)
                {
                    totDateAccords += item.dateAccords;
                    NbreDateAccords += 1;
                }
                if (item.datePC >= 0)
                {
                    totDatePC += item.datePC;
                    NbreDatePC += 1;
                }
                if (item.dateConsultation >= 0)
                {
                    totDateConsultation += item.dateConsultation;
                    NbreDateConsultation += 1;
                }
                if (item.dateTravaux >= 0)
                {
                    totDateTravaux += item.dateTravaux;
                    NbreDateTravaux += 1;
                }
                if (item.dateLivraison >= 0)
                {
                    totDateLivraison += item.dateLivraison;
                    NbreDateLivraison += 1;
                }
                if (item.surfacePlancher >= 0)
                {
                    totSurfacePlancher += item.surfacePlancher;
                    NbreSurfacePlancher += 1;
                }
                if (item.surfaceTerrain >= 0)
                {
                    totSurfaceTerrain += item.surfaceTerrain;
                    NbreSurfaceTerrain += 1;
                }
                if (item.surfaceTypologie >= 0)
                {
                    totSurfaceTypologie += item.surfaceTypologie;
                    NbreSurfaceTypologie += 1;
                }
                if (item.nombreBatiment >= 0)
                {
                    totNombreBatiment += item.nombreBatiment;
                    NbreNombreBatiment += 1;
                }
                if (item.nombreEtage >= 0)
                {
                    totNombreEtage += item.nombreEtage;
                    NbreNombreEtage += 1;
                }
                if (item.ascenseur >= 0)
                {
                    totAscenseur += item.ascenseur;
                    NbreAscenseur += 1;
                }
                if (item.nombreAscenseur >= 0)
                {
                    totNombreAscenseur += item.nombreAscenseur;
                    NbreNombreAscenseur += 1;
                }
                if (item.lieuExecution >= 0)
                {
                    totLieuExecution += item.lieuExecution;
                    NbreLieuExecution += 1;
                }
                //
                if (item.natureRs >= 0)
                {
                    totNatureRs += item.natureRs;
                    NbreNatureRs += 1;
                }

                if (item.typologieRs >= 0)
                {
                    totTypologieRs += item.typologieRs;
                    NbreTypologieRs += 1;
                }

                if (item.avancementRs >= 0)
                {
                    totAvancementRs += item.avancementRs;
                    NbreAvancementRs += 1;
                }

                if (item.destinationRs >= 0)
                {
                    totDestinationRs += item.destinationRs;
                    NbreDestinationRs += 1;
                }

                if (item.codeRPRs >= 0)
                {
                    totCodeRPRs += item.codeRPRs;
                    NbreCodeRPRs += 1;
                }
                if (item.nomProgrammeRs >= 0)
                {
                    totNomProgrammeRs += item.nomProgrammeRs;
                    NbreNomProgrammeRs += 1;
                }
                if (item.postITRs >= 0)
                {
                    totPostITRs += item.postITRs;
                    NbrePostITRs += 1;
                }
                if (item.objectsRs >= 0)
                {
                    totObjectRs += item.objectsRs;
                    NbreObjectRs += 1;
                }
                if (item.notreClientsRs >= 0)
                {
                    totNoteClientsRs += item.notreClientsRs;
                    NbreNoteClientsRs += 1;
                }
                if (item.moaRs >= 0)
                {
                    totMoaRs += item.moaRs;
                    NbreMoaRs += 1;
                }
                if (item.moaArchitectesRs >= 0)
                {
                    totMoaArchitectesRs += item.moaArchitectesRs;
                    NbreMoaArchitectesRs += 1;
                }
                if (item.bureauxEtudesRs >= 0)
                {
                    totBureauxEtudesRs += item.bureauxEtudesRs;
                    NbreBureauxEtudesRs += 1;
                }

                if (item.bailleurSociauxRs >= 0)
                {
                    totBailleurSociauxRs += item.bailleurSociauxRs;
                    NbreBailleurSociauxRs += 1;
                }
                if (item.mairieRs >= 0)
                {
                    totMairieRs += item.mairieRs;
                    NbreMairieRs += 1;
                }
                if (item.dateAccordsRs >= 0)
                {
                    totDateAccordsRs += item.dateAccordsRs;
                    NbreDateAccordsRs += 1;
                }
                if (item.datePCRs >= 0)
                {
                    totDatePCRs += item.datePCRs;
                    NbreDatePCRs += 1;
                }
                if (item.dateConsultationRs >= 0)
                {
                    totDateConsultationRs += item.dateConsultationRs;
                    NbreDateConsultationRs += 1;
                }
                if (item.dateTravauxRs >= 0)
                {
                    totDateTravauxRs += item.dateTravauxRs;
                    NbreDateTravauxRs += 1;
                }
                if (item.dateLivraisonRs >= 0)
                {
                    totDateLivraisonRs += item.dateLivraisonRs;
                    NbreDateLivraisonRs += 1;
                }
                if (item.surfacePlancherRs >= 0)
                {
                    totSurfacePlancherRs += item.surfacePlancherRs;
                    NbreSurfacePlancherRs += 1;
                }
                if (item.surfaceTerrainRs >= 0)
                {
                    totSurfaceTerrainRs += item.surfaceTerrainRs;
                    NbreSurfaceTerrainRs += 1;
                }
                if (item.surfaceTypologieRs >= 0)
                {
                    totSurfaceTypologieRs += item.surfaceTypologieRs;
                    NbreSurfaceTypologieRs += 1;
                }
                if (item.nombreBatimentRs >= 0)
                {
                    totNombreBatimentRs += item.nombreBatimentRs;
                    NbreNombreBatimentRs += 1;
                }
                if (item.nombreEtageRs >= 0)
                {
                    totNombreEtageRs += item.nombreEtageRs;
                    NbreNombreEtageRs += 1;
                }
                if (item.ascenseurRs >= 0)
                {
                    totAscenseurRs += item.ascenseurRs;
                    NbreAscenseurRs += 1;
                }
                if (item.nombreAscenseurRs >= 0)
                {
                    totNombreAscenseurRs += item.nombreAscenseurRs;
                    NbreNombreAscenseurRs += 1;
                }
                if (item.lieuExecutionRs >= 0)
                {
                    totLieuExecutionRs += item.lieuExecutionRs;
                    NbreLieuExecutionRs += 1;
                }

            }

            EvaluationVecteurPlusViewModel test = new EvaluationVecteurPlusViewModel();
            test.nbreEvaluations = Convert.ToInt32(nbreEvaluations);
            if (NbreNature != 0)
            {
                test.nature = (float)Math.Round((totNature / NbreNature ) * 100, 2);
            }
            else { test.nature = -1; }

            if (NbreTypologie != 0)
            {
                test.typologie = (float)Math.Round((totTypologie / NbreTypologie ) * 100, 2);
            }
            else { test.typologie = -1; }

            if (NbreAvancement != 0)
            {
                test.avancement = (float)Math.Round((totAvancement / NbreAvancement) * 100, 2);
            }
            else { test.avancement = -1; }

            if (NbreDestination != 0)
            {
                test.destination = (float)Math.Round((totDestination / NbreDestination) * 100, 2);
            }
            else { test.destination = -1; }

            if (NbreCodeRP != 0)
            {
                test.codeRP = (float)Math.Round((totCodeRP / NbreCodeRP) * 100, 2);
            }
            else { test.codeRP = -1; }

            if (NbreNomProgramme != 0)
            {
                test.nomProgramme = (float)Math.Round((totNomProgramme / NbreNomProgramme) * 100, 2);
            }
            else { test.nomProgramme = -1; }

            if (NbrePostIT != 0)
            {
                test.postIT = (float)Math.Round((totPostIT / NbrePostIT) * 100, 2);
            }
            else { test.postIT = -1; }

            if (NbreObject != 0)
            {
                test.objects = (float)Math.Round((totObject / NbreObject) * 100, 2);
            }
            else { test.objects = -1; }

            if (NbreNoteClients != 0)
            {
                test.notreClients = (float)Math.Round((totNoteClients / NbreNoteClients) * 100, 2);
            }
            else { test.notreClients = -1; }

            if (NbreMoa != 0)
            {
                test.moa = (float)Math.Round((totMoa / NbreMoa) * 100, 2);
            }
            else { test.moa = -1; }

            if (NbreMoaArchitectes != 0)
            {
                test.moaArchitectes = (float)Math.Round((totMoaArchitectes / NbreMoaArchitectes) * 100, 2);
            }
            else { test.moaArchitectes = -1; }

            if (NbreBureauxEtudes != 0)
            {
                test.bureauxEtudes = (float)Math.Round((totBureauxEtudes / NbreBureauxEtudes) * 100, 2);
            }
            else { test.bureauxEtudes = -1; }

            if (NbreBailleurSociaux != 0)
            {
                test.bailleurSociaux = (float)Math.Round((totBailleurSociaux / NbreBailleurSociaux) * 100, 2);
            }
            else { test.bailleurSociaux = -1; }

            if (NbreMairie != 0)
            {
                test.mairie = (float)Math.Round((totMairie / NbreMairie) * 100, 2);
            }
            else { test.mairie = -1; }

            if (NbreDateAccords != 0)
            {
                test.dateAccords = (float)Math.Round((totDateAccords / NbreDateAccords) * 100, 2);
            }
            else { test.dateAccords = -1; }

            if (NbreDatePC != 0)
            {
                test.datePC = (float)Math.Round((totDatePC / NbreDatePC) * 100, 2);
            }
            else { test.datePC = -1; }

            if (NbreDateConsultation != 0)
            {
                test.dateConsultation = (float)Math.Round((totDateConsultation / NbreDateConsultation) * 100, 2);
            }
            else { test.dateConsultation = -1; }

            if (NbreDateTravaux != 0)
            {
                test.dateTravaux = (float)Math.Round((totDateTravaux / NbreDateTravaux) * 100, 2);
            }
            else { test.dateTravaux = -1; }

            if (NbreDateLivraison != 0)
            {
                test.dateLivraison = (float)Math.Round((totDateLivraison / NbreDateLivraison) * 100, 2);
            }
            else { test.dateLivraison = -1; }

            if (NbreSurfacePlancher != 0)
            {
                test.surfacePlancher = (float)Math.Round((totSurfacePlancher / NbreSurfacePlancher) * 100, 2);
            }
            else { test.surfacePlancher = -1; }

            if (NbreSurfaceTerrain != 0)
            {
                test.surfaceTerrain = (float)Math.Round((totSurfaceTerrain / NbreSurfaceTerrain) * 100, 2);
            }
            else { test.surfaceTerrain = -1; }

            if (NbreSurfaceTypologie != 0)
            {
                test.surfaceTypologie = (float)Math.Round((totSurfaceTypologie / NbreSurfaceTypologie) * 100, 2);
            }
            else { test.surfaceTypologie = -1; }

            if (NbreNombreBatiment != 0)
            {
                test.nombreBatiment = (float)Math.Round((totNombreBatiment / NbreNombreBatiment) * 100, 2);
            }
            else { test.nombreBatiment = -1; }

            if (NbreNombreEtage != 0)
            {
                test.nombreEtage = (float)Math.Round((totNombreEtage / NbreNombreEtage) * 100, 2);
            }
            else { test.nombreEtage = -1; }

            if (NbreAscenseur != 0)
            {
                test.ascenseur = (float)Math.Round((totAscenseur / NbreAscenseur) * 100, 2);
            }
            else { test.ascenseur = -1; }

            if (NbreNombreAscenseur != 0)
            {
                test.nombreAscenseur = (float)Math.Round((totNombreAscenseur / NbreNombreAscenseur) * 100, 2);
            }
            else { test.nombreAscenseur = -1; }

            if (NbreLieuExecution != 0)
            {
                test.lieuExecution = (float)Math.Round((totLieuExecution / NbreLieuExecution) * 100, 2);
            }
            else { test.lieuExecution = -1; }

            if (nbreEvaluations != 0)
            {
                test.note = (float)Math.Round((totNotes / (nbreEvaluations * 100)) * 100, 2);
            }
            else
            {
                test.note = 0;
            }
            //
            if (NbreNatureRs != 0)
            {
                test.natureRs = (float)Math.Round((totNatureRs / NbreNatureRs) * 100, 2);
            }
            else { test.natureRs = -1; } 

            if (NbreTypologieRs != 0)
            {
                test.typologieRs = (float)Math.Round((totTypologieRs / NbreTypologieRs) * 100, 2);
            }
            else { test.typologieRs = -1; }

            if (NbreAvancementRs != 0)
            {
                test.avancementRs = (float)Math.Round((totAvancementRs / NbreAvancementRs) * 100, 2);
            }
            else { test.avancementRs = -1; }

            if (NbreDestinationRs != 0)
            {
                test.destinationRs = (float)Math.Round((totDestinationRs / NbreDestinationRs) * 100, 2);
            }
            else { test.destinationRs = -1; }

            if (NbreCodeRPRs != 0)
            {
                test.codeRPRs = (float)Math.Round((totCodeRPRs / NbreCodeRPRs) * 100, 2);
            }
            else { test.codeRPRs = -1; }

            if (NbreNomProgrammeRs != 0)
            {
                test.nomProgrammeRs = (float)Math.Round((totNomProgrammeRs / NbreNomProgrammeRs) * 100, 2);
            }
            else { test.nomProgrammeRs = -1; }

            if (NbrePostITRs != 0)
            {
                test.postITRs = (float)Math.Round((totPostITRs / NbrePostITRs) * 100, 2);
            }
            else { test.postITRs = -1; }

            if (NbreObjectRs != 0)
            {
                test.objectsRs = (float)Math.Round((totObjectRs / NbreObjectRs) * 100, 2);
            }
            else { test.objectsRs = -1; }

            if (NbreNoteClientsRs != 0)
            {
                test.notreClientsRs = (float)Math.Round((totNoteClientsRs / NbreNoteClientsRs) * 100, 2);
            }
            else { test.notreClientsRs = -1; }

            if (NbreMoaRs != 0)
            {
                test.moaRs = (float)Math.Round((totMoaRs / NbreMoaRs) * 100, 2);
            }
            else { test.moaRs = -1; }

            if (NbreMoaArchitectesRs != 0)
            {
                test.moaArchitectesRs = (float)Math.Round((totMoaArchitectesRs / NbreMoaArchitectesRs) * 100, 2);
            }
            else { test.moaArchitectesRs = -1; }

            if (NbreBureauxEtudesRs != 0)
            {
                test.bureauxEtudesRs = (float)Math.Round((totBureauxEtudesRs / NbreBureauxEtudesRs) * 100, 2);
            }
            else { test.bureauxEtudesRs = -1; }

            if (NbreBailleurSociauxRs != 0)
            {
                test.bailleurSociauxRs = (float)Math.Round((totBailleurSociauxRs / NbreBailleurSociauxRs) * 100, 2);
            }
            else { test.bailleurSociauxRs = -1; }

            if (NbreMairieRs != 0)
            {
                test.mairieRs = (float)Math.Round((totMairieRs / NbreMairieRs) * 100, 2);
            }
            else { test.mairieRs = -1; }

            if (NbreDateAccordsRs != 0)
            {
                test.dateAccordsRs = (float)Math.Round((totDateAccordsRs / NbreDateAccordsRs) * 100, 2);
            }
            else { test.dateAccordsRs = -1; }

            if (NbreDatePCRs != 0)
            {
                test.datePCRs = (float)Math.Round((totDatePCRs / NbreDatePCRs) * 100, 2);
            }
            else { test.datePCRs = -1; }

            if (NbreDateConsultationRs != 0)
            {
                test.dateConsultationRs = (float)Math.Round((totDateConsultationRs / NbreDateConsultationRs) * 100, 2);
            }
            else { test.dateConsultationRs = -1; }

            if (NbreDateTravauxRs != 0)
            {
                test.dateTravauxRs = (float)Math.Round((totDateTravauxRs / NbreDateTravauxRs) * 100, 2);
            }
            else { test.dateTravauxRs = -1; }

            if (NbreDateLivraisonRs != 0)
            {
                test.dateLivraisonRs = (float)Math.Round((totDateLivraisonRs / NbreDateLivraisonRs) * 100, 2);
            }
            else { test.dateLivraisonRs = -1; }

            if (NbreSurfacePlancherRs != 0)
            {
                test.surfacePlancherRs = (float)Math.Round((totSurfacePlancherRs / NbreSurfacePlancherRs) * 100, 2);
            }
            else { test.surfacePlancherRs = -1; }

            if (NbreSurfaceTerrainRs != 0)
            {
                test.surfaceTerrainRs = (float)Math.Round((totSurfaceTerrainRs / NbreSurfaceTerrainRs) * 100, 2);
            }
            else { test.surfaceTerrainRs = -1; }

            if (NbreSurfaceTypologieRs != 0)
            {
                test.surfaceTypologieRs = (float)Math.Round((totSurfaceTypologieRs / NbreSurfaceTypologieRs) * 100, 2);
            }
            else { test.surfaceTypologieRs = -1; }

            if (NbreNombreBatimentRs != 0)
            {
                test.nombreBatimentRs = (float)Math.Round((totNombreBatimentRs / NbreNombreBatimentRs) * 100, 2);
            }
            else { test.nombreBatimentRs = -1; }

            if (NbreNombreEtageRs != 0)
            {
                test.nombreEtageRs = (float)Math.Round((totNombreEtageRs / NbreNombreEtageRs) * 100, 2);
            }
            else { test.nombreEtageRs = -1; }

            if (NbreAscenseurRs != 0)
            {
                test.ascenseurRs = (float)Math.Round((totAscenseurRs / NbreAscenseurRs) * 100, 2);
            }
            else { test.ascenseurRs = -1; }

            if (NbreNombreAscenseurRs != 0)
            {
                test.nombreAscenseurRs = (float)Math.Round((totNombreAscenseurRs / NbreNombreAscenseurRs) * 100, 2);
            }
            else { test.nombreAscenseurRs = -1; }

            if (NbreLieuExecutionRs != 0)
            {
                test.lieuExecutionRs = (float)Math.Round((totLieuExecutionRs / NbreLieuExecutionRs) * 100, 2);
            }
            else { test.lieuExecutionRs = -1; }

            if (nbreEvaluations != 0)
            {
                test.noteRs = (float)Math.Round((totNotesRs / (nbreEvaluations * 100)) * 100, 2);
            }
            else
            {
                test.noteRs = 0;
            }
            //
            return Json(test, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region PRV VECTEUR PLUS
        [Authorize(Roles = "Qualité, Agent Qualité, Agent Qualité_PRV")]

        public ActionResult PRVVecteurPlus(string enregistrementFullName, string enregistrementUrl, string enregistrementDirectory, string agentName)
        {

            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 5))
            {
                ViewBag.role = "Agent Qualité";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 4))
            {
                ViewBag.role = "Qualité";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 2014))
            {
                ViewBag.role = "Agent Qualité_PRV";
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
        
        public ActionResult PRVVecteurPlusWithoutAgent(string enregistrementFullName, string enregistrementUrl, string enregistrementDirectory, string siteName)
        {
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            var a = new EvaluationViewModel();
            List<SelectListItem> agents = new List<SelectListItem>();
            List<Employee> logins = new List<Employee>();
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 5))
            {
                ViewBag.role = "Agent Qualité";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 4))
            {
                ViewBag.role = "Qualité";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 2014))
            {
                ViewBag.role = "Agent Qualité_PRV";
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

        public ActionResult SaveEvalPRVVecteurPlus(string nomAgent, string planDate, string presentation, string objetAppel, string validationDisponibilite, string questionnement, string reformulation, string propositionPRV, string objections, string calendrier, string validation, string recapitulatif, string discours, string attitude, string priseConge, string commentaire, string enregistrement, string enregistrementUrl, string enregistrementDirectory)
        {
            var userConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            Employee emp = serviceEmployee.getByPseudoName(nomAgent.ToLower());

            GrilleEvaluationPRV a = new GrilleEvaluationPRV();
            float total = 62;
            List<string> NEList = new List<string>(new string[] { validationDisponibilite, reformulation, propositionPRV, objections, calendrier, validation, recapitulatif, discours });
            float notes = float.Parse(presentation, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(objetAppel, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(questionnement, CultureInfo.InvariantCulture.NumberFormat) +
              float.Parse(attitude, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(priseConge, CultureInfo.InvariantCulture.NumberFormat);

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
            a.objetAppel = float.Parse(objetAppel, CultureInfo.InvariantCulture.NumberFormat);
            a.validationDisponibilite = float.Parse(validationDisponibilite, CultureInfo.InvariantCulture.NumberFormat);
            a.questionnement = float.Parse(questionnement, CultureInfo.InvariantCulture.NumberFormat);
            a.reformulation = float.Parse(reformulation, CultureInfo.InvariantCulture.NumberFormat);
            a.propositionPRV = float.Parse(propositionPRV, CultureInfo.InvariantCulture.NumberFormat);
            a.objections = float.Parse(objections, CultureInfo.InvariantCulture.NumberFormat);
            a.calendrier = float.Parse(calendrier, CultureInfo.InvariantCulture.NumberFormat);
            a.validation = float.Parse(validation, CultureInfo.InvariantCulture.NumberFormat);
            a.recapitulatif = float.Parse(recapitulatif, CultureInfo.InvariantCulture.NumberFormat);
            a.discours = float.Parse(discours, CultureInfo.InvariantCulture.NumberFormat);
            a.attitude = float.Parse(attitude, CultureInfo.InvariantCulture.NumberFormat);
            a.priseConge = float.Parse(priseConge, CultureInfo.InvariantCulture.NumberFormat);

            a.dateTempEvaluation = DateTime.Parse(planDate);
            a.employeeId = emp.Id;
            a.senderId = userConnected.Id;
            a.note = (notes / total) * 100;
            a.type = "PRV Vecteur Plus";
            a.commentaireQualite = commentaire;
            a.enregistrementFullName = enregistrement;
            a.enregistrementUrl = enregistrementUrl;
            a.enregistrementDirectory = enregistrementDirectory;

            PRVservice.Add(a);
            PRVservice.SaveChange();

            var eval = new EvaluationVecteurPlusViewModel();
            eval.agentName = nomAgent;
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
                return PartialView("EnvoiMailPartialView", eval);
            }
            return RedirectToAction("Acceuil", "Directory");

        }

        public ActionResult CalculPRVVecteurPlus(string nomAgent, string planDate, string presentation, string objetAppel, string validationDisponibilite, string questionnement, string reformulation, string propositionPRV, string objections, string calendrier, string validation, string recapitulatif, string discours, string attitude, string priseConge, string commentaire, string enregistrement, string enregistrementUrl, string enregistrementDirectory)
        {
            float total = 62;
            List<string> NEList = new List<string>(new string[] { validationDisponibilite, reformulation, propositionPRV, objections, calendrier, validation, recapitulatif, discours });
            float notes = float.Parse(presentation, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(objetAppel, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(questionnement, CultureInfo.InvariantCulture.NumberFormat) +
              float.Parse(attitude, CultureInfo.InvariantCulture.NumberFormat) + float.Parse(priseConge, CultureInfo.InvariantCulture.NumberFormat);

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
            var a = new EvaluationVecteurPlusViewModel();
            a.presentation = float.Parse(presentation, CultureInfo.InvariantCulture.NumberFormat);
            a.objetAppel = float.Parse(objetAppel, CultureInfo.InvariantCulture.NumberFormat);
            a.validationDisponibilite = float.Parse(validationDisponibilite, CultureInfo.InvariantCulture.NumberFormat);
            a.questionnement = float.Parse(questionnement, CultureInfo.InvariantCulture.NumberFormat);
            a.reformulation = float.Parse(reformulation, CultureInfo.InvariantCulture.NumberFormat);
            a.propositionPRV = float.Parse(propositionPRV, CultureInfo.InvariantCulture.NumberFormat);
            a.objections = float.Parse(objections, CultureInfo.InvariantCulture.NumberFormat);
            a.calendrier = float.Parse(calendrier, CultureInfo.InvariantCulture.NumberFormat);
            a.validation = float.Parse(validation, CultureInfo.InvariantCulture.NumberFormat);
            a.recapitulatif = float.Parse(recapitulatif, CultureInfo.InvariantCulture.NumberFormat);
            a.discours = float.Parse(discours, CultureInfo.InvariantCulture.NumberFormat);
            a.attitude = float.Parse(attitude, CultureInfo.InvariantCulture.NumberFormat);
            a.priseConge = float.Parse(priseConge, CultureInfo.InvariantCulture.NumberFormat);

            a.dateTempEvaluation = DateTime.Parse(planDate);
            a.agentName = nomAgent;
            a.note = notes;
            a.pourcentageNotes = (notes / total) * 100;
            a.type = "PRV Vecteur Plus";

            if (Request.IsAjaxRequest())
            {
                return PartialView("PRVResultPartialView", a);
            }
            return RedirectToAction("Acceuil", "Directory");
        }

        //PRV Historique
        public ActionResult PRVHistorique()
        {
            List<Employee> logins = new List<Employee>();
            EvaluationViewModel evaluation = new EvaluationViewModel();
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 5))
            {
                ViewBag.role = "Agent Qualité";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 4))
            {
                ViewBag.role = "Qualité";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 2014))
            {
                ViewBag.role = "Agent Qualité_PRV";
            }

            string d = "COMONLINE";
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
                if (!test.UserName.Equals(user.UserName) && test.Roles.Any(r => r.UserId == test.Id && r.RoleId == 2013))
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
   
        public ActionResult GetHistoPRVVecteurPlus(string username)
        {
            List<EvaluationVecteurPlusViewModel> a = new List<EvaluationVecteurPlusViewModel>();
            try
            {
                Employee emp = serviceEmployee.getByLogin(username);
                var historstions = PRVservice.GetEvaluationsByEmployee(emp.Id);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationVecteurPlusViewModel test = new EvaluationVecteurPlusViewModel();
                    test.Id = item.Id;
                    test.presentation = item.presentation;
                    test.objetAppel = item.objetAppel;
                    test.validationDisponibilite = item.validationDisponibilite;
                    test.questionnement = item.questionnement;
                    test.reformulation = item.reformulation;
                    test.propositionPRV = item.propositionPRV;
                    test.objections = item.objections;
                    test.calendrier = item.calendrier;
                    test.validation = item.validation;
                    test.recapitulatif = item.recapitulatif;
                    test.discours = item.discours;
                    test.attitude = item.attitude;
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
                    return PartialView("HistoriquePRVTablePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("QRHistorique");
        }
        public ActionResult GetHistoPRVByDate(string username, string dateDebut, string dateFin)
        {
            List<EvaluationVecteurPlusViewModel> a = new List<EvaluationVecteurPlusViewModel>();
            try
            {
                Employee emp = serviceEmployee.getByLogin(username);
                DateTime daterecherchedebut = DateTime.Parse(dateDebut);
                DateTime daterecherchefin = DateTime.Parse(dateFin);


                var historstions = PRVservice.GetEvaluationsEmployeeBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationVecteurPlusViewModel test = new EvaluationVecteurPlusViewModel();
                    test.Id = item.Id;
                    test.presentation = item.presentation;
                    test.objetAppel = item.objetAppel;
                    test.validationDisponibilite = item.validationDisponibilite;
                    test.questionnement = item.questionnement;
                    test.reformulation = item.reformulation;
                    test.propositionPRV = item.propositionPRV;
                    test.objections = item.objections;
                    test.calendrier = item.calendrier;
                    test.validation = item.validation;
                    test.recapitulatif = item.recapitulatif;
                    test.discours = item.discours;
                    test.attitude = item.attitude;
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
                    return PartialView("HistoriquePRVTablePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("QRHistorique");
        }
        //Archive (Par Agent Qualité) 
        public ActionResult ArchivePRVEvaluationsQualite()
        {
            EvaluationVecteurPlusViewModel evaluation = new EvaluationVecteurPlusViewModel();
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            var logins = serviceEmployee.GetAll();
            var employees = logins.Select(o => o).Distinct().ToList();

            foreach (var test in employees)
            {
                if (test.Roles.Any(r => r.UserId == test.Id && r.RoleId == 4 || r.RoleId == 5||r.RoleId==2014))
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

        public ActionResult GetPRVArchiveEvaluationsQualite(string username)
        {
            List<EvaluationVecteurPlusViewModel> a = new List<EvaluationVecteurPlusViewModel>();
            try
            {
                Employee emp = serviceEmployee.getByLogin(username);
                var historstions = PRVservice.GetEvaluationsBySenderId(emp.Id);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationVecteurPlusViewModel test = new EvaluationVecteurPlusViewModel();
                    test.Id = item.Id;
                    test.presentation = item.presentation;
                    test.objetAppel = item.objetAppel;
                    test.validationDisponibilite = item.validationDisponibilite;
                    test.questionnement = item.questionnement;
                    test.reformulation = item.reformulation;
                    test.propositionPRV = item.propositionPRV;
                    test.objections = item.objections;
                    test.calendrier = item.calendrier;
                    test.validation = item.validation;
                    test.recapitulatif = item.recapitulatif;
                    test.discours = item.discours;
                    test.attitude = item.attitude;
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
                    return PartialView("ArchivePRVTablePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("ArchivePRVEvaluationsQualite");
        }

        public ActionResult GetArchivePRVEvaluationsByDate(string username, string dateDebut, string dateFin)
        {
            List<EvaluationVecteurPlusViewModel> a = new List<EvaluationVecteurPlusViewModel>();
            try
            {
                Employee emp = serviceEmployee.getByLogin(username);
                DateTime daterecherchedebut = DateTime.Parse(dateDebut);
                DateTime daterecherchefin = DateTime.Parse(dateFin);
                var historstions = PRVservice.GetEvaluationsSenderBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationVecteurPlusViewModel test = new EvaluationVecteurPlusViewModel();
                    test.Id = item.Id;
                    test.presentation = item.presentation;
                    test.objetAppel = item.objetAppel;
                    test.validationDisponibilite = item.validationDisponibilite;
                    test.questionnement = item.questionnement;
                    test.reformulation = item.reformulation;
                    test.propositionPRV = item.propositionPRV;
                    test.objections = item.objections;
                    test.calendrier = item.calendrier;
                    test.validation = item.validation;
                    test.recapitulatif = item.recapitulatif;
                    test.discours = item.discours;
                    test.attitude = item.attitude;
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
                    return PartialView("ArchivePRVTablePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("ArchivePRVEvaluationsQualite");
        }

        //Page  Archive Agent Qualité (Mon Historique)
        public ActionResult ArchivePRVEvaluationsAgentQualite()
        {
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 5))
            {
                ViewBag.role = "Agent Qualité";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 2014))
            {
                ViewBag.role = "Agent Qualité_PRV";
            }
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
        public ActionResult GetArchivePRVEvaluationsAgentQualite()
        {
            List<EvaluationVecteurPlusViewModel> a = new List<EvaluationVecteurPlusViewModel>();
            try
            {
                Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

                var historstions = PRVservice.GetEvaluationsBySenderId(emp.Id);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationVecteurPlusViewModel test = new EvaluationVecteurPlusViewModel();
                    test.Id = item.Id;
                    test.presentation = item.presentation;
                    test.objetAppel = item.objetAppel;
                    test.validationDisponibilite = item.validationDisponibilite;
                    test.questionnement = item.questionnement;
                    test.reformulation = item.reformulation;
                    test.propositionPRV = item.propositionPRV;
                    test.objections = item.objections;
                    test.calendrier = item.calendrier;
                    test.validation = item.validation;
                    test.recapitulatif = item.recapitulatif;
                    test.discours = item.discours;
                    test.attitude = item.attitude;
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
                    return PartialView("ArchivePRVTablePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("ArchivePRVEvaluationsAgentQualite");
        }

        public ActionResult GetArchivePRVEvaluationsAgentQualiteByDate(string dateDebut, string dateFin)
        {
            List<EvaluationVecteurPlusViewModel> a = new List<EvaluationVecteurPlusViewModel>();
            try
            {
                Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
                DateTime daterecherchedebut = DateTime.Parse(dateDebut);
                DateTime daterecherchefin = DateTime.Parse(dateFin);
                var historstions = PRVservice.GetEvaluationsSenderBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationVecteurPlusViewModel test = new EvaluationVecteurPlusViewModel();
                    test.Id = item.Id;
                    test.presentation = item.presentation;
                    test.objetAppel = item.objetAppel;
                    test.validationDisponibilite = item.validationDisponibilite;
                    test.questionnement = item.questionnement;
                    test.reformulation = item.reformulation;
                    test.propositionPRV = item.propositionPRV;
                    test.objections = item.objections;
                    test.calendrier = item.calendrier;
                    test.validation = item.validation;
                    test.recapitulatif = item.recapitulatif;
                    test.discours = item.discours;
                    test.attitude = item.attitude;
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
                    return PartialView("ArchivePRVTablePartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("ArchivePRVEvaluationsAgentQualite");
        }
        #endregion

        #region historiqueAgent PRV
        [Authorize(Roles = "Agent_PRV")]
        public ActionResult HistoriqueAgent_PRV()
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
        public ActionResult GetHistoAgentPRV()
        {
            List<EvaluationVecteurPlusViewModel> a = new List<EvaluationVecteurPlusViewModel>();
            try
            {
                Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

                var historstions = PRVservice.GetEvaluationsByEmployee(emp.Id);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationVecteurPlusViewModel test = new EvaluationVecteurPlusViewModel();
                    test.Id = item.Id;
                    test.presentation = item.presentation;
                    test.objetAppel = item.objetAppel;
                    test.validationDisponibilite = item.validationDisponibilite;
                    test.questionnement = item.questionnement;
                    test.reformulation = item.reformulation;
                    test.propositionPRV = item.propositionPRV;
                    test.objections = item.objections;
                    test.calendrier = item.calendrier;
                    test.validation = item.validation;
                    test.recapitulatif = item.recapitulatif;
                    test.discours = item.discours;
                    test.attitude = item.attitude;
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
                    return PartialView("HistoriqueTablePRVAgentPartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("HistoriqueAgent_PRV");
        }

        public ActionResult GetHistoAgentPRVByDate(string dateDebut, string dateFin)
        {
            List<EvaluationVecteurPlusViewModel> a = new List<EvaluationVecteurPlusViewModel>();
            try
            {
                Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
                DateTime daterecherchedebut = DateTime.Parse(dateDebut);
                DateTime daterecherchefin = DateTime.Parse(dateFin);
                var historstions = PRVservice.GetEvaluationsEmployeeBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
                ViewBag.nbreEvaluations = historstions.Count();
                foreach (var item in historstions)
                {
                    EvaluationVecteurPlusViewModel test = new EvaluationVecteurPlusViewModel();
                    test.Id = item.Id;
                    test.presentation = item.presentation;
                    test.objetAppel = item.objetAppel;
                    test.validationDisponibilite = item.validationDisponibilite;
                    test.questionnement = item.questionnement;
                    test.reformulation = item.reformulation;
                    test.propositionPRV = item.propositionPRV;
                    test.objections = item.objections;
                    test.calendrier = item.calendrier;
                    test.validation = item.validation;
                    test.recapitulatif = item.recapitulatif;
                    test.discours = item.discours;
                    test.attitude = item.attitude;
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
                    return PartialView("HistoriqueTablePRVAgentPartialView", a);
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Data);
                return PartialView("SelectError");
            }

            return RedirectToAction("HistoriqueAgent_PRV");
        }
        public ActionResult ReponseEvaluationPRV(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GrilleEvaluationPRV evaluation = db.GrilleEvaluationPRVs.Find(id);
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
        [HttpPost, ActionName("ReponseEvaluationPRV")]
        [Authorize(Roles = "Agent_PRV")]
        public ActionResult ReponseEvaluationPRV(int? id, GrilleEvaluationPRV evaluation)
        {
            if (ModelState.IsValid)
            {

                db.Entry(evaluation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("HistoriqueAgent_PRV");
            }
            return View(evaluation);
        }
        #endregion
        #region Reportings PRV
        public ActionResult ReportsPRV()
        {
            List<Employee> logins = new List<Employee>();
            EvaluationViewModel evaluation = new EvaluationViewModel();
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 5))
            {
                ViewBag.role = "Agent Qualité";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 4))
            {
                ViewBag.role = "Qualité";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 2014))
            {
                ViewBag.role = "Agent Qualité_PRV";
            }

            string d = "COMONLINE";
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
                if (!test.UserName.Equals(user.UserName) && test.Roles.Any(r => r.UserId == test.Id && r.RoleId == 2013))
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

        public JsonResult GetReportsPRV(string username, string dateDebut, string dateFin)
        {
            float totpresentation = 0, totObjetAppel = 0, totvalidationDisponibilite = 0, totquestionnement = 0, totreformulation = 0,
                  totpropositionPRV = 0, totobjections = 0, totcalendrier = 0, totvalidation = 0, totrecapitulatif = 0,
                  totDiscours = 0, totAttitude = 0, totPriseCongé = 0, totNotes = 0;

            float NbrevalidationDisponibilite = 0, Nbrereformulation = 0, NbrepropositionPRV = 0, Nbreobjections = 0,
                Nbrecalendrier = 0, Nbrevalidation = 0, Nbrerecapitulatifs = 0, NbreDiscours = 0;

            DateTime daterecherchedebut = DateTime.Parse(dateDebut);
            DateTime daterecherchefin = DateTime.Parse(dateFin);

            var historstions = PRVservice.GetEvaluationsBetweenTwoDates(daterecherchedebut, daterecherchefin);
            if (username != "")
            {
                Employee emp = serviceEmployee.getByLogin(username);
                historstions = PRVservice.GetEvaluationsEmployeeBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
            }
            float nbreEvaluations = historstions.Count();
            ViewBag.nbreEvaluations = nbreEvaluations;
            foreach (var item in historstions)
            {
                totNotes += item.note;
                totpresentation += item.presentation;
                totObjetAppel += item.objetAppel;
                totquestionnement += item.questionnement;

                if (item.validationDisponibilite >= 0)
                {
                    totvalidationDisponibilite += item.validationDisponibilite;
                    NbrevalidationDisponibilite += 1;
                }
                if (item.reformulation >= 0)
                {
                    totreformulation += item.reformulation;
                    Nbrereformulation += 1;
                }
                if (item.propositionPRV >= 0)
                {
                    totpropositionPRV += item.propositionPRV;
                    NbrepropositionPRV += 1;
                }
                if (item.objections >= 0)
                {
                    totobjections += item.objections;
                    Nbreobjections += 1;
                }
                if (item.calendrier >= 0)
                {
                    totcalendrier += item.calendrier;
                    Nbrecalendrier += 1;
                }
                if (item.validation >= 0)
                {
                    totvalidation += item.validation;
                    Nbrevalidation += 1;
                }
                if (item.recapitulatif >= 0)
                {
                    totrecapitulatif += item.recapitulatif;
                    Nbrerecapitulatifs += 1;
                }
                if (item.discours >= 0)
                {
                    totDiscours += item.discours;
                    NbreDiscours += 1;
                }
               
                totAttitude += item.attitude;
                totPriseCongé += item.priseConge;
            }

            EvaluationVecteurPlusViewModel test = new EvaluationVecteurPlusViewModel();

            test.nbreEvaluations = Convert.ToInt32(nbreEvaluations);
            if (NbrevalidationDisponibilite != 0)
            {
                test.validationDisponibilite = (float)Math.Round((totvalidationDisponibilite / (NbrevalidationDisponibilite * 4)) * 100, 2);
            }
            else { test.validationDisponibilite = -4; }
            if (Nbrereformulation != 0)
            {
                test.reformulation = (float)Math.Round((totreformulation / (Nbrereformulation * 4)) * 100, 2);
            }
            else { test.reformulation = -4; }
            if (NbrepropositionPRV != 0)
            {
                test.propositionPRV = (float)Math.Round((totpropositionPRV / (NbrepropositionPRV * 6)) * 100, 2);
            }
            else { test.propositionPRV = -6; }
            if (Nbreobjections != 0)
            {
                test.objections = (float)Math.Round((totobjections / (Nbreobjections * 4)) * 100, 2);
            }
            else { test.objections = -4; }
            if (Nbrecalendrier != 0)
            {
                test.calendrier = (float)Math.Round((totcalendrier / (Nbrecalendrier * 4)) * 100, 2);
            }
            else { test.calendrier = -4; }
            if (Nbrevalidation != 0)
            {
                test.validation = (float)Math.Round((totvalidation / (Nbrevalidation * 4)) * 100, 2);
            }
            else { test.validation = -4; }
            if (Nbrerecapitulatifs != 0)
            {
                test.recapitulatif = (float)Math.Round((totrecapitulatif / (Nbrerecapitulatifs * 6)) * 100, 2);
            }
            else { test.recapitulatif = -6; }
            if (NbreDiscours != 0)
            {
                test.discours = (float)Math.Round((totDiscours / (NbreDiscours * 5)) * 100, 2);
            }
            else { test.discours = -5; }
          
            if (nbreEvaluations != 0)
            {
                test.note = (float)Math.Round((totNotes / (nbreEvaluations * 100)) * 100, 2);
                test.presentation = (float)Math.Round((totpresentation / (nbreEvaluations * 4)) * 100, 2);
                test.objetAppel = (float)Math.Round((totObjetAppel / (nbreEvaluations * 4)) * 100, 2);
                test.questionnement = (float)Math.Round((totquestionnement / (nbreEvaluations * 8)) * 100, 2);
                test.attitude = (float)Math.Round((totAttitude / (nbreEvaluations * 5)) * 100, 2);
                test.priseConge = (float)Math.Round((totPriseCongé / (nbreEvaluations * 4)) * 100, 2);
            }
            else
            {
                test.note = 0;
                test.presentationIdentification = 0;
                test.objetAppel = 0;
                test.discours = 0;
                test.attitude = 0;
                test.priseConge = 0;
            }
            return Json(test, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Edit et delete PRV
        [Authorize(Roles = "Qualité")]
        public ActionResult EditPRV(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GrilleEvaluationPRV evaluation = db.GrilleEvaluationPRVs.Find(id);
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
        [HttpPost, ActionName("EditPRV")]
        [Authorize(Roles = "Qualité")]
        public ActionResult EditPRV(int? id, GrilleEvaluationPRV evaluation)
        {
            float total = 62;
            List<float> NEList = new List<float>(new float[] { evaluation.validationDisponibilite, evaluation.reformulation, evaluation.propositionPRV, evaluation.objections, evaluation.calendrier, evaluation.validation, evaluation.recapitulatif, evaluation.discours });
            float notes = evaluation.presentation + evaluation.objetAppel + evaluation.questionnement + evaluation.attitude + evaluation.priseConge;

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
                return RedirectToAction("PRVHistorique");
            }
            return View(evaluation);
        }

        [Authorize(Roles = "Qualité,Agent Qualité_PRV")]
        [HttpGet]
        public ActionResult FindEvaluationPRV(int? Id)
        {
            GrilleEvaluationPRV item = PRVservice.getById(Id);

            if (Request.IsAjaxRequest())
            {
                return PartialView("_AlerteDeSuppressionPRV", item);
            }

            else
            {
                return View(item);
            }
        }

        [Authorize(Roles = "Qualité,Agent Qualité_PRV")]
        public ActionResult DeletePRV(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GrilleEvaluationPRV evaluation = PRVservice.getById(id);
            int? empId = evaluation.employeeId;
            PRVservice.DeleteEvaluations(id, empId);
            PRVservice.SaveChange();
            return RedirectToAction("PRVHistorique");
        }

        #endregion

        #region Edit et delete Typologie
        [Authorize(Roles = "Qualité")]

        public ActionResult EditTypologie(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GrilleEvaluationBPPTypologie evaluation = db.GrilleEvaluationBPPTypologies.Find(id);
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
        [HttpPost, ActionName("EditTypologie")]
        [Authorize(Roles = "Qualité")]
        public ActionResult EditTypologie(int? id, GrilleEvaluationBPPTypologie evaluation)
        {
            float total = 14;
            float totalRs = 14;
            List<float> NEList = new List<float>(new float[] { evaluation.nombreLogements, evaluation.nombreSociaux, evaluation.typeCommerce, evaluation.typeIndustrie, evaluation.typeEtablissementSante, evaluation.typeAcceuil, evaluation.capaciteAcceuilSante, evaluation.typeEtablissementHR, evaluation.nombreEtoile, evaluation.typeEnseignement, evaluation.capaciteAcceuilEnseignement, evaluation.typeLoisirs, evaluation.frigorifique, evaluation.plateformeLogistique });
            List<float> NEListRs = new List<float>(new float[] { evaluation.nombreLogementsRs, evaluation.nombreSociauxRs, evaluation.typeCommerceRs, evaluation.typeIndustrieRs, evaluation.typeEtablissementSanteRs, evaluation.typeAcceuilRs, evaluation.capaciteAcceuilSanteRs, evaluation.typeEtablissementHRRs, evaluation.nombreEtoileRs, evaluation.typeEnseignementRs, evaluation.capaciteAcceuilEnseignementRs, evaluation.typeLoisirsRs, evaluation.frigorifiqueRs, evaluation.plateformeLogistiqueRs });
            float notes = 0;
            float notesRs = 0;

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
            foreach (var i in NEListRs)
            {
                if (i < 0)
                {
                    totalRs += i;
                }
                else
                {
                    notesRs += i;
                }
            }
            if (ModelState.IsValid)
            {
                evaluation.note = (notes / total) * 100;
                evaluation.noteRs = (notesRs / totalRs) * 100;
                db.Entry(evaluation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("TypologieHistorique");
            }
            return View(evaluation);
        }
        [Authorize(Roles = "Qualité")]
        [HttpGet]
        public ActionResult FindEvaluationTypologie(int? Id)
        {
            GrilleEvaluationBPPTypologie item = BPPTypologieService.getById(Id);

            if (Request.IsAjaxRequest())
            {
                return PartialView("_AlerteDeSuppressionTypologie", item);
            }

            else
            {
                return View(item);
            }
        }
        [Authorize(Roles = "Qualité")]
        public ActionResult DeleteTypologie(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GrilleEvaluationBPPTypologie evaluation = BPPTypologieService.getById(id);
            int? empId = evaluation.employeeId;
            BPPTypologieService.DeleteEvaluations(id, empId);
            BPPTypologieService.SaveChange();
            return RedirectToAction("TypologieHistorique");
        }

        #endregion


        #region Reportings Typologie
        public ActionResult ReportsTypologie()
        {
            List<Employee> logins = new List<Employee>();
            EvaluationViewModel evaluation = new EvaluationViewModel();
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 5))
            {
                ViewBag.role = "Agent Qualité";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 4))
            {
                ViewBag.role = "Qualité";
            }

            string d = "COMONLINE";
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

        public JsonResult GetReportsTypologie(string username, string dateDebut, string dateFin)
        {
            float totNombreLogements = 0, totNombreSociaux = 0, totTypeCommerce = 0, totTypeIndustrie = 0, totTypeEtablissementSante = 0, totTypeAcceuil = 0, totCapaciteAcceuilSante = 0,
               totTypeEtablissementHR = 0, totNombreEtoile = 0, totTypeEnseignement = 0, totCapaciteAcceuilEnseignement = 0, totTypeLoisirs = 0, totFrigorifique = 0, totPlateformeLogistique = 0
               , totNombreLogementsRs = 0, totNombreSociauxRs = 0, totTypeCommerceRs = 0, totTypeIndustrieRs = 0, totTypeEtablissementSanteRs = 0, totTypeAcceuilRs = 0, totCapaciteAcceuilSanteRs = 0,
               totTypeEtablissementHRRs = 0, totNombreEtoileRs = 0, totTypeEnseignementRs = 0, totCapaciteAcceuilEnseignementRs = 0, totTypeLoisirsRs = 0, totFrigorifiqueRs = 0, totPlateformeLogistiqueRs = 0,
               totNotes = 0, totNotesRs = 0; ;               

            float NbreNombreLogements = 0, NbreNombreSociaux = 0, NbreTypeCommerce = 0, NbreTypeIndustrie = 0, NbreTypeEtablissementSante = 0, NbreTypeAcceuil = 0, NbreCapaciteAcceuilSante = 0,
               NbreTypeEtablissementHR = 0, NbreNombreEtoile = 0, NbreTypeEnseignement = 0, NbreCapaciteAcceuilEnseignement = 0, NbreTypeLoisirs = 0, NbreFrigorifique = 0, NbrePlateformeLogistique = 0
               , NbreNombreLogementsRs = 0, NbreNombreSociauxRs = 0, NbreTypeCommerceRs = 0, NbreTypeIndustrieRs = 0, NbreTypeEtablissementSanteRs = 0, NbreTypeAcceuilRs = 0, NbreCapaciteAcceuilSanteRs = 0,
               NbreTypeEtablissementHRRs = 0, NbreNombreEtoileRs = 0, NbreTypeEnseignementRs = 0, NbreCapaciteAcceuilEnseignementRs = 0, NbreTypeLoisirsRs = 0, NbreFrigorifiqueRs = 0, NbrePlateformeLogistiqueRs = 0;

            DateTime daterecherchedebut = DateTime.Parse(dateDebut);
            DateTime daterecherchefin = DateTime.Parse(dateFin);

            var historstionsTypologie = BPPTypologieService.GetEvaluationsBetweenTwoDates(daterecherchedebut, daterecherchefin);
            if (username != "")
            {
                Employee emp = serviceEmployee.getByLogin(username);
                historstionsTypologie = BPPTypologieService.GetEvaluationsEmployeeBetweenTwoDates(emp.Id, daterecherchedebut, daterecherchefin);
            }
            float nbreEvaluations = historstionsTypologie.Count();
            ViewBag.nbreEvaluations = nbreEvaluations;
            foreach (var item in historstionsTypologie)
            {
                totNotes += item.note;
                totNotesRs += item.noteRs;

                if (item.nombreLogements >= 0)
                {
                    totNombreLogements += item.nombreLogements;
                    NbreNombreLogements += 1;
                }

                if (item.nombreSociaux >= 0)
                {
                    totNombreSociaux += item.nombreSociaux;
                    NbreNombreSociaux += 1;
                }

                if (item.typeCommerce >= 0)
                {
                    totTypeCommerce += item.typeCommerce;
                    NbreTypeCommerce += 1;
                }

                if (item.typeIndustrie >= 0)
                {
                    totTypeIndustrie += item.typeIndustrie;
                    NbreTypeIndustrie += 1;
                }

                if (item.typeEtablissementSante >= 0)
                {
                    totTypeEtablissementSante += item.typeEtablissementSante;
                    NbreTypeEtablissementSante += 1;
                }
                if (item.typeAcceuil >= 0)
                {
                    totTypeAcceuil += item.typeAcceuil;
                    NbreTypeAcceuil += 1;
                }
                if (item.capaciteAcceuilSante >= 0)
                {
                    totCapaciteAcceuilSante += item.capaciteAcceuilSante;
                    NbreCapaciteAcceuilSante += 1;
                }
                if (item.typeEtablissementHR >= 0)
                {
                    totTypeEtablissementHR += item.typeEtablissementHR;
                    NbreTypeEtablissementHR += 1;
                }
                if (item.nombreEtoile >= 0)
                {
                    totNombreEtoile += item.nombreEtoile;
                    NbreNombreEtoile += 1;
                }
                if (item.typeEnseignement >= 0)
                {
                    totTypeEnseignement += item.typeEnseignement;
                    NbreTypeEnseignement += 1;
                }
                if (item.capaciteAcceuilEnseignement >= 0)
                {
                    totCapaciteAcceuilEnseignement += item.capaciteAcceuilEnseignement;
                    NbreCapaciteAcceuilEnseignement += 1;
                }
                if (item.typeLoisirs >= 0)
                {
                    totTypeLoisirs += item.typeLoisirs;
                    NbreTypeLoisirs += 1;
                }

                if (item.frigorifique >= 0)
                {
                    totFrigorifique += item.frigorifique;
                    NbreFrigorifique += 1;
                }
                if (item.plateformeLogistique >= 0)
                {
                    totPlateformeLogistique += item.plateformeLogistique;
                    NbrePlateformeLogistique += 1;
                }

                //

                if (item.nombreLogementsRs >= 0)
                {
                    totNombreLogementsRs += item.nombreLogementsRs;
                    NbreNombreLogementsRs += 1;
                }

                if (item.nombreSociauxRs >= 0)
                {
                    totNombreSociauxRs += item.nombreSociauxRs;
                    NbreNombreSociauxRs += 1;
                }

                if (item.typeCommerceRs >= 0)
                {
                    totTypeCommerceRs += item.typeCommerceRs;
                    NbreTypeCommerceRs += 1;
                }

                if (item.typeIndustrieRs >= 0)
                {
                    totTypeIndustrieRs += item.typeIndustrieRs;
                    NbreTypeIndustrieRs += 1;
                }

                if (item.typeEtablissementSanteRs >= 0)
                {
                    totTypeEtablissementSanteRs += item.typeEtablissementSanteRs;
                    NbreTypeEtablissementSanteRs += 1;
                }
                if (item.typeAcceuilRs >= 0)
                {
                    totTypeAcceuilRs += item.typeAcceuilRs;
                    NbreTypeAcceuilRs += 1;
                }
                if (item.capaciteAcceuilSanteRs >= 0)
                {
                    totCapaciteAcceuilSanteRs += item.capaciteAcceuilSanteRs;
                    NbreCapaciteAcceuilSanteRs += 1;
                }
                if (item.typeEtablissementHRRs >= 0)
                {
                    totTypeEtablissementHRRs += item.typeEtablissementHRRs;
                    NbreTypeEtablissementHRRs += 1;
                }
                if (item.nombreEtoileRs >= 0)
                {
                    totNombreEtoileRs += item.nombreEtoileRs;
                    NbreNombreEtoileRs += 1;
                }
                if (item.typeEnseignementRs >= 0)
                {
                    totTypeEnseignementRs += item.typeEnseignementRs;
                    NbreTypeEnseignementRs += 1;
                }
                if (item.capaciteAcceuilEnseignementRs >= 0)
                {
                    totCapaciteAcceuilEnseignementRs += item.capaciteAcceuilEnseignementRs;
                    NbreCapaciteAcceuilEnseignementRs += 1;
                }
                if (item.typeLoisirsRs >= 0)
                {
                    totTypeLoisirsRs += item.typeLoisirsRs;
                    NbreTypeLoisirsRs += 1;
                }

                if (item.frigorifiqueRs >= 0)
                {
                    totFrigorifiqueRs += item.frigorifiqueRs;
                    NbreFrigorifiqueRs += 1;
                }
                if (item.plateformeLogistiqueRs >= 0)
                {
                    totPlateformeLogistiqueRs += item.plateformeLogistiqueRs;
                    NbrePlateformeLogistiqueRs += 1;
                }


            }

            EvaluationVecteurPlusViewModel test = new EvaluationVecteurPlusViewModel();
            test.nbreEvaluations = Convert.ToInt32(nbreEvaluations);
            if (NbreNombreLogements != 0)
            {
                test.nombreLogements = (float)Math.Round((totNombreLogements / NbreNombreLogements) * 100, 2);
            }
            else { test.nombreLogements = -1; }

            if (NbreNombreSociaux != 0)
            {
                test.nombreSociaux = (float)Math.Round((totNombreSociaux / NbreNombreSociaux) * 100, 2);
            }
            else { test.nombreSociaux = -1; }

            if (NbreTypeCommerce != 0)
            {
                test.typeCommerce = (float)Math.Round((totTypeCommerce / NbreTypeCommerce) * 100, 2);
            }
            else { test.typeCommerce = -1; }

            if (NbreTypeIndustrie != 0)
            {
                test.typeIndustrie = (float)Math.Round((totTypeIndustrie / NbreTypeIndustrie) * 100, 2);
            }
            else { test.typeIndustrie = -1; }

            if (NbreTypeEtablissementSante != 0)
            {
                test.typeEtablissementSante = (float)Math.Round((totTypeEtablissementSante / NbreTypeEtablissementSante) * 100, 2);
            }
            else { test.typeEtablissementSante = -1; }

            if (NbreTypeAcceuil != 0)
            {
                test.typeAcceuil = (float)Math.Round((totTypeAcceuil / NbreTypeAcceuil) * 100, 2);
            }
            else { test.typeAcceuil = -1; }

            if (NbreCapaciteAcceuilSante != 0)
            {
                test.capaciteAcceuilSante = (float)Math.Round((totCapaciteAcceuilSante / NbreCapaciteAcceuilSante) * 100, 2);
            }
            else { test.capaciteAcceuilSante = -1; }

            if (NbreTypeEtablissementHR != 0)
            {
                test.typeEtablissementHR = (float)Math.Round((totTypeEtablissementHR / NbreTypeEtablissementHR) * 100, 2);
            }
            else { test.typeEtablissementHR = -1; }

            if (NbreNombreEtoile != 0)
            {
                test.nombreEtoile = (float)Math.Round((totNombreEtoile / NbreNombreEtoile) * 100, 2);
            }
            else { test.nombreEtoile = -1; }

            if (NbreTypeEnseignement != 0)
            {
                test.typeEnseignement = (float)Math.Round((totTypeEnseignement / NbreTypeEnseignement) * 100, 2);
            }
            else { test.typeEnseignement = -1; }

            if (NbreCapaciteAcceuilEnseignement != 0)
            {
                test.capaciteAcceuilEnseignement = (float)Math.Round((totCapaciteAcceuilEnseignement / NbreCapaciteAcceuilEnseignement) * 100, 2);
            }
            else { test.capaciteAcceuilEnseignement = -1; }

            if (NbreTypeLoisirs != 0)
            {
                test.typeLoisirs = (float)Math.Round((totTypeLoisirs / NbreTypeLoisirs) * 100, 2);
            }
            else { test.typeLoisirs = -1; }

            if (NbreFrigorifique != 0)
            {
                test.frigorifique = (float)Math.Round((totFrigorifique / NbreFrigorifique) * 100, 2);
            }
            else { test.frigorifique = -1; }

            if (NbrePlateformeLogistique != 0)
            {
                test.plateformeLogistique = (float)Math.Round((totPlateformeLogistique / NbrePlateformeLogistique) * 100, 2);
            }
            else { test.plateformeLogistique = -1; }

            if (nbreEvaluations != 0)
            {
                test.note = (float)Math.Round((totNotes / (nbreEvaluations * 100)) * 100, 2);
            }
            else
            {
                test.note = 0;
            }

            //
            if (NbreNombreLogementsRs != 0)
            {
                test.nombreLogementsRs = (float)Math.Round((totNombreLogementsRs / NbreNombreLogementsRs) * 100, 2);
            }
            else { test.nombreLogementsRs = -1; }

            if (NbreNombreSociauxRs != 0)
            {
                test.nombreSociauxRs = (float)Math.Round((totNombreSociauxRs / NbreNombreSociauxRs) * 100, 2);
            }
            else { test.nombreSociauxRs = -1; }

            if (NbreTypeCommerceRs != 0)
            {
                test.typeCommerceRs = (float)Math.Round((totTypeCommerceRs / NbreTypeCommerceRs) * 100, 2);
            }
            else { test.typeCommerceRs = -1; }

            if (NbreTypeIndustrieRs != 0)
            {
                test.typeIndustrieRs = (float)Math.Round((totTypeIndustrieRs / NbreTypeIndustrieRs) * 100, 2);
            }
            else { test.typeIndustrieRs = -1; }

            if (NbreTypeEtablissementSanteRs != 0)
            {
                test.typeEtablissementSanteRs = (float)Math.Round((totTypeEtablissementSanteRs / NbreTypeEtablissementSanteRs) * 100, 2);
            }
            else { test.typeEtablissementSanteRs = -1; }

            if (NbreTypeAcceuilRs != 0)
            {
                test.typeAcceuilRs = (float)Math.Round((totTypeAcceuilRs / NbreTypeAcceuilRs) * 100, 2);
            }
            else { test.typeAcceuilRs = -1; }

            if (NbreCapaciteAcceuilSanteRs != 0)
            {
                test.capaciteAcceuilSanteRs = (float)Math.Round((totCapaciteAcceuilSanteRs / NbreCapaciteAcceuilSanteRs) * 100, 2);
            }
            else { test.capaciteAcceuilSanteRs = -1; }

            if (NbreTypeEtablissementHRRs != 0)
            {
                test.typeEtablissementHRRs = (float)Math.Round((totTypeEtablissementHRRs / NbreTypeEtablissementHRRs) * 100, 2);
            }
            else { test.typeEtablissementHRRs = -1; }

            if (NbreNombreEtoileRs != 0)
            {
                test.nombreEtoileRs = (float)Math.Round((totNombreEtoileRs / NbreNombreEtoileRs) * 100, 2);
            }
            else { test.nombreEtoileRs = -1; }

            if (NbreTypeEnseignementRs != 0)
            {
                test.typeEnseignementRs = (float)Math.Round((totTypeEnseignementRs / NbreTypeEnseignementRs) * 100, 2);
            }
            else { test.typeEnseignementRs = -1; }

            if (NbreCapaciteAcceuilEnseignementRs != 0)
            {
                test.capaciteAcceuilEnseignementRs = (float)Math.Round((totCapaciteAcceuilEnseignementRs / NbreCapaciteAcceuilEnseignementRs) * 100, 2);
            }
            else { test.capaciteAcceuilEnseignementRs = -1; }

            if (NbreTypeLoisirsRs != 0)
            {
                test.typeLoisirsRs = (float)Math.Round((totTypeLoisirsRs / NbreTypeLoisirsRs) * 100, 2);
            }
            else { test.typeLoisirsRs = -1; }

            if (NbreFrigorifiqueRs != 0)
            {
                test.frigorifiqueRs = (float)Math.Round((totFrigorifiqueRs / NbreFrigorifiqueRs) * 100, 2);
            }
            else { test.frigorifiqueRs = -1; }

            if (NbrePlateformeLogistiqueRs != 0)
            {
                test.plateformeLogistiqueRs = (float)Math.Round((totPlateformeLogistiqueRs / NbrePlateformeLogistiqueRs) * 100, 2);
            }
            else { test.plateformeLogistiqueRs = -1; }

            if (nbreEvaluations != 0)
            {
                test.noteRs = (float)Math.Round((totNotesRs / (nbreEvaluations * 100)) * 100, 2);
            }
            else
            {
                test.noteRs = 0;
            }
            //
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
