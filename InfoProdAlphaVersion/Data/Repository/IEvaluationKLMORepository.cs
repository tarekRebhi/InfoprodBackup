using Domain.Entity;
using MyReports.Data.Infrastracture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repository
{
    public interface IEvaluationKLMORepository : IRepositoryBase<GrilleEvaluationKLMO>
    {
        List<GrilleEvaluationKLMO> GetEvaluationsByEmployee(int id);
        List<GrilleEvaluationKLMO> GetEvaluationsBySenderId(int id);
       // List<GrilleEvaluationKLMO> GetEvaluationsEmployeeByDate(int id, DateTime date);
       List<GrilleEvaluationKLMO> GetEvaluationsEmployeeBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin);
        List<GrilleEvaluationKLMO> GetEvaluationsBetweenTwoDates( DateTime dateDebut, DateTime dateFin);
        List<GrilleEvaluationKLMO> GetEvaluationsSenderBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin);
        void DeleteEvaluations(int? id, int? empId);
        GrilleEvaluationKLMO getById(int? id);
    }
}
