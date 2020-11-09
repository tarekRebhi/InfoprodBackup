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
    public class AccordsController : Controller
    {
        private ReportContext db = new ReportContext();
        IEmployeeService service;
        IGroupeEmployeeService serviceGroupeEmp;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        static List<string> AllTitresGisiReab;
        static List<string> AllTitresGisiPromo;
        static List<string> AllTitresGmtReab;
        static List<string> AllTitresGmtPromo;
        static List<Employee> employees;
        public AccordsController()
        {
            service = new EmployeeService();
            serviceGroupeEmp = new GroupesEmployeService();
        }

        public AccordsController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
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
        // GISI REAB
        public ActionResult GISIREAB()
        {
            Employee empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            var data = db.Details_Activite_REAB_GISI.ToList();

            AllTitresGisiReab = data.Select(gp => gp.TITRE_OPERATION).Distinct().ToList();
            ViewBag.AllTitres = AllTitresGisiReab;

            List<Employee> emps = serviceGroupeEmp.getListEmployeeByGroupeId(1);
            List<string> AgentsList = new List<string>();
            employees = new List<Employee>();
            foreach (var item in emps)
            {
                if (item.Roles.Any(r => r.UserId == item.Id && r.RoleId == 3))
                {
                    AgentsList.Add(item.UserName);
                    employees.Add(item);
                }
            }
            ViewBag.AgentsList = AgentsList;

            var a = new EvaluationViewModel();
            a.userName = empConnected.UserName;
            a.pseudoNameEmp = empConnected.pseudoName;
            if (empConnected.Content != null)
            {
                String strbase64 = Convert.ToBase64String(empConnected.Content);
                String Url = "data:" + empConnected.ContentType + ";base64," + strbase64;
                ViewBag.url = Url;
                a.Url = Url;

            }
            return View(a);
        }
        public JsonResult GetAccordsREABGISI(DateTime fromDate, DateTime toDate)
        {
            var data = db.Details_Activite_REAB_GISI.Where(d => d.AI == 1).ToList();
            List<accord> accords = new List<accord>();
            foreach (var ag in employees)
            {
                foreach (var tit in AllTitresGisiReab)
                {
                    int nbreaccord = 0;
                    foreach (var item in data)
                    {
                        string datestr = item.DATE.Substring(0, 4) + '-' + item.DATE.Substring(4, 2) + '-' + item.DATE.Substring(6, 2);
                        DateTime date = DateTime.Parse(datestr);
                        if (item.TITRE_OPERATION == tit && item.ID_TV == ag.IdHermes && date >= fromDate && date <= toDate)
                        {
                            nbreaccord += 1;
                        }
                    }
                    accords.Add(new accord { agent = ag.UserName, titre = tit, nbre = nbreaccord });
                }
            }
            return Json(accords, JsonRequestBehavior.AllowGet);
        }

        //GISI PROMO
        public ActionResult GISIPROMO()
        {
            Employee empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            var data = db.Details_Activite_PROMO_GISI.ToList();

            AllTitresGisiPromo = data.Select(gp => gp.TITRE_OPERATION).Distinct().ToList();
            ViewBag.AllTitres = AllTitresGisiPromo;

            List<Employee> emps = serviceGroupeEmp.getListEmployeeByGroupeId(2);
            List<string> AgentsList = new List<string>();
            employees = new List<Employee>();
            foreach (var item in emps)
            {
                if (item.Roles.Any(r => r.UserId == item.Id && r.RoleId == 3))
                {
                    AgentsList.Add(item.UserName);
                    employees.Add(item);
                }
            }
            ViewBag.AgentsList = AgentsList;
            var a = new EvaluationViewModel();
            a.userName = empConnected.UserName;
            a.pseudoNameEmp = empConnected.pseudoName;
            if (empConnected.Content != null)
            {
                String strbase64 = Convert.ToBase64String(empConnected.Content);
                String Url = "data:" + empConnected.ContentType + ";base64," + strbase64;
                ViewBag.url = Url;
                a.Url = Url;
            }
            return View(a);
        }
        public JsonResult GetAccordsPROMOGISI(DateTime fromDate, DateTime toDate)
        {

            var data = db.Details_Activite_PROMO_GISI.Where(d => d.ACCORD_TEMP == 1).ToList();
            List<accord> accords = new List<accord>();
            foreach (var ag in employees)
            {
                foreach (var tit in AllTitresGisiPromo)
                {
                    int nbreaccord = 0; int nbreaccordcross = 0;
                    foreach (var item in data)
                    {
                        string datestr = item.DATE.Substring(0, 4) + '-' + item.DATE.Substring(4, 2) + '-' + item.DATE.Substring(6, 2);
                        DateTime date = DateTime.Parse(datestr);
                        if (item.TITRE_OPERATION == tit && item.ID_TV == ag.IdHermes && date >= fromDate && date <= toDate)
                        {
                            nbreaccord += 1;
                            if (item.CODE_OPE == "CROSS")
                            {
                                nbreaccordcross += 1;
                            }
                        }
                    }
                    accords.Add(new accord { agent = ag.UserName, titre = tit, nbre = nbreaccord, nbrecross = nbreaccordcross });
                }
            }
            return Json(accords, JsonRequestBehavior.AllowGet);
        }
        //GMT REAB
        public ActionResult GMTREAB()
        {
            Employee empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            var data = db.Details_Activite_REAB_GMT.ToList();

            AllTitresGmtReab = data.Select(gp => gp.TITRE_OPERATION).Distinct().ToList();
            ViewBag.AllTitres = AllTitresGmtReab;

            List<Employee> emps = serviceGroupeEmp.getListEmployeeByGroupeId(3);
            List<string> AgentsList = new List<string>();
            employees = new List<Employee>();
            foreach (var item in emps)
            {
                if (item.Roles.Any(r => r.UserId == item.Id && r.RoleId == 3))
                {
                    AgentsList.Add(item.UserName);
                    employees.Add(item);
                }
            }
            ViewBag.AgentsList = AgentsList;
            var a = new EvaluationViewModel();
            a.userName = empConnected.UserName;
            a.pseudoNameEmp = empConnected.pseudoName;
            if (empConnected.Content != null)
            {
                String strbase64 = Convert.ToBase64String(empConnected.Content);
                String Url = "data:" + empConnected.ContentType + ";base64," + strbase64;
                ViewBag.url = Url;
                a.Url = Url;

            }
            return View(a);
        }
        public JsonResult GetAccordsREABGMT(DateTime fromDate, DateTime toDate)
        {

            var data = db.Details_Activite_REAB_GMT.Where(d => d.AI == 1).ToList();
            List<accord> accords = new List<accord>();
            foreach (var ag in employees)
            {
                foreach (var tit in AllTitresGmtReab)
                {
                    int nbreaccord = 0;
                    foreach (var item in data)
                    {
                        string datestr = item.DATE.Substring(0, 4) + '-' + item.DATE.Substring(4, 2) + '-' + item.DATE.Substring(6, 2);
                        DateTime date = DateTime.Parse(datestr);
                        if (item.TITRE_OPERATION == tit && item.ID_TV == ag.IdHermes && date >= fromDate && date <= toDate)
                        {
                            nbreaccord += 1;
                        }
                    }
                    accords.Add(new accord { agent = ag.UserName, titre = tit, nbre = nbreaccord });
                }
            }
            return Json(accords, JsonRequestBehavior.AllowGet);
        }

        //GMT PROMO
        public ActionResult GMTPROMO()
        {
            Employee empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            var data = db.Details_Activite_PROMO_GMT.ToList();

            AllTitresGmtPromo = data.Select(gp => gp.TITRE_OPERATION).Distinct().ToList();
            ViewBag.AllTitres = AllTitresGmtPromo;

            List<Employee> emps = serviceGroupeEmp.getListEmployeeByGroupeId(4);
            List<string> AgentsList = new List<string>();
            employees = new List<Employee>();
            foreach (var item in emps)
            {
                if (item.Roles.Any(r => r.UserId == item.Id && r.RoleId == 3))
                {
                    AgentsList.Add(item.UserName);
                    employees.Add(item);
                }
            }
            ViewBag.AgentsList = AgentsList;
            var a = new EvaluationViewModel();
            a.userName = empConnected.UserName;
            a.pseudoNameEmp = empConnected.pseudoName;
            if (empConnected.Content != null)
            {
                String strbase64 = Convert.ToBase64String(empConnected.Content);
                String Url = "data:" + empConnected.ContentType + ";base64," + strbase64;
                ViewBag.url = Url;
                a.Url = Url;

            }
            return View(a);
        }
        public JsonResult GetAccordsPROMOGMT(DateTime fromDate, DateTime toDate)
        {

            var data = db.Details_Activite_PROMO_GMT.Where(d => d.ACCORD_TEMP == 1).ToList();
            List<accord> accords = new List<accord>();
            foreach (var ag in employees)
            {
                foreach (var tit in AllTitresGmtPromo)
                {
                    int nbreaccord = 0; int nbreaccordcross = 0;
                    foreach (var item in data)
                    {
                        string datestr = item.DATE.Substring(0, 4) + '-' + item.DATE.Substring(4, 2) + '-' + item.DATE.Substring(6, 2);
                        DateTime date = DateTime.Parse(datestr);
                        if (item.TITRE_OPERATION == tit && item.ID_TV == ag.IdHermes && date >= fromDate && date <= toDate)
                        {
                            nbreaccord += 1;
                            if (item.CODE_OPE == "CROSS")
                            {
                                nbreaccordcross += 1;
                            }
                        }
                    }
                    accords.Add(new accord { agent = ag.UserName, titre = tit, nbre = nbreaccord, nbrecross = nbreaccordcross });
                }
            }
            return Json(accords, JsonRequestBehavior.AllowGet);
        }
    }
}
