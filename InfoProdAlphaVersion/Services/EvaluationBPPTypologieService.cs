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
    public class EvaluationBPPTypologieService :IEvaluationBppTypologieService
    {
        IDatabaseFactory dbfactory = null;
        IUnitOfWork uow = null;

        public EvaluationBPPTypologieService()
        {
            dbfactory = new DatabaseFactory();
            uow = new UnitOfWork(dbfactory);
        }
        public void Add(GrilleEvaluationBPPTypologie evaluation)
        {
            uow.EvaluationBPPTypologieRepository.Add(evaluation);
        }


        public void Delete(GrilleEvaluationBPPTypologie evaluation)
        {
            uow.EvaluationBPPTypologieRepository.Delete(evaluation);
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

        public GrilleEvaluationBPPTypologie getById(int? id)
        {
            return uow.EvaluationBPPTypologieRepository.getById(id);
        }
        public GrilleEvaluation getEvalById(int? id)
        {
            return uow.EvaluationRepository.getById(id);
        }
        public List<GrilleEvaluationBPPTypologie> GetEvaluationsByEmployee(int id)
        {
            return uow.EvaluationBPPTypologieRepository.GetEvaluationsByEmployee(id);
        }
        public List<GrilleEvaluationBPPTypologie> GetEvaluationsBySenderId(int id)
        {
            return uow.EvaluationBPPTypologieRepository.GetEvaluationsBySenderId(id);
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
            uow.EvaluationBPPTypologieRepository.DeleteEvaluations(id, empId);
        }

        public List<GrilleEvaluationBPPTypologie> GetEvaluationsEmployeeBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin)
        {
            return uow.EvaluationBPPTypologieRepository.GetEvaluationsEmployeeBetweenTwoDates(id, dateDebut, dateFin);
        }

        public List<GrilleEvaluationBPPTypologie> GetEvaluationsBetweenTwoDates(DateTime dateDebut, DateTime dateFin)
        {
            return uow.EvaluationBPPTypologieRepository.GetEvaluationsBetweenTwoDates(dateDebut, dateFin);
        }

        public List<GrilleEvaluationBPPTypologie> GetEvaluationsSenderBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin)
        {
            return uow.EvaluationBPPTypologieRepository.GetEvaluationsSenderBetweenTwoDates(id, dateDebut, dateFin);
        }
        public IEnumerable<GrilleEvaluationBPPTypologie> GetAll()
        {

            return uow.EvaluationBPPTypologieRepository.GetAll();
        }
    }
}
