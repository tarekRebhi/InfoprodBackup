using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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
    public class EvaluationAchatPublicRepository : RepositoryBase<GrilleEvaluationAchatPublic>, IEvaluationAchatPublicRepository
    {
        public EvaluationAchatPublicRepository(IDatabaseFactory dbFactory)
            : base(dbFactory)
        {

        }
        ReportContext context = new ReportContext();

        public void DeleteEvaluations(int? id, int? empId)
        {

            GrilleEvaluationAchatPublic evaluation = context.GrilleEvaluationAchatPublics.FirstOrDefault(a => a.Id == id && a.employeeId == empId);

            context.GrilleEvaluationAchatPublics.Remove(evaluation);

            context.SaveChanges();

        }
        public List<GrilleEvaluationAchatPublic> GetEvaluationsByEmployee(int id)
        {
            ReportContext context = new ReportContext();

            var evaluations = context.GrilleEvaluationAchatPublics.Where(a => a.employeeId == id);
            List<GrilleEvaluationAchatPublic> tests = new List<GrilleEvaluationAchatPublic>();
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

        public List<GrilleEvaluationAchatPublic> GetEvaluationsBySenderId(int id)
        {
            ReportContext context = new ReportContext();

            var evaluations = context.GrilleEvaluationAchatPublics.Where(a => a.senderId == id);
            List<GrilleEvaluationAchatPublic> tests = new List<GrilleEvaluationAchatPublic>();
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

       
        public List<GrilleEvaluationAchatPublic> GetEvaluationsEmployeeBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin)
        {

            ReportContext context = new ReportContext();

            var evaluations = context.GrilleEvaluationAchatPublics.Where(a => a.employeeId == id && (EntityFunctions.TruncateTime(a.dateTempEvaluation) >= dateDebut.Date && (EntityFunctions.TruncateTime(a.dateTempEvaluation)) <= dateFin.Date));
            List<GrilleEvaluationAchatPublic> tests = new List<GrilleEvaluationAchatPublic>();
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

        public List<GrilleEvaluationAchatPublic> GetEvaluationsBetweenTwoDates(DateTime dateDebut, DateTime dateFin)
        {

            ReportContext context = new ReportContext();

            var evaluations = context.GrilleEvaluationAchatPublics.Where(a => (EntityFunctions.TruncateTime(a.dateTempEvaluation) >= dateDebut.Date && (EntityFunctions.TruncateTime(a.dateTempEvaluation)) <= dateFin.Date));
            List<GrilleEvaluationAchatPublic> tests = new List<GrilleEvaluationAchatPublic>();
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

        public List<GrilleEvaluationAchatPublic> GetEvaluationsSenderBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin)
        {

            ReportContext context = new ReportContext();

            var evaluations = context.GrilleEvaluationAchatPublics.Where(a => a.senderId == id && (EntityFunctions.TruncateTime(a.dateTempEvaluation) >= dateDebut.Date && (EntityFunctions.TruncateTime(a.dateTempEvaluation)) <= dateFin.Date));
            List<GrilleEvaluationAchatPublic> tests = new List<GrilleEvaluationAchatPublic>();
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

        public GrilleEvaluationAchatPublic getById(int? id)
        {
            ReportContext context = new ReportContext();
            var evaluation = context.GrilleEvaluationAchatPublics.FirstOrDefault(a => a.Id == id);
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
