
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
    public class EvaluationPRVService : IEvaluationPRVService
    {
        IDatabaseFactory dbfactory = null;
        IUnitOfWork uow = null;

        public EvaluationPRVService()
        {
            dbfactory = new DatabaseFactory();
            uow = new UnitOfWork(dbfactory);
        }
        public void Add(GrilleEvaluationPRV evaluation)
        {
            uow.EvaluationPRVRepository.Add(evaluation);
        }


        public void Delete(GrilleEvaluationPRV evaluation)
        {
            uow.EvaluationPRVRepository.Delete(evaluation);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

      
        public GrilleEvaluationPRV getById(int? id)
        {
            return uow.EvaluationPRVRepository.getById(id);
        }
       
        public List<GrilleEvaluationPRV> GetEvaluationsByEmployee(int id)
        {
            return uow.EvaluationPRVRepository.GetEvaluationsByEmployee(id);
        }
        public List<GrilleEvaluationPRV> GetEvaluationsBySenderId(int id)
        {
            return uow.EvaluationPRVRepository.GetEvaluationsBySenderId(id);
        }

      
        public void SaveChange()
        {
            uow.Commit();
        }

        public void DeleteEvaluations(int? id, int? empId)
        {
            uow.EvaluationPRVRepository.DeleteEvaluations(id, empId);
        }

        public List<GrilleEvaluationPRV> GetEvaluationsEmployeeBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin)
        {
            return uow.EvaluationPRVRepository.GetEvaluationsEmployeeBetweenTwoDates(id, dateDebut, dateFin);
        }

        public List<GrilleEvaluationPRV> GetEvaluationsBetweenTwoDates(DateTime dateDebut, DateTime dateFin)
        {
            return uow.EvaluationPRVRepository.GetEvaluationsBetweenTwoDates(dateDebut, dateFin);
        }


        public List<GrilleEvaluationPRV> GetEvaluationsSenderBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin)
        {
            return uow.EvaluationPRVRepository.GetEvaluationsSenderBetweenTwoDates(id, dateDebut, dateFin);
        }
    }
}
