using Data;
using Domain.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using MVCWEB.Models;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MVCWEB.Controllers
{
    [Authorize(Roles = "Admin ,Manager,Agent,SuperManager")]

    public class HomeController : Controller
    {
        IEmployeeService serviceEmployees;
        IUtilisateurService serviceUser;
        IGroupeEmployeeService serviceGroupeEmp;
        IGroupeService serviceGroupe;
        IEventService serviceEvent;
        static int idEmpConnecte;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        public HomeController()
        {
            serviceEmployees = new EmployeeService();
            serviceUser = new UtilisateurService();
            serviceGroupeEmp = new GroupesEmployeService();
            serviceGroupe = new GroupeService();
            serviceEvent = new EventService();

        }
        public HomeController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
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

        [HttpGet]
        public ActionResult IndexManagerAgentTest(int id)
        {
            try
            {
                Employee empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
                //string value = (string)Session["loginIndex"];
                //Employee empConnected = serviceEmployees.getById(idEmpConnecte);
                if (empConnected.Content != null)
                {
                    String strbase64 = Convert.ToBase64String(empConnected.Content);
                    String empConnectedImage = "data:" + empConnected.ContentType + ";base64," + strbase64;
                    ViewBag.empConnectedImage = empConnectedImage;
                }
                ViewBag.nameEmpConnected = empConnected.UserName;
                ViewBag.pseudoNameEmpConnected = empConnected.pseudoName;
                var employees = serviceEmployees.GetAll();
                Groupe grp = serviceGroupe.getById(id);
                List<Employee> emps = serviceGroupeEmp.getListEmployeeByGroupeId(id);
                Utilisateur user = new Utilisateur();
                List<EmployeeViewModel> fVM = new List<EmployeeViewModel>();
                List<SelectListItem> groupesassocies = new List<SelectListItem>();
                String Url = null;
                foreach (var item in emps)
                {
                    if (item.Content != null)
                    {
                        String strbase64 = Convert.ToBase64String(item.Content);
                        Url = "data:" + item.ContentType + ";base64," + strbase64;
                        ViewBag.url = Url;
                    }
                    if (item.userId != null)
                    {
                        if (item.Id != empConnected.Id && item.Roles.Any(r => r.UserId == item.Id && r.RoleId == 3))
                        {

                            fVM.Add(
                              new EmployeeViewModel
                              {
                                  Id = item.Id,
                                  userName = item.UserName,
                                  pseudoName = item.pseudoName,
                                  userLogin = item.userLogin,
                                  Activite = grp.nom,
                                  IdHermes = item.IdHermes,
                                  image = Url,
                                  role = "Agent",
                              });
                        }

                    }
                    else
                    {
                        user = null;
                        if (item.Id != empConnected.Id)
                        {
                            fVM.Add(
                      new EmployeeViewModel
                      {
                          Id = item.Id,
                          userName = item.UserName,
                          pseudoName = item.pseudoName,
                          userLogin = "",
                          Activite = item.Activite,
                          IdHermes = item.IdHermes,
                          role = item.role,
                          image = Url
                          //groupesassocies = groupesassocies


                      });
                        }
                    }
                    //groupesassocies.Clear();


                }

                return View(fVM);   //fVM.Take(10)



                //if (empConnected == null)
                //{
                //    ViewBag.message = ("session cleared!");
                //    ViewBag.color = "red";
                //    return View("~/Views/Authentification/Index.cshtml");
                //}
                //else
                //{
                //}
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e);
                return RedirectToAction("Connect", "Authentification");

            }
        }
        [HttpGet]
        public ActionResult IndexManagerAgent()
        {
            try
            {
                string value = (string)Session["loginIndex"];
                var employees = serviceEmployees.GetAll();
                Utilisateur user = new Utilisateur();
                List<EmployeeViewModel> fVM = new List<EmployeeViewModel>();
                List<SelectListItem> groupesassocies = new List<SelectListItem>();

                foreach (var item in employees)
                {
                    if (item.Content != null)
                    {
                        String strbase64 = Convert.ToBase64String(item.Content);
                        String Url = "data:" + item.ContentType + ";base64," + strbase64;
                        ViewBag.url = Url;
                    }
                    if (item.userId != null)
                    {
                        user = serviceUser.getById(item.userId);

                        fVM.Add(
                          new EmployeeViewModel
                          {
                              Id = item.Id,
                              userName = item.UserName,
                              pseudoName = item.pseudoName,
                              userLogin = item.userLogin,
                              Activite = item.Activite,
                              IdHermes = item.IdHermes,
                              role = item.role
                          //groupesassocies=groupesassocies



                      });
                    }
                    else
                    {
                        user = null;

                        fVM.Add(
                      new EmployeeViewModel
                      {
                          Id = item.Id,
                          userName = item.UserName,
                          pseudoName = item.pseudoName,
                          userLogin = "",
                          Activite = item.Activite,
                          IdHermes = item.IdHermes,
                          role = item.role
                      //groupesassocies = groupesassocies


                  });
                    }
                    //groupesassocies.Clear();


                }




                if (value == null)
                {
                    ViewBag.message = ("session cleared!");
                    ViewBag.color = "red";
                    return View("~/Views/Authentification/Index.cshtml");
                }
                else
                {
                    return View(fVM);   //fVM.Take(10)
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e);
                return RedirectToAction("Connect", "Authentification");

            }
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [Authorize(Roles = "Manager,SuperManager")]
        public ActionResult IndexManagerGroupes()
        {
            try
            {
                //string value = (string)Session["loginIndex"];
                //var employees = service.GetAll();
                //if (id == null)
                //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                //
                //Employee item = serviceEmployees.getById(id);
                var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
                idEmpConnecte = user.Id;
                var a = new EmployeeViewModel();
                //a.Id = user.Id;
                //a.userName = user.UserName;
                //a.pseudoName = user.pseudoName;
                //a.IdAD = (int)user.userId;
                //a.IdHermes = user.IdHermes;
                //a.Activite = user.Activite;
                //a.role = user.role;
                //string type = form["typeGenerator"].ToString();
                //var logins = serviceUser.GetAll();
                //var tests = logins.Select(o => o.login).Distinct().ToList();
                //foreach (var test in tests)
                //{
                //    a.utilisateurs.Add(new SelectListItem { Text = test, Value = test });
                //}
                List<SelectListItem> groupes = new List<SelectListItem>();
                var groupesassociees = serviceGroupeEmp.getGroupeByIDEmployee(user.Id);
                var groupesassociees_tests = groupesassociees.Select(o => o).Distinct().ToList();
                foreach(var g in groupesassociees_tests)
                {
                    groupes.Add(new SelectListItem { Text = g.nom, Value = g.Id.ToString() });
                }
                ViewBag.Groupes = groupes;
                a.Group = new List<Groupe>();
                foreach (var test in groupesassociees_tests)
                {
                    a.Group.Add(test);
                }
                //if (user.Content != null)
                //{
                //    String strbase64 = Convert.ToBase64String(user.Content);
                //    String Url = "data:" + user.ContentType + ";base64," + strbase64;
                //    ViewBag.url = Url;
                //    a.image = Url;
                //}
                a.empId = "" + user.Id;

                a.userName = user.UserName;
                a.pseudoName = user.pseudoName;
                if (user.Content != null)
                {
                    String strbase64 = Convert.ToBase64String(user.Content);
                    String Url = "data:" + user.ContentType + ";base64," + strbase64;
                    ViewBag.url = Url;
                    a.Url = Url;
                }

                //  a.PhotoUrl = airflight.PhotoUrl;
                if (user == null)
                    return HttpNotFound();




                //if (value == null)
                //{
                //    ViewBag.message = ("session cleared!");
                //    ViewBag.color = "red";
                //    return View("~/Views/Authentification/Index.cshtml");
                //}
                //else
                //{
                return View(a);   //fVM.Take(10)
                                  //}
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e);
                return RedirectToAction("Connect", "Authentification");

            }
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        [HttpGet]
        public ActionResult FindEmployee(int? Id)
        {
            Employee item = serviceEmployees.getById(Id);


            var a = new EmployeeViewModel();
            a.Id = item.Id;
            a.userName = item.UserName;
            a.pseudoName = item.pseudoName;
            a.IdAD = (int)item.userId;
            a.IdHermes = item.IdHermes;
            a.Activite = item.Activite;
            a.role = item.role;


            if (Request.IsAjaxRequest())
            {
                return PartialView("_AlerteStatAttendance", a);
            }

            else
            {
                return View(a);
            }
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult TestChart()
        {

            return View();
        }
        public ActionResult ManagerGroupes()
        {
            //string value = (string)Session["loginIndex"];
            //Employee empConnected = serviceEmployees.getById(idEmpConnecte);

            Employee empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

            if (empConnected.Content != null)
            {
                String strbase64 = Convert.ToBase64String(empConnected.Content);
                String empConnectedImage = "data:" + empConnected.ContentType + ";base64," + strbase64;
                ViewBag.empConnectedImage = empConnectedImage;
            }
            if (empConnected == null)
            {
                ViewBag.message = ("session cleared!");
                ViewBag.color = "red";
                return View("~/Views/Authentification/Index.cshtml");
            }
            else
            {
                int IdEmpConnected = empConnected.Id;
                return RedirectToAction("IndexManagerGroupes", "Home");
            }
        }
        public ActionResult Calendar()
        {
            try
            {
                //String login = Session["loginIndex"].ToString();

                //Employee item = serviceEmployees.getByLoginUser(login);
                Employee item = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
                if (item.Content != null)
                {
                    String strbase64 = Convert.ToBase64String(item.Content);
                    String empConnectedImage = "data:" + item.ContentType + ";base64," + strbase64;
                    ViewBag.empConnectedImage = empConnectedImage;
                    ViewBag.nameEmpConnected = item.UserName;
                    ViewBag.pseudoNameEmpConnected = item.pseudoName;
                    ViewBag.IdEmpConnected = item.Id;
                }
                var groupesassociees = serviceGroupeEmp.getGroupeByIDEmployee(item.Id);
                var usersAssociees = serviceGroupeEmp.getListEmployeeByGroupe(item.Id);
                EventViewModel a = new EventViewModel();
                var logins = serviceUser.GetAll();
                var tests = usersAssociees.Select(o => o.UserName).Distinct().ToList();
                foreach (String test in tests)
                {
                    a.utilisateurs.Add(new SelectListItem { Text = test, Value = test });
                }
                var groupes = serviceGroupe.GetAll();
                var groupeslogins = groupesassociees.Select(o => o.nom).Distinct().ToList();
                foreach (var test in groupeslogins)
                {
                    //foreach(var assoc in groupesassociees){
                    //    if (!(test.nom).Equals(assoc.nom)){
                    a.groupesass.Add(new SelectListItem { Text = test, Value = test });
                    //}
                }
                using (ReportContext dc = new ReportContext())
                {
                    var events = dc.events.ToList();
                    List<Event> eventsTests = new List<Event>();
                    List<DateTime> dateTimes = new List<DateTime>();
                    List<String> titles = new List<String>();
                    string[] arr1 = new string[400];
                    foreach (var test in events)
                    {
                        eventsTests.Add(test);
                        dateTimes.Add(test.dateDebut);
                        titles.Add(test.titre);
                        //arr1[1]=(test.titre);
                        //arr1[2] = (""+test.dateDebut);
                    }
                    ViewBag.events = eventsTests;
                    ViewBag.titres = titles;
                    ViewBag.dateTimes = dateTimes;
                    ViewBag.arr1 = arr1;
                }
                return View(a);
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e);
                return RedirectToAction("Connect", "Authentification");

            }
            //using (ReportContext dc = new ReportContext())
            //{
            //    var events = dc.events.ToList();
            //    List<Event> eventsTests = new List<Event>();
            //    List<DateTime> dateTimes = new List<DateTime>();
            //    List<String> titles = new List<String>();

            //    foreach (var test in events)
            //    {
            //        eventsTests.Add(test);
            //        dateTimes.Add(test.dateDebut);
            //        titles.Add(test.titre);
            //    }
            //    ViewBag.titres = titles;
            //    ViewBag.dateTimes = dateTimes;
            //    return View(events);
            //}
        }
        public ActionResult GetEventsByImed()
        {
            try
            {
                //String login = Session["loginIndex"].ToString();
                //Employee item = serviceEmployees.getByLoginUser(login);
                Employee item = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
                List<Event> eventsTestsAll = new List<Event>();
                List<Employee> emps = serviceGroupeEmp.getListEmployeeByGroupe(item.Id);
                var testsEmps = emps.Distinct().ToList();
                List<Groupe> groupes = serviceGroupeEmp.getGroupeByIDEmployee(item.Id);
                List<Event> groupesEvents = serviceEvent.getListEventsByListGroupes(groupes);
                var groupesEventsDistinct = groupesEvents.Distinct().ToList();
                foreach (var groupeEvent in groupesEventsDistinct)
                {
                    //groupeEvent.start = groupes[0].nom;
                    eventsTestsAll.Add(groupeEvent);
                }
                foreach (var employee in testsEmps)
                {
                    var eventss = serviceEvent.getListEventsByEmployeeId(employee.Id);
                    foreach (var eventt in eventss)
                    {
                        eventsTestsAll.Add(eventt);
                    }
                }
                var eventsTests = serviceEvent.GetAll();
                var eventsAll = new List<EventViewModel>();
                foreach (var test in eventsTestsAll.Distinct())
                {
                    Employee emp = serviceEmployees.getById(test.employeeId);
                    EventViewModel a = new EventViewModel();
                    a.Id = test.Id;
                    a.start = test.start;
                    a.end = test.end;
                    a.dateDebut = test.dateDebut;
                    a.dateFin = test.dateFin;
                    a.themeColor = test.themeColor;
                    if (emp != null)
                    {
                        a.employeeName = emp.UserName;
                    }
                    a.description = test.description;
                    if (test.titre.Equals("Congé") || (test.titre.Equals("Autorisation")))
                    {
                        a.titre = test.titre + " " + emp.UserName;
                    }
                    else
                    {
                        a.titre = test.titre + " " + test.start;
                    }
                    eventsAll.Add(a);
                }
                var eventList = from e in eventsAll

                                select new
                                {

                                    id = e.Id,

                                    title = e.titre,

                                    start = e.dateDebut.ToString("s"),

                                    end = e.dateFin.ToString("s"),
                                    //end = e.dateFin.AddDays(1).ToString("s"),


                                    color = e.themeColor,

                                    allDay = false

                                };
                var events = eventList.ToArray();

                return Json(events, JsonRequestBehavior.AllowGet);
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e);
                return RedirectToAction("Connect", "Authentification");

            }
        }
        public JsonResult GetEvents(double start, double end)

        {
            var events = serviceEvent.GetAll();
            //List<Event> fVM = new List<Event>();
            //foreach (var test in events)
            //{
            //    fVM.Add(test);
            //}


            var fromDate = ConvertFromUnixTimestamp(start);

            var toDate = ConvertFromUnixTimestamp(end);




            var eventList = from e in events

                            select new
                            {

                                id = e.Id,

                                title = e.titre,

                                start = e.dateDebut.ToString("s") + " " + e.start + ":00",

                                end = e.dateFin.ToString("s") + " " + e.end + ":00",

                                color = e.themeColor,

                                allDay = false

                            };

            var rows = eventList.ToArray();

            return Json(rows, JsonRequestBehavior.AllowGet);

        }
        private static DateTime ConvertFromUnixTimestamp(double timestamp)

        {

            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);

            return origin.AddSeconds(timestamp);

        }

        public ActionResult FindEvent(int? Id)
        {
            Event eventt = serviceEvent.getById(Id);
            Employee item = serviceEmployees.getById(eventt.employeeId);


            var a = new EventViewModel();
            a.Id = eventt.Id;
            a.description = eventt.description;
            a.dateDebut = eventt.dateDebut;
            a.dateFin = eventt.dateFin;
            if (item != null)
            {
                a.employeeName = item.UserName;
            }
            if (eventt.titre.Equals("Congé") || (eventt.titre.Equals("Autorisation")))
            {
                a.titre = eventt.titre + " " + item.UserName;
            }
            else
            {
                a.titre = eventt.titre;
            }


            if (Request.IsAjaxRequest())
            {
                return PartialView("_AlerteDetailsEvent", a);
            }

            else
            {
                return View(a);
            }
        }
        public ActionResult DeleteEvent(int? id)
        {

            Event eventt = serviceEvent.getById(id);

            serviceEvent.Delete(eventt);
            serviceEvent.SaveChange();
            return RedirectToAction("Calendar");
        }
        public ActionResult Messages(int? id)
        {
            return View();
        }
        public ActionResult ManagerJournalierAgent()
        {
            //string value = (string)Session["loginIndex"];
            Employee empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            //Employee empConnected = serviceEmployees.getById(idEmpConnecte);
            if (empConnected == null)
            {
                ViewBag.message = ("session cleared!");
                ViewBag.color = "red";
                return View("~/Views/Authentification/Index.cshtml");
            }
            else
            {
                int IdEmpConnected = empConnected.Id;
                return RedirectToAction("Index", "Indicateurs", new { @id = IdEmpConnected });
            }
        }

        public ActionResult ManagerHebdoAgent()
        {
            //string value = (string)Session["loginIndex"];
            //Employee empConnected = serviceEmployees.getById(idEmpConnecte);
            Employee empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

            if (empConnected == null)
            {
                ViewBag.message = ("session cleared!");
                ViewBag.color = "red";
                return View("~/Views/Authentification/Index.cshtml");
            }
            else
            {
                int IdEmpConnected = empConnected.Id;
                return RedirectToAction("Hebdo", "Indicateurs", new { @id = IdEmpConnected });
            }
        }

        public ActionResult ManagerMensuelAgent()
        {
            //string value = (string)Session["loginIndex"];
            //Employee empConnected = serviceEmployees.getById(idEmpConnecte);
            Employee empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

            if (empConnected == null)
            {
                ViewBag.message = ("session cleared!");
                ViewBag.color = "red";
                return View("~/Views/Authentification/Index.cshtml");
            }
            else
            {
                int IdEmpConnected = empConnected.Id;
                return RedirectToAction("Mensuel", "Indicateurs", new { @id = IdEmpConnected });
            }
        }

        public ActionResult ManagerTrimestrielleAgent()
        {
            //string value = (string)Session["loginIndex"];
            //Employee empConnected = serviceEmployees.getById(idEmpConnecte);
            Employee empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

            if (empConnected == null)
            {
                ViewBag.message = ("session cleared!");
                ViewBag.color = "red";
                return View("~/Views/Authentification/Index.cshtml");
            }
            else
            {
                int IdEmpConnected = empConnected.Id;
                return RedirectToAction("Trimestriel", "Indicateurs", new { @id = IdEmpConnected });
            }
        }

        public ActionResult ManagerAnnuelleAgent()
        {
            //string value = (string)Session["loginIndex"];
            //Employee empConnected = serviceEmployees.getById(idEmpConnecte);
            Employee empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

            if (empConnected == null)
            {
                ViewBag.message = ("session cleared!");
                ViewBag.color = "red";
                return View("~/Views/Authentification/Index.cshtml");
            }
            else
            {
                int IdEmpConnected = empConnected.Id;
                return RedirectToAction("Annuelle", "Indicateurs", new { @id = IdEmpConnected });
            }
        }

        public ActionResult ManagerJournalierActivity()
        {
            //string value = (string)Session["loginIndex"];
            //Employee empConnected = serviceEmployees.getById(idEmpConnecte);
            Employee empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

            if (empConnected == null)
            {
                ViewBag.message = ("session cleared!");
                ViewBag.color = "red";
                return View("~/Views/Authentification/Index.cshtml");
            }
            else
            {
                int IdEmpConnected = empConnected.Id;
                return RedirectToAction("JournalierActivity", "IndicateursActivity", new { @id = IdEmpConnected });
            }
        }
        // Fin Controllers in Manager Template

        //Controllers in Agent Template   
        public ActionResult JournalierAgent()
        {
            //string value = (string)Session["loginIndex"];
            //Employee empConnected = serviceEmployees.getByLoginUser(value);
            Employee empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

            if (empConnected == null)
            {
                ViewBag.message = ("session cleared!");
                ViewBag.color = "red";
                return View("~/Views/Authentification/Index.cshtml");
            }
            else
            {
                int IdEmpConnected = empConnected.Id;
                return RedirectToAction("JournalierAgent", "Indicateurs", new { @id = IdEmpConnected });
            }
        }
        public ActionResult HebdoAgent()
        {
            //string value = (string)Session["loginIndex"];
            //Employee empConnected = serviceEmployees.getByLoginUser(value);
            Employee empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

            if (empConnected == null)
            {
                ViewBag.message = ("session cleared!");
                ViewBag.color = "red";
                return View("~/Views/Authentification/Index.cshtml");
            }
            else
            {
                int IdEmpConnected = empConnected.Id;
                return RedirectToAction("HebdoAgent", "Indicateurs", new { @id = IdEmpConnected });
            }
        }
        public ActionResult MensuelAgent()
        {
            //string value = (string)Session["loginIndex"];
            //Employee empConnected = serviceEmployees.getByLoginUser(value);
            Employee empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

            if (empConnected == null)
            {
                ViewBag.message = ("session cleared!");
                ViewBag.color = "red";
                return View("~/Views/Authentification/Index.cshtml");
            }
            else
            {
                int IdEmpConnected = empConnected.Id;
                return RedirectToAction("MensuelAgent", "Indicateurs", new { @id = IdEmpConnected });
            }
        }
        public ActionResult TrimestrielAgent()
        {
            //string value = (string)Session["loginIndex"];
            //Employee empConnected = serviceEmployees.getByLoginUser(value);
            Employee empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

            if (empConnected == null)
            {
                ViewBag.message = ("session cleared!");
                ViewBag.color = "red";
                return View("~/Views/Authentification/Index.cshtml");
            }
            else
            {
                int IdEmpConnected = empConnected.Id;
                return RedirectToAction("TrimestrielAgent", "Indicateurs", new { @id = IdEmpConnected });
            }
        }
        public ActionResult AnnuelleAgent()
        {
            //string value = (string)Session["loginIndex"];
            //Employee empConnected = serviceEmployees.getByLoginUser(value);
            Employee empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

            if (empConnected == null)
            {
                ViewBag.message = ("session cleared!");
                ViewBag.color = "red";
                return View("~/Views/Authentification/Index.cshtml");
            }
            else
            {
                int IdEmpConnected = empConnected.Id;
                return RedirectToAction("AnnuelleAgent", "Indicateurs", new { @id = IdEmpConnected });
            }
        }
        //Fin Controllers in Agent Template 

        // Controllers in Manager Template for selected agent
        public ActionResult JournalierSelectedAgent(int? id)
        {
            string value = (string)Session["loginIndex"];
            Employee empConnected = serviceEmployees.getByLoginUser(value);
            if (value == null)
            {
                ViewBag.message = ("session cleared!");
                ViewBag.color = "red";
                return View("~/Views/Authentification/Index.cshtml");
            }
            else
            {
                if (empConnected.Content != null)
                {
                    String strbase64 = Convert.ToBase64String(empConnected.Content);
                    String empConnectedImage = "data:" + empConnected.ContentType + ";base64," + strbase64;
                    ViewBag.empConnectedImage = empConnectedImage;
                }
                ViewBag.nameEmpConnected = empConnected.UserName;
                ViewBag.pseudoNameEmpConnected = empConnected.pseudoName;

                serviceEmployees = new EmployeeService();
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Employee emp = serviceEmployees.getById(id);
                if (emp == null)
                {
                    return HttpNotFound();
                }
                return View(emp);
            }
        }

        public ActionResult JournalierSelectedAgent1(int? id)
        {
            var empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (empConnected == null)
            {
                ViewBag.message = ("session cleared!");
                ViewBag.color = "red";
                return View("~/Views/Authentification/Index.cshtml");
            }
            else
            {
                if (empConnected.Content != null)
                {
                    String strbase64 = Convert.ToBase64String(empConnected.Content);
                    String empConnectedImage = "data:" + empConnected.ContentType + ";base64," + strbase64;
                    ViewBag.empConnectedImage = empConnectedImage;
                }
                ViewBag.nameEmpConnected = empConnected.UserName;
                ViewBag.pseudoNameEmpConnected = empConnected.pseudoName;

                serviceEmployees = new EmployeeService();
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Employee emp = serviceEmployees.getById(id);
                if (emp == null)
                {
                    return HttpNotFound();
                }
                return View(emp);
            }
        }
        public ActionResult HebdoSelectedAgent(int? id)
        {
            var semaines = new List<SelectListItem>();
            for (int m = 1; m <= 52; m++)
            {
                var val = m.ToString();

                semaines.Add(new SelectListItem { Text = "Semaine" + val, Value = val });
            }
            ViewBag.SemaineItems = semaines;
            var empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (empConnected == null)
            {
                ViewBag.message = ("session cleared!");
                ViewBag.color = "red";
                return View("~/Views/Authentification/Index.cshtml");
            }
            else
            {
                if (empConnected.Content != null)
                {
                    String strbase64 = Convert.ToBase64String(empConnected.Content);
                    String empConnectedImage = "data:" + empConnected.ContentType + ";base64," + strbase64;
                    ViewBag.empConnectedImage = empConnectedImage;
                }
                ViewBag.nameEmpConnected = empConnected.UserName;
                ViewBag.pseudoNameEmpConnected = empConnected.pseudoName;

                serviceEmployees = new EmployeeService();
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Employee emp = serviceEmployees.getById(id);
                if (emp == null)
                {
                    return HttpNotFound();
                }
                return View(emp);
            }
        }
        public ActionResult MensuelSelectedAgent(int? id)
        {
            var empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (empConnected == null)
            {
                ViewBag.message = ("session cleared!");
                ViewBag.color = "red";
                return View("~/Views/Authentification/Index.cshtml");
            }
            else
            {
                if (empConnected.Content != null)
                {
                    String strbase64 = Convert.ToBase64String(empConnected.Content);
                    String empConnectedImage = "data:" + empConnected.ContentType + ";base64," + strbase64;
                    ViewBag.empConnectedImage = empConnectedImage;
                }
                ViewBag.nameEmpConnected = empConnected.UserName;
                ViewBag.pseudoNameEmpConnected = empConnected.pseudoName;

                serviceEmployees = new EmployeeService();
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Employee emp = serviceEmployees.getById(id);
                if (emp == null)
                {
                    return HttpNotFound();
                }
                return View(emp);
            }
        }
        public ActionResult TrimestrielSelectedAgent(int? id)
        {
            var empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (empConnected == null)
            {
                ViewBag.message = ("session cleared!");
                ViewBag.color = "red";
                return View("~/Views/Authentification/Index.cshtml");
            }
            else
            {
                if (empConnected.Content != null)
                {
                    String strbase64 = Convert.ToBase64String(empConnected.Content);
                    String empConnectedImage = "data:" + empConnected.ContentType + ";base64," + strbase64;
                    ViewBag.empConnectedImage = empConnectedImage;
                }
                ViewBag.nameEmpConnected = empConnected.UserName;
                ViewBag.pseudoNameEmpConnected = empConnected.pseudoName;

                serviceEmployees = new EmployeeService();
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Employee emp = serviceEmployees.getById(id);
                if (emp == null)
                {
                    return HttpNotFound();
                }
                return View(emp);
            }
        }
        public ActionResult AnnuelleSelectedAgent(int? id)
        {
            var empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (empConnected == null)
            {
                ViewBag.message = ("session cleared!");
                ViewBag.color = "red";
                return View("~/Views/Authentification/Index.cshtml");
            }
            else
            {
                if (empConnected.Content != null)
                {
                    String strbase64 = Convert.ToBase64String(empConnected.Content);
                    String empConnectedImage = "data:" + empConnected.ContentType + ";base64," + strbase64;
                    ViewBag.empConnectedImage = empConnectedImage;
                }
                ViewBag.nameEmpConnected = empConnected.UserName;
                ViewBag.pseudoNameEmpConnected = empConnected.pseudoName;

                serviceEmployees = new EmployeeService();
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Employee emp = serviceEmployees.getById(id);
                if (emp == null)
                {
                    return HttpNotFound();
                }
                return View(emp);
            }
        }

        public ActionResult ManagerHebdoActivity()
        {
            //string value = (string)Session["loginIndex"];
            //Employee empConnected = serviceEmployees.getById(idEmpConnecte);
            Employee empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

            if (empConnected == null)
            {
                ViewBag.message = ("session cleared!");
                ViewBag.color = "red";
                return View("~/Views/Authentification/Index.cshtml");
            }
            else
            {
                int IdEmpConnected = empConnected.Id;
                return RedirectToAction("HebdoActivity", "IndicateursActivity", new { @id = IdEmpConnected });
            }
        }
        public ActionResult ManagerMensuelActivity()
        {
            //string value = (string)Session["loginIndex"];
            //Employee empConnected = serviceEmployees.getById(idEmpConnecte);
            Employee empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

            if (empConnected == null)
            {
                ViewBag.message = ("session cleared!");
                ViewBag.color = "red";
                return View("~/Views/Authentification/Index.cshtml");
            }
            else
            {
                int IdEmpConnected = empConnected.Id;
                return RedirectToAction("MensuelActivity", "IndicateursActivity", new { @id = IdEmpConnected });
            }
        }
        public ActionResult ManagerTrimestrielActivity()
        {
            //string value = (string)Session["loginIndex"];
            //Employee empConnected = serviceEmployees.getById(idEmpConnecte);
            Employee empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

            if (empConnected == null)
            {
                ViewBag.message = ("session cleared!");
                ViewBag.color = "red";
                return View("~/Views/Authentification/Index.cshtml");
            }
            else
            {
                int IdEmpConnected = empConnected.Id;
                return RedirectToAction("TrimestrielActivity", "IndicateursActivity", new { @id = IdEmpConnected });
            }
        }
        public ActionResult ManagerAnnuelleActivity()
        {
            //string value = (string)Session["loginIndex"];
            //Employee empConnected = serviceEmployees.getById(idEmpConnecte);
            Employee empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

            if (empConnected == null)
            {
                ViewBag.message = ("session cleared!");
                ViewBag.color = "red";
                return View("~/Views/Authentification/Index.cshtml");
            }
            else
            {
                int IdEmpConnected = empConnected.Id;
                return RedirectToAction("AnnuelleActivity", "IndicateursActivity", new { @id = IdEmpConnected });
            }
        }
        public ActionResult IndexAgentGroupes()
        {
            //string value = (string)Session["loginIndex"];
            //Employee empConnected = serviceEmployees.getByLoginUser(value);
            Employee empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

            if (empConnected == null)
            {
                ViewBag.message = ("session cleared!");
                ViewBag.color = "red";
                return View("~/Views/Authentification/Index.cshtml");
            }
            else
            {
                int IdEmpConnected = empConnected.Id;
                return RedirectToAction("IndexAgentGroupes", "IndicateursActivity", new { @id = IdEmpConnected });
            }
        }
        [Authorize(Roles = "Manager")]
        public ActionResult Notify()
        {
            return View();
        }
    }
}
