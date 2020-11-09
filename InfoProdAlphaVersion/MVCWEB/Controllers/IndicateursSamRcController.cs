using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Data;
using MVCWEB.Models;
using Domain.Entity;
using Services;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;


namespace MVCWEB.Controllers
{
    public class IndicateursSamRcController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        private ReportContext db = new ReportContext();
        IGroupeEmployeeService serviceGroupeEmp;
        IIndicateursSAMRCService service;
        public IndicateursSamRcController()
        {
            serviceGroupeEmp = new GroupesEmployeService();
            service = new IndicateursSAMRCService();
            //SelectList Semaine
            var semaines = new List<SelectListItem>();
            for (int m = 1; m <= 53; m++)
            {
                var val = m.ToString();

                semaines.Add(new SelectListItem { Text = "Semaine" + val, Value = val });
            }
            ViewBag.SemaineItems = semaines;
        
        }


        public IndicateursSamRcController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
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

    //Views
        public ActionResult SAMRC_Journalier()
        {

            var empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (empConnected == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            if (empConnected.Roles.Any(b => b.UserId == empConnected.Id && b.RoleId == 8))
            {
                ViewBag.role = "Agent Qualité_CustomerService";
            }
            if (empConnected.Roles.Any(b => b.UserId == empConnected.Id && b.RoleId == 4))
            {
                ViewBag.role = "Qualité";
            }
            var a = new EmployeeViewModel();
            if (empConnected.Content != null)
            {
                String strbase64 = Convert.ToBase64String(empConnected.Content);
                String empConnectedImage = "data:" + empConnected.ContentType + ";base64," + strbase64;
                a.Url = empConnectedImage;
            }
            a.userName = empConnected.UserName;
            a.pseudoName = empConnected.pseudoName;

            List<Employee> logins = new List<Employee>();
            List<SelectListItem> AgentsList = new List<SelectListItem>();
            List<Employee> emp = serviceGroupeEmp.getListEmployeeByGroupeId(1016);
            foreach (var e in emp)
            {
                if (!logins.Exists(l => l.UserName == e.UserName))
                {
                    logins.Add(e);
                }
            }
            var employees = logins.OrderBy(b => b.UserName).ToList();
            AgentsList.Add(new SelectListItem { Text = "Sélectionner un Agent", Value = "" });
            AgentsList.Add(new SelectListItem { Text = "Hermes", Value = "0" });
            foreach (var test in employees)
            {

                AgentsList.Add(new SelectListItem { Text = test.UserName, Value = test.IdHermes.ToString() });


            }
            ViewBag.AgentItems = AgentsList;
            return View(a);
        }

        public ActionResult SAMRC_Hebdomadaire()
        {

            var empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (empConnected == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            if (empConnected.Roles.Any(b => b.UserId == empConnected.Id && b.RoleId == 8))
            {
                ViewBag.role = "Agent Qualité_CustomerService";
            }
            if (empConnected.Roles.Any(b => b.UserId == empConnected.Id && b.RoleId == 4))
            {
                ViewBag.role = "Qualité";
            }
            var a = new EmployeeViewModel();
            if (empConnected.Content != null)
            {
                String strbase64 = Convert.ToBase64String(empConnected.Content);
                String empConnectedImage = "data:" + empConnected.ContentType + ";base64," + strbase64;
                a.Url = empConnectedImage;
            }
            a.userName = empConnected.UserName;
            a.pseudoName = empConnected.pseudoName;

            List<Employee> logins = new List<Employee>();
            List<SelectListItem> AgentsList = new List<SelectListItem>();
            List<Employee> emp = serviceGroupeEmp.getListEmployeeByGroupeId(1016);
            foreach (var e in emp)
            {
                if (!logins.Exists(l => l.UserName == e.UserName))
                {
                    logins.Add(e);
                }
            }
            var employees = logins.OrderBy(b => b.UserName).ToList();
            AgentsList.Add(new SelectListItem { Text = "Sélectionner un Agent", Value = "" });
            AgentsList.Add(new SelectListItem { Text = "Hermes", Value = "0" });
            foreach (var test in employees)
            {

                AgentsList.Add(new SelectListItem { Text = test.UserName, Value = test.IdHermes.ToString() });


            }
            ViewBag.AgentItems = AgentsList;
            return View(a);
        }

        public ActionResult SAMRC_Mensuel()
        {

            var empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (empConnected == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            if (empConnected.Roles.Any(b => b.UserId == empConnected.Id && b.RoleId == 8))
            {
                ViewBag.role = "Agent Qualité_CustomerService";
            }
            if (empConnected.Roles.Any(b => b.UserId == empConnected.Id && b.RoleId == 4))
            {
                ViewBag.role = "Qualité";
            }
            var a = new EmployeeViewModel();
            if (empConnected.Content != null)
            {
                String strbase64 = Convert.ToBase64String(empConnected.Content);
                String empConnectedImage = "data:" + empConnected.ContentType + ";base64," + strbase64;
                a.Url = empConnectedImage;
            }
            a.userName = empConnected.UserName;
            a.pseudoName = empConnected.pseudoName;

            List<Employee> logins = new List<Employee>();
            List<SelectListItem> AgentsList = new List<SelectListItem>();
            List<Employee> emp = serviceGroupeEmp.getListEmployeeByGroupeId(1016);
            foreach (var e in emp)
            {
                if (!logins.Exists(l => l.UserName == e.UserName))
                {
                    logins.Add(e);
                }
            }
            var employees = logins.OrderBy(b => b.UserName).ToList();
            AgentsList.Add(new SelectListItem { Text = "Sélectionner un Agent", Value = "" });
            AgentsList.Add(new SelectListItem { Text = "Hermes", Value = "0" });
            foreach (var test in employees)
            {

                AgentsList.Add(new SelectListItem { Text = test.UserName, Value = test.IdHermes.ToString() });


            }
            ViewBag.AgentItems = AgentsList;
            return View(a);
        }

        public ActionResult SAMRC_TrancheHoraire()
        {

            var empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (empConnected == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            if (empConnected.Roles.Any(b => b.UserId == empConnected.Id && b.RoleId == 8))
            {
                ViewBag.role = "Agent Qualité_CustomerService";
            }
            if (empConnected.Roles.Any(b => b.UserId == empConnected.Id && b.RoleId == 4))
            {
                ViewBag.role = "Qualité";
            }
            var a = new EmployeeViewModel();
            if (empConnected.Content != null)
            {
                String strbase64 = Convert.ToBase64String(empConnected.Content);
                String empConnectedImage = "data:" + empConnected.ContentType + ";base64," + strbase64;
                a.Url = empConnectedImage;
            }
            a.userName = empConnected.UserName;
            a.pseudoName = empConnected.pseudoName;
            return View(a);
        }

        public ActionResult SAMRC_TrancheDemiHeure()
        {

            var empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (empConnected == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            if (empConnected.Roles.Any(b => b.UserId == empConnected.Id && b.RoleId == 8))
            {
                ViewBag.role = "Agent Qualité_CustomerService";
            }
            if (empConnected.Roles.Any(b => b.UserId == empConnected.Id && b.RoleId == 4))
            {
                ViewBag.role = "Qualité";
            }
            var a = new EmployeeViewModel();
            if (empConnected.Content != null)
            {
                String strbase64 = Convert.ToBase64String(empConnected.Content);
                String empConnectedImage = "data:" + empConnected.ContentType + ";base64," + strbase64;
                a.Url = empConnectedImage;
            }
            a.userName = empConnected.UserName;
            a.pseudoName = empConnected.pseudoName;
            return View(a);
        }

        //Calculs
        public JsonResult GetJournalierValues(string agentSel, string daySel)
        {
            List<SamRcIndicator> calculs = new List<SamRcIndicator>();
            int year = int.Parse(daySel.Substring(0, 4));
            int month = int.Parse(daySel.Substring(5, 2));
            int day = int.Parse(daySel.Substring(8, 2));
            //Data
            var appelsEntrants = db.Appels_Entrants_SamRc.Where(app => app.Date.Year == year && app.Date.Month == month && app.Date.Day == day && app.FERIE == 0 && app.Closed == 0 && app.LastQueue != 0).ToList();
            var appelSortants = db.Appels_Sortants_SamRc.Where(s => s.date.Year == year && s.date.Month == month && s.date.Day == day && s.Closed == 0 ).ToList();
            var attendances = db.AttendanceHermes_SamRc.Where(att => att.Date.Year == year && att.Date.Month == month && att.Date.Day == day).ToList();
            var temps = db.Temps_SamRc.Where(t => t.date.Year == year && t.date.Month == month && t.date.Day == day).ToList();
            if (agentSel != "")
            {
                int IdHermes = int.Parse(agentSel);
                appelsEntrants = db.Appels_Entrants_SamRc.Where(app => app.Date.Year == year && app.Date.Month == month && app.Date.Day == day && app.LastAgent == IdHermes && app.FERIE == 0).ToList();
                appelSortants = db.Appels_Sortants_SamRc.Where(s => s.date.Year == year && s.date.Month == month && s.date.Day == day && s.Closed == 0 && s.LastAgent == IdHermes).ToList();
                attendances = db.AttendanceHermes_SamRc.Where(att => att.Date.Year == year && att.Date.Month == month && att.Date.Day == day && att.Id_Hermes == IdHermes).ToList();
                temps = db.Temps_SamRc.Where(t => t.date.Year == year && t.date.Month == month && t.date.Day == day && t.AgentId == IdHermes).ToList();
            }
            //variables appels Entrants 
            int TotAppelsPris = 0, TotAppelsDecroches = 0, TotAppelsDecrochesinfdix = 0, TotAppelsAbandonnes = 0, TotAppelsDebordes = 0, 
       TotAppelsNoAgent = 0, TotAppelsPerdus = 0, TotAppelsAttenteAvantAgent = 0, TotAppelsAttenteAvantAbandon = 0, TotConvDuration = 0;
            Double TotAppelsRecus = 0, ObjectifAppelsPris = 0, PerceintAtteinteObjectif = 0, PercentQS = 0, TauxCommEntrant = 0, totDMCEntrant = 0;
            //variables ppels Sortants 
            int TotAppelsEmis = 0, TotAppelsDecrochesSortant = 0, TotConvDurationSortant = 0;
            double TauxCommSortant = 0,  totDMCSortant = 0;
            //variables attendances
            int totJoursTravailles = 0; double tempsLog = 0; double ETP = 0;
            //variables Téléphoniques (temps)
            double totTempsComm = 0, totTepmsAttente = 0, totTempsACW = 0, totTempsPauseCafe = 0, totTempsPauseBrief = 0, totTempsPauseDej = 0, totTempsMissionOp = 0, totTempsFormation = 0, totTempsQualification = 0, totTempsPhoning = 0, totTempsAdmin = 0, totAppelIP = 0;
            double totTempsTravail = 0, totProdNet = 0;
            double TauxComm = 0, TauxAttente = 0, TauxACW = 0, TauxPauseCafe = 0, TauxPauseBrief = 0, TauxPauseDej = 0, TauxMissionOP = 0, TauxFormation = 0,
                TauxQualification = 0, TauxPhoning = 0, TauxTempsAdmin = 0, TauxAppelIP = 0, TauxProdBrut = 0, TauxProdNet = 0;

            //Table Appels Entrants
            foreach (var item in appelsEntrants)
            {
                TotConvDuration += item.ConvDuration;
                TotAppelsRecus += 1;
                if (item.LastAgent != 0)
                {
                    TotAppelsPris += 1;
                }
                if (item.ConvDuration != 0)
                {
                    TotAppelsDecroches += 1;
                }
                if (item.ConvDuration < 10)
                {
                    TotAppelsDecrochesinfdix += 1;
                }
                if (item.Abandon == 1 && item.ConvDuration == 0)
                {
                    TotAppelsAbandonnes += 1;
                }
                if (item.Overflow == 1)
                {
                    TotAppelsDebordes += 1;
                }
                if (item.NoAgent == 1)
                {
                    TotAppelsNoAgent += 1;
                }
                if (item.WaitDuration != 0)
                {
                    TotAppelsAttenteAvantAgent += 1;
                }
                if (item.WaitDuration != 0 && item.LastAgent != 0)
                {
                    TotAppelsAttenteAvantAbandon += 1;
                }
            }
            TotAppelsPerdus = TotAppelsAbandonnes + TotAppelsDebordes + TotAppelsNoAgent;
            ObjectifAppelsPris = Math.Round((TotAppelsRecus * 0.95), 0);
            var TempsCommEntrant = TimeSpan.FromSeconds(TotConvDuration);
            if(TotAppelsDecroches != 0)
            {
                 totDMCEntrant = TotConvDuration / TotAppelsDecroches;
            }
            
            var DMCEntrant = TimeSpan.FromSeconds(totDMCEntrant);
            if (ObjectifAppelsPris != 0)
            {
                PerceintAtteinteObjectif = Math.Round((TotAppelsPris / ObjectifAppelsPris) * 100, 2);
            }
            if (TotAppelsRecus != 0)
            {
                PercentQS = Math.Round((TotAppelsDecroches / TotAppelsRecus) * 100, 2);
            }

            //Table Appels Sortants
            foreach (var item in appelSortants)
            {
                TotConvDurationSortant += item.ConvDuration;
                TotAppelsEmis += 1;
                if(item.ConvDuration != 0)
                {
                    TotAppelsDecrochesSortant += 1;
                }
            }
            var TempsCommSortant = TimeSpan.FromSeconds(TotConvDurationSortant);
            if(TotAppelsDecrochesSortant != 0)
            {
                totDMCSortant = TotConvDurationSortant / TotAppelsDecrochesSortant;
            }
            var DMCSortant = TimeSpan.FromSeconds(totDMCSortant);

            //Table Attendances
            if (attendances != null)
            {
                totJoursTravailles = 1;
            }
            foreach (var att in attendances)
            {
                TimeSpan dep = (att.Depart.TimeOfDay);
                TimeSpan arr = (att.Arrive.TimeOfDay);
                tempsLog += (dep - arr).TotalHours ;
            }
           
            //Table Temps
            foreach (var item in temps)
            {
                totTempsComm += item.tps_com;
                totTepmsAttente += item.tps_att;
                totTempsACW += item.tps_acw;
                totTempsPauseCafe += item.Temps_P_Cafe;
                totTempsPauseBrief += item.Temps_P_Brief;
                totTempsPauseDej += item.Temps_P_Dej;
                totTempsMissionOp += item.Temps_P_Mission_OP;
                totTempsFormation += item.Temps_P_Formation;
                totTempsQualification += item.Temps_P_Qualif;
                totTempsPhoning += item.Temps_P_phonning;
                totTempsAdmin += item.Temps_P_admin;
                totAppelIP += item.Temps_Appel_Ip;
            }
            totTempsTravail = totTempsComm + totTepmsAttente + totTempsACW + totTempsMissionOp;
            totProdNet = totTempsTravail - (totTempsMissionOp + totTempsACW);
            //convertion en temps hh:mm:ss
            var TempsLog = TimeSpan.FromHours(tempsLog);
            var TempsProdBrut = TimeSpan.FromMilliseconds(totTempsTravail*10);
            var TempsProdNet = TimeSpan.FromMilliseconds(totProdNet*10);
            var TempsCommunication = TimeSpan.FromMilliseconds(totTempsComm*10);
            var TempsAttente = TimeSpan.FromMilliseconds(totTepmsAttente*10);
            var TempsACW = TimeSpan.FromMilliseconds(totTempsACW*10);
            var TempsPauseCafe = TimeSpan.FromMilliseconds(totTempsPauseCafe*10);
            var TempsPauseBrief = TimeSpan.FromMilliseconds(totTempsPauseBrief*10);
            var TempsPauseDej = TimeSpan.FromMilliseconds(totTempsPauseDej*10);
            var TempsMissionOp = TimeSpan.FromMilliseconds(totTempsMissionOp*10);
            var TempsFormation = TimeSpan.FromMilliseconds(totTempsFormation*10);
            var TempsQualification = TimeSpan.FromMilliseconds(totTempsQualification*10);
            var TempsPhoning = TimeSpan.FromMilliseconds(totTempsPhoning*10);
            var TempsAdmin = TimeSpan.FromMilliseconds(totTempsAdmin*10);
            var TempsAppelIP = TimeSpan.FromMilliseconds(totAppelIP*10);

            if (tempsLog != 0)
            {
            ETP = Math.Round((tempsLog / totJoursTravailles / 8), 2);
            TauxCommEntrant = Math.Round(((TotConvDuration/3600) / tempsLog) * 100, 2);
            TauxCommSortant = Math.Round(((TotConvDurationSortant/3600) / tempsLog) * 100, 2);

                TauxComm = Math.Round((totTempsComm / 360000 / tempsLog) * 100, 2);
                TauxAttente = Math.Round((totTepmsAttente / 360000 / tempsLog) * 100, 2);
                TauxACW = Math.Round((totTempsACW / 360000 / tempsLog) * 100, 2);
                TauxPauseCafe = Math.Round((totTempsPauseCafe / 360000 / tempsLog) * 100, 2);
                TauxPauseBrief = Math.Round((totTempsPauseBrief / 360000 / tempsLog) * 100, 2);
                TauxPauseDej = Math.Round((totTempsPauseDej / 360000 / tempsLog) * 100, 2);
                TauxMissionOP = Math.Round((totTempsMissionOp / 360000 / tempsLog) * 100, 2);
                TauxFormation = Math.Round((totTempsFormation / 360000 / tempsLog) * 100, 2);
                TauxQualification = Math.Round((totTempsQualification / 360000 / tempsLog) * 100, 2);
                TauxPhoning = Math.Round((totTempsPhoning / 360000 / tempsLog) * 100, 2);
                TauxTempsAdmin = Math.Round((totTempsAdmin / 360000 / tempsLog) * 100, 2);
                TauxAppelIP = Math.Round((totAppelIP / 360000 / tempsLog) * 100, 2);
                TauxProdBrut = Math.Round((totTempsTravail / 360000 / tempsLog) * 100, 2);
                TauxProdNet = Math.Round((totProdNet / 360000 / tempsLog) * 100, 2);
            }
         
            calculs.Add(new SamRcIndicator
            {
                AppelsRecus = TotAppelsRecus,
                AppelsPris = TotAppelsPris,
                AppelsDecroches = TotAppelsDecroches,
                AppelsDecrochesinfdix = TotAppelsDecrochesinfdix,
                AppelsAbandonnes = TotAppelsAbandonnes,
                AppelsDebordes = TotAppelsDebordes,
                AppelsNoAgent = TotAppelsNoAgent,
                AppelsPerdus = TotAppelsPerdus,
                AppelsAttenteAvantAgent = TotAppelsAttenteAvantAgent,
                AppelsAttenteAvantAbandon = TotAppelsAttenteAvantAbandon,
                ObjectifAppelsPris = Math.Truncate(ObjectifAppelsPris),
                AtteinteObjectif = PerceintAtteinteObjectif,
                QS = PercentQS,
                AppelsEmis = TotAppelsEmis,
                TempsCommEntrant = TempsCommEntrant,
                TauxCommEntrant = TauxCommEntrant,
                TempsCommSortant = TempsCommSortant,
                TauxCommSortant = TauxCommSortant,
                JoursTravailles = totJoursTravailles,
                DMCEntrant = DMCEntrant,
                DMCSortant = DMCSortant,
                TempsLog = TempsLog,
                ETP = ETP,
                TempsCommunication = TempsCommunication,
                TempsAttente = TempsAttente,
                TempsACW = TempsACW,
                TempsPauseCafe = TempsPauseCafe,
                TempsPauseBrief = TempsPauseBrief,
                TempsPauseDej = TempsPauseDej,
                TempsMissionOp = TempsMissionOp,
                TempsFormation = TempsFormation,
                TempsQualification = TempsQualification,
                TempsPhoning = TempsPhoning,
                TempsAdmin = TempsAdmin,
                TempsAppelIP = TempsAppelIP,
                TempsProdBrut = TempsProdBrut,
                TempsProdNet = TempsProdNet,
                TauxComm = TauxComm,
                TauxAttente = TauxAttente,
                TauxACW = TauxACW,
                TauxPauseCafe = TauxPauseCafe,
                TauxPauseBrief = TauxPauseBrief,
                TauxPauseDej = TauxPauseDej,
                TauxMissionOP = TauxMissionOP,
                TauxFormation = TauxFormation,
                TauxQualification = TauxQualification,
                TauxPhoning = TauxPhoning,
                TauxTempsAdmin = TauxTempsAdmin,
                TauxAppelIP = TauxAppelIP,
                TauxProdBrut = TauxProdBrut,
                TauxProdNet = TauxProdNet
            });
            return Json(calculs, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetHebdoValues(string agentSel, int semaineSeL, int anneeSel)
        {
            List<SamRcIndicator> calculs = new List<SamRcIndicator>();
            //Data
            var appelsEntrants = db.Appels_Entrants_SamRc.Where(app => app.semaine == semaineSeL && app.Annee == anneeSel && app.FERIE == 0 && app.Closed == 0 && app.LastQueue != 0).ToList();
            var appelSortants = db.Appels_Sortants_SamRc.Where(s => s.semaine == semaineSeL && s.Annee == anneeSel && s.Closed == 0).ToList();
            var attendances = db.AttendanceHermes_SamRc.Where(att => att.semaine == semaineSeL && att.Annee == anneeSel).ToList();
            var temps = db.Temps_SamRc.Where(t => t.semaine == semaineSeL && t.Annee == anneeSel).ToList();
            if (agentSel != "")
            {
                int IdHermes = int.Parse(agentSel);
                appelsEntrants = db.Appels_Entrants_SamRc.Where(app => app.semaine == semaineSeL && app.Annee == anneeSel && app.LastAgent == IdHermes && app.FERIE == 0).ToList();
                appelSortants = db.Appels_Sortants_SamRc.Where(s => s.semaine == semaineSeL && s.Annee == anneeSel && s.Closed == 0 && s.LastAgent == IdHermes).ToList();
                attendances = db.AttendanceHermes_SamRc.Where(att => att.semaine == semaineSeL && att.Annee == anneeSel && att.Id_Hermes == IdHermes).ToList();
                temps = db.Temps_SamRc.Where(t => t.semaine == semaineSeL && t.Annee == anneeSel && t.AgentId == IdHermes).ToList();
            }

            //Variable indicateurs des appels
            int TotAppelsPris = 0, TotAppelsDecroches = 0, TotAppelsDecrochesinfdix = 0, TotAppelsAbandonnes = 0,
            TotAppelsDebordes = 0, TotAppelsNoAgent = 0, TotAppelsPerdus = 0, TotAppelsAttenteAvantAgent = 0, TotAppelsAttenteAvantAbandon = 0, TotConvDuration = 0;
            Double TotAppelsRecus = 0, ObjectifAppelsPris = 0, PerceintAtteinteObjectif = 0, PercentQS = 0, TauxCommEntrant = 0, totDMCEntrant = 0;
            //variables ppels Sortants 
            int TotAppelsEmis = 0, TotAppelsDecrochesSortant = 0, TotConvDurationSortant = 0;
            double TauxCommSortant = 0, totDMCSortant = 0;
            //variables attendances
            int totJoursTravailles = 0; double tempsLog = 0; double ETP = 0;
            //variables Téléphoniques (temps)
            double totTempsComm = 0, totTepmsAttente = 0, totTempsACW = 0, totTempsPauseCafe = 0, totTempsPauseBrief = 0, totTempsPauseDej = 0, totTempsMissionOp = 0, totTempsFormation = 0, totTempsQualification = 0, totTempsPhoning = 0, totTempsAdmin = 0, totAppelIP = 0;
            double totTempsTravail = 0, totProdNet = 0;
            double TauxComm = 0, TauxAttente = 0, TauxACW = 0, TauxPauseCafe = 0, TauxPauseBrief = 0, TauxPauseDej = 0, TauxMissionOP = 0, TauxFormation = 0,
                TauxQualification = 0, TauxPhoning = 0, TauxTempsAdmin = 0, TauxAppelIP = 0, TauxProdBrut = 0, TauxProdNet = 0;

            //Table Appels Entrants
            foreach (var item in appelsEntrants)
            {
                TotConvDuration += item.ConvDuration;
                TotAppelsRecus += 1;
                if (item.LastAgent != 0)
                {
                    TotAppelsPris += 1;
                }
                if (item.ConvDuration != 0)
                {
                    TotAppelsDecroches += 1;
                }
                if (item.ConvDuration < 10)
                {
                    TotAppelsDecrochesinfdix += 1;
                }
                if (item.Abandon == 1 && item.ConvDuration == 0)
                {
                    TotAppelsAbandonnes += 1;
                }
                if (item.Overflow == 1)
                {
                    TotAppelsDebordes += 1;
                }
                if (item.NoAgent == 1)
                {
                    TotAppelsNoAgent += 1;
                }
                if (item.WaitDuration != 0)
                {
                    TotAppelsAttenteAvantAgent += 1;
                }
                if (item.WaitDuration != 0 && item.LastAgent != 0)
                {
                    TotAppelsAttenteAvantAbandon += 1;
                }
            }
            TotAppelsPerdus = TotAppelsAbandonnes + TotAppelsDebordes + TotAppelsNoAgent;
            ObjectifAppelsPris = Math.Round((TotAppelsRecus * 0.95), 0);
            var TempsCommEntrant = TimeSpan.FromSeconds(TotConvDuration);
            if (TotAppelsDecroches != 0)
            {
                totDMCEntrant = TotConvDuration / TotAppelsDecroches;
            }

            var DMCEntrant = TimeSpan.FromSeconds(totDMCEntrant);
            if (ObjectifAppelsPris != 0)
            {
                PerceintAtteinteObjectif = Math.Round((TotAppelsPris / ObjectifAppelsPris) * 100, 2);
            }
            if (TotAppelsRecus != 0)
            {
                PercentQS = Math.Round((TotAppelsDecroches / TotAppelsRecus) * 100, 2);
            }

            //Table Appels Sortants
            foreach (var item in appelSortants)
            {
                TotConvDurationSortant += item.ConvDuration;
                TotAppelsEmis += 1;
                if (item.ConvDuration != 0)
                {
                    TotAppelsDecrochesSortant += 1;
                }
            }
            var TempsCommSortant = TimeSpan.FromSeconds(TotConvDurationSortant);
            if (TotAppelsDecrochesSortant != 0)
            {
                totDMCSortant = TotConvDurationSortant / TotAppelsDecrochesSortant;
            }
            var DMCSortant = TimeSpan.FromSeconds(totDMCSortant);

            //Table Attendances
            if (attendances != null)
            {
                totJoursTravailles = 1;
            }
            foreach (var att in attendances)
            {
                TimeSpan dep = (att.Depart.TimeOfDay);
                TimeSpan arr = (att.Arrive.TimeOfDay);
                tempsLog += (dep - arr).TotalHours;
            }

            //Table Temps
            foreach (var item in temps)
            {
                totTempsComm += item.tps_com;
                totTepmsAttente += item.tps_att;
                totTempsACW += item.tps_acw;
                totTempsPauseCafe += item.Temps_P_Cafe;
                totTempsPauseBrief += item.Temps_P_Brief;
                totTempsPauseDej += item.Temps_P_Dej;
                totTempsMissionOp += item.Temps_P_Mission_OP;
                totTempsFormation += item.Temps_P_Formation;
                totTempsQualification += item.Temps_P_Qualif;
                totTempsPhoning += item.Temps_P_phonning;
                totTempsAdmin += item.Temps_P_admin;
                totAppelIP += item.Temps_Appel_Ip;
            }
            totTempsTravail = totTempsComm + totTepmsAttente + totTempsACW + totTempsMissionOp;
            totProdNet = totTempsTravail - (totTempsMissionOp + totTempsACW);
            //convertion en temps hh:mm:ss
            var TempsLog = TimeSpan.FromHours(tempsLog);
            var TempsProdBrut = TimeSpan.FromMilliseconds(totTempsTravail * 10);
            var TempsProdNet = TimeSpan.FromMilliseconds(totProdNet * 10);
            var TempsCommunication = TimeSpan.FromMilliseconds(totTempsComm * 10);
            var TempsAttente = TimeSpan.FromMilliseconds(totTepmsAttente * 10);
            var TempsACW = TimeSpan.FromMilliseconds(totTempsACW * 10);
            var TempsPauseCafe = TimeSpan.FromMilliseconds(totTempsPauseCafe * 10);
            var TempsPauseBrief = TimeSpan.FromMilliseconds(totTempsPauseBrief * 10);
            var TempsPauseDej = TimeSpan.FromMilliseconds(totTempsPauseDej * 10);
            var TempsMissionOp = TimeSpan.FromMilliseconds(totTempsMissionOp * 10);
            var TempsFormation = TimeSpan.FromMilliseconds(totTempsFormation * 10);
            var TempsQualification = TimeSpan.FromMilliseconds(totTempsQualification * 10);
            var TempsPhoning = TimeSpan.FromMilliseconds(totTempsPhoning * 10);
            var TempsAdmin = TimeSpan.FromMilliseconds(totTempsAdmin * 10);
            var TempsAppelIP = TimeSpan.FromMilliseconds(totAppelIP * 10);

            if (tempsLog != 0)
            {
                ETP = Math.Round((tempsLog / totJoursTravailles / 8), 2);
                TauxCommEntrant = Math.Round(((TotConvDuration / 3600) / tempsLog) * 100, 2);
                TauxCommSortant = Math.Round(((TotConvDurationSortant / 3600) / tempsLog) * 100, 2);

                TauxComm = Math.Round((totTempsComm / 360000 / tempsLog) * 100, 2);
                TauxAttente = Math.Round((totTepmsAttente / 360000 / tempsLog) * 100, 2);
                TauxACW = Math.Round((totTempsACW / 360000 / tempsLog) * 100, 2);
                TauxPauseCafe = Math.Round((totTempsPauseCafe / 360000 / tempsLog) * 100, 2);
                TauxPauseBrief = Math.Round((totTempsPauseBrief / 360000 / tempsLog) * 100, 2);
                TauxPauseDej = Math.Round((totTempsPauseDej / 360000 / tempsLog) * 100, 2);
                TauxMissionOP = Math.Round((totTempsMissionOp / 360000 / tempsLog) * 100, 2);
                TauxFormation = Math.Round((totTempsFormation / 360000 / tempsLog) * 100, 2);
                TauxQualification = Math.Round((totTempsQualification / 360000 / tempsLog) * 100, 2);
                TauxPhoning = Math.Round((totTempsPhoning / 360000 / tempsLog) * 100, 2);
                TauxTempsAdmin = Math.Round((totTempsAdmin / 360000 / tempsLog) * 100, 2);
                TauxAppelIP = Math.Round((totAppelIP / 360000 / tempsLog) * 100, 2);
                TauxProdBrut = Math.Round((totTempsTravail / 360000 / tempsLog) * 100, 2);
                TauxProdNet = Math.Round((totProdNet / 360000 / tempsLog) * 100, 2);
            }

            calculs.Add(new SamRcIndicator
            {
                AppelsRecus = TotAppelsRecus,
                AppelsPris = TotAppelsPris,
                AppelsDecroches = TotAppelsDecroches,
                AppelsDecrochesinfdix = TotAppelsDecrochesinfdix,
                AppelsAbandonnes = TotAppelsAbandonnes,
                AppelsDebordes = TotAppelsDebordes,
                AppelsNoAgent = TotAppelsNoAgent,
                AppelsPerdus = TotAppelsPerdus,
                AppelsAttenteAvantAgent = TotAppelsAttenteAvantAgent,
                AppelsAttenteAvantAbandon = TotAppelsAttenteAvantAbandon,
                ObjectifAppelsPris = Math.Truncate(ObjectifAppelsPris),
                AtteinteObjectif = PerceintAtteinteObjectif,
                QS = PercentQS,
                AppelsEmis = TotAppelsEmis,
                TempsCommEntrant = TempsCommEntrant,
                TauxCommEntrant = TauxCommEntrant,
                TempsCommSortant = TempsCommSortant,
                TauxCommSortant = TauxCommSortant,
                JoursTravailles = totJoursTravailles,
                DMCEntrant = DMCEntrant,
                DMCSortant = DMCSortant,
                TempsLog = TempsLog,
                ETP = ETP,
                TempsCommunication = TempsCommunication,
                TempsAttente = TempsAttente,
                TempsACW = TempsACW,
                TempsPauseCafe = TempsPauseCafe,
                TempsPauseBrief = TempsPauseBrief,
                TempsPauseDej = TempsPauseDej,
                TempsMissionOp = TempsMissionOp,
                TempsFormation = TempsFormation,
                TempsQualification = TempsQualification,
                TempsPhoning = TempsPhoning,
                TempsAdmin = TempsAdmin,
                TempsAppelIP = TempsAppelIP,
                TempsProdBrut = TempsProdBrut,
                TempsProdNet = TempsProdNet,
                TauxComm = TauxComm,
                TauxAttente = TauxAttente,
                TauxACW = TauxACW,
                TauxPauseCafe = TauxPauseCafe,
                TauxPauseBrief = TauxPauseBrief,
                TauxPauseDej = TauxPauseDej,
                TauxMissionOP = TauxMissionOP,
                TauxFormation = TauxFormation,
                TauxQualification = TauxQualification,
                TauxPhoning = TauxPhoning,
                TauxTempsAdmin = TauxTempsAdmin,
                TauxAppelIP = TauxAppelIP,
                TauxProdBrut = TauxProdBrut,
                TauxProdNet = TauxProdNet
            });
            return Json(calculs, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMensuelValues(string agentSel, string moisSel)
        {
            List<SamRcIndicator> calculs = new List<SamRcIndicator>();
            int year = int.Parse(moisSel.Substring(0, 4));
            int month = int.Parse(moisSel.Substring(5, 2));
            //Data
            var appelsEntrants = db.Appels_Entrants_SamRc.Where(app => app.Date.Month == month && app.Annee == year && app.FERIE == 0 && app.Closed == 0 && app.LastQueue != 0).ToList();
            var appelSortants = db.Appels_Sortants_SamRc.Where(s => s.date.Month == month && s.Annee == year && s.Closed == 0).ToList();
            var attendances = db.AttendanceHermes_SamRc.Where(att => att.Date.Month == month && att.Annee == year).ToList();
            var temps = db.Temps_SamRc.Where(t => t.date.Month == month && t.Annee == year).ToList();
            if (agentSel != "")
            {
                int IdHermes = int.Parse(agentSel);
                appelsEntrants = db.Appels_Entrants_SamRc.Where(app => app.Date.Month == month && app.Date.Year == year && app.LastAgent == IdHermes && app.FERIE == 0).ToList();
                appelSortants = db.Appels_Sortants_SamRc.Where(s => s.date.Month == month && s.Annee == year && s.Closed == 0 && s.LastAgent == IdHermes).ToList();
                attendances = db.AttendanceHermes_SamRc.Where(att => att.Date.Month == month && att.Annee == year && att.Id_Hermes == IdHermes).ToList();
                temps = db.Temps_SamRc.Where(t => t.date.Month == month && t.Annee == year && t.AgentId == IdHermes).ToList();
            }
            //variables appels Entrants 
            int TotAppelsPris = 0, TotAppelsDecroches = 0, TotAppelsDecrochesinfdix = 0, TotAppelsAbandonnes = 0, TotAppelsDebordes = 0,
       TotAppelsNoAgent = 0, TotAppelsPerdus = 0, TotAppelsAttenteAvantAgent = 0, TotAppelsAttenteAvantAbandon = 0, TotConvDuration = 0;
            Double TotAppelsRecus = 0, ObjectifAppelsPris = 0, PerceintAtteinteObjectif = 0, PercentQS = 0, TauxCommEntrant = 0, totDMCEntrant = 0;
            //variables ppels Sortants 
            int TotAppelsEmis = 0, TotAppelsDecrochesSortant = 0, TotConvDurationSortant = 0;
            double TauxCommSortant = 0, totDMCSortant = 0;
            //variables attendances
            int totJoursTravailles = 0; double tempsLog = 0; double ETP = 0;
            //variables Téléphoniques (temps)
            double totTempsComm = 0, totTepmsAttente = 0, totTempsACW = 0, totTempsPauseCafe = 0, totTempsPauseBrief = 0, totTempsPauseDej = 0, totTempsMissionOp = 0, totTempsFormation = 0, totTempsQualification = 0, totTempsPhoning = 0, totTempsAdmin = 0, totAppelIP = 0;
            double totTempsTravail = 0, totProdNet = 0;
            double TauxComm = 0, TauxAttente = 0, TauxACW = 0, TauxPauseCafe = 0, TauxPauseBrief = 0, TauxPauseDej = 0, TauxMissionOP = 0, TauxFormation = 0,
                TauxQualification = 0, TauxPhoning = 0, TauxTempsAdmin = 0, TauxAppelIP = 0, TauxProdBrut = 0, TauxProdNet = 0;

            //Table Appels Entrants
            foreach (var item in appelsEntrants)
            {
                TotConvDuration += item.ConvDuration;
                TotAppelsRecus += 1;
                if (item.LastAgent != 0)
                {
                    TotAppelsPris += 1;
                }
                if (item.ConvDuration != 0)
                {
                    TotAppelsDecroches += 1;
                }
                if (item.ConvDuration < 10)
                {
                    TotAppelsDecrochesinfdix += 1;
                }
                if (item.Abandon == 1 && item.ConvDuration == 0)
                {
                    TotAppelsAbandonnes += 1;
                }
                if (item.Overflow == 1)
                {
                    TotAppelsDebordes += 1;
                }
                if (item.NoAgent == 1)
                {
                    TotAppelsNoAgent += 1;
                }
                if (item.WaitDuration != 0)
                {
                    TotAppelsAttenteAvantAgent += 1;
                }
                if (item.WaitDuration != 0 && item.LastAgent != 0)
                {
                    TotAppelsAttenteAvantAbandon += 1;
                }
            }
            TotAppelsPerdus = TotAppelsAbandonnes + TotAppelsDebordes + TotAppelsNoAgent;
            ObjectifAppelsPris = Math.Round((TotAppelsRecus * 0.95), 0);
            var TempsCommEntrant = TimeSpan.FromSeconds(TotConvDuration);
            if (TotAppelsDecroches != 0)
            {
                totDMCEntrant = TotConvDuration / TotAppelsDecroches;
            }

            var DMCEntrant = TimeSpan.FromSeconds(totDMCEntrant);
            if (ObjectifAppelsPris != 0)
            {
                PerceintAtteinteObjectif = Math.Round((TotAppelsPris / ObjectifAppelsPris) * 100, 2);
            }
            if (TotAppelsRecus != 0)
            {
                PercentQS = Math.Round((TotAppelsDecroches / TotAppelsRecus) * 100, 2);
            }

            //Table Appels Sortants
            foreach (var item in appelSortants)
            {
                TotConvDurationSortant += item.ConvDuration;
                TotAppelsEmis += 1;
                if (item.ConvDuration != 0)
                {
                    TotAppelsDecrochesSortant += 1;
                }
            }
            var TempsCommSortant = TimeSpan.FromSeconds(TotConvDurationSortant);
            if (TotAppelsDecrochesSortant != 0)
            {
                totDMCSortant = TotConvDurationSortant / TotAppelsDecrochesSortant;
            }
            var DMCSortant = TimeSpan.FromSeconds(totDMCSortant);

            //Table Attendances
            if (attendances != null)
            {
                totJoursTravailles = 1;
            }
            foreach (var att in attendances)
            {
                TimeSpan dep = (att.Depart.TimeOfDay);
                TimeSpan arr = (att.Arrive.TimeOfDay);
                tempsLog += (dep - arr).TotalHours;
            }

            //Table Temps
            foreach (var item in temps)
            {
                totTempsComm += item.tps_com;
                totTepmsAttente += item.tps_att;
                totTempsACW += item.tps_acw;
                totTempsPauseCafe += item.Temps_P_Cafe;
                totTempsPauseBrief += item.Temps_P_Brief;
                totTempsPauseDej += item.Temps_P_Dej;
                totTempsMissionOp += item.Temps_P_Mission_OP;
                totTempsFormation += item.Temps_P_Formation;
                totTempsQualification += item.Temps_P_Qualif;
                totTempsPhoning += item.Temps_P_phonning;
                totTempsAdmin += item.Temps_P_admin;
                totAppelIP += item.Temps_Appel_Ip;
            }
            totTempsTravail = totTempsComm + totTepmsAttente + totTempsACW + totTempsMissionOp;
            totProdNet = totTempsTravail - (totTempsMissionOp + totTempsACW);
            //convertion en temps hh:mm:ss
            var TempsLog = TimeSpan.FromHours(tempsLog);
            var TempsProdBrut = TimeSpan.FromMilliseconds(totTempsTravail * 10);
            var TempsProdNet = TimeSpan.FromMilliseconds(totProdNet * 10);
            var TempsCommunication = TimeSpan.FromMilliseconds(totTempsComm * 10);
            var TempsAttente = TimeSpan.FromMilliseconds(totTepmsAttente * 10);
            var TempsACW = TimeSpan.FromMilliseconds(totTempsACW * 10);
            var TempsPauseCafe = TimeSpan.FromMilliseconds(totTempsPauseCafe * 10);
            var TempsPauseBrief = TimeSpan.FromMilliseconds(totTempsPauseBrief * 10);
            var TempsPauseDej = TimeSpan.FromMilliseconds(totTempsPauseDej * 10);
            var TempsMissionOp = TimeSpan.FromMilliseconds(totTempsMissionOp * 10);
            var TempsFormation = TimeSpan.FromMilliseconds(totTempsFormation * 10);
            var TempsQualification = TimeSpan.FromMilliseconds(totTempsQualification * 10);
            var TempsPhoning = TimeSpan.FromMilliseconds(totTempsPhoning * 10);
            var TempsAdmin = TimeSpan.FromMilliseconds(totTempsAdmin * 10);
            var TempsAppelIP = TimeSpan.FromMilliseconds(totAppelIP * 10);

            if (tempsLog != 0)
            {
                ETP = Math.Round((tempsLog / totJoursTravailles / 8), 2);
                TauxCommEntrant = Math.Round(((TotConvDuration / 3600) / tempsLog) * 100, 2);
                TauxCommSortant = Math.Round(((TotConvDurationSortant / 3600) / tempsLog) * 100, 2);

                TauxComm = Math.Round((totTempsComm / 360000 / tempsLog) * 100, 2);
                TauxAttente = Math.Round((totTepmsAttente / 360000 / tempsLog) * 100, 2);
                TauxACW = Math.Round((totTempsACW / 360000 / tempsLog) * 100, 2);
                TauxPauseCafe = Math.Round((totTempsPauseCafe / 360000 / tempsLog) * 100, 2);
                TauxPauseBrief = Math.Round((totTempsPauseBrief / 360000 / tempsLog) * 100, 2);
                TauxPauseDej = Math.Round((totTempsPauseDej / 360000 / tempsLog) * 100, 2);
                TauxMissionOP = Math.Round((totTempsMissionOp / 360000 / tempsLog) * 100, 2);
                TauxFormation = Math.Round((totTempsFormation / 360000 / tempsLog) * 100, 2);
                TauxQualification = Math.Round((totTempsQualification / 360000 / tempsLog) * 100, 2);
                TauxPhoning = Math.Round((totTempsPhoning / 360000 / tempsLog) * 100, 2);
                TauxTempsAdmin = Math.Round((totTempsAdmin / 360000 / tempsLog) * 100, 2);
                TauxAppelIP = Math.Round((totAppelIP / 360000 / tempsLog) * 100, 2);
                TauxProdBrut = Math.Round((totTempsTravail / 360000 / tempsLog) * 100, 2);
                TauxProdNet = Math.Round((totProdNet / 360000 / tempsLog) * 100, 2);
            }

            calculs.Add(new SamRcIndicator
            {
                AppelsRecus = TotAppelsRecus,
                AppelsPris = TotAppelsPris,
                AppelsDecroches = TotAppelsDecroches,
                AppelsDecrochesinfdix = TotAppelsDecrochesinfdix,
                AppelsAbandonnes = TotAppelsAbandonnes,
                AppelsDebordes = TotAppelsDebordes,
                AppelsNoAgent = TotAppelsNoAgent,
                AppelsPerdus = TotAppelsPerdus,
                AppelsAttenteAvantAgent = TotAppelsAttenteAvantAgent,
                AppelsAttenteAvantAbandon = TotAppelsAttenteAvantAbandon,
                ObjectifAppelsPris = Math.Truncate(ObjectifAppelsPris),
                AtteinteObjectif = PerceintAtteinteObjectif,
                QS = PercentQS,
                AppelsEmis = TotAppelsEmis,
                TempsCommEntrant = TempsCommEntrant,
                TauxCommEntrant = TauxCommEntrant,
                TempsCommSortant = TempsCommSortant,
                TauxCommSortant = TauxCommSortant,
                JoursTravailles = totJoursTravailles,
                DMCEntrant = DMCEntrant,
                DMCSortant = DMCSortant,
                TempsLog = TempsLog,
                ETP = ETP,
                TempsCommunication = TempsCommunication,
                TempsAttente = TempsAttente,
                TempsACW = TempsACW,
                TempsPauseCafe = TempsPauseCafe,
                TempsPauseBrief = TempsPauseBrief,
                TempsPauseDej = TempsPauseDej,
                TempsMissionOp = TempsMissionOp,
                TempsFormation = TempsFormation,
                TempsQualification = TempsQualification,
                TempsPhoning = TempsPhoning,
                TempsAdmin = TempsAdmin,
                TempsAppelIP = TempsAppelIP,
                TempsProdBrut = TempsProdBrut,
                TempsProdNet = TempsProdNet,
                TauxComm = TauxComm,
                TauxAttente = TauxAttente,
                TauxACW = TauxACW,
                TauxPauseCafe = TauxPauseCafe,
                TauxPauseBrief = TauxPauseBrief,
                TauxPauseDej = TauxPauseDej,
                TauxMissionOP = TauxMissionOP,
                TauxFormation = TauxFormation,
                TauxQualification = TauxQualification,
                TauxPhoning = TauxPhoning,
                TauxTempsAdmin = TauxTempsAdmin,
                TauxAppelIP = TauxAppelIP,
                TauxProdBrut = TauxProdBrut,
                TauxProdNet = TauxProdNet
            });
            return Json(calculs, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTrancheHoraireValues(DateTime dateDebut, DateTime dateFin)
        {
            List<SamRcIndicator> calculs = new List<SamRcIndicator>();
          

            for (int heure=7; heure <= 18; heure++)
            {
                var data = db.Appels_Entrants_SamRc.Where(app => app.FERIE == 0 && app.Closed == 0 && app.LastQueue != 0 && app.Heure == heure).ToList();

                double totAppelsRecus = 0, totAppelsDecroches = 0, totAppelsPerdus = 0, PercentQS = 0, totAppelsAbandonnes = 0, totAppelsDebordes = 0, totAppelsNoAgent = 0;
                double totConvDuration = 0, totWrapup = 0; float totDMC = 0, totDMT = 0;
                foreach (var item in data)
                {
                    DateTime ItemDate = item.Date;

                    if (ItemDate.Date >= dateDebut.Date && ItemDate.Date <= dateFin.Date)
                    {
                        totAppelsRecus += 1;
                        totConvDuration += item.ConvDuration;
                        totWrapup += item.WrapupDuration;
                        if (item.ConvDuration != 0)
                        {
                            totAppelsDecroches += 1;
                        }
                        if (item.Abandon == 1 && item.ConvDuration == 0)
                        {
                            totAppelsAbandonnes += 1;
                        }
                        if (item.Overflow == 1)
                        {
                            totAppelsDebordes += 1;
                        }
                        if (item.NoAgent == 1)
                        {
                            totAppelsNoAgent += 1;
                        }
                    }
                }

                if(totAppelsDecroches == 0)
                {
                     totDMC = 0;
                    totDMT = 0;
                }
                else
                {
                    totDMC = (float)(totConvDuration / totAppelsDecroches);
                    totDMT = (float)((totConvDuration + totWrapup)/ totAppelsDecroches);
                }
                
                var DMC = TimeSpan.FromSeconds(totDMC);
                var DMT = TimeSpan.FromSeconds(totDMT);
           
                totAppelsPerdus = totAppelsAbandonnes + totAppelsDebordes + totAppelsNoAgent;
                if (totAppelsDecroches != 0)
                {
                    PercentQS = Math.Round((totAppelsDecroches / totAppelsRecus) * 100, 2);
                }
                calculs.Add(new SamRcIndicator {Heure = heure, AppelsRecus = totAppelsRecus, AppelsDecroches = totAppelsDecroches, AppelsPerdus = totAppelsPerdus, QS = PercentQS, DMC = DMC, DMT= DMT });
            }

            return Json(calculs, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTrancheDemiHeureValues(DateTime dateDebut, DateTime dateFin)
        {
            List<SamRcIndicator> calculs = new List<SamRcIndicator>();
           
            var data = service.GetAppelsEntrantsBetweenTwoDates(dateDebut, dateFin);

            TimeSpan debut = new TimeSpan(07, 00, 00);
            TimeSpan fin = new TimeSpan(18, 00, 00);
        
                for (TimeSpan d = debut; d <= fin; d+=TimeSpan.FromMinutes(30))
            {
                double totAppelsRecus = 0, totAppelsDecroches = 0, totAppelsPerdus = 0, PercentQS = 0, totAppelsAbandonnes = 0, totAppelsDebordes = 0, totAppelsNoAgent = 0;
                double totConvDuration = 0, totWrapup = 0; float totDMC = 0, totDMT = 0;
                string fromHour = "";
                string toHour = "";
                TimeSpan f = d + TimeSpan.FromMinutes(30);
                foreach (var item in data)
                {
                    if (item.Date.TimeOfDay >= d && item.Date.TimeOfDay <= f) 
                    {
                        totAppelsRecus += 1;
                        totConvDuration += item.ConvDuration;
                        totWrapup += item.WrapupDuration;
                        if (item.ConvDuration != 0)
                        {
                            totAppelsDecroches += 1;
                        }
                        if (item.Abandon == 1 && item.ConvDuration == 0)
                        {
                            totAppelsAbandonnes += 1;
                        }
                        if (item.Overflow == 1)
                        {
                            totAppelsDebordes += 1;
                        }
                        if (item.NoAgent == 1)
                        {
                            totAppelsNoAgent += 1;
                        }
                    }
                }

                if (totAppelsDecroches == 0)
                {
                    totDMC = 0;
                    totDMT = 0;
                }
                else
                {
                    totDMC = (float)(totConvDuration / totAppelsDecroches);
                    totDMT = (float)((totConvDuration + totWrapup) / totAppelsDecroches);
                }

                var DMC = TimeSpan.FromSeconds(totDMC);
                var DMT = TimeSpan.FromSeconds(totDMT);
                totAppelsPerdus = totAppelsAbandonnes + totAppelsDebordes + totAppelsNoAgent;
                if (totAppelsDecroches != 0)
                {
                    PercentQS = Math.Round((totAppelsDecroches / totAppelsRecus) * 100, 2);
                }
                fromHour = d.ToString().Substring(0,5);
                toHour = f.ToString().Substring(0,5);
                calculs.Add(new SamRcIndicator {HeureDebut = fromHour, HeureFin = toHour, AppelsRecus = totAppelsRecus, AppelsDecroches = totAppelsDecroches, AppelsPerdus = totAppelsPerdus, QS = PercentQS, DMC = DMC, DMT = DMT });
            }

            return Json(calculs, JsonRequestBehavior.AllowGet);
        }

    }
}
