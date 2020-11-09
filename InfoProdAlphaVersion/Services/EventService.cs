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
    public class EventService : IEventService
    {
        IDatabaseFactory dbfactory = null;
        IUnitOfWork uow = null;

        public EventService()
        {
            dbfactory = new DatabaseFactory();
            uow = new UnitOfWork(dbfactory);
        }
        public void Add(Event even)
        {
            uow.EventRepository.Add(even);
        }

        public void AffectGroupeTOEvent(Groupe groupe, Event eventt)
        {
            uow.EventRepository.AffectGroupeTOEvent(groupe,eventt);
        }

        public void Delete(Event even)
        {
            uow.EventRepository.Delete(even);
        }

        public void Dispose()
        {
            uow.Dispose();

        }

        public Event findEventBy(string champ)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Event> GetAll()
        {
            return uow.EventRepository.GetAll();
        }

        public Event getById(string champ)
        {
            throw new NotImplementedException();
        }

        public Event getById(int? id)
        {
            return uow.EventRepository.GetById(id);
        }

        public List<Event> getGroupesListByEvent(int id)
        {
            return uow.EventRepository.getGroupesListByEvent(id);
        }

        public List<Event> getListEventsByEmployeeId(int id)
        {
            return uow.EventRepository.getListEventsByEmployeeId(id);
        }

        public List<Event> getListEventsByListGroupes(List<Groupe> groupes)
        {
            return uow.EventRepository.getListEventsByListGroupes(groupes);
        }

        public void SaveChange()
        {
            uow.Commit();
        }
        public void RemoveEventsOfEmployee(int id) {

            uow.EventRepository.RemoveEventsOfEmployee(id);
        }

        public List<Event> listEventHolidays(string userName)
        {
          return  uow.EventRepository.listEventHolidays(userName);

        }

        public List<Event> listEventAutorisations(string userName)
        {

            return uow.EventRepository.listEventAutorisations(userName);
        }
        public List<Event> listEventHolidaysAutorisations(String userName) {
            return uow.EventRepository.listEventHolidaysAutorisations(userName);

        }

        public List<Event> listEventsOthers()
        {
            return uow.EventRepository.listEventsOthers();
        }
    }
}
