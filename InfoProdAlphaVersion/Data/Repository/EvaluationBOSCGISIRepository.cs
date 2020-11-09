
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
    public class EvaluationBOSCGISIRepository : RepositoryBase<GrilleEvaluationBO>, IEvaluationBOSCGISIRepository
    {
        public EvaluationBOSCGISIRepository(IDatabaseFactory dbFactory)
            : base(dbFactory)
        {

        }
        ReportContext context = new ReportContext();

        public void DeleteEvaluations(int? id, int? empId)
        {
            GrilleEvaluationBO evaluation = context.GrilleEvaluationBOSCGISI.FirstOrDefault(a => a.Id == id && a.employeeId == empId);
            context.GrilleEvaluationBOSCGISI.Remove(evaluation);

            context.SaveChanges();

        }

        public List<GrilleEvaluationBO> GetEvaluationsByEmployee(int id)
        {
            ReportContext context = new ReportContext();

            var evaluations = context.GrilleEvaluationBOSCGISI.Where(a => a.employeeId == id);
            List<GrilleEvaluationBO> tests = new List<GrilleEvaluationBO>();
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

        public List<GrilleEvaluationBO> GetEvaluationsBySenderId(int id)
        {
            ReportContext context = new ReportContext();

            var evaluations = context.GrilleEvaluationBOSCGISI.Where(a => a.senderId == id);
            List<GrilleEvaluationBO> tests = new List<GrilleEvaluationBO>();
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


        public List<GrilleEvaluationBO> GetEvaluationsEmployeeBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin)
        {

            ReportContext context = new ReportContext();

            var evaluations = context.GrilleEvaluationBOSCGISI.Where(a => a.employeeId == id && (EntityFunctions.TruncateTime(a.dateTempEvaluation) >= dateDebut.Date && (EntityFunctions.TruncateTime(a.dateTempEvaluation)) <= dateFin.Date));
            List<GrilleEvaluationBO> tests = new List<GrilleEvaluationBO>();
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

        public List<GrilleEvaluationBO> GetEvaluationsBetweenTwoDates(DateTime dateDebut, DateTime dateFin)
        {

            ReportContext context = new ReportContext();

            var evaluations = context.GrilleEvaluationBOSCGISI.Where(a =>(EntityFunctions.TruncateTime(a.dateTempEvaluation) >= dateDebut.Date && (EntityFunctions.TruncateTime(a.dateTempEvaluation)) <= dateFin.Date));
            List<GrilleEvaluationBO> tests = new List<GrilleEvaluationBO>();
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

        public List<GrilleEvaluationBO> GetEvaluationsSenderBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin)
        {

            ReportContext context = new ReportContext();

            var evaluations = context.GrilleEvaluationBOSCGISI.Where(a => a.senderId == id && (EntityFunctions.TruncateTime(a.dateTempEvaluation) >= dateDebut.Date && (EntityFunctions.TruncateTime(a.dateTempEvaluation)) <= dateFin.Date));
            List<GrilleEvaluationBO> tests = new List<GrilleEvaluationBO>();
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

        public GrilleEvaluationBO getById(int? id)
        {
            ReportContext context = new ReportContext();
            var evaluation = context.GrilleEvaluationBOSCGISI.FirstOrDefault(a => a.Id == id);
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
