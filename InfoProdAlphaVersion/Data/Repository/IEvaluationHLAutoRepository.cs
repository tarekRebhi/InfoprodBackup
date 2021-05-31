using Domain.Entity;
using MyReports.Data.Infrastracture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Data.Repository
{
    public interface IEvaluationHLAutoRepository : IRepositoryBase<GrilleEvaluationHLAuto>
    {
        List<GrilleEvaluationHLAuto> GetEvaluationsByEmployee(int id);
        List<GrilleEvaluationHLAuto> GetEvaluationsBySenderId(int id);
        // List<GrilleEvaluationBPP> GetEvaluationsEmployeeByDate(int id, DateTime date);
        List<GrilleEvaluationHLAuto> GetEvaluationsEmployeeBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin);
        List<GrilleEvaluationHLAuto> GetEvaluationsBetweenTwoDates(DateTime dateDebut, DateTime dateFin);
        List<GrilleEvaluationHLAuto> GetEvaluationsSenderBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin);
        void DeleteEvaluations(int? id, int? empId);
        GrilleEvaluationHLAuto getById(int? id);
    }
}
