using Domain.Entity;
using MyReports.Data.Infrastracture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repository
{
    public interface IEvaluationEnqueteAutoRepository : IRepositoryBase<GrilleEvaluationEnqueteAuto>
    {
        List<GrilleEvaluationEnqueteAuto> GetEvaluationsByEmployee(int id);
        List<GrilleEvaluationEnqueteAuto> GetEvaluationsEmployeeBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin);
        List<GrilleEvaluationEnqueteAuto> GetEvaluationsBetweenTwoDates(DateTime dateDebut, DateTime dateFin);
        List<GrilleEvaluationEnqueteAuto> GetEvaluationsBySenderId(int id);
        //List<GrilleEvaluation> GetEvaluationsEmployeeByDate(int id, DateTime date);
        //List<GrilleEvaluation> GetEvaluationsEmployeeBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin);
        List<GrilleEvaluationEnqueteAuto> GetEvaluationsSenderBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin);
        void DeleteEvaluations(int? id, int? empId);
        GrilleEvaluationEnqueteAuto getById(int? id);
    }
}