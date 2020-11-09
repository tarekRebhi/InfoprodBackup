
using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IEvaluationEnqueteAutoService : IDisposable
    {
        void Add(GrilleEvaluationEnqueteAuto evaluation);

        void Delete(GrilleEvaluationEnqueteAuto evaluation);

        void SaveChange();
        //GrilleEvaluation findGrilleEvaluationBy(String champ);
        //GrilleEvaluation getById(int? id);
        //GrilleEvaluation getEvalById(int? id);
        //GrilleEvaluation getById(String champ);

        //IEnumerable<GrilleEvaluation> GetAll();

        List<GrilleEvaluationEnqueteAuto> GetEvaluationsByEmployee(int id);
        List<GrilleEvaluationEnqueteAuto> GetEvaluationsBySenderId(int id);
        void Dispose();

        //List<GrilleEvaluation> GetEvaluationsEmployeeByDate(int id, DateTime date);
        List<GrilleEvaluationEnqueteAuto> GetEvaluationsEmployeeBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin);
        List<GrilleEvaluationEnqueteAuto> GetEvaluationsBetweenTwoDates( DateTime dateDebut, DateTime dateFin);
        List<GrilleEvaluationEnqueteAuto> GetEvaluationsSenderBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin);
        void DeleteEvaluations(int? id, int? empId);
        GrilleEvaluationEnqueteAuto getById(int? id);

    }
}


