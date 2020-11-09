
using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IEvaluationAchatPublicService : IDisposable
    {
        void Add(GrilleEvaluationAchatPublic evaluation);

        void Delete(GrilleEvaluationAchatPublic evaluation);
        GrilleEvaluationAchatPublic getById(int? id);
        void SaveChange();
     
        List<GrilleEvaluationAchatPublic> GetEvaluationsByEmployee(int id);
        List<GrilleEvaluationAchatPublic> GetEvaluationsBySenderId(int id);
        void Dispose();

        List<GrilleEvaluationAchatPublic> GetEvaluationsEmployeeBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin);
        List<GrilleEvaluationAchatPublic> GetEvaluationsBetweenTwoDates(DateTime dateDebut, DateTime dateFin);
        List<GrilleEvaluationAchatPublic> GetEvaluationsSenderBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin);
        void DeleteEvaluations(int? id, int? empId);
        IEnumerable<GrilleEvaluationAchatPublic> GetAll();

    }
}


