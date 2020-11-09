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
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using Services;

namespace MVCWEB.Controllers
{
    public class InventaireController : Controller
    {
        private ReportContext db = new ReportContext();
        #region globalVariable
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        IEmployeeService serviceEmployee;
       
        #endregion

        #region constructor
        public InventaireController()
        {
            serviceEmployee = new EmployeeService();
          
        }
        public InventaireController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
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

        // GET: Inventaire
        public ActionResult Index()
        {
            Employee empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (empConnected.Roles.Any(b => b.UserId == empConnected.Id && b.RoleId == 2012))
            {
                ViewBag.role = "RH";
            }
            if (empConnected.Roles.Any(b => b.UserId == empConnected.Id && b.RoleId == 2011))
            {
                ViewBag.role = "IT";
            }
            {
                String strbase64 = Convert.ToBase64String(empConnected.Content);
                String empConnectedImage = "data:" + empConnected.ContentType + ";base64," + strbase64;
                ViewBag.empConnectedImage = empConnectedImage;
            }
            ViewBag.nameEmpConnected = empConnected.UserName;
            ViewBag.pseudoNameEmpConnected = empConnected.pseudoName;
            return View(db.Inventaires.ToList());
        }

        // GET: Inventaire/Details/5

      
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inventaire inventaire = db.Inventaires.Find(id);
            if (inventaire == null)
            {
                return HttpNotFound();
            }
            return View(inventaire);
        }

        // GET: Inventaire/Create
        public ActionResult Create()
        {
            Employee empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            {
                String strbase64 = Convert.ToBase64String(empConnected.Content);
                String empConnectedImage = "data:" + empConnected.ContentType + ";base64," + strbase64;
                ViewBag.empConnectedImage = empConnectedImage;
            }
            ViewBag.nameEmpConnected = empConnected.UserName;
            ViewBag.pseudoNameEmpConnected = empConnected.pseudoName;

            List<SelectListItem> ManagersList = new List<SelectListItem>();
         
            var logins = serviceEmployee.GetAll();
            var employes = logins.Select(o => o).Distinct().ToList();
            var ordredemployees = employes.OrderBy(a => a.UserName).ToList();
            foreach (var test in ordredemployees)
            {
                if (test.Roles.Any(r => r.UserId == test.Id && r.RoleId == 2 || r.RoleId == 5 || r.RoleId == 8 || r.RoleId == 9 || r.RoleId == 2009))
                {
                   ManagersList.Add(new SelectListItem { Text = test.UserName, Value = test.UserName });
                     
                }
            }
            ViewBag.managers = ManagersList;
            return View();
        }

        // POST: Inventaire/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Manager,Societe,Matricule,Nom,Prenom,Mail,Qualification,Service,Adresse,Ville,CIN,Date_Delivrance_CIN,Tel1,Tel2,Teletravail,PC,Connexion,Type,Debit,Hermes,Acces_VPN,Acces_TSE,Opérationnel,Commentaire1,OrdinateurPro,Modele,SacaDos,Ecran,Adaptateur_HDMI,Peripheriques,Commentaire2,Contrat_Teletravail,Date_Teletravail,Commentaire3")] Inventaire inventaire)
        {
            if (ModelState.IsValid)
            {
                db.Inventaires.Add(inventaire);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(inventaire);
        }

        // GET: Inventaire/Edit/5
        public ActionResult Edit(int? id)
        {
            Employee empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (empConnected.Roles.Any(b => b.UserId == empConnected.Id && b.RoleId == 2012))
            {
                ViewBag.role = "RH";
            }
            if (empConnected.Roles.Any(b => b.UserId == empConnected.Id && b.RoleId == 2011))
            {
                ViewBag.role = "IT";
            }
            {
                String strbase64 = Convert.ToBase64String(empConnected.Content);
                String empConnectedImage = "data:" + empConnected.ContentType + ";base64," + strbase64;
                ViewBag.empConnectedImage = empConnectedImage;
            }
            ViewBag.nameEmpConnected = empConnected.UserName;
            ViewBag.pseudoNameEmpConnected = empConnected.pseudoName;
            List<SelectListItem> ManagersList = new List<SelectListItem>();

            var logins = serviceEmployee.GetAll();
            var employes = logins.Select(o => o).Distinct().ToList();
            var ordredemployees = employes.OrderBy(a => a.UserName).ToList();
            foreach (var test in ordredemployees)
            {
                if (test.Roles.Any(r => r.UserId == test.Id && r.RoleId == 2 || r.RoleId == 5 || r.RoleId == 8 || r.RoleId == 9 || r.RoleId == 2009))
                {
                    ManagersList.Add(new SelectListItem { Text = test.UserName, Value = test.UserName });

                }
            }
            ViewBag.managers = ManagersList;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inventaire inventaire = db.Inventaires.Find(id);
            if (inventaire == null)
            {
                return HttpNotFound();
            }
            return View(inventaire);
        }

        // POST: Inventaire/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Manager,Societe,Matricule,Nom,Prenom,Mail,Qualification,Service,Adresse,Ville,CIN,Date_Delivrance_CIN,Tel1,Tel2,Teletravail,PC,Connexion,Type,Debit,Hermes,Acces_VPN,Acces_TSE,Opérationnel,Commentaire1,OrdinateurPro,Modele,SacaDos,Ecran,Adaptateur_HDMI,Peripheriques,Commentaire2,Contrat_Teletravail,Date_Teletravail,Commentaire3")] Inventaire inventaire)
        {
            if (ModelState.IsValid)
            {
                db.Entry(inventaire).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(inventaire);
        }

        public ActionResult EditIT(int? id)
        {
            Employee empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            if (empConnected.Roles.Any(b => b.UserId == empConnected.Id && b.RoleId == 2012))
            {
                ViewBag.role = "RH";
            }
            if (empConnected.Roles.Any(b => b.UserId == empConnected.Id && b.RoleId == 2011))
            {
                ViewBag.role = "IT";
            }
            {
                String strbase64 = Convert.ToBase64String(empConnected.Content);
                String empConnectedImage = "data:" + empConnected.ContentType + ";base64," + strbase64;
                ViewBag.empConnectedImage = empConnectedImage;
            }
            ViewBag.nameEmpConnected = empConnected.UserName;
            ViewBag.pseudoNameEmpConnected = empConnected.pseudoName;
            List<SelectListItem> ManagersList = new List<SelectListItem>();

            var logins = serviceEmployee.GetAll();
            var employes = logins.Select(o => o).Distinct().ToList();
            var ordredemployees = employes.OrderBy(a => a.UserName).ToList();
            foreach (var test in ordredemployees)
            {
                if (test.Roles.Any(r => r.UserId == test.Id && r.RoleId == 2 || r.RoleId == 5 || r.RoleId == 8 || r.RoleId == 9 || r.RoleId == 2009))
                {
                    ManagersList.Add(new SelectListItem { Text = test.UserName, Value = test.UserName });

                }
            }
            ViewBag.managers = ManagersList;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inventaire inventaire = db.Inventaires.Find(id);
            if (inventaire == null)
            {
                return HttpNotFound();
            }
            return View(inventaire);
        }

        // POST: Inventaire/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditIT([Bind(Include = "Id,Manager,Societe,Matricule,Nom,Prenom,Mail,Qualification,Service,Adresse,Ville,CIN,Date_Delivrance_CIN,Tel1,Tel2,Teletravail,PC,Connexion,Type,Debit,Hermes,Acces_VPN,Acces_TSE,Opérationnel,Commentaire1,OrdinateurPro,Modele,SacaDos,Ecran,Adaptateur_HDMI,Peripheriques,Commentaire2,Contrat_Teletravail,Date_Teletravail,Commentaire3")] Inventaire inventaire)
        {
            if (ModelState.IsValid)
            {
                db.Entry(inventaire).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(inventaire);
        }
        // GET: Inventaire/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Inventaire inventaire = db.Inventaires.Find(id);
            if (inventaire == null)
            {
                return HttpNotFound();
            }
            return View(inventaire);
        }

        // POST: Inventaire/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Inventaire inventaire = db.Inventaires.Find(id);
            db.Inventaires.Remove(inventaire);
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
