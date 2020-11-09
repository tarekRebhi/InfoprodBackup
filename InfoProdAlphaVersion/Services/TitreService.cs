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
    public class TitreService : ITitreService
    {
        IDatabaseFactory dbfactory = null;
        IUnitOfWork uow = null;

        public TitreService()
        {
            dbfactory = new DatabaseFactory();
            uow = new UnitOfWork(dbfactory);
        }
        public void Add(Titre titre)
        {
            uow.TitreRepository.Add(titre);
        }

        public void Delete(Titre titre)
        {
            uow.TitreRepository.Delete(titre);
        }

        public void Dispose()
        {
            uow.Dispose();
        }

        public Titre findEmployeeBy(string champ)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Titre> GetAll()
        {
            return uow.TitreRepository.GetAll();
        }

        public Titre getById(string champ)
        {
            throw new NotImplementedException();
        }

        public Titre getById(int? id)
        {
            return uow.TitreRepository.GetById(id);
        }

        public void SaveChange()
        {
            uow.Commit();
                }
    }
}
