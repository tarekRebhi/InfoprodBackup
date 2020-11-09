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
    public class EmployeeService : IEmployeeService
    {
        IDatabaseFactory dbfactory = null;
        IUnitOfWork uow = null;

        public EmployeeService()
        {
            dbfactory = new DatabaseFactory();
            uow = new UnitOfWork(dbfactory);
        }
        public void Add(Employee employee)
        {
            uow.EmployeeRepository.Add(employee);
        }

        public void Delete(Employee employee)
        {
           //uow.EmployeeRepository.Delete(employee);
            
        }

        public void Dispose()
        {
            uow.Dispose();
            
        }

        public Employee findEmployeeBy(string champ)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Employee> GetAll()
        {
           return  uow.EmployeeRepository.GetAll();
        }

        public Employee getById(string champ)
        {
            throw new NotImplementedException();
        }

        public Employee getById(int? id)
        {
            return uow.EmployeeRepository.GetById(id);
        }

        public void SaveChange()
        {
            uow.Commit();
        }
        public Employee getByLoginUser(string login)
        {

            return uow.EmployeeRepository.getByLoginUser(login);
        }

        public Employee getByLogin(string login)
        {
            return uow.EmployeeRepository.getByLogin(login);
        }

        public Employee getByIdHermes(int idHermes)
        {

            return uow.EmployeeRepository.getByIdHermes(idHermes);
        }
        public Employee getByPseudoName(string Pseudo)
        {

            return uow.EmployeeRepository.getByPseudoName(Pseudo);
        }
    }
}
