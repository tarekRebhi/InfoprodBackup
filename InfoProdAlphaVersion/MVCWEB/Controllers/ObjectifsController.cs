using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Data;
using Domain.Entity;

using MVCWEB.Models;
using Services;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace MVCWEB.Controllers
{
    [Authorize(Roles = "Qualité,Manager,SuperManager")]
    public class ObjectifsController : Controller
    {
        private ReportContext db = new ReportContext();
        IGroupeEmployeeService serviceGroupeEmp;
        IEmployeeService service;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        public ObjectifsController()
        {
            service = new EmployeeService();
            serviceGroupeEmp = new GroupesEmployeService();

        }
        public ObjectifsController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
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
        // GET: Objectifs
        public ActionResult Index()
        {
            Employee empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            var groupesassociees = serviceGroupeEmp.getGroupeByIDEmployee(empConnected.Id);
            List<SelectListItem> groupes = new List<SelectListItem>();
            foreach (var item in groupesassociees)
            {
                if (!(groupes.Exists(x => x.Value == item.nom)))
                {

                    groupes.Add(new SelectListItem { Text = item.nom, Value = item.nom });
                }
            }

            ViewBag.groupes = groupes;

            ViewBag.pseudoName = empConnected.pseudoName;
            ViewBag.userName = empConnected.UserName;
            if (empConnected.Content != null)
            {
                String strbase64 = Convert.ToBase64String(empConnected.Content);
                String Url = "data:" + empConnected.ContentType + ";base64," + strbase64;
                ViewBag.Url = Url;
            }
            return View(db.Objectifs.ToList());
        }

        // GET: Objectifs/Details/5
        public ActionResult Details(int? id)
        {
            Employee empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            var groupesassociees = serviceGroupeEmp.getGroupeByIDEmployee(empConnected.Id);
            List<SelectListItem> groupes = new List<SelectListItem>();
            foreach (var item in groupesassociees)
            {
                if (!(groupes.Exists(x => x.Value == item.nom)))
                {

                    groupes.Add(new SelectListItem { Text = item.nom, Value = item.nom });
                }
            }

            ViewBag.groupes = groupes;

            ViewBag.pseudoName = empConnected.pseudoName;
            ViewBag.userName = empConnected.UserName;
            if (empConnected.Content != null)
            {
                String strbase64 = Convert.ToBase64String(empConnected.Content);
                String Url = "data:" + empConnected.ContentType + ";base64," + strbase64;
                ViewBag.Url = Url;
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Objectif objectif = db.Objectifs.Find(id);
            if (objectif == null)
            {
                return HttpNotFound();
            }
            return View(objectif);
        }

        // GET: Objectifs/Create
        public ActionResult Create()
        {
            Employee empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            var groupesassociees = serviceGroupeEmp.getGroupeByIDEmployee(empConnected.Id);
            List<SelectListItem> groupes = new List<SelectListItem>();
            foreach (var item in groupesassociees)
            {
                if (!(groupes.Exists(x => x.Value == item.nom)))
                {

                    groupes.Add(new SelectListItem { Text = item.nom, Value = item.nom });
                }
            }
            ViewBag.groupes = groupes;
           
            ViewBag.pseudoName = empConnected.pseudoName;
            ViewBag.userName = empConnected.UserName;
            if (empConnected.Content != null)
            {
                String strbase64 = Convert.ToBase64String(empConnected.Content);
                String Url = "data:" + empConnected.ContentType + ";base64," + strbase64;
                ViewBag.Url = Url;               
            }
            return View();
        }

        // POST: Objectifs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Namegroupe,ObjectifAccords")] Objectif objectif)
        {
            if (ModelState.IsValid)
            {
                db.Objectifs.Add(objectif);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(objectif);
        }

        // GET: Objectifs/Edit/5
        public ActionResult Edit(int? id)
        {
            Employee empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            var groupesassociees = serviceGroupeEmp.getGroupeByIDEmployee(empConnected.Id);
            List<SelectListItem> groupes = new List<SelectListItem>();
            foreach (var item in groupesassociees)
            {
                if (!(groupes.Exists(x => x.Value == item.nom)))
                {

                    groupes.Add(new SelectListItem { Text = item.nom, Value = item.nom });
                }
            }

            ViewBag.groupes = groupes;

            ViewBag.pseudoName = empConnected.pseudoName;
            ViewBag.userName = empConnected.UserName;
            if (empConnected.Content != null)
            {
                String strbase64 = Convert.ToBase64String(empConnected.Content);
                String Url = "data:" + empConnected.ContentType + ";base64," + strbase64;
                ViewBag.Url = Url;
            }
                if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Objectif objectif = db.Objectifs.Find(id);
            if (objectif == null)
            {
                return HttpNotFound();
            }
            return View(objectif);
        }

        // POST: Objectifs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Namegroupe,ObjectifAccords")] Objectif objectif)
        {
            if (ModelState.IsValid)
            {
                db.Entry(objectif).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(objectif);
        }

        // GET: Objectifs/Delete/5
        public ActionResult Delete(int? id)
        {
            Employee empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            var groupesassociees = serviceGroupeEmp.getGroupeByIDEmployee(empConnected.Id);
            List<SelectListItem> groupes = new List<SelectListItem>();
            foreach (var item in groupesassociees)
            {
                if (!(groupes.Exists(x => x.Value == item.nom)))
                {

                    groupes.Add(new SelectListItem { Text = item.nom, Value = item.nom });
                }
            }

            ViewBag.groupes = groupes;

            ViewBag.pseudoName = empConnected.pseudoName;
            ViewBag.userName = empConnected.UserName;
            if (empConnected.Content != null)
            {
                String strbase64 = Convert.ToBase64String(empConnected.Content);
                String Url = "data:" + empConnected.ContentType + ";base64," + strbase64;
                ViewBag.Url = Url;
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Objectif objectif = db.Objectifs.Find(id);
            if (objectif == null)
            {
                return HttpNotFound();
            }
            return View(objectif);
        }

        // POST: Objectifs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Objectif objectif = db.Objectifs.Find(id);
            db.Objectifs.Remove(objectif);
            db.SaveChanges();
            return RedirectToAction("Index");
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
