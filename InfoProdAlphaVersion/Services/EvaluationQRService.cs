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
    public class EvaluationQRService : IEvaluationQRService
    {
        IDatabaseFactory dbfactory = null;
        IUnitOfWork uow = null;

        public EvaluationQRService()
        {
            dbfactory = new DatabaseFactory();
            uow = new UnitOfWork(dbfactory);
        }
        public void Add(GrilleEvaluationQR evaluation)
        {
            uow.EvaluationQRRepository.Add(evaluation);
        }


        public void Delete(GrilleEvaluationQR evaluation)
        {
            uow.EvaluationQRRepository.Delete(evaluation);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public GrilleEvaluation findGrilleEvaluationBy(string champ)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<GrilleEvaluation> GetAll()
        {

            return uow.EvaluationRepository.GetAll();
        }

        public GrilleEvaluation getById(string champ)
        {
            throw new NotImplementedException();
        }

        public GrilleEvaluationQR getById(int? id)
        {
            return uow.EvaluationQRRepository.getById(id);
        }
        public GrilleEvaluation getEvalById(int? id)
        {
            return uow.EvaluationRepository.getById(id);
        }
        public List<GrilleEvaluationQR> GetEvaluationsByEmployee(int id)
        {
            return uow.EvaluationQRRepository.GetEvaluationsByEmployee(id);
        }
        public List<GrilleEvaluationQR> GetEvaluationsBySenderId(int id)
        {
            return uow.EvaluationQRRepository.GetEvaluationsBySenderId(id);
        }

        public List<GrilleEvaluation> GetEvaluationsEmployeeByDate(int id, DateTime date)
        {
            return uow.EvaluationRepository.GetEvaluationsEmployeeByDate(id, date);
        }

        public void SaveChange()
        {
            uow.Commit();
        }

        public void DeleteEvaluations(int? id, int? empId)
        {
            uow.EvaluationQRRepository.DeleteEvaluations(id, empId);
        }

        public List<GrilleEvaluationQR> GetEvaluationsEmployeeBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin)
        {
            return uow.EvaluationQRRepository.GetEvaluationsEmployeeBetweenTwoDates(id, dateDebut, dateFin);
        }

        public List<GrilleEvaluationQR> GetEvaluationsBetweenTwoDates(DateTime dateDebut, DateTime dateFin)
        {
            return uow.EvaluationQRRepository.GetEvaluationsBetweenTwoDates(dateDebut, dateFin);
        }


        public List<GrilleEvaluationQR> GetEvaluationsSenderBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin)
        {
            return uow.EvaluationQRRepository.GetEvaluationsSenderBetweenTwoDates(id, dateDebut, dateFin);
        }
    }
}
