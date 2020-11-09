using Domain.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using MVCWEB;
using MVCWEB.Models;

using Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace RecordsMVC.Controllers
{
    [Authorize(Roles = "Qualité, Agent Qualité,  Agent Qualité_CustomerService,Agent Qualité_Diffusion,Agent Qualité_AchatPublic,Agent Qualité_PRV")]
    public class SuperviseurController : Controller
    {
        // GET: Superviseur
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        IUtilisateurService serviceUser;
        IEmployeeService serviceEmployee;
        IGroupeEmployeeService serviceGroupeEmp;
        IGroupeService serviceGroupe;
        //static IWavePlayer playerTest;
        DirectoryViewModel directoryModel;
        //TemplateModel baseModel;
        UserConnectedPropreties test;


        public SuperviseurController()
        {
            serviceUser = new UtilisateurService();
            serviceEmployee = new EmployeeService();
            serviceGroupeEmp = new GroupesEmployeService();
            directoryModel = new DirectoryViewModel();
            serviceGroupe = new GroupeService();
            //baseModel = new TemplateModel();
            test = new UserConnectedPropreties();

        }
        public SuperviseurController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
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
       
        public ActionResult ListeSites()
        {
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 5))
            {
                ViewBag.role = "Agent Qualité";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 4))
            {
                ViewBag.role = "Qualité";
            }
            var a = new DirectoryViewModel();
            var listDirectory = new DirectoryInfo(@"\\10.9.6.25\Enregistrements\");
            var files = listDirectory.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(y => y.CreationTime.Year == 2017);
            
            foreach (var file in files)
            {
                a.listeSites.Add(new SelectListItem { Value = "V4", Text = file.Name });
            }

            var listDirectoryV5 = new DirectoryInfo(@"\\10.9.6.25\Enregistrements\hermes_p\Files\");
           
            List<SelectListItem> filesV5 = new List<SelectListItem>();
            filesV5.Add(new SelectListItem() { Value = "5586770313D415B4", Text = "CUSTOMER SERVICE" });
            filesV5.Add(new SelectListItem() { Value = "5526B414C43414B4", Text = "ABONNEMENTS" });
            filesV5.Add(new SelectListItem() { Value = "55864774662494B4", Text = "NEW_DIGITAL" });
            filesV5.Add(new SelectListItem() { Value = "55053725143443B4", Text = "INFOPRO DATA" });
            filesV5.Add(new SelectListItem() { Value = "558667233464F6B4", Text = "INTERNATIONAL" });

            foreach (var file in filesV5)
            {
                a.listeSitesV5.Add(new SelectListItem { Value = file.Value, Text = file.Text });
            }
            test.affectAttributes(a, user.Id, user.UserName, user.pseudoName);
            if (user.Content != null)
            {
                String Url = test.renderImage(user.Content, user.ContentType);
                ViewBag.url = Url;
                a.Url = Url;

            }
            return View(a);
        }

        public ActionResult OptionRecherche(string Id, string Version)
        {
            Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (emp.Roles.Any(b => b.UserId == emp.Id && b.RoleId == 5))
            {
                ViewBag.role = "Agent Qualité";
            }
            if (emp.Roles.Any(b => b.UserId == emp.Id && b.RoleId == 4))
            {
                ViewBag.role = "Qualité";
            }
            if (emp.Roles.Any(b => b.UserId == emp.Id && b.RoleId == 8))
            {
                ViewBag.role = "Agent Qualité_CustomerService";
            }
            if (emp.Roles.Any(b => b.UserId == emp.Id && b.RoleId == 9))
            {
                ViewBag.role = "Agent Qualité_Diffusion";
            }
            if (emp.Roles.Any(b => b.UserId == emp.Id && b.RoleId == 2009))
            {
                ViewBag.role = "Agent Qualité_AchatPublic";
            }
            if (emp.Roles.Any(b => b.UserId == emp.Id && b.RoleId == 2014))
            {
                ViewBag.role = "Agent Qualité_PRV";
            }
            DirectoryViewModel a = new DirectoryViewModel();
            a.roleName = Id;
            a.roleKey = Version;
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            test.affectAttributes(a, user.Id, user.UserName, user.pseudoName);
            if (user.Content != null)
            {
                String Url = test.renderImage(user.Content, user.ContentType);
                ViewBag.url = Url;
                a.Url = Url;

            }
            return View(a);

        }
       
        // GET: Superviseur/Details/5
        public ActionResult Index(string Id, string Version)
        {
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));//get the current user
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 5))
            {
                ViewBag.role = "Agent Qualité";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 4))
            {
                ViewBag.role = "Qualité";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 8))
            {
                ViewBag.role = "Agent Qualité_CustomerService";
            }

            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 9))
            {
                ViewBag.role = "Agent Qualité_Diffusion";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 2009))
            {
                ViewBag.role = "Agent Qualité_AchatPublic";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 2014))
            {
                ViewBag.role = "Agent Qualité_PRV";
            }
            /* var roles = serviceEmployee.getListRoles(user).FirstOrDefault();*///get it roles
            directoryModel.roleName = Id;
            directoryModel.roleKey = Version;
            var tests = new DirectoryInfo(@"\\10.9.6.25\Enregistrements\" + Id);//point into the right records directory using the role of current user
            List<Employee> logins = new List<Employee>();
            if (Id == "DIFFUSION" && Version == "V4")
            {
                List<string> listdiffusiongr = new List<string>(new string[] { "GISI-REAB", "GISI-PROMO", "GMT-REAB", "GMT-PROMO" });
                foreach (var g in listdiffusiongr)
                {
                    Groupe grr = serviceGroupe.getByNom(g);

                    var loginsdiffusion = serviceGroupeEmp.getListEmployeeByGroupeId(grr.Id);
                    logins.AddRange(loginsdiffusion);
                }
            }
            else if (Id != "DIFFUSION" && Version == "V4")
            {
                logins = serviceGroupe.getListEmployeeBySelectedSite(Id);
            }
            else
            {
                tests = new DirectoryInfo(@"\\10.9.6.25\Enregistrements\hermes_p\Files\" + Id + "\\RECORDS");
                logins = serviceGroupe.getListEmployeeBySelectedSiteV5(Id);
            }
            
            var files = tests.EnumerateDirectories(
           "*.*",
            SearchOption.TopDirectoryOnly); //search criteria for example All Directorys
            var ordredListCompagnes = files.OrderBy(m => m.Name).ToList();//order the list of campagnes alphabitacly by the field name.
            foreach (var file in ordredListCompagnes)
            {
                directoryModel.topDirectorieDirectories.Add(new SelectListItem { Text = file.Name, Value = file.Name });

            }
            //List<Employee> logins = new List<Employee>();
            //if (Id == "DIFFUSION")
            //{
            //    List<string> listdiffusiongr = new List<string>(new string[] { "GISI-REAB", "GISI-PROMO", "GMT-REAB", "GMT-PROMO" });
            //    foreach (var g in listdiffusiongr)
            //    {
            //        Groupe grr = serviceGroupe.getByNom(g);

            //        var loginsdiffusion = serviceGroupeEmp.getListEmployeeByGroupeId(grr.Id);
            //        logins.AddRange(loginsdiffusion);
            //    }
            //}
            //else
            //{
            //    Groupe gr = serviceGroupe.getByNom(Id);
            //    logins = serviceGroupeEmp.getListEmployeeByGroupeId(gr.Id);
            //}
            var us = logins.Select(o => o).Distinct().ToList();
            var ordredNames = us.OrderBy(m => m.pseudoName).ToList();//order alphabitacly the list of the employees field of choice is pseudoName
            
            foreach (var t in ordredNames)
            {
                if ((t.UserName != user.UserName) && (t.Roles.Any(r => r.UserId == t.Id && r.RoleId == 3 || r.RoleId == 10 || r.RoleId == 1009 || r.RoleId == 1010 || r.RoleId == 1011 || r.RoleId == 2010 || r.RoleId == 2013)))
                {
                    directoryModel.utilisateursPseudonames.Add(new SelectListItem { Text = t.pseudoName, Value = t.pseudoName });
                }
            }

            test.affectAttributes(directoryModel, user.Id, user.UserName, user.pseudoName);
            if (user.Content != null)
            {
                String Url = test.renderImage(user.Content, user.ContentType);
                ViewBag.url = Url;
                directoryModel.Url = Url;

            }
            return View(directoryModel);
        }


        public ActionResult AllCases(string Id, string Version)
        {

            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));//get the current user
                              
           //var roles = serviceEmployee.getListRoles(user).FirstOrDefault();//get it roles
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 5))
            {
                ViewBag.role = "Agent Qualité";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 4))
            {
                ViewBag.role = "Qualité";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 8))
            {
                ViewBag.role = "Agent Qualité_CustomerService";
            }

            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 9))
            {
                ViewBag.role = "Agent Qualité_Diffusion";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 2009))
            {
                ViewBag.role = "Agent Qualité_AchatPublic";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 2014))
            {
                ViewBag.role = "Agent Qualité_PRV";
            }
            var tests = new DirectoryInfo(@"\\10.9.6.25\Enregistrements\" + Id);//point into the right records directory using the role of current user
           
            List<Employee> logins = new List<Employee>();
            if (Id == "DIFFUSION" && Version == "V4")
            {
                List<string> listdiffusiongr = new List<string>(new string[] { "GISI-REAB", "GISI-PROMO", "GMT-REAB", "GMT-PROMO" });
                foreach (var g in listdiffusiongr)
                {
                    Groupe grr = serviceGroupe.getByNom(g);

                    var loginsdiffusion = serviceGroupeEmp.getListEmployeeByGroupeId(grr.Id);
                    logins.AddRange(loginsdiffusion);
                }
            }
           else if (Id != "DIFFUSION" && Version == "V4")
            {
                logins = serviceGroupe.getListEmployeeBySelectedSite(Id);
            }
            else
            {
                tests = new DirectoryInfo(@"\\10.9.6.25\Enregistrements\hermes_p\Files\" + Id + "\\RECORDS");
                logins = serviceGroupe.getListEmployeeBySelectedSiteV5(Id);
            }
            var files = tests.EnumerateDirectories(
          "*.*",
           SearchOption.TopDirectoryOnly); //search criteria for example All Directorys
            var ordredListCompagnes = files.OrderBy(m => m.Name).ToList();//order the list of campagnes alphabitacly by the field name.
            foreach (var file in ordredListCompagnes)
            {
                directoryModel.topDirectorieDirectories.Add(new SelectListItem { Text = file.Name, Value = file.Name });
            }

            var us = logins.Select(o => o).Distinct().ToList();
            var ordredpseudoNames = us.OrderBy(a => a.pseudoName).ToList();
            int i = 0;
            while (i < ordredpseudoNames.Count)
            {
                if (!ordredpseudoNames[i].UserName.Equals(user.UserName) && (ordredpseudoNames[i].Roles.Any(r => r.UserId == ordredpseudoNames[i].Id && r.RoleId == 3 || r.RoleId == 10 || r.RoleId == 1009 || r.RoleId == 1010 || r.RoleId == 1011 || r.RoleId == 2010 || r.RoleId == 2013)))
                {
                    directoryModel.utilisateurs.Add(new SelectListItem { Text = ordredpseudoNames[i].UserName, Value = ordredpseudoNames[i].UserName });
                    directoryModel.utilisateursPseudonames.Add(new SelectListItem { Text = ordredpseudoNames[i].pseudoName, Value = ordredpseudoNames[i].pseudoName });
                    directoryModel.IdHermeses.Add(new SelectListItem { Text = "" + ordredpseudoNames[i].IdHermes, Value = "" + ordredpseudoNames[i].IdHermes });
                }
                i++;
            }
            test.affectAttributes(directoryModel, user.Id, user.UserName, user.pseudoName);
            if (user.Content != null)
            {
                String Url = test.renderImage(user.Content, user.ContentType);
                ViewBag.url = Url;
                directoryModel.Url = Url;
            }
            directoryModel.roleName = Id;
            directoryModel.roleKey = Version;
            return View(directoryModel);
        }
        public ActionResult CompagneParDate(string Id, string Version)
        {
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            //var roles = serviceEmployee.getListRoles(user).FirstOrDefault();
           
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 5))
            {
                ViewBag.role = "Agent Qualité";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 4))
            {
                ViewBag.role = "Qualité";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 8))
            {
                ViewBag.role = "Agent Qualité_CustomerService";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 9))
            {
                ViewBag.role = "Agent Qualité_Diffusion";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 2009))
            {
                ViewBag.role = "Agent Qualité_AchatPublic";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 2014))
            {
                ViewBag.role = "Agent Qualité_PRV";
            }
            //directoryModel.roleName = roles.Name;

            var tests = new DirectoryInfo(@"\\10.9.6.25\Enregistrements\" + Id);
            if (Version != "V4")
            {
                tests = new DirectoryInfo(@"\\10.9.6.25\Enregistrements\hermes_p\Files\" + Id + "\\RECORDS");
            }
            directoryModel.roleName = Id;
            directoryModel.roleKey = Version;

            var files = tests.EnumerateDirectories(
           "*.*",
            SearchOption.TopDirectoryOnly); //search criteria for example All Directorys

            var ordredCampagnes = files.OrderBy(f => f.Name).ToList();
            foreach (var file in ordredCampagnes)
            {
                directoryModel.topDirectorieDirectories.Add(new SelectListItem { Text = file.Name, Value = file.Name });

            }
            test.affectAttributes(directoryModel, user.Id, user.UserName, user.pseudoName);
            if (user.Content != null)
            {
                String Url = test.renderImage(user.Content, user.ContentType);
                ViewBag.url = Url;
                directoryModel.Url = Url;

            }
            return View(directoryModel);
        }
        public ActionResult IndexMultipleAgent(string Id, string Version)
        {
            Employee ManagerConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            //var roles = serviceEmployee.getListRoles(ManagerConnected).FirstOrDefault();
            if (ManagerConnected.Roles.Any(b => b.UserId == ManagerConnected.Id && b.RoleId == 5))
            {
                ViewBag.role = "Agent Qualité";
            }
            if (ManagerConnected.Roles.Any(b => b.UserId == ManagerConnected.Id && b.RoleId == 4))
            {
                ViewBag.role = "Qualité";
            }
            if (ManagerConnected.Roles.Any(b => b.UserId == ManagerConnected.Id && b.RoleId == 8))
            {
                ViewBag.role = "Agent Qualité_CustomerService";
            }
            if (ManagerConnected.Roles.Any(b => b.UserId == ManagerConnected.Id && b.RoleId == 9))
            {
                ViewBag.role = "Agent Qualité_Diffusion";
            }
            if (ManagerConnected.Roles.Any(b => b.UserId == ManagerConnected.Id && b.RoleId == 2009))
            {
                ViewBag.role = "Agent Qualité_AchatPublic";
            }
            if (ManagerConnected.Roles.Any(b => b.UserId == ManagerConnected.Id && b.RoleId == 2014))
            {
                ViewBag.role = "Agent Qualité_PRV";
            }
            directoryModel.roleName = Id;
            directoryModel.roleKey = Version;
            var employees = serviceGroupeEmp.getListEmployeeByGroupe(ManagerConnected.Id);
            List<Employee> logins = new List<Employee>();
            if (Id == "DIFFUSION" && Version == "V4")
            {
                List<string> listdiffusiongr = new List<string>(new string[] { "GISI-REAB", "GISI-PROMO", "GMT-REAB", "GMT-PROMO" });
                foreach (var g in listdiffusiongr)
                {
                    Groupe grr = serviceGroupe.getByNom(g);

                    var loginsdiffusion = serviceGroupeEmp.getListEmployeeByGroupeId(grr.Id);
                    logins.AddRange(loginsdiffusion);
                }
            }
            else if (Id != "DIFFUSION" && Version == "V4")
            {
                logins = serviceGroupe.getListEmployeeBySelectedSite(Id);
            }
            else
            {

                logins = serviceGroupe.getListEmployeeBySelectedSiteV5(Id);
            }

            var us = logins.Select(o => o).Distinct().ToList();

            var ordredpseudoNames = us.OrderBy(a => a.pseudoName).ToList();

            int i = 0;
            while (i < ordredpseudoNames.Count)
            {
                if (!ordredpseudoNames[i].UserName.Equals(ManagerConnected.UserName) && (ordredpseudoNames[i].Roles.Any(r => r.UserId == ordredpseudoNames[i].Id && r.RoleId == 3 || r.RoleId == 10 || r.RoleId == 1009 || r.RoleId == 1010 || r.RoleId == 1011 || r.RoleId == 2010 || r.RoleId == 2013)))
                {
                    directoryModel.utilisateurs.Add(new SelectListItem { Text = ordredpseudoNames[i].UserName, Value = ordredpseudoNames[i].UserName });
                    directoryModel.utilisateursPseudonames.Add(new SelectListItem { Text = ordredpseudoNames[i].pseudoName, Value = ordredpseudoNames[i].pseudoName });
                }
                i++;

            }
            test.affectAttributes(directoryModel, ManagerConnected.Id, ManagerConnected.UserName, ManagerConnected.pseudoName);
            if (ManagerConnected.Content != null)
            {
                String Url = test.renderImage(ManagerConnected.Content, ManagerConnected.ContentType);
                ViewBag.url = Url;
                directoryModel.Url = Url;

            }
            return View(directoryModel);
        }


        [HttpPost]
        public ActionResult GetChildrenofDirectory(string children)
        {
            var tests = new DirectoryInfo(@children);
            var files = tests.EnumerateDirectories("*.*",
            SearchOption.TopDirectoryOnly); //search criteria for example All Directorys
            foreach (var file in files)
            {
                // Display file path.
                directoryModel.topDirectorieDirectories.Add(new SelectListItem { Text = file.Name, Value = file.Name });

                //Console.WriteLine(file);
            }

            return PartialView("GetChildren", directoryModel);


        }

        public ActionResult Index2()
        {
            List<String> tests = new List<String>();
            List<String> testsFiles = new List<String>();

            var files = Directory.EnumerateDirectories(@"\\10.9.6.25\Enregistrements\DIFFUSION",
           "*.*",
            SearchOption.TopDirectoryOnly); //search criteria for example All Directorys
            foreach (string file in files)
            {
                // Display file path.
                directoryModel.topDirectorieDirectories.Add(new SelectListItem { Text = file, Value = file });

                Console.WriteLine(file);
            }
           
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

            var logins = serviceGroupeEmp.getListEmployeeByGroupe(user.Id);
            var us = logins.Select(o => o).Distinct().ToList();
            var uspseudoNames = logins.Select(o => o.pseudoName).Distinct().ToList();
            foreach (var test in us)
            {
                directoryModel.utilisateurs.Add(new SelectListItem { Text = test.UserName, Value = test.UserName });
                directoryModel.utilisateursPseudonames.Add(new SelectListItem { Text = test.pseudoName, Value = test.pseudoName });
                directoryModel.IdHermeses.Add(new SelectListItem { Text = "" + test.IdHermes, Value = "" + test.IdHermes });
            }
            test.affectAttributes(directoryModel, user.Id, user.UserName, user.pseudoName);
            if (user.Content != null)
            {
                String Url = test.renderImage(user.Content, user.ContentType);
                ViewBag.url = Url;
                directoryModel.Url = Url;

            }
            return View(directoryModel);
           
        }
        [HttpGet]
        public ActionResult Diffusion(string Id, string Version)
        {
            List<String> tests = new List<String>();
            List<String> testsFiles = new List<String>();

            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            // var roles = serviceEmployee.getListRoles(user).FirstOrDefault();
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 5))
            {
                ViewBag.role = "Agent Qualité";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 4))
            {
                ViewBag.role = "Qualité";
            }

            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 8))
            {
                ViewBag.role = "Agent Qualité_CustomerService";
            }

            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 9))
            {
                ViewBag.role = "Agent Qualité_Diffusion";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 2009))
            {
                ViewBag.role = "Agent Qualité_AchatPublic";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 2014))
            {
                ViewBag.role = "Agent Qualité_PRV";
            }
            directoryModel.roleName = Id;
            directoryModel.roleKey = Version;
            List<Employee> logins = new List<Employee>();
            if (Id == "DIFFUSION" && Version == "V4")
            {
                List<string> listdiffusiongr = new List<string>(new string[] { "GISI-REAB", "GISI-PROMO", "GMT-REAB", "GMT-PROMO" });
                foreach (var g in listdiffusiongr)
                {
                    Groupe grr = serviceGroupe.getByNom(g);

                    var loginsdiffusion = serviceGroupeEmp.getListEmployeeByGroupeId(grr.Id);
                    logins.AddRange(loginsdiffusion);
                }
            }
           else if (Id != "DIFFUSION" && Version == "V4")
            {
                logins = serviceGroupe.getListEmployeeBySelectedSite(Id);
            }
            else
            {
               
                logins = serviceGroupe.getListEmployeeBySelectedSiteV5(Id);
            }
           
            var us = logins.Select(o => o).Distinct().ToList();
            
            var ordredpseudoNames = us.OrderBy(a => a.pseudoName).ToList();
           
            int i = 0;
            while (i < ordredpseudoNames.Count)
            {
                if (!ordredpseudoNames[i].UserName.Equals(user.UserName) && (ordredpseudoNames[i].Roles.Any(r => r.UserId == ordredpseudoNames[i].Id && r.RoleId == 3 || r.RoleId == 10 || r.RoleId == 1009 || r.RoleId == 1010 || r.RoleId == 1011 || r.RoleId == 2010 || r.RoleId == 2013)))
                {
                    //Employee emp = serviceEmployee.getById(ordredpseudoNames[i].Id);
                    //var rolemp = serviceEmployee.getListRoles(emp).FirstOrDefault();
                    //if ( rolemp.Name== Id)
                    //{
                        directoryModel.utilisateurs.Add(new SelectListItem { Text = ordredpseudoNames[i].UserName, Value = ordredpseudoNames[i].UserName });
                        directoryModel.utilisateursPseudonames.Add(new SelectListItem { Text = ordredpseudoNames[i].pseudoName, Value = ordredpseudoNames[i].pseudoName });
                    //}
                }
                i++;

            }
            test.affectAttributes(directoryModel, user.Id, user.UserName, user.pseudoName);
            if (user.Content != null)
            {
                String Url = test.renderImage(user.Content, user.ContentType);
                ViewBag.url = Url;
                directoryModel.Url = Url;
            }
            return View(directoryModel);          
        }
        public JsonResult GetIdHermes(string PseudoNameSelected)
        {
            List<SelectListItem> IdHermesesList = new List<SelectListItem>();
            var data = serviceEmployee.getByPseudoName(PseudoNameSelected);
            string a = data.IdHermes.ToString();
            string b = data.IdHermes.ToString();
            IdHermesesList.Add(new SelectListItem { Text = a, Value = b });
            return Json(new SelectList(IdHermesesList, "Value", "Text"));
        }
        [HttpGet]
        public ActionResult DiffusionManager()
        {
            List<String> tests = new List<String>();
            List<String> testsFiles = new List<String>();

            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

            var logins = serviceGroupeEmp.getListEmployeeByGroupe(user.Id);
            var us = logins.Select(o => o).Distinct().ToList();
            var uspseudoNames = logins.Select(o => o.pseudoName).Distinct().ToList();
            foreach (var test in us)
            {
                directoryModel.utilisateurs.Add(new SelectListItem { Text = test.UserName, Value = test.UserName });
                directoryModel.utilisateursPseudonames.Add(new SelectListItem { Text = test.pseudoName, Value = test.pseudoName });
                directoryModel.IdHermeses.Add(new SelectListItem { Text = "" + test.IdHermes, Value = "" + test.IdHermes });
            }
            test.affectAttributes(directoryModel, user.Id, user.UserName, user.pseudoName);

            if (user.Content != null)
            {
                String Url = test.renderImage(user.Content, user.ContentType);
                ViewBag.url = Url;
                directoryModel.Url = Url;

            }
            return View(directoryModel);

        }
        [HttpGet]
        public ActionResult RechercheIndiceTel(string Id)
        {
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            //var roles = serviceEmployee.getListRoles(user).ToList().FirstOrDefault();
            directoryModel.roleName = Id;
            test.affectAttributes(directoryModel, user.Id, user.UserName, user.pseudoName);
            if (user.Content != null)
            {
                String Url = test.renderImage(user.Content, user.ContentType);
                ViewBag.url = Url;
                directoryModel.Url = Url;

            }
            return View(directoryModel);
        }
        [HttpGet]
        public ActionResult RechercheNumTel(string Id, string Version)
        {
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 5))
            {
                ViewBag.role = "Agent Qualité";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 4))
            {
                ViewBag.role = "Qualité";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 8))
            {
                ViewBag.role = "Agent Qualité_CustomerService";
            }

            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 9))
            {
                ViewBag.role = "Agent Qualité_Diffusion";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 2009))
            {
                ViewBag.role = "Agent Qualité_AchatPublic";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 2014))
            {
                ViewBag.role = "Agent Qualité_PRV";
            }
            //var roles = serviceEmployee.getListRoles(user).ToList().FirstOrDefault();
            directoryModel.roleName = Id;
            directoryModel.roleKey = Version;
            test.affectAttributes(directoryModel, user.Id, user.UserName, user.pseudoName);
            if (user.Content != null)
            {
                String Url = test.renderImage(user.Content, user.ContentType);
                ViewBag.url = Url;
                directoryModel.Url = Url;

            }
            return View(directoryModel);
        }
        [HttpGet]
        public ActionResult RechercheParNumeroFiche(string Id, string Version)
        {
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 5))
            {
                ViewBag.role = "Agent Qualité";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 4))
            {
                ViewBag.role = "Qualité";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 8))
            {
                ViewBag.role = "Agent Qualité_CustomerService";
            }

            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 9))
            {
                ViewBag.role = "Agent Qualité_Diffusion";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 2009))
            {
                ViewBag.role = "Agent Qualité_AchatPublic";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 2014))
            {
                ViewBag.role = "Agent Qualité_PRV";
            }
            //var role = serviceEmployee.getListRoles(user).ToList().FirstOrDefault();
            directoryModel.roleName = Id;
            directoryModel.roleKey = Version;
            test.affectAttributes(directoryModel, user.Id, user.UserName, user.pseudoName);
            if (user.Content != null)
            {
                String Url = test.renderImage(user.Content, user.ContentType);
                ViewBag.url = Url;
                directoryModel.Url = Url;

            }
            return View(directoryModel);
        }
       
        public ActionResult TestDiffusion()
        {
            List<String> tests = new List<String>();
            List<String> testsFiles = new List<String>();

            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

            var logins = serviceGroupeEmp.getListEmployeeByGroupe(user.Id);
            var us = logins.Select(o => o).Distinct().ToList();
            var uspseudoNames = logins.Select(o => o.pseudoName).Distinct().ToList();
            foreach (var test in us)
            {
                directoryModel.utilisateurs.Add(new SelectListItem { Text = test.UserName, Value = test.UserName });
                directoryModel.utilisateursPseudonames.Add(new SelectListItem { Text = test.pseudoName, Value = test.pseudoName });
                directoryModel.IdHermeses.Add(new SelectListItem { Text = "" + test.IdHermes, Value = "" + test.IdHermes });
            }
            test.affectAttributes(directoryModel, user.Id, user.UserName, user.pseudoName);

            if (user.Content != null)
            {
                String Url = test.renderImage(user.Content, user.ContentType);
                ViewBag.url = Url;
                directoryModel.Url = Url;

            }
            return View(directoryModel);
        }
        [HttpGet]
        public ActionResult TestDiffusion2()
        {
            List<String> tests = new List<String>();
            List<String> testsFiles = new List<String>();

            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

            var logins = serviceGroupeEmp.getListEmployeeByGroupe(user.Id);
            var us = logins.Select(o => o).Distinct().ToList();
            var uspseudoNames = logins.Select(o => o.pseudoName).Distinct().ToList();
            foreach (var test in us)
            {
                directoryModel.utilisateurs.Add(new SelectListItem { Text = test.UserName, Value = test.UserName });
                directoryModel.utilisateursPseudonames.Add(new SelectListItem { Text = test.pseudoName, Value = test.pseudoName });
                directoryModel.IdHermeses.Add(new SelectListItem { Text = "" + test.IdHermes, Value = "" + test.IdHermes });
            }
            test.affectAttributes(directoryModel, user.Id, user.UserName, user.pseudoName);

            if (user.Content != null)
            {
                String Url = test.renderImage(user.Content, user.ContentType);
                ViewBag.url = Url;
                directoryModel.Url = Url;

            }
            return View(directoryModel);
        }

        public ActionResult Details(String date, String utilisateurs)
        {
            List<String> tests = new List<String>();
            List<String> testsFiles = new List<String>();
            var files = Directory.EnumerateDirectories(@"\\10.9.6.25\Enregistrements\AUTO\REPARMAX_TRT",
            "*.*",
            SearchOption.TopDirectoryOnly).Where(x => new FileInfo(x).Name.Contains(date)); //search criteria for example All Directorys
            foreach (string file in files)
            {
                // Display file path.
                tests.Add(file);

                Console.WriteLine(file);
            }
            ViewBag.files = tests;

            var filesTests = Directory.EnumerateFiles(@"\\10.9.6.25\Enregistrements\AUTO\REPARMAX_TRT",
            "*.*",
            SearchOption.TopDirectoryOnly).Where(x => new FileInfo(x).Name.Contains(date)); //search criteria for example All Directorys
            foreach (string file in filesTests)
            {
                // Display file path.
                testsFiles.Add(file);

                Console.WriteLine(file);
            }
            ViewBag.testsFiles = testsFiles;
           
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            var a = new DirectoryViewModel();

            var logins = serviceGroupeEmp.getListEmployeeByGroupe(user.Id);
            var us = logins.Select(o => o.UserName).Distinct().ToList();
            foreach (var test in us)
            {
                a.utilisateurs.Add(new SelectListItem { Text = test, Value = test });
            }

           
            return View(a);
        }

        [HttpGet]

        public ActionResult Find(String utilisateurs, String pseudoNames, String roleName, string start, string end)
        {

            var a = new DirectoryViewModel();
            a.pseudoName = pseudoNames;
            a.agentName = pseudoNames;
            a.roleName = roleName;
            // ViewBag.agentName = pseudoNames;
            //string v = date.Replace("/", "_");
            string d = pseudoNames.Replace(" ", "-").Replace("é", "Ã©");
            string dd = pseudoNames.Replace(" ", "--").Replace("é", "Ã©");
            int i = 0;



            DateTime creationDate = DateTime.Now;
            DateTime EndDate = DateTime.Now;
            if (start != null && end != null)
            {
                creationDate = DateTime.Parse(start);
                EndDate = DateTime.Parse(end + " 23:00:00");
            }
            else
            {
                var b = new DirectoryViewModel();
                //ViewBag.message = ("vous n'avez pas selectionnée une date");
                return PartialView("SelectDateErreur", b);
            }


            var test = new DirectoryInfo(@"\\10.9.6.25\Enregistrements\" + roleName + "\\" + utilisateurs);
            var tests2 = test.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(y => y.LastWriteTime >= creationDate);


            DateTime startTime = DateTime.Now;
            foreach (var item in tests2)
            {
                var direct = item.EnumerateFiles("*", SearchOption.TopDirectoryOnly).Where(x => (x.Name.ToLower().Contains(d.ToLower()) || x.Name.ToLower().Contains(dd.ToLower())) && (x.CreationTime >= creationDate) && (x.CreationTime <= EndDate));
                foreach (var file in direct)
                {
                    a.filesTestsInfo.Add(file);
                    if (file != null)
                    {
                        i = i + 1;

                    }

                }
            }

            DateTime stopTime = DateTime.Now;
            int minutes = (stopTime - startTime).Minutes;
            int secondes = (stopTime - startTime).Seconds;
            a.minutes = minutes;
            a.secondes = secondes;
            a.nbresEnrefgistrements = i;
            if (Request.IsAjaxRequest())
            {
                if ((i == 0))
                {
                    return PartialView("FileError", a);

                }
                else if ((i != 0))
                {
                    return PartialView("DirectoryPartialView", a);

                }
                else
                {
                    return RedirectToAction("Index");
                }
            }

            else
            {
                return RedirectToAction("Details");
            }
        }
        [HttpGet]

        public ActionResult FindCompagneDate(String utilisateurs, String roleName, string start, string end)
        {

            int i = 0;
            var a = new DirectoryViewModel();

            a.roleName = roleName;
            DateTime creationDate = DateTime.Now;
            DateTime EndDate = DateTime.Now;
            if (start != null && end != null)
            {
                creationDate = DateTime.Parse(start);
                EndDate = DateTime.Parse(end + " 23:00:00");
            }
            else
            {
                var b = new DirectoryViewModel();
                return PartialView("SelectDateErreur", b);
            }


            var test = new DirectoryInfo(@"\\10.9.6.25\Enregistrements\" + roleName + "\\" + utilisateurs);
            var tests2 = test.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(y => y.LastWriteTime >= creationDate);


            DateTime startTime = DateTime.Now;
            foreach (var item in tests2)
            {
                var direct = item.EnumerateFiles("*", SearchOption.TopDirectoryOnly).Where(x => (x.CreationTime >= creationDate) && (x.CreationTime <= EndDate));
                foreach (var file in direct)
                {
                    a.filesTestsInfo.Add(file);
                    if (file != null)
                    {
                        i = i + 1;

                    }

                }
            }

            DateTime stopTime = DateTime.Now;
            int minutes = (stopTime - startTime).Minutes;
            int secondes = (stopTime - startTime).Seconds;
            a.minutes = minutes;
            a.secondes = secondes;
            a.nbresEnrefgistrements = i;
            if (Request.IsAjaxRequest())
            {
                if ((i == 0))
                {
                    return PartialView("FileError", a);

                }
                else if ((i != 0))
                {
                    return PartialView("DirectoryPartialViewWithoutAgent", a);

                }
                else
                {
                    return RedirectToAction("Index");
                }
            }

            else
            {
                return RedirectToAction("Details");
            }
        }
        [HttpGet]

        public ActionResult FindDiffusion(String utilisateurs, String pseudoNames, String roleName, String start, String end)
        {

            int i = 0;
            var a = new DirectoryViewModel();
            a.pseudoName = pseudoNames;
            a.agentName = pseudoNames;
            a.roleName = roleName;
           // ViewBag.agentName = pseudoNames;
            string d = pseudoNames.Replace(" ", "-").Replace("é", "Ã©");
            string dd = pseudoNames.Replace(" ", "--").Replace("é", "Ã©");
            DateTime creationDate = DateTime.Now;
            DateTime EndDate = DateTime.Now;
            if (start != null && end != null)
            {
                creationDate = DateTime.Parse(start);
                EndDate = DateTime.Parse(end + " 23:00:00");
            }
            else
            {
                var b = new DirectoryViewModel();
                //ViewBag.message = ("vous n'avez pas selectionnée une date");
                return PartialView("SelectDateErreur", b);
            }

            //string startdateModified= start.Replace("/", "_");
            //string enddateModified = end.Replace("/", "_");

            var test = new DirectoryInfo(@"\\10.9.6.25\Enregistrements\" + roleName);
            var tests2 = test.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(y => y.LastWriteTime >= creationDate);

            DateTime startTime = DateTime.Now;


            foreach (var file in tests2)
            {

                var dossier = new DirectoryInfo(@file.FullName);
                var Direcs = dossier.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(y => y.LastWriteTime >= creationDate);

                foreach (var direct in Direcs)
                {

                    var fichiers = direct.EnumerateFiles("*", SearchOption.TopDirectoryOnly).Where((x => ((x.Name.Contains("TV." + utilisateurs + "-")) && ((x.Name.ToLower().Contains(d.ToLower())) || (x.Name.ToLower().Contains(dd.ToLower())) || (x.Name.ToLower().Replace("--","-").Contains(d.ToLower())))) && ((x.CreationTime >= creationDate) && (x.CreationTime <= EndDate))));
                    foreach (var fic in fichiers)
                    {
                        a.filesTestsInfo.Add(fic);
                        if (fic != null)
                        {
                            i = i + 1;
                      
                        }

                    }



                }

            }


            DateTime stopTime = DateTime.Now;
            int minutes = (stopTime - startTime).Minutes;
            int secondes = (stopTime - startTime).Seconds;
            a.minutes = minutes;
            a.secondes = secondes;
            a.nbresEnrefgistrements = i;//a.filesTestsInfo.count

            if (Request.IsAjaxRequest())
            {
                if ((i == 0))
                {
                    return PartialView("FileError", a);

                }
                else if ((i != 0))
                {
                    return PartialView("DirectoryPartialView", a);

                }
                else
                {
                    return RedirectToAction("Index");
                }
            }

            else
            {
                return RedirectToAction("Details");
            }
        }
        [HttpGet]
        public ActionResult FindRechercheTel(String tel, String roleName, string start, string end)
        {
            int i = 0;
            var a = new DirectoryViewModel();
            DateTime startTime = DateTime.Now;
            DateTime creationDate = DateTime.Now;
            DateTime EndDate = DateTime.Now;
            a.roleName = roleName;
            if (start != null && end != null)
            {
                creationDate = DateTime.Parse(start);
                EndDate = DateTime.Parse(end + " 23:00:00");
            }
            else
            {
                var b = new DirectoryViewModel();
                return PartialView("SelectDateErreur", b);
            }
          
            var test = new DirectoryInfo(@"\\10.9.6.25\Enregistrements\" + roleName);
            var tests2 = test.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(y => y.LastWriteTime >= creationDate);


            foreach (var file in tests2)
            {

                var dossier = new DirectoryInfo(@file.FullName);
                var Direcs = dossier.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(y => y.LastWriteTime >= creationDate);

                foreach (var direct in Direcs)
                {
                    var fichiers = direct.EnumerateFiles("*", SearchOption.TopDirectoryOnly).Where((x => (x.Name.Contains(tel)) && ((x.CreationTime >= creationDate) && (x.CreationTime <= EndDate))));
                    foreach (var fic in fichiers)
                    {
                        a.filesTestsInfo.Add(fic);
                        
                        if (fic != null)
                        {
                            i = i + 1;

                        }

                    }

                }

            }
            DateTime stopTime = DateTime.Now;
            int minutes = (stopTime - startTime).Minutes;
            int secondes = (stopTime - startTime).Seconds;
            a.minutes = minutes;
            a.secondes = secondes;
            a.nbresEnrefgistrements = i;//a.filesTestsInfo.count


            if (Request.IsAjaxRequest())
            {
                if ((i == 0))
                {
                    return PartialView("FileError", a);

                }
                else if ((i != 0))
                {
                    return PartialView("DirectoryPartialViewWithoutAgent", a);

                }
                else
                {
                    return RedirectToAction("Index");
                }
            }

            else
            {
                return RedirectToAction("Details");
            }
        }
       
        [HttpGet]
        public ActionResult FindRechercheParNumeroFiche(String tel, String roleName, string start, string end)
        {
            int i = 0;
            var a = new DirectoryViewModel();
            DateTime startTime = DateTime.Now;
            DateTime creationDate = DateTime.Now;
            DateTime EndDate = DateTime.Now;
            a.roleName = roleName;
            if (start != null && end != null)
            {
                creationDate = DateTime.Parse(start);
                EndDate = DateTime.Parse(end + " 23:00:00");
            }
            else
            {
                var b = new DirectoryViewModel();
                return PartialView("SelectDateErreur", b);
            }
            //string v = date.Replace("/", "_");
            //DateTime dateRecherche = DateTime.Parse(date);
            var test = new DirectoryInfo(@"\\10.9.6.25\Enregistrements\" + roleName);
            var tests2 = test.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(y => y.LastWriteTime >= creationDate);


            foreach (var file in tests2)
            {

                var dossier = new DirectoryInfo(@file.FullName);
                var Direcs = dossier.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(y => y.LastWriteTime >= creationDate);

                foreach (var direct in Direcs)
                {
                    var fichiers = direct.EnumerateFiles("*", SearchOption.TopDirectoryOnly).Where((x => (x.Name.Contains("[IHN-" + tel + "]")) && ((x.CreationTime >= creationDate) && (x.CreationTime <= EndDate))));
                    foreach (var fic in fichiers)
                    {
                        a.filesTestsInfo.Add(fic);
                        if (fic != null)
                        {
                            i = i + 1;

                        }

                    }

                }

            }
            DateTime stopTime = DateTime.Now;
            int minutes = (stopTime - startTime).Minutes;
            int secondes = (stopTime - startTime).Seconds;
            a.minutes = minutes;
            a.secondes = secondes;
            a.nbresEnrefgistrements = i;//a.filesTestsInfo.count


            if (Request.IsAjaxRequest())
            {
                if ((i == 0))
                {
                    return PartialView("FileError", a);

                }
                else if ((i != 0))
                {
                    return PartialView("DirectoryPartialViewWithoutAgent", a);

                }
                else
                {
                    return RedirectToAction("Index");
                }
            }

            else
            {
                return RedirectToAction("Details");
            }
        }

        [HttpGet]
        public ActionResult FindDMultipleAgent(String[] pseudoNames, string roleName, String start, String end)
        {
            int i = 0;
            //int startNom;
            var a = new DirectoryViewModel();
            //a.pseudoName = pseudoNames;
            DateTime startTime = DateTime.Now;
            DateTime creationDate = DateTime.Now;
            DateTime EndDate = DateTime.Now;
            a.roleName = roleName;
            if (start != null && end != null)
            {
                creationDate = DateTime.Parse(start);
                EndDate = DateTime.Parse(end + " 23:00:00");
            }
            else
            {
                var b = new DirectoryViewModel();
                return PartialView("SelectDateErreur", b);
            }

            if (pseudoNames != null)
            {
                var test = new DirectoryInfo(@"\\10.9.6.25\Enregistrements\" + roleName);
                var tests2 = test.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(y => y.LastWriteTime >= creationDate);
                foreach (var item in pseudoNames)
                {
                    string d = item.Replace(" ", "-").Replace("é", "Ã©");
                    string dd = item.Replace(" ", "--").Replace("é", "Ã©");
                    foreach (var file in tests2)
                    {

                        var dossier = new DirectoryInfo(@file.FullName);
                        var Direcs = dossier.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(y => y.LastWriteTime >= creationDate);

                        foreach (var direct in Direcs)
                        {
                            var fichiers = direct.EnumerateFiles("*", SearchOption.TopDirectoryOnly).Where(x => (((x.Name.ToLower().Contains(d.ToLower())) || (x.Name.ToLower().Contains(dd.ToLower()))) && ((x.CreationTime >= creationDate) && (x.CreationTime <= EndDate))));

                            foreach (var fic in fichiers)
                            {
                                a.filesTestsInfo.Add(fic);
                                a.enregistrementFullName = fic.Name;
                               
                                if (fic != null)
                                {
                                    i = i + 1;
                                    TempData["nbEnr"] = i;
                                }
                            }
                        }
                    }
                }
                DateTime stopTime = DateTime.Now;
                int minutes = (stopTime - startTime).Minutes;
                int secondes = (stopTime - startTime).Seconds;
                a.minutes = minutes;
                a.secondes = secondes;
                a.nbresEnrefgistrements = i;
            }

            if (Request.IsAjaxRequest())
            {
                if ((i == 0))
                {
                    return PartialView("FileError", a);

                }
                else if ((i != 0))
                {
                    return PartialView("DirectoryPartialViewWithoutAgent", a);

                }
                else
                {
                    return RedirectToAction("Index");
                }
            }

            else
            {
                return RedirectToAction("Details");
            }
        }

        [HttpGet]
        public ActionResult FindAllCases(string compagne, string start, string end, String[] pseudoNames, string recherche, string idhermes, string roleName)
        {
            int i = 0;
            var a = new DirectoryViewModel();
            DateTime startTime = DateTime.Now;
            DateTime creationDate = DateTime.Now;
            DateTime EndDate = DateTime.Now;
            a.roleName = roleName;
            if (start != null && end != null)
            {
                creationDate = DateTime.Parse(start);
                EndDate = DateTime.Parse(end + " 23:00:00");
            }
            else
            {
                var b = new DirectoryViewModel();
                return PartialView("SelectDateErreur", b);
            }

            if (compagne.Equals(""))
            {
                var test = new DirectoryInfo(@"\\10.9.6.25\Enregistrements\" + roleName);
                var tests2 = test.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(y => y.LastWriteTime >= creationDate);

                if (pseudoNames != null)
                {
                    foreach (var item in pseudoNames)
                    {
                        string d = item.Replace(" ", "-").Replace("é", "Ã©");
                        string dd = item.Replace(" ", "--").Replace("é", "Ã©");
                        foreach (var file in tests2)
                        {

                            var dossier = new DirectoryInfo(@file.FullName);
                            var Direcs = dossier.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(y => y.LastWriteTime >= creationDate);

                            foreach (var direct in Direcs)
                            {
                                if (recherche != "" && idhermes != "")
                                {
                                    var fichiers = direct.EnumerateFiles("*", SearchOption.TopDirectoryOnly).Where(x => (((x.Name.ToLower().Contains(d.ToLower())) || (x.Name.ToLower().Contains(dd.ToLower()) || x.Name.Contains(recherche) || (x.Name.Contains("TV." + idhermes + "-")))) && ((x.CreationTime >= creationDate) && (x.CreationTime <= EndDate))));

                                    foreach (var fic in fichiers)
                                    {
                                        a.filesTestsInfo.Add(fic);
                                        if (fic != null)
                                        {
                                            i = i + 1;
                                            TempData["nbEnr"] = i;
                                        }
                                    }

                                }
                                else if (recherche == "" && idhermes != "")
                                {

                                    var fichiers = direct.EnumerateFiles("*", SearchOption.TopDirectoryOnly).Where(x => (((x.Name.ToLower().Contains(d.ToLower())) || (x.Name.ToLower().Contains(dd.ToLower()) || (x.Name.Contains("TV." + idhermes + "-")))) && ((x.CreationTime >= creationDate) && (x.CreationTime <= EndDate))));

                                    foreach (var fic in fichiers)
                                    {
                                        a.filesTestsInfo.Add(fic);
                                        if (fic != null)
                                        {
                                            i = i + 1;
                                            TempData["nbEnr"] = i;
                                        }
                                    }

                                }

                                else if (recherche != "" && idhermes == "")
                                {
                                    var fichiers = direct.EnumerateFiles("*", SearchOption.TopDirectoryOnly).Where(x => (((x.Name.ToLower().Contains(d.ToLower())) || (x.Name.ToLower().Contains(dd.ToLower()) || (x.Name.Contains(recherche)))) && ((x.CreationTime >= creationDate) && (x.CreationTime <= EndDate))));

                                    foreach (var fic in fichiers)
                                    {
                                        a.filesTestsInfo.Add(fic);
                                        if (fic != null)
                                        {
                                            i = i + 1;
                                            TempData["nbEnr"] = i;
                                        }
                                    }
                                }
                                else
                                {
                                    var fichiers = direct.EnumerateFiles("*", SearchOption.TopDirectoryOnly).Where(x => (((x.Name.ToLower().Contains(d.ToLower())) || (x.Name.ToLower().Contains(dd.ToLower()))) && ((x.CreationTime >= creationDate) && (x.CreationTime <= EndDate))));

                                    foreach (var fic in fichiers)
                                    {
                                        a.filesTestsInfo.Add(fic);
                                        if (fic != null)
                                        {
                                            i = i + 1;
                                            TempData["nbEnr"] = i;
                                        }
                                    }
                                }

                            }
                        }
                    }
                }
                else
                {

                    foreach (var file in tests2)
                    {

                        var dossier = new DirectoryInfo(@file.FullName);
                        var Direcs = dossier.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(y => y.LastWriteTime >= creationDate);

                        foreach (var direct in Direcs)
                        {
                            if (recherche != "" && idhermes != "")
                            {
                                var fichiers = direct.EnumerateFiles("*", SearchOption.TopDirectoryOnly).Where(x => (((x.Name.Contains(recherche) || (x.Name.Contains("TV." + idhermes + "-")))) && ((x.CreationTime >= creationDate) && (x.CreationTime <= EndDate))));

                                foreach (var fic in fichiers)
                                {
                                    a.filesTestsInfo.Add(fic);
                                    if (fic != null)
                                    {
                                        i = i + 1;
                                        TempData["nbEnr"] = i;
                                    }
                                }

                            }
                            else if (recherche == "" && idhermes != "")
                            {

                                var fichiers = direct.EnumerateFiles("*", SearchOption.TopDirectoryOnly).Where(x => ((((x.Name.Contains("TV." + idhermes + "-")))) && ((x.CreationTime >= creationDate) && (x.CreationTime <= EndDate))));

                                foreach (var fic in fichiers)
                                {
                                    a.filesTestsInfo.Add(fic);
                                    if (fic != null)
                                    {
                                        i = i + 1;
                                        TempData["nbEnr"] = i;
                                    }
                                }

                            }

                            else if (recherche != "" && idhermes == "")
                            {
                                var fichiers = direct.EnumerateFiles("*", SearchOption.TopDirectoryOnly).Where(x => ((((x.Name.Contains(recherche)))) && ((x.CreationTime >= creationDate) && (x.CreationTime <= EndDate))));

                                foreach (var fic in fichiers)
                                {
                                    a.filesTestsInfo.Add(fic);
                                    if (fic != null)
                                    {
                                        i = i + 1;
                                        TempData["nbEnr"] = i;
                                    }
                                }
                            }

                            


                        }
                    }
                }
                DateTime stopTime = DateTime.Now;
                int minutes = (stopTime - startTime).Minutes;
                int secondes = (stopTime - startTime).Seconds;
                a.minutes = minutes;
                a.secondes = secondes;
                a.nbresEnrefgistrements = i;
            }
            else if (!compagne.Equals(""))
            {
                var test = new DirectoryInfo(@"\\10.9.6.25\Enregistrements\" + roleName + "\\" + compagne);
                var tests2 = test.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(y => y.LastWriteTime >= creationDate && ((y.CreationTime >= creationDate) && (y.CreationTime <= EndDate))); 
                if (pseudoNames == null && recherche == "" && idhermes == "")
                {
                    foreach (var file in tests2)
                    {
                        var fichiers = file.EnumerateFiles("*", SearchOption.TopDirectoryOnly);

                        foreach (var fic in fichiers)
                        {
                            a.filesTestsInfo.Add(fic);
                            if (fic != null)
                            {
                                i = i + 1;
                                TempData["nbEnr"] = i;
                            }
                        }
                    }
                }


                if (pseudoNames != null)
                {


                    foreach (var item in pseudoNames)
                    {
                        string d = item.Replace(" ", "-");
                        string dd = item.Replace(" ", "--");



                        foreach (var direct in tests2)
                        {

                            if (recherche != "" && idhermes != "")
                            {
                                var fichiers = direct.EnumerateFiles("*", SearchOption.TopDirectoryOnly).Where(x => (((x.Name.ToLower().Contains(d.ToLower())) || (x.Name.ToLower().Contains(dd.ToLower()) || x.Name.Contains(recherche) || (x.Name.Contains("TV." + idhermes + "-")))) && ((x.CreationTime >= creationDate) && (x.CreationTime <= EndDate))));

                                foreach (var fic in fichiers)
                                {
                                    a.filesTestsInfo.Add(fic);
                                    if (fic != null)
                                    {
                                        i = i + 1;
                                        TempData["nbEnr"] = i;
                                    }
                                }

                            }
                            else if (recherche == "" && idhermes != "")
                            {

                                var fichiers = direct.EnumerateFiles("*", SearchOption.TopDirectoryOnly).Where(x => (((x.Name.ToLower().Contains(d.ToLower())) || (x.Name.ToLower().Contains(dd.ToLower()) || (x.Name.Contains("TV." + idhermes + "-")))) && ((x.CreationTime >= creationDate) && (x.CreationTime <= EndDate))));

                                foreach (var fic in fichiers)
                                {
                                    a.filesTestsInfo.Add(fic);
                                    if (fic != null)
                                    {
                                        i = i + 1;
                                        TempData["nbEnr"] = i;
                                    }
                                }

                            }

                            else if (recherche != "" && idhermes == "")
                            {
                                var fichiers = direct.EnumerateFiles("*", SearchOption.TopDirectoryOnly).Where(x => (((x.Name.ToLower().Contains(d.ToLower())) || (x.Name.ToLower().Contains(dd.ToLower()) || (x.Name.Contains(recherche)))) && ((x.CreationTime >= creationDate) && (x.CreationTime <= EndDate))));

                                foreach (var fic in fichiers)
                                {
                                    a.filesTestsInfo.Add(fic);
                                    if (fic != null)
                                    {
                                        i = i + 1;
                                        TempData["nbEnr"] = i;
                                    }
                                }
                            }

                            else
                            {
                                var fichiers = direct.EnumerateFiles("*", SearchOption.TopDirectoryOnly).Where(x => (((x.Name.ToLower().Contains(d.ToLower())) || (x.Name.ToLower().Contains(dd.ToLower()))) && ((x.CreationTime >= creationDate) && (x.CreationTime <= EndDate))));

                                foreach (var fic in fichiers)
                                {
                                    a.filesTestsInfo.Add(fic);
                                    if (fic != null)
                                    {
                                        i = i + 1;
                                        TempData["nbEnr"] = i;
                                    }
                                }
                            }


                        }

                    }
                }
                else
                {

                    foreach (var direct in tests2)
                    {





                        if (recherche != "" && idhermes != "")
                        {
                            var fichiers = direct.EnumerateFiles("*", SearchOption.TopDirectoryOnly).Where(x => (((x.Name.Contains(recherche) || (x.Name.Contains("TV." + idhermes + "-")))) && ((x.CreationTime >= creationDate) && (x.CreationTime <= EndDate))));

                            foreach (var fic in fichiers)
                            {
                                a.filesTestsInfo.Add(fic);
                                if (fic != null)
                                {
                                    i = i + 1;
                                    TempData["nbEnr"] = i;
                                }
                            }

                        }
                        else if (recherche == "" && idhermes != "")
                        {

                            var fichiers = direct.EnumerateFiles("*", SearchOption.TopDirectoryOnly).Where(x => ((((x.Name.Contains("TV." + idhermes + "-")))) && ((x.CreationTime >= creationDate) && (x.CreationTime <= EndDate))));

                            foreach (var fic in fichiers)
                            {
                                a.filesTestsInfo.Add(fic);
                                if (fic != null)
                                {
                                    i = i + 1;
                                    TempData["nbEnr"] = i;
                                }
                            }

                        }

                        else if (recherche != "" && idhermes == "")
                        {
                            var fichiers = direct.EnumerateFiles("*", SearchOption.TopDirectoryOnly).Where(x => ((((x.Name.Contains(recherche)))) && ((x.CreationTime >= creationDate) && (x.CreationTime <= EndDate))));

                            foreach (var fic in fichiers)
                            {
                                a.filesTestsInfo.Add(fic);
                                if (fic != null)
                                {
                                    i = i + 1;
                                    TempData["nbEnr"] = i;
                                }
                            }
                        }





                    }
                }


                DateTime stopTime = DateTime.Now;
                int minutes = (stopTime - startTime).Minutes;
                int secondes = (stopTime - startTime).Seconds;
                a.minutes = minutes;
                a.secondes = secondes;
                a.nbresEnrefgistrements = i;



            }
            if (Request.IsAjaxRequest())
            {
                if ((i == 0))
                {
                    return PartialView("FileError", a);

                }
                else if ((i != 0))
                {
                    return PartialView("DirectoryPartialViewWithoutAgent", a);

                }
                else
                {
                    return RedirectToAction("Index");
                }
            }

            else
            {
                return RedirectToAction("Details");
            }
        }

        [HttpGet]

        public ActionResult FindDiffusionTest(String utilisateurs, String pseudoNames, String start, String end)
        {
            int i = 0;
            var a = new DirectoryViewModel();
            a.pseudoName = pseudoNames;
           
            string d = pseudoNames.Replace(" ", "-").Replace("é", "Ã©");
            string dd = pseudoNames.Replace(" ", "--").Replace("é", "Ã©");
            DateTime creationDate = DateTime.Now;
            DateTime EndDate = DateTime.Now;
            if (start != null && end != null)
            {
                creationDate = DateTime.Parse(start);
                EndDate = DateTime.Parse(end);
            }
            else
            {
                var b = new DirectoryViewModel();
                return PartialView("SelectDateErreur", b);
            }

          

            var test = new DirectoryInfo(@"\\10.9.6.25\Enregistrements\DIFFUSION");
            var tests2 = test.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(y => y.LastWriteTime >= creationDate);

            DateTime startTime = DateTime.Now;

            Parallel.ForEach(tests2, (file) =>
            {

                var dossier = new DirectoryInfo(@file.FullName);
                var fichiers = dossier.EnumerateFiles("*", SearchOption.AllDirectories).Where((x => ((x.Name.Contains("TV." + utilisateurs + "-")) && ((x.Name.ToLower().Contains(d.ToLower())) || (x.Name.ToLower().Contains(dd.ToLower())))) && ((x.CreationTime >= creationDate) && (x.CreationTime <= EndDate))));

                foreach (var fic in fichiers)
                {
                    a.filesTestsInfo.Add(fic);
                    if (fic != null)
                    {
                        i = i + 1;
                    }
                }




            });

            DateTime stopTime = DateTime.Now;
            int minutes = (stopTime - startTime).Minutes;
            int secondes = (stopTime - startTime).Seconds;
            a.minutes = minutes;
            a.secondes = secondes;
            a.nbresEnrefgistrements = i;//a.filesTestsInfo.count


            if (Request.IsAjaxRequest())
            {
                if ((i == 0))
                {
                    return PartialView("FileError", a);

                }
                else if ((i != 0))
                {
                    return PartialView("DirectoryPartialViewTestDiffusion", a);

                }
                else
                {
                    return RedirectToAction("Index");
                }
            }

            else
            {
                return RedirectToAction("Details");
            }
        }
        [HttpGet]
        public ActionResult FindCustomEnregistrement(String name, String fullName, String DirectoryName)
        {
            var b = new DirectoryViewModel();
            Random rnd = new Random();
            int iss = rnd.Next(1, 10000);
         
            try
            {

                if (Request.IsAjaxRequest())
                {
                    string fullName2 = fullName.Replace(" ", "+");
                    string name2 = name.Replace("+", "");
                    b.pseudoName = name;
                    var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
                    b.userName = user.UserName;
                    b.enregistrementName = fullName;
                    b.directoryName = DirectoryName;
                    var fileInfo = new FileInfo(@name2);
                    System.IO.File.Copy(fullName, Server.MapPath("~/Files/") + name2, true);

                    return PartialView("CustomEnregistrement", b);
                }
            }
            catch (Exception)
            {

                return PartialView("CustomEnregistrement", b);
            }
            return RedirectToAction("Index");

        }
       
        public ActionResult Play(string name)
        {
            var b = new DirectoryViewModel();
            b.enregistrementName = name;
            if (Request.IsAjaxRequest())
            {
                
                return PartialView("Sound", b);
            }

            return RedirectToAction("Index");
            
        }
        
        public ActionResult Create()
        {
            return View();
        }

        // POST: Directory/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here


                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Directory/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Directory/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Directory/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Directory/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        public void download(string name)
        {
            //var fName = Path.GetFullPath(@name);
            //string extName = Path.GetExtension(name);
            //var fileInfo = new FileInfo(@name);

            //var response = HttpContext.Response;
            System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;

            //response.Clear();
            //response.ClearContent();
            //response.ClearHeaders();

            //response.Buffer = true;

            response.AddHeader("Content-Description", "attachment;filename=" + name);
            response.ContentType = "application/octet-stream";
            response.WriteFile(name);
            response.Flush();

            // Response.TransmitFile(Server.MapPath("~/Files/autorisation.wav"));
            response.End();
        }
        public void download2(string name)
        {
           
            var fileInfo = new FileInfo(@name);
            System.IO.File.Copy(name, "C:\\" + fileInfo.Name, true); // overwrite = true

        }
        public ActionResult deleteEnregistrement(string name)
        {

            try
            {
                System.IO.File.Delete(Server.MapPath("~/Files/") + name);

                return PartialView("SelectDateErreur");
            }
            catch (Exception)
            {
                Console.WriteLine("S'il vout plait attender quelques secondes afin que le serveur traite votre demande");
                return RedirectToAction("DirectoryError");
            }
        }
        public ActionResult Acceuil(string id)
        {

            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 5))
            {
                ViewBag.role = "Agent Qualité";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 4))
            {
                ViewBag.role = "Qualité";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 8))
            {
                ViewBag.role = "Agent Qualité_CustomerService";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 9))
            {
                ViewBag.role = "Agent Qualité_Diffusion";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 2009))
            {
                ViewBag.role = "Agent Qualité_AchatPublic";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 2014))
            {
                ViewBag.role = "Agent Qualité_PRV";
            }
            //var roles = serviceEmployee.getListRoles(user).FirstOrDefault();
            //directoryModel.roles.Add(new SelectListItem { Text = roles.Name, Value = roles.Name });
            directoryModel.roleName = id;
            string bb = "" + DateTime.Today.Date;
            string n = bb.Substring(0, 10);
            string v = n.Replace("/", "_");

            var testin = new DirectoryInfo(@"\\10.9.6.25\Enregistrements\" + id);
            var tests2 = testin.EnumerateDirectories("*", SearchOption.TopDirectoryOnly);

            DateTime startTime = DateTime.Now;

            int Max = 0;

            foreach (var file in tests2)
            {


                if (Directory.Exists(@"\\10.9.6.25\Enregistrements\" + id + "\\" + file.Name + "\\" + v) == true)
                {
                    var tt = new DirectoryInfo(@"\\10.9.6.25\Enregistrements\" + id + "\\" + file.Name + "\\" + v);
                    if (tt.EnumerateFiles().Count() > Max)
                    {
                        Max = tt.EnumerateFiles().Count();
                        directoryModel.indice = file.Name;
                    }
                    int value = tt.EnumerateFiles().Count();

                    directoryModel.compagneEnreg.Add(new SelectListItem { Text = file.Name, Value = "" + value });
                }

            }
            directoryModel.NombreEnregistrements = Max;

            test.affectAttributes(directoryModel, user.Id, user.UserName, user.pseudoName);
            if (user.Content != null)
            {
                String Url = test.renderImage(user.Content, user.ContentType);
                ViewBag.url = Url;
                directoryModel.Url = Url;

            }
            return View(directoryModel);
        }

        public ActionResult ReabRef()
        {
            
            return View();
        }
        public ActionResult PromoRef()
        {
            

            return View();
        }
        [HttpGet]
        public ActionResult AnnulerRecherche(String utilisateurs, String pseudoNames, String start, String end)
        {
            //throw new TimeoutException();

            return RedirectToAction("Diffusion");
        }

        #region Agent Qualité
        public ActionResult listeSitesAgentQualite()
        {
            var a = new DirectoryViewModel();
           // var listDirectory = new DirectoryInfo(@"\\10.9.6.25\Enregistrements\");
           // var files = listDirectory.EnumerateDirectories("*.*", SearchOption.TopDirectoryOnly);
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 8))
            {
                ViewBag.role = "Agent Qualité_CustomerService";
            }

            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 9))
            {
                ViewBag.role = "Agent Qualité_Diffusion";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 5))
            {
                ViewBag.role = "Agent Qualité";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 2009))
            {
                ViewBag.role = "Agent Qualité_AchatPublic";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 2014))
            {
                ViewBag.role = "Agent Qualité_PRV";
            }
            var groupes = serviceGroupeEmp.getGroupeByIDEmployee(user.Id);
            foreach (var g in groupes)
            {
                if(g.nom == "GISI-REAB" || g.nom == "GISI-PROMO" || g.nom == "GMT-REAB" || g.nom == "GMT-PROMO")
                {
                    g.nom = "DIFFUSION";
                }
                if (Directory.Exists(@"\\10.9.6.25\Enregistrements\" + g.nom) == true)
                {
                    a.listeSites.Add(new SelectListItem { Value = "V4", Text = g.nom });
                }
                else
                {
                    a.listeSitesV5.Add(new SelectListItem { Value = g.responsable, Text = g.nom });
                }
            }
            //var groupes = groupesassociees.Select(o => o.nom).Distinct().ToList();
            //foreach (var item in groupes)
            //{
            //    a.listeSites.Add(new SelectListItem { Value = item, Text = item });
            //}
           // baseModel.DirecoryModel = a;
          
            test.affectAttributes(a, user.Id, user.UserName, user.pseudoName);
            if (user.Content != null)
            {
                String Url = test.renderImage(user.Content, user.ContentType);
                ViewBag.url = Url;
                a.Url = Url;

            }
            return View(a);
        }
        #endregion

        #region V5 Hermes

        public ActionResult OptionRechercheV5(string Id, string Version)
        {
            Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (emp.Roles.Any(b => b.UserId == emp.Id && b.RoleId == 5))
            {
                ViewBag.role = "Agent Qualité";
            }
            if (emp.Roles.Any(b => b.UserId == emp.Id && b.RoleId == 4))
            {
                ViewBag.role = "Qualité";
            }
            if (emp.Roles.Any(b => b.UserId == emp.Id && b.RoleId == 8))
            {
                ViewBag.role = "Agent Qualité_CustomerService";
            }

            if (emp.Roles.Any(b => b.UserId == emp.Id && b.RoleId == 9))
            {
                ViewBag.role = "Agent Qualité_Diffusion";
            }
            if (emp.Roles.Any(b => b.UserId == emp.Id && b.RoleId == 2009))
            {
                ViewBag.role = "Agent Qualité_AchatPublic";
            }
            if (emp.Roles.Any(b => b.UserId == emp.Id && b.RoleId == 2014))
            {
                ViewBag.role = "Agent Qualité_PRV";
            }

            DirectoryViewModel a = new DirectoryViewModel();
            a.roleName = Id;
            a.roleKey = Version;
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            test.affectAttributes(a, user.Id, user.UserName, user.pseudoName);
            if (user.Content != null)
            {
                String Url = test.renderImage(user.Content, user.ContentType);
                ViewBag.url = Url;
                a.Url = Url;

            }
            return View(a);
        }

        public ActionResult AcceuilV5(string id, string Version)
        {
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 5))
            {
                ViewBag.role = "Agent Qualité";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 4))
            {
                ViewBag.role = "Qualité";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 8))
            {
                ViewBag.role = "Agent Qualité_CustomerService";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 9))
            {
                ViewBag.role = "Agent Qualité_Diffusion";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 2009))
            {
                ViewBag.role = "Agent Qualité_AchatPublic";
            }
            if (user.Roles.Any(b => b.UserId == user.Id && b.RoleId == 2014))
            {
                ViewBag.role = "Agent Qualité_PRV";
            }
            directoryModel.roleName = id;
            directoryModel.roleKey = Version;
       
            DateTime today = DateTime.Now;
            string month = today.Month.ToString();
            List<accueil> calculs = new List<accueil>();
            if (today.Month < 10)
            {
                month = "0" + month;
            }
            string day = today.Day.ToString();
            if (today.Day < 10)
            {
                day = "0" + day;
            }
            int Max = 0;
            double total = 0;
            var testin = new DirectoryInfo(@"\\10.9.6.25\Enregistrements\hermes_p\Files\" + id + "\\RECORDS");
            var tests2 = testin.EnumerateDirectories("*", SearchOption.TopDirectoryOnly);

            foreach (var ff in tests2)
            {

                if (Directory.Exists(@"\\10.9.6.25\Enregistrements\hermes_p\Files\" + id + "\\RECORDS\\" + ff.Name + "\\" + today.Year + "\\" + month + "\\" + day) == true)
                {
                    var tt = new DirectoryInfo(@"\\10.9.6.25\Enregistrements\hermes_p\Files\" + id + "\\RECORDS\\" + ff.Name + "\\" + today.Year + "\\" + month + "\\" + day);
                    var All = tt.EnumerateFiles("*", SearchOption.AllDirectories);

                    foreach (var i in All)
                    {
                        if (i != null)
                        {
                            total += 1;
                        }
                    }
                }
            }
            foreach (var file in tests2)
            {
                double value = 0;

                if (Directory.Exists(@"\\10.9.6.25\Enregistrements\hermes_p\Files\" + id + "\\RECORDS\\" + file.Name + "\\" + today.Year + "\\" + month + "\\" + day) == true)
                {
                    var tt = new DirectoryInfo(@"\\10.9.6.25\Enregistrements\hermes_p\Files\" + id + "\\RECORDS\\" + file.Name + "\\" + today.Year + "\\" + month + "\\" + day);
                    var All = tt.EnumerateFiles("*", SearchOption.AllDirectories);

                    foreach (var f in All)
                    {

                        if (All.Count() > Max)
                        {
                            Max = All.Count();
                            directoryModel.indice = file.Name;
                        }
                        if (f != null)
                        {
                            value += 1;
                        }
                    }
                    double p = Math.Round(((value / total) * 100), 2);

                    directoryModel.indice = file.Name;

                    directoryModel.compagneEnregV5.Add(new accueil { Text = file.Name, Value = value, Percent = p });

                }
            }
            directoryModel.NombreEnregistrements = Max;
            directoryModel.NombreEnregistrementsTotal = total;
            test.affectAttributes(directoryModel, user.Id, user.UserName, user.pseudoName);
            if (user.Content != null)
            {
                String Url = test.renderImage(user.Content, user.ContentType);
                ViewBag.url = Url;
                directoryModel.Url = Url;

            }
            return View(directoryModel);
        }

        [HttpGet]
        public ActionResult FindCompagneDateV5(String utilisateurs, String roleName, string roleKey, string start, string end)
        {
            int i = 0;
            var a = new DirectoryViewModel();
            a.roleName = roleKey;
            DateTime creationDate = DateTime.Now;
            DateTime EndDate = DateTime.Now;
            if (start != null && end != null)
            {
                creationDate = DateTime.Parse(start);
                EndDate = DateTime.Parse(end + " 23:00:00");
            }
            else
            {
                var b = new DirectoryViewModel();
                return PartialView("SelectDateErreur", b);
            }

            var test = new DirectoryInfo(@"\\10.9.6.25\Enregistrements\hermes_p\Files\" + roleName + @"\\RECORDS\" + utilisateurs);
            var YearFiltred = test.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(y => y.Name == (creationDate.Year).ToString() && y.Name == (EndDate.Year).ToString());

            DateTime startTime = DateTime.Now;
            foreach (var yearitem in YearFiltred)
            {
                var MonthFiltred = yearitem.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(x => (Int32.Parse(x.Name) >= creationDate.Month) && (Int32.Parse(x.Name) <= EndDate.Month));
                foreach (var monthitem in MonthFiltred)
                {
                    var DayFiltred = monthitem.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(x => (x.LastWriteTime >= creationDate) && (x.LastWriteTime <= EndDate));
                    foreach (var item in DayFiltred)
                    {
                        var AllAgents = item.EnumerateFiles("*", SearchOption.AllDirectories);

                        foreach (var file in AllAgents)
                        {
                            a.filesTestsInfo.Add(file);
                            if (file != null)
                            {
                                i = i + 1;
                            }
                        }
                    }
                }
            }

            DateTime stopTime = DateTime.Now;
            int minutes = (stopTime - startTime).Minutes;
            int secondes = (stopTime - startTime).Seconds;
            a.minutes = minutes;
            a.secondes = secondes;
            a.nbresEnrefgistrements = i;
            if (Request.IsAjaxRequest())
            {
                if ((i == 0))
                {
                    return PartialView("FileError", a);
                }
                else if ((i != 0))
                {
                    return PartialView("DirectoryPartialViewWithoutAgent", a);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return RedirectToAction("Details");
            }
        }
        [HttpGet]
        public ActionResult FindParAgentV5(String IdHermes, String pseudoNames, String roleName, String start, String end)
        {

            int i = 0;
            var a = new DirectoryViewModel();
            a.pseudoName = pseudoNames;
            a.agentName = pseudoNames;
            a.roleName = roleName;
            DateTime creationDate = DateTime.Now;
            DateTime EndDate = DateTime.Now;
            if (start != null && end != null)
            {
                creationDate = DateTime.Parse(start);
                EndDate = DateTime.Parse(end + " 23:00:00");
            }
            else
            {
                var b = new DirectoryViewModel();
                //ViewBag.message = ("vous n'avez pas selectionnée une date");
                return PartialView("SelectDateErreur", b);
            }

            var test = new DirectoryInfo(@"\\10.9.6.25\Enregistrements\hermes_p\Files\" + roleName + "\\RECORDS");
            var tests2 = test.EnumerateDirectories("*", SearchOption.AllDirectories);

            DateTime startTime = DateTime.Now;
            foreach (var dir in tests2)
            {
                var YearFiltred = dir.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(y => y.Name == (creationDate.Year).ToString() && y.Name == (EndDate.Year).ToString());
                foreach (var yearitem in YearFiltred)
                {
                    var MonthFiltred = yearitem.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(x => (Int32.Parse(x.Name) >= creationDate.Month) && (Int32.Parse(x.Name) <= EndDate.Month));
                    foreach (var monthitem in MonthFiltred)
                    {
                        var DayFiltred = monthitem.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(x => (x.LastWriteTime >= creationDate) && (x.LastWriteTime <= EndDate));
                        foreach (var item in DayFiltred)
                        {
                            var AgentFiltred = item.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(x => (x.Name == IdHermes));
                            foreach (var f in AgentFiltred)
                            {
                                var fichiers = f.EnumerateFiles("*", SearchOption.TopDirectoryOnly);
                                foreach (var file in fichiers)
                                {

                                    a.filesTestsInfo.Add(file);
                                    if (file != null)
                                    {
                                        i = i + 1;
                                    }
                                }
                            }

                        }
                    }
                }
            }


            DateTime stopTime = DateTime.Now;
            int minutes = (stopTime - startTime).Minutes;
            int secondes = (stopTime - startTime).Seconds;
            a.minutes = minutes;
            a.secondes = secondes;
            a.nbresEnrefgistrements = i;//a.filesTestsInfo.count

            if (Request.IsAjaxRequest())
            {
                if ((i == 0))
                {
                    return PartialView("FileError", a);

                }
                else if ((i != 0))
                {
                    return PartialView("DirectoryPartialView", a);

                }
                else
                {
                    return RedirectToAction("Index");
                }
            }

            else
            {
                return RedirectToAction("Details");
            }
        }

        [HttpGet]
        public ActionResult FindParCampagneParAgentV5(String utilisateurs, String pseudoNames, String roleName, string start, string end)
        {
            Employee emp = serviceEmployee.getByPseudoName(pseudoNames);
            string IdHermes = (emp.IdHermes).ToString();
            var a = new DirectoryViewModel();
            a.pseudoName = pseudoNames;
            a.agentName = pseudoNames;
            a.roleName = roleName;
            int i = 0;
            DateTime creationDate = DateTime.Now;
            DateTime EndDate = DateTime.Now;
            if (start != null && end != null)
            {
                creationDate = DateTime.Parse(start);
                EndDate = DateTime.Parse(end + " 23:00:00");
            }
            else
            {
                var b = new DirectoryViewModel();
                //ViewBag.message = ("vous n'avez pas selectionnée une date");
                return PartialView("SelectDateErreur", b);
            }

            var test = new DirectoryInfo(@"\\10.9.6.25\Enregistrements\hermes_p\Files\" + roleName + "\\RECORDS\\" + utilisateurs);
            var YearFiltred = test.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(y => y.Name == (creationDate.Year).ToString() && y.Name == (EndDate.Year).ToString());

            DateTime startTime = DateTime.Now;
            foreach (var yearitem in YearFiltred)
            {
                var MonthFiltred = yearitem.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(x => (Int32.Parse(x.Name) >= creationDate.Month) && (Int32.Parse(x.Name) <= EndDate.Month));
                foreach (var monthitem in MonthFiltred)
                {
                    var DayFiltred = monthitem.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(x => (x.LastWriteTime >= creationDate) && (x.LastWriteTime <= EndDate));
                    foreach (var item in DayFiltred)
                    {
                        var AgentFiltred = item.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(x => (x.Name == IdHermes));

                        foreach (var f in AgentFiltred)
                        {
                            var fichiers = f.EnumerateFiles("*", SearchOption.AllDirectories);
                            foreach (var file in fichiers)
                            {
                                a.filesTestsInfo.Add(file);
                                if (file != null)
                                {
                                    i = i + 1;
                                }
                            }
                        }
                    }
                }
            }

            DateTime stopTime = DateTime.Now;
            int minutes = (stopTime - startTime).Minutes;
            int secondes = (stopTime - startTime).Seconds;
            a.minutes = minutes;
            a.secondes = secondes;
            a.nbresEnrefgistrements = i;
            if (Request.IsAjaxRequest())
            {
                if ((i == 0))
                {
                    return PartialView("FileError", a);
                }
                else if ((i != 0))
                {
                    return PartialView("DirectoryPartialView", a);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }

            else
            {
                return RedirectToAction("Details");
            }
        }

        [HttpGet]
        public ActionResult FindDMultipleAgentV5(String[] pseudoNames, string roleName, string roleKey, String start, String end)
        {
            int i = 0;
            var a = new DirectoryViewModel();
            //a.pseudoName = pseudoNames;
            a.roleName = roleKey;
            DateTime creationDate = DateTime.Now;
            DateTime EndDate = DateTime.Now;
            if (start != null && end != null)
            {
                creationDate = DateTime.Parse(start);
                EndDate = DateTime.Parse(end + " 23:00:00");
            }
            else
            {
                var b = new DirectoryViewModel();
                return PartialView("SelectDateErreur", b);
            }

            if (pseudoNames != null)
            {

                var test = new DirectoryInfo(@"\\10.9.6.25\Enregistrements\hermes_p\Files\" + roleName + "\\RECORDS");
                var tests2 = test.EnumerateDirectories("*", SearchOption.TopDirectoryOnly);
                DateTime startTime = DateTime.Now;
                foreach (var ps in pseudoNames)
                {
                    Employee emp = serviceEmployee.getByPseudoName(ps);
                    string IdHermes = (emp.IdHermes).ToString();

                    foreach (var dir in tests2)
                    {
                        var YearFiltred = dir.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(y => y.Name == (creationDate.Year).ToString() && y.Name == (EndDate.Year).ToString());
                        foreach (var yearitem in YearFiltred)
                        {
                            var MonthFiltred = yearitem.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(x => (Int32.Parse(x.Name) >= creationDate.Month) && (Int32.Parse(x.Name) <= EndDate.Month));
                            foreach (var monthitem in MonthFiltred)
                            {
                                var DayFiltred = monthitem.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(x => (x.LastWriteTime >= creationDate) && (x.LastWriteTime <= EndDate));
                                foreach (var item in DayFiltred)
                                {
                                    var AgentFiltred = item.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(x => (x.Name == IdHermes));
                                    foreach (var f in AgentFiltred)
                                    {
                                        var fichiers = f.EnumerateFiles("*", SearchOption.TopDirectoryOnly);
                                        foreach (var file in fichiers)
                                        {

                                            a.filesTestsInfo.Add(file);
                                            if (file != null)
                                            {
                                                i = i + 1;
                                                TempData["nbEnr"] = i;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                DateTime stopTime = DateTime.Now;
                int minutes = (stopTime - startTime).Minutes;
                int secondes = (stopTime - startTime).Seconds;
                a.minutes = minutes;
                a.secondes = secondes;
                a.nbresEnrefgistrements = i;
            }

            if (Request.IsAjaxRequest())
            {
                if ((i == 0))
                {
                    return PartialView("FileError", a);
                }
                else if ((i != 0))
                {
                    return PartialView("DirectoryPartialViewWithoutAgent", a);

                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return RedirectToAction("Details");
            }
        }

        [HttpGet]
        public ActionResult FindRechercheNumTelV5(String tel, String roleName, string roleKey, string start, string end)
        {
            int i = 0;
            var a = new DirectoryViewModel();
            a.roleName = roleKey;
            DateTime creationDate = DateTime.Now;
            DateTime EndDate = DateTime.Now;

            if (start != null && end != null)
            {
                creationDate = DateTime.Parse(start);
                EndDate = DateTime.Parse(end + " 23:00:00");
            }
            else
            {
                var b = new DirectoryViewModel();
                return PartialView("SelectDateErreur", b);
            }

            var test = new DirectoryInfo(@"\\10.9.6.25\Enregistrements\hermes_p\Files\" + roleName + "\\RECORDS");
            var tests2 = test.EnumerateDirectories("*", SearchOption.TopDirectoryOnly);
            DateTime startTime = DateTime.Now;
            foreach (var dir in tests2)
            {
                var YearFiltred = dir.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(y => y.Name == (creationDate.Year).ToString() && y.Name == (EndDate.Year).ToString());
                foreach (var yearitem in YearFiltred)
                {
                    var MonthFiltred = yearitem.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(x => (Int32.Parse(x.Name) >= creationDate.Month) && (Int32.Parse(x.Name) <= EndDate.Month));
                    foreach (var monthitem in MonthFiltred)
                    {
                        var DayFiltred = monthitem.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(x => (x.LastWriteTime >= creationDate) && (x.LastWriteTime <= EndDate));
                        foreach (var item in DayFiltred)
                        {
                            var AgentFiltred = item.EnumerateDirectories("*", SearchOption.AllDirectories);
                            foreach (var f in AgentFiltred)
                            {
                                var fichiers = f.EnumerateFiles("*", SearchOption.TopDirectoryOnly).Where((x => (x.Name.Contains("-" + tel + "]") || x.Name.Contains("[" + tel + "]"))));
                                foreach (var file in fichiers)
                                {

                                    a.filesTestsInfo.Add(file);
                                    if (file != null)
                                    {
                                        i = i + 1;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            DateTime stopTime = DateTime.Now;
            int minutes = (stopTime - startTime).Minutes;
            int secondes = (stopTime - startTime).Seconds;
            a.minutes = minutes;
            a.secondes = secondes;
            a.nbresEnrefgistrements = i;//a.filesTestsInfo.count

            if (Request.IsAjaxRequest())
            {
                if ((i == 0))
                {
                    return PartialView("FileError", a);
                }
                else if ((i != 0))
                {
                    return PartialView("DirectoryPartialViewWithoutAgent", a);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return RedirectToAction("Details");
            }
        }

        [HttpGet]
        public ActionResult FindRechercheParNumeroFicheV5(String indice, String roleName, string roleKey, string start, string end)
        {
            int i = 0;
            var a = new DirectoryViewModel();
            a.roleName = roleKey;
            DateTime creationDate = DateTime.Now;
            DateTime EndDate = DateTime.Now;
            if (start != null && end != null)
            {
                creationDate = DateTime.Parse(start);
                EndDate = DateTime.Parse(end + " 23:00:00");
            }
            else
            {
                var b = new DirectoryViewModel();
                return PartialView("SelectDateErreur", b);
            }
            var test = new DirectoryInfo(@"\\10.9.6.25\Enregistrements\hermes_p\Files\" + roleName + "\\RECORDS");
            var tests2 = test.EnumerateDirectories("*", SearchOption.TopDirectoryOnly);
            DateTime startTime = DateTime.Now;
            foreach (var dir in tests2)
            {
                var YearFiltred = dir.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(y => y.Name == (creationDate.Year).ToString() && y.Name == (EndDate.Year).ToString());
                foreach (var yearitem in YearFiltred)
                {
                    var MonthFiltred = yearitem.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(x => (Int32.Parse(x.Name) >= creationDate.Month) && (Int32.Parse(x.Name) <= EndDate.Month));
                    foreach (var monthitem in MonthFiltred)
                    {
                        var DayFiltred = monthitem.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(x => (x.LastWriteTime >= creationDate) && (x.LastWriteTime <= EndDate));
                        foreach (var item in DayFiltred)
                        {
                            var AgentFiltred = item.EnumerateDirectories("*", SearchOption.AllDirectories);
                            foreach (var f in AgentFiltred)
                            {
                                var fichiers = f.EnumerateFiles("*", SearchOption.TopDirectoryOnly).Where((x => x.Name.Contains("[IHN-" + indice + "]")));
                                foreach (var file in fichiers)
                                {

                                    a.filesTestsInfo.Add(file);
                                    if (file != null)
                                    {
                                        i = i + 1;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            DateTime stopTime = DateTime.Now;
            int minutes = (stopTime - startTime).Minutes;
            int secondes = (stopTime - startTime).Seconds;
            a.minutes = minutes;
            a.secondes = secondes;
            a.nbresEnrefgistrements = i;//a.filesTestsInfo.count

            if (Request.IsAjaxRequest())
            {
                if ((i == 0))
                {
                    return PartialView("FileError", a);
                }
                else if ((i != 0))
                {
                    return PartialView("DirectoryPartialViewWithoutAgent", a);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return RedirectToAction("Details");
            }
        }

        [HttpGet]
        public ActionResult FindAllCasesV5(string compagne, string start, string end, String[] pseudoNames, string recherche, string idhermes, string roleName, string roleKey)
        {
            int i = 0;
            var a = new DirectoryViewModel();
            a.roleName = roleKey;
            DateTime startTime = DateTime.Now;
            DateTime creationDate = DateTime.Now;
            DateTime EndDate = DateTime.Now;
            if (start != null && end != null)
            {
                creationDate = DateTime.Parse(start);
                EndDate = DateTime.Parse(end + " 23:00:00");
            }
            else
            {
                var b = new DirectoryViewModel();
                return PartialView("SelectDateErreur", b);
            }
            //méthode de recherche spécifique: on utilise OU entre les autres critères que campagne
            //si la campagne est vide
            if (compagne.Equals(""))
            {
                var test = new DirectoryInfo(@"\\10.9.6.25\Enregistrements\hermes_p\Files\" + roleName + "\\RECORDS");
                var tests2 = test.EnumerateDirectories("*", SearchOption.TopDirectoryOnly);
                if (pseudoNames != null && idhermes != "")
                {
                    foreach (var ps in pseudoNames)
                    {
                        Employee emp = serviceEmployee.getByPseudoName(ps);
                        string IdHermes = (emp.IdHermes).ToString();
                        foreach (var dir in tests2)
                        {
                            var YearFiltred = dir.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(y => y.Name == (creationDate.Year).ToString() && y.Name == (EndDate.Year).ToString());
                            foreach (var yearitem in YearFiltred)
                            {
                                var MonthFiltred = yearitem.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(x => (Int32.Parse(x.Name) >= creationDate.Month) && (Int32.Parse(x.Name) <= EndDate.Month));
                                foreach (var monthitem in MonthFiltred)
                                {
                                    var DayFiltred = monthitem.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(x => (x.LastWriteTime >= creationDate) && (x.LastWriteTime <= EndDate));
                                    foreach (var item in DayFiltred)
                                    {
                                        var AgentFiltred = item.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(x => (x.Name == IdHermes) || (x.Name == idhermes));
                                        foreach (var f in AgentFiltred)
                                        {
                                            var fichiers = f.EnumerateFiles("*", SearchOption.TopDirectoryOnly);
                                            foreach (var file in fichiers)
                                            {

                                                a.filesTestsInfo.Add(file);
                                                if (file != null)
                                                {
                                                    i = i + 1;
                                                    TempData["nbEnr"] = i;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (pseudoNames == null && idhermes != "")
                {
                    foreach (var dir in tests2)
                    {
                        var YearFiltred = dir.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(y => y.Name == (creationDate.Year).ToString() && y.Name == (EndDate.Year).ToString());
                        foreach (var yearitem in YearFiltred)
                        {
                            var MonthFiltred = yearitem.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(x => (Int32.Parse(x.Name) >= creationDate.Month) && (Int32.Parse(x.Name) <= EndDate.Month));
                            foreach (var monthitem in MonthFiltred)
                            {
                                var DayFiltred = monthitem.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(x => (x.LastWriteTime >= creationDate) && (x.LastWriteTime <= EndDate));
                                foreach (var item in DayFiltred)
                                {
                                    var AgentFiltred = item.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(x => (x.Name == idhermes));
                                    foreach (var f in AgentFiltred)
                                    {
                                        var fichiers = f.EnumerateFiles("*", SearchOption.TopDirectoryOnly);
                                        foreach (var file in fichiers)
                                        {

                                            a.filesTestsInfo.Add(file);
                                            if (file != null)
                                            {
                                                i = i + 1;
                                                TempData["nbEnr"] = i;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (pseudoNames != null && idhermes == "")
                {
                    foreach (var ps in pseudoNames)
                    {
                        Employee emp = serviceEmployee.getByPseudoName(ps);
                        string IdHermes = (emp.IdHermes).ToString();
                        foreach (var dir in tests2)
                        {
                            var YearFiltred = dir.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(y => y.Name == (creationDate.Year).ToString() && y.Name == (EndDate.Year).ToString());
                            foreach (var yearitem in YearFiltred)
                            {
                                var MonthFiltred = yearitem.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(x => (Int32.Parse(x.Name) >= creationDate.Month) && (Int32.Parse(x.Name) <= EndDate.Month));
                                foreach (var monthitem in MonthFiltred)
                                {
                                    var DayFiltred = monthitem.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(x => (x.LastWriteTime >= creationDate) && (x.LastWriteTime <= EndDate));
                                    foreach (var item in DayFiltred)
                                    {
                                        var AgentFiltred = item.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(x => (x.Name == IdHermes));
                                        foreach (var f in AgentFiltred)
                                        {
                                            var fichiers = f.EnumerateFiles("*", SearchOption.TopDirectoryOnly);
                                            foreach (var file in fichiers)
                                            {
                                                a.filesTestsInfo.Add(file);
                                                if (file != null)
                                                {
                                                    i = i + 1;
                                                    TempData["nbEnr"] = i;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }

            // Si la campagne n'est pas vide
            if (!compagne.Equals(""))
            {
                var test = new DirectoryInfo(@"\\10.9.6.25\Enregistrements\hermes_p\Files\" + roleName + "\\RECORDS\\" + compagne);
                var YearFiltred = test.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(y => y.Name == (creationDate.Year).ToString() && y.Name == (EndDate.Year).ToString());
                if (pseudoNames != null && idhermes != "")
                {
                    foreach (var ps in pseudoNames)
                    {
                        Employee emp = serviceEmployee.getByPseudoName(ps);
                        string IdHermes = (emp.IdHermes).ToString();


                        foreach (var yearitem in YearFiltred)
                        {
                            var MonthFiltred = yearitem.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(x => (Int32.Parse(x.Name) >= creationDate.Month) && (Int32.Parse(x.Name) <= EndDate.Month));
                            foreach (var monthitem in MonthFiltred)
                            {
                                var DayFiltred = monthitem.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(x => (x.LastWriteTime >= creationDate) && (x.LastWriteTime <= EndDate));
                                foreach (var item in DayFiltred)
                                {
                                    var AgentFiltred = item.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(x => (x.Name == IdHermes) || (x.Name == idhermes));
                                    foreach (var f in AgentFiltred)
                                    {
                                        var fichiers = f.EnumerateFiles("*", SearchOption.TopDirectoryOnly);
                                        foreach (var file in fichiers)
                                        {

                                            a.filesTestsInfo.Add(file);
                                            if (file != null)
                                            {
                                                i = i + 1;
                                                TempData["nbEnr"] = i;

                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (pseudoNames == null && idhermes != "")
                {
                    foreach (var yearitem in YearFiltred)
                    {
                        var MonthFiltred = yearitem.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(x => (Int32.Parse(x.Name) >= creationDate.Month) && (Int32.Parse(x.Name) <= EndDate.Month));
                        foreach (var monthitem in MonthFiltred)
                        {
                            var DayFiltred = monthitem.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(x => (x.LastWriteTime >= creationDate) && (x.LastWriteTime <= EndDate));
                            foreach (var item in DayFiltred)
                            {
                                var AgentFiltred = item.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(x => (x.Name == idhermes));
                                foreach (var f in AgentFiltred)
                                {
                                    var fichiers = f.EnumerateFiles("*", SearchOption.TopDirectoryOnly);
                                    foreach (var file in fichiers)
                                    {

                                        a.filesTestsInfo.Add(file);
                                        if (file != null)
                                        {
                                            i = i + 1;
                                            TempData["nbEnr"] = i;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (pseudoNames != null && idhermes == "")
                {
                    foreach (var ps in pseudoNames)
                    {
                        Employee emp = serviceEmployee.getByPseudoName(ps);
                        string IdHermes = (emp.IdHermes).ToString();
                        foreach (var yearitem in YearFiltred)
                        {
                            var MonthFiltred = yearitem.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(x => (Int32.Parse(x.Name) >= creationDate.Month) && (Int32.Parse(x.Name) <= EndDate.Month));
                            foreach (var monthitem in MonthFiltred)
                            {
                                var DayFiltred = monthitem.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(x => (x.LastWriteTime >= creationDate) && (x.LastWriteTime <= EndDate));
                                foreach (var item in DayFiltred)
                                {
                                    var AgentFiltred = item.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(x => (x.Name == IdHermes));
                                    foreach (var f in AgentFiltred)
                                    {
                                        var fichiers = f.EnumerateFiles("*", SearchOption.TopDirectoryOnly);
                                        foreach (var file in fichiers)
                                        {
                                            a.filesTestsInfo.Add(file);
                                            if (file != null)
                                            {
                                                i = i + 1;
                                                TempData["nbEnr"] = i;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (pseudoNames == null && idhermes == "")
                {
                    foreach (var yearitem in YearFiltred)
                    {
                        var MonthFiltred = yearitem.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(x => (Int32.Parse(x.Name) >= creationDate.Month) && (Int32.Parse(x.Name) <= EndDate.Month));
                        foreach (var monthitem in MonthFiltred)
                        {
                            var DayFiltred = monthitem.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(x => (x.LastWriteTime >= creationDate) && (x.LastWriteTime <= EndDate));
                            foreach (var item in DayFiltred)
                            {
                                var AgentFiltred = item.EnumerateDirectories("*", SearchOption.TopDirectoryOnly);
                                foreach (var f in AgentFiltred)
                                {
                                    var fichiers = f.EnumerateFiles("*", SearchOption.AllDirectories);
                                    foreach (var file in fichiers)
                                    {

                                        a.filesTestsInfo.Add(file);
                                        if (file != null)
                                        {
                                            i = i + 1;
                                            TempData["nbEnr"] = i;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }

            //méthode de recherche générale
            if (recherche != "" && compagne == "" && idhermes == "" && pseudoNames == null)
            {
                var test = new DirectoryInfo(@"\\10.9.6.25\Enregistrements\hermes_p\Files\" + roleName + "\\RECORDS");
                var tests2 = test.EnumerateDirectories("*", SearchOption.TopDirectoryOnly);
                foreach (var dir in tests2)
                {
                    var YearFiltred = dir.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(y => y.Name == (creationDate.Year).ToString() && y.Name == (EndDate.Year).ToString());
                    foreach (var yearitem in YearFiltred)
                    {
                        var MonthFiltred = yearitem.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(x => (Int32.Parse(x.Name) >= creationDate.Month) && (Int32.Parse(x.Name) <= EndDate.Month));
                        foreach (var monthitem in MonthFiltred)
                        {
                            var DayFiltred = monthitem.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(x => (x.LastWriteTime >= creationDate) && (x.LastWriteTime <= EndDate));
                            foreach (var item in DayFiltred)
                            {
                                var AgentFiltred = item.EnumerateDirectories("*", SearchOption.TopDirectoryOnly);
                                foreach (var f in AgentFiltred)
                                {
                                    var fichiers = f.EnumerateFiles("*", SearchOption.TopDirectoryOnly).Where(x => x.Name.Contains(recherche));
                                    foreach (var file in fichiers)
                                    {

                                        a.filesTestsInfo.Add(file);
                                        if (file != null)
                                        {
                                            i = i + 1;
                                            TempData["nbEnr"] = i;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }
            DateTime stopTime = DateTime.Now;
            int minutes = (stopTime - startTime).Minutes;
            int secondes = (stopTime - startTime).Seconds;
            a.minutes = minutes;
            a.secondes = secondes;
            a.nbresEnrefgistrements = i;


            if (Request.IsAjaxRequest())
            {
                if ((i == 0))
                {
                    return PartialView("FileError", a);

                }
                else if ((i != 0))
                {
                    return PartialView("DirectoryPartialViewWithoutAgent", a);

                }
                else
                {
                    return RedirectToAction("Index");
                }
            }

            else
            {
                return RedirectToAction("Details");
            }
        }

        #endregion
    }
}
