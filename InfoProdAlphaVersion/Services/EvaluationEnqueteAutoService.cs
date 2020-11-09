
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
    public class EvaluationEnqueteAutoService : IEvaluationEnqueteAutoService
    {
        IDatabaseFactory dbfactory = null;
        IUnitOfWork uow = null;

        public EvaluationEnqueteAutoService()
        {
            dbfactory = new DatabaseFactory();
            uow = new UnitOfWork(dbfactory);
        }
        public void Add(GrilleEvaluationEnqueteAuto evaluation)
        {
            uow.EvaluationEnqueteAutoRepository.Add(evaluation);
        }


        public void Delete(GrilleEvaluationEnqueteAuto evaluation)
        {
            uow.EvaluationEnqueteAutoRepository.Delete(evaluation);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        //public GrilleEvaluation findGrilleEvaluationBy(string champ)
        //{
        //    throw new NotImplementedException();
        //}

        //public IEnumerable<GrilleEvaluation> GetAll()
        //{

        //    return uow.EvaluationRepository.GetAll();
        //}

        //public GrilleEvaluation getById(string champ)
        //{
        //    throw new NotImplementedException();
        //}

        public GrilleEvaluationEnqueteAuto getById(int? id)
        {
            return uow.EvaluationEnqueteAutoRepository.getById(id);
        }
        public List<GrilleEvaluationEnqueteAuto> GetEvaluationsByEmployee(int id)
        {
            return uow.EvaluationEnqueteAutoRepository.GetEvaluationsByEmployee(id);
        }
        public List<GrilleEvaluationEnqueteAuto> GetEvaluationsBySenderId(int id)
        {
            return uow.EvaluationEnqueteAutoRepository.GetEvaluationsBySenderId(id);
        }

        //public List<GrilleEvaluation> GetEvaluationsEmployeeByDate(int id, DateTime date)
        //{
        //    return uow.EvaluationRepository.GetEvaluationsEmployeeByDate(id, date);
        //}

        public void SaveChange()
        {
            uow.Commit();
        }

        public void DeleteEvaluations(int? id, int? empId)
        {
            uow.EvaluationEnqueteAutoRepository.DeleteEvaluations(id, empId);
        }

        public List<GrilleEvaluationEnqueteAuto> GetEvaluationsEmployeeBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin)
        {
            return uow.EvaluationEnqueteAutoRepository.GetEvaluationsEmployeeBetweenTwoDates(id, dateDebut, dateFin);
        }

        public List<GrilleEvaluationEnqueteAuto> GetEvaluationsBetweenTwoDates(DateTime dateDebut, DateTime dateFin)
        {
            return uow.EvaluationEnqueteAutoRepository.GetEvaluationsBetweenTwoDates(dateDebut, dateFin);
        }

        public List<GrilleEvaluationEnqueteAuto> GetEvaluationsSenderBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin)
        {
            return uow.EvaluationEnqueteAutoRepository.GetEvaluationsSenderBetweenTwoDates(id, dateDebut, dateFin);
        }
    }
}
