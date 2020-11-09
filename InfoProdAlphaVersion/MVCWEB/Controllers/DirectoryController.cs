using Domain.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using MVCWEB.Models;
using NAudio.Wave;
using Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Web;
using System.Web.Mvc;

namespace MVCWEB.Controllers
{
    [Authorize(Roles = "Qualité,Manager,Agent,Agent Qualité,Agent Qualité_CustomerService,Agent Qualité_Diffusion,Agent Qualité_AchatPublic,SuperManager,Agent_CustomerService, Agent_SAMRC,Agent_QR,Agent_KLMO,Agent_PRV,Agent Qualité_PRV")]

    public class DirectoryController : Controller
    {
        // GET: Directory
        #region memberVariablesClass
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        IUtilisateurService serviceUser;
        IEmployeeService serviceEmployee;
        IGroupeEmployeeService serviceGroupeEmp;
        //static IWavePlayer playerTest;
        IWavePlayer playerTest = null;
        #endregion


        #region Constructor
        public DirectoryController()
        {
            serviceUser = new UtilisateurService();
            serviceEmployee = new EmployeeService();
            serviceGroupeEmp = new GroupesEmployeService();

        }
        public DirectoryController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
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
        #region affichage

        [Authorize(Roles = "Qualité")]
        public ActionResult Index()
        {
            //List<String> tests = new List<String>();
            //List<String> testsFiles = new List<String>();
            var a = new DirectoryViewModel();

            var tests = new DirectoryInfo(@"\\10.9.6.25\Enregistrements\DIFFUSION");
            var files = tests.EnumerateDirectories(
            "*.*",
             SearchOption.TopDirectoryOnly); //search criteria for example All Directorys
            foreach (var file in files)
            {
                // Display file path.
                a.topDirectorieDirectories.Add(new SelectListItem { Text = file.Name, Value = file.Name });

                //Console.WriteLine(file);
            }
            //  ViewBag.files = tests;
            //  var filesTests = Directory.EnumerateFiles(@"\\10.9.6.25\Enregistrements\AUTO",
            //"*.*",
            //SearchOption.TopDirectoryOnly); //search criteria for example All Directorys
            //  foreach (string file in filesTests)
            //  {
            //      // Display file path.
            //      testsFiles.Add(file);

            //      Console.WriteLine(file);
            //  }
            //  ViewBag.testsFiles = testsFiles;
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

            //string type = form["typeGenerator"].ToString();
            var logins = serviceGroupeEmp.getListEmployeeByGroupe(user.Id);
            var us = logins.Select(o => o).Distinct().ToList();
            var uspseudoNames = logins.Select(o => o.pseudoName).Distinct().ToList();
            var ordredByPseudoNames = us.OrderBy(e => e.pseudoName).ToList();
            foreach (var test in ordredByPseudoNames)
            {
                if (!test.UserName.Equals(user.UserName))
                {
                    a.utilisateurs.Add(new SelectListItem { Text = test.UserName, Value = test.UserName });
                    a.utilisateursPseudonames.Add(new SelectListItem { Text = test.pseudoName, Value = test.pseudoName });
                    a.IdHermeses.Add(new SelectListItem { Text = "" + test.IdHermes, Value = "" + test.IdHermes });
                }

            }
            a.empId = "" + user.Id;

            a.userName = user.UserName;
            a.pseudoNameEmp = user.pseudoName;
            if (user.Content != null)
            {
                String strbase64 = Convert.ToBase64String(user.Content);
                String Url = "data:" + user.ContentType + ";base64," + strbase64;
                ViewBag.url = Url;
                a.Url = Url;

            }

            //var todayFiles = Directory.GetFiles("path_to_directory")
            //     .Where(x => new FileInfo(x).CreationTime.Date == DateTime.Today.Date);
            return View(a);
        }
        [Authorize(Roles = ("Qualité,Manager,SuperManager"))]

        public ActionResult CompagneParDate()
        {
            var a = new DirectoryViewModel();
            var files = Directory.EnumerateDirectories(@"\\10.9.6.25\Enregistrements\DIFFUSION",
                       "*.*",
                        SearchOption.TopDirectoryOnly); //search criteria for example All Directorys
            foreach (string file in files)
            {
                a.topDirectorieDirectories.Add(new SelectListItem { Text = file, Value = file });

            }
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            a.userName = user.UserName;
            a.pseudoNameEmp = user.pseudoName;
            if (user.Content != null)
            {
                String strbase64 = Convert.ToBase64String(user.Content);
                String Url = "data:" + user.ContentType + ";base64," + strbase64;
                ViewBag.url = Url;
                a.Url = Url;

            }
            return View(a);
        }


        [Authorize(Roles = ("Manager , Qualité,SuperManager"))]
        [HttpPost]
        public ActionResult GetChildrenofDirectory(string children)
        {
            var a = new DirectoryViewModel();
            var tests = new DirectoryInfo(@children);
            var files = tests.EnumerateDirectories("*.*",
            SearchOption.TopDirectoryOnly); //search criteria for example All Directorys
            foreach (var file in files)
            {
                // Display file path.
                a.topDirectorieDirectories.Add(new SelectListItem { Text = file.Name, Value = file.Name });

                //Console.WriteLine(file);
            }
            //if (Request.IsAjaxRequest())
            //{
            return PartialView("GetChildren", a);

            //}
            //return View("Index");
        }
        [Authorize(Roles = "Manager,SuperManager")]

        public ActionResult Index2()
        {
            //List<String> tests = new List<String>();
            //List<String> testsFiles = new List<String>();
            var a = new DirectoryViewModel();

            var tests = new DirectoryInfo(@"\\10.9.6.25\Enregistrements\DIFFUSION");
            var files = tests.EnumerateDirectories(
            "*.*",
             SearchOption.TopDirectoryOnly); //search criteria for example All Directorys
            foreach (var file in files)
            {
                // Display file path.
                a.topDirectorieDirectories.Add(new SelectListItem { Text = file.Name, Value = file.Name });

                //Console.WriteLine(file);
            }
            //  ViewBag.files = tests;
            //  var filesTests = Directory.EnumerateFiles(@"\\10.9.6.25\Enregistrements\AUTO",
            //"*.*",
            //SearchOption.TopDirectoryOnly); //search criteria for example All Directorys
            //  foreach (string file in filesTests)
            //  {
            //      // Display file path.
            //      testsFiles.Add(file);

            //      Console.WriteLine(file);
            //  }
            //  ViewBag.testsFiles = testsFiles;
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

            //string type = form["typeGenerator"].ToString();
            var logins = serviceGroupeEmp.getListEmployeeByGroupe(user.Id);
            // (item.Roles.Any(r => r.UserId == item.Id && r.RoleId == 3))
            var us = logins.Select(o => o).Distinct().ToList();
            var uspseudoNames = logins.Select(o => o.pseudoName).Distinct().ToList();
            var ordredByPseudoNames = us.OrderBy(e => e.pseudoName).ToList();
            foreach (var test in ordredByPseudoNames)
            {
                if (!test.UserName.Equals(user.UserName) && test.Roles.Any(r => r.UserId == test.Id && r.RoleId == 3))
                {
                    a.utilisateurs.Add(new SelectListItem { Text = test.UserName, Value = test.UserName });
                    a.utilisateursPseudonames.Add(new SelectListItem { Text = test.pseudoName, Value = test.pseudoName });
                    a.IdHermeses.Add(new SelectListItem { Text = "" + test.IdHermes, Value = "" + test.IdHermes });
                }

            }
            a.empId = "" + user.Id;

            a.userName = user.UserName;
            a.pseudoNameEmp = user.pseudoName;
            if (user.Content != null)
            {
                String strbase64 = Convert.ToBase64String(user.Content);
                String Url = "data:" + user.ContentType + ";base64," + strbase64;
                ViewBag.url = Url;
                a.Url = Url;

            }

            //var todayFiles = Directory.GetFiles("path_to_directory")
            //     .Where(x => new FileInfo(x).CreationTime.Date == DateTime.Today.Date);
            return View(a);
        }
        [HttpGet]
        [Authorize(Roles = "Qualité,Manager,SuperManager")]
        public ActionResult Diffusion()
        {
            List<String> tests = new List<String>();
            List<String> testsFiles = new List<String>();
            var a = new DirectoryViewModel();

            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

            //string type = form["typeGenerator"].ToString();
            var logins = serviceGroupeEmp.getListEmployeeByGroupe(user.Id);
            var us = logins.Select(o => o).Distinct().ToList();
            var uspseudoNames = logins.Select(o => o.pseudoName).Distinct().ToList();
            var ordredNames = us.OrderBy(e => e.pseudoName).ToList();
            var ordredByIdHermes = us.OrderBy(e => e.IdHermes).ToList();
            foreach (var test in ordredNames)
            {
                if (!test.UserName.Equals(user.UserName))
                {
                    a.utilisateurs.Add(new SelectListItem { Text = test.UserName, Value = test.UserName });
                    a.utilisateursPseudonames.Add(new SelectListItem { Text = test.pseudoName, Value = test.pseudoName });
                }

            }
            foreach (var test in ordredByIdHermes)
            {
                if (!test.UserName.Equals(user.UserName))
                {
                    a.IdHermeses.Add(new SelectListItem { Text = "" + test.IdHermes, Value = "" + test.IdHermes });

                }

            }
            a.empId = "" + user.Id;

            a.userName = user.UserName;
            a.pseudoNameEmp = user.pseudoName;
            if (user.Content != null)
            {
                String strbase64 = Convert.ToBase64String(user.Content);
                String Url = "data:" + user.ContentType + ";base64," + strbase64;
                ViewBag.url = Url;
                a.Url = Url;
            }
            return View(a);
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
        [Authorize(Roles = "Qualité,Manager,SuperManager")]
        public ActionResult DiffusionManager()
        {
            List<String> tests = new List<String>();
            List<String> testsFiles = new List<String>();
            var a = new DirectoryViewModel();

            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

            //string type = form["typeGenerator"].ToString();
            var logins = serviceGroupeEmp.getListEmployeeByGroupe(user.Id);
            var us = logins.Select(o => o).Distinct().ToList();
            var uspseudoNames = logins.Select(o => o.pseudoName).Distinct().ToList();
            var ordredByPseudoNames = us.OrderBy(e => e.pseudoName).ToList();
            foreach (var test in ordredByPseudoNames)
            {
                if (!test.UserName.Equals(user.UserName) && test.Roles.Any(r => r.UserId == test.Id && r.RoleId == 3))
                {
                    a.utilisateurs.Add(new SelectListItem { Text = test.UserName, Value = test.UserName });
                    a.utilisateursPseudonames.Add(new SelectListItem { Text = test.pseudoName, Value = test.pseudoName });
                    a.IdHermeses.Add(new SelectListItem { Text = "" + test.IdHermes, Value = "" + test.IdHermes });
                }
            }
            a.empId = "" + user.Id;

            a.userName = user.UserName;
            a.pseudoNameEmp = user.pseudoName;
            if (user.Content != null)
            {
                String strbase64 = Convert.ToBase64String(user.Content);
                String Url = "data:" + user.ContentType + ";base64," + strbase64;
                ViewBag.url = Url;
                a.Url = Url;

            }

            return View(a);

        }
        [HttpGet]
        [Authorize(Roles = "Qualité")]
        public ActionResult TestDiffusion()
        {
            List<String> tests = new List<String>();
            List<String> testsFiles = new List<String>();
            var a = new DirectoryViewModel();

            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

            //string type = form["typeGenerator"].ToString();
            var logins = serviceGroupeEmp.getListEmployeeByGroupe(user.Id);
            var us = logins.Select(o => o).Distinct().ToList();
            var uspseudoNames = logins.Select(o => o.pseudoName).Distinct().ToList();
            foreach (var test in us)
            {
                a.utilisateurs.Add(new SelectListItem { Text = test.UserName, Value = test.UserName });
                a.utilisateursPseudonames.Add(new SelectListItem { Text = test.pseudoName, Value = test.pseudoName });
                a.IdHermeses.Add(new SelectListItem { Text = "" + test.IdHermes, Value = "" + test.IdHermes });
            }
            a.empId = "" + user.Id;

            a.userName = user.UserName;
            a.pseudoNameEmp = user.pseudoName;
            if (user.Content != null)
            {
                String strbase64 = Convert.ToBase64String(user.Content);
                String Url = "data:" + user.ContentType + ";base64," + strbase64;
                ViewBag.url = Url;
                a.Url = Url;

            }

            // Session["nombreDeFichiers"] = 0;
            //ViewBag.message = "aaaa";
            return View(a);
        }
        [HttpGet]
        [Authorize(Roles = "Manager,SuperManager")]
        public ActionResult TestDiffusion2()
        {
            List<String> tests = new List<String>();
            List<String> testsFiles = new List<String>();
            var a = new DirectoryViewModel();

            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));

            //string type = form["typeGenerator"].ToString();
            var logins = serviceGroupeEmp.getListEmployeeByGroupe(user.Id);
            var us = logins.Select(o => o).Distinct().ToList();
            var uspseudoNames = logins.Select(o => o.pseudoName).Distinct().ToList();
            foreach (var test in us)
            {
                a.utilisateurs.Add(new SelectListItem { Text = test.UserName, Value = test.UserName });
                a.utilisateursPseudonames.Add(new SelectListItem { Text = test.pseudoName, Value = test.pseudoName });
                a.IdHermeses.Add(new SelectListItem { Text = "" + test.IdHermes, Value = "" + test.IdHermes });
            }
            a.empId = "" + user.Id;

            a.userName = user.UserName;
            a.pseudoNameEmp = user.pseudoName;
            if (user.Content != null)
            {
                String strbase64 = Convert.ToBase64String(user.Content);
                String Url = "data:" + user.ContentType + ";base64," + strbase64;
                ViewBag.url = Url;
                a.Url = Url;

            }
            // Session["nombreDeFichiers"] = 0;
            //ViewBag.message = "aaaa";
            return View(a);
        }
        // GET: Directory/Details/5
        [Authorize(Roles = "Qualité")]

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
            //var todayFiles = Directory.GetFiles("path_to_directory")
            //     .Where(x => new FileInfo(x).CreationTime.Date == DateTime.Today.Date);
            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            var a = new DirectoryViewModel();

            //string type = form["typeGenerator"].ToString();
            var logins = serviceGroupeEmp.getListEmployeeByGroupe(user.Id);
            var us = logins.Select(o => o.UserName).Distinct().ToList();
            foreach (var test in us)
            {
                a.utilisateurs.Add(new SelectListItem { Text = test, Value = test });
            }

            //var todayFiles = Directory.GetFiles("path_to_directory")
            //     .Where(x => new FileInfo(x).CreationTime.Date == DateTime.Today.Date);
            return View(a);
        }
        #endregion
        #region CustomEnregistrement
        [HttpPost]
        [Authorize(Roles = "Qualité,Manager,SuperManager")]

        public ActionResult Find(String date, String utilisateurs, String pseudoNames)
        {

            var a = new DirectoryViewModel();
            a.pseudoName = pseudoNames;
            a.agentName = pseudoNames;
            // ViewBag.agentName = pseudoNames;
            List<String> tests = new List<string>();
            string v = date.Replace("/", "_");
            string d = pseudoNames.Replace(" ", "-").Replace("é", "Ã©");
            string dd = pseudoNames.Replace(" ", "--").Replace("é", "Ã©");
            if (Directory.Exists(@"\\10.9.6.25\Enregistrements\DIFFUSION\" + utilisateurs + "\\" + v) == true)
            {

                //var files = Directory.EnumerateDirectories(@utilisateurs + "\\" + v,
                //"*.*",
                //SearchOption.AllDirectories).Where(x => new FileInfo(x).Name.Contains(v) && new FileInfo(x).Name.Contains(d)); //search criteria for example All Directorys
                //DateTime start = DateTime.Now;
                //foreach (var file in files)
                //{
                //    // Display file path.
                //    a.files.Add(file);

                //    //Console.WriteLine(file);
                //}
                DateTime start = DateTime.Now;

                var test = new DirectoryInfo(@"\\10.9.6.25\Enregistrements\DIFFUSION\" + utilisateurs + "\\" + v);
                var tests2 = test.EnumerateFiles().Where(x => x.Name.Contains(v) || x.Name.ToLower().Contains(d.ToLower()) || x.Name.ToLower().Contains(dd.ToLower()));
                var filesTests = Directory.EnumerateFiles(@"\\10.9.6.25\Enregistrements\DIFFUSION\" + utilisateurs + "\\" + v,
                "*.*",
                SearchOption.AllDirectories).Where(x => new FileInfo(x).Name.Contains(v) || new FileInfo(x).Name.ToLower().Contains(d.ToLower()) || new FileInfo(x).Name.ToLower().Contains(dd.ToLower())); //search criteria for example All Directorys
                foreach (var file in tests2)
                {
                    // Display file path.

                    a.filesTestsInfo.Add(file);
                    tests.Add(file.Name);
                    //addfiles

                }
                DateTime stop = DateTime.Now;
                int minutes = (stop - start).Minutes;
                int secondes = (stop - start).Seconds;
                //Session["minutes"] = minutes;
                //Session["secondes"] = secondes;
                a.minutes = minutes;
                a.secondes = secondes;
                a.nbresEnrefgistrements = a.filesTestsInfo.Count;
            }
            //Session["nombreDeFichiers"] = a.filesTests.Count;

            //var todayFiles = Directory.GetFiles("path_to_directory")
            //     .Where(x => new FileInfo(x).CreationTime.Date == DateTime.Today.Date);

            if (Request.IsAjaxRequest())
            {
                if (Directory.Exists(@"\\10.9.6.25\Enregistrements\DIFFUSION\" + utilisateurs + "\\" + v) == false)
                {
                    return PartialView("DirectoryError", a);

                }
                else if ((Directory.Exists(@"\\10.9.6.25\Enregistrements\DIFFUSION\" + utilisateurs + "\\" + v) == true) && (tests.Count == 0))
                {
                    return PartialView("FileError", a);

                }
                else if ((Directory.Exists(@"\\10.9.6.25\Enregistrements\DIFFUSION\" + utilisateurs + "\\" + v) == true) && (tests.Count != 0))
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

        public ActionResult Find2(String date, String utilisateurs, String pseudoNames)
        {

            var a = new DirectoryViewModel();
            a.pseudoName = pseudoNames;
            a.agentName = pseudoNames;
            // ViewBag.agentName = pseudoNames;
            List<String> tests = new List<string>();
            string v = date.Replace("/", "_");
            string d = pseudoNames.Replace(" ", "-").Replace("é", "Ã©");
            string dd = pseudoNames.Replace(" ", "--").Replace("é", "Ã©");
            if (Directory.Exists(@"\\10.9.6.25\Enregistrements\DIFFUSION\" + utilisateurs + "\\" + v) == true)
            {

                //var files = Directory.EnumerateDirectories(@utilisateurs + "\\" + v,
                //"*.*",
                //SearchOption.AllDirectories).Where(x => new FileInfo(x).Name.Contains(v) && new FileInfo(x).Name.Contains(d)); //search criteria for example All Directorys
                //DateTime start = DateTime.Now;
                //foreach (var file in files)
                //{
                //    // Display file path.
                //    a.files.Add(file);

                //    //Console.WriteLine(file);
                //}
                DateTime start = DateTime.Now;

                var test = new DirectoryInfo(@"\\10.9.6.25\Enregistrements\DIFFUSION\" + utilisateurs + "\\" + v);
                var tests2 = test.EnumerateFiles().Where(x => x.Name.Contains(v) || x.Name.ToLower().Contains(d.ToLower()) || x.Name.ToLower().Contains(dd.ToLower()));
                var filesTests = Directory.EnumerateFiles(@"\\10.9.6.25\Enregistrements\DIFFUSION\" + utilisateurs + "\\" + v,
                "*.*",
                SearchOption.AllDirectories).Where(x => new FileInfo(x).Name.Contains(v) || new FileInfo(x).Name.ToLower().Contains(d.ToLower()) || new FileInfo(x).Name.ToLower().Contains(dd.ToLower())); //search criteria for example All Directorys
                foreach (var file in tests2)
                {
                    // Display file path.

                    a.filesTestsInfo.Add(file);
                    tests.Add(file.Name);
                    //addfiles

                }
                DateTime stop = DateTime.Now;
                int minutes = (stop - start).Minutes;
                int secondes = (stop - start).Seconds;
                //Session["minutes"] = minutes;
                //Session["secondes"] = secondes;
                a.minutes = minutes;
                a.secondes = secondes;
                a.nbresEnrefgistrements = a.filesTestsInfo.Count;
            }
            //Session["nombreDeFichiers"] = a.filesTests.Count;

            //var todayFiles = Directory.GetFiles("path_to_directory")
            //     .Where(x => new FileInfo(x).CreationTime.Date == DateTime.Today.Date);

            if (Request.IsAjaxRequest())
            {
                if (Directory.Exists(@"\\10.9.6.25\Enregistrements\DIFFUSION\" + utilisateurs + "\\" + v) == false)
                {
                    return PartialView("DirectoryError", a);

                }
                else if ((Directory.Exists(@"\\10.9.6.25\Enregistrements\DIFFUSION\" + utilisateurs + "\\" + v) == true) && (tests.Count == 0))
                {
                    return PartialView("FileError", a);

                }
                else if ((Directory.Exists(@"\\10.9.6.25\Enregistrements\DIFFUSION\" + utilisateurs + "\\" + v) == true) && (tests.Count != 0))
                {
                    return PartialView("DirectoryPartialView2", a);

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
        [HttpPost]
        [Authorize(Roles = "Qualité,Manager,SuperManager")]

        public ActionResult FindCompagneDate(String date, String utilisateurs)
        {

            var a = new DirectoryViewModel();
            List<String> tests = new List<string>();
            string v = date.Replace("/", "_");

            if (Directory.Exists(@utilisateurs + "\\" + v) == true)
            {

                //var files = Directory.EnumerateDirectories(@utilisateurs + "\\" + v,
                //"*.*",
                //SearchOption.AllDirectories).Where(x => new FileInfo(x).Name.Contains(v) && new FileInfo(x).Name.Contains(d)); //search criteria for example All Directorys
                //DateTime start = DateTime.Now;
                //foreach (var file in files)
                //{
                //    // Display file path.
                //    a.files.Add(file);

                //    //Console.WriteLine(file);
                //}
                DateTime start = DateTime.Now;

                var test = new DirectoryInfo(@utilisateurs + "\\" + v);
                var tests2 = test.EnumerateFiles();

                foreach (var file in tests2)
                {
                    // Display file path.

                    a.filesTestsInfo.Add(file);
                    tests.Add(file.Name);
                    //addfiles

                }
                DateTime stop = DateTime.Now;
                int minutes = (stop - start).Minutes;
                int secondes = (stop - start).Seconds;
                //Session["minutes"] = minutes;
                //Session["secondes"] = secondes;
                a.minutes = minutes;
                a.secondes = secondes;
                a.nbresEnrefgistrements = a.filesTestsInfo.Count;
            }
            //Session["nombreDeFichiers"] = a.filesTests.Count;

            //var todayFiles = Directory.GetFiles("path_to_directory")
            //     .Where(x => new FileInfo(x).CreationTime.Date == DateTime.Today.Date);

            if (Request.IsAjaxRequest())
            {
                if (Directory.Exists(@utilisateurs + "\\" + v) == false)
                {
                    return PartialView("DirectoryError", a);

                }
                else if ((Directory.Exists(@utilisateurs + "\\" + v) == true) && (tests.Count == 0))
                {
                    return PartialView("FileError", a);

                }
                else if ((Directory.Exists(@utilisateurs + "\\" + v) == true) && (tests.Count != 0))
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
        [Authorize(Roles = "Qualité,Manager,SuperManager")]

        public ActionResult FindDiffusion(String utilisateurs, String pseudoNames, String start, String end)
        {

            int i = 0;
            var a = new DirectoryViewModel();
            a.pseudoName = pseudoNames;
            a.agentName = pseudoNames;
            //List<String> tests = new List<string>();
            //string v = date.Replace("/", "_");
            //     String pseudoNamesCopy=
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

            var test = new DirectoryInfo(@"\\10.9.6.25\Enregistrements\DIFFUSION");
            var tests2 = test.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(y => y.LastWriteTime >= creationDate);

            DateTime startTime = DateTime.Now;


            foreach (var file in tests2)
            {

                var dossier = new DirectoryInfo(@file.FullName);
                var Direcs = dossier.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(y => y.LastWriteTime >= creationDate);

                foreach (var direct in Direcs)
                {

                    var fichiers = direct.EnumerateFiles("*", SearchOption.TopDirectoryOnly).Where((x => ((x.Name.Contains("TV." + utilisateurs + "-")) && ((x.Name.ToLower().Contains(d.ToLower())) || (x.Name.ToLower().Contains(dd.ToLower())))) && ((x.CreationTime >= creationDate) && (x.CreationTime <= EndDate))));
                    foreach (var fic in fichiers)
                    {
                        a.filesTestsInfo.Add(fic);
                        if (fic != null)
                        {
                            i = i + 1;
                            //if(i>= 10)
                            //{
                            //    DateTime stopTimes = DateTime.Now;
                            //    int minutess = (stopTimes - startTime).Minutes;
                            //    int secondess = (stopTimes - startTime).Seconds;
                            //    a.minutes = minutess;
                            //    a.secondes = secondess;
                            //    a.nbresEnrefgistrements = i;
                            //    return PartialView("DirectoryPartialViewTestDiffusion", a);

                            //}
                        }
                        //a.enregistrementName = fic.FullName;                    //tests.Add(file.Name);

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

        public ActionResult FindDiffusion2(String utilisateurs, String pseudoNames, String start, String end)
        {

            int i = 0;
            var a = new DirectoryViewModel();
            a.pseudoName = pseudoNames;
            a.agentName = pseudoNames;
            //List<String> tests = new List<string>();
            //string v = date.Replace("/", "_");
            //     String pseudoNamesCopy=
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

            var test = new DirectoryInfo(@"\\10.9.6.25\Enregistrements\DIFFUSION");
            var tests2 = test.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(y => y.LastWriteTime >= creationDate);

            DateTime startTime = DateTime.Now;


            foreach (var file in tests2)
            {

                var dossier = new DirectoryInfo(@file.FullName);
                var Direcs = dossier.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(y => y.LastWriteTime >= creationDate);

                foreach (var direct in Direcs)
                {

                    var fichiers = direct.EnumerateFiles("*", SearchOption.TopDirectoryOnly).Where((x => ((x.Name.Contains("TV." + utilisateurs + "-")) && ((x.Name.ToLower().Contains(d.ToLower())) || (x.Name.ToLower().Contains(dd.ToLower())))) && ((x.CreationTime >= creationDate) && (x.CreationTime <= EndDate))));
                    foreach (var fic in fichiers)
                    {
                        a.filesTestsInfo.Add(fic);
                        if (fic != null)
                        {
                            i = i + 1;
                            //if(i>= 10)
                            //{
                            //    DateTime stopTimes = DateTime.Now;
                            //    int minutess = (stopTimes - startTime).Minutes;
                            //    int secondess = (stopTimes - startTime).Seconds;
                            //    a.minutes = minutess;
                            //    a.secondes = secondess;
                            //    a.nbresEnrefgistrements = i;
                            //    return PartialView("DirectoryPartialViewTestDiffusion", a);

                            //}
                        }
                        //a.enregistrementName = fic.FullName;                    //tests.Add(file.Name);

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
                    return PartialView("DirectoryPartialView2", a);

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
        [Authorize(Roles = "Qualité,Manager,SuperManager")]

        public ActionResult FindDiffusionTest(String utilisateurs, String pseudoNames, String start, String end)
        {
            int i = 0;
            var a = new DirectoryViewModel();
            a.pseudoName = pseudoNames;
            //List<String> tests = new List<string>();
            //string v = date.Replace("/", "_");
            //     String pseudoNamesCopy=
            string d = pseudoNames.Replace(" ", "-");
            string dd = pseudoNames.Replace(" ", "--");
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
                //ViewBag.message = ("vous n'avez pas selectionnée une date");
                return PartialView("SelectDateErreur", b);
            }

            //string startdateModified= start.Replace("/", "_");
            //string enddateModified = end.Replace("/", "_");

            var test = new DirectoryInfo(@"\\10.9.6.25\Enregistrements\DIFFUSION");
            var tests2 = test.EnumerateDirectories("*", SearchOption.TopDirectoryOnly).Where(y => y.LastWriteTime >= creationDate);

            DateTime startTime = DateTime.Now;

            Parallel.ForEach(tests2, (file) =>
            {

                var dossier = new DirectoryInfo(@file.FullName);
                var fichiers = dossier.EnumerateFiles("*", SearchOption.AllDirectories).Where((x => ((x.Name.Contains("TV." + utilisateurs + "-")) && ((x.Name.Contains(d)) || (x.Name.Contains(dd)))) && ((x.CreationTime >= creationDate) && (x.CreationTime <= EndDate))));

                foreach (var fic in fichiers)
                {
                    a.filesTestsInfo.Add(fic);
                    if (fic != null)
                    {
                        i = i + 1;
                    }
                    //a.enregistrementName = fic.FullName;                    //tests.Add(file.Name);
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
        public ActionResult FindCustomEnregistrement(String name, String fullName, String DirectoryName)
        {
            var b = new DirectoryViewModel();

            try
            {

                if (Request.IsAjaxRequest())
                {

                    b.pseudoName = name;
                    b.enregistrementName = fullName;
                    b.directoryName = DirectoryName;
                    var fileInfo = new FileInfo(@name);

                    System.IO.File.Copy(fullName, Server.MapPath("~/Files/") + name, true);
                    // player2 = new WaveOutEvent();
                    return PartialView("CustomEnregistrement", b);
                }
            }
            catch (Exception)
            {

                return PartialView("CustomEnregistrement", b);
            }
            return RedirectToAction("Index");

        }
        #endregion
        #region play
        public ActionResult Play(string name)
        {
            var b = new DirectoryViewModel();
            b.enregistrementName = name;
            if (Request.IsAjaxRequest())
            {
                //AudioFileReader a = new AudioFileReader(name);
                //var mplayer = new SoundPlayer();
                //mplayer.SoundLocation = @name;
                //mplayer.Play();
                ////WaveStream mainOutputStream = new WaveFileReader(name);
                ////WaveChannel32 volumeStream = new WaveChannel32(mainOutputStream);

                ////WaveOutEvent player = new WaveOutEvent();

                ////player.Init(volumeStream);

                ////player.Play();

                return PartialView("Sound", b);
            }

            return RedirectToAction("Index");
            // SoundPlayer player = new SoundPlayer(@name);
            //player.Play();
            //WaveFileReader wave = new WaveFileReader(name);
            //DirectSoundOut output = new DirectSoundOut();
            //output.Init(wave);
            //output.Play();
            //System.Diagnostics.Process.Start(name);

        }
        public void Play2(string name, string status)
        {

            WaveFileReader audio = new WaveFileReader(name);
            //IWavePlayer player = new WaveOut(WaveCallbackInfo.FunctionCallback());
            //IWavePlayer player = new WaveOutEvent();
            playerTest = new WaveOut(WaveCallbackInfo.FunctionCallback());
            playerTest.Volume = 1;
            playerTest.Init(audio);

            if (status.Equals("play"))
            {
                playerTest.Play();
            }
            else if (status.Equals("stop"))
            {

                playerTest.Volume = 0;
                playerTest.Stop();
                playerTest.Dispose();

                //Thread.CurrentThread.Resume;
                //playerTest.Dispose();
            }

        }

        // GET: Directory/Create
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
            //System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
            //response.ContentType = "application/octet-stream";
            //response.AppendHeader("Content-Description", "attachment;filename=" + name);
            //response.TransmitFile(name);
            //response.Flush();
            //response.End();
            var fileInfo = new FileInfo(@name);
            System.IO.File.Copy(name, "C:\\" + fileInfo.Name, true); // overwrite = true

        }
        public void deleteEnregistrement(string name)
        {

            System.IO.File.Delete(Server.MapPath("~/Files/") + name);
        }
        public ActionResult Acceuil()
        {

            Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            var directory = new DirectoryViewModel();
            directory.empId = "" + emp.Id;

            directory.userName = emp.UserName;
            directory.pseudoNameEmp = emp.pseudoName;
            if (emp.Content != null)
            {
                String strbase64 = Convert.ToBase64String(emp.Content);
                String Url = "data:" + emp.ContentType + ";base64," + strbase64;
                ViewBag.url = Url;
                directory.Url = Url;

            }

            return View(directory);
        }
        public ActionResult ReabRef()
        {
            Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            var directory = new DirectoryViewModel();
            directory.empId = "" + emp.Id;

            if (emp.Roles.Any(b => b.UserId == emp.Id && b.RoleId == 9))
            {
                ViewBag.role = "Agent Qualité_Diffusion";
            }
            if (emp.Roles.Any(b => b.UserId == emp.Id && b.RoleId == 4))
            {
                ViewBag.role = "Qualité";
            }
            directory.userName = emp.UserName;
            directory.pseudoNameEmp = emp.pseudoName;
            if (emp.Content != null)
            {
                String strbase64 = Convert.ToBase64String(emp.Content);
                String Url = "data:" + emp.ContentType + ";base64," + strbase64;
                ViewBag.url = Url;
                directory.Url = Url;

            }

            return View(directory);
        }
        public ActionResult PromoRef()
        {
            Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            var directory = new DirectoryViewModel();
            directory.empId = "" + emp.Id;
            if (emp.Roles.Any(b => b.UserId == emp.Id && b.RoleId == 9))
            {
                ViewBag.role = "Agent Qualité_Diffusion";
            }
            if (emp.Roles.Any(b => b.UserId == emp.Id && b.RoleId == 4))
            {
                ViewBag.role = "Qualité";
            }
            directory.userName = emp.UserName;
            directory.pseudoNameEmp = emp.pseudoName;
            if (emp.Content != null)
            {
                String strbase64 = Convert.ToBase64String(emp.Content);
                String Url = "data:" + emp.ContentType + ";base64," + strbase64;
                ViewBag.url = Url;
                directory.Url = Url;

            }

            return View(directory);
        }
   
        public ActionResult Ref_QR_VecteurPlus()
        {
            Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            var directory = new DirectoryViewModel();
            directory.empId = "" + emp.Id;

            if (emp.Roles.Any(b => b.UserId == emp.Id && b.RoleId == 5))
            {
                ViewBag.role = "Agent Qualité";
            }
            if (emp.Roles.Any(b => b.UserId == emp.Id && b.RoleId == 4))
            {
                ViewBag.role = "Qualité";
            }
            if (emp.Roles.Any(b => b.UserId == emp.Id && b.RoleId == 1010))
            {
                ViewBag.role = "Agent_QR";
            }
            directory.userName = emp.UserName;
            directory.pseudoNameEmp = emp.pseudoName;
            if (emp.Content != null)
            {
                String strbase64 = Convert.ToBase64String(emp.Content);
                String Url = "data:" + emp.ContentType + ";base64," + strbase64;
                ViewBag.url = Url;
                directory.Url = Url;

            }

            return View(directory);
        }
        public ActionResult Ref_KLMO_VecteurPlus()
        {
            Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            var directory = new DirectoryViewModel();
            directory.empId = "" + emp.Id;

            if (emp.Roles.Any(b => b.UserId == emp.Id && b.RoleId == 5))
            {
                ViewBag.role = "Agent Qualité";
            }
            if (emp.Roles.Any(b => b.UserId == emp.Id && b.RoleId == 4))
            {
                ViewBag.role = "Qualité";
            }
            if (emp.Roles.Any(b => b.UserId == emp.Id && b.RoleId == 1011))
            {
                ViewBag.role = "Agent_KLMO";
            }
            directory.userName = emp.UserName;
            directory.pseudoNameEmp = emp.pseudoName;
            if (emp.Content != null)
            {
                String strbase64 = Convert.ToBase64String(emp.Content);
                String Url = "data:" + emp.ContentType + ";base64," + strbase64;
                ViewBag.url = Url;
                directory.Url = Url;

            }

            return View(directory);
        }
        public ActionResult Ref_BPP_VecteurPlus()
        {
            Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            var directory = new DirectoryViewModel();
            directory.empId = "" + emp.Id;

            if (emp.Roles.Any(b => b.UserId == emp.Id && b.RoleId == 5))
            {
                ViewBag.role = "Agent Qualité";
            }
            if (emp.Roles.Any(b => b.UserId == emp.Id && b.RoleId == 4))
            {
                ViewBag.role = "Qualité";
            }
            if (emp.Roles.Any(b => b.UserId == emp.Id && b.RoleId == 3))
            {
                ViewBag.role = "Agent";
            }
            directory.userName = emp.UserName;
            directory.pseudoNameEmp = emp.pseudoName;
            if (emp.Content != null)
            {
                String strbase64 = Convert.ToBase64String(emp.Content);
                String Url = "data:" + emp.ContentType + ";base64," + strbase64;
                ViewBag.url = Url;
                directory.Url = Url;

            }

            return View(directory);
        }
        public ActionResult Ref_Battonage_VecteurPlus()
        {
            Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            var directory = new DirectoryViewModel();
            directory.empId = "" + emp.Id;

            if (emp.Roles.Any(b => b.UserId == emp.Id && b.RoleId == 5))
            {
                ViewBag.role = "Agent Qualité";
            }
            if (emp.Roles.Any(b => b.UserId == emp.Id && b.RoleId == 4))
            {
                ViewBag.role = "Qualité";
            }
            if (emp.Roles.Any(b => b.UserId == emp.Id && b.RoleId == 3))
            {
                ViewBag.role = "Agent";
            }
            directory.userName = emp.UserName;
            directory.pseudoNameEmp = emp.pseudoName;
            if (emp.Content != null)
            {
                String strbase64 = Convert.ToBase64String(emp.Content);
                String Url = "data:" + emp.ContentType + ";base64," + strbase64;
                ViewBag.url = Url;
                directory.Url = Url;

            }

            return View(directory);
        }
        [Authorize(Roles = "Qualité,Manager,Agent Qualité_CustomerService,Agent_CustomerService")]
        public ActionResult Ref_Enquete_Auto()
        {
            Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            var directory = new DirectoryViewModel();
            directory.empId = "" + emp.Id;

            if (emp.Roles.Any(b => b.UserId == emp.Id && b.RoleId == 8))
            {
                ViewBag.role = "Agent Qualité_CustomerService";
            }
            if (emp.Roles.Any(b => b.UserId == emp.Id && b.RoleId == 4))
            {
                ViewBag.role = "Qualité";
            }
            if (emp.Roles.Any(b => b.UserId == emp.Id && b.RoleId == 10))
            {
                ViewBag.role = "Agent_CustomerService";
            }
            directory.userName = emp.UserName;
            directory.pseudoNameEmp = emp.pseudoName;
            if (emp.Content != null)
            {
                String strbase64 = Convert.ToBase64String(emp.Content);
                String Url = "data:" + emp.ContentType + ";base64," + strbase64;
                ViewBag.url = Url;
                directory.Url = Url;

            }

            return View(directory);
        }
        [Authorize(Roles = "Qualité,Manager,Agent Qualité_CustomerService,Agent_SAMRC")]
        public ActionResult Ref_FO_SCSAMRC()
        {
            Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            var directory = new DirectoryViewModel();
            directory.empId = "" + emp.Id;

            if (emp.Roles.Any(b => b.UserId == emp.Id && b.RoleId == 8))
            {
                ViewBag.role = "Agent Qualité_CustomerService";
            }
            if (emp.Roles.Any(b => b.UserId == emp.Id && b.RoleId == 4))
            {
                ViewBag.role = "Qualité";
            }
            if (emp.Roles.Any(b => b.UserId == emp.Id && b.RoleId == 1009))
            {
                ViewBag.role = "Agent_SAMRC";
            }
            directory.userName = emp.UserName;
            directory.pseudoNameEmp = emp.pseudoName;
            if (emp.Content != null)
            {
                String strbase64 = Convert.ToBase64String(emp.Content);
                String Url = "data:" + emp.ContentType + ";base64," + strbase64;
                ViewBag.url = Url;
                directory.Url = Url;

            }

            return View(directory);
        }
        [Authorize(Roles = "Qualité,Manager,Agent Qualité_CustomerService,Agent_SAMRC")]
        public ActionResult Ref_BO_SCSAMRC()
        {
            Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            var directory = new DirectoryViewModel();
            directory.empId = "" + emp.Id;

            if (emp.Roles.Any(b => b.UserId == emp.Id && b.RoleId == 8))
            {
                ViewBag.role = "Agent Qualité_CustomerService";
            }
            if (emp.Roles.Any(b => b.UserId == emp.Id && b.RoleId == 4))
            {
                ViewBag.role = "Qualité";
            }
            if (emp.Roles.Any(b => b.UserId == emp.Id && b.RoleId == 1009))
            {
                ViewBag.role = "Agent_SAMRC";
            }
            directory.userName = emp.UserName;
            directory.pseudoNameEmp = emp.pseudoName;
            if (emp.Content != null)
            {
                String strbase64 = Convert.ToBase64String(emp.Content);
                String Url = "data:" + emp.ContentType + ";base64," + strbase64;
                ViewBag.url = Url;
                directory.Url = Url;

            }

            return View(directory);
        }
        [Authorize(Roles = "Qualité,Manager,Agent Qualité_Diffusion")]
        public ActionResult Ref_BO_SCDiff()
        {
            Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            var directory = new DirectoryViewModel();
            directory.empId = "" + emp.Id;

            if (emp.Roles.Any(b => b.UserId == emp.Id && b.RoleId == 9))
            {
                ViewBag.role = "Agent Qualité_Diffusion";
            }
            if (emp.Roles.Any(b => b.UserId == emp.Id && b.RoleId == 4))
            {
                ViewBag.role = "Qualité";
            }
            directory.userName = emp.UserName;
            directory.pseudoNameEmp = emp.pseudoName;
            if (emp.Content != null)
            {
                String strbase64 = Convert.ToBase64String(emp.Content);
                String Url = "data:" + emp.ContentType + ";base64," + strbase64;
                ViewBag.url = Url;
                directory.Url = Url;

            }

            return View(directory);
        }
        [Authorize(Roles = "Qualité,Manager,Agent Qualité_Diffusion")]
        public ActionResult Ref_FO_SCDiff()
        {
            Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            var directory = new DirectoryViewModel();
            directory.empId = "" + emp.Id;

            if (emp.Roles.Any(b => b.UserId == emp.Id && b.RoleId == 9))
            {
                ViewBag.role = "Agent Qualité_Diffusion";
            }
            if (emp.Roles.Any(b => b.UserId == emp.Id && b.RoleId == 4))
            {
                ViewBag.role = "Qualité";
            }
            directory.userName = emp.UserName;
            directory.pseudoNameEmp = emp.pseudoName;
            if (emp.Content != null)
            {
                String strbase64 = Convert.ToBase64String(emp.Content);
                String Url = "data:" + emp.ContentType + ";base64," + strbase64;
                ViewBag.url = Url;
                directory.Url = Url;

            }

            return View(directory);
        }
        [Authorize(Roles = "Qualité,Agent Qualité_AchatPublic")]
        public ActionResult Ref_SC_AchatPublic()
        {
            Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            var directory = new DirectoryViewModel();
            directory.empId = "" + emp.Id;

            if (emp.Roles.Any(b => b.UserId == emp.Id && b.RoleId == 2009))
            {
                ViewBag.role = "Agent Qualité_AchatPublic";
            }
            if (emp.Roles.Any(b => b.UserId == emp.Id && b.RoleId == 4))
            {
                ViewBag.role = "Qualité";
            }
           
            directory.userName = emp.UserName;
            directory.pseudoNameEmp = emp.pseudoName;
            if (emp.Content != null)
            {
                String strbase64 = Convert.ToBase64String(emp.Content);
                String Url = "data:" + emp.ContentType + ";base64," + strbase64;
                ViewBag.url = Url;
                directory.Url = Url;

            }

            return View(directory);
        }
        [Authorize(Roles = "Qualité,Agent Qualité,Agent_PRV,Agent Qualité_PRV")]
        public ActionResult Ref_PRV_VecteurPlus()
        {
            Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            var directory = new DirectoryViewModel();
            directory.empId = "" + emp.Id;

            if (emp.Roles.Any(b => b.UserId == emp.Id && b.RoleId == 5))
            {
                ViewBag.role = "Agent Qualité";
            }
            if (emp.Roles.Any(b => b.UserId == emp.Id && b.RoleId == 2014))
            {
                ViewBag.role = "Agent Qualité_PRV";
            }
            if (emp.Roles.Any(b => b.UserId == emp.Id && b.RoleId == 4))
            {
                ViewBag.role = "Qualité";
            }
            if (emp.Roles.Any(b => b.UserId == emp.Id && b.RoleId == 3))
            {
                ViewBag.role = "Agent";
            }
            directory.userName = emp.UserName;
            directory.pseudoNameEmp = emp.pseudoName;
            if (emp.Content != null)
            {
                String strbase64 = Convert.ToBase64String(emp.Content);
                String Url = "data:" + emp.ContentType + ";base64," + strbase64;
                ViewBag.url = Url;
                directory.Url = Url;

            }

            return View(directory);
        }
        [HttpGet]
        public ActionResult AnnulerRecherche(String utilisateurs, String pseudoNames, String start, String end)
        {
            //throw new TimeoutException();

            return RedirectToAction("Diffusion");
        }
        #endregion

        #region AgentQualité
        [Authorize(Roles = "Agent Qualité")]
        public ActionResult AcceuilAgentQualite()
        {

            Employee emp = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            var directory = new DirectoryViewModel();
            directory.empId = "" + emp.Id;

            directory.userName = emp.UserName;
            directory.pseudoNameEmp = emp.pseudoName;
            if (emp.Content != null)
            {
                String strbase64 = Convert.ToBase64String(emp.Content);
                String Url = "data:" + emp.ContentType + ";base64," + strbase64;
                ViewBag.url = Url;
                directory.Url = Url;

            }

            return View(directory);
        }
        #endregion

    }
}
