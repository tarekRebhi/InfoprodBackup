
using Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IEvaluationBOSAMRCService : IDisposable
    {
        void Add(GrilleEvaluationBOSAMRC evaluation);

        void Delete(GrilleEvaluationBOSAMRC evaluation);
        GrilleEvaluationBOSAMRC getById(int? id);
        void SaveChange();
        //GrilleEvaluation findGrilleEvaluationBy(String champ);
        //GrilleEvaluation getById(int? id);
        //GrilleEvaluation getEvalById(int? id);
        //GrilleEvaluation getById(String champ);

        //IEnumerable<GrilleEvaluation> GetAll();
        List<GrilleEvaluationBOSAMRC> GetEvaluationsByEmployee(int id);
        List<GrilleEvaluationBOSAMRC> GetEvaluationsBySenderId(int id);
        void Dispose();

        //List<GrilleEvaluation> GetEvaluationsEmployeeByDate(int id, DateTime date);
        List<GrilleEvaluationBOSAMRC> GetEvaluationsEmployeeBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin);
        List<GrilleEvaluationBOSAMRC> GetEvaluationsBetweenTwoDates(DateTime dateDebut, DateTime dateFin);
        List<GrilleEvaluationBOSAMRC> GetEvaluationsSenderBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin);
        void DeleteEvaluations(int? id, int? empId);
        IEnumerable<GrilleEvaluationBOSAMRC> GetAll();

    }
}


