using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IEvaluationBPPService : IDisposable
    {
        void Add(GrilleEvaluationBPP evaluation);

        void Delete(GrilleEvaluationBPP evaluation);
        GrilleEvaluationBPP getById(int? id);
        void SaveChange();
        //GrilleEvaluation findGrilleEvaluationBy(String champ);
        //GrilleEvaluation getById(int? id);
        //GrilleEvaluation getEvalById(int? id);
        //GrilleEvaluation getById(String champ);

        //IEnumerable<GrilleEvaluation> GetAll();
        List<GrilleEvaluationBPP> GetEvaluationsByEmployee(int id);
        List<GrilleEvaluationBPP> GetEvaluationsBySenderId(int id);
        void Dispose();

        //List<GrilleEvaluation> GetEvaluationsEmployeeByDate(int id, DateTime date);
        List<GrilleEvaluationBPP> GetEvaluationsEmployeeBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin);
        List<GrilleEvaluationBPP> GetEvaluationsBetweenTwoDates(DateTime dateDebut, DateTime dateFin);
        List<GrilleEvaluationBPP> GetEvaluationsSenderBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin);
        void DeleteEvaluations(int? id, int? empId);
        IEnumerable<GrilleEvaluationBPP> GetAll();
    
    }
}


