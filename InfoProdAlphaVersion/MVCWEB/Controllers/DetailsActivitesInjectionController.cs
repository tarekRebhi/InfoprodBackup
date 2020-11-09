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
    [Authorize(Roles = "Qualité,Manager,SuperManager")]
    public class DetailsActivitesInjectionController : Controller
    {
        private ReportContext db = new ReportContext();
        IGroupeEmployeeService serviceGroupeEmp;
        IEmployeeService service;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        public DetailsActivitesInjectionController()
        {
            service = new EmployeeService();
            serviceGroupeEmp = new GroupesEmployeService();

        }
        public DetailsActivitesInjectionController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
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

        #region REAB GISI
        // View REAB_GISI Par Injection
        [HttpGet]
        public ActionResult GISI_REAB_Injection()
        {
            try
            {
                Employee empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

                var GisiReabdata = db.Details_Activite_REAB_GISI.ToList();

                List<string> AllTitres = GisiReabdata.Select(gp => gp.TITRE_OPERATION).Distinct().ToList();
                ViewBag.AllTitres = AllTitres;
                var a = new EmployeeViewModel();
                a.pseudoName = empConnected.pseudoName;
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


        public JsonResult GetReabGisiValuesParTitre(String SelectedTitre, String DateInj, String rang)
        {
            var GisiReab = db.Details_Activite_REAB_GISI.Where(a => a.TITRE_OPERATION == SelectedTitre && a.DATE_INJECTION == DateInj).ToList();
            if (rang != "")
            {
                GisiReab = db.Details_Activite_REAB_GISI.Where(a => a.TITRE_OPERATION == SelectedTitre && a.DATE_INJECTION == DateInj && a.RANG == rang).ToList();
            }
            var GisiReabInjected = db.DT_INJECTED.Where(i => i.ACTIVITE == "GISI_REAB" && i.DATE_INJECTION == DateInj && i.TITRE_OPERATION == SelectedTitre);

            List<qualification> qualifs = new List<qualification>();
            var ListQualiif = new List<SelectListItem>();

            IList<int?> ListCodesClotures = new List<int?>() { 1, 21, 22, 24, 39, 40, 41, 45, 47, 49, 51, 53, 55, 57, 59, 61, 63, 64, 65, 71, 91 };
            double fichestraites = 0;
            double fichesclotures = 0;
            double fichesCA = 0;
            double fichesinjectes = 0;
            double TauxExploitation = 0;
            double PercentAccord = 0;
            int nbre1 = 0; int nbre2 = 0; int nbre3 = 0; int nbre4 = 0; int nbre5 = 0; int nbre6 = 0; int nbre7 = 0; int nbre8 = 0; int nbre9 = 0; int nbre10 = 0;
            int nbre11 = 0; int nbre12 = 0; int nbre13 = 0; int nbre14 = 0; int nbre15 = 0; int nbre16 = 0; int nbre17 = 0; int nbre18 = 0; int nbre19 = 0; int nbre20 = 0;
            int nbre21 = 0; int nbre22 = 0; int nbre23 = 0; int nbre24 = 0; int nbre25 = 0; int nbre26 = 0; int nbre27 = 0; int nbre28 = 0; int nbre29 = 0; int nbre30 = 0; int nbre31 = 0; int nbre32 = 0;
            string type1 = "CA Positif";
            string type2 = "CA Négatif";
            string type3 = "Autre";

            foreach(var inj in GisiReabInjected)
            {
                fichesinjectes += inj.TOTAL;
            }
            foreach (var item in GisiReab)
            {
               
                    fichestraites += 1;
                    if (ListCodesClotures.Contains(item.STATUS))
                    {
                        fichesclotures += 1;
                    }
                    if (item.Argued == true)
                    {
                        fichesCA += 1;
                    }
                    if (item.AI == 1)
                    {
                        nbre1 += 1;
                    }
                    switch (item.STATUS)
                    {
                        //souhaite etre rappelé a deux codes 2 et 66
                        case 2:
                            nbre2 += 1;
                            break;
                        case 66:
                            nbre2 += 1;
                            break;
                        case 21:
                            nbre3 += 1;
                            break;
                        case 22:
                            nbre4 += 1;
                            break;
                        case 24:
                            nbre5 += 1;
                            break;
                        case 39:
                            nbre6 += 1;
                            break;
                        case 40:
                            nbre7 += 1;
                            break;
                        case 41:
                            nbre31 += 1;
                            break;
                        case 45:
                            nbre8 += 1;
                            break;
                        case 47:
                            nbre9 += 1;
                            break;
                        case 49:
                            nbre10 += 1;
                            break;
                        case 51:
                            nbre11 += 1;
                            break;
                        case 53:
                            nbre12 += 1;
                            break;
                        case 55:
                            nbre13 += 1;
                            break;
                        case 57:
                            nbre14 += 1;
                            break;
                        case 59:
                            nbre15 += 1;
                            break;
                        case 61:
                            nbre16 += 1;
                            break;
                        case 63:
                            nbre17 += 1;
                            break;
                        case 64:
                            nbre18 += 1;
                            break;
                        case 65:
                            nbre19 += 1;
                            break;
                        case 71:
                            nbre20 += 1;
                            break;
                        case 70:
                            nbre21 += 1;
                            break;
                        case 89:
                            nbre22 += 1;
                            break;
                        case 90:
                            nbre32 += 1;
                            break;
                        case 91:
                            nbre23 += 1;
                            break;
                        case 92:
                            nbre24 += 1;
                            break;
                        case 93:
                            nbre25 += 1;
                            break;
                        case 94:
                            nbre26 += 1;
                            break;
                        case 95:
                            nbre27 += 1;
                            break;
                        case 96:
                            nbre28 += 1;
                            break;
                        case 98:
                            nbre29 += 1;
                            break;
                        case 99:
                            nbre30 += 1;
                            break;
                }
            }
            if (fichesCA != 0)
            {
                PercentAccord = Math.Round((nbre1 / fichesCA) * 100, 2);
            }

            if (fichestraites != 0)
            {
                // TauxExploitation = Math.Round((fichesclotures / fichestraites) * 100, 2);            
                qualifs.Add(new qualification { nom = "ACCORD", type = type1, nombre = nbre1, pourcentage = Math.Round((nbre1 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "souhaite être rappelé", type = type2, nombre = nbre2, pourcentage = Math.Round((nbre2 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Abo déjà renouvelé", type = type2, nombre = nbre3, pourcentage = Math.Round((nbre3 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Déjà un abo dans la société c'est suffisant", type = type2, nombre = nbre4, pourcentage = Math.Round((nbre4 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Le lit déjà par revue de presse interne", type = type2, nombre = nbre5, pourcentage = Math.Round((nbre5 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Déjà lecteur d'un ou plusieurs magazines pro", type = type2, nombre = nbre6, pourcentage = Math.Round((nbre6 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Internet me suffit", type = type2, nombre = nbre7, pourcentage = Math.Round((nbre7 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Les newsletters me suffisent", type = type2, nombre = nbre31, pourcentage = Math.Round((nbre31 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Pas le temps de lire", type = type2, nombre = nbre8, pourcentage = Math.Round((nbre8 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Trop généraliste", type = type2, nombre = nbre9, pourcentage = Math.Round((nbre9 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Trop technique", type = type2, nombre = nbre10, pourcentage = Math.Round((nbre10 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Manque de valeur ajoutée", type = type2, nombre = nbre11, pourcentage = Math.Round((nbre11 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Trop cher", type = type2, nombre = nbre12, pourcentage = Math.Round((nbre12 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Restriction budgétaire", type = type2, nombre = nbre13, pourcentage = Math.Round((nbre13 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Gestion des abos confiée à une société extérieure", type = type2, nombre = nbre14, pourcentage = Math.Round((nbre14 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Pas décisionnaire sur les abos", type = type2, nombre = nbre15, pourcentage = Math.Round((nbre15 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Pas concerné (changement de fonction, retraité...)", type = type2, nombre = nbre16, pourcentage = Math.Round((nbre16 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Uniquement par courrier", type = type2, nombre = nbre17, pourcentage = Math.Round((nbre17 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Trop tot pour mon reab", type = type2, nombre = nbre18, pourcentage = Math.Round((nbre18 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Refus de commander par tél", type = type2, nombre = nbre19, pourcentage = Math.Round((nbre19 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Refus de réponse", type = type2, nombre = nbre20, pourcentage = Math.Round((nbre20 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Interlocuteur injoignable", type = type3, nombre = nbre21, pourcentage = Math.Round((nbre21 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Appel raccroché", type = type3, nombre = nbre22, pourcentage = Math.Round((nbre22 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Occupé", type = type3, nombre = nbre32, pourcentage = Math.Round((nbre32 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Faux numéro", type = type3, nombre = nbre23, pourcentage = Math.Round((nbre23 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Absent", type = type3, nombre = nbre24, pourcentage = Math.Round((nbre24 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Répondeur", type = type3, nombre = nbre25, pourcentage = Math.Round((nbre25 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Rappel personnel", type = type3, nombre = nbre26, pourcentage = Math.Round((nbre26 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Relance", type = type3, nombre = nbre27, pourcentage = Math.Round((nbre27 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Indisponible", type = type3, nombre = nbre28, pourcentage = Math.Round((nbre28 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "recycled record", type = type3, nombre = nbre29, pourcentage = Math.Round((nbre29 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Limite Inaccessible", type = type3, nombre = nbre30, pourcentage = Math.Round((nbre30 / fichestraites) * 100, 2) });
            }
            else
            {
                qualifs.Add(new qualification { nom = "ACCORD", type = type1, nombre = nbre1, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "souhaite être rappelé", type = type2, nombre = nbre2, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Abo déjà renouvelé", type = type2, nombre = nbre3, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Déjà un abo dans la société c'est suffisant", type = type2, nombre = nbre4, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Le lit déjà par revue de presse interne", type = type2, nombre = nbre5, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Déjà lecteur d'un ou plusieurs magazines pro", type = type2, nombre = nbre6, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Internet me suffit", type = type2, nombre = nbre7, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Les newsletters me suffisent", type = type2, nombre = nbre31, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Pas le temps de lire", type = type2, nombre = nbre8, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Trop généraliste", type = type2, nombre = nbre9, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Trop technique", type = type2, nombre = nbre10, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Manque de valeur ajoutée", type = type2, nombre = nbre11, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Trop cher", type = type2, nombre = nbre12, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Restriction budgétaire", type = type2, nombre = nbre13, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Gestion des abos confiée à une société extérieure", type = type2, nombre = nbre14, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Pas décisionnaire sur les abos", type = type2, nombre = nbre15, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Pas concerné (changement de fonction, retraité...)", type = type2, nombre = nbre16, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Uniquement par courrier", type = type2, nombre = nbre17, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Trop tot pour mon reab", type = type2, nombre = nbre18, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Refus de commander par tél", type = type2, nombre = nbre19, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Refus de réponse", type = type2, nombre = nbre20, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Interlocuteur injoignable", type = type3, nombre = nbre21, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Appel raccroché", type = type3, nombre = nbre22, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Occupé", type = type3, nombre = nbre32, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Faux numéro", type = type3, nombre = nbre23, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Absent", type = type3, nombre = nbre24, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Répondeur", type = type3, nombre = nbre25, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Rappel personnel", type = type3, nombre = nbre26, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Relance", type = type3, nombre = nbre27, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Indisponible", type = type3, nombre = nbre28, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "recycled record", type = type3, nombre = nbre29, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Limite Inaccessible", type = type3, nombre = nbre30, pourcentage = 0 });
            }
            if (fichesinjectes != 0)
            {
                TauxExploitation = Math.Round((fichestraites / fichesinjectes) * 100, 2);
            }
                qualifs.Add(new qualification { nom = "Fiche traitées", nombre = fichestraites, pourcentage = 100 });
            qualifs.Add(new qualification { nom = "Fiches Cloturées", nombre = fichesclotures, pourcentage = TauxExploitation });
            qualifs.Add(new qualification { nom = "Contacts Argumentés ", nombre = fichesCA, pourcentage = PercentAccord });
            qualifs.Add(new qualification { nom = "Fiche injectées", nombre = fichesinjectes, pourcentage = 100 });
            return Json(qualifs, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDatesInjGISIREAB(string SelectedTitre)
        {
            List<SelectListItem> DatesInjectionItems = new List<SelectListItem>();
            var data = db.Details_Activite_REAB_GISI.Where(a => a.TITRE_OPERATION == SelectedTitre).ToList();
            DatesInjectionItems.Insert(0, new SelectListItem { Text = "Sélectionner date d'injection", Value = "" });
            foreach (var item in data)
            {
                    if (!(DatesInjectionItems.Exists(x => x.Value == item.DATE_INJECTION)))
                    {
                        if (item.DATE_INJECTION != "" && item.DATE_INJECTION != null)
                        {
                            DatesInjectionItems.Add(new SelectListItem { Text = item.DATE_INJECTION, Value = item.DATE_INJECTION });
                        }
                    }
                }            

            return Json(new SelectList(DatesInjectionItems, "Value", "Text"));
        }
        #endregion

        #region GMT REAB par date injection

        [HttpGet]
        public ActionResult GMT_REAB_Injection()
        {
            try
            {
                Employee empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

                var GmtReabdata = db.Details_Activite_REAB_GMT.ToList();

                List<string> AllTitres = GmtReabdata.Select(gp => gp.TITRE_OPERATION).Distinct().ToList();
                ViewBag.AllTitres = AllTitres;
                var a = new EmployeeViewModel();
                a.pseudoName = empConnected.pseudoName;
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
        public JsonResult GetReabGmtValuesParTitre(String SelectedTitre, String DateInj, String rang)
        {
            var data = db.Details_Activite_REAB_GMT.Where(a => a.TITRE_OPERATION == SelectedTitre && a.DATE_INJECTION == DateInj).ToList();

            if (rang != "")
            {
                data = db.Details_Activite_REAB_GMT.Where(a => a.TITRE_OPERATION == SelectedTitre && a.DATE_INJECTION == DateInj && a.CODE_PROP_1 == rang).ToList();
            }
                   
            var GmtReabInjected = db.DT_INJECTED.Where(i => i.ACTIVITE == "GMT_REAB" && i.DATE_INJECTION == DateInj && i.TITRE_OPERATION == SelectedTitre);

            List <qualification> qualifs = new List<qualification>();
            var ListQualiif = new List<SelectListItem>();
            IList<int?> ListCodesClotures = new List<int?>() { 1, 21, 22, 24, 39, 40, 41, 45, 51, 53, 55, 57, 59, 61, 63, 64, 65, 71, 91 };
            double fichestraites = 0;
            double fichesclotures = 0;
            double fichesCA = 0;
            double TauxExploitation = 0;
            double PercentAccord = 0;
            double fichesinjectes = 0;
            int nbre1 = 0; int nbre2 = 0; int nbre3 = 0; int nbre4 = 0; int nbre5 = 0; int nbre6 = 0; int nbre7 = 0; int nbre8 = 0; int nbre9 = 0; int nbre10 = 0;
            int nbre11 = 0; int nbre12 = 0; int nbre13 = 0; int nbre14 = 0; int nbre15 = 0; int nbre16 = 0; int nbre17 = 0; int nbre18 = 0; int nbre19 = 0; int nbre20 = 0;
            int nbre21 = 0; int nbre22 = 0; int nbre23 = 0; int nbre24 = 0; int nbre25 = 0; int nbre26 = 0; int nbre27 = 0; int nbre28 = 0; int nbre29 = 0; int nbre30 = 0;
            int nbre31 = 0; int nbre32 = 0;
            string type1 = "CA Positif";
            string type2 = "CA Négatif";
            string type3 = "Autre";

            foreach (var inj in GmtReabInjected)
            {
                fichesinjectes += inj.TOTAL;
            }

            foreach (var item in data)
            {
                    fichestraites += 1;
                    if (ListCodesClotures.Contains(item.STATUS))
                    {
                        fichesclotures += 1;
                    }
                    if (item.Argued == true)
                    {
                        fichesCA += 1;
                    }
                    if (item.AI == 1)
                    {
                        nbre1 += 1;
                    }
                    switch (item.STATUS)
                    {
                        case 2:
                            nbre2 += 1;
                            break;
                        case 21:
                            nbre3 += 1;
                            break;
                        case 22:
                            nbre4 += 1;
                            break;
                        case 24:
                            nbre5 += 1;
                            break;
                        case 39:
                            nbre6 += 1;
                            break;
                        case 40:
                            nbre7 += 1;
                            break;
                        case 41:
                            nbre8 += 1;
                            break;
                        case 45:
                            nbre9 += 1;
                            break;
                        case 47:
                            nbre31 += 1;
                            break;
                        case 49:
                            nbre32 += 1;
                            break;
                        case 51:
                            nbre10 += 1;
                            break;
                        case 53:
                            nbre11 += 1;
                            break;
                        case 55:
                            nbre12 += 1;
                            break;
                        case 57:
                            nbre13 += 1;
                            break;
                        case 59:
                            nbre14 += 1;
                            break;
                        case 61:
                            nbre15 += 1;
                            break;
                        case 63:
                            nbre16 += 1;
                            break;
                        case 64:
                            nbre17 += 1;
                            break;
                        case 65:
                            nbre18 += 1;
                            break;
                        //souhaite être rappelé est doublé par deux codes (2 et 66)
                        case 66:
                            nbre2 += 1;
                            break;
                        case 71:
                            nbre19 += 1;
                            break;
                        case 75:
                            nbre20 += 1;
                            break;
                        case 70:
                            nbre21 += 1;
                            break;
                        case 89:
                            nbre22 += 1;
                            break;
                        case 90:
                            nbre23 += 1;
                            break;
                        case 91:
                            nbre24 += 1;
                            break;
                        case 92:
                            nbre25 += 1;
                            break;
                        case 93:
                            nbre26 += 1;
                            break;
                        case 94:
                            nbre27 += 1;
                            break;
                        case 95:
                            nbre28 += 1;
                            break;
                        case 98:
                            nbre29 += 1;
                            break;
                        case 99:
                            nbre30 += 1;
                            break;
                }
            }
            if (fichesCA != 0)
            {
                PercentAccord = Math.Round((nbre1 / fichesCA) * 100, 2);
            }

            if (fichestraites != 0)
            {
               // PercentClotures = Math.Round((fichesclotures / fichestraites) * 100, 2);
                qualifs.Add(new qualification { nom = "ACCORD", type = type1, nombre = nbre1, pourcentage = Math.Round((nbre1 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "souhaite être rappelé", type = type2, nombre = nbre2, pourcentage = Math.Round((nbre2 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Abo déjà renouvelé", type = type2, nombre = nbre3, pourcentage = Math.Round((nbre3 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Déjà un abo dans la société c'est suffisant", type = type2, nombre = nbre4, pourcentage = Math.Round((nbre4 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Le lit déjà par revue de presse interne", type = type2, nombre = nbre5, pourcentage = Math.Round((nbre5 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Déjà lecteur d'un ou plusieurs magazines pro", type = type2, nombre = nbre6, pourcentage = Math.Round((nbre6 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Internet me suffit", type = type2, nombre = nbre7, pourcentage = Math.Round((nbre7 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Les newsletters me suffisent", type = type2, nombre = nbre8, pourcentage = Math.Round((nbre8 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Pas le temps de lire", type = type2, nombre = nbre9, pourcentage = Math.Round((nbre9 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Trop généraliste", type = type2, nombre = nbre31, pourcentage = Math.Round((nbre31 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Trop Technique", type = type2, nombre = nbre32, pourcentage = Math.Round((nbre32 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Manque de valeur ajoutée", type = type2, nombre = nbre10, pourcentage = Math.Round((nbre10 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Trop cher", type = type2, nombre = nbre11, pourcentage = Math.Round((nbre11 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Restriction budgétaire", type = type2, nombre = nbre12, pourcentage = Math.Round((nbre12 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Gestion des abos confiée à une société extérieure", type = type2, nombre = nbre13, pourcentage = Math.Round((nbre13 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Pas décisionnaire sur les abos", type = type2, nombre = nbre14, pourcentage = Math.Round((nbre14 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Pas concerné (changement de fonction, retraité...)", type = type2, nombre = nbre15, pourcentage = Math.Round((nbre15 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Uniquement par courrier", type = type2, nombre = nbre16, pourcentage = Math.Round((nbre16 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Trop tôt pour mon réab", type = type2, nombre = nbre17, pourcentage = Math.Round((nbre17 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Refus de commander par tél", type = type2, nombre = nbre18, pourcentage = Math.Round((nbre18 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Refus de réponse", type = type2, nombre = nbre19, pourcentage = Math.Round((nbre19 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Post_Appel_Depassé", type = type3, nombre = nbre20, pourcentage = Math.Round((nbre20 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Interlocuteur injoignable", type = type3, nombre = nbre21, pourcentage = Math.Round((nbre21 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Appel raccroché", type = type3, nombre = nbre22, pourcentage = Math.Round((nbre22 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Occupé", type = type3, nombre = nbre23, pourcentage = Math.Round((nbre23 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Faux numéro", type = type3, nombre = nbre24, pourcentage = Math.Round((nbre24 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Absent", type = type3, nombre = nbre25, pourcentage = Math.Round((nbre25 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Répondeur", type = type3, nombre = nbre26, pourcentage = Math.Round((nbre26 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Rappel personnel", type = type3, nombre = nbre27, pourcentage = Math.Round((nbre27 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Relance", type = type3, nombre = nbre28, pourcentage = Math.Round((nbre28 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "recycled record", type = type3, nombre = nbre29, pourcentage = Math.Round((nbre29 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Limite Inaccessible", type = type3, nombre = nbre30, pourcentage = Math.Round((nbre30 / fichestraites) * 100, 2) });
            }
            else
            {
                qualifs.Add(new qualification { nom = "ACCORD", type = type1, nombre = nbre1, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "souhaite être rappelé", type = type2, nombre = nbre2, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Abo déjà renouvelé", type = type2, nombre = nbre3, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Déjà un abo dans la société c'est suffisant", type = type2, nombre = nbre4, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Le lit déjà par revue de presse interne", type = type2, nombre = nbre5, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Déjà lecteur d'un ou plusieurs magazines pro", type = type2, nombre = nbre6, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Internet me suffit", type = type2, nombre = nbre7, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Les newsletters me suffisent", type = type2, nombre = nbre8, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Pas le temps de lire", type = type2, nombre = nbre9, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Trop généraliste", type = type2, nombre = nbre31, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Trop Technique", type = type2, nombre = nbre32, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Manque de valeur ajoutée", type = type2, nombre = nbre10, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Trop cher", type = type2, nombre = nbre11, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Restriction budgétaire", type = type2, nombre = nbre12, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Gestion des abos confiée à une société extérieure", type = type2, nombre = nbre13, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Pas décisionnaire sur les abos", type = type2, nombre = nbre14, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Pas concerné (changement de fonction, retraité...)", type = type2, nombre = nbre15, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Uniquement par courrier", type = type2, nombre = nbre16, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Trop tôt pour mon réab", type = type2, nombre = nbre17, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Refus de commander par tél", type = type2, nombre = nbre18, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Refus de réponse", type = type2, nombre = nbre19, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Post_Appel_Depassé", type = type3, nombre = nbre20, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Interlocuteur injoignable", type = type3, nombre = nbre21, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Appel raccroché", type = type3, nombre = nbre22, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Occupé", type = type3, nombre = nbre23, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Faux numéro", type = type3, nombre = nbre24, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Absent", type = type3, nombre = nbre25, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Répondeur", type = type3, nombre = nbre26, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Rappel personnel", type = type3, nombre = nbre27, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Relance", type = type3, nombre = nbre28, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "recycled record", type = type3, nombre = nbre29, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Limite Inaccessible", type = type3, nombre = nbre30, pourcentage = 0 });

            }
            if (fichesinjectes != 0)
            {
                TauxExploitation = Math.Round((fichestraites / fichesinjectes) * 100, 2);
            }
            qualifs.Add(new qualification { nom = "Fiche traitées", nombre = fichestraites, pourcentage = 100 });
            qualifs.Add(new qualification { nom = "Fiches Cloturées", nombre = fichesclotures, pourcentage = TauxExploitation });
            qualifs.Add(new qualification { nom = "Contacts Argumentés ", nombre = fichesCA, pourcentage = PercentAccord });
            qualifs.Add(new qualification { nom = "Fiche injectées", nombre = fichesinjectes, pourcentage = 100 });
            return Json(qualifs, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDatesInjGMTREAB(string SelectedTitre)
        {
            List<SelectListItem> DatesInjectionItems = new List<SelectListItem>();
            var data = db.Details_Activite_REAB_GMT.Where(a => a.TITRE_OPERATION == SelectedTitre).ToList();
            DatesInjectionItems.Insert(0, new SelectListItem { Text = "Sélectionner date d'injection", Value = "" });
            foreach (var item in data)
            {
                if (!(DatesInjectionItems.Exists(x => x.Value == item.DATE_INJECTION)))
                {
                    if (item.DATE_INJECTION != "" && item.DATE_INJECTION != null)
                    {
                        DatesInjectionItems.Add(new SelectListItem { Text = item.DATE_INJECTION, Value = item.DATE_INJECTION });
                    }
                }
            }

            return Json(new SelectList(DatesInjectionItems, "Value", "Text"));
        }
        #endregion


        #region GISI PROMO par injection
        [HttpGet]
        public ActionResult GISI_PROMO_Injection()
        {
            try
            {
                Employee empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

                var GisiPromodata = db.Details_Activite_PROMO_GISI.ToList();

                List<string> AllTitres = GisiPromodata.Select(gp => gp.TITRE_OPERATION).Distinct().ToList();
                ViewBag.AllTitres = AllTitres;
                var a = new EmployeeViewModel();
                a.pseudoName = empConnected.pseudoName;
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
        public JsonResult GetPromoGisiValuesParTitre(String SelectedTitre, String CodeOpe, String CodeProv, String DateInj)
        {

            var data = db.Details_Activite_PROMO_GISI.Where(a => a.DATE_INJECTION == DateInj && a.TITRE_OPERATION == SelectedTitre).ToList();
         
            if ( CodeOpe != "" && CodeProv == "")
            {
                data = db.Details_Activite_PROMO_GISI.Where(a => a.DATE_INJECTION == DateInj && a.TITRE_OPERATION == SelectedTitre  && a.CODE_OPE == CodeOpe).ToList();
            }

            if (CodeOpe != "" && CodeProv != "")
            {
                data = db.Details_Activite_PROMO_GISI.Where(a => a.DATE_INJECTION == DateInj && a.TITRE_OPERATION == SelectedTitre && a.CODE_OPE == CodeOpe && a.CODE_PROV_RELANCE == CodeProv).ToList();
            }
            var GisiPromoInjected = db.DT_INJECTED.Where(i => i.ACTIVITE == "GISI_PROMO" && i.DATE_INJECTION == DateInj && i.TITRE_OPERATION == SelectedTitre);

            List<qualification> qualifs = new List<qualification>();
            var ListQualiif = new List<SelectListItem>();
            IList<int?> ListCodesClotures = new List<int?>() { 1, 2, 3, 4, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 71, 91 };
            double fichestraites = 0;
            double fichesclotures = 0;
            double fichesCA = 0;
            double TauxExploitation = 0;
            double PercentAccord = 0;
            double fichesinjectes = 0;
            int nbre1 = 0; int nbre2 = 0; int nbre3 = 0; int nbre4 = 0; int nbre5 = 0; int nbre6 = 0; int nbre7 = 0; int nbre8 = 0; int nbre9 = 0; int nbre10 = 0;
            int nbre11 = 0; int nbre12 = 0; int nbre13 = 0; int nbre14 = 0; int nbre15 = 0; int nbre16 = 0; int nbre17 = 0; int nbre18 = 0; int nbre19 = 0; int nbre20 = 0;
            int nbre21 = 0; int nbre22 = 0; int nbre23 = 0; int nbre24 = 0; int nbre25 = 0; int nbre26 = 0; int nbre27 = 0; int nbre28 = 0; int nbre29 = 0;
            string type1 = "CA Positif";
            string type2 = "CA Négatif";
            string type3 = "Autre";

            foreach (var inj in GisiPromoInjected)
            {
                fichesinjectes += inj.TOTAL;
            }
            foreach (var item in data)
            {
                string d = item.DATE.Substring(0, 4) + '-' + item.DATE.Substring(4, 2) + '-' + item.DATE.Substring(6, 2);
                DateTime date = Convert.ToDateTime(d);

                fichestraites += 1;
                if (ListCodesClotures.Contains(item.STATUS))
                {
                    fichesclotures += 1;
                }
                if (item.Argued == true)
                {
                    fichesCA += 1;
                }
                if (item.ACCORD_TEMP == 1)
                {
                    nbre1 += 1;
                }
                if (item.ACCORD_TEMP == 1 && item.CODE_OPE == "CROSS")
                {
                    nbre2 += 1;
                }
                switch (item.STATUS)
                {
                    case 3:
                        nbre29 += 1;
                        break;
                    case 4:
                        nbre3 += 1;
                        break;
                    case 6:
                        nbre4 += 1;
                        break;
                    case 7:
                        nbre5 += 1;
                        break;
                    case 8:
                        nbre6 += 1;
                        break;
                    case 9:
                        nbre7 += 1;
                        break;
                    case 10:
                        nbre8 += 1;
                        break;
                    case 11:
                        nbre9 += 1;
                        break;
                    case 12:
                        nbre10 += 1;
                        break;
                    case 13:
                        nbre11 += 1;
                        break;
                    case 14:
                        nbre12 += 1;
                        break;
                    case 15:
                        nbre13 += 1;
                        break;
                    case 16:
                        nbre14 += 1;
                        break;
                    case 17:
                        nbre15 += 1;
                        break;
                    case 20:
                        nbre16 += 1;
                        break;
                    case 66:
                        nbre17 += 1;
                        break;
                    case 70:
                        nbre18 += 1;
                        break;
                    case 71:
                        nbre19 += 1;
                        break;
                    case 89:
                        nbre20 += 1;
                        break;
                    case 90:
                        nbre21 += 1;
                        break;
                    case 91:
                        nbre22 += 1;
                        break;
                    case 92:
                        nbre23 += 1;
                        break;
                    case 93:
                        nbre24 += 1;
                        break;
                    case 94:
                        nbre25 += 1;
                        break;
                    case 95:
                        nbre26 += 1;
                        break;
                    case 98:
                        nbre27 += 1;
                        break;
                    case 99:
                        nbre28 += 1;
                        break;

                }
            }
            if (fichesCA != 0)
            {
                PercentAccord = Math.Round((nbre1 / fichesCA) * 100, 2);
            }

            if (fichestraites != 0)
            {
               // PercentClotures = Math.Round((fichesclotures / fichestraites) * 100, 2);
                qualifs.Add(new qualification { nom = "ACCORD téléphonique", type = type1, nombre = nbre1, pourcentage = Math.Round((nbre1 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "ACCORD téléphonique Cross", type = type1, nombre = nbre2, pourcentage = Math.Round((nbre2 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Déjà abonné nominativement", type = type2, nombre = nbre3, pourcentage = Math.Round((nbre3 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Déjà abonné dans l'entreprise ou le service", type = type2, nombre = nbre4, pourcentage = Math.Round((nbre4 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Le lit déjà par une revue de presse", type = type2, nombre = nbre5, pourcentage = Math.Round((nbre5 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Déjà abonné à un autre magazine", type = type2, nombre = nbre6, pourcentage = Math.Round((nbre6 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Trop généraliste", type = type2, nombre = nbre7, pourcentage = Math.Round((nbre7 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Trop spécialisé", type = type2, nombre = nbre8, pourcentage = Math.Round((nbre8 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Contenus pas intéressants", type = type2, nombre = nbre9, pourcentage = Math.Round((nbre9 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Trop cher", type = type2, nombre = nbre10, pourcentage = Math.Round((nbre10 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Restriction budgétaire", type = type2, nombre = nbre11, pourcentage = Math.Round((nbre11 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Pas le temps de lire", type = type2, nombre = nbre12, pourcentage = Math.Round((nbre12 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "S'informe exclusivement sur Internet", type = type2, nombre = nbre13, pourcentage = Math.Round((nbre13 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Pas décisionnaire sur les abonnements", type = type2, nombre = nbre14, pourcentage = Math.Round((nbre14 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "N'est pas concerné (chgmt de fctn, retraité ...)", type = type2, nombre = nbre15, pourcentage = Math.Round((nbre15 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Non qualifié", type = type2, nombre = nbre16, pourcentage = Math.Round((nbre16 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Souhaite être rappelé", type = type2, nombre = nbre17, pourcentage = Math.Round((nbre17 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Refus de réponse", type = type2, nombre = nbre19, pourcentage = Math.Round((nbre19 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Interlocuteur injoignable", type = type3, nombre = nbre18, pourcentage = Math.Round((nbre18 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Appel raccroché", type = type3, nombre = nbre20, pourcentage = Math.Round((nbre20 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Occupé", type = type3, nombre = nbre21, pourcentage = Math.Round((nbre21 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "faux numéro", type = type3, nombre = nbre22, pourcentage = Math.Round((nbre22 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Absent", type = type3, nombre = nbre23, pourcentage = Math.Round((nbre23 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "répondeur", type = type3, nombre = nbre24, pourcentage = Math.Round((nbre24 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Rappel personnel", type = type3, nombre = nbre25, pourcentage = Math.Round((nbre25 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Relance", type = type3, nombre = nbre26, pourcentage = Math.Round((nbre26 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "recycled record", type = type3, nombre = nbre27, pourcentage = Math.Round((nbre27 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Limite Inaccessible", type = type3, nombre = nbre28, pourcentage = Math.Round((nbre28 / fichestraites) * 100, 2) });
            }
            else
            {
                qualifs.Add(new qualification { nom = "ACCORD téléphonique", type = type1, nombre = nbre1, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "ACCORD téléphonique Cross", type = type1, nombre = nbre2, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Déjà abonné nominativement", type = type2, nombre = nbre3, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Déjà abonné dans l'entreprise ou le service", type = type2, nombre = nbre4, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Le lit déjà par une revue de presse", type = type2, nombre = nbre5, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Déjà abonné à un autre magazine", type = type2, nombre = nbre6, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Trop généraliste", type = type2, nombre = nbre7, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Trop spécialisé", type = type2, nombre = nbre8, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Contenus pas intéressants", type = type2, nombre = nbre9, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Trop cher", type = type2, nombre = nbre10, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Restriction budgétaire", type = type2, nombre = nbre11, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Pas le temps de lire", type = type2, nombre = nbre12, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "S'informe exclusivement sur Internet", type = type2, nombre = nbre13, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Pas décisionnaire sur les abonnements", type = type2, nombre = nbre14, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "N'est pas concerné (chgmt de fctn, retraité ...)", type = type2, nombre = nbre15, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Non qualifié", type = type2, nombre = nbre16, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Souhaite être rappelé", type = type2, nombre = nbre17, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Refus de réponse", type = type2, nombre = nbre19, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Interlocuteur injoignable", type = type3, nombre = nbre18, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Appel raccroché", type = type3, nombre = nbre20, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Occupé", type = type3, nombre = nbre21, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "faux numéro", type = type3, nombre = nbre22, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Absent", type = type3, nombre = nbre23, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "répondeur", type = type3, nombre = nbre24, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Rappel personnel", type = type3, nombre = nbre25, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Relance", type = type3, nombre = nbre26, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "recycled record", type = type3, nombre = nbre27, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Limite Inaccessible", type = type3, nombre = nbre28, pourcentage = 0 });
            }
            if (fichesinjectes != 0)
            {
                TauxExploitation = Math.Round((fichestraites / fichesinjectes) * 100, 2);
            }
            qualifs.Add(new qualification { nom = "Fiche traitées", nombre = fichestraites, pourcentage = 100 });
            qualifs.Add(new qualification { nom = "Fiches Cloturées", nombre = fichesclotures, pourcentage = TauxExploitation });
            qualifs.Add(new qualification { nom = "Contacts Argumentés ", nombre = fichesCA, pourcentage = PercentAccord });
            qualifs.Add(new qualification { nom = "Fiche injectées", nombre = fichesinjectes, pourcentage = 100 });
            return Json(qualifs, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDatesInjGISIPROMO(string SelectedTitre)
        {
            List<SelectListItem> DatesInjectionItems = new List<SelectListItem>();
            var data = db.Details_Activite_PROMO_GISI.Where(a => a.TITRE_OPERATION == SelectedTitre).ToList();
            DatesInjectionItems.Insert(0, new SelectListItem { Text = "Sélectionner date d'injection", Value = "" });
            foreach (var item in data)
            {
                if (!(DatesInjectionItems.Exists(x => x.Value == item.DATE_INJECTION)))
                {
                    if (item.DATE_INJECTION != "" && item.DATE_INJECTION != null)
                    {
                        DatesInjectionItems.Add(new SelectListItem { Text = item.DATE_INJECTION, Value = item.DATE_INJECTION });
                    }
                }
            }

            return Json(new SelectList(DatesInjectionItems, "Value", "Text"));
        }
        public JsonResult GetCodesOpeOfTitresGISIPROMO(string SelectedTitre, string DateInj)
        {
            List<SelectListItem> CodesOpeItems = new List<SelectListItem>();

               var data = db.Details_Activite_PROMO_GISI.Where(a => a.TITRE_OPERATION == SelectedTitre && a.DATE_INJECTION == DateInj).ToList();
           
            CodesOpeItems.Insert(0, new SelectListItem { Text = "Sélectionner code opération", Value = "" });
            foreach (var item in data)
            {
                if (!(CodesOpeItems.Exists(x => x.Value == item.CODE_OPE)))
                {
                    CodesOpeItems.Add(new SelectListItem { Text = item.CODE_OPE, Value = item.CODE_OPE });
                }

            }
            return Json(new SelectList(CodesOpeItems, "Value", "Text"));
        }

        public JsonResult GetCodesProvGISIPROMO(string SelectedTitre, string codeOperation, string DateInj)
        {
            List<SelectListItem> CodesProvItems = new List<SelectListItem>();
 
               var data = db.Details_Activite_PROMO_GISI.Where(a => a.TITRE_OPERATION == SelectedTitre && a.CODE_OPE == codeOperation && a.DATE_INJECTION == DateInj).ToList();
          
            CodesProvItems.Insert(0, new SelectListItem { Text = "Sélectionner code prov-relance", Value = "" });
            foreach (var item in data)
            {
                if (!(CodesProvItems.Exists(x => x.Value == item.CODE_PROV_RELANCE)))
                {
                    if (item.CODE_PROV_RELANCE != "" && item.CODE_PROV_RELANCE != null)
                    {
                        CodesProvItems.Add(new SelectListItem { Text = item.CODE_PROV_RELANCE, Value = item.CODE_PROV_RELANCE });
                    }
                }
            }
            return Json(new SelectList(CodesProvItems, "Value", "Text"));
        }
        #endregion

        #region GMT PROMO par injection 
        [HttpGet]
        public ActionResult GMT_PROMO_Injection()
        {
            try
            {
                Employee empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

                var GmtPromodata = db.Details_Activite_PROMO_GMT.ToList();

                List<string> AllTitres = GmtPromodata.Select(gp => gp.TITRE_OPERATION).Distinct().ToList();
                ViewBag.AllTitres = AllTitres;
                var a = new EmployeeViewModel();
                a.pseudoName = empConnected.pseudoName;
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
        public JsonResult GetPromoGmtValuesParTitre(String SelectedTitre, String CodeOpe, String CodeProv, String DateInj)
        {
            List<qualification> qualifs = new List<qualification>();
            var data = db.Details_Activite_PROMO_GMT.Where(a => a.DATE_INJECTION == DateInj && a.TITRE_OPERATION == SelectedTitre).ToList();

            if (CodeOpe != "" && CodeProv == "")
            {
                data = db.Details_Activite_PROMO_GMT.Where(a => a.DATE_INJECTION == DateInj && a.TITRE_OPERATION == SelectedTitre && a.CODE_OPE == CodeOpe).ToList();
            }

            if (CodeOpe != "" && CodeProv != "")
            {
                data = db.Details_Activite_PROMO_GMT.Where(a => a.DATE_INJECTION == DateInj && a.TITRE_OPERATION == SelectedTitre && a.CODE_OPE == CodeOpe && a.CODE_PROV_RELANCE == CodeProv).ToList();
            }
            var GMTPROMOInjected = db.DT_INJECTED.Where(i => i.ACTIVITE == "GMT_PROMO" && i.DATE_INJECTION == DateInj && i.TITRE_OPERATION == SelectedTitre);

            IList<int?> ListCodesClotures = new List<int?>() { 1, 4, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 71, 91 };
            double fichestraites = 0;
            double fichesclotures = 0;
            double fichesCA = 0;
            double TauxExploitation = 0;
            double PercentAccord = 0;
            double fichesinjectes = 0;
            int nbre1 = 0; int nbre2 = 0; int nbre3 = 0; int nbre4 = 0; int nbre5 = 0; int nbre6 = 0; int nbre7 = 0; int nbre8 = 0; int nbre9 = 0; int nbre10 = 0;
            int nbre11 = 0; int nbre12 = 0; int nbre13 = 0; int nbre14 = 0; int nbre15 = 0; int nbre16 = 0; int nbre17 = 0; int nbre18 = 0; int nbre19 = 0; int nbre20 = 0;
            int nbre21 = 0; int nbre22 = 0; int nbre23 = 0; int nbre24 = 0; int nbre25 = 0; int nbre26 = 0; int nbre27 = 0; int nbre28 = 0;
            string type1 = "CA Positif";
            string type2 = "CA Négatif";
            string type3 = "Autre";


            foreach (var inj in GMTPROMOInjected)
            {
                fichesinjectes += inj.TOTAL;
            }

            foreach (var item in data)
            {
                string d = item.DATE.Substring(0, 4) + '-' + item.DATE.Substring(4, 2) + '-' + item.DATE.Substring(6, 2);
                DateTime date = Convert.ToDateTime(d);

                fichestraites += 1;
                if (ListCodesClotures.Contains(item.STATUS))
                {
                    fichesclotures += 1;
                }
                if (item.Argued == true)
                {
                    fichesCA += 1;
                }
                if (item.ACCORD_TEMP == 1)
                {
                    nbre1 += 1;
                }
                if (item.ACCORD_TEMP == 1 && item.CODE_OPE == "CROSS")
                {
                    nbre28 += 1;
                }
                switch (item.STATUS)
                {
                    case 4:
                        nbre2 += 1;
                        break;
                    case 6:
                        nbre3 += 1;
                        break;
                    case 7:
                        nbre4 += 1;
                        break;
                    case 8:
                        nbre5 += 1;
                        break;
                    case 9:
                        nbre6 += 1;
                        break;
                    case 10:
                        nbre7 += 1;
                        break;
                    case 11:
                        nbre8 += 1;
                        break;
                    case 12:
                        nbre9 += 1;
                        break;
                    case 13:
                        nbre10 += 1;
                        break;
                    case 14:
                        nbre11 += 1;
                        break;
                    case 15:
                        nbre12 += 1;
                        break;
                    case 16:
                        nbre13 += 1;
                        break;
                    case 17:
                        nbre14 += 1;
                        break;
                    case 66:
                        nbre15 += 1;
                        break;
                    case 71:
                        nbre16 += 1;
                        break;
                    case 75:
                        nbre17 += 1;
                        break;
                    case 70:
                        nbre18 += 1;
                        break;
                    case 89:
                        nbre19 += 1;
                        break;
                    case 90:
                        nbre20 += 1;
                        break;
                    case 91:
                        nbre21 += 1;
                        break;
                    case 92:
                        nbre22 += 1;
                        break;
                    case 93:
                        nbre23 += 1;
                        break;
                    case 94:
                        nbre24 += 1;
                        break;
                    case 95:
                        nbre25 += 1;
                        break;
                    case 98:
                        nbre26 += 1;
                        break;
                    case 99:
                        nbre27 += 1;
                        break;
                }
            }

            if (fichesCA != 0)
            {
                PercentAccord = Math.Round((nbre1 / fichesCA) * 100, 2);
            }

            if (fichestraites != 0)
            {
               // PercentClotures = Math.Round((fichesclotures / fichestraites) * 100, 2);
                qualifs.Add(new qualification { nom = "ACCORD téléphonique", type = type1, nombre = nbre1, pourcentage = Math.Round((nbre1 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "ACCORD téléphonique Cross", type = type1, nombre = nbre28, pourcentage = Math.Round((nbre28 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Déjà abonné nominativement", type = type2, nombre = nbre2, pourcentage = Math.Round((nbre2 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Déjà abonné dans l'entreprise ou le service", type = type2, nombre = nbre3, pourcentage = Math.Round((nbre3 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Le lit déjà par une revue de presse", type = type2, nombre = nbre4, pourcentage = Math.Round((nbre4 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Déjà abonné à une autre magazine", type = type2, nombre = nbre5, pourcentage = Math.Round((nbre5 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Trop généraliste", type = type2, nombre = nbre6, pourcentage = Math.Round((nbre6 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Trop spécialisé", type = type2, nombre = nbre7, pourcentage = Math.Round((nbre7 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Contenus pas intéressants", type = type2, nombre = nbre8, pourcentage = Math.Round((nbre8 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Trop cher", type = type2, nombre = nbre9, pourcentage = Math.Round((nbre9 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Restriction budgétaire", type = type2, nombre = nbre10, pourcentage = Math.Round((nbre10 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Pas le temps de lire", type = type2, nombre = nbre11, pourcentage = Math.Round((nbre11 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "S'informe exclusivement sur Internet", type = type2, nombre = nbre12, pourcentage = Math.Round((nbre12 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Pas décisionnaire sur les abonnements", type = type2, nombre = nbre13, pourcentage = Math.Round((nbre13 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Pas concerné (changement de fonction, retraité...)", type = type2, nombre = nbre14, pourcentage = Math.Round((nbre14 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Souhaite être rappelé", type = type2, nombre = nbre15, pourcentage = Math.Round((nbre15 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Refus de réponse", type = type2, nombre = nbre16, pourcentage = Math.Round((nbre16 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Post_Appel_Depassé", type = type2, nombre = nbre17, pourcentage = Math.Round((nbre17 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Interlocuteur injoignable", type = type3, nombre = nbre18, pourcentage = Math.Round((nbre18 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Appel raccroché", type = type3, nombre = nbre19, pourcentage = Math.Round((nbre19 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Occupé", type = type3, nombre = nbre20, pourcentage = Math.Round((nbre20 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Faux numéro", type = type3, nombre = nbre21, pourcentage = Math.Round((nbre21 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Absent", type = type3, nombre = nbre22, pourcentage = Math.Round((nbre22 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Répondeur", type = type3, nombre = nbre23, pourcentage = Math.Round((nbre23 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Rappel personnel", type = type3, nombre = nbre24, pourcentage = Math.Round((nbre24 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Relance", type = type3, nombre = nbre25, pourcentage = Math.Round((nbre25 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "recycled record", type = type3, nombre = nbre26, pourcentage = Math.Round((nbre26 / fichestraites) * 100, 2) });
                qualifs.Add(new qualification { nom = "Limite Inaccessible", type = type3, nombre = nbre27, pourcentage = Math.Round((nbre27 / fichestraites) * 100, 2) });
            }
            else
            {
                qualifs.Add(new qualification { nom = "ACCORD téléphonique", type = type1, nombre = nbre1, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "ACCORD téléphonique Cross", type = type1, nombre = nbre28, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Déjà abonné nominativement", type = type2, nombre = nbre2, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Déjà abonné dans l'entreprise ou le service", type = type2, nombre = nbre3, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Le lit déjà par une revue de presse", type = type2, nombre = nbre4, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Déjà abonné à une autre magazine", type = type2, nombre = nbre5, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Trop généraliste", type = type2, nombre = nbre6, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Trop spécialisé", type = type2, nombre = nbre7, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Contenus pas intéressants", type = type2, nombre = nbre8, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Trop cher", type = type2, nombre = nbre9, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Restriction budgétaire", type = type2, nombre = nbre10, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Pas le temps de lire", type = type2, nombre = nbre11, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "S'informe exclusivement sur Internet", type = type2, nombre = nbre12, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Pas décisionnaire sur les abonnements", type = type2, nombre = nbre13, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Pas concerné (changement de fonction, retraité...)", type = type2, nombre = nbre14, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Souhaite être rappelé", type = type2, nombre = nbre15, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Refus de réponse", type = type2, nombre = nbre16, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Post_Appel_Depassé", type = type2, nombre = nbre17, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Interlocuteur injoignable", type = type3, nombre = nbre18, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Appel raccroché", type = type3, nombre = nbre19, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Occupé", type = type3, nombre = nbre20, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Faux numéro", type = type3, nombre = nbre21, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Absent", type = type3, nombre = nbre22, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Répondeur", type = type3, nombre = nbre23, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Rappel personnel", type = type3, nombre = nbre24, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Relance", type = type3, nombre = nbre25, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "recycled record", type = type3, nombre = nbre26, pourcentage = 0 });
                qualifs.Add(new qualification { nom = "Limite Inaccessible", type = type3, nombre = nbre27, pourcentage = 0 });

            }
            if (fichesinjectes != 0)
            {
                TauxExploitation = Math.Round((fichestraites / fichesinjectes) * 100, 2);
            }
            qualifs.Add(new qualification { nom = "Fiche traitées", nombre = fichestraites, pourcentage = 100 });
            qualifs.Add(new qualification { nom = "Fiches Cloturées", nombre = fichesclotures, pourcentage = TauxExploitation });
            qualifs.Add(new qualification { nom = "Contacts Argumentés ", nombre = fichesCA, pourcentage = PercentAccord });
            qualifs.Add(new qualification { nom = "Fiche injectées", nombre = fichesinjectes, pourcentage = 100 });
            return Json(qualifs, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDatesInjGMTPROMO(string SelectedTitre)
        {
            List<SelectListItem> DatesInjectionItems = new List<SelectListItem>();
            var data = db.Details_Activite_PROMO_GMT.Where(a => a.TITRE_OPERATION == SelectedTitre).ToList();
            DatesInjectionItems.Insert(0, new SelectListItem { Text = "Sélectionner date d'injection", Value = "" });
            foreach (var item in data)
            {
                if (!(DatesInjectionItems.Exists(x => x.Value == item.DATE_INJECTION)))
                {
                    if (item.DATE_INJECTION != "" && item.DATE_INJECTION != null)
                    {
                        DatesInjectionItems.Add(new SelectListItem { Text = item.DATE_INJECTION, Value = item.DATE_INJECTION });
                    }
                }
            }

            return Json(new SelectList(DatesInjectionItems, "Value", "Text"));
        }
        public JsonResult GetCodesOpeOfTitresGMTPROMO(string SelectedTitre, string DateInj)
        {
            List<SelectListItem> CodesOpeItems = new List<SelectListItem>();

            var data = db.Details_Activite_PROMO_GMT.Where(a => a.TITRE_OPERATION == SelectedTitre && a.DATE_INJECTION == DateInj).ToList();

            CodesOpeItems.Insert(0, new SelectListItem { Text = "Sélectionner code opération", Value = "" });
            foreach (var item in data)
            {
                if (!(CodesOpeItems.Exists(x => x.Value == item.CODE_OPE)))
                {
                    CodesOpeItems.Add(new SelectListItem { Text = item.CODE_OPE, Value = item.CODE_OPE });
                }

            }
            return Json(new SelectList(CodesOpeItems, "Value", "Text"));
        }

        public JsonResult GetCodesProvGMTPROMO(string SelectedTitre, string codeOperation, string DateInj)
        {
            List<SelectListItem> CodesProvItems = new List<SelectListItem>();

            var data = db.Details_Activite_PROMO_GMT.Where(a => a.TITRE_OPERATION == SelectedTitre && a.CODE_OPE == codeOperation && a.DATE_INJECTION == DateInj).ToList();

            CodesProvItems.Insert(0, new SelectListItem { Text = "Sélectionner code prov-relance", Value = "" });
            foreach (var item in data)
            {
                if (!(CodesProvItems.Exists(x => x.Value == item.CODE_PROV_RELANCE)))
                {
                    if (item.CODE_PROV_RELANCE != "" && item.CODE_PROV_RELANCE != null)
                    {
                        CodesProvItems.Add(new SelectListItem { Text = item.CODE_PROV_RELANCE, Value = item.CODE_PROV_RELANCE });
                    }
                }
            }
            return Json(new SelectList(CodesProvItems, "Value", "Text"));
        }
        #endregion
    }
}
