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
using Services;

namespace MVCWEB.Controllers
{
    public class GrilleEvaluationsController : Controller
    {
        private ReportContext db = new ReportContext();
        IEmployeeService serviceEmployee;
        public GrilleEvaluationsController()
        {
            serviceEmployee = new EmployeeService();
        }
        // GET: GrilleEvaluations
        public ActionResult Index()
        {
            var evaluations = db.evaluations.Include(g => g.employee);
            return View(evaluations.ToList());
        }

        // GET: GrilleEvaluations/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    GrilleEvaluation grilleEvaluation = db.evaluations.Find(id);
        //    if (grilleEvaluation == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(grilleEvaluation);
        //}

        //// GET: GrilleEvaluations/Create
        //public ActionResult Create()
        //{
        //    ViewBag.employeeId = new SelectList(db.Employees, "Id", "pseudoName");
        //    return View();
        //}

        //// POST: GrilleEvaluations/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,acceuilPresentation,objetAppel,presentationOffre,gestionObjection,vCContrat,pCross,discours,attitude,priseConge,decouverteBesoins,ppOffre,dateTempEvaluation,type,employeeId,note,commentaireQualite,commentaireAgent,enregistrementFullName,enregistrementUrl,enregistrementDirectory")] GrilleEvaluation grilleEvaluation)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.evaluations.Add(grilleEvaluation);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.employeeId = new SelectList(db.Employees, "Id", "pseudoName", grilleEvaluation.employeeId);
        //    return View(grilleEvaluation);
        //}

        // GET: GrilleEvaluations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GrilleEvaluation grilleEvaluation = db.evaluations.Find(id);
            if (grilleEvaluation == null)
            {
                return HttpNotFound();
            }
            //  ViewBag.employeeId = new SelectList(db.Employees, "Id", "pseudoName", grilleEvaluation.employeeId);
            // var employeeId = serviceEmployee.getById(grilleEvaluation.employeeId);
            return View(grilleEvaluation);
        }

        // POST: GrilleEvaluations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? id, GrilleEvaluation grilleEvaluation)
        {
            //GrilleEvaluation grilleEvaluation = db.evaluations.Find(id);
            if (ModelState.IsValid)
            {
               
                db.Entry(grilleEvaluation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
          //  ViewBag.employeeId = serviceEmployee.getById(grilleEvaluation.employeeId);
            // ViewBag.employeeId = new SelectList(db.Employees, "Id", "pseudoName", grilleEvaluation.employeeId);
            return View(grilleEvaluation);
        }

        // GET: GrilleEvaluations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GrilleEvaluation grilleEvaluation = db.evaluations.Find(id);
            if (grilleEvaluation == null)
            {
                return HttpNotFound();
            }
            return View(grilleEvaluation);
        }

        // POST: GrilleEvaluations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            GrilleEvaluation grilleEvaluation = db.evaluations.Find(id);
            db.evaluations.Remove(grilleEvaluation);
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
