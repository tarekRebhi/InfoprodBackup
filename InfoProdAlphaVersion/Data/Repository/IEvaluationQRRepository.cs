using Domain.Entity;
using MyReports.Data.Infrastracture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repository
{
    public interface IEvaluationQRRepository : IRepositoryBase<GrilleEvaluationQR>
    {
      List<GrilleEvaluationQR> GetEvaluationsByEmployee(int id);
        List<GrilleEvaluationQR> GetEvaluationsEmployeeBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin);
        List<GrilleEvaluationQR> GetEvaluationsBetweenTwoDates(DateTime dateDebut, DateTime dateFin);
        List<GrilleEvaluationQR> GetEvaluationsBySenderId(int id);
        //List<GrilleEvaluation> GetEvaluationsEmployeeByDate(int id, DateTime date);
        //List<GrilleEvaluation> GetEvaluationsEmployeeBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin);
        List<GrilleEvaluationQR> GetEvaluationsSenderBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin);
        void DeleteEvaluations(int? id, int? empId);
        GrilleEvaluationQR getById(int? id);
    }
}
