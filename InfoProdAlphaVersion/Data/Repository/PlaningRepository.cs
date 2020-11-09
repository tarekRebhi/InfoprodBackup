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
   public class PlaningRepository : RepositoryBase<Planing>, IPlaningRepository
    {
        public PlaningRepository (IDatabaseFactory dbFactory)
            : base(dbFactory)
        {

        }
    }
}
