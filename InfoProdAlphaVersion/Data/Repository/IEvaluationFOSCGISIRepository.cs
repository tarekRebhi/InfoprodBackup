
using Domain.Entity;
using MyReports.Data.Infrastracture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repository
{
    public interface IEvaluationFOSCGISIRepository : IRepositoryBase<GrilleEvaluationFO>
    {
        List<GrilleEvaluationFO> GetEvaluationsByEmployee(int id);
        List<GrilleEvaluationFO> GetEvaluationsEmployeeBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin);
        List<GrilleEvaluationFO> GetEvaluationsBetweenTwoDates(DateTime dateDebut, DateTime dateFin);

        List<GrilleEvaluationFO> GetEvaluationsBySenderId(int id);
        //List<GrilleEvaluation> GetEvaluationsEmployeeByDate(int id, DateTime date);
        //List<GrilleEvaluation> GetEvaluationsEmployeeBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin);
        List<GrilleEvaluationFO> GetEvaluationsSenderBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin);
        void DeleteEvaluations(int? id, int? empId);
        GrilleEvaluationFO getById(int? id);
    }
}