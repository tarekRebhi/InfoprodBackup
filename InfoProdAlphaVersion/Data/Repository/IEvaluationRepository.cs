using Domain.Entity;
using MyReports.Data.Infrastracture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repository
{
    public interface IEvaluationRepository : IRepositoryBase<GrilleEvaluation>
    {
       List<GrilleEvaluation> GetEvaluationsByEmployee(int id);
        List<GrilleEvaluation> GetEvaluationsBySenderId(int id);
       List<GrilleEvaluation> GetEvaluationsEmployeeByDate(int id, DateTime date);
        List<GrilleEvaluation> GetEvaluationsEmployeeBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin);
        List<GrilleEvaluation> GetEvaluationsBetweenTwoDates(DateTime dateDebut, DateTime dateFin);
        List<GrilleEvaluation> GetEvaluationsReabEmployeeBetweenTwoDates(string type, int id, DateTime dateDebut, DateTime dateFin);
        List<GrilleEvaluation> GetEvaluationsReabBetweenTwoDates(string type, DateTime dateDebut, DateTime dateFin);
        List<GrilleEvaluation> GetEvaluationsSenderBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin);
        void DeleteEvaluations(int? id);       
        GrilleEvaluation getById(int? id);
    }
}
