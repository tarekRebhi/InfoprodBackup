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
    public class IndicateurService : IIndicateurService
    {
        IDatabaseFactory dbfactory = null;
        IUnitOfWork uow = null;

        public IndicateurService()
        {
            dbfactory = new DatabaseFactory();
            uow = new UnitOfWork(dbfactory);
        }
        public void Add(Indicateur indicateur)
        {
            uow.IndicateurRepository.Add(indicateur);
        }

        public void Delete(Indicateur indicateur)
        {
            uow.IndicateurRepository.Delete(indicateur);

        }

        public void Dispose()
        {
            uow.Dispose();
        }

        public Indicateur findIndicateurBy(string champ)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Indicateur> GetAll()
        {
            return uow.IndicateurRepository.GetAll();
        }

        public Indicateur getById(string champ)
        {
            throw new NotImplementedException();
        }

        public Indicateur getById(int? id)
        {
            return uow.IndicateurRepository.GetById(id);
        }

        public void SaveChange()
        {
            uow.Commit();        }
    }
}
