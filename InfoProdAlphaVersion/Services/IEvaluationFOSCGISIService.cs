
using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IEvaluationFOSCGISIService : IDisposable
    {
        void Add(GrilleEvaluationFO evaluation);

        void Delete(GrilleEvaluationFO evaluation);

        void SaveChange();
        //GrilleEvaluation findGrilleEvaluationBy(String champ);
        GrilleEvaluationFO getById(int? id);
        //GrilleEvaluation getEvalById(int? id);
        //GrilleEvaluation getById(String champ);

        //IEnumerable<GrilleEvaluation> GetAll();

        List<GrilleEvaluationFO> GetEvaluationsByEmployee(int id);
        List<GrilleEvaluationFO> GetEvaluationsBySenderId(int id);
        void Dispose();

        //List<GrilleEvaluation> GetEvaluationsEmployeeByDate(int id, DateTime date);
        List<GrilleEvaluationFO> GetEvaluationsEmployeeBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin);
        List<GrilleEvaluationFO> GetEvaluationsBetweenTwoDates(DateTime dateDebut, DateTime dateFin);


        List<GrilleEvaluationFO> GetEvaluationsSenderBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin);
        void DeleteEvaluations(int? id, int? empId);


    }
}


