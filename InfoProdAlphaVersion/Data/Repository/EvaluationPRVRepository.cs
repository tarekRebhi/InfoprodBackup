
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
    public class EvaluationPRVRepository : RepositoryBase<GrilleEvaluationPRV>, IEvaluationPRVRepository
    {
        public EvaluationPRVRepository(IDatabaseFactory dbFactory)
            : base(dbFactory)
        {

        }
        ReportContext context = new ReportContext();

        public void DeleteEvaluations(int? id, int? empId)
        {
            GrilleEvaluationPRV evaluation = context.GrilleEvaluationPRVs.FirstOrDefault(a => a.Id == id && a.employeeId == empId);

            context.GrilleEvaluationPRVs.Remove(evaluation);

            context.SaveChanges();

        }

        public List<GrilleEvaluationPRV> GetEvaluationsByEmployee(int id)
        {
            ReportContext context = new ReportContext();

            var evaluations = context.GrilleEvaluationPRVs.Where(a => a.employeeId == id);
            List<GrilleEvaluationPRV> tests = new List<GrilleEvaluationPRV>();
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

        public List<GrilleEvaluationPRV> GetEvaluationsBySenderId(int id)
        {
            ReportContext context = new ReportContext();

            var evaluations = context.GrilleEvaluationPRVs.Where(a => a.senderId == id);
            List<GrilleEvaluationPRV> tests = new List<GrilleEvaluationPRV>();
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

        public List<GrilleEvaluationPRV> GetEvaluationsEmployeeBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin)
        {

            ReportContext context = new ReportContext();

            var evaluations = context.GrilleEvaluationPRVs.Where(a => a.employeeId == id && (EntityFunctions.TruncateTime(a.dateTempEvaluation) >= dateDebut.Date && (EntityFunctions.TruncateTime(a.dateTempEvaluation)) <= dateFin.Date));
            List<GrilleEvaluationPRV> tests = new List<GrilleEvaluationPRV>();
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

        public List<GrilleEvaluationPRV> GetEvaluationsBetweenTwoDates(DateTime dateDebut, DateTime dateFin)
        {

            ReportContext context = new ReportContext();

            var evaluations = context.GrilleEvaluationPRVs.Where(a => (EntityFunctions.TruncateTime(a.dateTempEvaluation) >= dateDebut.Date && (EntityFunctions.TruncateTime(a.dateTempEvaluation)) <= dateFin.Date));
            List<GrilleEvaluationPRV> tests = new List<GrilleEvaluationPRV>();
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

        public List<GrilleEvaluationPRV> GetEvaluationsSenderBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin)
        {

            ReportContext context = new ReportContext();

            var evaluations = context.GrilleEvaluationPRVs.Where(a => a.senderId == id && (EntityFunctions.TruncateTime(a.dateTempEvaluation) >= dateDebut.Date && (EntityFunctions.TruncateTime(a.dateTempEvaluation)) <= dateFin.Date));
            List<GrilleEvaluationPRV> tests = new List<GrilleEvaluationPRV>();
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

        public GrilleEvaluationPRV getById(int? id)
        {
            ReportContext context = new ReportContext();
            var evaluation = context.GrilleEvaluationPRVs.FirstOrDefault(a => a.Id == id);
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
