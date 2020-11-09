
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
    public class EvaluationFOSAMRCRepository : RepositoryBase<GrilleEvaluationFOSAMRC>, IEvaluationFOSAMRCRepository
    {
        public EvaluationFOSAMRCRepository(IDatabaseFactory dbFactory)
            : base(dbFactory)
        {

        }
        ReportContext context = new ReportContext();

        public void DeleteEvaluations(int? id, int? empId)
        {

            //var evaluationsemps = context.GrilleEvaluationBPPs.Where(a => a.employeeId == id);
            GrilleEvaluationFOSAMRC evaluation = context.GrilleEvaluationFOSAMRCs.FirstOrDefault(a => a.Id == id && a.employeeId == empId);

            context.GrilleEvaluationFOSAMRCs.Remove(evaluation);

            context.SaveChanges();

        }
        public List<GrilleEvaluationFOSAMRC> GetEvaluationsByEmployee(int id)
        {
            ReportContext context = new ReportContext();

            var evaluations = context.GrilleEvaluationFOSAMRCs.Where(a => a.employeeId == id);
            List<GrilleEvaluationFOSAMRC> tests = new List<GrilleEvaluationFOSAMRC>();
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

        public List<GrilleEvaluationFOSAMRC> GetEvaluationsBySenderId(int id)
        {
            ReportContext context = new ReportContext();

            var evaluations = context.GrilleEvaluationFOSAMRCs.Where(a => a.senderId == id);
            List<GrilleEvaluationFOSAMRC> tests = new List<GrilleEvaluationFOSAMRC>();
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

        public List<GrilleEvaluationFOSAMRC> GetEvaluationsEmployeeBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin)
        {

            ReportContext context = new ReportContext();

            var evaluations = context.GrilleEvaluationFOSAMRCs.Where(a => a.employeeId == id && (EntityFunctions.TruncateTime(a.dateTempEvaluation) >= dateDebut.Date && (EntityFunctions.TruncateTime(a.dateTempEvaluation)) <= dateFin.Date));
            List<GrilleEvaluationFOSAMRC> tests = new List<GrilleEvaluationFOSAMRC>();
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

        public List<GrilleEvaluationFOSAMRC> GetEvaluationsBetweenTwoDates(DateTime dateDebut, DateTime dateFin)
        {

            ReportContext context = new ReportContext();

            var evaluations = context.GrilleEvaluationFOSAMRCs.Where(a => (EntityFunctions.TruncateTime(a.dateTempEvaluation) >= dateDebut.Date && (EntityFunctions.TruncateTime(a.dateTempEvaluation)) <= dateFin.Date));
            List<GrilleEvaluationFOSAMRC> tests = new List<GrilleEvaluationFOSAMRC>();
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

        public List<GrilleEvaluationFOSAMRC> GetEvaluationsSenderBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin)
        {

            ReportContext context = new ReportContext();

            var evaluations = context.GrilleEvaluationFOSAMRCs.Where(a => a.senderId == id && (EntityFunctions.TruncateTime(a.dateTempEvaluation) >= dateDebut.Date && (EntityFunctions.TruncateTime(a.dateTempEvaluation)) <= dateFin.Date));
            List<GrilleEvaluationFOSAMRC> tests = new List<GrilleEvaluationFOSAMRC>();
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

        public GrilleEvaluationFOSAMRC getById(int? id)
        {
            ReportContext context = new ReportContext();
            var evaluation = context.GrilleEvaluationFOSAMRCs.FirstOrDefault(a => a.Id == id);
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
