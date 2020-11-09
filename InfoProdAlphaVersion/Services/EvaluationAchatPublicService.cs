
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
    public class EvaluationAchatPublicService : IEvaluationAchatPublicService
    {
        IDatabaseFactory dbfactory = null;
        IUnitOfWork uow = null;

        public EvaluationAchatPublicService()
        {
            dbfactory = new DatabaseFactory();
            uow = new UnitOfWork(dbfactory);
        }
        public void Add(GrilleEvaluationAchatPublic evaluation)
        {
            uow.EvaluationAchatPublicRepository.Add(evaluation);
        }


        public void Delete(GrilleEvaluationAchatPublic evaluation)
        {
            uow.EvaluationAchatPublicRepository.Delete(evaluation);
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

        public GrilleEvaluationAchatPublic getById(int? id)
        {
            return uow.EvaluationAchatPublicRepository.getById(id);
        }
        public GrilleEvaluation getEvalById(int? id)
        {
            return uow.EvaluationRepository.getById(id);
        }
        public List<GrilleEvaluationAchatPublic> GetEvaluationsByEmployee(int id)
        {
            return uow.EvaluationAchatPublicRepository.GetEvaluationsByEmployee(id);
        }
        public List<GrilleEvaluationAchatPublic> GetEvaluationsBySenderId(int id)
        {
            return uow.EvaluationAchatPublicRepository.GetEvaluationsBySenderId(id);
        }


        public void SaveChange()
        {
            uow.Commit();
        }

        public void DeleteEvaluations(int? id, int? empId)
        {
            uow.EvaluationAchatPublicRepository.DeleteEvaluations(id, empId);
        }

        public List<GrilleEvaluationAchatPublic> GetEvaluationsEmployeeBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin)
        {
            return uow.EvaluationAchatPublicRepository.GetEvaluationsEmployeeBetweenTwoDates(id, dateDebut, dateFin);
        }

        public List<GrilleEvaluationAchatPublic> GetEvaluationsBetweenTwoDates(DateTime dateDebut, DateTime dateFin)
        {
            return uow.EvaluationAchatPublicRepository.GetEvaluationsBetweenTwoDates(dateDebut, dateFin);
        }

        public List<GrilleEvaluationAchatPublic> GetEvaluationsSenderBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin)
        {
            return uow.EvaluationAchatPublicRepository.GetEvaluationsSenderBetweenTwoDates(id, dateDebut, dateFin);
        }
        public IEnumerable<GrilleEvaluationAchatPublic> GetAll()
        {

            return uow.EvaluationAchatPublicRepository.GetAll();
        }
    }
}
