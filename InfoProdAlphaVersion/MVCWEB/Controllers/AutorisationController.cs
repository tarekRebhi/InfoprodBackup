using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Data;
using Domain.Entity;
using Services;
using MVCWEB.Models;

namespace MVCWEB.Controllers
{
    public class AutorisationController : Controller
    {

        IEventService service;
        IGroupeService serviceGroupe;
        IUtilisateurService serviceUser;
        IEmployeeService serviceEmployees;
        IGroupeEmployeeService serviceGroupeEmp;
        public AutorisationController()
        {
            service = new EventService();
            serviceGroupe = new GroupeService();
            serviceUser = new UtilisateurService();
            serviceEmployees = new EmployeeService();
            serviceGroupeEmp = new GroupesEmployeService();
        }
        // GET: Autorisation
        [HttpGet]
        [CustomAuthorise(Roles = "Admin")]

        public ActionResult Index()
        {
            String login = Session["loginIndex"].ToString();
            Employee item = serviceEmployees.getByLoginUser(login);
            var groupesassociees = serviceGroupeEmp.getGroupeByIDEmployee(item.Id);
            var usersAssociees = serviceGroupeEmp.getListEmployeeByGroupe(item.Id);
            EventViewModel a = new EventViewModel();
            var logins = serviceUser.GetAll();
            var tests = usersAssociees.Select(o => o.userLogin).Distinct().ToList();
            foreach (String test in tests)
            {
                a.utilisateurs.Add(new SelectListItem { Text = test, Value = test });
            }
            var groupes = serviceGroupe.GetAll();
            var groupeslogins = groupesassociees.Select(o => o.nom).Distinct().ToList();
            foreach (var test in groupeslogins)
            {
                //foreach(var assoc in groupesassociees){
                //    if (!(test.nom).Equals(assoc.nom)){
                a.groupesass.Add(new SelectListItem { Text = test, Value = test });
                //}
            }
            return View(a);
        }
        [HttpPost]
        [Authorize(Roles = "Manager,SuperManager")]
        public ActionResult EnregistrerEvent(string NewPlanTitre, string NewPlanDescription, DateTime NewPlanDate, DateTime NewPlanDate2, string NewPlanTime, string NewPlanTime2, string NewplanGroups, string ThemeColor, string NewplanUser)
        {
            var events = service.GetAll();
            TimeSpan diff = NewPlanDate2 - NewPlanDate;
            double hours = diff.TotalHours;
            int days = diff.Days;
            float seconds = diff.Seconds;
            double test = seconds / 3600.0;
            double x = hours / 24;
            double difference = x - Math.Truncate(x);
            if (NewPlanDate.DayOfWeek == DayOfWeek.Saturday || NewPlanDate2.DayOfWeek == DayOfWeek.Sunday)
            {
                return PartialView("_AlerteDayOfWeek");
            }
            if (NewplanUser != "")
            {

                Employee emp = serviceEmployees.getByLogin(NewplanUser);
                List<Groupe> groupes = serviceGroupeEmp.getGroupeByIDEmployee(emp.Id);

                if (NewPlanTitre.Equals("Autorisation"))
                {
                    ThemeColor = "#cc0000";
                    if (hours == 0 || hours > 2)
                    {
                        return PartialView("_AlerteAutorisation");

                    }
                    var conges = service.listEventAutorisations(NewplanUser);
                    foreach (var conge in conges)
                    {

                        if (NewPlanDate < conge.dateFin && NewPlanDate2 >= conge.dateDebut)
                        {
                            return PartialView("_AlerteAutorisationSameDay");

                        }// si il y'a deux autorisations au meme temps ou chavauchent dans le meme temps donc erreur
                        //return RedirectToAction("Calendar", "Home");
                    }
                    var others = events.Where(a => a.titre != "Autorisation" && a.employeeId == emp.Id || a.titre != "Autorisation" && a.employeeId == null);
                    foreach (var autorisation in others)
                    {
                        if (autorisation.employeeId == null)
                        {
                            foreach (var groupe in groupes)
                            {
                                if (groupe.nom.Equals(autorisation.start))
                                {
                                    if (NewPlanDate.Date == autorisation.dateDebut.Date || NewPlanDate.Date == autorisation.dateFin.Date)
                                    {
                                        ViewBag.message = "vous ne pouvez pas ajouter une autorisation dans une date ou il existe un autre évenement par activité Fermeture Annuel/Jours fériés ";
                                        return PartialView("_AlerteAutorisationHoliday");

                                    }
                                }
                            }
                        }

                        else
                        {
                            if (NewPlanDate.Date == autorisation.dateDebut.Date || NewPlanDate.Date == autorisation.dateFin.Date)
                            {
                                ViewBag.message = "vous ne pouvez pas ajouter une autorisation dans une date ou vous avez alloué un congé pour cet Agent ";

                                return PartialView("_AlerteAutorisationHoliday");

                            }
                        }

                        //return RedirectToAction("Calendar", "Home");
                    }

                }
                else if (NewPlanTitre == "Congé")
                {
                    ThemeColor = "#26A69A";
                    var conges = service.listEventHolidays(NewplanUser);
                    foreach (var conge in conges)
                    {
                        if (NewPlanDate <= conge.dateFin && NewPlanDate2 >= conge.dateDebut)
                        {
                            return PartialView("_AlerteCongéSameDay");

                        }
                        //return RedirectToAction("Calendar", "Home");
                    }

                    var others = events.Where(a => a.titre != "Congé" && a.employeeId == emp.Id || a.titre != "Congé" && a.employeeId == null);

                    //var tests = service.listEventAutorisations(NewplanUser);
                    foreach (var conge in others)
                    {
                        if (conge.employeeId == null)
                        {
                            foreach (var groupe in groupes)
                            {
                                if (groupe.nom.Equals(conge.start))
                                {
                                    if (NewPlanDate.Date <= conge.dateFin.Date && NewPlanDate2.Date >= conge.dateDebut.Date)
                                    {
                                        ViewBag.message = "vous ne pouvez pas ajouter un congé pour un agent dans une date ou il existe un autre évenement par activité Fermeture Annuel/Jours fériés du service dans lequel se trouve l'agent ";
                                        return PartialView("_AlerteAutorisationHoliday");

                                    }
                                }
                            }
                        }
                        else
                        {
                            if (NewPlanDate.Date <= conge.dateDebut.Date && NewPlanDate2.Date >= conge.dateFin.Date)
                            {
                                ViewBag.message = "vous ne pouvez pas ajouter un congé dans une date ou vous avez alloué une autorisation pour cet Agent ";

                                return PartialView("_AlerteAutorisationHoliday");

                            }
                        }
                        if ((days == 0 && hours < 4 || (hours > 4 && hours < 8)))
                        {
                            return PartialView("_AlerteCongé");

                        }
                        if ((days > 0 && (difference < 0.166666666666666) || (difference > 0.166666666666667 && difference < 0.33)))
                        {


                            ViewBag.a = difference;

                            return PartialView("_AlerteCongé");


                        }
                        //if (NewPlanDate.Date < conge.dateFin.Date && NewPlanDate2.Date >= conge.dateDebut.Date)

                        //return RedirectToAction("Calendar", "Home");
                    }

                }
            }
            else if (NewplanGroups != null)
            {

                if (NewPlanTitre == "Jours Fériés")
                {
                    ThemeColor = "#0099ff";
                    var joursFeriés = events.Where(a => a.titre == "Jours Fériés" && a.start.Equals(NewplanGroups));

                    foreach (var joursFer in joursFeriés)
                    {
                        if (NewPlanDate.Date <= joursFer.dateFin.Date && NewPlanDate2.Date >= joursFer.dateDebut.Date)
                        {
                            ViewBag.A = "Votre demande n'est pas effectuée a cause de l'existance d'un autre évenement de type jours fériés existant dans la même période temporaire pour le même groupe";
                            return PartialView("_AlertePJF");

                        }
                    }
                    var others = events.Where(a => a.titre != "Jours Fériés");
                    foreach (var JF in others)
                    {
                        if (JF.start != null && JF.start.Equals(NewplanGroups))
                        {
                            if (NewPlanDate.Date <= JF.dateFin.Date && NewPlanDate2.Date >= JF.dateDebut.Date)
                            {
                                ViewBag.A = "Votre demande n'est pas effectuée a cause de l'existance d'un autre évenement de type Planning / Fermeture Annuelle dans la même durée du même groupe ";
                                return PartialView("_AlertePJF");

                            }
                        }
                        else if (JF.start == null)
                        {
                            try
                            {
                                Groupe groupe = serviceGroupe.getByNom(NewplanGroups);

                                //Employee employe= serviceEmployees.getById(JF.employeeId);

                                List<Employee> employees = serviceGroupeEmp.getListEmployeeByGroupeId(groupe.Id);
                                foreach (var agent in employees)
                                {
                                    if (agent.Id == JF.employeeId)
                                    {
                                        if (NewPlanDate.Date <= JF.dateFin.Date && NewPlanDate2.Date >= JF.dateDebut.Date)
                                        {
                                            ViewBag.A = "Votre demande n'est pas effectuée a cause de l'existance d'un autre évenement de type congé/Autorisation des agent dans la même durée du même groupe";
                                            return PartialView("_AlertePJF");

                                        }

                                    }
                                }

                            }
                            catch (NullReferenceException a)
                            {
                                Console.WriteLine(a);
                                return RedirectToAction("Index", "Authentification");
                            }
                        }
                    }
                }
                else if (NewPlanTitre == "Fermeture Annuel")
                {
                    ThemeColor = "#5C6BC0";
                    var joursFeriés = events.Where(a => a.titre == "Fermeture Annuel" && a.start.Equals(NewplanGroups));

                    foreach (var joursFer in joursFeriés)
                    {
                        if (NewPlanDate.Date <= joursFer.dateFin.Date && NewPlanDate2.Date >= joursFer.dateDebut.Date)
                        {
                            ViewBag.A = "Votre demande n'est pas effectuée a cause de l'existance d'un autre évenement de type Fermeturel Annuelle dans la meme durée du meme groupe";
                            return PartialView("_AlertePJF");

                        }
                    }
                    var others = events.Where(a => a.titre != "Fermeture Annuel");
                    foreach (var JF in others)
                    {
                        if (JF.start != null && JF.start == NewplanGroups)
                        {
                            if (NewPlanDate.Date <= JF.dateFin.Date && NewPlanDate2.Date >= JF.dateDebut.Date)
                            {
                                ViewBag.A = "Votre demande n'est pas effectuée a cause de l'existance d'un autre évenement de type Plannig/Jours Fériés dans la meme durée du meme groupe ";
                                return PartialView("_AlertePJF");

                            }
                        }
                        else if (JF.start == null)
                        {
                            try
                            {
                                Groupe groupe = serviceGroupe.getByNom(NewplanGroups);

                                //Employee employe= serviceEmployees.getById(JF.employeeId);

                                List<Employee> employees = serviceGroupeEmp.getListEmployeeByGroupeId(groupe.Id);
                                foreach (var agent in employees)
                                {
                                    if (agent.Id == JF.employeeId)
                                    {
                                        if (NewPlanDate.Date <= JF.dateFin.Date && NewPlanDate2.Date >= JF.dateDebut.Date)
                                        {
                                            ViewBag.A = "Votre demande n'est pas effectuée a cause de l'existance d'un autre évenement de type congé/Autorisation des agent dans la meme durée du meme groupe";
                                            return PartialView("_AlertePJF");

                                        }

                                    }
                                }

                            }
                            catch (NullReferenceException a)
                            {
                                Console.WriteLine(a);
                                ViewBag.A = "   Nope there is something which is null";
                                return PartialView("_AlertePJF");
                                //return RedirectToAction("Index", "Authentification");
                            }
                        }
                    }
                }

                else if (NewPlanTitre == "Planning")
                {
                    ThemeColor = "#ff66ff";
                }
            }


            using (ReportContext context = new ReportContext())
            {
                Event eventt = new Event();
                if (NewplanUser != "")
                {
                    Employee emp = context.Users.FirstOrDefault(c => c.UserName == NewplanUser);


                    eventt.titre = NewPlanTitre;
                    eventt.description = NewPlanDescription;
                    eventt.dateDebut = NewPlanDate;
                    eventt.dateFin = NewPlanDate2;
                    eventt.start = NewPlanTime;
                    eventt.end = NewPlanTime2;
                    eventt.themeColor = ThemeColor;
                    eventt.employeeId = emp.Id;



                    context.events.Add(eventt);
                    context.SaveChanges();

                }
                if (NewplanGroups != "")
                {
                    Groupe a = context.groupes.FirstOrDefault(c => c.nom == NewplanGroups);
                    eventt.titre = NewPlanTitre;
                    eventt.description = NewPlanDescription;
                    eventt.dateDebut = NewPlanDate;
                    eventt.dateFin = NewPlanDate2;
                    eventt.start = a.nom;//NewPlanTime;
                    eventt.end = NewPlanTime2;
                    eventt.themeColor = ThemeColor;
                    context.events.Add(eventt);
                    context.SaveChanges();
                    Event eventt2 = context.events.Find(eventt.Id);
                    eventt2.groupes.Add(a);
                    context.SaveChanges();
                    //it's imed i m using the context directly because i have two service and they call each a different instance of context and in this case we must have a single context to affect  two objects existing in the same context.
                }
                return RedirectToAction("Index", "Home");

            }
        }

        public ActionResult GetEvents()
        {
            using (ReportContext dc = new ReportContext())
            {
                var events = dc.events.ToList();
                foreach (var test in events)
                {
                    test.dateDebut = DateTime.Now;
                    test.dateFin = DateTime.Now;
                }
                ViewBag.eventsTest = events;
                return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }



    }
}
