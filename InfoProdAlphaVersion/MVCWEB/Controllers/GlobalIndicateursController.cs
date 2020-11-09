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
using System.Globalization;


namespace MVCWEB.Controllers
{
    [Authorize(Roles = "Manager,SuperManager")]
    public class GlobalIndicateursController : Controller
    {
        private ReportContext db = new ReportContext();
        IGroupeEmployeeService serviceGroupeEmp;
        IEmployeeService service;
        IGroupeService serviceGroupe;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        #region constructor and security
        public GlobalIndicateursController()
        {
            service = new EmployeeService();
            serviceGroupeEmp = new GroupesEmployeService();
            serviceGroupe = new GroupeService();
        }
        public GlobalIndicateursController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
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

        public JsonResult GetGlobalMensuelValuesParActivite(int SelectedActivite, int SelectedAnnee)
        {
            Employee empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));        
            List<globalmensuel> globalmensuels = new List<globalmensuel>();
            List<Employee> emp = serviceGroupeEmp.getListEmployeeByGroupeId(SelectedActivite);
            List<Employee> empassocies = new List<Employee>();
            foreach (var e in emp)
            {
                if (e.Id != empConnected.Id && e.Roles.Any(r => r.UserId == e.Id && r.RoleId == 3))
                {
                    empassocies.Add(e);
                }
                if (e.Id == empConnected.Id && e.Roles.Any(r => r.UserId == e.Id && r.RoleId == 3))
                {
                    empassocies.Add(e);
                }
            }
            Groupe groupe = serviceGroupe.getById(SelectedActivite);
            //Traitement Table Appel et Attendances
            var appels = new List<Appel>();
            var attendances = new List<AttendanceHermes>();
            foreach (var e in empassocies)
            {
                var appelsass = db.appels.Where(a => a.Id_Hermes == e.IdHermes && a.date.Year == SelectedAnnee).ToList();
                appels.AddRange(appelsass);
                var att = db.attendancesHermes.Where(at => at.Id_Hermes == e.IdHermes && at.Annee == SelectedAnnee).ToList();
                attendances.AddRange(att);
            }
            int m = 11;
            if (SelectedAnnee == DateTime.Now.Year)
            {
                m = DateTime.Now.Month; }
           
                for (var i = 1; i <= m + 1; i++)
                {
                    string mois = CultureInfo.CurrentUICulture.DateTimeFormat.MonthNames[i - 1];
                    double totCA = 0, totAccords = 0, TotEmis = 0;
                    double tauxVenteHeure = 0, TauxConcrétisation = 0;
                    foreach (var app in appels)
                    {
                        if (app.date.Month == i)
                        {
                            TotEmis += app.TotalAppelEmis;
                        }
                    }
                    double dep = 0;
                    double arr = 0;
                    double totLog = 0;
                    foreach (var item in attendances)
                    {
                        if (item.date.Month == i)
                        {
                            if (item.Depart == null)
                            {
                                item.Depart = new DateTime(item.date.Year, item.date.Month, item.date.Day, 17, 0, 0);
                            }
                            dep = (item.Depart.Value).Hour;
                            arr = (item.Arrive.Value).Hour;
                            totLog += ((dep - arr) - 1) * 360000;
                        }
                    }
                    //Traitement Table Details_Activite suivant le groupe

                    if (groupe.nom == "GISI-REAB")
                    {
                        var data = db.Details_Activite_REAB_GISI.Where(d => d.Annee == SelectedAnnee && d.Argued == true).ToList();
                        foreach (var item in data)
                        {
                            DateTime d = DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture);
                            if (d.Month == i)
                            {
                                //  totTraites += 1;
                                totCA += 1;
                                if (item.Positive == true)
                                {
                                    totAccords += 1;
                                }
                            }
                        }
                    }
                    if (groupe.nom == "GISI-PROMO")
                    {
                        var data = db.Details_Activite_PROMO_GISI.Where(d => d.Annee == SelectedAnnee && d.Argued == true).ToList();
                        foreach (var item in data)
                        {
                        DateTime d = DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture);
                        if (d.Month == i)
                            {
                                //  totTraites += 1;
                                totCA += 1;
                                if (item.Positive == true)
                                {
                                    totAccords += 1;
                                }
                            }
                        }
                    }
                    if (groupe.nom == "GMT-REAB")
                    {
                        var data = db.Details_Activite_REAB_GMT.Where(d => d.Annee == SelectedAnnee && d.Argued == true).ToList();
                        foreach (var item in data)
                        {
                        DateTime d = DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture);
                        if (d.Month == i)
                            {
                                //totTraites += 1;                          
                                totCA += 1;
                                if (item.Positive == true)
                                {
                                    totAccords += 1;
                                }
                            }
                        }
                    }
                    if (groupe.nom == "GMT-PROMO")
                    {
                        var data = db.Details_Activite_PROMO_GMT.Where(d => d.Annee == SelectedAnnee && d.Argued == true).ToList();
                        foreach (var item in data)
                        {
                        DateTime d = DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture);
                        if (d.Month == i)
                            {
                                // totTraites += 1;
                                totCA += 1;
                                if (item.Positive == true)
                                {
                                    totAccords += 1;
                                }
                            }
                        }
                    }
                    //Traitement Table DT_INJECTED
                    //string activite = groupe.nom.Replace("-", "_");
                    //var injecteddata = db.DT_INJECTED.Where(inj => inj.Annee == SelectedAnnee && inj.ACTIVITE == activite).ToList();
                    //foreach(var it in injecteddata)
                    //{
                    //    DateTime dateinj = DateTime.ParseExact(it.DATE_INJECTION, "yyyyMMdd", CultureInfo.InvariantCulture);
                    //    if (dateinj.Month == i)
                    //    {
                    //        totInjectes += it.TOTAL;
                    //    }
                    //}
                    //Calcul Taux
                    if (totCA != 0)
                    {
                        TauxConcrétisation = Math.Round(((totAccords / totCA) * 100), 2);
                    }
                    if (totLog != 0)
                    {
                        tauxVenteHeure = Math.Round((totAccords / (totLog / 360000)), 2);
                    }
                    //if (totInjectes != 0)
                    //{
                    //    tauxExploitation = Math.Round(((totTraites / totInjectes) * 100), 2);
                    //}
                    globalmensuels.Add(new globalmensuel { Mois = mois, AppelsEmis = TotEmis, CA = totCA, Accords = totAccords, TauxVenteHeure = tauxVenteHeure, TauxConcrétisation = TauxConcrétisation });
                }
            
            return Json(globalmensuels, JsonRequestBehavior.AllowGet);
        }

      
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
