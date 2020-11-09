using Domain.Entity;
using MyReports.Data.Infrastracture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repository
{
   public interface IGroupesEmployeesRepository : IRepositoryBase<GroupesEmployees>
    {
         List<GroupesEmployees> getByIDEmployee(int? Id);
        List<Groupe> getGroupeByIDEmployee(int? Id);
        void deletegroupeEmployeeByName(int? Id, string name);
        List<Employee> getListEmployeeByGroupe(int? Id);
        List<Employee> getListEmployeeByGroupeId(int? groupeId);
      
        void DeletebyIdEmployee(int? Id);
    }
}
