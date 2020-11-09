using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Data;
using Domain.Entity;
using Services;
using Microsoft.AspNet.Identity.Owin;

namespace MVCWEB.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        #region globalMemberVariable
        IUtilisateurService service;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        #endregion
        #region constructor
        public UserController()
        {
            service = new UtilisateurService();
        }
        public UserController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
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
        // GET: User
        public ActionResult Index(String search, FormCollection form)
        {
            //string value = (string)Session["loginIndex"];
            try { 
            var users = service.GetAll();
            List<Utilisateur> fVM = new List<Utilisateur>();


            foreach (var item in users)
            {
                fVM.Add(item);
            }
            if (!String.IsNullOrEmpty(search))
            {

                fVM = fVM.Where(p => p.nomPrenom.ToLower().Contains(search.ToLower())).ToList<Utilisateur>();


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
            }catch(NullReferenceException a)
            {
                Console.WriteLine(a);
                return View("~/Views/Authentification/Index.cshtml");
            }
        }

        // GET: User/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Utilisateur user = service.getById(id);
            return View(user);
        }

        // GET: User/Create

        // POST: User/Create


        // GET: User/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Utilisateur user = service.getById(id);

            return View(user);
        }

        // POST: User/Edit/5
        [HttpPost, ActionName("Edit")]
        public ActionResult EditUser(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = service.getById(id);

            if (TryUpdateModel(user))
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
            return View(user);

        }
        public ActionResult Create()
        {
            return View(new Utilisateur());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Utilisateur utilisateur)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Erreur = "vous avez entrée des informations incompatible pour le modéle ";
                ViewBag.color = "red";

                return View("Create");
            }
            try
            {
                Utilisateur utilisateurenr = utilisateur;
                service.Add(utilisateurenr);
                service.SaveChange();
            }
            catch (Exception)
            {
                ViewBag.Erreur = "Il y'a une erreur quelque part";
                ViewBag.color = "red";

                return View("Create");
            }
            return RedirectToAction("Index");
        }

        // GET: User/Delete/5
        public ActionResult Delete(int? id)
        {
            Utilisateur user = service.getById(id);

            service.Delete(user);
            service.SaveChange();
            return RedirectToAction("Index");
        }

        // POST: User/Delete/5
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
        #endregion
        #region popUp
        [HttpGet]
        public ActionResult FindUser(int? Id)
        {
            Utilisateur item = service.getById(Id);


           // var a = new EmployeeViewModel();
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
