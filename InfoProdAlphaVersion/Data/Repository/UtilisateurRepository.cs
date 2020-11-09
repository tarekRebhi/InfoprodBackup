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
   public class UtilisateurRepository : RepositoryBase<Utilisateur>, IUtilisateurRepository
    {
        public UtilisateurRepository(IDatabaseFactory dbFactory)
            : base(dbFactory)
        {

        }
        ReportContext context = new ReportContext();
        public Utilisateur getByLogin(string login)
        {

            var user = context.utilisateurs.FirstOrDefault(a => a.login == login);


            if (user != null)
            {

                return user;
            }


            else
            {
                return null;
            }
            }
        public List<Utilisateur> getByLoginList(string login) {
            var usersquery = context.utilisateurs.Where(a => a.login == login);

            List<Utilisateur> users = new List<Utilisateur>();

            if (usersquery != null)
            {
                foreach (Utilisateur item in usersquery) {

                    users.Add(item);
                }

                return users;
            }


            else
            {
                return null;
            }

        }
        //public Utilisateur getUserLastLogin(string login)
        //{

        //    var user = context.utilisateurs.FirstOrDefault(a => a.login == login);


        //    if (user != null)
        //    {

        //        return user;
        //    }


        //    else
        //    {
        //        return null;
        //    }
        //}

        public Utilisateur getUserLastLogin(string login)
        {

            var user = context.utilisateurs.Where(a => a.login == login).ToList().Last();


            if (user != null)
            {
                user.logSortie = DateTime.Now;
                context.SaveChanges();
                return user;
            }


            else
            {
                return null;
            }
        }
    }
}
