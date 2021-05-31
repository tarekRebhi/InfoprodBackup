
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
    public class EvaluationHLAutoService : IEvaluationHLAutoService
    {
        IDatabaseFactory dbfactory = null;
        IUnitOfWork uow = null;

        public EvaluationHLAutoService()
        {
            dbfactory = new DatabaseFactory();
            uow = new UnitOfWork(dbfactory);
        }
        public void Add(GrilleEvaluationHLAuto evaluation)
        {
            uow.EvaluationHLAutoRepository.Add(evaluation);
        }


        public void Delete(GrilleEvaluationHLAuto evaluation)
        {
            uow.EvaluationHLAutoRepository.Delete(evaluation);
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

        public GrilleEvaluationHLAuto getById(int? id)
        {
            return uow.EvaluationHLAutoRepository.getById(id);
        }
        public GrilleEvaluation getEvalById(int? id)
        {
            return uow.EvaluationRepository.getById(id);
        }
        public List<GrilleEvaluationHLAuto> GetEvaluationsByEmployee(int id)
        {
            return uow.EvaluationHLAutoRepository.GetEvaluationsByEmployee(id);
        }
        public List<GrilleEvaluationHLAuto> GetEvaluationsBySenderId(int id)
        {
            return uow.EvaluationHLAutoRepository.GetEvaluationsBySenderId(id);
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
            uow.EvaluationHLAutoRepository.DeleteEvaluations(id, empId);
        }

        public List<GrilleEvaluationHLAuto> GetEvaluationsEmployeeBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin)
        {
            return uow.EvaluationHLAutoRepository.GetEvaluationsEmployeeBetweenTwoDates(id, dateDebut, dateFin);
        }

        public List<GrilleEvaluationHLAuto> GetEvaluationsBetweenTwoDates(DateTime dateDebut, DateTime dateFin)
        {
            return uow.EvaluationHLAutoRepository.GetEvaluationsBetweenTwoDates(dateDebut, dateFin);
        }


        public List<GrilleEvaluationHLAuto> GetEvaluationsSenderBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin)
        {
            return uow.EvaluationHLAutoRepository.GetEvaluationsSenderBetweenTwoDates(id, dateDebut, dateFin);
        }
        public IEnumerable<GrilleEvaluationHLAuto> GetAll()
        {

            return uow.EvaluationHLAutoRepository.GetAll();
        }
    }
}

