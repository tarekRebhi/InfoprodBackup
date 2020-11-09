using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVCWEB;
using MVCWEB.Models;

using Domain.Entity;
using Services;
using Data;
using System.Globalization;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace MVCWEB.Controllers
{
    [Authorize(Roles = "Agent,Manager,SuperManager")]
    public class IndicateursController : Controller
    {
        IGroupeEmployeeService serviceGroupeEmp;
        IEmployeeService service;
        static int idEmpConnecte;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        private ReportContext db = new ReportContext();
        public IndicateursController()
        {
            service = new EmployeeService();
            serviceGroupeEmp = new GroupesEmployeService();
            //SelectList Semaine
            var semaines = new List<SelectListItem>();
            for (int m = 1; m <= 52; m++)
            {
                var val = m.ToString();

                semaines.Add(new SelectListItem { Text = "Semaine" + val, Value = val });
            }
            ViewBag.SemaineItems = semaines;
            // SelectList Trimestre
            var timestres = new List<SelectListItem>();
            timestres.Add(new SelectListItem { Text = "Trimestre1", Value = "1" });
            timestres.Add(new SelectListItem { Text = "Trimestre2", Value = "2" });
            timestres.Add(new SelectListItem { Text = "Trimestre3", Value = "3" });
            timestres.Add(new SelectListItem { Text = "Trimestre4", Value = "4" });
            ViewBag.TrimestreItems = timestres;
        }

        public IndicateursController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
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
        // GET: IndexParAgent Journalier View
        [HttpGet]
        public ActionResult Index(int? id)
        {

            var empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (empConnected == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //Employee empConnected = service.getById(user.Id);
            var a = new EmployeeViewModel();
            if (empConnected.Content != null)
            {
                String strbase64 = Convert.ToBase64String(empConnected.Content);
                String empConnectedImage = "data:" + empConnected.ContentType + ";base64," + strbase64;
                a.Url = empConnectedImage;
            }
            a.userName = empConnected.UserName;
            a.pseudoName = empConnected.pseudoName;

            List<Groupe> groupes = serviceGroupeEmp.getGroupeByIDEmployee(id);
            List<SelectListItem> groupesassocies = new List<SelectListItem>();
            // groupesassocies.Insert(0, new SelectListItem { Text = "Sélectionner l'Agent", Value = "0" });
            foreach (var item in groupes)
            {
                List<Employee> emp = serviceGroupeEmp.getListEmployeeByGroupeId(item.Id);
                foreach (var e in emp)
                {
                    string IdHermes = e.IdHermes.ToString();
                    if (e.Id != empConnected.Id)
                    {
                        if (!(groupesassocies.Exists(x => x.Text == e.UserName && x.Value == IdHermes)))
                        {
                            groupesassocies.Add(new SelectListItem { Text = e.UserName, Value = IdHermes });
                        }
                    }
                }
            }
            ViewBag.AgentItems = groupesassocies;
            return View(a);
        }

        //Hebdo View Par Agent
        [HttpGet]
        public ActionResult Hebdo(int? id)
        {
            var empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (empConnected == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //Employee empConnected = service.getById(user.Id);
            var a = new EmployeeViewModel();
            if (empConnected.Content != null)
            {
                String strbase64 = Convert.ToBase64String(empConnected.Content);
                String empConnectedImage = "data:" + empConnected.ContentType + ";base64," + strbase64;
                a.Url = empConnectedImage;
            }
            a.userName = empConnected.UserName;
            a.pseudoName = empConnected.pseudoName;

            List<Groupe> groupes = serviceGroupeEmp.getGroupeByIDEmployee(id);
            List<SelectListItem> groupesassocies = new List<SelectListItem>();
            // groupesassocies.Insert(0, new SelectListItem {Text = "Sélectionner l'Agent", Value = "0" });
            foreach (var item in groupes)
            {
                List<Employee> emp = serviceGroupeEmp.getListEmployeeByGroupeId(item.Id);
                foreach (var e in emp)
                {
                    string IdHermes = e.IdHermes.ToString();
                    if (e.Id != empConnected.Id)
                    {
                        if (!(groupesassocies.Exists(x => x.Text == e.UserName && x.Value == IdHermes)))
                        {
                            groupesassocies.Add(new SelectListItem { Text = e.UserName, Value = IdHermes });
                        }
                    }
                }
            }
            ViewBag.AgentItems = groupesassocies;
            return View(a);
        }

        // Json data et calcul Journalier Par Agent
        public JsonResult GetJournalierValues(string agentSel, string daySel)
        {
            List<Calcul> calculs = new List<Calcul>();
            var appels = db.appels.ToList();
            var temps = db.temps.ToList();
            var attendances = db.attendancesHermes.ToList();
            var events = new List<Event>(); ;
            int IdHermes = int.Parse(agentSel);
            Employee e = service.getByIdHermes(IdHermes);
            List<GroupesEmployees> groupesass = serviceGroupeEmp.getByIDEmployee(e.Id);
            foreach (var grp in groupesass)
            {
                var eventgroupeassocies = db.events.Where(ev => ev.groupes.Any(c => c.Id == grp.groupeId)).ToList();
                events.AddRange(eventgroupeassocies);

            }
            var eventemployeeassocies = db.events.Where(v => v.employeeId == e.Id);
            events.AddRange(eventemployeeassocies);

            var d = new DateTime();
            if ((daySel).Equals(""))
            {
                d = DateTime.Now;
            }
            else
            {
                d = DateTime.Parse(daySel);
            }
            var dates = new List<DateTime>();
            double TotAccord = 0;
            double TotCA = 0;
            double TotCNA = 0;
            double TotAcw = 0;
            // double TotLog = 0;
            double TotPreview = 0;
            double TotPausePerso = 0;
            double TotOccupation = 0;
            double TotCommunication = 0;
            double TotAppelEmis = 0;
            double TotAppelAboutis = 0;
            double TotProdReel = 0;
            // double tempsPresence = 0;
            double TotJourTravaillés = 0;
            double dep = 0;
            double arr = 0;
            double TempsLog = 0;
            double TauxAbs = 0;
            double tempsPlaning = 0;
            double tempsCongé = 0;
            double tempsAutorisation = 0;
            int Joursfériés = 0;
            foreach (var grp in groupesass)
            {
                if (grp.groupe.nom == "GISI-REAB")
                {
                    var data = db.Details_Activite_REAB_GISI.ToList();
                    foreach (var item in data)
                    {
                        int ag = int.Parse(agentSel);
                        if (DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture) == d && item.ID_TV == ag)
                        {
                            if (item.Argued == true)
                            {
                                TotCA = TotCA + 1;
                            }
                            //TotCNA += item.CNA;
                            if (item.Positive == true)
                            {
                                TotAccord += 1;
                            }
                        }
                    }
                }
                else if (grp.groupe.nom == "GISI-PROMO")
                {
                    var data = db.Details_Activite_PROMO_GISI.ToList();
                    foreach (var item in data)
                    {
                        int ag = int.Parse(agentSel);
                        if (DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture) == d && item.ID_TV == ag)
                        {
                            if (item.Argued == true)
                            {
                                TotCA = TotCA + 1;
                            }
                            //TotCNA += item.CNA;
                            if (item.Positive == true)
                            {
                                TotAccord += 1;
                            }
                        }
                    }
                }
                else if (grp.groupe.nom == "GMT-REAB")
                {
                    var data = db.Details_Activite_REAB_GMT.ToList();
                    foreach (var item in data)
                    {
                        int ag = int.Parse(agentSel);
                        if (DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture) == d && item.ID_TV == ag)
                        {
                            if (item.Argued == true)
                            {
                                TotCA = TotCA + 1;
                            }
                            //TotCNA += item.CNA;
                            if (item.Positive == true)
                            {
                                TotAccord += 1;
                            }
                        }
                    }
                }
                else if (grp.groupe.nom == "GMT-PROMO")
                {
                    var data = db.Details_Activite_PROMO_GMT.ToList();
                    foreach (var item in data)
                    {
                        int ag = int.Parse(agentSel);
                        if (DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture) == d && item.ID_TV == ag)
                        {
                            if (item.Argued == true)
                            {
                                TotCA = TotCA + 1;
                            }
                            //TotCNA += item.CNA;
                            if (item.Positive == true)
                            {
                                TotAccord += 1;
                            }
                        }
                    }
                }
            }
                foreach (var item in appels)
                {
                    int ag = int.Parse(agentSel);
                    if ((item.date) == d && item.Id_Hermes == ag)
                    {
                        TotAppelEmis += item.TotalAppelEmis;
                        TotAppelAboutis += item.TotalAppelAboutis;
                        if (!(dates.Exists(x => x == item.date)))
                        {
                            dates.Add(item.date);
                        }
                        TotJourTravaillés = dates.LongCount();
                    }

                }

                foreach (var item in temps)
                {
                    int ag = int.Parse(agentSel);
                    if (item.date == d && item.Id_Hermes == ag)
                    {
                        TotAcw += item.tempsACW;
                        // TotLog += item.tempsLog;
                        TotPreview += item.tempsPreview;
                        TotPausePerso += item.tempsPausePerso;
                        TotOccupation += item.tempscom + item.tempsAtt + item.tempsPreview;
                        TotCommunication += item.tempscom + item.tempsAtt;
                        TotProdReel += (item.tempscom + item.tempsAtt);
                        // tempsPresence += (item.tempsLog / 360000);

                    }
                }

                foreach (var item in attendances)
                {
                    int ag = int.Parse(agentSel);
                    if (item.date == d && item.Id_Hermes == ag)
                    {
                        if (item.Depart != null)
                        {
                            dep = (item.Depart.Value).Hour;
                            arr = (item.Arrive.Value).Hour;
                        }
                        else
                        {
                            item.Depart = new DateTime(item.date.Year, item.date.Month, item.date.Day, 17, 0, 0);
                            dep = (item.Depart.Value).Hour;
                            arr = (item.Arrive.Value).Hour;
                        }
                        // dep = (item.Depart.Value).Hour;
                        // arr = (item.Arrive.Value).Hour;
                        TempsLog += (dep - arr) - 1;
                    }
                }

                foreach (var item in events)
                {
                    if (item.titre == "Planning" && item.dateDebut.Date <= d && item.dateFin.Date >= d)
                    {
                        tempsPlaning += ((item.dateFin.Hour) - (item.dateDebut.Hour) - 1);
                    }
                    if (item.titre == "Congé" && item.dateDebut.Date == d && item.dateFin.Date == d)
                    {
                        tempsCongé += ((item.dateFin.Hour) - (item.dateDebut.Hour) - 1);
                    }
                    if (item.titre == "Autorisation" && item.dateDebut.Date == d && item.dateFin.Date == d)
                    {
                        tempsAutorisation += (item.dateFin.Hour) - (item.dateDebut.Hour);
                    }
                    if (item.titre == "Jours Fériés" && item.dateDebut.Date == d && item.dateFin.Date == d)
                    {
                        Joursfériés += ((item.dateFin.Hour) - (item.dateDebut.Hour) - 1);
                    }

                }
                double tempsPlanifie = tempsPlaning - tempsCongé - tempsAutorisation - Joursfériés;
                if (TempsLog > tempsPlanifie && tempsPlanifie != 0)
                {
                    TempsLog = tempsPlanifie;
                }
                if (tempsPlanifie == 0)
                {
                    TauxAbs = 0;
                }
                else
                {
                    TauxAbs = Math.Round(1 - (TempsLog / tempsPlanifie), 2);
                }

            
            
            //traitement fiches
                calculs.Add(new Calcul { value = TotAppelEmis, name = "Appels Emis" });
                calculs.Add(new Calcul { value = TotAppelAboutis, name = "Appels Aboutis" });
                calculs.Add(new Calcul { value = TotCA, name = "Contact Argumenté" });
                calculs.Add(new Calcul { value = TotAccord, name = "Contact Argumenté Positif" });
                calculs.Add(new Calcul { value = TotCNA, name = "Contact Argumenté Négatif" });

                if (TotJourTravaillés == 0)
                {
                    double MoyenneAccord = 0;
                    double MoyenneAppels = 0;

                    calculs.Add(new Calcul { value = MoyenneAccord, name = "Moyenne CA+" });
                    calculs.Add(new Calcul { value = MoyenneAppels, name = "Moyenne des Appels" });

                }
                else
                {
                    double ETPplanifie = Math.Round((TempsLog / TotJourTravaillés / 8), 2);
                    if (ETPplanifie != 0)
                    {
                        double MoyenneAccord = Math.Round((TotAccord / TotJourTravaillés / ETPplanifie), 2);

                        double MoyenneAppels = Math.Round((TotAppelEmis / TotJourTravaillés / ETPplanifie), 2);

                        calculs.Add(new Calcul { value = MoyenneAccord, name = "Moyenne CA+" });
                        calculs.Add(new Calcul { value = MoyenneAppels, name = "Moyenne des Appels" });
                    }
                    else
                    {
                        double MoyenneAccord = 0;
                        double MoyenneAppels = 0;

                        calculs.Add(new Calcul { value = MoyenneAccord, name = "Moyenne CA+" });
                        calculs.Add(new Calcul { value = MoyenneAppels, name = "Moyenne des Appels" });
                    }

                }

                if (TotCA != 0)
                {
                    double TauxVentesHebdo = Math.Round(((TotAccord / TotCA) * 100), 2);
                    calculs.Add(new Calcul { value = TauxVentesHebdo, name = "Taux De Concrétisation" });
                }
                else
                {
                    double TauxVentesHebdo = 0;
                    calculs.Add(new Calcul { value = TauxVentesHebdo, name = "Taux De Concrétisation" });
                }
                if (TempsLog != 0)
                {
                    double TauxVenteParHeure = Math.Round((TotAccord / TempsLog), 2);
                    calculs.Add(new Calcul { value = TauxVenteParHeure, name = "Taux de Ventes/Heure" });
                }
                else
                {
                    double TauxVenteParHeure = 0;
                    calculs.Add(new Calcul { value = TauxVenteParHeure, name = "Taux de Ventes/Heure" });
                }

                TotProdReel = Math.Round((TotProdReel / 360000), 2);
                //temps présence
                calculs.Add(new Calcul { value = TotJourTravaillés, name = "Nombre des jours travailés" });
                calculs.Add(new Calcul { value = TempsLog, name = "Temps de Présence/Heure" });
                calculs.Add(new Calcul { value = TotProdReel, name = "Temps de Prod réel/Heure" });
                if (TotJourTravaillés != 0)
                {
                    double ETPplanifie = Math.Round((TempsLog / TotJourTravaillés / 8), 2);
                    calculs.Add(new Calcul { value = ETPplanifie, name = "ETP planifié" });
                }
                else
                {
                    double ETPplanifie = 0;
                    calculs.Add(new Calcul { value = ETPplanifie, name = "ETP planifié" });
                }

                //téléphonie
                if (TempsLog != 0)
                {
                    double TauxACWHebdo = Math.Round(((TotAcw / (TempsLog * 360000)) * 100), 2);

                    double TauxPreviewHebdo = Math.Round(((TotPreview / (TempsLog * 360000)) * 100), 2);

                    double TauxPauseBriefHebdo = 0;

                    double TauxPausePersoHebdo = Math.Round(((TotPausePerso / (TempsLog * 360000)) * 100), 2);

                    double TauxOccupation = Math.Round(((TotOccupation / (TempsLog * 360000)) * 100), 2);

                    double TauxComunication = Math.Round(((TotCommunication / (TempsLog * 360000)) * 100), 2);

                    calculs.Add(new Calcul { value = TauxComunication, name = "Taux de Communication" });
                    calculs.Add(new Calcul { value = TauxOccupation, name = "Taux d'occupation" });
                    calculs.Add(new Calcul { value = TauxACWHebdo, name = "Taux ACW" });
                    calculs.Add(new Calcul { value = TauxPreviewHebdo, name = "Taux Preview" });
                    calculs.Add(new Calcul { value = TauxPauseBriefHebdo, name = "Taux Pause Brief" });
                    calculs.Add(new Calcul { value = TauxPausePersoHebdo, name = "Taux Pause Perso" });
                }
                else
                {
                    double TauxACWHebdo = 0;
                    double TauxPreviewHebdo = 0;
                    double TauxPauseBriefHebdo = 0;
                    double TauxPausePersoHebdo = 0;
                    double TauxOccupation = 0;
                    double TauxComunication = 0;

                    
                    calculs.Add(new Calcul { value = TauxComunication, name = "Taux de Communication" });
                    calculs.Add(new Calcul { value = TauxOccupation, name = "Taux d'occupation" });
                    calculs.Add(new Calcul { value = TauxACWHebdo, name = "Taux ACW(Post-Appel)" });
                    calculs.Add(new Calcul { value = TauxPreviewHebdo, name = "Taux Preview" });
                    calculs.Add(new Calcul { value = TauxPauseBriefHebdo, name = "Taux Pause Brief" });
                    calculs.Add(new Calcul { value = TauxPausePersoHebdo, name = "Taux Pause Perso" });
                }
                // calculs.Add(new Calcul { value = tempsCongé, name = "Temps Log" });
                //calculs.Add(new Calcul { value = tempsAutorisation, name = "Temps planifié" });
            calculs.Add(new Calcul { value = TauxAbs, name = "Taux d'absentéisme" });
            TotAcw = Math.Round((TotAcw/ 360000), 2);
            calculs.Add(new Calcul { value = TotAcw, name = "Post Appel" });
            return Json(calculs, JsonRequestBehavior.AllowGet);
        }

        //Json data et calcul spécifiques Hebdo Par Agent
        public JsonResult GetHebdoValues(string agentSel, string semaineSel, string anneeSel)
        {
            List<Calcul> calculs = new List<Calcul>();
            var appels = db.appels.ToList();
            var temps = db.temps.ToList();
            var attendances = db.attendancesHermes.ToList();
            var events = new List<Event>();
            var planings = new List<Event>();
            int IdHermes = int.Parse(agentSel);
            Employee e = service.getByIdHermes(IdHermes);
            List<GroupesEmployees> groupesass = serviceGroupeEmp.getByIDEmployee(e.Id);
            double TotAccord = 0;
            double TotCA = 0;
            double TotCNA = 0;
            double TotAcw = 0;
            double TotPreview = 0;
            double TotPausePerso = 0;
            double TotOccupation = 0;
            double TotCommunication = 0;
            double TotAppelEmis = 0;
            double TotAppelAboutis = 0;
            double TotProdReel = 0;
            double TotJourTravaillés = 0;
            double dep = 0;
            double arr = 0;
            double TempsLog = 0;
            double TauxAbs = 0;
            double tempsPlaning = 0;
            double tempsCongé = 0;
            double tempsAutorisation = 0;
            int Joursfériés = 0;
            int JoursSansWeekend = 0;
            int Joursouvrés = 0;
            double tempsPlanifie = 0;
            var dates = new List<DateTime>();

            var test = 0;
            if (semaineSel == null)
            {
                test = 0;
            }
            else
            {
                test = int.Parse(semaineSel);
            }
            int year = 1990;
            if (anneeSel == null || anneeSel.Equals(""))
            {
                year = 1990;
            }
            else
            {
                year = int.Parse(anneeSel);
            }

            foreach (var grp in groupesass)
            {
                var planningsgroupeassocies = db.events.Where(p => p.groupes.Any(g => g.Id == grp.groupeId) && p.titre == "Planning").ToList();
                planings.AddRange(planningsgroupeassocies);

                var eventgroupeassocies = db.events.Where(ev => ev.groupes.Any(c => c.Id == grp.groupeId) && ev.titre != "Planning").ToList();
                events.AddRange(eventgroupeassocies);

                if(grp.groupe.nom== "GISI-REAB")
                {
                    var data = db.Details_Activite_REAB_GISI.ToList();
                    foreach(var item in data)
                    {
                        int ag = int.Parse(agentSel);
                        int yearr = DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture).Year;
                        if (item.Semaine == test && yearr == year && item.ID_TV == ag)
                        {
                            if (item.Argued == true)
                            {
                                TotCA = TotCA + 1;
                            }
                            //TotCNA += item.CNA;
                            if (item.Positive == true)
                            {
                                TotAccord += 1;
                            }
                        }

                    }
                }

                else if(grp.groupe.nom == "GISI-PROMO")
                {
                    var data = db.Details_Activite_PROMO_GISI.ToList();
                    foreach (var item in data)
                    {
                        int ag = int.Parse(agentSel);
                        int yearr = DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture).Year;
                        if (item.Semaine == test && yearr == year && item.ID_TV == ag)
                        {
                            if (item.Argued == true)
                            {
                                TotCA = TotCA + 1;
                            }
                            //TotCNA += item.CNA;
                            if (item.Positive == true)
                            {
                                TotAccord += 1;
                            }
                        }

                    }
                }
                else if (grp.groupe.nom == "GMT-REAB")
                {
                    var data = db.Details_Activite_REAB_GMT.ToList();
                    foreach (var item in data)
                    {
                        int ag = int.Parse(agentSel);
                        int yearr = DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture).Year;
                        if (item.Semaine == test && yearr == year && item.ID_TV == ag)
                        {
                            if (item.Argued == true)
                            {
                                TotCA = TotCA + 1;
                            }
                            //TotCNA += item.CNA;
                            if (item.Positive == true)
                            {
                                TotAccord += 1;
                            }
                        }

                    }
                }
                else if (grp.groupe.nom == "GMT-PROMO")
                {
                    var data = db.Details_Activite_PROMO_GISI.ToList();
                    foreach (var item in data)
                    {
                        int ag = int.Parse(agentSel);
                        int yearr = DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture).Year;
                        if (item.Semaine == test && yearr == year && item.ID_TV == ag)
                        {
                            if (item.Argued == true)
                            {
                                TotCA = TotCA + 1;
                            }
                            //TotCNA += item.CNA;
                            if (item.Positive == true)
                            {
                                TotAccord += 1;
                            }
                        }

                    }
                }


            }
            var eventemployeeassocies = db.events.Where(v => v.employeeId == e.Id);
            events.AddRange(eventemployeeassocies);
            //var test = 0;
            //if (semaineSel == null)
            //{
            //    test = 0;
            //}
            //else
            //{
            //    test = int.Parse(semaineSel);
            //}
            //int year = 1990;
            //if (anneeSel == null || anneeSel.Equals(""))
            //{
            //    year = 1990;
            //}
            //else
            //{
            //    year = int.Parse(anneeSel);
            //}
            //double TotAccord = 0;
            //double TotCA = 0;
            //double TotCNA = 0;
            //double TotAcw = 0;
            //double TotPreview = 0;
            //double TotPausePerso = 0;
            //double TotOccupation = 0;
            //double TotCommunication = 0;
            //double TotAppelEmis = 0;
            //double TotAppelAboutis = 0;
            //double TotProdReel = 0;
            //double TotJourTravaillés = 0;
            //double dep = 0;
            //double arr = 0;
            //double TempsLog = 0;
            //double TauxAbs = 0;
            //double tempsPlaning = 0;
            //double tempsCongé = 0;
            //double tempsAutorisation = 0;
            //int Joursfériés = 0;
            //int JoursSansWeekend = 0;
            //int Joursouvrés = 0;
            //double tempsPlanifie = 0;
            //var dates = new List<DateTime>();
            foreach (var item in appels)
            {
                int ag = int.Parse(agentSel);
                if (item.semaine == test && item.date.Year == year && item.Id_Hermes == ag)
                {
                    //TotCA += item.CA;
                    //TotCNA += item.CNA;
                    //TotAccord += item.Accords;
                    TotAppelEmis += item.TotalAppelEmis;
                    TotAppelAboutis += item.TotalAppelAboutis;
                    if (!(dates.Exists(x => x == item.date)))
                    {
                        dates.Add(item.date);
                    }
                    TotJourTravaillés = dates.LongCount();
                }
            }
            foreach (var item in temps)
            {
                int ag = int.Parse(agentSel);

                if (item.semaine == test && item.date.Year == year && item.Id_Hermes == ag)
                {
                    TotAcw += item.tempsACW;
                    TotPreview += item.tempsPreview;
                    TotPausePerso += item.tempsPausePerso;
                    TotOccupation += item.tempscom + item.tempsAtt + item.tempsPreview;
                    TotCommunication += item.tempscom + item.tempsAtt;
                    TotProdReel += (item.tempscom + item.tempsAtt);
                }
            }
            foreach (var item in attendances)
            {
                int ag = int.Parse(agentSel);
                if (item.semaine == test && item.date.Year == year && item.Id_Hermes == ag)
                {
                    if (item.Depart != null)
                    {
                        dep = (item.Depart.Value).Hour;
                        arr = (item.Arrive.Value).Hour;
                    }
                    else
                    {
                        item.Depart = new DateTime(item.date.Year, item.date.Month, item.date.Day, 17, 0, 0);
                        dep = (item.Depart.Value).Hour;
                        arr = (item.Arrive.Value).Hour;
                    }
                    // dep = (item.Depart.Value).Hour;
                    // arr = (item.Arrive.Value).Hour;
                    TempsLog += (dep - arr) - 1;
                }
            }
            int week = int.Parse(semaineSel);
            //int year = 2018;
            DayOfWeek day = DayOfWeek.Monday;
            DateTime startOfYear = new DateTime(year, 1, 1);
            int daysToFirstCorrectDay = (((int)day - (int)startOfYear.DayOfWeek) + 7) % 7;
            string fd = "";
            string ed = "";
            foreach (var plan in planings)
            {
                DateTime FirstSem = startOfYear.AddDays(7 * (week - 1) + daysToFirstCorrectDay).AddDays(1);
                DateTime EndSem = startOfYear.AddDays(7 * (week - 1) + daysToFirstCorrectDay).AddDays(5);
                fd = FirstSem.ToString();
                ed = EndSem.ToString();

                if (plan.dateDebut.Date <= FirstSem && plan.dateFin >= EndSem)
                {
                    tempsPlaning = ((plan.dateFin.Hour) - (plan.dateDebut.Hour) - 1);

                    foreach (var item in events)
                    {
                        if (item.titre == "Congé" && item.dateDebut.Date >= FirstSem && item.dateFin <= EndSem)
                        {
                            tempsCongé += ((item.dateFin.Hour) - (item.dateDebut.Hour) - 1);
                        }
                        if (item.titre == "Autorisation" && item.dateDebut.Date >= FirstSem && item.dateFin <= EndSem)
                        {
                            tempsAutorisation += (item.dateFin.Hour) - (item.dateDebut.Hour);
                        }
                        if (item.titre == "Jours Fériés" && item.dateDebut.Date >= FirstSem && item.dateFin <= EndSem)
                        {
                            Joursfériés += 1;
                        }
                    }

                    Joursouvrés = 5 - Joursfériés;
                    tempsPlanifie = (tempsPlaning * Joursouvrés) - tempsCongé - tempsAutorisation;
                }
                //Fin If pour planing include between two dates
                if (plan.dateDebut.Date >= FirstSem && plan.dateFin <= EndSem)
                {
                    tempsPlaning = ((plan.dateFin.Hour) - (plan.dateDebut.Hour) - 1);
                    FirstSem = plan.dateDebut;
                    EndSem = plan.dateFin;
                    foreach (var item in events)
                    {
                        if (item.titre == "Congé" && item.dateDebut.Date >= FirstSem && item.dateFin <= EndSem)
                        {
                            tempsCongé += ((item.dateFin.Hour) - (item.dateDebut.Hour) - 1);
                        }
                        if (item.titre == "Autorisation" && item.dateDebut.Date >= FirstSem && item.dateFin <= EndSem)
                        {
                            tempsAutorisation += (item.dateFin.Hour) - (item.dateDebut.Hour);
                        }
                        if (item.titre == "Jours Fériés" && item.dateDebut.Date >= FirstSem && item.dateFin <= EndSem)
                        {
                            Joursfériés += 1;
                        }
                    }
                    while (FirstSem < EndSem)
                    {
                        if (FirstSem.DayOfWeek != DayOfWeek.Saturday && FirstSem.DayOfWeek != DayOfWeek.Sunday)
                        { JoursSansWeekend++; }
                        FirstSem = FirstSem.AddDays(1);

                    }
                    Joursouvrés = JoursSansWeekend - Joursfériés;
                    tempsPlanifie = (tempsPlaning * Joursouvrés) - tempsCongé - tempsAutorisation;
                }
                //Fin If pour planing include inverse between two dates
                if (plan.dateDebut.Date >= FirstSem && plan.dateFin >= EndSem)
                {

                    tempsPlaning = ((plan.dateFin.Hour) - (plan.dateDebut.Hour) - 1);
                    FirstSem = plan.dateDebut.Date;
                    foreach (var item in events)
                    {
                        if (item.titre == "Congé" && item.dateDebut.Date >= FirstSem && item.dateFin <= EndSem)
                        {
                            tempsCongé += ((item.dateFin.Hour) - (item.dateDebut.Hour) - 1);
                        }
                        if (item.titre == "Autorisation" && item.dateDebut.Date >= FirstSem && item.dateFin <= EndSem)
                        {
                            tempsAutorisation += (item.dateFin.Hour) - (item.dateDebut.Hour);
                        }
                        if (item.titre == "Jours Fériés" && item.dateDebut.Date >= FirstSem && item.dateFin <= EndSem)
                        {
                            Joursfériés += 1;
                        }
                    }
                    while (FirstSem < EndSem)
                    {
                        if (FirstSem.DayOfWeek != DayOfWeek.Saturday && FirstSem.DayOfWeek != DayOfWeek.Sunday)
                        { JoursSansWeekend++; }
                        FirstSem = FirstSem.AddDays(1);

                    }
                    Joursouvrés = JoursSansWeekend - Joursfériés;
                    tempsPlanifie = (tempsPlaning * Joursouvrés) - tempsCongé - tempsAutorisation;
                } //Fin If pour planing date debut > debut trimestre
                if (plan.dateDebut.Date <= FirstSem && plan.dateFin <= EndSem)
                {
                    tempsPlaning = ((plan.dateFin.Hour) - (plan.dateDebut.Hour) - 1);
                    EndSem = plan.dateFin.Date;
                    foreach (var item in events)
                    {
                        if (item.titre == "Congé" && item.dateDebut.Date >= FirstSem && item.dateFin <= EndSem)
                        {
                            tempsCongé += ((item.dateFin.Hour) - (item.dateDebut.Hour) - 1);
                        }
                        if (item.titre == "Autorisation" && item.dateDebut.Date >= FirstSem && item.dateFin <= EndSem)
                        {
                            tempsAutorisation += (item.dateFin.Hour) - (item.dateDebut.Hour);
                        }
                        if (item.titre == "Jours Fériés" && item.dateDebut.Date >= FirstSem && item.dateFin <= EndSem)
                        {
                            Joursfériés += 1;
                        }
                    }
                    while (FirstSem < EndSem)
                    {
                        if (FirstSem.DayOfWeek != DayOfWeek.Saturday && FirstSem.DayOfWeek != DayOfWeek.Sunday)
                        { JoursSansWeekend++; }
                        FirstSem = FirstSem.AddDays(1);
                    }
                    Joursouvrés = JoursSansWeekend - Joursfériés;
                    tempsPlanifie = (tempsPlaning * Joursouvrés) - tempsCongé - tempsAutorisation;
                }
                //Fin If pour planing date Fin < fin trimestre
            }
            if (TempsLog > tempsPlanifie && tempsPlanifie != 0)
            {
                TempsLog = tempsPlanifie;
            }
            if (tempsPlanifie == 0)
            {
                TauxAbs = 0;
            }
            else
            {
                TauxAbs = Math.Round(1 - (TempsLog / tempsPlanifie), 2);
            }
            //tratement fiches
            calculs.Add(new Calcul { value = TotAppelEmis, name = "Appels Emis" });
            calculs.Add(new Calcul { value = TotAppelAboutis, name = "Appels Aboutis" });
            calculs.Add(new Calcul { value = TotCA, name = "Contact Argumenté" });
            calculs.Add(new Calcul { value = TotAccord, name = "Contact Argumenté Positif" });
            calculs.Add(new Calcul { value = TotCNA, name = "Contact Argumenté Négatif" });

            if (TotJourTravaillés == 0)
            {
                double MoyenneAccord = 0;
                double MoyenneAppels = 0;

                calculs.Add(new Calcul { value = MoyenneAccord, name = "Moyenne CA+" });
                calculs.Add(new Calcul { value = MoyenneAppels, name = "Moyenne des Appels" });

            }
            else
            {
                double ETPplanifie = Math.Round((TempsLog / TotJourTravaillés / 8), 2);
                if (ETPplanifie != 0)
                {
                    double MoyenneAccord = Math.Round((TotAccord / TotJourTravaillés / ETPplanifie), 2);

                    double MoyenneAppels = Math.Round((TotAppelEmis / TotJourTravaillés / ETPplanifie), 2);

                    calculs.Add(new Calcul { value = MoyenneAccord, name = "Moyenne CA+" });
                    calculs.Add(new Calcul { value = MoyenneAppels, name = "Moyenne des Appels" });
                }
                else
                {
                    double MoyenneAccord = 0;
                    double MoyenneAppels = 0;

                    calculs.Add(new Calcul { value = MoyenneAccord, name = "Moyenne CA+" });
                    calculs.Add(new Calcul { value = MoyenneAppels, name = "Moyenne des Appels" });
                }

            }

            if (TotCA != 0)
            {
                double TauxVentesHebdo = Math.Round(((TotAccord / TotCA) * 100), 2);
                calculs.Add(new Calcul { value = TauxVentesHebdo, name = "Taux De Concrétisation" });
            }
            else
            {
                double TauxVentesHebdo = 0;
                calculs.Add(new Calcul { value = TauxVentesHebdo, name = "Taux De Concrétisation" });
            }
            if (TempsLog != 0)
            {
                double TauxVenteParHeure = Math.Round((TotAccord / TempsLog), 2);
                calculs.Add(new Calcul { value = TauxVenteParHeure, name = "Taux de Ventes/Heure" });
            }
            else
            {
                double TauxVenteParHeure = 0;
                calculs.Add(new Calcul { value = TauxVenteParHeure, name = "Taux de Ventes/Heure" });
            }

            TotProdReel = Math.Round((TotProdReel / 360000), 2);
            //temps présence
            calculs.Add(new Calcul { value = TotJourTravaillés, name = "Nombre des jours travailés" });
            calculs.Add(new Calcul { value = TempsLog, name = "Temps de Présence/Heure" });
            calculs.Add(new Calcul { value = TotProdReel, name = "Temps de Prod réel/Heure" });
            if (TotJourTravaillés != 0)
            {
                double ETPplanifie = Math.Round((TempsLog / TotJourTravaillés / 8), 2);
                calculs.Add(new Calcul { value = ETPplanifie, name = "ETP planifié" });
            }
            else
            {
                double ETPplanifie = 0;
                calculs.Add(new Calcul { value = ETPplanifie, name = "ETP planifié" });
            }

            //téléphonie
            if (TempsLog != 0)
            {
                double TauxACWHebdo = Math.Round(((TotAcw / (TempsLog * 360000)) * 100), 2);

                double TauxPreviewHebdo = Math.Round(((TotPreview / (TempsLog * 360000)) * 100), 2);

                double TauxPauseBriefHebdo = 0;

                double TauxPausePersoHebdo = Math.Round(((TotPausePerso / (TempsLog * 360000)) * 100), 2);

                double TauxOccupation = Math.Round(((TotOccupation / (TempsLog * 360000)) * 100), 2);

                double TauxComunication = Math.Round(((TotCommunication / (TempsLog * 360000)) * 100), 2);

                calculs.Add(new Calcul { value = TauxComunication, name = "Taux de Communication" });
                calculs.Add(new Calcul { value = TauxOccupation, name = "Taux d'occupation" });
                calculs.Add(new Calcul { value = TauxACWHebdo, name = "Taux ACW" });
                calculs.Add(new Calcul { value = TauxPreviewHebdo, name = "Taux Preview" });
                calculs.Add(new Calcul { value = TauxPauseBriefHebdo, name = "Taux Pause Brief" });
                calculs.Add(new Calcul { value = TauxPausePersoHebdo, name = "Taux Pause Perso" });
            }
            else
            {
                double TauxACWHebdo = 0;
                double TauxPreviewHebdo = 0;
                double TauxPauseBriefHebdo = 0;
                double TauxPausePersoHebdo = 0;
                double TauxOccupation = 0;
                double TauxComunication = 0;

                calculs.Add(new Calcul { value = TauxComunication, name = "Taux de Communication" });
                calculs.Add(new Calcul { value = TauxOccupation, name = "Taux d'occupation" });
                calculs.Add(new Calcul { value = TauxACWHebdo, name = "Taux ACW(Post-Appel)" });
                calculs.Add(new Calcul { value = TauxPreviewHebdo, name = "Taux Preview" });
                calculs.Add(new Calcul { value = TauxPauseBriefHebdo, name = "Taux Pause Brief" });
                calculs.Add(new Calcul { value = TauxPausePersoHebdo, name = "Taux Pause Perso" });
            }

            TotAcw = Math.Round((TotAcw / 360000), 2);
            calculs.Add(new Calcul { value = TauxAbs, name = "Taux d'absentéisme" });
            calculs.Add(new Calcul { value = TotAcw, name = "Post Appel" });
            return Json(calculs, JsonRequestBehavior.AllowGet);
        }

        // View Mensuelle Par Agent
        [HttpGet]
        public ActionResult Mensuel(int? id)
        {
            var empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (empConnected == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //Employee empConnected = service.getById(user.Id);
            var a = new EmployeeViewModel();
            if (empConnected.Content != null)
            {
                String strbase64 = Convert.ToBase64String(empConnected.Content);
                String empConnectedImage = "data:" + empConnected.ContentType + ";base64," + strbase64;
                a.Url = empConnectedImage;
            }
            a.userName = empConnected.UserName;
            a.pseudoName = empConnected.pseudoName;

            List<Groupe> groupes = serviceGroupeEmp.getGroupeByIDEmployee(id);
            List<SelectListItem> groupesassocies = new List<SelectListItem>();
            // groupesassocies.Insert(0, new SelectListItem { Text = "Sélectionner l'Agent", Value = "0" });
            foreach (var item in groupes)
            {
                List<Employee> emp = serviceGroupeEmp.getListEmployeeByGroupeId(item.Id);
                foreach (var e in emp)
                {
                    string IdHermes = e.IdHermes.ToString();
                    if (e.Id != empConnected.Id)
                    {
                        if (!(groupesassocies.Exists(x => x.Text == e.UserName && x.Value == IdHermes)))
                        {
                            groupesassocies.Add(new SelectListItem { Text = e.UserName, Value = IdHermes });
                        }
                    }
                }
            }
            ViewBag.AgentItems = groupesassocies;
            return View(a);
        }

        //Json data et calcul spécifiques Mensuelle Par Agent
        public JsonResult GetMensuelValues(string moisSel, string agentSel)
        {
            List<Calcul> calculs = new List<Calcul>();
            var appels = db.appels.ToList();
            var temps = db.temps.ToList();
            var attendances = db.attendancesHermes.ToList();
            var events = new List<Event>();
            var planings = new List<Event>();
            int IdHermes = int.Parse(agentSel);
            Employee e = service.getByIdHermes(IdHermes);
            List<GroupesEmployees> groupesass = serviceGroupeEmp.getByIDEmployee(e.Id);

            double TotAccord = 0;
            double TotCA = 0;
            double TotCNA = 0;
            double TotAcw = 0;
            double TotPreview = 0;
            double TotPausePerso = 0;
            double TotOccupation = 0;
            double TotCommunication = 0;
            double TotAppelEmis = 0;
            double TotAppelAboutis = 0;
            double TotProdReel = 0;
            double TotJourTravaillés = 0;
            double dep = 0;
            double arr = 0;
            double TempsLog = 0;
            double TauxAbs = 0;
            double tempsPlanifie = 0;
            double tempsPlaning = 0;
            double tempsCongé = 0;
            double tempsAutorisation = 0;
            int JoursSansWeekend = 0;
            int Joursfériés = 0;
            int Joursouvrés = 0;
            var dates = new List<DateTime>();
            int yearSel = 0;
            int monthSel = 0;
            if (moisSel == null || moisSel.Equals(""))
            {
                yearSel = 0;
                monthSel = 0;
            }
            else
            {
                yearSel = int.Parse(moisSel.Substring(0, 4));
                monthSel = int.Parse(moisSel.Substring(5, 2));
            }

            var a = new DateTime(1900, 1, 1);
            if (moisSel == null || moisSel.Equals(""))
            {
                a = new DateTime(1900, 1, 1);
            }
            else
            {
                a = DateTime.Parse(moisSel);
            }
            int daysmonth = DateTime.DaysInMonth(a.Year, a.Month);
            // string daysnum = daysmonth.ToString();


            foreach (var grp in groupesass)
            {
                var planningsgroupeassocies = db.events.Where(p => p.groupes.Any(g => g.Id == grp.groupeId) && p.titre == "Planning").ToList();
                planings.AddRange(planningsgroupeassocies);

                var eventgroupeassocies = db.events.Where(ev => ev.groupes.Any(c => c.Id == grp.groupeId) && ev.titre != "Planning").ToList();
                events.AddRange(eventgroupeassocies);

                if(grp.groupe.nom=="GISI-REAB")
                {
                    
                    var data = db.Details_Activite_REAB_GISI.ToList();
                    foreach(var item in data)
                    {
                        int ag = int.Parse(agentSel);
                        int year = DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture).Year;
                        int month = DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture).Month;
                        if (year == yearSel && month == monthSel && item.ID_TV == ag)
                        {
                            if (item.Argued == true)
                            {
                                TotCA = TotCA + 1;
                            }
                            //TotCNA += item.CNA;
                            if (item.Positive == true)
                            {
                                TotAccord += 1;
                            }
                        }
                    }
                }

                if (grp.groupe.nom == "GISI-PROMO")
                {

                    var data = db.Details_Activite_PROMO_GISI.ToList();
                    foreach (var item in data)
                    {
                        int ag = int.Parse(agentSel);
                        int year = DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture).Year;
                        int month = DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture).Month;
                        if (year == yearSel && month == monthSel && item.ID_TV == ag)
                        {
                            if (item.Argued == true)
                            {
                                TotCA = TotCA + 1;
                            }
                            //TotCNA += item.CNA;
                            if (item.Positive == true)
                            {
                                TotAccord += 1;
                            }
                        }
                    }
                }

                if (grp.groupe.nom == "GMT-REAB")
                {

                    var data = db.Details_Activite_REAB_GMT.ToList();

                    foreach (var item in data)
                    {
                        int ag = int.Parse(agentSel);
                        int year = DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture).Year;
                        int month = DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture).Month;
                        if (year == yearSel && month == monthSel && item.ID_TV == ag)
                        {
                            if (item.Argued == true)
                            {
                                TotCA = TotCA + 1;
                            }
                            //TotCNA += item.CNA;
                            if (item.Positive == true)
                            {
                                TotAccord += 1;
                            }
                        }
                    }
                }


                if (grp.groupe.nom == "GMT-PROMO")
                {

                    var data = db.Details_Activite_PROMO_GMT.ToList();
                    foreach (var item in data)
                    {
                        int ag = int.Parse(agentSel);
                        int year = DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture).Year;
                        int month = DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture).Month;
                        if (year == yearSel && month == monthSel && item.ID_TV == ag)
                        {
                            if (item.Argued == true)
                            {
                                TotCA = TotCA + 1;
                            }
                            //TotCNA += item.CNA;
                            if (item.Positive == true)
                            {
                                TotAccord += 1;
                            }
                        }
                    }
                }
                //if (grp.groupe.nom == "GMT-REAB")
                //{

                //    var data = db.Details_Activite_REAB_GMT.ToList();
                //    foreach (var item in data)
                //    {
                //        int ag = int.Parse(agentSel);
                //        int year = DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture).Year;
                //        int month = DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture).Month;
                //        if (year == yearSel && month == monthSel && item.ID_TV == ag)
                //        {
                //            if (item.Argued == true)
                //            {
                //                TotCA = TotCA + 1;
                //            }
                //            //TotCNA += item.CNA;
                //            if (item.Positive == true)
                //            {
                //                TotAccord += 1;
                //            }
                //        }
                //    }
                //}


            }
            var eventemployeeassocies = db.events.Where(v => v.employeeId == e.Id);
            events.AddRange(eventemployeeassocies);
            //double TotAccord = 0;
            //double TotCA = 0;
            //double TotCNA = 0;
            //double TotAcw = 0;
            //double TotPreview = 0;
            //double TotPausePerso = 0;
            //double TotOccupation = 0;
            //double TotCommunication = 0;
            //double TotAppelEmis = 0;
            //double TotAppelAboutis = 0;
            //double TotProdReel = 0;
            //double TotJourTravaillés = 0;
            //double dep = 0;
            //double arr = 0;
            //double TempsLog = 0;
            //double TauxAbs = 0;
            //double tempsPlanifie = 0;
            //double tempsPlaning = 0;
            //double tempsCongé = 0;
            //double tempsAutorisation = 0;
            //int JoursSansWeekend = 0;
            //int Joursfériés = 0;
            //int Joursouvrés = 0;
            //var dates = new List<DateTime>();
            //int yearSel = 0;
            //int monthSel = 0;
            //if (moisSel == null || moisSel.Equals(""))
            //{
            //    yearSel = 0;
            //    monthSel = 0;
            //}
            //else
            //{
            //    yearSel = int.Parse(moisSel.Substring(0, 4));
            //    monthSel = int.Parse(moisSel.Substring(5, 2));
            //}

            //var a = new DateTime(1900, 1, 1);
            //if (moisSel == null || moisSel.Equals(""))
            //{
            //    a = new DateTime(1900, 1, 1);
            //}
            //else
            //{
            //    a = DateTime.Parse(moisSel);
            //}
            //int daysmonth = DateTime.DaysInMonth(a.Year, a.Month);
            //// string daysnum = daysmonth.ToString();

            foreach (var item in appels)
            {
                string ym = item.date.ToString("yyyy-MM");
                int ag = int.Parse(agentSel);
                if (item.date.Year == yearSel && item.date.Month == monthSel && item.Id_Hermes == ag)
                {
                    //TotCA += item.CA;
                    //TotCNA += item.CNA;
                    //TotAccord += item.Accords;
                    TotAppelEmis += item.TotalAppelEmis;
                    TotAppelAboutis += item.TotalAppelAboutis;
                    if (!(dates.Exists(x => x == item.date)))
                    {
                        dates.Add(item.date);
                    }
                    TotJourTravaillés = dates.LongCount();
                }
            }
            foreach (var item in temps)
            {
                string ym = item.date.ToString("yyyy-MM");
                int ag = int.Parse(agentSel);
                if (ym == moisSel && item.Id_Hermes == ag)
                {
                    TotAcw += item.tempsACW;
                    TotPreview += item.tempsPreview;
                    TotPausePerso += item.tempsPausePerso;
                    TotOccupation += item.tempscom + item.tempsAtt + item.tempsPreview;
                    TotCommunication += item.tempscom + item.tempsAtt;
                    TotProdReel += (item.tempscom + item.tempsAtt);
                }
            }
            foreach (var item in attendances)
            {
                string ym = item.date.ToString("yyyy-MM");
                int ag = int.Parse(agentSel);
                if (ym == moisSel && item.Id_Hermes == ag)
                {
                    if (item.Depart == null)
                    {
                        item.Depart = new DateTime(item.date.Year, item.date.Month, item.date.Day, 17, 0, 0);
                    }
                    dep = (item.Depart.Value).Hour;
                    arr = (item.Arrive.Value).Hour;
                    TempsLog += (dep - arr) - 1;
                }
            }
            string f = "";
            string end = "";
            foreach (var plan in planings)
            {
                DateTime firstMonth = new DateTime(a.Year, a.Month, 1);
                DateTime endMonth = new DateTime(a.Year, a.Month, daysmonth);
                f = firstMonth.ToString();
                end = endMonth.ToString();
                if (plan.dateDebut.Date <= firstMonth && plan.dateFin >= endMonth)
                {
                    tempsPlaning = ((plan.dateFin.Hour) - (plan.dateDebut.Hour) - 1);

                    foreach (var item in events)
                    {
                        if (item.titre == "Congé" && item.dateDebut.Date >= firstMonth && item.dateFin <= endMonth)
                        {
                            tempsCongé += ((item.dateFin.Hour) - (item.dateDebut.Hour) - 1);
                        }
                        if (item.titre == "Autorisation" && item.dateDebut.Date >= firstMonth && item.dateFin <= endMonth)
                        {
                            tempsAutorisation += (item.dateFin.Hour) - (item.dateDebut.Hour);
                        }
                        if (item.titre == "Jours Fériés" && item.dateDebut.Date >= firstMonth && item.dateFin <= endMonth)
                        {
                            Joursfériés += 1;
                        }
                    }
                    for (int i = 1; i <= DateTime.DaysInMonth(a.Year, a.Month); i++)
                    {
                        DateTime thisDay = new DateTime(a.Year, a.Month, i);
                        if (thisDay.DayOfWeek != DayOfWeek.Saturday && thisDay.DayOfWeek != DayOfWeek.Sunday)
                        {
                            JoursSansWeekend += 1;
                        }
                    }
                    Joursouvrés = JoursSansWeekend - Joursfériés;
                    tempsPlanifie = (tempsPlaning * Joursouvrés) - tempsCongé - tempsAutorisation;
                }
                //Fin If pour planing include between two dates
                if (plan.dateDebut.Date >= firstMonth && plan.dateFin <= endMonth)
                {
                    tempsPlaning = ((plan.dateFin.Hour) - (plan.dateDebut.Hour) - 1);
                    firstMonth = plan.dateDebut;
                    endMonth = plan.dateFin;
                    foreach (var item in events)
                    {
                        if (item.titre == "Congé" && item.dateDebut.Date >= firstMonth && item.dateFin <= endMonth)
                        {
                            tempsCongé += ((item.dateFin.Hour) - (item.dateDebut.Hour) - 1);
                        }
                        if (item.titre == "Autorisation" && item.dateDebut.Date >= firstMonth && item.dateFin <= endMonth)
                        {
                            tempsAutorisation += (item.dateFin.Hour) - (item.dateDebut.Hour);
                        }
                        if (item.titre == "Jours Fériés" && item.dateDebut.Date >= firstMonth && item.dateFin <= endMonth)
                        {
                            Joursfériés += 1;
                        }
                    }
                    while (firstMonth < endMonth)
                    {
                        if (firstMonth.DayOfWeek != DayOfWeek.Saturday && firstMonth.DayOfWeek != DayOfWeek.Sunday)
                        { JoursSansWeekend++; }
                        firstMonth = firstMonth.AddDays(1);

                    }
                    Joursouvrés = JoursSansWeekend - Joursfériés;
                    tempsPlanifie = (tempsPlaning * Joursouvrés) - tempsCongé - tempsAutorisation;
                }
                //Fin If pour planing include inverse between two dates
                if (plan.dateDebut.Date >= firstMonth && plan.dateFin >= endMonth)
                {

                    tempsPlaning = ((plan.dateFin.Hour) - (plan.dateDebut.Hour) - 1);
                    firstMonth = plan.dateDebut.Date;
                    foreach (var item in events)
                    {
                        if (item.titre == "Congé" && item.dateDebut.Date >= firstMonth && item.dateFin <= endMonth)
                        {
                            tempsCongé += ((item.dateFin.Hour) - (item.dateDebut.Hour) - 1);
                        }
                        if (item.titre == "Autorisation" && item.dateDebut.Date >= firstMonth && item.dateFin <= endMonth)
                        {
                            tempsAutorisation += (item.dateFin.Hour) - (item.dateDebut.Hour);
                        }
                        if (item.titre == "Jours Fériés" && item.dateDebut.Date >= firstMonth && item.dateFin <= endMonth)
                        {
                            Joursfériés += 1;
                        }
                    }
                    while (firstMonth < endMonth)
                    {
                        if (firstMonth.DayOfWeek != DayOfWeek.Saturday && firstMonth.DayOfWeek != DayOfWeek.Sunday)
                        { JoursSansWeekend++; }
                        firstMonth = firstMonth.AddDays(1);

                    }
                    Joursouvrés = JoursSansWeekend - Joursfériés;
                    tempsPlanifie = (tempsPlaning * Joursouvrés) - tempsCongé - tempsAutorisation;
                } //Fin If pour planing date debut > debut trimestre
                if (plan.dateDebut.Date <= firstMonth && plan.dateFin <= endMonth)
                {
                    tempsPlaning = ((plan.dateFin.Hour) - (plan.dateDebut.Hour) - 1);
                    endMonth = plan.dateFin.Date;
                    foreach (var item in events)
                    {
                        if (item.titre == "Congé" && item.dateDebut.Date >= firstMonth && item.dateFin <= endMonth)
                        {
                            tempsCongé += ((item.dateFin.Hour) - (item.dateDebut.Hour) - 1);
                        }
                        if (item.titre == "Autorisation" && item.dateDebut.Date >= firstMonth && item.dateFin <= endMonth)
                        {
                            tempsAutorisation += (item.dateFin.Hour) - (item.dateDebut.Hour);
                        }
                        if (item.titre == "Jours Fériés" && item.dateDebut.Date >= firstMonth && item.dateFin <= endMonth)
                        {
                            Joursfériés += 1;
                        }
                    }
                    while (firstMonth < endMonth)
                    {
                        if (firstMonth.DayOfWeek != DayOfWeek.Saturday && firstMonth.DayOfWeek != DayOfWeek.Sunday)
                        { JoursSansWeekend++; }
                        firstMonth = firstMonth.AddDays(1);
                    }
                    Joursouvrés = JoursSansWeekend - Joursfériés;
                    tempsPlanifie = (tempsPlaning * Joursouvrés) - tempsCongé - tempsAutorisation;
                }
                //Fin If pour planing date Fin < fin trimestre
            }

            if (TempsLog > tempsPlanifie && tempsPlanifie != 0)
            {
                TempsLog = tempsPlanifie;
            }
            if (tempsPlanifie == 0)
            {
                TauxAbs = 0;
            }

            else
            {
                TauxAbs = Math.Round(1 - (TempsLog / tempsPlanifie), 2);
            }
            //traitement
            calculs.Add(new Calcul { value = TotAppelEmis, name = "Appels Emis" });
            calculs.Add(new Calcul { value = TotAppelAboutis, name = "Appels Aboutis" });
            calculs.Add(new Calcul { value = TotCA, name = "Contact Argumenté" });
            calculs.Add(new Calcul { value = TotAccord, name = "Contact Argumenté Positif" });
            calculs.Add(new Calcul { value = TotCNA, name = "Contact Argumenté Négatif" });
            if (TotJourTravaillés == 0)
            {
                double MoyenneAccord = 0;
                double MoyenneAppels = 0;
                calculs.Add(new Calcul { value = MoyenneAccord, name = "Moyenne CA+" });
                calculs.Add(new Calcul { value = MoyenneAppels, name = "Moyenne des Appels" });
            }
            else
            {
                double ETPplanifie = Math.Round((TempsLog / TotJourTravaillés / 8), 2);
                if (ETPplanifie != 0)
                {
                    double MoyenneAccord = Math.Round((TotAccord / TotJourTravaillés / ETPplanifie), 2);

                    double MoyenneAppels = Math.Round((TotAppelEmis / TotJourTravaillés / ETPplanifie), 2);

                    calculs.Add(new Calcul { value = MoyenneAccord, name = "Moyenne CA+" });
                    calculs.Add(new Calcul { value = MoyenneAppels, name = "Moyenne des Appels" });
                }
                else
                {
                    double MoyenneAccord = 0;
                    double MoyenneAppels = 0;

                    calculs.Add(new Calcul { value = MoyenneAccord, name = "Moyenne CA+" });
                    calculs.Add(new Calcul { value = MoyenneAppels, name = "Moyenne des Appels" });
                }

            }

            if (TotCA != 0)
            {
                double TauxVentesHebdo = Math.Round(((TotAccord / TotCA) * 100), 2);
                calculs.Add(new Calcul { value = TauxVentesHebdo, name = "Taux De Concrétisation" });
            }
            else
            {
                double TauxVentesHebdo = 0;
                calculs.Add(new Calcul { value = TauxVentesHebdo, name = "Taux De Concrétisation" });
            }
            if (TempsLog != 0)
            {
                double TauxVenteParHeure = Math.Round((TotAccord / TempsLog), 2);
                calculs.Add(new Calcul { value = TauxVenteParHeure, name = "Taux de Ventes/Heure" });
            }
            else
            {
                double TauxVenteParHeure = 0;
                calculs.Add(new Calcul { value = TauxVenteParHeure, name = "Taux de Ventes/Heure" });
            }
            //temps présence
            TotProdReel = Math.Round((TotProdReel / 360000), 2);
            calculs.Add(new Calcul { value = TotJourTravaillés, name = "Nombre des jours travailés" });
            calculs.Add(new Calcul { value = TempsLog, name = "Temps de Présence/Heure" });
            calculs.Add(new Calcul { value = TotProdReel, name = "Temps de Prod réel/Heure" });
            if (TotJourTravaillés != 0)
            {
                double ETPplanifie = Math.Round((TempsLog / TotJourTravaillés / 8), 2);
                calculs.Add(new Calcul { value = ETPplanifie, name = "ETP planifié" });
            }
            else
            {
                double ETPplanifie = 0;
                calculs.Add(new Calcul { value = ETPplanifie, name = "ETP planifié" });
            }

            //téléphonie
            if (TempsLog != 0)
            {
                double TauxACWHebdo = Math.Round(((TotAcw / (TempsLog * 360000)) * 100), 2);

                double TauxPreviewHebdo = Math.Round(((TotPreview / (TempsLog * 360000)) * 100), 2);

                double TauxPauseBriefHebdo = 0;

                double TauxPausePersoHebdo = Math.Round(((TotPausePerso / (TempsLog * 360000)) * 100), 2);

                double TauxOccupation = Math.Round(((TotOccupation / (TempsLog * 360000)) * 100), 2);

                double TauxComunication = Math.Round(((TotCommunication / (TempsLog * 360000)) * 100), 2);

                calculs.Add(new Calcul { value = TauxComunication, name = "Taux de Communication" });
                calculs.Add(new Calcul { value = TauxOccupation, name = "Taux d'occupation" });
                calculs.Add(new Calcul { value = TauxACWHebdo, name = "Taux ACW" });
                calculs.Add(new Calcul { value = TauxPreviewHebdo, name = "Taux Preview" });
                calculs.Add(new Calcul { value = TauxPauseBriefHebdo, name = "Taux Pause Brief" });
                calculs.Add(new Calcul { value = TauxPausePersoHebdo, name = "Taux Pause Perso" });
            }
            else
            {
                double TauxACWHebdo = 0;
                double TauxPreviewHebdo = 0;
                double TauxPauseBriefHebdo = 0;
                double TauxPausePersoHebdo = 0;
                double TauxOccupation = 0;
                double TauxComunication = 0;

                calculs.Add(new Calcul { value = TauxComunication, name = "Taux de Communication" });
                calculs.Add(new Calcul { value = TauxOccupation, name = "Taux d'occupation" });
                calculs.Add(new Calcul { value = TauxACWHebdo, name = "Taux ACW(Post-Appel)" });
                calculs.Add(new Calcul { value = TauxPreviewHebdo, name = "Taux Preview" });
                calculs.Add(new Calcul { value = TauxPauseBriefHebdo, name = "Taux Pause Brief" });
                calculs.Add(new Calcul { value = TauxPausePersoHebdo, name = "Taux Pause Perso" });
            }
            calculs.Add(new Calcul { value = TauxAbs, name = "Taux d'absentéisme" });
            TotAcw = Math.Round((TotAcw / 360000), 2);
            calculs.Add(new Calcul { value = TotAcw, name = "Post Appel" });
            return Json(calculs, JsonRequestBehavior.AllowGet);
        }

        //View Annuelle
        [HttpGet]
        public ActionResult Annuelle(int? id)
        {
            var empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (empConnected == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //Employee empConnected = service.getById(user.Id);
            var a = new EmployeeViewModel();
            if (empConnected.Content != null)
            {
                String strbase64 = Convert.ToBase64String(empConnected.Content);
                String empConnectedImage = "data:" + empConnected.ContentType + ";base64," + strbase64;
                a.Url = empConnectedImage;
            }
            a.userName = empConnected.UserName;
            a.pseudoName = empConnected.pseudoName;

            List<Groupe> groupes = serviceGroupeEmp.getGroupeByIDEmployee(id);
            List<SelectListItem> groupesassocies = new List<SelectListItem>();
            // groupesassocies.Insert(0, new SelectListItem { Text = "Sélectionner l'Agent", Value = "0" });
            foreach (var item in groupes)
            {
                List<Employee> emp = serviceGroupeEmp.getListEmployeeByGroupeId(item.Id);
                foreach (var e in emp)
                {
                    string IdHermes = e.IdHermes.ToString();
                    if (e.Id != empConnected.Id)
                    {
                        if (!(groupesassocies.Exists(x => x.Text == e.UserName && x.Value == IdHermes)))
                        {
                            groupesassocies.Add(new SelectListItem { Text = e.UserName, Value = IdHermes });
                        }
                    }
                }
            }
            ViewBag.AgentItems = groupesassocies;
            return View(a);
        }

        //Json data et calcul spécifiques Annuelle Par Agent
        public JsonResult GetAnnuelValues(string anneeSel, string agentSel)
        {
            List<Calcul> calculs = new List<Calcul>();
            var appels = db.appels.ToList();
            var temps = db.temps.ToList();
            var attendances = db.attendancesHermes.ToList();
            var events = new List<Event>(); ;
            int IdHermes = int.Parse(agentSel);
            Employee e = service.getByIdHermes(IdHermes);
            List<GroupesEmployees> groupesass = serviceGroupeEmp.getByIDEmployee(e.Id);

            double TotAccord = 0;
            double TotCA = 0;
            double TotCNA = 0;
            double TotAcw = 0;
            // double TotLog = 0;
            double TotPreview = 0;
            double TotPausePerso = 0;
            double TotOccupation = 0;
            double TotCommunication = 0;
            double TotAppelEmis = 0;
            double TotAppelAboutis = 0;
            double TotProdReel = 0;
            // double tempsPresence = 0;
            double TotJourTravaillés = 0;
            double dep = 0;
            double arr = 0;
            double TempsLog = 0;
            double TauxAbs = 0;
            double tempsPlaning = 0;
            double tempsCongé = 0;
            double tempsAutorisation = 0;
            int JoursSansWeekend = 0;
            int Joursfériés = 0;
            int Joursouvrés = 0;
            var dates = new List<DateTime>();

            var test = 0;

            if (anneeSel == null || anneeSel.Equals(""))
            {
                test = 0;
            }
            else
            {
                test = int.Parse(anneeSel);
            }


            foreach (var grp in groupesass)
            {
                var eventgroupeassocies = db.events.Where(ev => ev.groupes.Any(c => c.Id == grp.groupeId)).ToList();
                events.AddRange(eventgroupeassocies);

                if (grp.groupe.nom == "GISI-REAB")
                {
                    var data = db.Details_Activite_REAB_GISI.ToList();
                    foreach (var item in data)
                    {
                        int year = DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture).Year;
                        int ag = int.Parse(agentSel);
                        if (year == test && item.ID_TV == ag)
                        {
                            if (item.Argued == true)
                            {
                                TotCA += 1;
                            }
                            if (item.Positive == true)
                            {
                                TotAccord += 1;
                            }

                        }
                    }

                }
                if (grp.groupe.nom == "GISI-PROMO")
                {
                    var data = db.Details_Activite_PROMO_GISI.ToList();
                    foreach (var item in data)
                    {
                        int year = DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture).Year;
                        int ag = int.Parse(agentSel);
                        if (year == test && item.ID_TV == ag)
                        {
                            if (item.Argued == true)
                            {
                                TotCA += 1;
                            }
                            if (item.Positive == true)
                            {
                                TotAccord += 1;
                            }

                        }
                    }

                }
                if (grp.groupe.nom == "GMT-REAB")
                {
                    var data = db.Details_Activite_REAB_GMT.ToList();
                    foreach (var item in data)
                    {
                        int year = DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture).Year;
                        int ag = int.Parse(agentSel);
                        if (year == test && item.ID_TV == ag)
                        {
                            if (item.Argued == true)
                            {
                                TotCA += 1;
                            }
                            if (item.Positive == true)
                            {
                                TotAccord += 1;
                            }

                        }
                    }

                }
                if (grp.groupe.nom == "GMT-PROMO")
                {
                    var data = db.Details_Activite_PROMO_GMT.ToList();
                    foreach (var item in data)
                    {
                        int year = DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture).Year;
                        int ag = int.Parse(agentSel);
                        if (year == test && item.ID_TV == ag)
                        {
                            if (item.Argued == true)
                            {
                                TotCA += 1;
                            }
                            if (item.Positive == true)
                            {
                                TotAccord += 1;
                            }

                        }
                    }

                }
            }

                var eventemployeeassocies = db.events.Where(v => v.employeeId == e.Id);
                events.AddRange(eventemployeeassocies);
            
                //var test = 0;

                //if (anneeSel == null || anneeSel.Equals(""))
                //{
                //    test = 0;
                //}
                //else
                //{
                //    test = int.Parse(anneeSel);
                //}
                //double TotAccord = 0;
                //double TotCA = 0;
                //double TotCNA = 0;
                //double TotAcw = 0;
                //// double TotLog = 0;
                //double TotPreview = 0;
                //double TotPausePerso = 0;
                //double TotOccupation = 0;
                //double TotCommunication = 0;
                //double TotAppelEmis = 0;
                //double TotAppelAboutis = 0;
                //double TotProdReel = 0;
                //// double tempsPresence = 0;
                //double TotJourTravaillés = 0;
                //double dep = 0;
                //double arr = 0;
                //double TempsLog = 0;
                //double TauxAbs = 0;
                //double tempsPlaning = 0;
                //double tempsCongé = 0;
                //double tempsAutorisation = 0;
                //int JoursSansWeekend = 0;
                //int Joursfériés = 0;
                //int Joursouvrés = 0;
                //var dates = new List<DateTime>();
                foreach (var item in appels)
                {
                    int ag = int.Parse(agentSel);
                    if (item.date.Year == test && item.Id_Hermes == ag)
                    {
                        //TotCA += item.CA;
                        //TotCNA += item.CNA;
                        //TotAccord += item.Accords;
                        TotAppelEmis += item.TotalAppelEmis;
                        TotAppelAboutis += item.TotalAppelAboutis;
                        if (!(dates.Exists(x => x == item.date)))
                        {
                            dates.Add(item.date);
                        }
                        TotJourTravaillés = dates.LongCount();
                    }
                }
                foreach (var item in temps)
                {
                    int ag = int.Parse(agentSel);
                    if (item.date.Year == test && item.Id_Hermes == ag)
                    {
                        TotAcw += item.tempsACW;
                        //TotLog += item.tempsLog;
                        TotPreview += item.tempsPreview;
                        TotPausePerso += item.tempsPausePerso;
                        TotOccupation += item.tempscom + item.tempsAtt + item.tempsPreview;
                        TotCommunication += item.tempscom + item.tempsAtt;
                        TotProdReel += (item.tempscom + item.tempsAtt);
                        // tempsPresence += (item.tempsLog / 360000);
                    }
                }
                foreach (var item in attendances)
                {
                    string ym = item.date.ToString("yyyy-MM");
                    int ag = int.Parse(agentSel);
                    if (item.date.Year == test && item.Id_Hermes == ag)
                    {
                        if (item.Depart == null)
                        {
                            item.Depart = new DateTime(item.date.Year, item.date.Month, item.date.Day, 17, 0, 0);
                        }
                        dep = (item.Depart.Value).Hour;
                        arr = (item.Arrive.Value).Hour;
                        TempsLog += (dep - arr) - 1;
                    }
                }

                foreach (var item in events)
                {
                    if (item.titre == "Planning" && item.dateDebut.Year == test && item.dateFin.Year == test)
                    {
                        tempsPlaning += ((item.dateFin.Hour) - (item.dateDebut.Hour) - 1);
                    }
                    if (item.titre == "Congé" && item.dateDebut.Year == test && item.dateFin.Year == test)
                    {
                        tempsCongé += ((item.dateFin.Hour) - (item.dateDebut.Hour) - 1);
                    }
                    if (item.titre == "Autorisation" && item.dateDebut.Year == test && item.dateFin.Year == test)
                    {
                        tempsAutorisation += (item.dateFin.Hour) - (item.dateDebut.Hour);
                    }
                    if (item.titre == "Jours Fériés" && item.dateDebut.Year == test && item.dateFin.Year == test)
                    {
                        Joursfériés += 1;
                    }
                }
                //calcul des jours ouvrés par année
                DateTime from = new DateTime(1990, 01, 01);
                DateTime to = new DateTime(1990, 12, 31);

                if (anneeSel == null || anneeSel.Equals(""))
                {
                    from = new DateTime(1990, 01, 01);
                    to = new DateTime(1990, 12, 31);
                }
                else
                {
                    // int y = int.Parse(anneeSel);
                    from = new DateTime(test, 01, 01);
                    to = new DateTime(test, 12, 31);
                }
                for (var date = from; date < to; date = date.AddDays(1))
                {
                    if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                    {
                        JoursSansWeekend += 1;
                    }
                }
                Joursouvrés = JoursSansWeekend - Joursfériés;
                double tempsPlanifie = (tempsPlaning * Joursouvrés) - tempsCongé - tempsAutorisation;
                if (TempsLog > tempsPlanifie && tempsPlanifie != 0)
                {
                    TempsLog = tempsPlanifie;
                }
                if (tempsPlanifie == 0)
                {
                    TauxAbs = 0;
                }
                else
                {
                    TauxAbs = Math.Round(1 - (TempsLog / tempsPlanifie), 2);
                }
                //traitement fiches
                calculs.Add(new Calcul { value = TotAppelEmis, name = "Appels Emis" });
                calculs.Add(new Calcul { value = TotAppelAboutis, name = "Appels Aboutis" });
                calculs.Add(new Calcul { value = TotCA, name = "Contact Argumenté" });
                calculs.Add(new Calcul { value = TotAccord, name = "Contact Argumenté Positif" });
                calculs.Add(new Calcul { value = TotCNA, name = "Contact Argumenté Négatif" });

                if (TotJourTravaillés == 0)
                {
                    double MoyenneAccord = 0;
                    double MoyenneAppels = 0;
                    calculs.Add(new Calcul { value = MoyenneAccord, name = "Moyenne CA+" });
                    calculs.Add(new Calcul { value = MoyenneAppels, name = "Moyenne des Appels" });

                }
                else
                {
                    double ETPplanifie = Math.Round((TempsLog / TotJourTravaillés / 8), 2);
                    if (ETPplanifie != 0)
                    {
                        double MoyenneAccord = Math.Round((TotAccord / TotJourTravaillés / ETPplanifie), 2);

                        double MoyenneAppels = Math.Round((TotAppelEmis / TotJourTravaillés / ETPplanifie), 2);

                        calculs.Add(new Calcul { value = MoyenneAccord, name = "Moyenne CA+" });
                        calculs.Add(new Calcul { value = MoyenneAppels, name = "Moyenne des Appels" });
                    }
                    else
                    {
                        double MoyenneAccord = 0;
                        double MoyenneAppels = 0;

                        calculs.Add(new Calcul { value = MoyenneAccord, name = "Moyenne CA+" });
                        calculs.Add(new Calcul { value = MoyenneAppels, name = "Moyenne des Appels" });
                    }

                }

                if (TotCA != 0)
                {
                    double TauxVentesHebdo = Math.Round(((TotAccord / TotCA) * 100), 2);
                    calculs.Add(new Calcul { value = TauxVentesHebdo, name = "Taux De Concrétisation" });
                }
                else
                {
                    double TauxVentesHebdo = 0;
                    calculs.Add(new Calcul { value = TauxVentesHebdo, name = "Taux De Concrétisation" });
                }
                if (TempsLog != 0)
                {
                    double TauxVenteParHeure = Math.Round((TotAccord / TempsLog), 2);
                    calculs.Add(new Calcul { value = TauxVenteParHeure, name = "Taux de Ventes/Heure" });
                }
                else
                {
                    double TauxVenteParHeure = 0;
                    calculs.Add(new Calcul { value = TauxVenteParHeure, name = "Taux de Ventes/Heure" });
                }

                TotProdReel = Math.Round((TotProdReel / 360000), 2);
                //temps présence
                calculs.Add(new Calcul { value = TotJourTravaillés, name = "Nombre des jours travailés" });
                calculs.Add(new Calcul { value = TempsLog, name = "Temps de Présence/Heure" });
                calculs.Add(new Calcul { value = TotProdReel, name = "Temps de Prod réel/Heure" });
                if (TotJourTravaillés != 0)
                {
                    double ETPplanifie = Math.Round((TempsLog / TotJourTravaillés / 8), 2);
                    calculs.Add(new Calcul { value = ETPplanifie, name = "ETP planifié" });
                }
                else
                {
                    double ETPplanifie = 0;
                    calculs.Add(new Calcul { value = ETPplanifie, name = "ETP planifié" });
                }

                //téléphonie
                if (TempsLog != 0)
                {
                    double TauxACWHebdo = Math.Round(((TotAcw / (TempsLog * 360000)) * 100), 2);

                    double TauxPreviewHebdo = Math.Round(((TotPreview / (TempsLog * 360000)) * 100), 2);

                    double TauxPauseBriefHebdo = 0;

                    double TauxPausePersoHebdo = Math.Round(((TotPausePerso / (TempsLog * 360000)) * 100), 2);

                    double TauxOccupation = Math.Round(((TotOccupation / (TempsLog * 360000)) * 100), 2);

                    double TauxComunication = Math.Round(((TotCommunication / (TempsLog * 360000)) * 100), 2);

                    calculs.Add(new Calcul { value = TauxComunication, name = "Taux de Communication" });
                    calculs.Add(new Calcul { value = TauxOccupation, name = "Taux d'occupation" });
                    calculs.Add(new Calcul { value = TauxACWHebdo, name = "Taux ACW" });
                    calculs.Add(new Calcul { value = TauxPreviewHebdo, name = "Taux Preview" });
                    calculs.Add(new Calcul { value = TauxPauseBriefHebdo, name = "Taux Pause Brief" });
                    calculs.Add(new Calcul { value = TauxPausePersoHebdo, name = "Taux Pause Perso" });
                }
                else
                {
                    double TauxACWHebdo = 0;
                    double TauxPreviewHebdo = 0;
                    double TauxPauseBriefHebdo = 0;
                    double TauxPausePersoHebdo = 0;
                    double TauxOccupation = 0;
                    double TauxComunication = 0;

                    calculs.Add(new Calcul { value = TauxComunication, name = "Taux de Communication" });
                    calculs.Add(new Calcul { value = TauxOccupation, name = "Taux d'occupation" });
                    calculs.Add(new Calcul { value = TauxACWHebdo, name = "Taux ACW(Post-Appel)" });
                    calculs.Add(new Calcul { value = TauxPreviewHebdo, name = "Taux Preview" });
                    calculs.Add(new Calcul { value = TauxPauseBriefHebdo, name = "Taux Pause Brief" });
                    calculs.Add(new Calcul { value = TauxPausePersoHebdo, name = "Taux Pause Perso" });
                }
                calculs.Add(new Calcul { value = TauxAbs, name = "Taux d'absentéisme" });
                TotAcw = Math.Round((TotAcw / 360000), 2);
                calculs.Add(new Calcul { value = TotAcw, name = "Post Appel" });
            
            return Json(calculs, JsonRequestBehavior.AllowGet);
            
        }

        // View Trimestrielle
        [HttpGet]
        public ActionResult Trimestriel(int? id)
        {
            var empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (empConnected == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //Employee empConnected = service.getById(user.Id);
            var a = new EmployeeViewModel();
            if (empConnected.Content != null)
            {
                String strbase64 = Convert.ToBase64String(empConnected.Content);
                String empConnectedImage = "data:" + empConnected.ContentType + ";base64," + strbase64;
                a.Url = empConnectedImage;
            }
            a.userName = empConnected.UserName;
            a.pseudoName = empConnected.pseudoName;

            List<Groupe> groupes = serviceGroupeEmp.getGroupeByIDEmployee(id);
            List<SelectListItem> groupesassocies = new List<SelectListItem>();
            // groupesassocies.Insert(0, new SelectListItem { Text = "Sélectionner l'Agent", Value = "0" });
            foreach (var item in groupes)
            {
                List<Employee> emp = serviceGroupeEmp.getListEmployeeByGroupeId(item.Id);
                foreach (var e in emp)
                {
                    string IdHermes = e.IdHermes.ToString();
                    if (e.Id != empConnected.Id)
                    {
                        if (!(groupesassocies.Exists(x => x.Text == e.UserName && x.Value == IdHermes)))
                        {
                            groupesassocies.Add(new SelectListItem { Text = e.UserName, Value = IdHermes });
                        }
                    }
                }
            }
            ViewBag.AgentItems = groupesassocies;
            return View(a);
        }

        //Json data et calcul spécifiques Trimestriel Par Agent
        public JsonResult GetTrimestrielValues(string trimestreSel, string agentSel, string anneeSel)
        {
            List<Calcul> calculs = new List<Calcul>();
            var appels = db.appels.ToList();
            var temps = db.temps.ToList();
            var attendances = db.attendancesHermes.ToList();
            var planings = new List<Event>();
            var events = new List<Event>();
            int IdHermes = int.Parse(agentSel);
            Employee e = service.getByIdHermes(IdHermes);
            List<GroupesEmployees> groupesass = serviceGroupeEmp.getByIDEmployee(e.Id);

            double TotAccord = 0;
            double TotCA = 0;
            double TotCNA = 0;
            double TotAcw = 0;
            double TotPreview = 0;
            double TotPausePerso = 0;
            double TotOccupation = 0;
            double TotCommunication = 0;
            double TotAppelEmis = 0;
            double TotAppelAboutis = 0;
            double TotProdReel = 0;
            double TotJourTravaillés = 0;
            double dep = 0;
            double arr = 0;
            double TempsLog = 0;
            double tempsPlaning = 0;
            double tempsCongé = 0;
            double tempsAutorisation = 0;
            int JoursSansWeekend = 0;
            int Joursfériés = 0;
            int Joursouvrés = 0;
            double TauxAbs = 0;
            double tempsPlanifie = 0;
            var dates = new List<DateTime>();

            var test = 0;
            if (trimestreSel == null || trimestreSel.Equals(""))
            {
                test = 0;
            }
            else
            {
                test = int.Parse(trimestreSel);
            }
            int year = 1990;
            if (anneeSel == null || anneeSel.Equals(""))
            {
                year = 1990;
            }
            else
            {
                year = int.Parse(anneeSel);
            }
            int ag = int.Parse(agentSel);

            foreach (var grp in groupesass)
            {
                var planningsgroupeassocies = db.events.Where(p => p.groupes.Any(g => g.Id == grp.groupeId) && p.titre == "Planning").ToList();
                planings.AddRange(planningsgroupeassocies);

                var eventgroupeassocies = db.events.Where(ev => ev.groupes.Any(c => c.Id == grp.groupeId) && ev.titre != "Planning").ToList();
                events.AddRange(eventgroupeassocies);

                if (grp.groupe.nom == "GISI-REAB")
                {
                    var data = db.Details_Activite_REAB_GISI.ToList();
                    switch (test)
                    {
                        case 1:
                            foreach (var item in data)
                            {
                                int yearr = DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture).Year;
                                int month = DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture).Month;
                                if (item.ID_TV == ag && yearr == year)
                                {
                                    if (month == 1 || month == 2 || month == 3)
                                    {
                                        if (item.Argued == true)
                                        {
                                            TotCA += 1;
                                        }
                                        if (item.Positive == true)
                                            TotAccord += 1;
                                    }
                                }
                            }

                            break;
                        case 2:
                            foreach (var item in data)
                            {
                                int yearr = DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture).Year;
                                int month = DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture).Month;
                                if (item.ID_TV == ag && yearr == year)
                                {
                                    if (month == 4 || month == 5 || month == 6)
                                    {
                                        if (item.Argued == true)
                                        {
                                            TotCA += 1;
                                        }
                                        if (item.Positive == true)
                                            TotAccord += 1;
                                    }
                                }
                            }
                            break;
                        case 3:
                            foreach (var item in data)
                            {
                                int yearr = DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture).Year;
                                int month = DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture).Month;
                                if (item.ID_TV == ag && yearr == year)
                                {
                                    if (month == 7 || month == 8 || month == 9)
                                    {
                                        if (item.Argued == true)
                                        {
                                            TotCA += 1;
                                        }
                                        if (item.Positive == true)
                                            TotAccord += 1;
                                    }
                                }
                            }
                            break;
                        case 4:
                            foreach (var item in data)
                            {
                                int yearr = DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture).Year;
                                int month = DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture).Month;
                                if (item.ID_TV == ag && yearr == year)
                                {
                                    if (month == 10 || month == 11 || month == 12)
                                    {
                                        if (item.Argued == true)
                                        {
                                            TotCA += 1;
                                        }
                                        if (item.Positive == true)
                                            TotAccord += 1;
                                    }
                                }
                            }
                            break;

                    }
                }

                if (grp.groupe.nom == "GISI-PROMO")
                {
                    var data = db.Details_Activite_PROMO_GISI.ToList();
                    switch (test)
                    {
                        case 1:
                            foreach (var item in data)
                            {
                                int yearr = DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture).Year;
                                int month = DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture).Month;
                                if (item.ID_TV == ag && yearr == year)
                                {
                                    if (month == 1 || month == 2 || month == 3)
                                    {
                                        if (item.Argued == true)
                                        {
                                            TotCA += 1;
                                        }
                                        if (item.Positive == true)
                                            TotAccord += 1;
                                    }
                                }
                            }

                            break;
                        case 2:
                            foreach (var item in data)
                            {
                                int yearr = DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture).Year;
                                int month = DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture).Month;
                                if (item.ID_TV == ag && yearr == year)
                                {
                                    if (month == 4 || month == 5 || month == 6)
                                    {
                                        if (item.Argued == true)
                                        {
                                            TotCA += 1;
                                        }
                                        if (item.Positive == true)
                                            TotAccord += 1;
                                    }
                                }
                            }
                            break;
                        case 3:
                            foreach (var item in data)
                            {
                                int yearr = DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture).Year;
                                int month = DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture).Month;
                                if (item.ID_TV == ag && yearr == year)
                                {
                                    if (month == 7 || month == 8 || month == 9)
                                    {
                                        if (item.Argued == true)
                                        {
                                            TotCA += 1;
                                        }
                                        if (item.Positive == true)
                                            TotAccord += 1;
                                    }
                                }
                            }
                            break;
                        case 4:
                            foreach (var item in data)
                            {
                                int yearr = DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture).Year;
                                int month = DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture).Month;
                                if (item.ID_TV == ag && yearr == year)
                                {
                                    if (month == 10 || month == 11 || month == 12)
                                    {
                                        if (item.Argued == true)
                                        {
                                            TotCA += 1;
                                        }
                                        if (item.Positive == true)
                                            TotAccord += 1;
                                    }
                                }
                            }
                            break;

                    }
                }

                if (grp.groupe.nom == "GMT-REAB")
                {
                    var data = db.Details_Activite_REAB_GMT.ToList();
                    switch (test)
                    {
                        case 1:
                            foreach (var item in data)
                            {
                                int yearr = DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture).Year;
                                int month = DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture).Month;
                                if (item.ID_TV == ag && yearr == year)
                                {
                                    if (month == 1 || month == 2 || month == 3)
                                    {
                                        if (item.Argued == true)
                                        {
                                            TotCA += 1;
                                        }
                                        if (item.Positive == true)
                                            TotAccord += 1;
                                    }
                                }
                            }

                            break;
                        case 2:
                            foreach (var item in data)
                            {
                                int yearr = DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture).Year;
                                int month = DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture).Month;
                                if (item.ID_TV == ag && yearr == year)
                                {
                                    if (month == 4 || month == 5 || month == 6)
                                    {
                                        if (item.Argued == true)
                                        {
                                            TotCA += 1;
                                        }
                                        if (item.Positive == true)
                                            TotAccord += 1;
                                    }
                                }
                            }
                            break;
                        case 3:
                            foreach (var item in data)
                            {
                                int yearr = DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture).Year;
                                int month = DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture).Month;
                                if (item.ID_TV == ag && yearr == year)
                                {
                                    if (month == 7 || month == 8 || month == 9)
                                    {
                                        if (item.Argued == true)
                                        {
                                            TotCA += 1;
                                        }
                                        if (item.Positive == true)
                                            TotAccord += 1;
                                    }
                                }
                            }
                            break;
                        case 4:
                            foreach (var item in data)
                            {
                                int yearr = DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture).Year;
                                int month = DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture).Month;
                                if (item.ID_TV == ag && yearr == year)
                                {
                                    if (month == 10 || month == 11 || month == 12)
                                    {
                                        if (item.Argued == true)
                                        {
                                            TotCA += 1;
                                        }
                                        if (item.Positive == true)
                                            TotAccord += 1;
                                    }
                                }
                            }
                            break;

                    }
                }

                if (grp.groupe.nom == "GMT-PROMO")
                {
                    var data = db.Details_Activite_PROMO_GMT.ToList();
                    switch (test)
                    {
                        case 1:
                            foreach (var item in data)
                            {
                                int yearr = DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture).Year;
                                int month = DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture).Month;
                                if (item.ID_TV == ag && yearr == year)
                                {
                                    if (month == 1 || month == 2 || month == 3)
                                    {
                                        if (item.Argued == true)
                                        {
                                            TotCA += 1;
                                        }
                                        if (item.Positive == true)
                                            TotAccord += 1;
                                    }
                                }
                            }

                            break;
                        case 2:
                            foreach (var item in data)
                            {
                                int yearr = DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture).Year;
                                int month = DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture).Month;
                                if (item.ID_TV == ag && yearr == year)
                                {
                                    if (month == 4 || month == 5 || month == 6)
                                    {
                                        if (item.Argued == true)
                                        {
                                            TotCA += 1;
                                        }
                                        if (item.Positive == true)
                                            TotAccord += 1;
                                    }
                                }
                            }
                            break;
                        case 3:
                            foreach (var item in data)
                            {
                                int yearr = DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture).Year;
                                int month = DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture).Month;
                                if (item.ID_TV == ag && yearr == year)
                                {
                                    if (month == 7 || month == 8 || month == 9)
                                    {
                                        if (item.Argued == true)
                                        {
                                            TotCA += 1;
                                        }
                                        if (item.Positive == true)
                                            TotAccord += 1;
                                    }
                                }
                            }
                            break;
                        case 4:
                            foreach (var item in data)
                            {
                                int yearr = DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture).Year;
                                int month = DateTime.ParseExact(item.DATE, "yyyyMMdd", CultureInfo.InvariantCulture).Month;
                                if (item.ID_TV == ag && yearr == year)
                                {
                                    if (month == 10 || month == 11 || month == 12)
                                    {
                                        if (item.Argued == true)
                                        {
                                            TotCA += 1;
                                        }
                                        if (item.Positive == true)
                                            TotAccord += 1;
                                    }
                                }
                            }
                            break;

                    }
                }




            }
            var eventemployeeassocies = db.events.Where(v => v.employeeId == e.Id);
            events.AddRange(eventemployeeassocies);

            //double TotAccord = 0;
            //double TotCA = 0;
            //double TotCNA = 0;
            //double TotAcw = 0;
            //double TotPreview = 0;
            //double TotPausePerso = 0;
            //double TotOccupation = 0;
            //double TotCommunication = 0;
            //double TotAppelEmis = 0;
            //double TotAppelAboutis = 0;
            //double TotProdReel = 0;
            //double TotJourTravaillés = 0;
            //double dep = 0;
            //double arr = 0;
            //double TempsLog = 0;
            //double tempsPlaning = 0;
            //double tempsCongé = 0;
            //double tempsAutorisation = 0;
            //int JoursSansWeekend = 0;
            //int Joursfériés = 0;
            //int Joursouvrés = 0;
            //double TauxAbs = 0;
            //double tempsPlanifie = 0;
            //var dates = new List<DateTime>();

            //var test = 0;
            //if (trimestreSel == null || trimestreSel.Equals(""))
            //{
            //    test = 0;
            //}
            //else
            //{
            //    test = int.Parse(trimestreSel);
            //}
            //int year = 1990;
            //if (anneeSel == null || anneeSel.Equals(""))
            //{
            //    year = 1990;
            //}
            //else
            //{
            //    year = int.Parse(anneeSel);
            //}
            //int ag = int.Parse(agentSel);
            switch (test)
            {
                case 1:
                    foreach (var item in appels)
                    {
                        if (item.Id_Hermes == ag && item.date.Year == year)
                        {
                            if (item.date.Month == 1 || item.date.Month == 2 || item.date.Month == 3)
                            {
                                //TotCA += item.CA;
                                //TotCNA += item.CNA;
                                //TotAccord += item.Accords;
                                TotAppelEmis += item.TotalAppelEmis;
                                TotAppelAboutis += item.TotalAppelAboutis;
                                if (!(dates.Exists(x => x == item.date)))
                                {
                                    dates.Add(item.date);
                                }
                                TotJourTravaillés = dates.LongCount();
                            }
                        }
                    }
                    foreach (var item in temps)
                    {
                        if (item.Id_Hermes == ag && item.date.Year == year)
                        {
                            if (item.date.Month == 1 || item.date.Month == 2 || item.date.Month == 3)
                            {
                                TotAcw += item.tempsACW;
                                //TotLog += item.tempsLog;
                                TotPreview += item.tempsPreview;
                                TotPausePerso += item.tempsPausePerso;
                                TotOccupation += item.tempscom + item.tempsAtt + item.tempsPreview;
                                TotCommunication += item.tempscom + item.tempsAtt;
                                TotProdReel += (item.tempscom + item.tempsAtt);
                                // tempsPresence += (item.tempsLog / 360000);
                            }
                        }
                    }
                    foreach (var item in attendances)
                    {
                        if (item.Id_Hermes == ag && item.date.Year == year)
                        {
                            if (item.date.Month == 1 || item.date.Month == 2 || item.date.Month == 3)
                            {
                                if (item.Depart == null)
                                {
                                    item.Depart = new DateTime(item.date.Year, item.date.Month, item.date.Day, 17, 0, 0);
                                }
                                dep = (item.Depart.Value).Hour;
                                arr = (item.Arrive.Value).Hour;
                                TempsLog += (dep - arr) - 1;
                            }
                        }
                    }

                    foreach (var plan in planings)
                    {
                        DateTime firstTrimestre = new DateTime(year, 1, 1);
                        DateTime endTrimestre = new DateTime(year, 3, 31);
                        if (plan.dateDebut.Date <= firstTrimestre && plan.dateFin >= endTrimestre)
                        {
                            tempsPlaning = ((plan.dateFin.Hour) - (plan.dateDebut.Hour) - 1);

                            foreach (var item in events)
                            {
                                if (item.titre == "Congé" && item.dateDebut.Date >= firstTrimestre && item.dateFin <= endTrimestre)
                                {
                                    tempsCongé += ((item.dateFin.Hour) - (item.dateDebut.Hour) - 1);
                                }
                                if (item.titre == "Autorisation" && item.dateDebut.Date >= firstTrimestre && item.dateFin <= endTrimestre)
                                {
                                    tempsAutorisation += (item.dateFin.Hour) - (item.dateDebut.Hour);
                                }
                                if (item.titre == "Jours Fériés" && item.dateDebut.Date >= firstTrimestre && item.dateFin <= endTrimestre)
                                {
                                    Joursfériés += 1;
                                }
                            }
                            while (firstTrimestre < endTrimestre)
                            {
                                if (firstTrimestre.DayOfWeek != DayOfWeek.Saturday && firstTrimestre.DayOfWeek != DayOfWeek.Sunday)
                                { JoursSansWeekend++; }
                                firstTrimestre = firstTrimestre.AddDays(1);

                            }
                            Joursouvrés = JoursSansWeekend - Joursfériés;
                            tempsPlanifie = (tempsPlaning * Joursouvrés) - tempsCongé - tempsAutorisation;
                        }
                        //Fin If pour planing include between two dates
                        if (plan.dateDebut.Date >= firstTrimestre && plan.dateFin <= endTrimestre)
                        {
                            tempsPlaning = ((plan.dateFin.Hour) - (plan.dateDebut.Hour) - 1);
                            firstTrimestre = plan.dateDebut;
                            endTrimestre = plan.dateFin;
                            foreach (var item in events)
                            {
                                if (item.titre == "Congé" && item.dateDebut.Date >= firstTrimestre && item.dateFin <= endTrimestre)
                                {
                                    tempsCongé += ((item.dateFin.Hour) - (item.dateDebut.Hour) - 1);
                                }
                                if (item.titre == "Autorisation" && item.dateDebut.Date >= firstTrimestre && item.dateFin <= endTrimestre)
                                {
                                    tempsAutorisation += (item.dateFin.Hour) - (item.dateDebut.Hour);
                                }
                                if (item.titre == "Jours Fériés" && item.dateDebut.Date >= firstTrimestre && item.dateFin <= endTrimestre)
                                {
                                    Joursfériés += 1;
                                }
                            }
                            while (firstTrimestre < endTrimestre)
                            {
                                if (firstTrimestre.DayOfWeek != DayOfWeek.Saturday && firstTrimestre.DayOfWeek != DayOfWeek.Sunday)
                                { JoursSansWeekend++; }
                                firstTrimestre = firstTrimestre.AddDays(1);

                            }
                            Joursouvrés = JoursSansWeekend - Joursfériés;
                            tempsPlanifie = (tempsPlaning * Joursouvrés) - tempsCongé - tempsAutorisation;
                        }
                        //Fin If pour planing include inverse between two dates
                        if (plan.dateDebut.Date >= firstTrimestre && plan.dateFin == endTrimestre)
                        {

                            tempsPlaning = ((plan.dateFin.Hour) - (plan.dateDebut.Hour) - 1);
                            firstTrimestre = plan.dateDebut.Date;
                            foreach (var item in events)
                            {
                                if (item.titre == "Congé" && item.dateDebut.Date >= firstTrimestre && item.dateFin <= endTrimestre)
                                {
                                    tempsCongé += ((item.dateFin.Hour) - (item.dateDebut.Hour) - 1);
                                }
                                if (item.titre == "Autorisation" && item.dateDebut.Date >= firstTrimestre && item.dateFin <= endTrimestre)
                                {
                                    tempsAutorisation += (item.dateFin.Hour) - (item.dateDebut.Hour);
                                }
                                if (item.titre == "Jours Fériés" && item.dateDebut.Date >= firstTrimestre && item.dateFin <= endTrimestre)
                                {
                                    Joursfériés += 1;
                                }
                            }
                            while (firstTrimestre < endTrimestre)
                            {
                                if (firstTrimestre.DayOfWeek != DayOfWeek.Saturday && firstTrimestre.DayOfWeek != DayOfWeek.Sunday)
                                { JoursSansWeekend++; }
                                firstTrimestre = firstTrimestre.AddDays(1);

                            }
                            Joursouvrés = JoursSansWeekend - Joursfériés;
                            tempsPlanifie = (tempsPlaning * Joursouvrés) - tempsCongé - tempsAutorisation;
                        }
                        //Fin If pour planing date debut > debut trimestre
                        if (plan.dateDebut.Date == firstTrimestre && plan.dateFin <= endTrimestre)
                        {
                            tempsPlaning = ((plan.dateFin.Hour) - (plan.dateDebut.Hour) - 1);
                            endTrimestre = plan.dateFin.Date;
                            foreach (var item in events)
                            {
                                if (item.titre == "Congé" && item.dateDebut.Date >= firstTrimestre && item.dateFin <= endTrimestre)
                                {
                                    tempsCongé += ((item.dateFin.Hour) - (item.dateDebut.Hour) - 1);
                                }
                                if (item.titre == "Autorisation" && item.dateDebut.Date >= firstTrimestre && item.dateFin <= endTrimestre)
                                {
                                    tempsAutorisation += (item.dateFin.Hour) - (item.dateDebut.Hour);
                                }
                                if (item.titre == "Jours Fériés" && item.dateDebut.Date >= firstTrimestre && item.dateFin <= endTrimestre)
                                {
                                    Joursfériés += 1;
                                }
                            }
                            while (firstTrimestre < endTrimestre)
                            {
                                if (firstTrimestre.DayOfWeek != DayOfWeek.Saturday && firstTrimestre.DayOfWeek != DayOfWeek.Sunday)
                                { JoursSansWeekend++; }
                                firstTrimestre = firstTrimestre.AddDays(1);
                            }
                            Joursouvrés = JoursSansWeekend - Joursfériés;
                            tempsPlanifie = (tempsPlaning * Joursouvrés) - tempsCongé - tempsAutorisation;
                        }
                        //Fin If pour planing date Fin < fin trimestre
                    }
                    break;
                case 2:
                    foreach (var item in appels)
                    {
                        if (item.Id_Hermes == ag && item.date.Year == year)
                        {
                            if (item.date.Month == 4 || item.date.Month == 5 || item.date.Month == 6)
                            {
                                //TotCA += item.CA;
                                //TotCNA += item.CNA;
                                //TotAccord += item.Accords;
                                TotAppelEmis += item.TotalAppelEmis;
                                TotAppelAboutis += item.TotalAppelAboutis;
                                if (!(dates.Exists(x => x == item.date)))
                                {
                                    dates.Add(item.date);
                                }
                                TotJourTravaillés = dates.LongCount();
                            }
                        }
                    }
                    foreach (var item in temps)
                    {
                        if (item.Id_Hermes == ag && item.date.Year == year)
                        {
                            if (item.date.Month == 4 || item.date.Month == 5 || item.date.Month == 6)
                            {
                                TotAcw += item.tempsACW;
                                TotPreview += item.tempsPreview;
                                TotPausePerso += item.tempsPausePerso;
                                TotOccupation += item.tempscom + item.tempsAtt + item.tempsPreview;
                                TotCommunication += item.tempscom + item.tempsAtt;
                                TotProdReel += (item.tempscom + item.tempsAtt);
                            }
                        }
                    }
                    foreach (var item in attendances)
                    {
                        if (item.Id_Hermes == ag && item.date.Year == year)
                        {
                            if (item.date.Month == 4 || item.date.Month == 5 || item.date.Month == 6)
                            {
                                if (item.Depart == null)
                                {
                                    item.Depart = new DateTime(item.date.Year, item.date.Month, item.date.Day, 17, 0, 0);
                                }
                                dep = (item.Depart.Value).Hour;
                                arr = (item.Arrive.Value).Hour;
                                TempsLog += (dep - arr) - 1;
                            }
                        }
                    }

                    foreach (var plan in planings)
                    {
                        DateTime firstTrimestre2 = new DateTime(year, 4, 1);
                        DateTime endTrimestre2 = new DateTime(year, 6, 30);
                        if (plan.dateDebut.Date <= firstTrimestre2 && plan.dateFin >= endTrimestre2)
                        {
                            tempsPlaning = ((plan.dateFin.Hour) - (plan.dateDebut.Hour) - 1);

                            foreach (var item in events)
                            {
                                if (item.titre == "Congé" && item.dateDebut.Date >= firstTrimestre2 && item.dateFin <= endTrimestre2)
                                {
                                    tempsCongé += ((item.dateFin.Hour) - (item.dateDebut.Hour) - 1);
                                }
                                if (item.titre == "Autorisation" && item.dateDebut.Date >= firstTrimestre2 && item.dateFin <= endTrimestre2)
                                {
                                    tempsAutorisation += (item.dateFin.Hour) - (item.dateDebut.Hour);
                                }
                                if (item.titre == "Jours Fériés" && item.dateDebut.Date >= firstTrimestre2 && item.dateFin <= endTrimestre2)
                                {
                                    Joursfériés += 1;
                                }
                            }
                            while (firstTrimestre2 < endTrimestre2)
                            {
                                if (firstTrimestre2.DayOfWeek != DayOfWeek.Saturday && firstTrimestre2.DayOfWeek != DayOfWeek.Sunday)
                                { JoursSansWeekend++; }
                                firstTrimestre2 = firstTrimestre2.AddDays(1);

                            }
                            Joursouvrés = JoursSansWeekend - Joursfériés;
                            tempsPlanifie = (tempsPlaning * Joursouvrés) - tempsCongé - tempsAutorisation;
                        }
                        //Fin If pour planing include between two dates
                        if (plan.dateDebut.Date >= firstTrimestre2 && plan.dateFin <= endTrimestre2)
                        {
                            tempsPlaning = ((plan.dateFin.Hour) - (plan.dateDebut.Hour) - 1);
                            firstTrimestre2 = plan.dateDebut;
                            endTrimestre2 = plan.dateFin;
                            foreach (var item in events)
                            {
                                if (item.titre == "Congé" && item.dateDebut.Date >= firstTrimestre2 && item.dateFin <= endTrimestre2)
                                {
                                    tempsCongé += ((item.dateFin.Hour) - (item.dateDebut.Hour) - 1);
                                }
                                if (item.titre == "Autorisation" && item.dateDebut.Date >= firstTrimestre2 && item.dateFin <= endTrimestre2)
                                {
                                    tempsAutorisation += (item.dateFin.Hour) - (item.dateDebut.Hour);
                                }
                                if (item.titre == "Jours Fériés" && item.dateDebut.Date >= firstTrimestre2 && item.dateFin <= endTrimestre2)
                                {
                                    Joursfériés += 1;
                                }
                            }
                            while (firstTrimestre2 < endTrimestre2)
                            {
                                if (firstTrimestre2.DayOfWeek != DayOfWeek.Saturday && firstTrimestre2.DayOfWeek != DayOfWeek.Sunday)
                                { JoursSansWeekend++; }
                                firstTrimestre2 = firstTrimestre2.AddDays(1);

                            }
                            Joursouvrés = JoursSansWeekend - Joursfériés;
                            tempsPlanifie = (tempsPlaning * Joursouvrés) - tempsCongé - tempsAutorisation;
                        }
                        //Fin If pour planing include inverse between two dates
                        if (plan.dateDebut.Date >= firstTrimestre2 && plan.dateFin >= endTrimestre2)
                        {

                            tempsPlaning = ((plan.dateFin.Hour) - (plan.dateDebut.Hour) - 1);
                            firstTrimestre2 = plan.dateDebut.Date;
                            foreach (var item in events)
                            {
                                if (item.titre == "Congé" && item.dateDebut.Date >= firstTrimestre2 && item.dateFin <= endTrimestre2)
                                {
                                    tempsCongé += ((item.dateFin.Hour) - (item.dateDebut.Hour) - 1);
                                }
                                if (item.titre == "Autorisation" && item.dateDebut.Date >= firstTrimestre2 && item.dateFin <= endTrimestre2)
                                {
                                    tempsAutorisation += (item.dateFin.Hour) - (item.dateDebut.Hour);
                                }
                                if (item.titre == "Jours Fériés" && item.dateDebut.Date >= firstTrimestre2 && item.dateFin <= endTrimestre2)
                                {
                                    Joursfériés += 1;
                                }
                            }
                            while (firstTrimestre2 < endTrimestre2)
                            {
                                if (firstTrimestre2.DayOfWeek != DayOfWeek.Saturday && firstTrimestre2.DayOfWeek != DayOfWeek.Sunday)
                                { JoursSansWeekend++; }
                                firstTrimestre2 = firstTrimestre2.AddDays(1);

                            }
                            Joursouvrés = JoursSansWeekend - Joursfériés;
                            tempsPlanifie = (tempsPlaning * Joursouvrés) - tempsCongé - tempsAutorisation;
                        }
                        //Fin If pour planing date debut et fin  >= debut et fin trimestre
                        if (plan.dateDebut.Date <= firstTrimestre2 && plan.dateFin <= endTrimestre2)
                        {
                            tempsPlaning = ((plan.dateFin.Hour) - (plan.dateDebut.Hour) - 1);
                            endTrimestre2 = plan.dateFin.Date;
                            foreach (var item in events)
                            {
                                if (item.titre == "Congé" && item.dateDebut.Date >= firstTrimestre2 && item.dateFin <= endTrimestre2)
                                {
                                    tempsCongé += ((item.dateFin.Hour) - (item.dateDebut.Hour) - 1);
                                }
                                if (item.titre == "Autorisation" && item.dateDebut.Date >= firstTrimestre2 && item.dateFin <= endTrimestre2)
                                {
                                    tempsAutorisation += (item.dateFin.Hour) - (item.dateDebut.Hour);
                                }
                                if (item.titre == "Jours Fériés" && item.dateDebut.Date >= firstTrimestre2 && item.dateFin <= endTrimestre2)
                                {
                                    Joursfériés += 1;
                                }
                            }
                            while (firstTrimestre2 < endTrimestre2)
                            {
                                if (firstTrimestre2.DayOfWeek != DayOfWeek.Saturday && firstTrimestre2.DayOfWeek != DayOfWeek.Sunday)
                                { JoursSansWeekend++; }
                                firstTrimestre2 = firstTrimestre2.AddDays(1);
                            }
                            Joursouvrés = JoursSansWeekend - Joursfériés;
                            tempsPlanifie = (tempsPlaning * Joursouvrés) - tempsCongé - tempsAutorisation;
                        }
                        //Fin If pour planing date debut et fin  <= debut et fin trimestre
                    }
                    break;
                case 3:
                    foreach (var item in appels)
                    {
                        if (item.Id_Hermes == ag && item.date.Year == year)
                        {
                            if (item.date.Month == 7 || item.date.Month == 8 || item.date.Month == 9)
                            {
                                //TotCA += item.CA;
                                //TotCNA += item.CNA;
                                //TotAccord += item.Accords;
                                TotAppelEmis += item.TotalAppelEmis;
                                TotAppelAboutis += item.TotalAppelAboutis;
                                if (!(dates.Exists(x => x == item.date)))
                                {
                                    dates.Add(item.date);
                                }
                                TotJourTravaillés = dates.LongCount();
                            }
                        }
                    }
                    foreach (var item in temps)
                    {
                        if (item.Id_Hermes == ag && item.date.Year == year)
                        {
                            if (item.date.Month == 7 || item.date.Month == 8 || item.date.Month == 9)
                            {
                                TotAcw += item.tempsACW;
                                TotPreview += item.tempsPreview;
                                TotPausePerso += item.tempsPausePerso;
                                TotOccupation += item.tempscom + item.tempsAtt + item.tempsPreview;
                                TotCommunication += item.tempscom + item.tempsAtt;
                                TotProdReel += (item.tempscom + item.tempsAtt);
                            }
                        }
                    }
                    foreach (var item in attendances)
                    {
                        if (item.Id_Hermes == ag && item.date.Year == year)
                        {
                            if (item.date.Month == 7 || item.date.Month == 8 || item.date.Month == 9)
                            {
                                if (item.Depart == null)
                                {
                                    item.Depart = new DateTime(item.date.Year, item.date.Month, item.date.Day, 17, 0, 0);
                                }
                                dep = (item.Depart.Value).Hour;
                                arr = (item.Arrive.Value).Hour;
                                TempsLog += (dep - arr) - 1;
                            }
                        }
                    }

                    foreach (var plan in planings)
                    {
                        DateTime firstTrimestre3 = new DateTime(year, 7, 31);
                        DateTime endTrimestre3 = new DateTime(year, 9, 30);
                        if (plan.dateDebut.Date <= firstTrimestre3 && plan.dateFin >= endTrimestre3)
                        {
                            tempsPlaning = ((plan.dateFin.Hour) - (plan.dateDebut.Hour) - 1);

                            foreach (var item in events)
                            {
                                if (item.titre == "Congé" && item.dateDebut.Date >= firstTrimestre3 && item.dateFin <= endTrimestre3)
                                {
                                    tempsCongé += ((item.dateFin.Hour) - (item.dateDebut.Hour) - 1);
                                }
                                if (item.titre == "Autorisation" && item.dateDebut.Date >= firstTrimestre3 && item.dateFin <= endTrimestre3)
                                {
                                    tempsAutorisation += (item.dateFin.Hour) - (item.dateDebut.Hour);
                                }
                                if (item.titre == "Jours Fériés" && item.dateDebut.Date >= firstTrimestre3 && item.dateFin <= endTrimestre3)
                                {
                                    Joursfériés += 1;
                                }
                            }
                            while (firstTrimestre3 < endTrimestre3)
                            {
                                if (firstTrimestre3.DayOfWeek != DayOfWeek.Saturday && firstTrimestre3.DayOfWeek != DayOfWeek.Sunday)
                                { JoursSansWeekend++; }
                                firstTrimestre3 = firstTrimestre3.AddDays(1);

                            }
                            Joursouvrés = JoursSansWeekend - Joursfériés;
                            tempsPlanifie = (tempsPlaning * Joursouvrés) - tempsCongé - tempsAutorisation;
                        }
                        //Fin If pour planing include between two dates
                        if (plan.dateDebut.Date >= firstTrimestre3 && plan.dateFin <= endTrimestre3)
                        {
                            tempsPlaning = ((plan.dateFin.Hour) - (plan.dateDebut.Hour) - 1);
                            firstTrimestre3 = plan.dateDebut;
                            endTrimestre3 = plan.dateFin;
                            foreach (var item in events)
                            {
                                if (item.titre == "Congé" && item.dateDebut.Date >= firstTrimestre3 && item.dateFin <= endTrimestre3)
                                {
                                    tempsCongé += ((item.dateFin.Hour) - (item.dateDebut.Hour) - 1);
                                }
                                if (item.titre == "Autorisation" && item.dateDebut.Date >= firstTrimestre3 && item.dateFin <= endTrimestre3)
                                {
                                    tempsAutorisation += (item.dateFin.Hour) - (item.dateDebut.Hour);
                                }
                                if (item.titre == "Jours Fériés" && item.dateDebut.Date >= firstTrimestre3 && item.dateFin <= endTrimestre3)
                                {
                                    Joursfériés += 1;
                                }
                            }
                            while (firstTrimestre3 < endTrimestre3)
                            {
                                if (firstTrimestre3.DayOfWeek != DayOfWeek.Saturday && firstTrimestre3.DayOfWeek != DayOfWeek.Sunday)
                                { JoursSansWeekend++; }
                                firstTrimestre3 = firstTrimestre3.AddDays(1);

                            }
                            Joursouvrés = JoursSansWeekend - Joursfériés;
                            tempsPlanifie = (tempsPlaning * Joursouvrés) - tempsCongé - tempsAutorisation;
                        }
                        //Fin If pour planing include inverse between two dates
                        if (plan.dateDebut.Date >= firstTrimestre3 && plan.dateFin >= endTrimestre3)
                        {

                            tempsPlaning = ((plan.dateFin.Hour) - (plan.dateDebut.Hour) - 1);
                            firstTrimestre3 = plan.dateDebut.Date;
                            foreach (var item in events)
                            {
                                if (item.titre == "Congé" && item.dateDebut.Date >= firstTrimestre3 && item.dateFin <= endTrimestre3)
                                {
                                    tempsCongé += ((item.dateFin.Hour) - (item.dateDebut.Hour) - 1);
                                }
                                if (item.titre == "Autorisation" && item.dateDebut.Date >= firstTrimestre3 && item.dateFin <= endTrimestre3)
                                {
                                    tempsAutorisation += (item.dateFin.Hour) - (item.dateDebut.Hour);
                                }
                                if (item.titre == "Jours Fériés" && item.dateDebut.Date >= firstTrimestre3 && item.dateFin <= endTrimestre3)
                                {
                                    Joursfériés += 1;
                                }
                            }
                            while (firstTrimestre3 < endTrimestre3)
                            {
                                if (firstTrimestre3.DayOfWeek != DayOfWeek.Saturday && firstTrimestre3.DayOfWeek != DayOfWeek.Sunday)
                                { JoursSansWeekend++; }
                                firstTrimestre3 = firstTrimestre3.AddDays(1);

                            }
                            Joursouvrés = JoursSansWeekend - Joursfériés;
                            tempsPlanifie = (tempsPlaning * Joursouvrés) - tempsCongé - tempsAutorisation;
                        }
                        //Fin If pour planing date debut et fin  >= debut et fin trimestre
                        if (plan.dateDebut.Date <= firstTrimestre3 && plan.dateFin <= endTrimestre3)
                        {
                            tempsPlaning = ((plan.dateFin.Hour) - (plan.dateDebut.Hour) - 1);
                            endTrimestre3 = plan.dateFin.Date;
                            foreach (var item in events)
                            {
                                if (item.titre == "Congé" && item.dateDebut.Date >= firstTrimestre3 && item.dateFin <= endTrimestre3)
                                {
                                    tempsCongé += ((item.dateFin.Hour) - (item.dateDebut.Hour) - 1);
                                }
                                if (item.titre == "Autorisation" && item.dateDebut.Date >= firstTrimestre3 && item.dateFin <= endTrimestre3)
                                {
                                    tempsAutorisation += (item.dateFin.Hour) - (item.dateDebut.Hour);
                                }
                                if (item.titre == "Jours Fériés" && item.dateDebut.Date >= firstTrimestre3 && item.dateFin <= endTrimestre3)
                                {
                                    Joursfériés += 1;
                                }
                            }
                            while (firstTrimestre3 < endTrimestre3)
                            {
                                if (firstTrimestre3.DayOfWeek != DayOfWeek.Saturday && firstTrimestre3.DayOfWeek != DayOfWeek.Sunday)
                                { JoursSansWeekend++; }
                                firstTrimestre3 = firstTrimestre3.AddDays(1);
                            }
                            Joursouvrés = JoursSansWeekend - Joursfériés;
                            tempsPlanifie = (tempsPlaning * Joursouvrés) - tempsCongé - tempsAutorisation;
                        }
                        //Fin If pour planing date debut et fin  <= debut et fin trimestre
                    }
                    break;
                case 4:
                    foreach (var item in appels)
                    {
                        if (item.Id_Hermes == ag && item.date.Year == year)
                        {
                            if (item.date.Month == 10 || item.date.Month == 11 || item.date.Month == 12)
                            {
                                //TotCA += item.CA;
                                //TotCNA += item.CNA;
                                //TotAccord += item.Accords;
                                TotAppelEmis += item.TotalAppelEmis;
                                TotAppelAboutis += item.TotalAppelAboutis;
                                if (!(dates.Exists(x => x == item.date)))
                                {
                                    dates.Add(item.date);
                                }
                                TotJourTravaillés = dates.LongCount();
                            }
                        }
                    }
                    foreach (var item in temps)
                    {
                        if (item.Id_Hermes == ag && item.date.Year == year)
                        {
                            if (item.date.Month == 10 || item.date.Month == 11 || item.date.Month == 12)
                            {
                                TotAcw += item.tempsACW;
                                TotPreview += item.tempsPreview;
                                TotPausePerso += item.tempsPausePerso;
                                TotOccupation += item.tempscom + item.tempsAtt + item.tempsPreview;
                                TotCommunication += item.tempscom + item.tempsAtt;
                                TotProdReel += (item.tempscom + item.tempsAtt);
                            }
                        }
                    }
                    foreach (var item in attendances)
                    {
                        if (item.Id_Hermes == ag && item.date.Year == year)
                        {
                            if (item.date.Month == 10 || item.date.Month == 11 || item.date.Month == 12)
                            {
                                if (item.Depart == null)
                                {
                                    item.Depart = new DateTime(item.date.Year, item.date.Month, item.date.Day, 17, 0, 0);
                                }
                                dep = (item.Depart.Value).Hour;
                                arr = (item.Arrive.Value).Hour;
                                TempsLog += (dep - arr) - 1;
                            }
                        }
                    }

                    foreach (var plan in planings)
                    {
                        DateTime firstTrimestre4 = new DateTime(year, 10, 31);
                        DateTime endTrimestre4 = new DateTime(year, 12, 31);
                        if (plan.dateDebut.Date <= firstTrimestre4 && plan.dateFin >= endTrimestre4)
                        {
                            tempsPlaning = ((plan.dateFin.Hour) - (plan.dateDebut.Hour) - 1);

                            foreach (var item in events)
                            {
                                if (item.titre == "Congé" && item.dateDebut.Date >= firstTrimestre4 && item.dateFin <= endTrimestre4)
                                {
                                    tempsCongé += ((item.dateFin.Hour) - (item.dateDebut.Hour) - 1);
                                }
                                if (item.titre == "Autorisation" && item.dateDebut.Date >= firstTrimestre4 && item.dateFin <= endTrimestre4)
                                {
                                    tempsAutorisation += (item.dateFin.Hour) - (item.dateDebut.Hour);
                                }
                                if (item.titre == "Jours Fériés" && item.dateDebut.Date >= firstTrimestre4 && item.dateFin <= endTrimestre4)
                                {
                                    Joursfériés += 1;
                                }
                            }
                            while (firstTrimestre4 < endTrimestre4)
                            {
                                if (firstTrimestre4.DayOfWeek != DayOfWeek.Saturday && firstTrimestre4.DayOfWeek != DayOfWeek.Sunday)
                                { JoursSansWeekend++; }
                                firstTrimestre4 = firstTrimestre4.AddDays(1);

                            }
                            Joursouvrés = JoursSansWeekend - Joursfériés;
                            tempsPlanifie = (tempsPlaning * Joursouvrés) - tempsCongé - tempsAutorisation;
                        }
                        //Fin If pour planing include between two dates
                        if (plan.dateDebut.Date >= firstTrimestre4 && plan.dateFin <= endTrimestre4)
                        {
                            tempsPlaning = ((plan.dateFin.Hour) - (plan.dateDebut.Hour) - 1);
                            firstTrimestre4 = plan.dateDebut;
                            endTrimestre4 = plan.dateFin;
                            foreach (var item in events)
                            {
                                if (item.titre == "Congé" && item.dateDebut.Date >= firstTrimestre4 && item.dateFin <= endTrimestre4)
                                {
                                    tempsCongé += ((item.dateFin.Hour) - (item.dateDebut.Hour) - 1);
                                }
                                if (item.titre == "Autorisation" && item.dateDebut.Date >= firstTrimestre4 && item.dateFin <= endTrimestre4)
                                {
                                    tempsAutorisation += (item.dateFin.Hour) - (item.dateDebut.Hour);
                                }
                                if (item.titre == "Jours Fériés" && item.dateDebut.Date >= firstTrimestre4 && item.dateFin <= endTrimestre4)
                                {
                                    Joursfériés += 1;
                                }
                            }
                            while (firstTrimestre4 < endTrimestre4)
                            {
                                if (firstTrimestre4.DayOfWeek != DayOfWeek.Saturday && firstTrimestre4.DayOfWeek != DayOfWeek.Sunday)
                                { JoursSansWeekend++; }
                                firstTrimestre4 = firstTrimestre4.AddDays(1);

                            }
                            Joursouvrés = JoursSansWeekend - Joursfériés;
                            tempsPlanifie = (tempsPlaning * Joursouvrés) - tempsCongé - tempsAutorisation;
                        }
                        //Fin If pour planing include inverse between two dates
                        if (plan.dateDebut.Date >= firstTrimestre4 && plan.dateFin >= endTrimestre4)
                        {

                            tempsPlaning = ((plan.dateFin.Hour) - (plan.dateDebut.Hour) - 1);
                            firstTrimestre4 = plan.dateDebut.Date;
                            foreach (var item in events)
                            {
                                if (item.titre == "Congé" && item.dateDebut.Date >= firstTrimestre4 && item.dateFin <= endTrimestre4)
                                {
                                    tempsCongé += ((item.dateFin.Hour) - (item.dateDebut.Hour) - 1);
                                }
                                if (item.titre == "Autorisation" && item.dateDebut.Date >= firstTrimestre4 && item.dateFin <= endTrimestre4)
                                {
                                    tempsAutorisation += (item.dateFin.Hour) - (item.dateDebut.Hour);
                                }
                                if (item.titre == "Jours Fériés" && item.dateDebut.Date >= firstTrimestre4 && item.dateFin <= endTrimestre4)
                                {
                                    Joursfériés += 1;
                                }
                            }
                            while (firstTrimestre4 < endTrimestre4)
                            {
                                if (firstTrimestre4.DayOfWeek != DayOfWeek.Saturday && firstTrimestre4.DayOfWeek != DayOfWeek.Sunday)
                                { JoursSansWeekend++; }
                                firstTrimestre4 = firstTrimestre4.AddDays(1);

                            }
                            Joursouvrés = JoursSansWeekend - Joursfériés;
                            tempsPlanifie = (tempsPlaning * Joursouvrés) - tempsCongé - tempsAutorisation;
                        }
                        //Fin If pour planing date debut et fin  >= debut et fin trimestre
                        if (plan.dateDebut.Date <= firstTrimestre4 && plan.dateFin <= endTrimestre4)
                        {
                            tempsPlaning = ((plan.dateFin.Hour) - (plan.dateDebut.Hour) - 1);
                            endTrimestre4 = plan.dateFin.Date;
                            foreach (var item in events)
                            {
                                if (item.titre == "Congé" && item.dateDebut.Date >= firstTrimestre4 && item.dateFin <= endTrimestre4)
                                {
                                    tempsCongé += ((item.dateFin.Hour) - (item.dateDebut.Hour) - 1);
                                }
                                if (item.titre == "Autorisation" && item.dateDebut.Date >= firstTrimestre4 && item.dateFin <= endTrimestre4)
                                {
                                    tempsAutorisation += (item.dateFin.Hour) - (item.dateDebut.Hour);
                                }
                                if (item.titre == "Jours Fériés" && item.dateDebut.Date >= firstTrimestre4 && item.dateFin <= endTrimestre4)
                                {
                                    Joursfériés += 1;
                                }
                            }
                            while (firstTrimestre4 < endTrimestre4)
                            {
                                if (firstTrimestre4.DayOfWeek != DayOfWeek.Saturday && firstTrimestre4.DayOfWeek != DayOfWeek.Sunday)
                                 { JoursSansWeekend++; }
                                firstTrimestre4 = firstTrimestre4.AddDays(1);
                            }
                            Joursouvrés = JoursSansWeekend - Joursfériés;
                            tempsPlanifie = (tempsPlaning * Joursouvrés) - tempsCongé - tempsAutorisation;
                        }
                        //Fin If pour planing date debut et fin  <= debut et fin trimestre
                    }

                    break;
            }
            if (TempsLog > tempsPlanifie && tempsPlanifie != 0)
            {
                TempsLog = tempsPlanifie;
            }
            if (tempsPlanifie == 0)
            {
                TauxAbs = 0;
            }
            else
            {
                TauxAbs = Math.Round(1 - (TempsLog / tempsPlanifie), 2);
            }
            //traitement
            calculs.Add(new Calcul { value = TotAppelEmis, name = "Appels Emis" });
            calculs.Add(new Calcul { value = TotAppelAboutis, name = "Appels Aboutis" });
            calculs.Add(new Calcul { value = TotCA, name = "Contact Argumenté" });
            calculs.Add(new Calcul { value = TotAccord, name = "Contact Argumenté Positif" });
            calculs.Add(new Calcul { value = TotCNA, name = "Contact Argumenté Négatif" });

            if (TotJourTravaillés == 0)
            {
                double MoyenneAccord = 0;
                double MoyenneAppels = 0;

                calculs.Add(new Calcul { value = MoyenneAccord, name = "Moyenne CA+" });
                calculs.Add(new Calcul { value = MoyenneAppels, name = "Moyenne des Appels" });

            }
            else
            {
                double ETPplanifie = Math.Round((TempsLog / TotJourTravaillés / 8), 2);
                if (ETPplanifie != 0)
                {
                    double MoyenneAccord = Math.Round((TotAccord / TotJourTravaillés / ETPplanifie), 2);

                    double MoyenneAppels = Math.Round((TotAppelEmis / TotJourTravaillés / ETPplanifie), 2);

                    calculs.Add(new Calcul { value = MoyenneAccord, name = "Moyenne CA+" });
                    calculs.Add(new Calcul { value = MoyenneAppels, name = "Moyenne des Appels" });
                }
                else
                {
                    double MoyenneAccord = 0;
                    double MoyenneAppels = 0;

                    calculs.Add(new Calcul { value = MoyenneAccord, name = "Moyenne CA+" });
                    calculs.Add(new Calcul { value = MoyenneAppels, name = "Moyenne des Appels" });
                }

            }

            if (TotCA != 0)
            {
                double TauxVentesHebdo = Math.Round(((TotAccord / TotCA) * 100), 2);
                calculs.Add(new Calcul { value = TauxVentesHebdo, name = "Taux De Concrétisation" });
            }
            else
            {
                double TauxVentesHebdo = 0;
                calculs.Add(new Calcul { value = TauxVentesHebdo, name = "Taux De Concrétisation" });
            }
            if (TempsLog != 0)
            {
                double TauxVenteParHeure = Math.Round((TotAccord / TempsLog), 2);
                calculs.Add(new Calcul { value = TauxVenteParHeure, name = "Taux de Ventes/Heure" });
            }
            else
            {
                double TauxVenteParHeure = 0;
                calculs.Add(new Calcul { value = TauxVenteParHeure, name = "Taux de Ventes/Heure" });
            }

            TotProdReel = Math.Round((TotProdReel / 360000), 2);
            //temps présence
            calculs.Add(new Calcul { value = TotJourTravaillés, name = "Nombre des jours travailés" });
            calculs.Add(new Calcul { value = TempsLog, name = "Temps de Présence/Heure" });
            calculs.Add(new Calcul { value = TotProdReel, name = "Temps de Prod réel/Heure" });
            if (TotJourTravaillés != 0)
            {
                double ETPplanifie = Math.Round((TempsLog / TotJourTravaillés / 8), 2);
                calculs.Add(new Calcul { value = ETPplanifie, name = "ETP planifié" });
            }
            else
            {
                double ETPplanifie = 0;
                calculs.Add(new Calcul { value = ETPplanifie, name = "ETP planifié" });
            }

            //téléphonie
            if (TempsLog != 0)
            {
                double TauxACWHebdo = Math.Round(((TotAcw / (TempsLog * 360000)) * 100), 2);

                double TauxPreviewHebdo = Math.Round(((TotPreview / (TempsLog * 360000)) * 100), 2);

                double TauxPauseBriefHebdo = 0;

                double TauxPausePersoHebdo = Math.Round(((TotPausePerso / (TempsLog * 360000)) * 100), 2);

                double TauxOccupation = Math.Round(((TotOccupation / (TempsLog * 360000)) * 100), 2);

                double TauxComunication = Math.Round(((TotCommunication / (TempsLog * 360000)) * 100), 2);

                calculs.Add(new Calcul { value = TauxComunication, name = "Taux de Communication" });
                calculs.Add(new Calcul { value = TauxOccupation, name = "Taux d'occupation" });
                calculs.Add(new Calcul { value = TauxACWHebdo, name = "Taux ACW" });
                calculs.Add(new Calcul { value = TauxPreviewHebdo, name = "Taux Preview" });
                calculs.Add(new Calcul { value = TauxPauseBriefHebdo, name = "Taux Pause Brief" });
                calculs.Add(new Calcul { value = TauxPausePersoHebdo, name = "Taux Pause Perso" });
            }
            else
            {
                double TauxACWHebdo = 0;
                double TauxPreviewHebdo = 0;
                double TauxPauseBriefHebdo = 0;
                double TauxPausePersoHebdo = 0;
                double TauxOccupation = 0;
                double TauxComunication = 0;

                calculs.Add(new Calcul { value = TauxComunication, name = "Taux de Communication" });
                calculs.Add(new Calcul { value = TauxOccupation, name = "Taux d'occupation" });
                calculs.Add(new Calcul { value = TauxACWHebdo, name = "Taux ACW(Post-Appel)" });
                calculs.Add(new Calcul { value = TauxPreviewHebdo, name = "Taux Preview" });
                calculs.Add(new Calcul { value = TauxPauseBriefHebdo, name = "Taux Pause Brief" });
                calculs.Add(new Calcul { value = TauxPausePersoHebdo, name = "Taux Pause Perso" });
            }
            calculs.Add(new Calcul { value = TauxAbs, name = "Taux d'absentéisme" });
            TotAcw = Math.Round((TotAcw / 360000), 2);
            calculs.Add(new Calcul { value = TotAcw, name = "Post Appel" });
            return Json(calculs, JsonRequestBehavior.AllowGet);
        }

        //Controllers in Agent Template
        [HttpGet]
        public ActionResult JournalierAgent(int? id)
        {
            //string value = (string)Session["loginIndex"];
            var empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                idEmpConnecte = (int)id;
            }
            if (empConnected.Content != null)
            {
                String strbase64 = Convert.ToBase64String(empConnected.Content);
                String empConnectedImage = "data:" + empConnected.ContentType + ";base64," + strbase64;
                ViewBag.empConnectedImage = empConnectedImage;
            }
            ViewBag.nameEmpConnected = empConnected.UserName;
            ViewBag.pseudoNameEmpConnected = empConnected.pseudoName;
            return View(empConnected);
        }

        public ActionResult HebdoAgent(int? id)
        {
            //string value = (string)Session["loginIndex"];
            //var empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (emp == null)
            {
                return HttpNotFound();
            }
            if (emp.Content != null)
            {
                String strbase64 = Convert.ToBase64String(emp.Content);
                String empConnectedImage = "data:" + emp.ContentType + ";base64," + strbase64;
                ViewBag.empConnectedImage = empConnectedImage;
            }
            ViewBag.nameEmpConnected = emp.UserName;
            ViewBag.pseudoNameEmpConnected = emp.pseudoName;
            return View(emp);
        }

        public ActionResult MensuelAgent(int? id)
        {
            //string value = (string)Session["loginIndex"];
            //Employee empConnected = service.getById(idEmpConnecte);
            Employee empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

            //service = new EmployeeService();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Employee emp = service.getById(id);
            if (empConnected == null)
            {
                return HttpNotFound();
            }
            if (empConnected.Content != null)
            {
                String strbase64 = Convert.ToBase64String(empConnected.Content);
                String empConnectedImage = "data:" + empConnected.ContentType + ";base64," + strbase64;
                ViewBag.empConnectedImage = empConnectedImage;
            }
            ViewBag.nameEmpConnected = empConnected.UserName;
            ViewBag.pseudoNameEmpConnected = empConnected.pseudoName;
            return View(empConnected);
        }
        public ActionResult TrimestrielAgent(int? id)
        {
            //string value = (string)Session["loginIndex"];
            //Employee empConnected = service.getById(idEmpConnecte);
            Employee empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

            //service = new EmployeeService();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Employee emp = service.getById(id);
            if (empConnected == null)
            {
                return HttpNotFound();
            }
            if (empConnected.Content != null)
            {
                String strbase64 = Convert.ToBase64String(empConnected.Content);
                String empConnectedImage = "data:" + empConnected.ContentType + ";base64," + strbase64;
                ViewBag.empConnectedImage = empConnectedImage;
            }
            ViewBag.nameEmpConnected = empConnected.UserName;
            ViewBag.pseudoNameEmpConnected = empConnected.pseudoName;
            return View(empConnected);
        }
        public ActionResult AnnuelleAgent(int? id)
        {
            //string value = (string)Session["loginIndex"];
            //Employee empConnected = service.getById(idEmpConnecte);
            //service = new EmployeeService();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Employee emp = service.getById(id);
            Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

            if (emp == null)
            {
                return HttpNotFound();
            }
            if (emp.Content != null)
            {
                String strbase64 = Convert.ToBase64String(emp.Content);
                String empConnectedImage = "data:" + emp.ContentType + ";base64," + strbase64;
                ViewBag.empConnectedImage = empConnectedImage;
            }
            ViewBag.nameEmpConnected = emp.UserName;
            ViewBag.pseudoNameEmpConnected = emp.pseudoName;
            return View(emp);
        }

        public ActionResult TestStat(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View();
        }
        //Fin Controllers in Agent Template






      
    }
}
