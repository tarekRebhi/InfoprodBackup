using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
   public interface IEmployeeService :IDisposable
    {
        void Add(Employee employee);

        void Delete(Employee employee);

        void SaveChange();
        Employee findEmployeeBy(String champ);
        Employee getById(int? id);
        Employee getById(String champ);
        IEnumerable<Employee> GetAll();

        void Dispose();
        Employee getByLoginUser(string login);
        Employee getByLogin(string login);

        Employee getByIdHermes(int idHermes);
        Employee getByPseudoName(string Pseudo);
    }
}
