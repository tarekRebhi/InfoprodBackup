
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
    public class EvaluationBOSAMRCService : IEvaluationBOSAMRCService
    {
        IDatabaseFactory dbfactory = null;
        IUnitOfWork uow = null;

        public EvaluationBOSAMRCService()
        {
            dbfactory = new DatabaseFactory();
            uow = new UnitOfWork(dbfactory);
        }
        public void Add(GrilleEvaluationBOSAMRC evaluation)
        {
            uow.EvaluationBOSAMRCRepository.Add(evaluation);
        }


        public void Delete(GrilleEvaluationBOSAMRC evaluation)
        {
            uow.EvaluationBOSAMRCRepository.Delete(evaluation);
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

        public GrilleEvaluationBOSAMRC getById(int? id)
        {
            return uow.EvaluationBOSAMRCRepository.getById(id);
        }
        public GrilleEvaluation getEvalById(int? id)
        {
            return uow.EvaluationRepository.getById(id);
        }
        public List<GrilleEvaluationBOSAMRC> GetEvaluationsByEmployee(int id)
        {
            return uow.EvaluationBOSAMRCRepository.GetEvaluationsByEmployee(id);
        }
        public List<GrilleEvaluationBOSAMRC> GetEvaluationsBySenderId(int id)
        {
            return uow.EvaluationBOSAMRCRepository.GetEvaluationsBySenderId(id);
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
            uow.EvaluationBOSAMRCRepository.DeleteEvaluations(id, empId);
        }

        public List<GrilleEvaluationBOSAMRC> GetEvaluationsEmployeeBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin)
        {
            return uow.EvaluationBOSAMRCRepository.GetEvaluationsEmployeeBetweenTwoDates(id, dateDebut, dateFin);
        }

        public List<GrilleEvaluationBOSAMRC> GetEvaluationsBetweenTwoDates(DateTime dateDebut, DateTime dateFin)
        {
            return uow.EvaluationBOSAMRCRepository.GetEvaluationsBetweenTwoDates(dateDebut, dateFin);
        }

        public List<GrilleEvaluationBOSAMRC> GetEvaluationsSenderBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin)
        {
            return uow.EvaluationBOSAMRCRepository.GetEvaluationsSenderBetweenTwoDates(id, dateDebut, dateFin);
        }
        public IEnumerable<GrilleEvaluationBOSAMRC> GetAll()
        {

            return uow.EvaluationBOSAMRCRepository.GetAll();
        }
    }
}

