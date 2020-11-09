using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IGrilleEvaluationService : IDisposable
    {
        void Add(GrilleEvaluation evaluation);

        void Delete(GrilleEvaluation evaluation);

        void SaveChange();
        GrilleEvaluation findGrilleEvaluationBy(String champ);
        GrilleEvaluation getById(int? id);
        GrilleEvaluation getEvalById(int? id);
        GrilleEvaluation getById(String champ);

        IEnumerable<GrilleEvaluation> GetAll();
        List<GrilleEvaluation> GetEvaluationsByEmployee(int id);
        List<GrilleEvaluation> GetEvaluationsBySenderId(int id);
        void Dispose();

        List<GrilleEvaluation> GetEvaluationsEmployeeByDate(int id, DateTime date);
        List<GrilleEvaluation> GetEvaluationsEmployeeBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin);
        List<GrilleEvaluation> GetEvaluationsBetweenTwoDates( DateTime dateDebut, DateTime dateFin);

        List<GrilleEvaluation> GetEvaluationsReabEmployeeBetweenTwoDates(string type, int id, DateTime dateDebut, DateTime dateFin);
        List<GrilleEvaluation> GetEvaluationsReabBetweenTwoDates(string type, DateTime dateDebut, DateTime dateFin);
        List<GrilleEvaluation> GetEvaluationsSenderBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin);
        void DeleteEvaluations(int? id);


    }
}
    

   