using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IEvaluationKLMOService : IDisposable
    {
        void Add(GrilleEvaluationKLMO evaluation);

        void Delete(GrilleEvaluationKLMO evaluation);

        void SaveChange();
        //GrilleEvaluation findGrilleEvaluationBy(String champ);
        GrilleEvaluationKLMO getById(int? id);
        //GrilleEvaluation getEvalById(int? id);
        //GrilleEvaluation getById(String champ);

        //IEnumerable<GrilleEvaluation> GetAll();
        List<GrilleEvaluationKLMO> GetEvaluationsByEmployee(int id);
        List<GrilleEvaluationKLMO> GetEvaluationsBySenderId(int id);
        void Dispose();

        //List<GrilleEvaluation> GetEvaluationsEmployeeByDate(int id, DateTime date);
       List<GrilleEvaluationKLMO> GetEvaluationsEmployeeBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin);
        List<GrilleEvaluationKLMO> GetEvaluationsBetweenTwoDates(DateTime dateDebut, DateTime dateFin);
        List<GrilleEvaluationKLMO> GetEvaluationsSenderBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin);
        void DeleteEvaluations(int? id, int? empId);


    }
}


