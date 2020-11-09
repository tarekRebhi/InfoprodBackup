using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
   public interface IUtilisateurService:IDisposable
    {
        void Add(Utilisateur user);

        void Delete(Utilisateur user);

        void SaveChange();
        Utilisateur findUserBy(String champ);
        Utilisateur getById(int? id);
        Utilisateur getBylogin(String champ);
        IEnumerable<Utilisateur> GetAll();

        void Dispose();
        Utilisateur getByTempSortie(string login);
        Utilisateur getUserLastLogin(string login);

    }
}
