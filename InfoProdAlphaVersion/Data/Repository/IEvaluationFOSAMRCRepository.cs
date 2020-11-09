
using Domain.Entity;
using MyReports.Data.Infrastracture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repository
{
    public interface IEvaluationFOSAMRCRepository : IRepositoryBase<GrilleEvaluationFOSAMRC>
    {
        List<GrilleEvaluationFOSAMRC> GetEvaluationsByEmployee(int id);
        List<GrilleEvaluationFOSAMRC> GetEvaluationsBySenderId(int id);
        // List<GrilleEvaluationBPP> GetEvaluationsEmployeeByDate(int id, DateTime date);
        List<GrilleEvaluationFOSAMRC> GetEvaluationsEmployeeBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin);
        List<GrilleEvaluationFOSAMRC> GetEvaluationsBetweenTwoDates(DateTime dateDebut, DateTime dateFin);
        List<GrilleEvaluationFOSAMRC> GetEvaluationsSenderBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin);
        void DeleteEvaluations(int? id, int? empId);
        GrilleEvaluationFOSAMRC getById(int? id);
    }
}
