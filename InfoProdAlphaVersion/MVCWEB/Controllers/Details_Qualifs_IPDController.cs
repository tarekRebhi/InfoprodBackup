using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Data;
using MVCWEB.Models;
using Services;
using Domain.Entity;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace MVCWEB.Controllers
{
    [Authorize(Roles = "Qualité,Manager,SuperManager,ManagerIPD")]
    public class Details_Qualifs_IPDController : Controller
    {
        private ReportContext db = new ReportContext();
        IGroupeEmployeeService serviceGroupeEmp;
        IEmployeeService service;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
   
        public Details_Qualifs_IPDController()
        {
            service = new EmployeeService();
            serviceGroupeEmp = new GroupesEmployeService();
   
        }
        public Details_Qualifs_IPDController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            RoleManager = roleManager;
        }
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
        // View IPD Global
        [HttpGet]
        public ActionResult IPD_Global()
        {
            try
            {
                Employee empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

                var a = new EvaluationViewModel();
                a.pseudoNameEmp = empConnected.pseudoName;
                a.userName = empConnected.UserName;
                if (empConnected.Content != null)
                {
                    String strbase64 = Convert.ToBase64String(empConnected.Content);
                    String Url = "data:" + empConnected.ContentType + ";base64," + strbase64;
                    ViewBag.url = Url;
                    a.Url = Url;

                }
                return View(a);
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e);
                return RedirectToAction("Connect", "Authentification");

            }
        }
        // Calcul IPD Global 
        public JsonResult GetIPDGlobalValues(String dateSel)
        {
            var data = db.Details_Activite_IPD.ToList();
            var appels = db.appels.Where(a => a.Customer_Id == 2).ToList();
            List<qualification> qualifs = new List<qualification>();
            var ListQualiif = new List<SelectListItem>();
            DateTime from = DateTime.Parse(dateSel.Substring(0, 10));
            DateTime to = DateTime.Parse(dateSel.Substring(13, 10));
            IList<int?> ListCodesClotures = new List<int?>() { 1, 2, 3, 4, 5, 6, 91, 99 }; double fichescrees = 0;
            double totApp = 0;
            double totAboutis = 0;
            double fichestraitees = 0;
            double fichesclotures = 0;
            double fichesCA = 0;
            double PercentExploitation = 0;
            double PercentAccord = 0;
            int nbre1 = 0; int nbre2 = 0; int nbre3 = 0; int nbre4 = 0; int nbre5 = 0; int nbre6 = 0; int nbre7 = 0; int nbre8 = 0; int nbre9 = 0; int nbre10 = 0;
            int nbre11 = 0; int nbre12 = 0; int nbre13 = 0; int nbre14 = 0;
            string type1 = "CA Positif";
            string type2 = "CA Négatif";
            string type3 = "Système";

            foreach (var app in appels)
            {
                //string d = app.DATE.Substring(0, 4) + '-' + app.DATE.Substring(4, 2) + '-' + app.DATE.Substring(6, 2);
                //DateTime date = Convert.ToDateTime(d);
                if (app.date >= from && app.date <= to)
                {
                    totApp += app.TotalAppelEmis;
                    totAboutis += app.TotalAppelAboutis;
                }
            }

            foreach (var item in data)
            {
                fichescrees += 1;
                if (item.DATE != null)
                {
                    string d = item.DATE.Substring(0, 4) + '-' + item.DATE.Substring(4, 2) + '-' + item.DATE.Substring(6, 2);
                    DateTime date = Convert.ToDateTime(d);
                    if (date >= from && date <= to)
                    {
                        if (item.STATUS != null)
                        {
                            fichestraitees += 1;
                        }
                        if (ListCodesClotures.Contains(item.STATUS))
                        {
                            fichesclotures += 1;
                        }
                        if (item.Argued == true)
                        {
                            fichesCA += 1;
                        }
                        if (item.Positive == true)
                        {
                            nbre1 += 1;
                        }
                        switch (item.STATUS)
                        {
                            case 2:
                                nbre2 += 1;
                                break;
                            case 3:
                                nbre3 += 1;
                                break;
                            case 4:
                                nbre4 += 1;
                                break;
                            case 5:
                                nbre5 += 1;
                                break;
                            case 6:
                                nbre6 += 1;
                                break;
                            case 90:
                                nbre7 += 1;
                                break;
                            case 91:
                                nbre8 += 1;
                                break;
                            case 92:
                                nbre9 += 1;
                                break;
                            case 93:
                                nbre10 += 1;
                                break;
                            case 94:
                                nbre11 += 1;
                                break;
                            case 95:
                                nbre12 += 1;
                                break;
                            case 98:
                                nbre13 += 1;
                                break;
                            case 99:
                                nbre14 += 1;
                                break;
                        }
                    }
                }
            }
            if (fichesCA != 0)
            {
                PercentAccord = Math.Round((nbre1 / fichesCA) * 100, 2);
            }

            if (fichestraitees != 0)
            {
                qualifs.Add(new qualification { nom = "OK Démo", type = type1, nombre = nbre1, pourcentage = Math.Round((nbre1 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "KO Démo", type = type2, nombre = nbre2, pourcentage = Math.Round((nbre2 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "Barrage", type = type2, nombre = nbre3, pourcentage = Math.Round((nbre3 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "SND", type = type2, nombre = nbre4, pourcentage = Math.Round((nbre4 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "OPT OUT", type = type2, nombre = nbre5, pourcentage = Math.Round((nbre5 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "Hors Cible", type = type2, nombre = nbre6, pourcentage = Math.Round((nbre6 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "Occupé", type = type3, nombre = nbre7, pourcentage = Math.Round((nbre7 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "Faux numéro", type = type3, nombre = nbre8, pourcentage = Math.Round((nbre8 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "NRP", type = type3, nombre = nbre9, pourcentage = Math.Round((nbre9 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "Répondeur", type = type3, nombre = nbre10, pourcentage = Math.Round((nbre10 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "Rappel personnel", type = type3, nombre = nbre11, pourcentage = Math.Round((nbre11 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "Relance", type = type3, nombre = nbre12, pourcentage = Math.Round((nbre12 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "recycled record", type = type3, nombre = nbre13, pourcentage = Math.Round((nbre13 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "Limite Inaccessible", type = type3, nombre = nbre14, pourcentage = Math.Round((nbre14 / fichestraitees) * 100, 2) });
            }
            else
            {

                qualifs.Add(new qualification { nom = "OK Démo", type = type1, nombre = nbre1, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "KO Démo", type = type2, nombre = nbre2, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Barrage", type = type2, nombre = nbre3, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "SND", type = type2, nombre = nbre4, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "OPT OUT", type = type2, nombre = nbre5, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Hors Cible", type = type2, nombre = nbre6, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Occupé", type = type3, nombre = nbre7, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Faux numéro", type = type3, nombre = nbre8, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "NRP", type = type3, nombre = nbre9, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Répondeur", type = type3, nombre = nbre10, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Rappel personnel", type = type3, nombre = nbre11, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Relance", type = type3, nombre = nbre12, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "recycled record", type = type3, nombre = nbre13, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Limite Inaccessible", type = type3, nombre = nbre14, pourcentage = 0 });
            }
            if (fichescrees != 0)
            {
                PercentExploitation = Math.Round((fichesclotures / fichescrees) * 100, 2);
            }
            qualifs.Add(new qualification { nom = "Fiche Crées", nombre = fichescrees, pourcentage = 100 });
            qualifs.Add(new qualification { nom = "Total Appels", nombre = fichesclotures });
            qualifs.Add(new qualification { nom = "Appels Aboutis ", nombre = fichesCA });
            qualifs.Add(new qualification { nom = "Fiches Cloturées", nombre = fichesclotures, pourcentage = PercentExploitation });
            qualifs.Add(new qualification { nom = "Contacts Argumentés ", nombre = fichesCA, pourcentage = PercentAccord });
            qualifs.Add(new qualification { nom = "Fiches Traitées", nombre = fichestraitees });
            return Json(qualifs, JsonRequestBehavior.AllowGet);
        }

        // View IPD Par Operation
        [HttpGet]
        public ActionResult IPD_ParOperation()
        {
            try
            {
                Employee empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
                var data = db.Details_Activite_IPD.ToList();
                List<string> AllTitres = data.Select(gp => gp.NOM_OPERATION).Distinct().ToList();
                ViewBag.AllTitres = AllTitres;
                var a = new EvaluationViewModel();
                a.pseudoNameEmp = empConnected.pseudoName;
                a.userName = empConnected.UserName;
                if (empConnected.Content != null)
                {
                    String strbase64 = Convert.ToBase64String(empConnected.Content);
                    String Url = "data:" + empConnected.ContentType + ";base64," + strbase64;
                    ViewBag.url = Url;
                    a.Url = Url;

                }
                return View(a);
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e);
                return RedirectToAction("Connect", "Authentification");

            }
        }

        // Calcul IPD Par Operation 
        public JsonResult GetIPDParOperationValues(String TitreOperation, String dateSel)
        {
            var data = db.Details_Activite_IPD.Where(d => d.NOM_OPERATION == TitreOperation).ToList();
            var appels = db.appels.Where(a => a.Customer_Id == 2).ToList();
            List<qualification> qualifs = new List<qualification>();
            var ListQualiif = new List<SelectListItem>();
            DateTime from = DateTime.Parse(dateSel.Substring(0, 10));
            DateTime to = DateTime.Parse(dateSel.Substring(13, 10));
            IList<int?> ListCodesClotures = new List<int?>() { 1, 2, 3, 4, 5, 6, 91, 99 };
            double fichescrees = 0;
            double totApp = 0;
            double totAboutis = 0;
            double fichesclotures = 0;
            double fichestraitees = 0;
            double fichesCA = 0;
            double PercentExploitation = 0;
            double PercentAccord = 0;
            int nbre1 = 0; int nbre2 = 0; int nbre3 = 0; int nbre4 = 0; int nbre5 = 0; int nbre6 = 0; int nbre7 = 0; int nbre8 = 0; int nbre9 = 0; int nbre10 = 0;
            int nbre11 = 0; int nbre12 = 0; int nbre13 = 0; int nbre14 = 0;
            string type1 = "CA Positif";
            string type2 = "CA Négatif";
            string type3 = "Système";

            foreach (var app in appels)
            {
                if (app.date >= from && app.date <= to && app.nomCompagne == TitreOperation)
                {
                    totApp += app.TotalAppelEmis;
                    totAboutis += app.TotalAppelAboutis;
                }
            }
            foreach (var item in data)
            {
                fichescrees += 1;
                if (item.DATE != null)
                {
                    string d = item.DATE.Substring(0, 4) + '-' + item.DATE.Substring(4, 2) + '-' + item.DATE.Substring(6, 2);
                    DateTime date = Convert.ToDateTime(d);
                    if (date >= from && date <= to)
                    {
                        if (item.STATUS != null)
                        {
                            fichestraitees += 1;
                        }
                        if (ListCodesClotures.Contains(item.STATUS))
                        {
                            fichesclotures += 1;
                        }
                        if (item.Argued == true)
                        {
                            fichesCA += 1;
                        }
                        if (item.Positive == true)
                        {
                            nbre1 += 1;
                        }
                        switch (item.STATUS)
                        {
                            case 2:
                                nbre2 += 1;
                                break;
                            case 3:
                                nbre3 += 1;
                                break;
                            case 4:
                                nbre4 += 1;
                                break;
                            case 5:
                                nbre5 += 1;
                                break;
                            case 6:
                                nbre6 += 1;
                                break;
                            case 90:
                                nbre7 += 1;
                                break;
                            case 91:
                                nbre8 += 1;
                                break;
                            case 92:
                                nbre9 += 1;
                                break;
                            case 93:
                                nbre10 += 1;
                                break;
                            case 94:
                                nbre11 += 1;
                                break;
                            case 95:
                                nbre12 += 1;
                                break;
                            case 98:
                                nbre13 += 1;
                                break;
                            case 99:
                                nbre14 += 1;
                                break;
                        }
                    }
                }
            }
            if (fichesCA != 0)
            {
                PercentAccord = Math.Round((nbre1 / fichesCA) * 100, 2);
            }

            if (fichestraitees != 0)
            {
                qualifs.Add(new qualification { nom = "OK Démo", type = type1, nombre = nbre1, pourcentage = Math.Round((nbre1 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "KO Démo", type = type2, nombre = nbre2, pourcentage = Math.Round((nbre2 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "Barrage", type = type2, nombre = nbre3, pourcentage = Math.Round((nbre3 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "SND", type = type2, nombre = nbre4, pourcentage = Math.Round((nbre4 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "OPT OUT", type = type2, nombre = nbre5, pourcentage = Math.Round((nbre5 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "Hors Cible", type = type2, nombre = nbre6, pourcentage = Math.Round((nbre6 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "Occupé", type = type3, nombre = nbre7, pourcentage = Math.Round((nbre7 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "Faux numéro", type = type3, nombre = nbre8, pourcentage = Math.Round((nbre8 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "NRP", type = type3, nombre = nbre9, pourcentage = Math.Round((nbre9 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "Répondeur", type = type3, nombre = nbre10, pourcentage = Math.Round((nbre10 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "Rappel personnel", type = type3, nombre = nbre11, pourcentage = Math.Round((nbre11 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "Relance", type = type3, nombre = nbre12, pourcentage = Math.Round((nbre12 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "recycled record", type = type3, nombre = nbre13, pourcentage = Math.Round((nbre13 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "Limite Inaccessible", type = type3, nombre = nbre14, pourcentage = Math.Round((nbre14 / fichestraitees) * 100, 2) });
            }
            else
            {

                qualifs.Add(new qualification { nom = "OK Démo", type = type1, nombre = nbre1, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "KO Démo", type = type2, nombre = nbre2, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Barrage", type = type2, nombre = nbre3, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "SND", type = type2, nombre = nbre4, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "OPT OUT", type = type2, nombre = nbre5, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Hors Cible", type = type2, nombre = nbre6, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Occupé", type = type3, nombre = nbre7, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Faux numéro", type = type3, nombre = nbre8, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "NRP", type = type3, nombre = nbre9, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Répondeur", type = type3, nombre = nbre10, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Rappel personnel", type = type3, nombre = nbre11, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Relance", type = type3, nombre = nbre12, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "recycled record", type = type3, nombre = nbre13, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Limite Inaccessible", type = type3, nombre = nbre14, pourcentage = 0 });
            }
            if (fichescrees != 0)
            {
                PercentExploitation = Math.Round((fichesclotures / fichescrees) * 100, 2);
            }
            qualifs.Add(new qualification { nom = "Fiche Crées", nombre = fichescrees, pourcentage = 100 });
            qualifs.Add(new qualification { nom = "Total Appels", nombre = totApp });
            qualifs.Add(new qualification { nom = "Appels Aboutis ", nombre = totAboutis });
            qualifs.Add(new qualification { nom = "Fiches Cloturées", nombre = fichesclotures, pourcentage = PercentExploitation });
            qualifs.Add(new qualification { nom = "Contacts Argumentés ", nombre = fichesCA, pourcentage = PercentAccord });
            qualifs.Add(new qualification { nom = "Appels Traitées", nombre = fichestraitees });
            return Json(qualifs, JsonRequestBehavior.AllowGet);
        }

        // View IPD Par Agent
        [HttpGet]
        public ActionResult IPD_ParAgent()
        {
            try
            {
                Employee empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            
                List<SelectListItem> employees = new List<SelectListItem>();
                List<Employee> emps = serviceGroupeEmp.getListEmployeeByGroupeId(15);
                foreach (var e in emps)
                {
                    string IdHermes = e.IdHermes.ToString();
                    if (e.Id != empConnected.Id && e.Roles.Any(r => r.RoleId == 3))
                    {
                        if (!(employees.Exists(x => x.Text == e.UserName && x.Value == IdHermes)))
                        {
                            employees.Add(new SelectListItem { Text = e.UserName, Value = IdHermes });
                        }
                    }
                }

                ViewBag.AgentItems = employees;
                var a = new EvaluationViewModel();
                a.pseudoNameEmp = empConnected.pseudoName;
                a.userName = empConnected.UserName;
                if (empConnected.Content != null)
                {
                    String strbase64 = Convert.ToBase64String(empConnected.Content);
                    String Url = "data:" + empConnected.ContentType + ";base64," + strbase64;
                    ViewBag.url = Url;
                    a.Url = Url;

                }
                return View(a);
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e);
                return RedirectToAction("Connect", "Authentification");

            }
        }

        // Calcul IPD Par Agent 
        public JsonResult GetIPDParAgentValues(int idHermes, String dateSel)
        {
            var data = db.Details_Activite_IPD.ToList();
            var appels = db.appels.Where(a => a.Customer_Id == 2).ToList();
            List<qualification> qualifs = new List<qualification>();
            var ListQualiif = new List<SelectListItem>();
            DateTime from = DateTime.Parse(dateSel.Substring(0, 10));
            DateTime to = DateTime.Parse(dateSel.Substring(13, 10));
            IList<int?> ListCodesClotures = new List<int?>() { 1, 2, 3, 4, 5, 6, 91, 99 };
            double fichescrees = 0;
            double totApp = 0;
            double totAboutis = 0;
            double fichesclotures = 0;
            double fichestraitees = 0;
            double fichesCA = 0;
            double PercentExploitation = 0;
            double PercentAccord = 0;
            int nbre1 = 0; int nbre2 = 0; int nbre3 = 0; int nbre4 = 0; int nbre5 = 0; int nbre6 = 0; int nbre7 = 0; int nbre8 = 0; int nbre9 = 0; int nbre10 = 0;
            int nbre11 = 0; int nbre12 = 0; int nbre13 = 0; int nbre14 = 0;
            string type1 = "CA Positif";
            string type2 = "CA Négatif";
            string type3 = "Système";

            foreach (var app in appels)
            {
                if (app.date >= from && app.date <= to && app.Id_Hermes == idHermes)
                {
                    totApp += app.TotalAppelEmis;
                    totAboutis += app.TotalAppelAboutis;
                }
            }

            foreach (var item in data)
            {
                fichescrees += 1;
                if (item.DATE != null)
                {
                    string d = item.DATE.Substring(0, 4) + '-' + item.DATE.Substring(4, 2) + '-' + item.DATE.Substring(6, 2);
                DateTime date = Convert.ToDateTime(d);
                if (date >= from && date <= to && item.ID_TV == idHermes)
                {
                        if (item.STATUS != null)
                        {
                            fichestraitees += 1;
                        }
                        if (ListCodesClotures.Contains(item.STATUS))
                        {
                            fichesclotures += 1;
                        }
                        if (item.Argued == true)
                        {
                            fichesCA += 1;
                        }
                        if (item.Positive == true)
                        {
                            nbre1 += 1;
                        }
                        switch (item.STATUS)
                        {
                            case 2:
                                nbre2 += 1;
                                break;
                            case 3:
                                nbre3 += 1;
                                break;
                            case 4:
                                nbre4 += 1;
                                break;
                            case 5:
                                nbre5 += 1;
                                break;
                            case 6:
                                nbre6 += 1;
                                break;
                            case 90:
                                nbre7 += 1;
                                break;
                            case 91:
                                nbre8 += 1;
                                break;
                            case 92:
                                nbre9 += 1;
                                break;
                            case 93:
                                nbre10 += 1;
                                break;
                            case 94:
                                nbre11 += 1;
                                break;
                            case 95:
                                nbre12 += 1;
                                break;
                            case 98:
                                nbre13 += 1;
                                break;
                            case 99:
                                nbre14 += 1;
                                break;
                        }
                    }
                }
            }
            if (fichesCA != 0)
            {
                PercentAccord = Math.Round((nbre1 / fichesCA) * 100, 2);
            }

            if (fichestraitees != 0)
            {
                qualifs.Add(new qualification { nom = "OK Démo", type = type1, nombre = nbre1, pourcentage = Math.Round((nbre1 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "KO Démo", type = type2, nombre = nbre2, pourcentage = Math.Round((nbre2 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "Barrage", type = type2, nombre = nbre3, pourcentage = Math.Round((nbre3 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "SND", type = type2, nombre = nbre4, pourcentage = Math.Round((nbre4 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "OPT OUT", type = type2, nombre = nbre5, pourcentage = Math.Round((nbre5 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "Hors Cible", type = type2, nombre = nbre6, pourcentage = Math.Round((nbre6 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "Occupé", type = type3, nombre = nbre7, pourcentage = Math.Round((nbre7 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "Faux numéro", type = type3, nombre = nbre8, pourcentage = Math.Round((nbre8 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "NRP", type = type3, nombre = nbre9, pourcentage = Math.Round((nbre9 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "Répondeur", type = type3, nombre = nbre10, pourcentage = Math.Round((nbre10 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "Rappel personnel", type = type3, nombre = nbre11, pourcentage = Math.Round((nbre11 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "Relance", type = type3, nombre = nbre12, pourcentage = Math.Round((nbre12 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "recycled record", type = type3, nombre = nbre13, pourcentage = Math.Round((nbre13 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "Limite Inaccessible", type = type3, nombre = nbre14, pourcentage = Math.Round((nbre14 / fichestraitees) * 100, 2) });
            }
            else
            {

                qualifs.Add(new qualification { nom = "OK Démo", type = type1, nombre = nbre1, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "KO Démo", type = type2, nombre = nbre2, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Barrage", type = type2, nombre = nbre3, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "SND", type = type2, nombre = nbre4, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "OPT OUT", type = type2, nombre = nbre5, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Hors Cible", type = type2, nombre = nbre6, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Occupé", type = type3, nombre = nbre7, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Faux numéro", type = type3, nombre = nbre8, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "NRP", type = type3, nombre = nbre9, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Répondeur", type = type3, nombre = nbre10, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Rappel personnel", type = type3, nombre = nbre11, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Relance", type = type3, nombre = nbre12, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "recycled record", type = type3, nombre = nbre13, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Limite Inaccessible", type = type3, nombre = nbre14, pourcentage = 0 });
            }
            if (fichescrees != 0)
            {
                PercentExploitation = Math.Round((fichesclotures / fichescrees) * 100, 2);
            }
            qualifs.Add(new qualification { nom = "Fiche Crées", nombre = fichescrees, pourcentage = 100 });
            qualifs.Add(new qualification { nom = "Total Appels", nombre = totApp });
            qualifs.Add(new qualification { nom = "Appels Aboutis ", nombre = totAboutis });
            qualifs.Add(new qualification { nom = "Fiches Cloturées", nombre = fichesclotures, pourcentage = PercentExploitation });
            qualifs.Add(new qualification { nom = "Contacts Argumentés ", nombre = fichesCA, pourcentage = PercentAccord });
            qualifs.Add(new qualification { nom = "Appels Traitées", nombre = fichestraitees });
            return Json(qualifs, JsonRequestBehavior.AllowGet);
        }

        // View IPD Par Agent et Operation
        [HttpGet]
        public ActionResult IPD_ParAgentEtOperation()
        {
            try
            {
                Employee empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
                List<SelectListItem> employees = new List<SelectListItem>();
                List<Employee> emps = serviceGroupeEmp.getListEmployeeByGroupeId(15);
                foreach (var e in emps)
                {
                    string IdHermes = e.IdHermes.ToString();
                    if (e.Id != empConnected.Id && e.Roles.Any(r => r.RoleId == 3))
                    {
                        if (!(employees.Exists(x => x.Text == e.UserName && x.Value == IdHermes)))
                        {
                            employees.Add(new SelectListItem { Text = e.UserName, Value = IdHermes });
                        }
                    }
                }
                ViewBag.AgentItems = employees;

                var data = db.Details_Activite_IPD.ToList();
                List<string> AllTitres = data.Select(gp => gp.NOM_OPERATION).Distinct().ToList();
                ViewBag.AllTitres = AllTitres;

                var a = new EvaluationViewModel();
                a.pseudoNameEmp = empConnected.pseudoName;
                a.userName = empConnected.UserName;
                if (empConnected.Content != null)
                {
                    String strbase64 = Convert.ToBase64String(empConnected.Content);
                    String Url = "data:" + empConnected.ContentType + ";base64," + strbase64;
                    ViewBag.url = Url;
                    a.Url = Url;

                }
                return View(a);
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e);
                return RedirectToAction("Connect", "Authentification");

            }
        }

        // Calcul IPD Par Agent ET Operation
        public JsonResult GetIPDParAgentEtOperationValues(int idHermes, string TitreOperation, String dateSel)
        {
            var data = db.Details_Activite_IPD.Where(d => d.NOM_OPERATION == TitreOperation).ToList();
            var appels = db.appels.Where(a => a.Customer_Id == 2 && a.nomCompagne == TitreOperation).ToList();
            List<qualification> qualifs = new List<qualification>();
            var ListQualiif = new List<SelectListItem>();
            DateTime from = DateTime.Parse(dateSel.Substring(0, 10));
            DateTime to = DateTime.Parse(dateSel.Substring(13, 10));
            IList<int?> ListCodesClotures = new List<int?>() { 1, 2, 3, 4, 5, 6, 91, 99 };
            double fichescrees = 0;
            double totApp = 0;
            double totAboutis = 0;
            double fichesclotures = 0;
            double fichestraitees = 0;
            double fichesCA = 0;
            double PercentExploitation = 0;
            double PercentAccord = 0;
            int nbre1 = 0; int nbre2 = 0; int nbre3 = 0; int nbre4 = 0; int nbre5 = 0; int nbre6 = 0; int nbre7 = 0; int nbre8 = 0; int nbre9 = 0; int nbre10 = 0;
            int nbre11 = 0; int nbre12 = 0; int nbre13 = 0; int nbre14 = 0;
            string type1 = "CA Positif";
            string type2 = "CA Négatif";
            string type3 = "Système";
            foreach (var app in appels)
            {
                if (app.date >= from && app.date <= to && app.Id_Hermes == idHermes)
                {
                    totApp += app.TotalAppelEmis;
                    totAboutis += app.TotalAppelAboutis;
                }
            }

            foreach (var item in data)
            {
                fichescrees += 1;
                if (item.DATE != null)
                {
                    string d = item.DATE.Substring(0, 4) + '-' + item.DATE.Substring(4, 2) + '-' + item.DATE.Substring(6, 2);
                DateTime date = Convert.ToDateTime(d);
                if (date >= from && date <= to && item.ID_TV == idHermes)
                {
                        if (item.STATUS != null)
                        {
                            fichestraitees += 1;
                        }
                        if (ListCodesClotures.Contains(item.STATUS))
                        {
                            fichesclotures += 1;
                        }
                        if (item.Argued == true)
                        {
                            fichesCA += 1;
                        }
                        if (item.Positive == true)
                        {
                            nbre1 += 1;
                        }
                        switch (item.STATUS)
                        {
                            case 2:
                                nbre2 += 1;
                                break;
                            case 3:
                                nbre3 += 1;
                                break;
                            case 4:
                                nbre4 += 1;
                                break;
                            case 5:
                                nbre5 += 1;
                                break;
                            case 6:
                                nbre6 += 1;
                                break;
                            case 90:
                                nbre7 += 1;
                                break;
                            case 91:
                                nbre8 += 1;
                                break;
                            case 92:
                                nbre9 += 1;
                                break;
                            case 93:
                                nbre10 += 1;
                                break;
                            case 94:
                                nbre11 += 1;
                                break;
                            case 95:
                                nbre12 += 1;
                                break;
                            case 98:
                                nbre13 += 1;
                                break;
                            case 99:
                                nbre14 += 1;
                                break;
                        }
                    }
                }
            }
            if (fichesCA != 0)
            {
                PercentAccord = Math.Round((nbre1 / fichesCA) * 100, 2);
            }

            if (fichestraitees != 0)
            {
                qualifs.Add(new qualification { nom = "OK Démo", type = type1, nombre = nbre1, pourcentage = Math.Round((nbre1 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "KO Démo", type = type2, nombre = nbre2, pourcentage = Math.Round((nbre2 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "Barrage", type = type2, nombre = nbre3, pourcentage = Math.Round((nbre3 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "SND", type = type2, nombre = nbre4, pourcentage = Math.Round((nbre4 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "OPT OUT", type = type2, nombre = nbre5, pourcentage = Math.Round((nbre5 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "Hors Cible", type = type2, nombre = nbre6, pourcentage = Math.Round((nbre6 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "Occupé", type = type3, nombre = nbre7, pourcentage = Math.Round((nbre7 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "Faux numéro", type = type3, nombre = nbre8, pourcentage = Math.Round((nbre8 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "NRP", type = type3, nombre = nbre9, pourcentage = Math.Round((nbre9 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "Répondeur", type = type3, nombre = nbre10, pourcentage = Math.Round((nbre10 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "Rappel personnel", type = type3, nombre = nbre11, pourcentage = Math.Round((nbre11 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "Relance", type = type3, nombre = nbre12, pourcentage = Math.Round((nbre12 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "recycled record", type = type3, nombre = nbre13, pourcentage = Math.Round((nbre13 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "Limite Inaccessible", type = type3, nombre = nbre14, pourcentage = Math.Round((nbre14 / fichestraitees) * 100, 2) });
            }
            else
            {
                qualifs.Add(new qualification { nom = "OK Démo", type = type1, nombre = nbre1, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "KO Démo", type = type2, nombre = nbre2, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Barrage", type = type2, nombre = nbre3, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "SND", type = type2, nombre = nbre4, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "OPT OUT", type = type2, nombre = nbre5, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Hors Cible", type = type2, nombre = nbre6, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Occupé", type = type3, nombre = nbre7, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Faux numéro", type = type3, nombre = nbre8, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "NRP", type = type3, nombre = nbre9, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Répondeur", type = type3, nombre = nbre10, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Rappel personnel", type = type3, nombre = nbre11, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Relance", type = type3, nombre = nbre12, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "recycled record", type = type3, nombre = nbre13, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Limite Inaccessible", type = type3, nombre = nbre14, pourcentage = 0 });
            }
            if (fichescrees != 0)
            {
                PercentExploitation = Math.Round((fichesclotures / fichescrees) * 100, 2);
            }
            qualifs.Add(new qualification { nom = "Fiche Crées", nombre = fichescrees, pourcentage = 100 });
            qualifs.Add(new qualification { nom = "Total Appels", nombre = totApp });
            qualifs.Add(new qualification { nom = "Appels Aboutis ", nombre = totAboutis });
            qualifs.Add(new qualification { nom = "Fiches Cloturées", nombre = fichesclotures, pourcentage = PercentExploitation });
            qualifs.Add(new qualification { nom = "Contacts Argumentés ", nombre = fichesCA, pourcentage = PercentAccord });
            qualifs.Add(new qualification { nom = "Appels Traitées", nombre = fichestraitees });
            return Json(qualifs, JsonRequestBehavior.AllowGet);
        }


        // View IPD Accueil: Etat de Campagne
        [HttpGet]
        public ActionResult IPD_EtatCampagne()
        {
            try
            {
                Employee empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

                var a = new EvaluationViewModel();
                a.pseudoNameEmp = empConnected.pseudoName;
                a.userName = empConnected.UserName;
                if (empConnected.Content != null)
                {
                    String strbase64 = Convert.ToBase64String(empConnected.Content);
                    String Url = "data:" + empConnected.ContentType + ";base64," + strbase64;
                    ViewBag.url = Url;
                    a.Url = Url;

                }
                return View(a);
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e);
                return RedirectToAction("Connect", "Authentification");
            }
        }


        // Calcul IPD Etat de ccampagne
        public JsonResult GetEtatCampagneValues()
        {
            var appels = db.appels.Where(a => a.Customer_Id == 2).ToList();
            var Attendances = db.AttendanceHermes_Campagnes.ToList();
            var data = db.Details_Activite_IPD.ToList();
            List<qualification> qualifs = new List<qualification>();
            var ListQualiif = new List<SelectListItem>();

            IList<int?> ListCodesClotures = new List<int?>() { 1, 2, 3, 4, 5, 6, 91, 99};
            double fichescrees = 0;
            double fichestraitees = 0;
            double totApp = 0;
           // double totAboutis = 0;
            double fichesclotures = 0;
            double fichesCA = 0;
            double PercentExploitation = 0;
            double PercentAccord = 0;
            double JH, t = 0;
            int nbre1 = 0; int nbre2 = 0; int nbre3 = 0; int nbre4 = 0; int nbre5 = 0; int nbre6 = 0; int nbre7 = 0; int nbre8 = 0; int nbre9 = 0; int nbre10 = 0;
            int nbre11 = 0; int nbre12 = 0; int nbre13 = 0; int nbre14 = 0;
            string type1 = "CA Positif";
            string type2 = "CA Négatif";
            string type3 = "Système";
            TimeSpan tempsLog = new TimeSpan(0, 0, 0);
            foreach (var att in Attendances)
            {
                    tempsLog += TimeSpan.Parse(att.Temps_Log);
                    t = tempsLog.TotalHours;
            }
            foreach ( var app in appels)
            {
                totApp += app.TotalAppelEmis;
            }
            foreach (var item in data)
            {
                fichescrees += 1;
              //  totAboutis += 1;
               if(item.STATUS != null)
                {
                    fichestraitees += 1;
                }
                if (ListCodesClotures.Contains(item.STATUS))
                {
                    fichesclotures += 1;
                }
                if (item.Argued == true)
                {
                    fichesCA += 1;
                }
                if (item.Positive == true)
                {
                    nbre1 += 1;
                }
                switch (item.STATUS)
                {
                    case 2:
                        nbre2 += 1;
                        break;
                    case 3:
                        nbre3 += 1;
                        break;
                    case 4:
                        nbre4 += 1;
                        break;
                    case 5:
                        nbre5 += 1;
                        break;
                    case 6:
                        nbre6 += 1;
                        break;
                    case 90:
                        nbre7 += 1;
                        break;
                    case 91:
                        nbre8 += 1;
                        break;
                    case 92:
                        nbre9 += 1;
                        break;
                    case 93:
                        nbre10 += 1;
                        break;
                    case 94:
                        nbre11 += 1;
                        break;
                    case 95:
                        nbre12 += 1;
                        break;
                    case 98:
                        nbre13 += 1;
                        break;
                    case 99:
                        nbre14 += 1;
                        break;
                }
            }

            if (fichesCA != 0)
            {
                PercentAccord = Math.Round((nbre1 / fichesCA) * 100, 2);
            }
         //  fichestraitees = nbre1 + nbre2 + nbre3 + nbre4 + nbre5 + nbre6 + nbre7 + nbre8 + nbre9 + nbre10 + nbre11 + nbre12 + nbre13 + nbre14;
            if (fichestraitees != 0)
            {
                qualifs.Add(new qualification { nom = "OK Démo", type = type1, nombre = nbre1, pourcentage = Math.Round((nbre1 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "KO Démo", type = type2, nombre = nbre2, pourcentage = Math.Round((nbre2 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "Barrage", type = type2, nombre = nbre3, pourcentage = Math.Round((nbre3 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "SND", type = type2, nombre = nbre4, pourcentage = Math.Round((nbre4 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "OPT OUT", type = type2, nombre = nbre5, pourcentage = Math.Round((nbre5 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "Hors Cible", type = type2, nombre = nbre6, pourcentage = Math.Round((nbre6 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "Occupé", type = type3, nombre = nbre7, pourcentage = Math.Round((nbre7 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "Faux numéro", type = type3, nombre = nbre8, pourcentage = Math.Round((nbre8 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "NRP", type = type3, nombre = nbre9, pourcentage = Math.Round((nbre9 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "Répondeur", type = type3, nombre = nbre10, pourcentage = Math.Round((nbre10 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "Rappel personnel", type = type3, nombre = nbre11, pourcentage = Math.Round((nbre11 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "Relance", type = type3, nombre = nbre12, pourcentage = Math.Round((nbre12 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "recycled record", type = type3, nombre = nbre13, pourcentage = Math.Round((nbre13 / fichestraitees) * 100, 2) });
                qualifs.Add(new qualification { nom = "Limite Inaccessible", type = type3, nombre = nbre14, pourcentage = Math.Round((nbre14 / fichestraitees) * 100, 2) });
            }
            else
            {
                qualifs.Add(new qualification { nom = "OK Démo", type = type1, nombre = nbre1, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "KO Démo", type = type2, nombre = nbre2, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Barrage", type = type2, nombre = nbre3, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "SND", type = type2, nombre = nbre4, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "OPT OUT", type = type2, nombre = nbre5, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Hors Cible", type = type2, nombre = nbre6, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Occupé", type = type3, nombre = nbre7, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Faux numéro", type = type3, nombre = nbre8, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "NRP", type = type3, nombre = nbre9, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Répondeur", type = type3, nombre = nbre10, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Rappel personnel", type = type3, nombre = nbre11, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Relance", type = type3, nombre = nbre12, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "recycled record", type = type3, nombre = nbre13, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Limite Inaccessible", type = type3, nombre = nbre14, pourcentage = 0 });
            }
            if (fichescrees != 0)
            {
                PercentExploitation = Math.Round((fichesclotures / fichescrees) * 100, 2);
            }
            JH = Math.Round((t / 7.5), 2);
            qualifs.Add(new qualification { nom = "Fiche Crées", nombre = fichescrees, pourcentage = 100 });
            qualifs.Add(new qualification { nom = "Total Appels", nombre = totApp });
            qualifs.Add(new qualification { nom = "Appels Traitées ", nombre = fichestraitees });
            qualifs.Add(new qualification { nom = "Fiches Cloturées", nombre = fichesclotures, pourcentage = PercentExploitation });
            qualifs.Add(new qualification { nom = "Contacts Argumentés ", nombre = fichesCA, pourcentage = PercentAccord });
            qualifs.Add(new qualification { nom = "JourHomme", nombre = JH });
            return Json(qualifs, JsonRequestBehavior.AllowGet);
        }

        //Calcul Comparaison entre les agents
        public JsonResult GetComparaisonAgentsValues()
        {
            Employee empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
     
            List<SelectListItem> employees = new List<SelectListItem>();           
                List<Employee> emps = serviceGroupeEmp.getListEmployeeByGroupeId(15);
                foreach (var e in emps)
                {
                    string IdHermes = e.IdHermes.ToString();
                    if (e.Id != empConnected.Id && e.Roles.Any(r => r.RoleId == 3))
                    {
                        if (!(employees.Exists(x => x.Text == e.UserName && x.Value == IdHermes)))
                        {
                            employees.Add(new SelectListItem { Text = e.UserName, Value = IdHermes });
                        }
                    }
            }
            var data = db.Details_Activite_IPD.ToList();
            var appels = db.appels.Where(a => a.Customer_Id == 2).ToList();
            var Attendances = db.AttendanceHermes_Campagnes.ToList();
            List<ProdAgent> prod = new List<ProdAgent>();
           
            foreach (var emp in employees)
            {
                TimeSpan tempsLog = new TimeSpan(0, 0, 0);
                double nbAccords = 0, nbCA = 0, appAboutis = 0;
                double tauxAccords = 0, JH = 0; double t = 0;
                foreach(var att in Attendances)
                {
                    if (att.ID_Hermes == emp.Value)
                    {
                        tempsLog +=  TimeSpan.Parse(att.Temps_Log);
                         t = tempsLog.TotalHours ;
                    }
                }
                foreach (var app in appels)
                {
                    if (app.Id_Hermes == Int32.Parse(emp.Value))
                    {
                        appAboutis += app.TotalAppelAboutis;
                    }
                }

                foreach (var item in data)
                {
                    if (item.ID_TV == Int32.Parse(emp.Value))
                    {
                        if (item.Argued == true)
                        {
                            nbCA += 1;
                        }
                        if (item.Argued == true && item.Positive == true)
                        {
                            nbAccords += 1;
                        }
                    }
                }
                if (nbCA != 0)
                {
                    tauxAccords = Math.Round((nbAccords / nbCA) * 100, 2);
                }

                JH = Math.Round((t / 7.5), 2);
                prod.Add(new ProdAgent { agent = emp.Text, nbAccords = nbAccords, nbCA = nbCA, appAboutis = appAboutis, tauxAccords = tauxAccords, JH = JH});

            }
            return Json(prod, JsonRequestBehavior.AllowGet);
        }


        //Calcul Comparaison entre les opérations
        public JsonResult GetComparaisonOperationsValues()
        {
            Employee empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            List<ProdOperation> qualifs = new List<ProdOperation>();
            var data = db.Details_Activite_IPD.ToList();
            List<string> AllTitres = data.Select(gp => gp.NOM_OPERATION).Distinct().ToList();
           foreach (var op in AllTitres)
            {
                double fichestraitees = 0;
                int nbre1 = 0; int nbre2 = 0; int nbre3 = 0; int nbre4 = 0; int nbre5 = 0; int nbre6 = 0;
                var OPEdata = db.Details_Activite_IPD.Where(d => d.NOM_OPERATION == op).ToList();
                foreach (var item in OPEdata)
                {
                    if (item.STATUS != null)
                    {
                        fichestraitees += 1;
                    }
                    if (item.Positive == true)
                    {
                        nbre1 += 1;
                    }
                    switch (item.STATUS)
                    {
                        case 2:
                            nbre2 += 1;
                            break;
                        case 3:
                            nbre3 += 1;
                            break;
                        case 4:
                            nbre4 += 1;
                            break;
                        case 5:
                            nbre5 += 1;
                            break;
                        case 6:
                            nbre6 += 1;
                            break;
                    }
                    }
                if (fichestraitees != 0)
                {
                    double ok = Math.Round((nbre1 / fichestraitees) * 100, 2);
                    double ko = Math.Round((nbre2 / fichestraitees) * 100, 2);
                    double barrage = Math.Round((nbre3 / fichestraitees) * 100, 2);
                    double snd = Math.Round((nbre4 / fichestraitees) * 100, 2);
                    double optout = Math.Round((nbre5 / fichestraitees) * 100, 2);
                    double horscible = Math.Round((nbre6 / fichestraitees) * 100, 2);
                    qualifs.Add(new ProdOperation { nomOpe = op, PercentOk = ok, PercentKo = ko, PercentBarrage = barrage, PercentSnd = snd, PercentOPTOUT = optout, PercentHorscible = horscible  });
                }
                }

                return Json(qualifs, JsonRequestBehavior.AllowGet);
        }



        }
    }
