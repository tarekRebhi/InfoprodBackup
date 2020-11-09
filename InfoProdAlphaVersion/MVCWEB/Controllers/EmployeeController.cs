using Data;
using Data.Store;
using Domain.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using MVCWEB.Models;
using MyFinance.Data.Infrastructure;
using MyReports.Data.Infrastructure;
using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace MVCWEB.Controllers
{
    [Authorize(Roles = "Admin,Qualité, Agent Qualité,Agent Qualité_CustomerService,Agent Qualité_Diffusion,Agent Qualité_AchatPublic,Agent Qualité_PRV")]
    public class EmployeeController : Controller
    {
        // GET: Employee
        #region controllerGlobalVariable 
        IEmployeeService service;
        IUtilisateurService serviceUser;
        IGroupeService serviceGroupe;
        IGroupeEmployeeService servicegroupeEmployee;
        IEventService serviceEvent;
        IAlerteService serviceAlerte;
        ReportContext reportContext;
        IDatabaseFactory dbFactory;
        IUnitOfWork uow;
        IGroupeEmployeeService servicegroupemp;

        IGrilleEvaluationService serviceEvaluation;

        static int Idtest;
        static String[] ps;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        ReportContext context;
        #endregion
        #region Constructor
        public EmployeeController()
        {

            service = new EmployeeService();
            serviceUser = new UtilisateurService();
            serviceGroupe = new GroupeService();
            servicegroupeEmployee = new GroupesEmployeService();
            serviceEvent = new EventService();
            serviceAlerte = new AlerteService();
            reportContext = new ReportContext();
            dbFactory = new DatabaseFactory();
            uow = new UnitOfWork(dbFactory);
            servicegroupemp = new GroupesEmployeService();
            serviceEvaluation = new GrilleEvaluationService();
            context = new ReportContext();
        }
        public EmployeeController(ApplicationUserManager userManager, ApplicationSignInManager signInManager,ApplicationRoleManager roleManager)
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
        #region adminManipulationEmployee
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult Index(String search, FormCollection form,int? CallsToMake)
        {
            //string value = (string)Session["loginIndex"];
            var employees = service.GetAll();
            Utilisateur user = new Utilisateur();
            List<EmployeeViewModel> fVM = new List<EmployeeViewModel>();
            List<SelectListItem> groupesassocies = new List<SelectListItem>();

            foreach (var item in employees)
            {
                if (item.userId != null) { 
                    user = serviceUser.getById(item.userId);
                    //var groupesassociees = servicegroupemp.getGroupeByIDEmployee(item.Id);
                    //var groupesassociees_tests = groupesassociees.Select(o => o.nom).Distinct().ToList();
                   
                    //    foreach (var test in groupesassociees_tests)
                    //    {
                    //       groupesassocies.Add(new SelectListItem { Text = test, Value = test });
                    //    }
                    
                    fVM.Add(
                      new EmployeeViewModel
                      {
                          Id = item.Id,
                          userName = item.UserName,
                          pseudoName = item.pseudoName,
                          userLogin = item.userLogin,
                          Activite=item.Activite,
                          IdHermes=item.IdHermes,
                          role=item.role
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
                      userLogin ="",
                      Activite = item.Activite,
                      IdHermes = item.IdHermes,
                      role = item.role
                      //groupesassocies = groupesassocies


                  });
                }
                //groupesassocies.Clear();


            }
           
            if (!String.IsNullOrEmpty(search))
            {

                fVM = fVM.Where(p => p.userName.ToLower().Contains(search.ToLower())).ToList<EmployeeViewModel>();

            }
            //if (value == null)
            //{
            //    ViewBag.message = ("session cleared!");
            //    ViewBag.color = "red";
            //    return View("~/Views/Authentification/Index.cshtml");
            //}
            //else
            //{
                return View(fVM);   //fVM.Take(10)
            //}
        }
      
        [HttpGet]
        [AllowAnonymous]

        public ActionResult RefreshTable(String search, FormCollection form, int? CallsToMake)
        {
            var employees = service.GetAll();
            List<Employee> fVM = new List<Employee>();
            //string type = form["test"].ToString();
            //int numVal = Int32.Parse(type);
            foreach (var item in employees)
            {
                fVM.Add(item);
            }
            if (!String.IsNullOrEmpty(search))
            {

                fVM = fVM.Where(p => p.UserName.ToLower().Contains(search.ToLower())).ToList<Employee>();


            }

            return View(fVM);
        }
        [Authorize(Roles = "Admin")]

        // GET: Employee/Details/5
        public ActionResult Details(int? id)
        {
            List<Groupe> gremp = servicegroupemp.getGroupeByIDEmployee(id);
            var tests = gremp.Select(o => o.nom).Distinct().ToList();
            List<EmployeeViewModel> fVM = new List<EmployeeViewModel>();
            List<SelectListItem> groupes = new List<SelectListItem>();
            Utilisateur user = new Utilisateur();
            Employee item = service.getById(id);
            var a = new EmployeeViewModel();
            a.Id = item.Id;
            a.userName = item.UserName;
            a.pseudoName = item.pseudoName;
           // a.IdAD = (int)item.userId;
            a.IdHermes = item.IdHermes;
            a.Activite = item.Activite;
            a.role = item.role;
            user = serviceUser.getById(item.userId);
            a.userLogin = item.userLogin;
            foreach (var test in tests)
            {
                groupes.Add(new SelectListItem { Text = test, Value = test });
            }
            a.groupes = groupes;
            if (item.Content != null)
            {
                String strbase64 = Convert.ToBase64String(item.Content);
                String Url = "data:" + item.ContentType + ";base64," + strbase64;

                ViewBag.url = Url;
            }
            //  a.PhotoUrl = airflight.PhotoUrl;
            if (item == null)
                return HttpNotFound();
            return View(a);
        }
        [Authorize(Roles = "Admin")]

        // GET: Employee/Create
       
        public ActionResult Create()
        {
            var employee = new EmployeeViewModel();
            var logins = serviceUser.GetAll();
            var tests= logins.Select(o => o.login).Distinct().ToList();
            foreach (var test in tests)
            {
                employee.utilisateurs.Add(new SelectListItem { Text = test, Value = test });
            }
            var groupes = serviceGroupe.GetAll();
            foreach (var test in groupes)
            {
                employee.groupes.Add(new SelectListItem { Text = test.nom, Value = test.nom });
            }
            return View(employee);
        }

        // POST: Medcin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult> Create(EmployeeViewModel item, FormCollection form, String utilisateur, String groupes, HttpPostedFileBase file)
        {

            //string type = form["typeGenerator"].ToString();
            //var path = Path.GetFullPath(file.FileName);
           
            var usr = serviceUser.getBylogin(utilisateur);

        
                if (ModelState.IsValid)
                {
                    var emp = new Employee
                    {
                        UserName = item.userName,
                        //userId = usr.Id,
                        //userLogin = usr.login,
                        role = item.role,
                        pseudoName = item.pseudoName,
                        Email = item.Email,
                        //IdAD = "" + usr.Id,
                        Activite = item.Activite,
                        IdHermes = item.IdHermes
                    };

                    var result = await UserManager.CreateAsync(emp, item.Password);
                    if (result.Succeeded)
                    {
                        //await SignInManager.SignInAsync(emp, isPersistent: false, rememberBrowser: false);

                        return RedirectToAction("Index", "Home");
                    }
                 }
                    // If we got this far, something failed, redisplay form
                    //AddErrors(result);

                    //service.Add(emp);
                    //service.SaveChange();

                    //        var role = new CustomRole();
                    //        role.Name = "Admin";
                    //    context.Roles.Add(role);


                    //var result =  UserManager.Create(emp, item.pseudoName);

                    //   if (result.Succeeded)
                    //    {
                    //        var result1 = UserManager.AddToRole(emp.Id, "Admin");


                    //        return RedirectToAction("Index");
                    //    }
                    //Employee emp = new Employee
                    //{
                    //    pseudoName = item.pseudoName,
                    //    userId = usr.Id,
                    //    Activite = item.Activite,
                    //    IdHermes = item.IdHermes,
                    //    role = item.role,
                    //    userLogin=item.userLogin,
                    //    ContentType = type,
                    //    Content = bytes,
                    //    EmailConfirmed=false,
                    //    PhoneNumberConfirmed=false,
                    //    TwoFactorEnabled=true,
                    //    LockoutEnabled=false,
                    //    AccessFailedCount=1


                    //};

                
            
         
        
            return RedirectToAction("Create");

        }

        // GET: Employee/Edit/5
        [Authorize(Roles = "Admin")]

        public ActionResult Edit(int? id)
                {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Employee item = service.getById(id);
            //User user = serviceUser.getBylogin("OTOUNSI");
            Idtest = item.Id;
            var a = new EmployeeViewModel();
            a.Id = item.Id;
            a.userName = item.UserName;
            a.pseudoName = item.pseudoName;
            //a.IdAD = (int)item.userId;
            a.IdHermes = item.IdHermes;
            a.Activite = item.Activite;
            a.role = item.role;
            //string type = form["typeGenerator"].ToString();
            var logins = serviceUser.GetAll();
            var tests = logins.Select(o => o.login).Distinct().ToList();
            foreach (var test in tests)
            {
                a.utilisateurs.Add(new SelectListItem { Text=test,Value=test});
            }
            var groupes = serviceGroupe.GetAll();
            foreach (var test in groupes)
            {

                a.groupes.Add(new SelectListItem { Text = test.nom, Value = test.nom });
                //a.GroupeTests.Add(test);
            }
            var groupesassociees = servicegroupemp.getGroupeByIDEmployee(item.Id);
            var groupesassociees_tests = groupesassociees.Select(o => o.nom).Distinct().ToList();
            foreach (var test in groupesassociees_tests)
            {
                a.groupesassocies.Add(new SelectListItem { Text = test, Value = test });
            }
            var roles = context.Roles.ToList();
            foreach(var role in roles)
            {

                a.roles.Add(new SelectListItem { Text = role.Name, Value = role.Name });
            }
            //  a.PhotoUrl = airflight.PhotoUrl;
            if (item == null)
                return HttpNotFound();
            return View(a);
        

        }
        [Authorize(Roles = "Admin")]

        public ActionResult EditTest(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Employee item = service.getById(id);
            Idtest = item.Id;
            var a = new EmployeeViewModel();
            a.Id = item.Id;
            a.userName = item.UserName;
            a.pseudoName = item.pseudoName;
            //a.IdAD = (int)item.userId;
            a.IdHermes = item.IdHermes;
            a.Activite = item.Activite;
            a.role = item.role;
            a.Email = item.Email;
            
            //string type = form["typeGenerator"].ToString();
            var logins = serviceUser.GetAll();
            var tests = logins.Select(o => o.login).Distinct().ToList();
            foreach (var test in tests)
            {
                a.utilisateurs.Add(new SelectListItem { Text = test, Value = test });
            }
            var groupes = serviceGroupe.GetAll();
            var groupesassociees = servicegroupemp.getGroupeByIDEmployee(item.Id);
            var groupesassociees_tests = groupesassociees.Select(o => o.nom).Distinct().ToList();
            foreach (var test in groupes)
            {
                //foreach(var assoc in groupesassociees){
                //    if (!(test.nom).Equals(assoc.nom)){
                        a.groupes.Add(new SelectListItem { Text = test.nom, Value = test.nom });
                    //}
                }
                //a.GroupeTests.Add(test);
            
            
            foreach (var test in groupesassociees_tests)
            {
                a.groupesassocies.Add(new SelectListItem { Text = test, Value = test });
            }
            //  a.PhotoUrl = airflight.PhotoUrl;
            if (item == null)
                return HttpNotFound();
            return View(a);


        }
        // POST: Medcin/Edit/5
        [HttpPost, ActionName("Edit")]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult> EditEmployee(int? id, String utilisateur, String role, FormCollection form, HttpPostedFileBase file)
        {
            List<string> objs = new List<string>();
            //objs = tests.Select(p => p.ToString()).ToList();

            //string combindedString = string.Join(",", objs.ToArray());

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var employee = service.getById(id);
            var rolesForUser = await UserManager.GetRolesAsync(employee.Id);
            if (role != null)
            {
                using (var transaction = context.Database.BeginTransaction())
            {


                if (rolesForUser.Count() > 0)
                {
                    foreach (var item in rolesForUser.ToList())
                    {
                        // item should be the name of the role
                        var result = await UserManager.RemoveFromRoleAsync(employee.Id, item);
                    }
                }

            }
           
                UserManager.AddToRole(employee.Id, role);
            }

                if (utilisateur == null)
                {
                    var usr = serviceUser.getById(employee.userId);
                    employee.userId = usr.Id;
                    if (ps != null)
                    {

                        foreach (var value in ps)
                        {

                            Groupe groupe = serviceGroupe.getByNom((value));

                            GroupesEmployees gremp = new GroupesEmployees();
                            gremp.employeeId = employee.Id;
                            gremp.groupeId = groupe.Id;
                            servicegroupemp.Add(gremp);
                            servicegroupemp.SaveChange();


                        }
                    }
                }

                else if (!utilisateur.Equals(""))
                {
                    var usr = serviceUser.getBylogin(utilisateur);
                    employee.userId = usr.Id;
                    employee.userLogin = usr.login;
                    if (ps != null)
                    {

                        foreach (var value in ps)
                        {

                            Groupe groupe = serviceGroupe.getByNom((value));

                            GroupesEmployees gremp = new GroupesEmployees();
                            gremp.employeeId = employee.Id;
                            gremp.groupeId = groupe.Id;
                            servicegroupemp.Add(gremp);
                            servicegroupemp.SaveChange();


                        }
                    }
                }
                //string type = form["typeGenerator"].ToString();
                //var path = Path.GetFullPath(file.FileName);
                //var fileName = Path.GetFileName(file.FileName);
                //string contenttype = String.Empty;
                //FileStream fs1 = new FileStream("C:\\images\\" + fileName, FileMode.Open, FileAccess.Read);
                //BinaryReader br = new BinaryReader(fs1);
                //Byte[] bytes = br.ReadBytes((Int32)fs1.Length);
                //employee.ContentType = type;
                //employee.Content = bytes;
                //Groupe groupe = serviceGroupe.getByNom(groupes);
                //ReportContext contextReport = new ReportContext();
                //var employee = contextReport.employees.Find(id);
                //var usr = uow.UserRepository.getByLogin(utilisateur);
                //var group = contextReport.groupes.Find(groupe.Id);

                //employee.Group.Add(group);
                //employee.Group.Add(group);


                service.SaveChange();
                return RedirectToAction("Details", "Employee", new { @id = id });


            }
        [HttpPost, ActionName("EditTest")]
        [Authorize(Roles = "Admin")]

        public ActionResult EditEmployeeTest(EmployeeViewModel test,int? id, String utilisateur, FormCollection form, HttpPostedFileBase file)
        {
            List<string> objs = new List<string>();
            //objs = tests.Select(p => p.ToString()).ToList();

            //string combindedString = string.Join(",", objs.ToArray());

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var employee = service.getById(id);
            
            string type = form["typeGenerator"].ToString();
            //var path = Path.GetFullPath(file.FileName);
            if (file != null)
            {
                var fileName = Path.GetFileName(file.FileName);
                string contenttype = String.Empty;
                FileStream fs1 = new FileStream("C:\\images\\" + fileName, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs1);
                Byte[] bytes = br.ReadBytes((Int32)fs1.Length);
                employee.ContentType = type;
                employee.Content = bytes;
                employee.role = test.role;
                employee.pseudoName = test.pseudoName;
                employee.IdHermes = test.IdHermes;
                employee.Email = test.Email;
            }
            else
            {
               
                employee.role = test.role;
                employee.pseudoName = test.pseudoName;
                employee.IdHermes = test.IdHermes;
                employee.Email = test.Email;

            }
            //Groupe groupe = serviceGroupe.getByNom(groupes);
            //ReportContext contextReport = new ReportContext();
            //var employee = contextReport.employees.Find(id);
            //var usr = uow.UserRepository.getByLogin(utilisateur);
            //var group = contextReport.groupes.Find(groupe.Id);

            //employee.Group.Add(group);
            //employee.Group.Add(group);

            service.SaveChange();
                    return RedirectToAction("Details", "Employee", new { @id = id });
                
            }

        
        static string ConvertStringArrayToString(string[] array)
        {
            // Concatenate all the elements into a StringBuilder.
            StringBuilder builder = new StringBuilder();
            foreach (string value in array)
            {
                builder.Append(value);
                //builder.Append('.');
            }
            return builder.ToString();
        }

        // GET: Employee/Delete/5
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee emp = new Employee();
            emp = service.getById(id);
            var user = await UserManager.FindByIdAsync(emp.Id);

            //List Logins associated with user
            var logins = user.Logins;

            //Gets list of Roles associated with current user
            var rolesForUser = await UserManager.GetRolesAsync(emp.Id);

            using (var transaction = context.Database.BeginTransaction())
            {
                foreach (var login in logins.ToList())
                {
                    await UserManager.RemoveLoginAsync(login.UserId, new UserLoginInfo(login.LoginProvider, login.ProviderKey));
                }

                if (rolesForUser.Count() > 0)
                {
                    foreach (var item in rolesForUser.ToList())
                    {
                        // item should be the name of the role
                        var result = await UserManager.RemoveFromRoleAsync(user.Id, item);
                    }
                }

                //Delete User
                serviceEvent.RemoveEventsOfEmployee(user.Id);
                servicegroupeEmployee.DeletebyIdEmployee(user.Id);
                serviceAlerte.RemoveRecivedAlerteOfEmployee(user.Id);
                serviceEvaluation.DeleteEvaluations(user.Id);
                await UserManager.DeleteAsync(user);

                //Employee emp = new Employee();
                //emp = service.getById(id);
                //EmployeeViewModel a = new EmployeeViewModel();
                //a.Id = emp.Id;
                //service.Delete(emp);

                //service.SaveChange();
                //if (emp == null)
                //    return HttpNotFound();
                return RedirectToAction("Index");
            }
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
        [Authorize(Roles = "Admin")]

        public ActionResult FindEmployee(int? Id)
        {
            Employee item = service.getById(Id);


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
                return PartialView("_AlerteDeSuppression", a);
            }

            else
            {
                return View(a);
            }
        }
        [Authorize(Roles = "Admin")]

        public ActionResult AffectationUserError()
        {
            EmployeeViewModel a = new EmployeeViewModel();
            return PartialView("_AffectationUserError",a);
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]

        public ActionResult FindGroupesAssociees(int Id, String utilisateurs,string roles, String[] groupess, String str)
        {
           
            Employee item = service.getById(Id);
            Utilisateur usr = new Utilisateur() ;
            if (!utilisateurs.Equals(""))
            {
                 usr = serviceUser.getBylogin(utilisateurs);
                item.userId = usr.Id;

            }
            var a = new EmployeeViewModel();
            a.Id = item.Id;
            a.userName = item.UserName;
            a.pseudoName = item.pseudoName;
            a.IdAD = (int)item.userId;
            a.IdHermes = item.IdHermes;
            a.Activite = item.Activite;
            a.role = item.role;
            a.userLogin=usr.login;
            ps = groupess;
            if (groupess!= null)
            {
                foreach (var test in groupess)
                {
                    //String test1 = Json(test.VA).ToString();

                    a.groupesassocies.Add(new SelectListItem { Text = test, Value = test });
                    a.tests.Add(test);
                }
                ViewBag.tests = a.groupesassocies;
            }
            a.role=roles;
            if (Request.IsAjaxRequest())
            {
                return PartialView("_AlerteAjoutsGroupes", a);
            }
            else
            {
                return View(a);
            }
        }
        //public ActionResult FindUserAssociees(String utilisateurs)
        //{

        //    return RedirectToAction("Create", new {utilisateur = utilisateurs}, FormMethod.Post);    
        //}
        [HttpGet]
        [Authorize(Roles = "Admin")]

        public ActionResult FindAffectationEmployee(int? Id)
        {
            Employee item = service.getById(Id);


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
                return PartialView("_AffectationUserError", a);
            }

            else
            {
                return View(a);
            }
        }

        //[HttpGet]
        //public ActionResult FindGroupeEmployee(int? Id)
        //{
        //    GroupesEmployees a = new GroupesEmployees();
        //    var groupesassociees = servicegroupemp.getByIDEmployee(Id);
        //    a.employeeId = groupesassociees[1].employeeId;
        //    a.groupeId = groupesassociees[1].groupeId;
        //    if (Request.IsAjaxRequest())
        //    {
        //        return PartialView("_AlerteDeSuppressionGroupe", a);
        //    }

        //    else
        //    {
        //        return View(a);
        //    }
        //}
        //[HttpPost]
        //public ActionResult DeleteGrooupeEmployee()
        //{

        //    return View();
        //}
        [Authorize(Roles = "Admin")]

        public ActionResult deleteGrooupeEmployee(int? id,EmployeeViewModel model,String nom)
        {
            //String groupeName = TempData["name"].ToString();
            servicegroupemp.deletegroupeEmployeeByName(Idtest, nom);
            servicegroupemp.SaveChange();
            //TempData["name"] = null;
            return RedirectToAction("Edit","Employee",new { @id = Idtest });
        }

        //public ActionResult deleteGrooupeEmployee(int id, FormCollection collection)
        //{
        //    try
        //    {
        //    TODO: Add delete logic here

        //        return RedirectToAction("Edit");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
        #endregion

        #region QualiteManipulationEmployee
        [HttpGet]
        [Authorize(Roles = "Qualité")]
        public ActionResult IndexQualite(String search, FormCollection form, int? CallsToMake)
        {
            var connectedEmp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            ViewBag.userName = connectedEmp.UserName;
            ViewBag.pseudoNameEmp = connectedEmp.pseudoName;
            if (connectedEmp.Content != null)
            {
                String strbase64 = Convert.ToBase64String(connectedEmp.Content);
                String Url = "data:" + connectedEmp.ContentType + ";base64," + strbase64;
                ViewBag.url = Url;
               

            }

            //string value = (string)Session["loginIndex"];
            var employees = service.GetAll();
            Utilisateur user = new Utilisateur();
            List<EmployeeViewModel> fVM = new List<EmployeeViewModel>();
           

            foreach (var item in employees)
            {
                List<SelectListItem> groupesassocies = new List<SelectListItem>();
                if (item.userId != null)
                {
                    user = serviceUser.getById(item.userId);
                    var groupesassociees = servicegroupemp.getGroupeByIDEmployee(item.Id);
                    var groupesassociees_tests = groupesassociees.Select(o => o.nom).Distinct().ToList();

                    foreach (var test in groupesassociees_tests)
                    {
                        groupesassocies.Add(new SelectListItem { Text = test, Value = test });
                    }

                    fVM.Add(
                      new EmployeeViewModel
                      {
                          Id = item.Id,
                          userName = item.UserName,
                          pseudoName = item.pseudoName,
                          userLogin = item.userLogin,
                          Activite = item.Activite,
                          IdHermes = item.IdHermes,
                          role = item.role,
                          groupes = groupesassocies



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
                      role = item.role,
                      groupes = groupesassocies


                  });
                }
              //  groupesassocies.Clear();


            }

            if (!String.IsNullOrEmpty(search))
            {

                fVM = fVM.Where(p => p.userName.ToLower().Contains(search.ToLower())).ToList<EmployeeViewModel>();

            }   
            return View(fVM);   //fVM.Take(10)
           
        }
        #endregion

        #region AgentQualiteEmployee
        [HttpGet]
        [Authorize(Roles = "Agent Qualité,Agent Qualité_CustomerService,Agent Qualité_Diffusion,Agent Qualité_AchatPublic,Agent Qualité_PRV")]
        public ActionResult IndexAgentQualite(String search, FormCollection form, int? CallsToMake)
        {
            var connectedEmp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (connectedEmp.Roles.Any(b => b.UserId == connectedEmp.Id && b.RoleId == 8))
            {
                ViewBag.role = "Agent Qualité_CustomerService";
            }
            if (connectedEmp.Roles.Any(b => b.UserId == connectedEmp.Id && b.RoleId == 9))
            {
                ViewBag.role = "Agent Qualité_Diffusion";
            }
            if (connectedEmp.Roles.Any(b => b.UserId == connectedEmp.Id && b.RoleId == 5))
            {
                ViewBag.role = "Agent Qualité";
            }
            if (connectedEmp.Roles.Any(b => b.UserId == connectedEmp.Id && b.RoleId == 2009))
            {
                ViewBag.role = "Agent Qualité_AchatPublic";
            }
            if (connectedEmp.Roles.Any(b => b.UserId == connectedEmp.Id && b.RoleId == 2014))
            {
                ViewBag.role = "Agent Qualité_PRV";
            }
            ViewBag.userName = connectedEmp.UserName;
            ViewBag.pseudoNameEmp = connectedEmp.pseudoName;
            if (connectedEmp.Content != null)
            {
                String strbase64 = Convert.ToBase64String(connectedEmp.Content);
                String Url = "data:" + connectedEmp.ContentType + ";base64," + strbase64;
                ViewBag.url = Url;
            }
            var groupes = servicegroupemp.getGroupeByIDEmployee(connectedEmp.Id);
            List<Employee> employees = new List<Employee>();
            List<Employee> Allemployees = new List<Employee>();
            foreach (var g in groupes) {
                var empgroupes = servicegroupeEmployee.getListEmployeeByGroupeId(g.Id);
                foreach(var eg in empgroupes)
                {
                    if(!(employees.Exists(x => x.Id == eg.Id))){

                        employees.Add(eg);

                    }
                }

            }
   
            Utilisateur user = new Utilisateur();
            List<EmployeeViewModel> fVM = new List<EmployeeViewModel>();


            foreach (var item in employees)
            {
                List<SelectListItem> groupesassocies = new List<SelectListItem>();
                if (item.userId != null)
                {
                    user = serviceUser.getById(item.userId);
                    var groupesassociees = servicegroupemp.getGroupeByIDEmployee(item.Id);
                    var groupesassociees_tests = groupesassociees.Select(o => o.nom).Distinct().ToList();

                    foreach (var test in groupesassociees_tests)
                    {
                        groupesassocies.Add(new SelectListItem { Text = test, Value = test });
                    }

                    fVM.Add(
                      new EmployeeViewModel
                      {
                          Id = item.Id,
                          userName = item.UserName,
                          pseudoName = item.pseudoName,
                          userLogin = item.userLogin,
                          Activite = item.Activite,
                          IdHermes = item.IdHermes,
                          role = item.role,
                          groupes = groupesassocies



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
                      role = item.role,
                      groupes = groupesassocies


                  });
                }
            }

            if (!String.IsNullOrEmpty(search))
            {

                fVM = fVM.Where(p => p.userName.ToLower().Contains(search.ToLower())).ToList<EmployeeViewModel>();

            }
            return View(fVM);   //fVM.Take(10)

        }
        #endregion
    }
}
