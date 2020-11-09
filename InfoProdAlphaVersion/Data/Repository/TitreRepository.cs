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
   public class TitreRepository : RepositoryBase<Titre>, ITitreRepository
    {
        public TitreRepository(IDatabaseFactory dbFactory)
            : base(dbFactory)
        {

        }
    
    }
}
