using Domain.Entity;
using MyReports.Data.Infrastracture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repository
{
    public interface IUtilisateurRepository: IRepositoryBase<Utilisateur>
    {
        Utilisateur getByLogin(string login) ;
         List<Utilisateur> getByLoginList(string login);

        Utilisateur getUserLastLogin(string login);


    }
}
