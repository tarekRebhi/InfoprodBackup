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
    public class GrilleEvaluationService : IGrilleEvaluationService
    {
        IDatabaseFactory dbfactory = null;
        IUnitOfWork uow = null;

        public GrilleEvaluationService()
        {
            dbfactory = new DatabaseFactory();
            uow = new UnitOfWork(dbfactory);
        }
        public void Add(GrilleEvaluation evaluation)
        {
            uow.EvaluationRepository.Add(evaluation);
        }
       

        public void Delete(GrilleEvaluation evaluation)
        {
            uow.EvaluationRepository.Delete(evaluation);
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

        public GrilleEvaluation getById(int? id)
        {
            return uow.EvaluationRepository.getById(id); 
        }
        public GrilleEvaluation getEvalById(int? id)
        {
            return uow.EvaluationRepository.getById(id);
        }
        public List<GrilleEvaluation> GetEvaluationsByEmployee(int id)
        {
            return uow.EvaluationRepository.GetEvaluationsByEmployee(id);
                }
        public List<GrilleEvaluation> GetEvaluationsBySenderId(int id)
        {
            return uow.EvaluationRepository.GetEvaluationsBySenderId(id);
        }

        public List<GrilleEvaluation> GetEvaluationsEmployeeByDate(int id, DateTime date)
        {
            return uow.EvaluationRepository.GetEvaluationsEmployeeByDate(id, date);
        }
      
        public void SaveChange()
        {
            uow.Commit();
        }

        public void DeleteEvaluations(int? id)
        {
            uow.EvaluationRepository.DeleteEvaluations(id);
        }

        public List<GrilleEvaluation> GetEvaluationsEmployeeBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin)
        {
            return uow.EvaluationRepository.GetEvaluationsEmployeeBetweenTwoDates(id, dateDebut,dateFin);
        }

        public List<GrilleEvaluation> GetEvaluationsBetweenTwoDates(DateTime dateDebut, DateTime dateFin)
        {
            return uow.EvaluationRepository.GetEvaluationsBetweenTwoDates(dateDebut, dateFin);
        }

        public List<GrilleEvaluation> GetEvaluationsReabEmployeeBetweenTwoDates(string type, int id, DateTime dateDebut, DateTime dateFin)
        {
            return uow.EvaluationRepository.GetEvaluationsReabEmployeeBetweenTwoDates(type, id, dateDebut, dateFin);
        }

        public List<GrilleEvaluation> GetEvaluationsReabBetweenTwoDates(string type, DateTime dateDebut, DateTime dateFin)
        {
            return uow.EvaluationRepository.GetEvaluationsReabBetweenTwoDates(type, dateDebut, dateFin);
        }
        public List<GrilleEvaluation> GetEvaluationsSenderBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin)
        {
            return uow.EvaluationRepository.GetEvaluationsSenderBetweenTwoDates(id, dateDebut, dateFin);
        }
    }
}
