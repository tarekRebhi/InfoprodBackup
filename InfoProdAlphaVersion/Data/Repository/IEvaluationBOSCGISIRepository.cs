
using Domain.Entity;
using MyReports.Data.Infrastracture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repository
{
    public interface IEvaluationBOSCGISIRepository : IRepositoryBase<GrilleEvaluationBO>
    {
        List<GrilleEvaluationBO> GetEvaluationsByEmployee(int id);
        List<GrilleEvaluationBO> GetEvaluationsEmployeeBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin);

        List<GrilleEvaluationBO> GetEvaluationsBetweenTwoDates(DateTime dateDebut, DateTime dateFin);
        List<GrilleEvaluationBO> GetEvaluationsBySenderId(int id);
     
        List<GrilleEvaluationBO> GetEvaluationsSenderBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin);
        void DeleteEvaluations(int? id, int? empId);
        GrilleEvaluationBO getById(int? id);
    }
}