
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
    public class EvaluationFOSCGISIService : IEvaluationFOSCGISIService
    {
        IDatabaseFactory dbfactory = null;
        IUnitOfWork uow = null;

        public EvaluationFOSCGISIService()
        {
            dbfactory = new DatabaseFactory();
            uow = new UnitOfWork(dbfactory);
        }
        public void Add(GrilleEvaluationFO evaluation)
        {
            uow.EvaluationFOSCGISIRepository.Add(evaluation);
        }


        public void Delete(GrilleEvaluationFO evaluation)
        {
            uow.EvaluationFOSCGISIRepository.Delete(evaluation);
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

        public GrilleEvaluationFO getById(int? id)
        {
            return uow.EvaluationFOSCGISIRepository.getById(id);
        }
        //public GrilleEvaluation getEvalById(int? id)
        //{
        //    return uow.EvaluationRepository.getById(id);
        //}
        public List<GrilleEvaluationFO> GetEvaluationsByEmployee(int id)
        {
            return uow.EvaluationFOSCGISIRepository.GetEvaluationsByEmployee(id);
        }
        public List<GrilleEvaluationFO> GetEvaluationsBySenderId(int id)
        {
            return uow.EvaluationFOSCGISIRepository.GetEvaluationsBySenderId(id);
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
            uow.EvaluationFOSCGISIRepository.DeleteEvaluations(id, empId);
        }

        public List<GrilleEvaluationFO> GetEvaluationsEmployeeBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin)
        {
            return uow.EvaluationFOSCGISIRepository.GetEvaluationsEmployeeBetweenTwoDates(id, dateDebut, dateFin);
        }
        public List<GrilleEvaluationFO> GetEvaluationsBetweenTwoDates(DateTime dateDebut, DateTime dateFin)
        {
            return uow.EvaluationFOSCGISIRepository.GetEvaluationsBetweenTwoDates(dateDebut, dateFin);
        }
        public List<GrilleEvaluationFO> GetEvaluationsSenderBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin)
        {
            return uow.EvaluationFOSCGISIRepository.GetEvaluationsSenderBetweenTwoDates(id, dateDebut, dateFin);
        }
    }
}
