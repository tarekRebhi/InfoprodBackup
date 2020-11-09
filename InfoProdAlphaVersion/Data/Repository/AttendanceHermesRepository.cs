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
    public class AttendanceHermesRepository:RepositoryBase<AttendanceHermes>, IAttencanceHermesRepository
    {
        public AttendanceHermesRepository(IDatabaseFactory dbFactory)
            : base(dbFactory)
        {

        }
        ReportContext context = new ReportContext();

        public List<AttendanceHermes> getListByDate(DateTime date)
        {

            //var groupe=context.groupes
            //var blogs = from b in context.groupes
            //            where b.employees.("B")
            //            select b;
            //context.groupes.Where
            var attendances = context.attendancesHermes.Where(a =>a.date==date);
            List<AttendanceHermes> att = new List<AttendanceHermes>();
            foreach (var test in attendances)
            {
                att.Add(test);

            }

            if (att != null)
            {

                return att;
            }


            else
            {
                return null;
            }
        }
        public List<AttendanceHermes> getListByIdHermes(DateTime date,int idHermes)
        {
            var att = getListByDate(date);
            var attendances = att.Where(a => a.Id_Hermes == idHermes);
            List<AttendanceHermes> attendanceHS = new List<AttendanceHermes>();
            foreach (var test in attendances)
            {
                attendanceHS.Add(test);

            }

            if (attendanceHS != null)
            {

                return attendanceHS;
            }


            else
            {
                return null;
            }
        }
        public AttendanceHermes getAttendanceByIdHermesDate(DateTime date, int idHermes)
        {
            var att = getListByDate(date);
            var attendance = att.FirstOrDefault(a => a.Id_Hermes == idHermes);
            AttendanceHermes attendanceHS = new AttendanceHermes();
            attendanceHS = attendance;
            if (attendanceHS != null)
            {

                return attendanceHS;
            }


            else
            {
                return null;
            }
        }
        public float DiffDateTimes(AttendanceHermes attendance)
        {

            //return (attendance.Depart - attendance.Arrive).Hours;
            return 0;
        }
    }

}
