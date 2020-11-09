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
    public class GroupeService : IGroupeService
    {

        IDatabaseFactory dbfactory = null;
        IUnitOfWork uow = null;

        public GroupeService()
        {
            dbfactory = new DatabaseFactory();
            uow = new UnitOfWork(dbfactory);
        }
        public void Add(Groupe groupe)
        {
           uow.GroupeRepository.Add(groupe);
        }

        public void Delete(Groupe groupe)
        {
            uow.GroupeRepository.Delete(groupe);
        }

        public void Dispose()
        {
            uow.Dispose();
        }

        public Groupe findGroupeBy(string champ)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Groupe> GetAll()
        {
            return uow.GroupeRepository.GetAll();
        }

        public Groupe getById(string champ)
        {
            throw new NotImplementedException();
        }

        public Groupe getById(int? id)
        {
            return uow.GroupeRepository.GetById(id);
        }

        public void SaveChange()
        {
            uow.Commit();
        }
        public Groupe getByNom(string nom)
        {

            return uow.GroupeRepository.getByNom(nom);
        }
        public Groupe getConatinsByNom(string nom) {
            return uow.GroupeRepository.getConatinsByNom(nom);
        }
        public List<Employee> getListEmployeeBySelectedSite(string Nom)
        {
            return uow.GroupeRepository.getListEmployeeBySelectedSite(Nom);
        }
        public List<Employee> getListEmployeeBySelectedSiteV5(string Nom)
        {
            return uow.GroupeRepository.getListEmployeeBySelectedSiteV5(Nom);
        }
    }
}
