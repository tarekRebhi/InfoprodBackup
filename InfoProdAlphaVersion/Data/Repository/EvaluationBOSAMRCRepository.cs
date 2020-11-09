
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
    public class EvaluationBOSAMRCRepository : RepositoryBase<GrilleEvaluationBOSAMRC>, IEvaluationBOSAMRCRepository
    {
        public EvaluationBOSAMRCRepository(IDatabaseFactory dbFactory)
            : base(dbFactory)
        {

        }
        ReportContext context = new ReportContext();

        public void DeleteEvaluations(int? id, int? empId)
        {

            //var evaluationsemps = context.GrilleEvaluationBPPs.Where(a => a.employeeId == id);
            GrilleEvaluationBOSAMRC evaluation = context.GrilleEvaluationBOSAMRCs.FirstOrDefault(a => a.Id == id && a.employeeId == empId);

            context.GrilleEvaluationBOSAMRCs.Remove(evaluation);

            context.SaveChanges();

        }
        public List<GrilleEvaluationBOSAMRC> GetEvaluationsByEmployee(int id)
        {
            ReportContext context = new ReportContext();

            var evaluations = context.GrilleEvaluationBOSAMRCs.Where(a => a.employeeId == id);
            List<GrilleEvaluationBOSAMRC> tests = new List<GrilleEvaluationBOSAMRC>();
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

        public List<GrilleEvaluationBOSAMRC> GetEvaluationsBySenderId(int id)
        {
            ReportContext context = new ReportContext();

            var evaluations = context.GrilleEvaluationBOSAMRCs.Where(a => a.senderId == id);
            List<GrilleEvaluationBOSAMRC> tests = new List<GrilleEvaluationBOSAMRC>();
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

        public List<GrilleEvaluationBOSAMRC> GetEvaluationsEmployeeBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin)
        {

            ReportContext context = new ReportContext();

            var evaluations = context.GrilleEvaluationBOSAMRCs.Where(a => a.employeeId == id && (EntityFunctions.TruncateTime(a.dateTempEvaluation) >= dateDebut.Date && (EntityFunctions.TruncateTime(a.dateTempEvaluation)) <= dateFin.Date));
            List<GrilleEvaluationBOSAMRC> tests = new List<GrilleEvaluationBOSAMRC>();
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

        public List<GrilleEvaluationBOSAMRC> GetEvaluationsBetweenTwoDates(DateTime dateDebut, DateTime dateFin)
        {

            ReportContext context = new ReportContext();

            var evaluations = context.GrilleEvaluationBOSAMRCs.Where(a => (EntityFunctions.TruncateTime(a.dateTempEvaluation) >= dateDebut.Date && (EntityFunctions.TruncateTime(a.dateTempEvaluation)) <= dateFin.Date));
            List<GrilleEvaluationBOSAMRC> tests = new List<GrilleEvaluationBOSAMRC>();
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

        public List<GrilleEvaluationBOSAMRC> GetEvaluationsSenderBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin)
        {

            ReportContext context = new ReportContext();

            var evaluations = context.GrilleEvaluationBOSAMRCs.Where(a => a.senderId == id && (EntityFunctions.TruncateTime(a.dateTempEvaluation) >= dateDebut.Date && (EntityFunctions.TruncateTime(a.dateTempEvaluation)) <= dateFin.Date));
            List<GrilleEvaluationBOSAMRC> tests = new List<GrilleEvaluationBOSAMRC>();
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

        public GrilleEvaluationBOSAMRC getById(int? id)
        {
            ReportContext context = new ReportContext();
            var evaluation = context.GrilleEvaluationBOSAMRCs.FirstOrDefault(a => a.Id == id);
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
