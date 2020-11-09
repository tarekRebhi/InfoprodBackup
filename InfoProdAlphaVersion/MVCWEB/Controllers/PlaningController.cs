using Domain.Entity;
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
    public class PlaningController : Controller
    {
        // GET: Planing
        IPlaningService service;
        public PlaningController()
        {

            service = new PlaningService();
        }
        public ActionResult Index(String search, FormCollection form)
        {
            try
            {
                //string value = (string)Session["loginIndex"];
                var planings = service.GetAll();
                List<Planing> fVM = new List<Planing>();
                //string type = form["test"].ToString();
                //int numVal = Int32.Parse(type);

                foreach (var item in planings)
                {
                    fVM.Add(item);
                }
                //if (!String.IsNullOrEmpty(search))
                //{

                //    fVM = fVM.Where(p => p.login.ToLower().Contains(search.ToLower())).ToList<Employee>();


                //}
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

        // GET: Employee/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Planing planing = service.getById(id);
            return View(planing);
        }

        public ActionResult EnregistrerPlaning(DateTime NewPlanDate, DateTime NewPlanDate2, string NewPlanTime, string NewPlanTime2,string planGroups)
        {
            Planing planing = new Planing
            {
                
                dateDebut= NewPlanDate,
                dateFin = NewPlanDate2,
                heureDebut = NewPlanTime,
                heureFin = NewPlanTime2

            };
            service.Add(planing);
            service.SaveChange();
            return View("~/Views/Calendar/index.cshtml");
        }

        // GET: Employee/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Planing planing = service.getById(id);

            return View(planing);
        }

        // POST: Medcin/Edit/5
        [HttpPost, ActionName("Edit")]
        public ActionResult EditEmployee(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var planing = service.getById(id);

            if (TryUpdateModel(planing))
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
            return View(planing);

        }

        // GET: Employee/Delete/5
        public ActionResult Delete(int? id)
        {

            Planing planing = service.getById(id);

            service.Delete(planing);
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
        public ActionResult FindPlaning(int? Id)
        {
            Planing item = service.getById(Id);


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
