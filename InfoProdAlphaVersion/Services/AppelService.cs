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
    public class AppelService : IAppelService
    {
        IDatabaseFactory dbfactory = null;
        IUnitOfWork uow = null;
        public AppelService()
        {
            dbfactory = new DatabaseFactory();
            uow = new UnitOfWork(dbfactory);
        }
        public void Add(Appel appel)
        {
            uow.AppelRepository.Add(appel);
        }

        public void Delete(Appel appel)
        {
            uow.AppelRepository.Delete(appel);
        }

        public void Dispose()
        {
            uow.Dispose();
        }

        public Appel findAppelBy(string champ)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Appel> GetAll()
        {
            return uow.AppelRepository.GetAll();
        }

        public Appel getById(string champ)
        {
            throw new NotImplementedException();
        }

        public Appel getById(int? id)
        {
            return uow.AppelRepository.GetById(id);
        }

        public void SaveChange()
        {
            uow.Commit();        }
    }
}
