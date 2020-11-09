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
    public class EvaluationKLMOService : IEvaluationKLMOService
    {
        IDatabaseFactory dbfactory = null;
        IUnitOfWork uow = null;

        public EvaluationKLMOService()
        {
            dbfactory = new DatabaseFactory();
            uow = new UnitOfWork(dbfactory);
        }
        public void Add(GrilleEvaluationKLMO evaluation)
        {
            uow.EvaluationKLMORepository.Add(evaluation);
        }


        public void Delete(GrilleEvaluationKLMO evaluation)
        {
            uow.EvaluationKLMORepository.Delete(evaluation);
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

        public GrilleEvaluationKLMO getById(int? id)
        {
            return uow.EvaluationKLMORepository.getById(id);
        }
        public GrilleEvaluation getEvalById(int? id)
        {
            return uow.EvaluationRepository.getById(id);
        }
        public List<GrilleEvaluationKLMO> GetEvaluationsByEmployee(int id)
        {
            return uow.EvaluationKLMORepository.GetEvaluationsByEmployee(id);
        }
        public List<GrilleEvaluationKLMO> GetEvaluationsBySenderId(int id)
        {
            return uow.EvaluationKLMORepository.GetEvaluationsBySenderId(id);
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
            uow.EvaluationKLMORepository.DeleteEvaluations(id, empId);
        }

        public List<GrilleEvaluationKLMO> GetEvaluationsEmployeeBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin)
        {
            return uow.EvaluationKLMORepository.GetEvaluationsEmployeeBetweenTwoDates(id, dateDebut, dateFin);
        }

        public List<GrilleEvaluationKLMO> GetEvaluationsBetweenTwoDates(DateTime dateDebut, DateTime dateFin)
        {
            return uow.EvaluationKLMORepository.GetEvaluationsBetweenTwoDates(dateDebut, dateFin);
        }

        public List<GrilleEvaluationKLMO> GetEvaluationsSenderBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin)
        {
            return uow.EvaluationKLMORepository.GetEvaluationsSenderBetweenTwoDates(id, dateDebut, dateFin);
        }
    }
}
