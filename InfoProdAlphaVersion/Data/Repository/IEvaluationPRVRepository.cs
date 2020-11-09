
using Domain.Entity;
using MyReports.Data.Infrastracture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repository
{
    public interface IEvaluationPRVRepository : IRepositoryBase<GrilleEvaluationPRV>
    {
        List<GrilleEvaluationPRV> GetEvaluationsByEmployee(int id);
        List<GrilleEvaluationPRV> GetEvaluationsEmployeeBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin);
        List<GrilleEvaluationPRV> GetEvaluationsBetweenTwoDates(DateTime dateDebut, DateTime dateFin);
        List<GrilleEvaluationPRV> GetEvaluationsBySenderId(int id);
        List<GrilleEvaluationPRV> GetEvaluationsSenderBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin);
        void DeleteEvaluations(int? id, int? empId);
        GrilleEvaluationPRV getById(int? id);
    }
}
