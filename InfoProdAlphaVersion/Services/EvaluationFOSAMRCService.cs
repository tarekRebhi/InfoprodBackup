
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
    public class EvaluationFOSAMRCService : IEvaluationFOSAMRCService
    {
        IDatabaseFactory dbfactory = null;
        IUnitOfWork uow = null;

        public EvaluationFOSAMRCService()
        {
            dbfactory = new DatabaseFactory();
            uow = new UnitOfWork(dbfactory);
        }
        public void Add(GrilleEvaluationFOSAMRC evaluation)
        {
            uow.EvaluationFOSAMRCRepository.Add(evaluation);
        }


        public void Delete(GrilleEvaluationFOSAMRC evaluation)
        {
            uow.EvaluationFOSAMRCRepository.Delete(evaluation);
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

        public GrilleEvaluationFOSAMRC getById(int? id)
        {
            return uow.EvaluationFOSAMRCRepository.getById(id);
        }
        public GrilleEvaluation getEvalById(int? id)
        {
            return uow.EvaluationRepository.getById(id);
        }
        public List<GrilleEvaluationFOSAMRC> GetEvaluationsByEmployee(int id)
        {
            return uow.EvaluationFOSAMRCRepository.GetEvaluationsByEmployee(id);
        }
        public List<GrilleEvaluationFOSAMRC> GetEvaluationsBySenderId(int id)
        {
            return uow.EvaluationFOSAMRCRepository.GetEvaluationsBySenderId(id);
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
            uow.EvaluationFOSAMRCRepository.DeleteEvaluations(id, empId);
        }

        public List<GrilleEvaluationFOSAMRC> GetEvaluationsEmployeeBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin)
        {
            return uow.EvaluationFOSAMRCRepository.GetEvaluationsEmployeeBetweenTwoDates(id, dateDebut, dateFin);
        }

        public List<GrilleEvaluationFOSAMRC> GetEvaluationsBetweenTwoDates(DateTime dateDebut, DateTime dateFin)
        {
            return uow.EvaluationFOSAMRCRepository.GetEvaluationsBetweenTwoDates(dateDebut, dateFin);
        }


        public List<GrilleEvaluationFOSAMRC> GetEvaluationsSenderBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin)
        {
            return uow.EvaluationFOSAMRCRepository.GetEvaluationsSenderBetweenTwoDates(id, dateDebut, dateFin);
        }
        public IEnumerable<GrilleEvaluationFOSAMRC> GetAll()
        {

            return uow.EvaluationFOSAMRCRepository.GetAll();
        }
    }
}

