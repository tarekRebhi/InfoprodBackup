using Data.Repository;
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
    public class IndicateurRepository:RepositoryBase<Indicateur>, IIndicateurRepository
    {
        public IndicateurRepository(IDatabaseFactory dbFactory)
            : base(dbFactory)
        {

    }
}
}
