using Domain.Entity;
using MyReports.Data.Infrastracture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repository
{
    public interface IEvaluationBPPTypologieRepository : IRepositoryBase<GrilleEvaluationBPPTypologie>
    {
        List<GrilleEvaluationBPPTypologie> GetEvaluationsByEmployee(int id);
        List<GrilleEvaluationBPPTypologie> GetEvaluationsBySenderId(int id);
        // List<GrilleEvaluationBPP> GetEvaluationsEmployeeByDate(int id, DateTime date);
        List<GrilleEvaluationBPPTypologie> GetEvaluationsEmployeeBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin);
        List<GrilleEvaluationBPPTypologie> GetEvaluationsBetweenTwoDates(DateTime dateDebut, DateTime dateFin);
        List<GrilleEvaluationBPPTypologie> GetEvaluationsSenderBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin);
        void DeleteEvaluations(int? id, int? empId);
        GrilleEvaluationBPPTypologie getById(int? id);
    }
}
