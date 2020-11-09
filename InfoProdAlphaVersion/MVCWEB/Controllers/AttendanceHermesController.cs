using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Data;
using MVCWEB.Models;
using Services;
using Domain.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Web;

namespace MVCWEB.Controllers
{
    [Authorize(Roles = "Manager,SuperManager")]
    public class AttendanceHermesController : Controller
    {
        private ReportContext db = new ReportContext();
        IGroupeEmployeeService serviceGroupeEmp;
        IEmployeeService service;
        //static String login;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        public AttendanceHermesController()
        {
            service = new EmployeeService();
            serviceGroupeEmp = new GroupesEmployeService();

        }
        public AttendanceHermesController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
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
        public ActionResult SuiviTemps()
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
                //login = "";
                //string strSessionID = HttpContext.Current.Session.SessionID;
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e);
                return RedirectToAction("Connect", "Authentification");

            }
        }

        public JsonResult GetHermesValues(string date)
        {
            var attendances = db.attendancesHermes.ToList();
            DateTime dateSel = DateTime.Parse(date);

            //string  l = login;           
            Employee empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

            List<Groupe> groupes = serviceGroupeEmp.getGroupeByIDEmployee(empConnected.Id);
            List<EmployeeViewModel> agentsassocies = new List<EmployeeViewModel>();
            foreach (var item in groupes)
            {
                List<Employee> emp = serviceGroupeEmp.getListEmployeeByGroupeId(item.Id);

                String Url = null;
                foreach (var e in emp)
                {
                    {
                        if (e.Id != empConnected.Id && e.Content != null && e.Roles.Any(r => r.UserId == e.Id && r.RoleId == 3))
                        {
                            if (!(agentsassocies.Exists(x => x.userName == e.UserName && x.IdHermes == e.IdHermes)))
                            {
                                String strbase64 = Convert.ToBase64String(e.Content);
                                Url = "data:" + e.ContentType + ";base64," + strbase64;
                                ViewBag.url = Url;
                                agentsassocies.Add(new EmployeeViewModel { Id = e.Id, userName = e.UserName, IdHermes = e.IdHermes, image = Url });
                            }
                        }
                    }
                }
            }
            ViewBag.AgentItems = agentsassocies;

            List<temps> Temps = new List<temps>();

            foreach (var item in agentsassocies)
            {
                List<GroupesEmployees> AgentGrp = serviceGroupeEmp.getByIDEmployee(item.Id);
                TimeSpan currentarrive;
                TimeSpan.TryParse("08:00:00", out currentarrive);
                TimeSpan currentdepart;
                TimeSpan.TryParse("17:00:00", out currentdepart);
                string arr = "-";
                string dep = "-";
                string retardarrive = "-";
                string retarddepart = "-";
                string cumulretard = "-";
                string signearrive = "";
                string signedepart = "";
                string signecumul = "";
                string agentgroupe = "";
                TimeSpan diffarrive = TimeSpan.Parse("00:00:00");
                TimeSpan diffdepart = TimeSpan.Parse("00:00:00");
                TimeSpan cumul = TimeSpan.Parse("00:00:00");
                var events = new List<Event>();
                foreach (var grp in AgentGrp)
                {
                    var groupeass = db.groupes.Where(g => g.Id == grp.groupeId).ToList();

                    foreach (var gr in groupeass)
                    {
                        agentgroupe = gr.nom;
                    }
                    var eventgroupeassocies = db.events.Where(ev => ev.groupes.Any(c => c.Id == grp.groupeId)).ToList();
                    events.AddRange(eventgroupeassocies);
                }

                foreach (var evt in events)
                {
                    if (evt.titre == "Planning" && evt.dateDebut.Date <= dateSel && evt.dateFin.Date >= dateSel)
                    {
                        currentarrive = evt.dateDebut.TimeOfDay;
                        currentdepart = evt.dateFin.TimeOfDay;
                    }
                }

                foreach (var a in attendances)
                {
                    if (item.IdHermes == a.Id_Hermes && dateSel == a.date)
                    {
                        if (a.Depart == null)
                        {
                            a.Depart = new DateTime(1991, 5, 1, 17, 00, 00);
                        }
                        arr = a.Arrive.Value.ToLongTimeString();
                        dep = a.Depart.Value.ToLongTimeString();
                        TimeSpan arrivetime = a.Arrive.Value.TimeOfDay;
                        TimeSpan departtime = a.Depart.Value.TimeOfDay;

                        diffarrive = currentarrive - arrivetime;
                        if (diffarrive >= TimeSpan.Parse("00:00:00"))
                        {
                            retardarrive = diffarrive.ToString().Substring(0, 8);
                            signearrive = "positif";
                        }
                        if (diffarrive < TimeSpan.Parse("00:00:00"))
                        {
                            retardarrive = diffarrive.ToString().Substring(0, 9);
                            signearrive = "negatif";
                        }

                        diffdepart = departtime - currentdepart;
                        if (diffdepart >= TimeSpan.Parse("00:00:00"))
                        {
                            retarddepart = diffdepart.ToString().Substring(0, 8);
                            signedepart = "positif";
                        }
                        if (diffdepart < TimeSpan.Parse("00:00:00"))
                        {
                            retarddepart = diffdepart.ToString().Substring(0, 9);
                            signedepart = "negatif";
                        }

                        cumul = diffarrive + diffdepart;
                        if (cumul >= TimeSpan.Parse("00:00:00"))
                        {
                            cumulretard = cumul.ToString().Substring(0, 8);
                            signecumul = "positif";
                        }
                        if (cumul < TimeSpan.Parse("00:00:00"))
                        {
                            cumulretard = cumul.ToString().Substring(0, 9);
                            signecumul = "negatif";
                        }

                    }
                }
                Temps.Add(new temps { Image = item.image, Agent = item.userName, IdHermes = item.IdHermes, groupe = agentgroupe, Arrive = arr, Depart = dep, RetardArrive = retardarrive, RetardDepart = retarddepart, CumulRetard = cumulretard, typearrive = signearrive, typedepart = signedepart, typecumul = signecumul });
            }

            return Json(Temps, JsonRequestBehavior.AllowGet);
        }






    }
}
