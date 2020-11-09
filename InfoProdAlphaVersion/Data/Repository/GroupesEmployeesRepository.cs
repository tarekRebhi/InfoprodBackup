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
    public class GroupesEmployeesRepository :RepositoryBase<GroupesEmployees>, IGroupesEmployeesRepository
    {
        public GroupesEmployeesRepository(IDatabaseFactory dbFactory)
            : base(dbFactory)
        {

        }
        ReportContext context = new ReportContext();
        //ReportIdentityTestContext context2 = new ReportIdentityTestContext();
        public List<GroupesEmployees> getByIDEmployee(int? Id)
        {

            var groupes = context.employeesGroupes.Where(a => a.employeeId == Id);
            List<GroupesEmployees> groupesEmp = new List<GroupesEmployees>();
            foreach (var test in groupes)
            {
                groupesEmp.Add(test);

            }

            if (groupes != null)
            {

                return groupesEmp;
            }


            else
            {
                return null;
            }
        }
        public void DeletebyIdEmployee(int? Id)
        {

            var groupesEmp = context.employeesGroupes.Where(a => a.employeeId == Id);
            foreach (var test in groupesEmp)
            {
                context.employeesGroupes.Remove(test);

            }

            context.SaveChanges();


           
        }
        public List<Groupe> getGroupeByIDEmployee(int? Id)
        {
            List<GroupesEmployees> groupesEmp = new List<GroupesEmployees>();

            groupesEmp = getByIDEmployee(Id);
            List<Groupe> groupes = new List<Groupe>();
            foreach(var test in groupesEmp)
            {
               Groupe groupe= context.groupes.Find(test.groupeId);

                groupes.Add(groupe);


            }
            return groupes;
        }
        public List<Employee> getListEmployeeByGroupe(int? IdEmployee)
        {
            ReportContext context2 = new ReportContext();

            List<Groupe> groupes = getGroupeByIDEmployee(IdEmployee);
            List<Employee> employees = new List<Employee>();
            foreach(var groupe in groupes)
            {
                var groupesEmps =context.employeesGroupes.Where(a => a.groupeId == groupe.Id);
                foreach(var groupesEmp in groupesEmps)
                {
                    var emp = context2.Users.FirstOrDefault(a => a.Id == groupesEmp.employeeId);
                    employees.Add(emp);
                }
            }

            return employees;
        }
        public List<Employee> getListEmployeeByGroupeId(int? groupeId)
        {
            ReportContext context2 = new ReportContext();
            List<Employee> employees = new List<Employee>();
           
                var groupesEmps = context2.employeesGroupes.Where(a => a.groupeId == groupeId);
                foreach (var groupesEmp in groupesEmps)
                {
                    var emp = context.Users.FirstOrDefault(a => a.Id == groupesEmp.employeeId);
                    employees.Add(emp);
                }
           

            return employees;
        }
       
        //{"Un DataReader associé à cette Command est déjà ouvert. Il doit d'abord être fermé."}
        //c'est pour ca qu'il y'a deux context; 
        public void deletegroupeEmployeeByName(int?Id ,String name)
        {

            List<GroupesEmployees> groupesEmp = new List<GroupesEmployees>();

            groupesEmp = getByIDEmployee(Id);


            foreach (var test in groupesEmp)
            {
                Groupe groupe = context.groupes.Find(test.groupeId);

                if((groupe.nom).Equals(name))
                {
                    GroupesEmployees emp = new GroupesEmployees();
                     emp = context.employeesGroupes.Find(test.Id);
                    context.employeesGroupes.Remove(emp);
                    context.SaveChanges();
                }
            }
        }
      
    }
}
