using Domain.Entity;
using MyReports.Data.Infrastracture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repository
{
    public interface IAlerteRepository : IRepositoryBase<Alerte>
    {
         List<Alerte> getByDate(DateTime date, int reciverId);
        void RemoveRecivedAlerteOfEmployee(int id);
    }
}
