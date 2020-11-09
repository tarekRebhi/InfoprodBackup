using Domain.Entity;
using Microsoft.AspNet.Identity.Owin;
using Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MVCWEB.Controllers
{
    [Authorize(Roles = "Admin")]

    public class GroupeController : Controller
    {
        // GET: Groupe
        #region globalMemberVariables
        IGroupeService service;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        #endregion
        #region Constructor
        public GroupeController()
        {

            service = new GroupeService();
        }
        public GroupeController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
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
        #region CRUD
        [Authorize(Roles = "Admin")]
        public ActionResult Index(String search, FormCollection form,int? CallsToMake)
        {
            //string value = (string)Session["loginIndex"];

            var groupes = service.GetAll();
            List<Groupe> fVM = new List<Groupe>();
           //int numVal = Int32.Parse(type);

            foreach (var item in groupes)
            {
                fVM.Add(item);
            }
            if (!String.IsNullOrEmpty(search))
            {

                fVM = fVM.Where(p => p.nom.ToLower().Contains(search.ToLower())).ToList<Groupe>();


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

        // GET: Employee/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Groupe groupe = service.getById(id);
            return View(groupe);
        }

        // GET: Employee/Create
        public ActionResult Create()
        {
            var groupe = new Groupe();
            return View(groupe);
        }

        // POST: Medcin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Groupe c, FormCollection form)
        {
            if (!ModelState.IsValid)
            {
                RedirectToAction("Create");
            }
            Groupe groupe = new Groupe
            {
                Id = c.Id,
                nom = c.nom,
                responsable=c.responsable
                

            };
            service.Add(groupe);
            service.SaveChange();


            return RedirectToAction("Index");

        }

        // GET: Employee/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Groupe groupe = service.getById(id);

            return View(groupe);
        }

        // POST: Medcin/Edit/5
        [HttpPost, ActionName("Edit")]
        public ActionResult EditGroupe(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var groupe = service.getById(id);

            if (TryUpdateModel(groupe))
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
            return View(groupe);

        }

        // GET: Employee/Delete/5
        public ActionResult Delete(int? id)
        {

            Groupe groupe = service.getById(id);

            service.Delete(groupe);
            service.SaveChange();
            return RedirectToAction("Index");
        }

        // POST: Medcin/Delete/5
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
        [HttpGet]
        public ActionResult FindGroupe(int? Id)
        {
            Groupe item = service.getById(Id);


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
    }
}

