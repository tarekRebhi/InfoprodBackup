using Data.Repository;
using Domain.Entity;
using MyReports.Data.Infrastracture;
using MyReports.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repository
{
   public class AlerteRepository : RepositoryBase<Alerte>, IAlerteRepository
    {
        public AlerteRepository(IDatabaseFactory dbFactory)
            : base(dbFactory)
        {

        }
        ReportContext context = new ReportContext();
        public List<Alerte> getByDate(DateTime date,int reciverId)
        {

            var alertes = context.alertes.Where(a => a.dateCreation == date && a.reciverId== reciverId);
            List<Alerte> alertesTests = new List<Alerte>();
            foreach(var alerte in alertes) 
            {
                alertesTests.Add(alerte);
            }

            if (alertes != null)
            {

                return alertesTests;
            }


            else
            {
                return null;
            }
        }
        public void RemoveRecivedAlerteOfEmployee(int id)
        {
            var alertes = context.alertes.Where(s => s.senderId == id || s.reciverId==id);

            foreach (var test in alertes)
            {
                context.alertes.Remove(test);
            }
            context.SaveChanges();

        }
    }
}
