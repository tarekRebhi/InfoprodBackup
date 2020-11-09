using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Data;
using Domain.Entity;

namespace MVCWEB.Controllers
{
    public class AlertesAPIController : ApiController
    {
        private ReportContext db = new ReportContext();

        // GET: api/AlertesControllerAPI
        public IQueryable<Alerte> Getalertes()
        {
            return db.alertes;
        }

        // GET: api/AlertesControllerAPI/5
        [ResponseType(typeof(Alerte))]
        public IHttpActionResult GetAlerte(int id)
        {
            Alerte alerte = db.alertes.Find(id);
            if (alerte == null)
            {
                return NotFound();
            }

            return Ok(alerte);
        }

        // PUT: api/AlertesControllerAPI/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutAlerte(int id, Alerte alerte)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != alerte.Id)
            {
                return BadRequest();
            }

            db.Entry(alerte).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AlerteExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/AlertesControllerAPI
        [ResponseType(typeof(Alerte))]
        public IHttpActionResult PostAlerte(Alerte alerte)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.alertes.Add(alerte);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = alerte.Id }, alerte);
        }

        // DELETE: api/AlertesControllerAPI/5
        [ResponseType(typeof(Alerte))]
        public IHttpActionResult DeleteAlerte(int id)
        {
            Alerte alerte = db.alertes.Find(id);
            if (alerte == null)
            {
                return NotFound();
            }

            db.alertes.Remove(alerte);
            db.SaveChanges();

            return Ok(alerte);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AlerteExists(int id)
        {
            return db.alertes.Count(e => e.Id == id) > 0;
        }
    }
}