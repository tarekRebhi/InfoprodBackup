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
    public class GroupesAPIController : ApiController
    {
        private ReportContext db = new ReportContext();

        // GET: api/GroupesAPI
        public IQueryable<Groupe> Getgroupes()
        {
            return db.groupes;
        }

        // GET: api/GroupesAPI/5
        [ResponseType(typeof(Groupe))]
        public IHttpActionResult GetGroupe(int id)
        {
            Groupe groupe = db.groupes.Find(id);
            if (groupe == null)
            {
                return NotFound();
            }

            return Ok(groupe);
        }

        // PUT: api/GroupesAPI/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutGroupe(int id, Groupe groupe)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != groupe.Id)
            {
                return BadRequest();
            }

            db.Entry(groupe).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GroupeExists(id))
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

        // POST: api/GroupesAPI
        [ResponseType(typeof(Groupe))]
        public IHttpActionResult PostGroupe(Groupe groupe)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.groupes.Add(groupe);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = groupe.Id }, groupe);
        }

        // DELETE: api/GroupesAPI/5
        [ResponseType(typeof(Groupe))]
        public IHttpActionResult DeleteGroupe(int id)
        {
            Groupe groupe = db.groupes.Find(id);
            if (groupe == null)
            {
                return NotFound();
            }

            db.groupes.Remove(groupe);
            db.SaveChanges();

            return Ok(groupe);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool GroupeExists(int id)
        {
            return db.groupes.Count(e => e.Id == id) > 0;
        }
    }
}