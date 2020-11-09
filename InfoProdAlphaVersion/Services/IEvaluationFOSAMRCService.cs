
using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IEvaluationFOSAMRCService : IDisposable
    {
        void Add(GrilleEvaluationFOSAMRC evaluation);

        void Delete(GrilleEvaluationFOSAMRC evaluation);
        GrilleEvaluationFOSAMRC getById(int? id);
        void SaveChange();
        //GrilleEvaluation findGrilleEvaluationBy(String champ);
        //GrilleEvaluation getById(int? id);
        //GrilleEvaluation getEvalById(int? id);
        //GrilleEvaluation getById(String champ);

        //IEnumerable<GrilleEvaluation> GetAll();
        List<GrilleEvaluationFOSAMRC> GetEvaluationsByEmployee(int id);
        List<GrilleEvaluationFOSAMRC> GetEvaluationsBySenderId(int id);
        void Dispose();

        //List<GrilleEvaluation> GetEvaluationsEmployeeByDate(int id, DateTime date);
        List<GrilleEvaluationFOSAMRC> GetEvaluationsEmployeeBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin);
        List<GrilleEvaluationFOSAMRC> GetEvaluationsBetweenTwoDates(DateTime dateDebut, DateTime dateFin);
        List<GrilleEvaluationFOSAMRC> GetEvaluationsSenderBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin);
        void DeleteEvaluations(int? id, int? empId);
        IEnumerable<GrilleEvaluationFOSAMRC> GetAll();

    }
}


