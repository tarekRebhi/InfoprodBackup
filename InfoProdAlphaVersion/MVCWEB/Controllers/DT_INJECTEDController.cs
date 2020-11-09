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
    public class DT_INJECTEDController : Controller
    {
        private ReportContext db = new ReportContext();
        IGroupeEmployeeService serviceGroupeEmp;
        IEmployeeService service;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        #region constructor and security
        public DT_INJECTEDController()
        {
            service = new EmployeeService();
            serviceGroupeEmp = new GroupesEmployeService();

        }
        public DT_INJECTEDController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
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
        #endregion
        // GET: DT_INJECTED
        public ActionResult Index()
        {
            try
            {
                Employee empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
                var groupesassociees = serviceGroupeEmp.getGroupeByIDEmployee(empConnected.Id);
                var groupesassociees_tests = groupesassociees.Select(o => o.nom).Distinct().ToList();
             
                 List<string> TitresGisiReab = new List<string>();
                List<string> TitresGisiPromo = new List<string>();
                List<string> TitresGmtReab = new List<string>();
                List<string> TitresGmtPromo = new List<string>();
                foreach (var g in groupesassociees_tests)
                {
                    if( g == "GISI-REAB")
                    {
                       TitresGisiReab = db.Details_Activite_REAB_GISI.Select(gp => gp.TITRE_OPERATION).Distinct().ToList();
                    }
                    if (g == "GISI-PROMO")
                    {
                       TitresGisiPromo = db.Details_Activite_PROMO_GISI.Select(gp => gp.TITRE_OPERATION).Distinct().ToList();
                    }
                    if (g == "GMT-REAB")
                    {
                       TitresGmtReab = db.Details_Activite_REAB_GMT.Select(gp => gp.TITRE_OPERATION).Distinct().ToList();
                    }
                    if (g == "GMT-PROMO")
                    {
                    TitresGmtPromo = db.Details_Activite_PROMO_GMT.Select(gp => gp.TITRE_OPERATION).Distinct().ToList();
                    }
                }   
                ViewBag.Groupes = groupesassociees_tests;
                ViewBag.TitresGisiReab = TitresGisiReab;             
                ViewBag.TitresGisiPromo = TitresGisiPromo;
                ViewBag.TitresGmtReab = TitresGmtReab;
                ViewBag.TitresGmtPromo = TitresGmtPromo;

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

        #region GISI REAB injection par titre
        public JsonResult GetValuesGISIREAB(string SelectedTitre, string DateInj)
        {
            double TotFI = 0, TotCA = 0, TotAccords = 0;
            double TauxCA=0, TauxAccords=0, Obj =0, RevsObj=0;
            List<injection> injections = new List<injection>();
            var data = db.Details_Activite_REAB_GISI.Where(a => a.TITRE_OPERATION == SelectedTitre && a.DATE_INJECTION == DateInj).ToList();
            var dataInjected = db.DT_INJECTED.Where(b => b.ACTIVITE == "GISI_REAB" && b.TITRE_OPERATION == SelectedTitre && b.DATE_INJECTION == DateInj).ToList();

            Objectif objectifs = db.Objectifs.FirstOrDefault(ob => ob.Namegroupe == "GISI-REAB");
            if (objectifs != null)
            {
                Obj = objectifs.ObjectifAccords;
            }
            foreach (var it in dataInjected)
            {
                TotFI += it.TOTAL;
            }
            foreach (var item in data) {
                if (item.Argued == true)
                {
                    TotCA += 1;
                }
                if (item.Argued == true && item.Positive == true)
                {
                    TotAccords += 1;
                }
            }
            
            if (TotFI != 0) {
                TauxCA = Math.Round((TotCA / TotFI) * 100, 0);
                TauxAccords = Math.Round((TotAccords / TotFI) * 100, 0);
            }
            if (TauxAccords > Obj) {
                RevsObj = TauxAccords - Obj;
            }
            if (TauxAccords < Obj)
            {
                RevsObj = Obj - TauxAccords;
            }
            injections.Add(new injection { TotFichesInjectes = TotFI , TotCA = TotCA , TotAccords = TotAccords , TauxCA = TauxCA , TauxAccords = TauxAccords, Objectif = Obj, ReelVSObj = RevsObj });

            return Json(injections, JsonRequestBehavior.AllowGet);
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
        public JsonResult GetMensuelValuesGISIREAB(string SelectedMois)
        {
            double Obj = 0;
            List<injection> injections = new List<injection>();
            string month = SelectedMois.Replace("-", "");
            var data = db.Details_Activite_REAB_GISI.Where(a => a.DATE_INJECTION.Substring(0,6) == month).ToList();
            var dataInjected = db.DT_INJECTED.Where(b => b.ACTIVITE == "GISI_REAB" && b.DATE_INJECTION.Substring(0, 6) == month).ToList();
            var Titres = db.Details_Activite_REAB_GISI.Select(t => t.TITRE_OPERATION).Distinct().ToList();
            Objectif objectifs = db.Objectifs.FirstOrDefault(ob => ob.Namegroupe == "GISI-REAB");
            if (objectifs != null)
            {
                Obj = objectifs.ObjectifAccords;
            }
            foreach (var tit in Titres)
            {
                double TotFI = 0, TotCA = 0, TotAccords = 0;
                double TauxCA = 0, TauxAccords = 0, RevsObj = 0;
                string titre = "";
                
                titre = tit;
                foreach (var di in dataInjected)
                {
                    if (di.TITRE_OPERATION == tit)
                    {
                        TotFI += di.TOTAL;
                    }
                }
                foreach (var item in data)
                {
                    if (item.TITRE_OPERATION == tit)
                    {
                        if (item.Argued == true)
                        {
                            TotCA += 1;
                        }
                        if (item.Argued == true && item.Positive == true)
                        {
                            TotAccords += 1;
                        }
                    }
                }
                if (TotFI != 0)
                {
                    TauxCA = Math.Round((TotCA / TotFI) * 100, 0);
                    TauxAccords = Math.Round((TotAccords / TotFI) * 100, 0);
                }
                if (TauxAccords > Obj)
                {
                    RevsObj = TauxAccords - Obj;
                }
                if (TauxAccords < Obj)
                {
                    RevsObj = Obj - TauxAccords;
                }
                injections.Add(new injection {Titre = titre, TotFichesInjectes = TotFI, TotCA = TotCA, TotAccords = TotAccords, TauxCA = TauxCA, TauxAccords = TauxAccords, Objectif = Obj, ReelVSObj = RevsObj });
            }
            return Json(injections, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region GISI PROMO injection par titre
        public JsonResult GetValuesGISIPROMO(string SelectedTitre, string DateInj)
        {
            double TotFI = 0, TotCA = 0, TotAccords = 0;
            double TauxCA = 0, TauxAccords = 0, Obj = 0, RevsObj = 0;
            List<injection> injections = new List<injection>();
            var data = db.Details_Activite_PROMO_GISI.Where(a => a.TITRE_OPERATION == SelectedTitre && a.DATE_INJECTION == DateInj).ToList();
            var dataInjected = db.DT_INJECTED.Where(b => b.ACTIVITE == "GISI_PROMO" && b.TITRE_OPERATION == SelectedTitre && b.DATE_INJECTION == DateInj).ToList();

            Objectif objectifs = db.Objectifs.FirstOrDefault(ob => ob.Namegroupe == "GISI-PROMO");
            if(objectifs != null) {
                Obj = objectifs.ObjectifAccords;
            }       

            foreach (var it in dataInjected)
            {
                TotFI += it.TOTAL;
            }
            foreach (var item in data)
            {
                if (item.Argued == true)
                {
                    TotCA += 1;
                }
                if (item.Argued == true && item.Positive == true)
                {
                    TotAccords += 1;
                }
            }

            if (TotFI != 0)
            {
                TauxCA = Math.Round((TotCA / TotFI) * 100, 0);
                TauxAccords = Math.Round((TotAccords / TotFI) * 100, 0);
            }
            if (TauxAccords > Obj)
            {
                RevsObj = TauxAccords - Obj;
            }
            if (TauxAccords < Obj)
            {
                RevsObj = Obj - TauxAccords;
            }
            injections.Add(new injection { TotFichesInjectes = TotFI, TotCA = TotCA, TotAccords = TotAccords, TauxCA = TauxCA, TauxAccords = TauxAccords, Objectif = Obj, ReelVSObj = RevsObj });

            return Json(injections, JsonRequestBehavior.AllowGet);
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
        public JsonResult GetMensuelValuesGISIPROMO(string SelectedMois)
        {
            double Obj = 0;
            List<injection> injections = new List<injection>();
            string month = SelectedMois.Replace("-", "");
            var data = db.Details_Activite_PROMO_GISI.Where(a => a.DATE_INJECTION.Substring(0, 6) == month).ToList();
            var dataInjected = db.DT_INJECTED.Where(b => b.ACTIVITE == "GISI_PROMO" && b.DATE_INJECTION.Substring(0, 6) == month).ToList();
            var Titres = db.Details_Activite_PROMO_GISI.Select(t => t.TITRE_OPERATION).Distinct().ToList();
            Objectif objectifs = db.Objectifs.FirstOrDefault(ob => ob.Namegroupe == "GISI-PROMO");
            if(objectifs != null) {
                Obj = objectifs.ObjectifAccords;
            }
          
            foreach (var tit in Titres)
            {
                double TotFI = 0, TotCA = 0, TotAccords = 0;
                double TauxCA = 0, TauxAccords = 0, RevsObj = 0;
                string titre = "";

                titre = tit;
                foreach (var di in dataInjected)
                {
                    if (di.TITRE_OPERATION == tit)
                    {
                        TotFI += di.TOTAL;
                    }
                }
                foreach (var item in data)
                {
                    if (item.TITRE_OPERATION == tit)
                    {
                        if (item.Argued == true)
                        {
                            TotCA += 1;
                        }
                        if (item.Argued == true && item.Positive == true)
                        {
                            TotAccords += 1;
                        }
                    }
                }
                if (TotFI != 0)
                {
                    TauxCA = Math.Round((TotCA / TotFI) * 100, 0);
                    TauxAccords = Math.Round((TotAccords / TotFI) * 100, 0);
                }
                if (TauxAccords > Obj)
                {
                    RevsObj = TauxAccords - Obj;
                }
                if (TauxAccords < Obj)
                {
                    RevsObj = Obj - TauxAccords;
                }
                injections.Add(new injection { Titre = titre, TotFichesInjectes = TotFI, TotCA = TotCA, TotAccords = TotAccords, TauxCA = TauxCA, TauxAccords = TauxAccords, Objectif = Obj, ReelVSObj = RevsObj });
            }
            return Json(injections, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region GMT REAB injection par titre
        public JsonResult GetValuesGMTREAB(string SelectedTitre, string DateInj)
        {
            double TotFI = 0, TotCA = 0, TotAccords = 0;
            double TauxCA = 0, TauxAccords = 0, Obj = 0, RevsObj = 0;
            List<injection> injections = new List<injection>();
            var data = db.Details_Activite_REAB_GMT.Where(a => a.TITRE_OPERATION == SelectedTitre && a.DATE_INJECTION == DateInj).ToList();
            var dataInjected = db.DT_INJECTED.Where(b => b.ACTIVITE == "GMT_REAB" && b.TITRE_OPERATION == SelectedTitre && b.DATE_INJECTION == DateInj).ToList();

            Objectif objectifs = db.Objectifs.FirstOrDefault(ob => ob.Namegroupe == "GMT-REAB");
            if (objectifs != null)
            {
                Obj = objectifs.ObjectifAccords;
            }
            foreach (var it in dataInjected)
            {
                TotFI += it.TOTAL;
            }
            foreach (var item in data)
            {
                if (item.Argued == true)
                {
                    TotCA += 1;
                }
                if (item.Argued == true && item.Positive == true)
                {
                    TotAccords += 1;
                }
            }

            if (TotFI != 0)
            {
                TauxCA = Math.Round((TotCA / TotFI) * 100, 0);
                TauxAccords = Math.Round((TotAccords / TotFI) * 100, 0);
            }
            if (TauxAccords > Obj)
            {
                RevsObj = TauxAccords - Obj;
            }
            if (TauxAccords < Obj)
            {
                RevsObj = Obj - TauxAccords;
            }
            injections.Add(new injection { TotFichesInjectes = TotFI, TotCA = TotCA, TotAccords = TotAccords, TauxCA = TauxCA, TauxAccords = TauxAccords, Objectif = Obj, ReelVSObj = RevsObj });

            return Json(injections, JsonRequestBehavior.AllowGet);
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
        public JsonResult GetMensuelValuesGMTREAB(string SelectedMois)
        {
            double Obj = 0;
            List<injection> injections = new List<injection>();
            string month = SelectedMois.Replace("-", "");
            var data = db.Details_Activite_REAB_GMT.Where(a => a.DATE_INJECTION.Substring(0, 6) == month).ToList();
            var dataInjected = db.DT_INJECTED.Where(b => b.ACTIVITE == "GMT_REAB" && b.DATE_INJECTION.Substring(0, 6) == month).ToList();
            var Titres = db.Details_Activite_REAB_GMT.Select(t => t.TITRE_OPERATION).Distinct().ToList();
            Objectif objectifs = db.Objectifs.FirstOrDefault(ob => ob.Namegroupe == "GMT-REAB");
            if (objectifs != null)
            {
                Obj = objectifs.ObjectifAccords;
            }
            foreach (var tit in Titres)
            {
                double TotFI = 0, TotCA = 0, TotAccords = 0;
                double TauxCA = 0, TauxAccords = 0, RevsObj = 0;
                string titre = "";

                titre = tit;
                foreach (var di in dataInjected)
                {
                    if (di.TITRE_OPERATION == tit)
                    {
                        TotFI += di.TOTAL;
                    }
                }
                foreach (var item in data)
                {
                    if (item.TITRE_OPERATION == tit)
                    {
                        if (item.Argued == true)
                        {
                            TotCA += 1;
                        }
                        if (item.Argued == true && item.Positive == true)
                        {
                            TotAccords += 1;
                        }
                    }
                }
                if (TotFI != 0)
                {
                    TauxCA = Math.Round((TotCA / TotFI) * 100, 0);
                    TauxAccords = Math.Round((TotAccords / TotFI) * 100, 0);
                }
                if (TauxAccords > Obj)
                {
                    RevsObj = TauxAccords - Obj;
                }
                if (TauxAccords < Obj)
                {
                    RevsObj = Obj - TauxAccords;
                }
                injections.Add(new injection { Titre = titre, TotFichesInjectes = TotFI, TotCA = TotCA, TotAccords = TotAccords, TauxCA = TauxCA, TauxAccords = TauxAccords, Objectif = Obj, ReelVSObj = RevsObj });
            }
            return Json(injections, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region GMT PROMO injection par titre
        public JsonResult GetValuesGMTPROMO(string SelectedTitre, string DateInj)
        {
            double TotFI = 0, TotCA = 0, TotAccords = 0;
            double TauxCA = 0, TauxAccords = 0, Obj = 0, RevsObj = 0;
            List<injection> injections = new List<injection>();
            var data = db.Details_Activite_PROMO_GMT.Where(a => a.TITRE_OPERATION == SelectedTitre && a.DATE_INJECTION == DateInj).ToList();
            var dataInjected = db.DT_INJECTED.Where(b => b.ACTIVITE == "GMT_PROMO" && b.TITRE_OPERATION == SelectedTitre && b.DATE_INJECTION == DateInj).ToList();

            Objectif objectifs = db.Objectifs.FirstOrDefault(ob => ob.Namegroupe == "GMT-PROMO");
            if (objectifs != null)
            {
                Obj = objectifs.ObjectifAccords;
            }
            foreach (var it in dataInjected)
            {
                TotFI += it.TOTAL;
            }
            foreach (var item in data)
            {
                if (item.Argued == true)
                {
                    TotCA += 1;
                }
                if (item.Argued == true && item.Positive == true)
                {
                    TotAccords += 1;
                }
            }

            if (TotFI != 0)
            {
                TauxCA = Math.Round((TotCA / TotFI) * 100, 0);
                TauxAccords = Math.Round((TotAccords / TotFI) * 100, 0);
            }
            if (TauxAccords > Obj)
            {
                RevsObj = TauxAccords - Obj;
            }
            if (TauxAccords < Obj)
            {
                RevsObj = Obj - TauxAccords;
            }
            injections.Add(new injection { TotFichesInjectes = TotFI, TotCA = TotCA, TotAccords = TotAccords, TauxCA = TauxCA, TauxAccords = TauxAccords, Objectif = Obj, ReelVSObj = RevsObj });

            return Json(injections, JsonRequestBehavior.AllowGet);
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
        public JsonResult GetMensuelValuesGMTPROMO(string SelectedMois)
        {
            double Obj = 0;
            List<injection> injections = new List<injection>();
            string month = SelectedMois.Replace("-", "");
            var data = db.Details_Activite_PROMO_GMT.Where(a => a.DATE_INJECTION.Substring(0, 6) == month).ToList();
            var dataInjected = db.DT_INJECTED.Where(b => b.ACTIVITE == "GMT_PROMO" && b.DATE_INJECTION.Substring(0, 6) == month).ToList();
            var Titres = db.Details_Activite_PROMO_GMT.Select(t => t.TITRE_OPERATION).Distinct().ToList();
            Objectif objectifs = db.Objectifs.FirstOrDefault(ob => ob.Namegroupe == "GMT-PROMO");
            if (objectifs != null)
            {
                Obj = objectifs.ObjectifAccords;
            }
            foreach (var tit in Titres)
            {
                double TotFI = 0, TotCA = 0, TotAccords = 0;
                double TauxCA = 0, TauxAccords = 0, RevsObj = 0;
                string titre = "";

                titre = tit;
                foreach (var di in dataInjected)
                {
                    if (di.TITRE_OPERATION == tit)
                    {
                        TotFI += di.TOTAL;
                    }
                }
                foreach (var item in data)
                {
                    if (item.TITRE_OPERATION == tit)
                    {
                        if (item.Argued == true)
                        {
                            TotCA += 1;
                        }
                        if (item.Argued == true && item.Positive == true)
                        {
                            TotAccords += 1;
                        }
                    }
                }
                if (TotFI != 0)
                {
                    TauxCA = Math.Round((TotCA / TotFI) * 100, 0);
                    TauxAccords = Math.Round((TotAccords / TotFI) * 100, 0);
                }
                if (TauxAccords > Obj)
                {
                    RevsObj = TauxAccords - Obj;
                }
                if (TauxAccords < Obj)
                {
                    RevsObj = Obj - TauxAccords;
                }
                injections.Add(new injection { Titre = titre, TotFichesInjectes = TotFI, TotCA = TotCA, TotAccords = TotAccords, TauxCA = TauxCA, TauxAccords = TauxAccords, Objectif = Obj, ReelVSObj = RevsObj });
            }
            return Json(injections, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}