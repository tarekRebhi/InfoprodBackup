
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entity;
using MyReports.Data.Infrastructure;
using MyFinance.Data.Infrastructure;

namespace Services
{
    public class EvaluationBattonageService : IEvaluationBattonageService
    {
        IDatabaseFactory dbfactory = null;
        IUnitOfWork uow = null;

        public EvaluationBattonageService()
        {
            dbfactory = new DatabaseFactory();
            uow = new UnitOfWork(dbfactory);
        }
        public void Add(GrilleEvaluationBattonage evaluation)
        {
            uow.EvaluationBattonageRepository.Add(evaluation);
        }


        public void Delete(GrilleEvaluationBattonage evaluation)
        {
            uow.EvaluationBattonageRepository.Delete(evaluation);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public GrilleEvaluation findGrilleEvaluationBy(string champ)
        {
            throw new NotImplementedException();
        }



        public GrilleEvaluation getById(string champ)
        {
            throw new NotImplementedException();
        }

        public GrilleEvaluationBattonage getById(int? id)
        {
            return uow.EvaluationBattonageRepository.getById(id);
        }
        public GrilleEvaluation getEvalById(int? id)
        {
            return uow.EvaluationRepository.getById(id);
        }
        public List<GrilleEvaluationBattonage> GetEvaluationsByEmployee(int id)
        {
            return uow.EvaluationBattonageRepository.GetEvaluationsByEmployee(id);
        }
        public List<GrilleEvaluationBattonage> GetEvaluationsBySenderId(int id)
        {
            return uow.EvaluationBattonageRepository.GetEvaluationsBySenderId(id);
        }

        //public List<GrilleEvaluationBPP> GetEvaluationsEmployeeByDate(int id, DateTime date)
        //{
        //    return uow.EvaluationBPPRepository.GetEvaluationsEmployeeByDate(id, date);
        //}

        public void SaveChange()
        {
            uow.Commit();
        }

        public void DeleteEvaluations(int? id, int? empId)
        {
            uow.EvaluationBattonageRepository.DeleteEvaluations(id, empId);
        }

        public List<GrilleEvaluationBattonage> GetEvaluationsEmployeeBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin)
        {
            return uow.EvaluationBattonageRepository.GetEvaluationsEmployeeBetweenTwoDates(id, dateDebut, dateFin);
        }

        public List<GrilleEvaluationBattonage> GetEvaluationsBetweenTwoDates(DateTime dateDebut, DateTime dateFin)
        {
            return uow.EvaluationBattonageRepository.GetEvaluationsBetweenTwoDates(dateDebut, dateFin);
        }

        public List<GrilleEvaluationBattonage> GetEvaluationsSenderBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin)
        {
            return uow.EvaluationBattonageRepository.GetEvaluationsSenderBetweenTwoDates(id, dateDebut, dateFin);
        }
        public IEnumerable<GrilleEvaluationBattonage> GetAll()
        {

            return uow.EvaluationBattonageRepository.GetAll();
        }
    }
}
