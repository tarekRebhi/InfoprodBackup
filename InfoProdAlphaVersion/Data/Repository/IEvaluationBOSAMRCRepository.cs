
using Domain.Entity;
using MyReports.Data.Infrastracture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repository
{
    public interface IEvaluationBOSAMRCRepository : IRepositoryBase<GrilleEvaluationBOSAMRC>
    {
        List<GrilleEvaluationBOSAMRC> GetEvaluationsByEmployee(int id);
        List<GrilleEvaluationBOSAMRC> GetEvaluationsBySenderId(int id);
        // List<GrilleEvaluationBPP> GetEvaluationsEmployeeByDate(int id, DateTime date);
        List<GrilleEvaluationBOSAMRC> GetEvaluationsEmployeeBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin);
        List<GrilleEvaluationBOSAMRC> GetEvaluationsBetweenTwoDates(DateTime dateDebut, DateTime dateFin);
        List<GrilleEvaluationBOSAMRC> GetEvaluationsSenderBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin);
        void DeleteEvaluations(int? id, int? empId);
        GrilleEvaluationBOSAMRC getById(int? id);
    }
}
