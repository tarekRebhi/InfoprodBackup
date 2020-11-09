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
    public class AttendanceHermesService:IAttendanceHermesService
    {
        IDatabaseFactory dbfactory = null;
        IUnitOfWork uow = null;
        public AttendanceHermesService()
        {
            dbfactory = new DatabaseFactory();
            uow = new UnitOfWork(dbfactory);
        }

        public void Add(AttendanceHermes attendance)
        {
            uow.AttendanceHermesRepository.Add(attendance);
        }

        public void Delete(AttendanceHermes attendance)
        {
            uow.AttendanceHermesRepository.Delete(attendance);
        }

        public float DiffDateTimes(AttendanceHermes attendance)
        {
            return uow.AttendanceHermesRepository.DiffDateTimes(attendance); 
        }

        public void Dispose()
        {
            uow.Dispose();        }

      
        public AttendanceHermes findAttendanceHermesBy(string champ)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AttendanceHermes> GetAll()
        {
            return uow.AttendanceHermesRepository.GetAll();
        }

        public AttendanceHermes getAttendanceByIdHermesDate(DateTime date, int idHermes)
        {
            return uow.AttendanceHermesRepository.getAttendanceByIdHermesDate(date,idHermes);
        }

        public AttendanceHermes getById(string champ)
        {
            throw new NotImplementedException();
        }

        public AttendanceHermes getById(int? id)
        {
            return uow.AttendanceHermesRepository.GetById(id);
        }

        public List<AttendanceHermes> getListByDate(DateTime date)
        {
            return uow.AttendanceHermesRepository.getListByDate(date);
        }

        public List<AttendanceHermes> getListByIdHermes(DateTime date, int idHermes)
        {
            return uow.AttendanceHermesRepository.getListByIdHermes(date,idHermes);
        }

        public void SaveChange()
        {
            uow.Commit();        }
    }
}
