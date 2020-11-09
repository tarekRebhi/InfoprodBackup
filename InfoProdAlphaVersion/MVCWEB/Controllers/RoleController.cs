using Data;
using Data.Store;
using Domain.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MVCWEB.Controllers
{
    public class RoleController : Controller
    {
        // GET: Role
        #region member variablesDefinitions
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        ReportContext context;
        
        #endregion
        #region Constructor
        public RoleController(){

            context = new ReportContext();
            
            }

        public RoleController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
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
        #region AffichageRole
        [AllowAnonymous]
        public ActionResult Index()
        {
            try
            {
                var roles = context.Roles.ToList();
                return View(roles);
            }
            catch (NullReferenceException a)
            {
                Console.WriteLine(a);
                return View("~/Views/Authentification/Index.cshtml");
            }
        }
        #endregion
        #region crudRole
        // GET: Role/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Role/Create
        [AllowAnonymous]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Role/Create
        [HttpPost]
        [AllowAnonymous]

        public ActionResult Create(CustomRole item,FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here
                if (!RoleManager.RoleExists(item.Name))
                {
                    // first we create Admin rool
                    var role = new CustomRole();
                    role.Name = item.Name;
                    RoleManager.Create(role);
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Create");

            }
            catch
            {
                return View();
            }
        }

        // GET: Role/Edit/5
        public ActionResult Edit(int id)
        {
            var role = context.Roles.Find(id);

            return View(role);
        }

        // POST: Role/Edit/5
        [HttpPost, ActionName("Edit")]
        public ActionResult EditRole(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var role = context.Roles.Find(id);

            if (TryUpdateModel(role))
            {
                try
                {
                    context.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Erreur!!!!!");
                }
            }
            return View();

        }

        public ActionResult Delete(int id)
        {
            var role = context.Roles.Find(id);
            context.Roles.Remove(role);
            context.SaveChanges();
            return RedirectToAction("Index");
        }

        // POST: Role/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                var role = context.Roles.Find(id);
                context.Roles.Remove(role);
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        #endregion
    }
}
