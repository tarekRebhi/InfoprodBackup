using Data;
using Domain.Entity;
using MyFinance.Data.Infrastructure;
using MyReports.Data.Infrastructure;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestFixture]
    public class Test
    {
        //[Test]
        //public void AffectEventToGroupe()
        //{
        //    using (ReportContext context = new ReportContext())
        //    {

        //        //Event eventt = new Event();
        //        //eventt.titre = "Autorisation";
        //        //eventt.description = "aaa";
        //        //eventt.dateDebut = new DateTime(2018,05,03,08,00,00);
        //        //eventt.dateFin = new DateTime(2018, 05, 04,17,00,00);
        //        //eventt.start = "08:00";
        //        //eventt.end = "17:00";

        //        //context.events.Add(eventt);
        //        //context.events.Attach(eventt);
        //        //Groupe groupe = new Groupe();
        //        //context.events.Add(eventt);
        //        //groupe.nom = "salah";
        //        //context.groupes.Add(groupe);
        //        //context.groupes.Attach(groupe);
        //        //eventt.groupes.Add(groupe);
        //        //context.SaveChanges();
        //        Groupe groupe = context.groupes.Find(8);
        //        Event evento = context.events.Find(25);
        //        evento.groupes.Add(groupe);
        //        context.SaveChanges();

        //    }
        //}
        //[Test]

        //public void AffectNewEventToExistingGroupe()
        //{
        //    using (ReportContext context = new ReportContext())
        //    {
        //        Groupe groupe = context.groupes.Find(9);
        //        Event eventt = new Event();
        //        eventt.titre = "Congé";
        //        eventt.description = "aaa";
        //        eventt.dateDebut = new DateTime(2018, 05, 03, 08, 00, 00);
        //        eventt.dateFin = new DateTime(2018, 05, 04, 17, 00, 00);
        //        eventt.start = "08:00";
        //        eventt.end = "17:00";

        //        context.events.Add(eventt);
        //        context.SaveChanges();
        //        Event eventt2 = context.events.Find(eventt.Id);
        //        eventt2.groupes.Add(groupe);
        //        context.SaveChanges();
        //    }
        //}
        //[Test]
        //public void AddEmployee()

        //{
        //    ReportContext context = new ReportContext();

        //    Employee employee = new Employee { Id = 8520, login = "salah" };
        //    context.employees.Add(employee);
        //    context.SaveChanges();


        //}
        //[Test]
        //public void DeleteEmployee() {
        //    ReportContext context1 = new ReportContext();
        //    Employee employee = new Employee();
        //     employee = context1.employees.Find(7);
        //    //Employee employee1 = uow.EmployeeRepository.GetById(6);
        //    //Employee employee2 = uow.EmployeeRepository.GetById(7);
        //    //Employee employee3 = uow.EmployeeRepository.GetById(8);
        //    //Employee employee4 = uow.EmployeeRepository.GetById(9);
        //    context1.employees.Remove(employee);
        //    context1.SaveChanges();

        //}
        //[Test]
        //public void update() {

        //    ReportContext context1 = new ReportContext();
        //    DatabaseFactory dbfactory = new DatabaseFactory();
        //    UnitOfWork uow = new UnitOfWork(dbfactory);
        //    Employee employee = new Employee();
        //    employee = uow.EmployeeRepository.GetById(6);
        //    employee.login = "souhaiel";
        //    //Employee employee1 = uow.EmployeeRepository.GetById(6);
        //    //Employee employee2 = uow.EmployeeRepository.GetById(7);
        //    //Employee employee3 = uow.EmployeeRepository.GetById(8);
        //    //Employee employee4 = uow.EmployeeRepository.GetById(9);
        //    uow.EmployeeRepository.Update(employee);
        //    uow.Commit();

        //}
        //[Test]
        //public void AffectNewGroupeToNewEvent()
        //{
        //    using (ReportContext context = new ReportContext())
        //    {
        //        Event eventt = new Event();
        //        eventt.titre = "AutorisationImedLakhel";
        //        eventt.description = "aaa";
        //        eventt.dateDebut = new DateTime(2018, 05, 03, 08, 00, 00);
        //        eventt.dateFin = new DateTime(2018, 05, 04, 17, 00, 00);
        //        eventt.start = "08:00";
        //        eventt.end = "17:00";

        //        //context.events.Add(eventt);
        //        //context.SaveChanges();
        //        Groupe groupe = new Groupe();
        //        context.events.Add(eventt);
        //        groupe.nom = "Tarek";
        //        context.groupes.Add(groupe);
        //        context.SaveChanges();
        //        Groupe groupe1 = context.groupes.Find(groupe.Id);
        //        Event evento = context.events.Find(eventt.Id);
        //        evento.groupes.Add(groupe1);
        //        context.SaveChanges();
        //    }
        //}
    }
   
}
