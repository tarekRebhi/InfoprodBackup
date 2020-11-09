
using Domain.Entity;
using MyReports.Data.Infrastracture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repository
{
    public interface IEvaluationBattonageRepository : IRepositoryBase<GrilleEvaluationBattonage>
    {
        List<GrilleEvaluationBattonage> GetEvaluationsByEmployee(int id);
        List<GrilleEvaluationBattonage> GetEvaluationsBySenderId(int id);
        // List<GrilleEvaluationBPP> GetEvaluationsEmployeeByDate(int id, DateTime date);
        List<GrilleEvaluationBattonage> GetEvaluationsEmployeeBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin);
        List<GrilleEvaluationBattonage> GetEvaluationsBetweenTwoDates(DateTime dateDebut, DateTime dateFin);
        List<GrilleEvaluationBattonage> GetEvaluationsSenderBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin);
        void DeleteEvaluations(int? id, int? empId);
        GrilleEvaluationBattonage getById(int? id);
    }
}

