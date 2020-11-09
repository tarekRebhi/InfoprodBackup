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
    public class UtilisateurService : IUtilisateurService
    {
        IDatabaseFactory dbfactory = null;
        IUnitOfWork uow = null;

        public UtilisateurService()
        {
            dbfactory = new DatabaseFactory();
            uow = new UnitOfWork(dbfactory);
        }
        public void Add(Utilisateur user)
        {

            uow.UserRepository.Add(user); 
                }

        public void Delete(Utilisateur user)
        {
            uow.UserRepository.Delete(user);
        }

        public void Dispose()
        {
            uow.Dispose();
        }

        public Utilisateur findUserBy(string champ)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Utilisateur> GetAll()
        {

            return uow.UserRepository.GetAll();
        }

        public Utilisateur getBylogin(string login)
        {
            return uow.UserRepository.getByLogin(login);
        }

        public Utilisateur getById(int? id)
        {

            return uow.UserRepository.GetById(id);
        }

        public void SaveChange()
        {
            uow.Commit();
        }

        public Utilisateur getByTempSortie(string login)
        {
            List<Utilisateur> users = uow.UserRepository.getByLoginList(login);
            var user =dbfactory.DataContext.utilisateurs.FirstOrDefault(a=>a.logSortie==null);
            return user;


        }
        public Utilisateur getUserLastLogin(string login)
        {
            return uow.UserRepository.getUserLastLogin(login);
        }
    }
}
