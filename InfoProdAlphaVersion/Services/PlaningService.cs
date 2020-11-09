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
    public class PlaningService : IPlaningService
    {

        IDatabaseFactory dbfactory = null;
        IUnitOfWork uow = null;

        public PlaningService() { 
    
        dbfactory = new DatabaseFactory();
        uow = new UnitOfWork(dbfactory);
                                 }
    
        public void Add(Planing planing)
        {
            uow.PlaningRepository.Add(planing);
        }

        public void Delete(Planing planing)
        {
            uow.PlaningRepository.Delete(planing);
        }

        public void Dispose()
        {
            uow.Dispose();
        }

        public Planing findPlaningBy(string champ)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Planing> GetAll()
        {
            return uow.PlaningRepository.GetAll();
        }

        public Planing getById(string champ)
        {
            throw new NotImplementedException();
        }

        public Planing getById(int? id)
        {
            return uow.PlaningRepository.GetById(id);
        }

        public void SaveChange()
        {
             uow.Commit();
        }
    }
}
