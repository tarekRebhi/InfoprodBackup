
using Domain.Entity;
using MyReports.Data.Infrastracture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repository
{
    public interface IEvaluationAchatPublicRepository : IRepositoryBase<GrilleEvaluationAchatPublic>
    {
        List<GrilleEvaluationAchatPublic> GetEvaluationsByEmployee(int id);
        List<GrilleEvaluationAchatPublic> GetEvaluationsBySenderId(int id);
        List<GrilleEvaluationAchatPublic> GetEvaluationsEmployeeBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin);
        List<GrilleEvaluationAchatPublic> GetEvaluationsBetweenTwoDates(DateTime dateDebut, DateTime dateFin);
        List<GrilleEvaluationAchatPublic> GetEvaluationsSenderBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin);
        void DeleteEvaluations(int? id, int? empId);
        GrilleEvaluationAchatPublic getById(int? id);
    }
}
