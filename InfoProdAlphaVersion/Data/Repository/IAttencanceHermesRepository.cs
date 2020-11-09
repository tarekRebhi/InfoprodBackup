using Domain.Entity;
using MyReports.Data.Infrastracture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repository
{
    public interface IAttencanceHermesRepository : IRepositoryBase<AttendanceHermes>
    {
        List<AttendanceHermes> getListByDate(DateTime date);

        List<AttendanceHermes> getListByIdHermes(DateTime date, int idHermes);

        float DiffDateTimes(AttendanceHermes attendance);

        AttendanceHermes getAttendanceByIdHermesDate(DateTime date, int idHermes);


    }
}
