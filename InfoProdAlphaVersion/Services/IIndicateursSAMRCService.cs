
using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IIndicateursSAMRCService : IDisposable
    {
       
       // GrilleEvaluationBOSAMRC getById(int? id);
       // void SaveChange();

        void Dispose();
        
        List<Appels_Entrants_SamRc> GetAppelsEntrantsBetweenTwoDates(DateTime dateDebut, DateTime dateFin);
       /// IEnumerable<GrilleEvaluationBOSAMRC> GetAll();

    }
}


