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
    public class EvaluationBPPRepository : RepositoryBase<GrilleEvaluationBPP>, IEvaluationBPPRepository
    {
        public EvaluationBPPRepository(IDatabaseFactory dbFactory)
            : base(dbFactory)
        {

        }
        ReportContext context = new ReportContext();

        public void DeleteEvaluations(int? id, int? empId)
        {

            //var evaluationsemps = context.GrilleEvaluationBPPs.Where(a => a.employeeId == id);
            GrilleEvaluationBPP evaluation = context.GrilleEvaluationBPPs.FirstOrDefault(a => a.Id == id && a.employeeId == empId);
           
                context.GrilleEvaluationBPPs.Remove(evaluation);

            context.SaveChanges();

        }
        public List<GrilleEvaluationBPP> GetEvaluationsByEmployee(int id)
        {
            ReportContext context = new ReportContext();

            var evaluations = context.GrilleEvaluationBPPs.Where(a => a.employeeId == id);
            List<GrilleEvaluationBPP> tests = new List<GrilleEvaluationBPP>();
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

        public List<GrilleEvaluationBPP> GetEvaluationsBySenderId(int id)
        {
            ReportContext context = new ReportContext();

            var evaluations = context.GrilleEvaluationBPPs.Where(a => a.senderId == id);
            List<GrilleEvaluationBPP> tests = new List<GrilleEvaluationBPP>();
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

        public List<GrilleEvaluationBPP> GetEvaluationsEmployeeBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin)
        {

            ReportContext context = new ReportContext();

            var evaluations = context.GrilleEvaluationBPPs.Where(a => a.employeeId == id && (EntityFunctions.TruncateTime(a.dateTempEvaluation) >= dateDebut.Date && (EntityFunctions.TruncateTime(a.dateTempEvaluation)) <= dateFin.Date));
            List<GrilleEvaluationBPP> tests = new List<GrilleEvaluationBPP>();
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

        public List<GrilleEvaluationBPP> GetEvaluationsBetweenTwoDates(DateTime dateDebut, DateTime dateFin)
        {

            ReportContext context = new ReportContext();

            var evaluations = context.GrilleEvaluationBPPs.Where(a => (EntityFunctions.TruncateTime(a.dateTempEvaluation) >= dateDebut.Date && (EntityFunctions.TruncateTime(a.dateTempEvaluation)) <= dateFin.Date));
            List<GrilleEvaluationBPP> tests = new List<GrilleEvaluationBPP>();
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

        public List<GrilleEvaluationBPP> GetEvaluationsSenderBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin)
        {

            ReportContext context = new ReportContext();

            var evaluations = context.GrilleEvaluationBPPs.Where(a => a.senderId == id && (EntityFunctions.TruncateTime(a.dateTempEvaluation) >= dateDebut.Date && (EntityFunctions.TruncateTime(a.dateTempEvaluation)) <= dateFin.Date));
            List<GrilleEvaluationBPP> tests = new List<GrilleEvaluationBPP>();
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

        public GrilleEvaluationBPP getById(int? id)
        {
            ReportContext context = new ReportContext();
            var evaluation = context.GrilleEvaluationBPPs.FirstOrDefault(a => a.Id == id);
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
