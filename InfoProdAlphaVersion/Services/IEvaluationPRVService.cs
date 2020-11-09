
using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IEvaluationPRVService : IDisposable
    {
        void Add(GrilleEvaluationPRV evaluation);

        void Delete(GrilleEvaluationPRV evaluation);

        void SaveChange();
        GrilleEvaluationPRV getById(int? id);
        List<GrilleEvaluationPRV> GetEvaluationsByEmployee(int id);
        List<GrilleEvaluationPRV> GetEvaluationsBySenderId(int id);
        void Dispose();
        List<GrilleEvaluationPRV> GetEvaluationsEmployeeBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin);
        List<GrilleEvaluationPRV> GetEvaluationsBetweenTwoDates(DateTime dateDebut, DateTime dateFin);
        List<GrilleEvaluationPRV> GetEvaluationsSenderBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin);
        void DeleteEvaluations(int? id, int? empId);


    }
}


