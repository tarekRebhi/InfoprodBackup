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
   public class IndicateursSAMRCService : IIndicateursSAMRCService

    {
        IDatabaseFactory dbfactory = null;
        IUnitOfWork uow = null;

        public IndicateursSAMRCService()
        {
            dbfactory = new DatabaseFactory();
            uow = new UnitOfWork(dbfactory);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
        public List<Appels_Entrants_SamRc> GetAppelsEntrantsBetweenTwoDates(DateTime dateDebut, DateTime dateFin)
        {
            return uow.IndicateursSAMRCRepository.GetAppelsEntrantsBetweenTwoDates(dateDebut, dateFin);
        }

    }
}
