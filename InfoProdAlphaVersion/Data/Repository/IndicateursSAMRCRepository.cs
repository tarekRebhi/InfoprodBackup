
using Domain.Entity;
using MyReports.Data.Infrastracture;
using MyReports.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repository
{
    public class IndicateursSAMRCRepository : RepositoryBase<Appels_Entrants_SamRc>, IIndicateursSAMRCRepository
    {
        public IndicateursSAMRCRepository(IDatabaseFactory dbFactory)
            : base(dbFactory)
        {

        }
        ReportContext context = new ReportContext();

      
        public List<Appels_Entrants_SamRc> GetAppelsEntrantsBetweenTwoDates(DateTime dateDebut, DateTime dateFin)
        {

            ReportContext context = new ReportContext();

            var appelsEntrants = context.Appels_Entrants_SamRc.Where(a => a.FERIE == 0 && a.Closed == 0 && a.LastQueue != 0 &&(EntityFunctions.TruncateTime(a.Date) >= dateDebut.Date && (EntityFunctions.TruncateTime(a.Date)) <= dateFin.Date));
            List<Appels_Entrants_SamRc> tests = new List<Appels_Entrants_SamRc>();
            tests.AddRange(appelsEntrants);
            if (appelsEntrants != null)
            {
                return tests;
            }
            else
            {
                return null;
            }
        }

     
       
    }
}
