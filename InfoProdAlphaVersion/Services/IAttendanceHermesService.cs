using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IAttendanceHermesService : IDisposable
    {
        void Add(AttendanceHermes attendance);

        void Delete(AttendanceHermes attendance);

        void SaveChange();
        AttendanceHermes findAttendanceHermesBy(String champ);
        AttendanceHermes getById(int? id);
        AttendanceHermes getById(String champ);

        void Dispose();

        IEnumerable<AttendanceHermes> GetAll();
        List<AttendanceHermes> getListByDate(DateTime date);

        List<AttendanceHermes> getListByIdHermes(DateTime date, int idHermes);

        float DiffDateTimes(AttendanceHermes attendance);

        AttendanceHermes getAttendanceByIdHermesDate(DateTime date, int idHermes);
    }
}
