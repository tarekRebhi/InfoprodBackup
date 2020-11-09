using Domain.Entity;
using MyReports.Data.Infrastracture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repository
{
   public interface IGroupeRepository : IRepositoryBase<Groupe>
    {
        Groupe getByNom(string nom);
        List<Groupe> getListByCorrespondance(String nomCorrespondance);
        Groupe getConatinsByNom(string nom);
        List<Employee> getListEmployeeBySelectedSite(string Nom);
        List<Employee> getListEmployeeBySelectedSiteV5(string Nom);

    }
}
