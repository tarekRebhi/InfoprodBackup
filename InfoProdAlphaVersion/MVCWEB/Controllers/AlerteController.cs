using Domain.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using MVCWEB.Models;
using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Threading.Tasks;

namespace MVCWEB.Controllers
{
    [CustomAuthorise(Roles = "Admin ,Manager,Agent,SuperManager")]

    //C # dosen't allow to inherit fom multiple classes but allow you to implement from multiple Interfaces
    public class AlerteController : Controller
    {
        // GET: Alerte
        #region memberVriable
        IAlerteService service;
        IEmployeeService serviceEmployee;
        IGroupeEmployeeService serviceGroupeEmp;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        private readonly static ConnectionDictionnary<string> connections =
           new ConnectionDictionnary<string>();
        IHubContext hubContext;
        #endregion

        #region Constructor
        public AlerteController()
        {

            service = new AlerteService();
            serviceEmployee = new EmployeeService();
            serviceGroupeEmp = new GroupesEmployeService();
            hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
        }
        public AlerteController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
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
        #region fonctionnalitéAdmin
        [CustomAuthorise(Roles = "Admin")]

        public ActionResult Index(String search, FormCollection form)
        {
            //string value = (string)Session["loginIndex"];
            try
            {
                var alertes = service.GetAll();
                List<Alerte> fVM = new List<Alerte>();
                //string type = form["test"].ToString();
                //int numVal = Int32.Parse(type);

                foreach (var item in alertes)
                {
                    fVM.Add(item);
                }
                if (!String.IsNullOrEmpty(search))
                {

                    fVM = fVM.Where(p => p.titreAlerte.ToLower().Contains(search.ToLower())).ToList<Alerte>();


                }
                return View(fVM);
                //if (value == null)
                //{
                //    ViewBag.message = ("session cleared!");
                //    ViewBag.color = "red";
                //    return View("~/Views/Authentification/Index.cshtml");
                //}
                //else
                //{
            }
            catch (NullReferenceException a)
            {
                Console.WriteLine(a);
                return View("~/Views/Authentification/Index.cshtml");
            }
            //fVM.Take(10)
            //}
        }

        // GET: Employee/Details/5
        [System.Web.Mvc.Authorize(Roles = "Admin")]

        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Alerte alerte = service.getById(id);
            return View(alerte);
        }

        // GET: Employee/Create
        [System.Web.Mvc.Authorize(Roles = "Admin")]

        public ActionResult Create()
        {
            var alerte = new Alerte();
            return View(alerte);
        }

        // POST: Medcin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [System.Web.Mvc.Authorize(Roles = "Admin")]

        public ActionResult Create(Alerte c, FormCollection form)
        {
            if (!ModelState.IsValid)
            {
                RedirectToAction("Create");
            }
            Alerte alerte = new Alerte
            {
                Id = c.Id,
                dateCreation = c.dateCreation,
                dateReponse = c.dateReponse,
                description = c.description,
                etatAlerte = c.etatAlerte,
                reponseAlerte = c.reponseAlerte,
                titreAlerte = c.titreAlerte


            };
            service.Add(alerte);
            service.SaveChange();


            return RedirectToAction("Index");

        }

        // GET: Employee/Edit/5
        [System.Web.Mvc.Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Alerte alerte = service.getById(id);

            return View(alerte);
        }

        // POST: Medcin/Edit/5
        [HttpPost, ActionName("Edit")]
        [System.Web.Mvc.Authorize(Roles = "Admin")]
        public ActionResult EditAlerte(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var alerte = service.getById(id);

            if (TryUpdateModel(alerte))
            {
                try
                {
                    service.SaveChange();
                    return RedirectToAction("Index");
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Erreur!!!!!");
                }
            }
            return View(alerte);

        }

        // GET: Employee/Delete/5
        [System.Web.Mvc.Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {

            Alerte alerte = service.getById(id);

            service.Delete(alerte);
            service.SaveChange();
            return RedirectToAction("Index");
        }

        // POST: Medcin/Delete/5
        //[HttpPost]
        //public ActionResult Delete(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add delete logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
        [HttpGet]
        [System.Web.Mvc.Authorize(Roles = "Admin")]

        public ActionResult FindAlerteAdmin(int? Id)
        {
            Alerte item = service.getById(Id);


            //var a = new EmployeeViewModel();
            //a.Id = item.Id;
            //a.userName = item.userName;
            //a.pseudoName = item.pseudoName;
            //a.IdAD = (int)item.userId;
            //a.IdHermes = item.IdHermes;
            //a.Activite = item.Activite;
            //a.role = item.role;
            if (Request.IsAjaxRequest())
            {
                return PartialView("_AlerteDeSuppression", item);
            }

            else
            {
                return View(item);
            }
        }
        #endregion
        #region fonctionnalitésManager
        [System.Web.Mvc.Authorize(Roles = "Manager,SuperManager")]

        public ActionResult IndexAgentManager(String search, FormCollection form)
        {
            try
            {
                //string value = (string)Session["loginIndex"];
                Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
                List<AlerteViewModel> a = new List<AlerteViewModel>();
                var alertes = service.GetAll();
                List<Alerte> fVM = new List<Alerte>();
                //string type = form["test"].ToString();
                //int numVal = Int32.Parse(type);

                foreach (var item in alertes)
                {
                    AlerteViewModel test = new AlerteViewModel();
                    test.titreAlerte = item.titreAlerte;
                    test.description = item.description;
                    if (item.senderId != null)
                    {
                        test.senderId = item.senderId;
                        Employee sender = serviceEmployee.getById(item.senderId);
                        test.senderName = sender.UserName;
                        test.Id = item.Id;
                        test.dateCreation = item.dateCreation;
                        test.reponseAlerte = item.reponseAlerte;
                        test.statusReponse = item.statusReponse;

                    }
                    if (item.reciverId != null)
                    {
                        test.reciverId = item.reciverId;
                        Employee reciver = serviceEmployee.getById(item.reciverId);
                        test.reciverName = reciver.UserName;
                        test.Id = item.Id;
                        test.dateCreation = item.dateCreation;
                        test.reponseAlerte = item.reponseAlerte;
                        test.statusReponse = item.statusReponse;

                    }
                    a.Add(test);
                }


                a = a.Where(p => p.senderId == emp.Id).ToList<AlerteViewModel>();

                List<AlerteViewModel> tests = a.Where(p => p.senderId == emp.Id).ToList<AlerteViewModel>();


                int b = tests.Count;
                //Session.Remove("alertes");
                //Session.Remove("nbAlertes");

                //Session["alertes"] = tests;
                //Session["nbAlertes"] = b;
                if (emp == null)
                {
                    ViewBag.message = ("session cleared!");
                    ViewBag.color = "red";
                    return View("~/Views/Authentification/Index.cshtml");
                }
                else
                {
                    if (emp.Content != null)
                    {
                        String strbase64 = Convert.ToBase64String(emp.Content);
                        String empConnectedImage = "data:" + emp.ContentType + ";base64," + strbase64;
                        ViewBag.empConnectedImage = empConnectedImage;
                    }
                    //else //
                    //{

                    //}
                    ViewBag.nameEmpConnected = emp.UserName;// la suite des instructions sera executé dans les deux cas if or else
                    ViewBag.pseudoNameEmpConnected = emp.pseudoName;
                    return View(a);   //fVM.Take(10)
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e);
                return RedirectToAction("Connect", "Authentification");

            }
        }
        public ActionResult IndexAgentManagerReciver(String search, FormCollection form)
        {
            try
            {
                string value = (string)Session["loginIndex"];
                Employee emp = serviceEmployee.getByLoginUser(value);
                List<AlerteViewModel> a = new List<AlerteViewModel>();
                var alertes = service.GetAll();
                List<Alerte> fVM = new List<Alerte>();
                //string type = form["test"].ToString();
                //int numVal = Int32.Parse(type);

                foreach (var item in alertes)
                {
                    AlerteViewModel test = new AlerteViewModel();
                    test.titreAlerte = item.titreAlerte;
                    test.description = item.description;
                    if (item.senderId != null)
                    {
                        Employee sender = serviceEmployee.getById(item.senderId);
                        test.senderId = item.senderId;
                        test.senderName = sender.UserName;
                        test.Id = item.Id;
                        test.dateCreation = item.dateCreation;


                    }
                    if (item.reciverId != null)
                    {
                        Employee reciver = serviceEmployee.getById(item.reciverId);
                        test.reciverName = reciver.UserName;
                        test.reciverId = item.reciverId;
                        test.Id = item.Id;
                        test.dateCreation = item.dateCreation;

                    }
                    a.Add(test);
                }


                a = a.Where(p => p.reciverId == emp.Id).ToList<AlerteViewModel>();
                int b = a.Count;


                if (value == null)
                {
                    ViewBag.nb = b;
                    ViewBag.message = ("session cleared!");
                    ViewBag.color = "red";
                    return View("~/Views/Authentification/Index.cshtml");
                }
                else
                {
                    ViewBag.alertes = a;
                    return View(a);   //fVM.Take(10)
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e);
                return RedirectToAction("Connect", "Authentification");

            }
        }
        #endregion
        [System.Web.Mvc.Authorize(Roles = "Agent")]

        public ActionResult IndexAgentRecived(String search, FormCollection form)
        {
            try
            {
                //string value = (string)Session["loginIndex"];
                //Employee emp = serviceEmployee.getByLoginUser(value);
                Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

                List<AlerteViewModel> a = new List<AlerteViewModel>();
                var alertes = service.GetAll();
                List<Alerte> fVM = new List<Alerte>();
                //string type = form["test"].ToString();
                //int numVal = Int32.Parse(type);

                foreach (var item in alertes)
                {
                    AlerteViewModel test = new AlerteViewModel();
                    test.titreAlerte = item.titreAlerte;
                    test.description = item.description;
                    if (item.senderId != null)
                    {
                        Employee sender = serviceEmployee.getById(item.senderId);
                        test.senderId = item.senderId;
                        test.senderName = sender.UserName;
                        test.status = item.status;
                        test.Id = item.Id;
                        test.dateCreation = item.dateCreation;
                        test.reponseAlerte = item.reponseAlerte;
                        test.statusReponse = item.statusReponse;
                    }
                    if (item.reciverId != null)
                    {
                        Employee reciver = serviceEmployee.getById(item.reciverId);
                        test.reciverName = reciver.UserName;
                        test.reciverId = item.reciverId;
                        test.status = item.status;
                        test.Id = item.Id;
                        test.dateCreation = item.dateCreation;
                        test.reponseAlerte = item.reponseAlerte;
                        test.statusReponse = item.statusReponse;
                    }
                    a.Add(test);
                }


                List<AlerteViewModel> recivedAlertes = a.Where(p => p.reciverId == emp.Id).ToList<AlerteViewModel>();
                a = a.Where(p => p.reciverId == emp.Id).ToList<AlerteViewModel>();
                //int b = 0;
                //if (a != null)
                //{
                //    b = a.Count;

                //}
                //else
                //{
                //    a = null;
                //    b = 0;

                //}
                if (emp == null)
                {
                    ViewBag.message = ("session cleared!");
                    ViewBag.color = "red";
                    return View("~/Views/Authentification/Index.cshtml");
                }
                else
                {
                    if (emp.Content != null)
                    {
                        String strbase64 = Convert.ToBase64String(emp.Content);
                        String empConnectedImage = "data:" + emp.ContentType + ";base64," + strbase64;
                        ViewBag.empConnectedImage = empConnectedImage;
                    }
                    ViewBag.nameEmpConnected = emp.UserName;
                    ViewBag.pseudoNameEmpConnected = emp.pseudoName;
                    //ViewBag.nb = b;
                    ViewBag.alertes = a;
                    return View(recivedAlertes);   //fVM.Take(10)
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e);
                return RedirectToAction("Connect", "Authentification");

            }
        }
        [System.Web.Mvc.Authorize(Roles = "Manager,SuperManager")]

        public ActionResult CreateAlerteManager()
        {
            //if (Session["loginIndex"].ToString() == null)
            //{
            //    return RedirectToAction("Connect", "Authentification");

            //}
            try
            {

                //String login = Session["loginIndex"].ToString();
                //Employee item = serviceEmployee.getByLoginUser(login);
                var item = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
                var a = new AlerteViewModel();
                var usersAssociees = serviceGroupeEmp.getListEmployeeByGroupe(item.Id);
                var tests = usersAssociees.Select(o => o).Distinct().ToList();
                foreach (var test in tests)
                {
                    if (test.UserName != item.UserName)
                    {
                        a.utilisateurs.Add(new SelectListItem { Value = test.UserName, Text = test.UserName });
                    }
                }
                if (item.Content != null)
                {
                    String strbase64 = Convert.ToBase64String(item.Content);
                    String empConnectedImage = "data:" + item.ContentType + ";base64," + strbase64;
                    ViewBag.empConnectedImage = empConnectedImage;
                }
                ViewBag.nameEmpConnected = item.UserName;
                ViewBag.pseudoNameEmpConnected = item.pseudoName;
                return View(a);

            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e);
                return RedirectToAction("Connect", "Authentification");

            }



        }

        // POST: Medcin/Create
        [HttpPost]

        //[ValidateAntiForgeryToken]
        [System.Web.Mvc.Authorize(Roles = "Manager,SuperManager")]

        public ActionResult CreateAlerteManager(AlerteViewModel a, string Id, string dateCreation, FormCollection form)
        {

            //String login = Session["loginIndex"].ToString();
            //Employee item = serviceEmployee.getByLoginUser(login);
            var item = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

            string username = form["typeGenerator"].ToString();
            //string message = form["message"].ToString();
            string titre = form["titre"].ToString();
            string description = form["description"].ToString();
            string dateTest = form["dateCreation"].ToString();
            DateTime dateCreationtest = DateTime.Parse(dateTest);

            Employee emp = serviceEmployee.getByLogin(username);
            //List<Alerte> alertes = service.getByDate(dateCreationtest, emp.Id);

            Alerte alerte = new Alerte
            {

                description = description,
                //reponseAlerte = message,
                titreAlerte = titre,
                reciverId = emp.Id,
                senderId = item.Id,
                dateCreation = dateCreationtest,
                status = "NotSeen",
                statusReponse = "NotSeen"

            };
            service.Add(alerte);
            service.SaveChange();
            string name = "Nouvelle Alerte du" + " " + item.UserName;
            string message = alerte.description;
            hubContext.Clients.User(emp.UserName).sendMessage(name, message);

            //NotificationHub hub = new NotificationHub();
            //string message = "Nouvelle Alerte Crée de la part du manager" + item.UserName;
            //hub.Broadcast(emp.UserName, message);
            return RedirectToAction("IndexAgentManager");



        }
        [System.Web.Mvc.Authorize(Roles = "Agent")]

        public ActionResult AlerteSeenByAgent()
        {
            try
            {
                //string value = (string)Session["loginIndex"];
                //Employee emp = serviceEmployee.getByLoginUser(value);
                Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
                var alertes = service.GetAll();
                List<AlerteViewModel> a = new List<AlerteViewModel>();
                //string type = form["test"].ToString();
                //int numVal = Int32.Parse(type);

                foreach (var item in alertes)
                {
                    AlerteViewModel test = new AlerteViewModel();
                    test.titreAlerte = item.titreAlerte;
                    test.description = item.description;
                    if (item.senderId != null)
                    {
                        Employee sender = serviceEmployee.getById(item.senderId);
                        test.senderId = item.senderId;
                        test.senderName = sender.UserName;
                        test.status = item.status;
                        test.Id = item.Id;

                    }
                    if (item.reciverId != null)
                    {
                        Employee reciver = serviceEmployee.getById(item.reciverId);
                        test.reciverName = reciver.UserName;
                        test.reciverId = item.reciverId;
                        test.status = item.status;
                        test.Id = item.Id;
                    }
                    a.Add(test);
                }


                List<AlerteViewModel> recivedAlertes = a.Where(p => p.reciverId == emp.Id && p.status == "seen").ToList<AlerteViewModel>();
                a = a.Where(p => p.reciverId == emp.Id && p.status == "NotSeen").ToList<AlerteViewModel>();
                int b = 0;

                if (a != null)
                {
                    b = a.Count;

                }
                else
                {
                    a = null;
                    b = 0;

                }
                if (emp == null)
                {

                    ViewBag.message = ("session cleared!");
                    ViewBag.color = "red";
                    return View("~/Views/Authentification/Index.cshtml");
                }
                else
                {
                    //Session.Remove("alertesAgent");
                    //Session.Remove("nbAlertesAgent");
                    //Session["alertesAgent"] = a;
                    //Session["nbAlertesAgent"] = b;
                    ViewBag.nb = b;
                    ViewBag.alertes = a;
                    return View(recivedAlertes);   //fVM.Take(10)
                }
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e);
                return RedirectToAction("Connect", "Authentification");

            }
        }
        [HttpGet]
        public ActionResult FindAlerte(string Id, string dateCreation)
        {


            Employee emp = serviceEmployee.getByLogin(Id);
            DateTime date = DateTime.Parse(dateCreation);
            List<Alerte> alertes = service.getByDate(date, 112);
            //String login = Session["loginIndex"].ToString();
            //Employee item = serviceEmployee.getByLoginUser(login);
            var a = new AlerteViewModel();
            var usersAssociees = serviceGroupeEmp.getListEmployeeByGroupe(104);
            var tests = usersAssociees.Select(o => o.UserName).Distinct().ToList();
            foreach (String test in tests)
            {
                a.utilisateurs.Add(new SelectListItem { Value = test, Text = test });
            }

            if (Request.IsAjaxRequest())
            {
                return View(a);

            }
            else
            {
                return PartialView("_AlerteDajoutAlerte", a);
            }
        }
        public ActionResult UpdateAlerte(int? Id)
        {
            Alerte alerte = service.getById(Id);
            if (TryUpdateModel(alerte))
            {
                try
                {
                    alerte.status = "seen";
                    service.SaveChange();
                    return RedirectToAction("AlerteSeenByAgent");
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Erreur!!!!!");
                }
            }
            return RedirectToAction("AlerteSeenByAgent");

        }
        public ActionResult UpdateAlerteReponseStatus(int? Id)
        {
            Alerte alerte = service.getById(Id);
            if (TryUpdateModel(alerte))
            {
                try
                {
                    alerte.statusReponse = "Lu";
                    service.SaveChange();
                    return RedirectToAction("IndexAgentManager");
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Erreur!!!!!");
                }
            }
            return RedirectToAction("IndexAgentManager");

        }
        [CustomAuthorise(Roles = "Agent")]
        public ActionResult ReponseAlerte(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Alerte alerte = service.getById(id);
            AlerteViewModel a = new AlerteViewModel();
            a.dateCreation = alerte.dateCreation;
            a.description = alerte.description;
            a.titreAlerte = alerte.titreAlerte;
            var emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

            if (emp.Content != null)
            {
                String strbase64 = Convert.ToBase64String(emp.Content);
                String empConnectedImage = "data:" + emp.ContentType + ";base64," + strbase64;
                ViewBag.empConnectedImage = empConnectedImage;
            }
            ViewBag.nameEmpConnected = emp.UserName;
            ViewBag.pseudoNameEmpConnected = emp.pseudoName;
            return View(a);
        }

        // POST: Medcin/Edit/5
        [HttpPost, ActionName("ReponseAlerte")]
        [CustomAuthorise(Roles = "Agent")]

        public ActionResult ReponseAlerteTest(int? id, FormCollection form)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var alerte = service.getById(id);
            string message = form["message"].ToString();
            alerte.reponseAlerte = message;
            alerte.statusReponse = "seen";
            service.SaveChange();

            Employee emp = serviceEmployee.getById(alerte.senderId);
            Employee emp2 = serviceEmployee.getById(alerte.reciverId);
            string name = "Alerte du" + " " + emp2.UserName;
            string reponse = alerte.reponseAlerte;
            hubContext.Clients.User(emp.UserName).reponseMessage(name, reponse);

            //hubContext.Clients.User(emp.UserName).sendMessage(name, reponse);
            //int a = Int32.Parse(Session["nbAlertes"].ToString())+1;
            //Session.Remove("nbAlertes");
            //Session["nbAlertes"] = a;
            return RedirectToAction("IndexAgentRecived");



        }

    }
}
