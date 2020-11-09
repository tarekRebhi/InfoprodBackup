using Domain.Entity;
using MyReports.Data.Infrastracture;
using MyReports.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repository
{
    public class EvaluationKLMORepository : RepositoryBase<GrilleEvaluationKLMO>, IEvaluationKLMORepository
    {
        public EvaluationKLMORepository(IDatabaseFactory dbFactory)
            : base(dbFactory)
        {

        }
        ReportContext context = new ReportContext();

        public void DeleteEvaluations(int? id, int? empId)
        {
            GrilleEvaluationKLMO evaluation = context.GrilleEvaluationKLMOes.FirstOrDefault(a => a.Id == id && a.employeeId == empId);
            context.GrilleEvaluationKLMOes.Remove(evaluation);
            context.SaveChanges();

        }
        public List<GrilleEvaluationKLMO> GetEvaluationsByEmployee(int id)
        {
            ReportContext context = new ReportContext();

            var evaluations = context.GrilleEvaluationKLMOes.Where(a => a.employeeId == id);
            List<GrilleEvaluationKLMO> tests = new List<GrilleEvaluationKLMO>();
            tests.AddRange(evaluations);
            if (evaluations != null)
            {
                return tests;
            }
            else
            {
                return null;
            }
        }

        public List<GrilleEvaluationKLMO> GetEvaluationsBySenderId(int id)
        {
            ReportContext context = new ReportContext();

            var evaluations = context.GrilleEvaluationKLMOes.Where(a => a.senderId == id);
            List<GrilleEvaluationKLMO> tests = new List<GrilleEvaluationKLMO>();
            tests.AddRange(evaluations);
            if (evaluations != null)
            {
                return tests;
            }
            else
            {
                return null;
            }
        }

        //public List<GrilleEvaluation> GetEvaluationsEmployeeByDate(int id, DateTime date)
        //{
        //    ReportContext context = new ReportContext();

        //    var evaluations = context.evaluations.Where(a => a.employeeId == id && EntityFunctions.TruncateTime(a.dateTempEvaluation) == date.Date);
        //    List<GrilleEvaluation> tests = new List<GrilleEvaluation>();
        //    tests.AddRange(evaluations);
        //    if (evaluations != null)
        //    {
        //        return tests;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        public List<GrilleEvaluationKLMO> GetEvaluationsEmployeeBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin)
        {

            ReportContext context = new ReportContext();

            var evaluations = context.GrilleEvaluationKLMOes.Where(a => a.employeeId == id && (EntityFunctions.TruncateTime(a.dateTempEvaluation) >= dateDebut.Date && (EntityFunctions.TruncateTime(a.dateTempEvaluation)) <= dateFin.Date));
            List<GrilleEvaluationKLMO> tests = new List<GrilleEvaluationKLMO>();
            tests.AddRange(evaluations);
            if (evaluations != null)
            {
                return tests;
            }
            else
            {
                return null;
            }
        }

        public List<GrilleEvaluationKLMO> GetEvaluationsBetweenTwoDates(DateTime dateDebut, DateTime dateFin)
        {

            ReportContext context = new ReportContext();

            var evaluations = context.GrilleEvaluationKLMOes.Where(a => (EntityFunctions.TruncateTime(a.dateTempEvaluation) >= dateDebut.Date && (EntityFunctions.TruncateTime(a.dateTempEvaluation)) <= dateFin.Date));
            List<GrilleEvaluationKLMO> tests = new List<GrilleEvaluationKLMO>();
            tests.AddRange(evaluations);
            if (evaluations != null)
            {
                return tests;
            }
            else
            {
                return null;
            }
        }


        public List<GrilleEvaluationKLMO> GetEvaluationsSenderBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin)
        {

            ReportContext context = new ReportContext();

            var evaluations = context.GrilleEvaluationKLMOes.Where(a => a.senderId == id && (EntityFunctions.TruncateTime(a.dateTempEvaluation) >= dateDebut.Date && (EntityFunctions.TruncateTime(a.dateTempEvaluation)) <= dateFin.Date));
            List<GrilleEvaluationKLMO> tests = new List<GrilleEvaluationKLMO>();
            tests.AddRange(evaluations);
            if (evaluations != null)
            {
                return tests;
            }
            else
            {
                return null;
            }
        }

        public GrilleEvaluationKLMO getById(int? id)
        {
            ReportContext context = new ReportContext();
            var evaluation = context.GrilleEvaluationKLMOes.FirstOrDefault(a => a.Id == id);
            if (evaluation != null)
            {

                return evaluation;
            }
            else
            {
                return null;
            }
        }
    }
}
