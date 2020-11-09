using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entity;
using MyReports.Data.Infrastructure;
using MyFinance.Data.Infrastructure;

namespace Services
{
    public class AlerteService : IAlerteService

    {

        IDatabaseFactory dbfactory = null;
        IUnitOfWork uow = null;

        public AlerteService()
        {
            dbfactory = new DatabaseFactory();
            uow = new UnitOfWork(dbfactory);
        }
        public void Add(Alerte alerte)
        {
            uow.AlerteRepository.Add(alerte);
        }

        public void Delete(Alerte alerte)
        {
            uow.AlerteRepository.Delete(alerte);
        }

        public void Dispose()
        {
            uow.Dispose();
        }

        public Alerte findAlerteBy(string champ)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Alerte> GetAll()
        {
            return uow.AlerteRepository.GetAll();
            
        }

        public Alerte getById(string champ)
        {
            throw new NotImplementedException();
        }

        public Alerte getById(int? id)
        {
            return uow.AlerteRepository.GetById(id);
        }

        public void SaveChange()
        {
            uow.Commit();
         }
       public List<Alerte> getByDate(DateTime date, int reciverId)
        {
            return uow.AlerteRepository.getByDate(date,reciverId);
        }
        public void RemoveRecivedAlerteOfEmployee(int id)
        {

            uow.AlerteRepository.RemoveRecivedAlerteOfEmployee(id);
        }

    }
}
