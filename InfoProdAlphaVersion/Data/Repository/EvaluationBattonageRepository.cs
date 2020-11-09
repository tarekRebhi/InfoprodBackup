
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
    public class EvaluationBattonageRepository : RepositoryBase<GrilleEvaluationBattonage>, IEvaluationBattonageRepository
    {
        public EvaluationBattonageRepository(IDatabaseFactory dbFactory)
            : base(dbFactory)
        {

        }
        ReportContext context = new ReportContext();

        public void DeleteEvaluations(int? id, int? empId)
        {

            //var evaluationsemps = context.GrilleEvaluationBPPs.Where(a => a.employeeId == id);
            GrilleEvaluationBattonage evaluation = context.GrilleEvaluationBattonages.FirstOrDefault(a => a.Id == id && a.employeeId == empId);

            context.GrilleEvaluationBattonages.Remove(evaluation);

            context.SaveChanges();

        }
        public List<GrilleEvaluationBattonage> GetEvaluationsByEmployee(int id)
        {
            ReportContext context = new ReportContext();

            var evaluations = context.GrilleEvaluationBattonages.Where(a => a.employeeId == id);
            List<GrilleEvaluationBattonage> tests = new List<GrilleEvaluationBattonage>();
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

        public List<GrilleEvaluationBattonage> GetEvaluationsBySenderId(int id)
        {
            ReportContext context = new ReportContext();

            var evaluations = context.GrilleEvaluationBattonages.Where(a => a.senderId == id);
            List<GrilleEvaluationBattonage> tests = new List<GrilleEvaluationBattonage>();
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

        public List<GrilleEvaluationBattonage> GetEvaluationsEmployeeBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin)
        {

            ReportContext context = new ReportContext();

            var evaluations = context.GrilleEvaluationBattonages.Where(a => a.employeeId == id && (EntityFunctions.TruncateTime(a.dateTempEvaluation) >= dateDebut.Date && (EntityFunctions.TruncateTime(a.dateTempEvaluation)) <= dateFin.Date));
            List<GrilleEvaluationBattonage> tests = new List<GrilleEvaluationBattonage>();
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


        public List<GrilleEvaluationBattonage> GetEvaluationsBetweenTwoDates(DateTime dateDebut, DateTime dateFin)
        {

            ReportContext context = new ReportContext();

            var evaluations = context.GrilleEvaluationBattonages.Where(a => (EntityFunctions.TruncateTime(a.dateTempEvaluation) >= dateDebut.Date && (EntityFunctions.TruncateTime(a.dateTempEvaluation)) <= dateFin.Date));
            List<GrilleEvaluationBattonage> tests = new List<GrilleEvaluationBattonage>();
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

        public List<GrilleEvaluationBattonage> GetEvaluationsSenderBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin)
        {

            ReportContext context = new ReportContext();

            var evaluations = context.GrilleEvaluationBattonages.Where(a => a.senderId == id && (EntityFunctions.TruncateTime(a.dateTempEvaluation) >= dateDebut.Date && (EntityFunctions.TruncateTime(a.dateTempEvaluation)) <= dateFin.Date));
            List<GrilleEvaluationBattonage> tests = new List<GrilleEvaluationBattonage>();
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

        public GrilleEvaluationBattonage getById(int? id)
        {
            ReportContext context = new ReportContext();
            var evaluation = context.GrilleEvaluationBattonages.FirstOrDefault(a => a.Id == id);
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
