using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
   public interface IGroupeEmployeeService : IDisposable
    {
        void Add(GroupesEmployees groupeemployee);

        void Delete(GroupesEmployees groupeemployee);

        void SaveChange();
        GroupesEmployees findgroupeemployeeBy(String champ);
        GroupesEmployees getById(int? id);
        GroupesEmployees getById(String champ);
        IEnumerable<GroupesEmployees> GetAll();
        List<GroupesEmployees> getByIDEmployee(int? Id);
        List<Groupe> getGroupeByIDEmployee(int? Id);
        void deletegroupeEmployeeByName(int? Id, string name);
        List<Employee> getListEmployeeByGroupe(int? Id);
        List<Employee> getListEmployeeByGroupeId(int? groupeId);
        void DeletebyIdEmployee(int? Id);
        void Dispose();
    }
}
