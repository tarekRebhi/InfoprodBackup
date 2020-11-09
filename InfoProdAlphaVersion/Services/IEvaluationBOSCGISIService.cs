
using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IEvaluationBOSCGISIService : IDisposable
    {
        void Add(GrilleEvaluationBO evaluation);

        void Delete(GrilleEvaluationBO evaluation);

        void SaveChange();
        //GrilleEvaluation findGrilleEvaluationBy(String champ);
        GrilleEvaluationBO getById(int? id);
        //GrilleEvaluation getEvalById(int? id);
        //GrilleEvaluation getById(String champ);

        //IEnumerable<GrilleEvaluation> GetAll();

        List<GrilleEvaluationBO> GetEvaluationsByEmployee(int id);
        List<GrilleEvaluationBO> GetEvaluationsBySenderId(int id);
        void Dispose();

        //List<GrilleEvaluation> GetEvaluationsEmployeeByDate(int id, DateTime date);
        List<GrilleEvaluationBO> GetEvaluationsEmployeeBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin);
        List<GrilleEvaluationBO> GetEvaluationsBetweenTwoDates(DateTime dateDebut, DateTime dateFin);
        List<GrilleEvaluationBO> GetEvaluationsSenderBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin);
        void DeleteEvaluations(int? id, int? empId);


    }
}


