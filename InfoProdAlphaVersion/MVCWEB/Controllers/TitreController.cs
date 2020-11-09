using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Net;
using Services;
using Domain.Entity;

namespace MVCWEB.Controllers
{
    public class TitreController : Controller
    {

        ITitreService service;
        public TitreController()
        {

            service = new TitreService();
        }

        public ActionResult Index(string search, FormCollection form)
        {
            //string value = (string)Session["loginIndex"];
            try {
                var titres = service.GetAll();
                List<Titre> fVM = new List<Titre>();
                //string type = form["test"].ToString();
                //int numVal = Int32.Parse(type);

                foreach (var item in titres)
                {
                    fVM.Add(item);
                }
                if (!String.IsNullOrEmpty(search))
                {

                    fVM = fVM.Where(p => p.libelle.ToLower().Contains(search.ToLower())).ToList<Titre>();


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
            catch(NullReferenceException a)
            {
                Console.WriteLine(a);
                return View("~/Views/Authentification/Index.cshtml");
            }
        }


        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Titre titre = service.getById(id);
            return View(titre);
        }

        public ActionResult Create()
        {
            var titre = new Titre();
            return View(titre);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Titre t, FormCollection form)
        {
            if (!ModelState.IsValid)
            {
                RedirectToAction("Create");
            }
            Titre titre = new Titre
            {
                Id = t.Id,
                activite = t.activite,
                type = t.type,
                libelle = t.libelle,
                codeOperation = t.codeOperation,
                codeProvRelance = t.codeProvRelance,
                dateInjection = t.dateInjection,
                nombreFichesInjectees = t.nombreFichesInjectees

            };
            service.Add(titre);
            service.SaveChange();
            return RedirectToAction("Index");
        }


        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Titre titre = service.getById(id);

            return View(titre);
        }


        [HttpPost, ActionName("Edit")]
        public ActionResult EditTitre(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var titre = service.getById(id);

            if (TryUpdateModel(titre))
            {
                try
                {
                    service.SaveChange();
                    return RedirectToAction("Index");
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "error inputs");
                }
            }
            return View(titre);

        }


        public ActionResult Delete(int? id)
        {

            Titre titre = service.getById(id);

            service.Delete(titre);
            service.SaveChange();
            return RedirectToAction("Index");
        }


        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {


                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [HttpGet]
        public ActionResult FindTitre(int? Id)
        {
            Titre item = service.getById(Id);


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
    }
}
