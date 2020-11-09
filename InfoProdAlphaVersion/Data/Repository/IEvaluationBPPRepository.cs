using Domain.Entity;
using MyReports.Data.Infrastracture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repository
{
    public interface IEvaluationBPPRepository : IRepositoryBase<GrilleEvaluationBPP>
    {
         List<GrilleEvaluationBPP> GetEvaluationsByEmployee(int id);
        List<GrilleEvaluationBPP> GetEvaluationsBySenderId(int id);
       // List<GrilleEvaluationBPP> GetEvaluationsEmployeeByDate(int id, DateTime date);
        List<GrilleEvaluationBPP> GetEvaluationsEmployeeBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin);
        List<GrilleEvaluationBPP> GetEvaluationsBetweenTwoDates(DateTime dateDebut, DateTime dateFin);
        List<GrilleEvaluationBPP> GetEvaluationsSenderBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin);
        void DeleteEvaluations(int? id, int? empId);
        GrilleEvaluationBPP getById(int? id);
    }
}
