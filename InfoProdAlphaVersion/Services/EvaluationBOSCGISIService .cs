
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
    public class EvaluationBOSCGISIService : IEvaluationBOSCGISIService
    {
        IDatabaseFactory dbfactory = null;
        IUnitOfWork uow = null;

        public EvaluationBOSCGISIService()
        {
            dbfactory = new DatabaseFactory();
            uow = new UnitOfWork(dbfactory);
        }
        public void Add(GrilleEvaluationBO evaluation)
        {
            uow.EvaluationBOSCGISIRepository.Add(evaluation);
        }


        public void Delete(GrilleEvaluationBO evaluation)
        {
            uow.EvaluationBOSCGISIRepository.Delete(evaluation);
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

        public GrilleEvaluationBO getById(int? id)
        {
            return uow.EvaluationBOSCGISIRepository.getById(id);
        }
        //public GrilleEvaluation getEvalById(int? id)
        //{
        //    return uow.EvaluationRepository.getById(id);
        //}
        public List<GrilleEvaluationBO> GetEvaluationsByEmployee(int id)
        {
            return uow.EvaluationBOSCGISIRepository.GetEvaluationsByEmployee(id);
        }
        public List<GrilleEvaluationBO> GetEvaluationsBySenderId(int id)
        {
            return uow.EvaluationBOSCGISIRepository.GetEvaluationsBySenderId(id);
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
            uow.EvaluationBOSCGISIRepository.DeleteEvaluations(id, empId);
        }

        public List<GrilleEvaluationBO> GetEvaluationsEmployeeBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin)
        {
            return uow.EvaluationBOSCGISIRepository.GetEvaluationsEmployeeBetweenTwoDates(id, dateDebut, dateFin);
        }

        public List<GrilleEvaluationBO> GetEvaluationsBetweenTwoDates(DateTime dateDebut, DateTime dateFin)
        {
            return uow.EvaluationBOSCGISIRepository.GetEvaluationsBetweenTwoDates(dateDebut, dateFin);
        }
        public List<GrilleEvaluationBO> GetEvaluationsSenderBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin)
        {
            return uow.EvaluationBOSCGISIRepository.GetEvaluationsSenderBetweenTwoDates(id, dateDebut, dateFin);
        }
    }
}
