using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IEvaluationQRService : IDisposable
    {
        void Add(GrilleEvaluationQR evaluation);

        void Delete(GrilleEvaluationQR evaluation);

        void SaveChange();
        //GrilleEvaluation findGrilleEvaluationBy(String champ);
        GrilleEvaluationQR getById(int? id);
        //GrilleEvaluation getEvalById(int? id);
        //GrilleEvaluation getById(String champ);

        //IEnumerable<GrilleEvaluation> GetAll();

        List<GrilleEvaluationQR> GetEvaluationsByEmployee(int id);
        List<GrilleEvaluationQR> GetEvaluationsBySenderId(int id);
        void Dispose();

        //List<GrilleEvaluation> GetEvaluationsEmployeeByDate(int id, DateTime date);
        List<GrilleEvaluationQR> GetEvaluationsEmployeeBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin);
        List<GrilleEvaluationQR> GetEvaluationsBetweenTwoDates( DateTime dateDebut, DateTime dateFin);
        List<GrilleEvaluationQR> GetEvaluationsSenderBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin);
        void DeleteEvaluations(int? id, int? empId);


    }
}


