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
    public class GroupesEmployeService : IGroupeEmployeeService
    {
        IDatabaseFactory dbfactory = null;
        IUnitOfWork uow = null;

        public GroupesEmployeService()
        {
            dbfactory = new DatabaseFactory();
            uow = new UnitOfWork(dbfactory);
        }
        public void Add(GroupesEmployees groupeemployee)
        {
            uow.GroupesEmployeesRepository.Add(groupeemployee);

        }

        public void Delete(GroupesEmployees groupeemployee)
        {
            uow.GroupesEmployeesRepository.Delete(groupeemployee);
        }

        public void Dispose()
        {
            uow.Dispose();
        }

        public GroupesEmployees findgroupeemployeeBy(string champ)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<GroupesEmployees> GetAll()
        {
            return uow.GroupesEmployeesRepository.GetAll();
        }

        public GroupesEmployees getById(string champ)
        {
            throw new NotImplementedException();
        }

        public GroupesEmployees getById(int? id)
        {
            return uow.GroupesEmployeesRepository.GetById(id);
        }
        public List<GroupesEmployees> getByIDEmployee(int? Id)
        {
            return uow.GroupesEmployeesRepository.getByIDEmployee(Id);

        }
         public  List<Groupe> getGroupeByIDEmployee(int? Id)
        {
            return uow.GroupesEmployeesRepository.getGroupeByIDEmployee(Id);

        }
        public void SaveChange()
        {
            uow.Commit();
        }
        public void deletegroupeEmployeeByName(int? Id, string name) {

            uow.GroupesEmployeesRepository.deletegroupeEmployeeByName(Id, name);
        }
       public List<Employee> getListEmployeeByGroupe(int? Id)
        {
            return uow.GroupesEmployeesRepository.getListEmployeeByGroupe(Id);
        }
        public List<Employee> getListEmployeeByGroupeId(int? groupeId)
        {
            return uow.GroupesEmployeesRepository.getListEmployeeByGroupeId(groupeId);
        }
        public  void DeletebyIdEmployee(int? Id)
        {
            uow.GroupesEmployeesRepository.DeletebyIdEmployee(Id);
        }

        
    }
}
