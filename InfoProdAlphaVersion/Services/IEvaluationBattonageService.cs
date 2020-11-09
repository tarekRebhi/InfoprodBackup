
using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IEvaluationBattonageService : IDisposable
    {
        void Add(GrilleEvaluationBattonage evaluation);

        void Delete(GrilleEvaluationBattonage evaluation);
        GrilleEvaluationBattonage getById(int? id);
        void SaveChange();
        //GrilleEvaluation findGrilleEvaluationBy(String champ);
        //GrilleEvaluation getById(int? id);
        //GrilleEvaluation getEvalById(int? id);
        //GrilleEvaluation getById(String champ);

        //IEnumerable<GrilleEvaluation> GetAll();
        List<GrilleEvaluationBattonage> GetEvaluationsByEmployee(int id);
        List<GrilleEvaluationBattonage> GetEvaluationsBySenderId(int id);
        void Dispose();

        //List<GrilleEvaluation> GetEvaluationsEmployeeByDate(int id, DateTime date);
        List<GrilleEvaluationBattonage> GetEvaluationsEmployeeBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin);
        List<GrilleEvaluationBattonage> GetEvaluationsBetweenTwoDates(DateTime dateDebut, DateTime dateFin);
        List<GrilleEvaluationBattonage> GetEvaluationsSenderBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin);
        void DeleteEvaluations(int? id, int? empId);
        IEnumerable<GrilleEvaluationBattonage> GetAll();

    }
}



