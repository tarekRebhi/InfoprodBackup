using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
   public interface IGroupeService : IDisposable
    {
        void Add(Groupe groupe);

        void Delete(Groupe groupe);

        void SaveChange();
        Groupe findGroupeBy(String champ);
        Groupe getById(int? id);
        Groupe getById(String champ);

        IEnumerable<Groupe> GetAll();

        void Dispose();
        Groupe getByNom(String nom);
        Groupe getConatinsByNom(string nom);
        List<Employee> getListEmployeeBySelectedSite(string Nom);
        List<Employee> getListEmployeeBySelectedSiteV5(string Nom);
    }
}
