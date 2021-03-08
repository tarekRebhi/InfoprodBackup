using Domain.Entity;
using MyReports.Data.Infrastracture;
using MyReports.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;

namespace Data.Repository
{
    public class EvaluationBPPTypologieRepository : RepositoryBase<GrilleEvaluationBPPTypologie>, IEvaluationBPPTypologieRepository
    {
        
            public EvaluationBPPTypologieRepository(IDatabaseFactory dbFactory)
                : base(dbFactory)
            {

            }
            ReportContext context = new ReportContext();

            public void DeleteEvaluations(int? id, int? empId)
            {

                //var evaluationsemps = context.GrilleEvaluationBPPs.Where(a => a.employeeId == id);
                GrilleEvaluationBPPTypologie evaluation = context.GrilleEvaluationBPPTypologies.FirstOrDefault(a => a.Id == id && a.employeeId == empId);

                context.GrilleEvaluationBPPTypologies.Remove(evaluation);

                context.SaveChanges();

            }
            public List<GrilleEvaluationBPPTypologie> GetEvaluationsByEmployee(int id)
            {
                ReportContext context = new ReportContext();

                var evaluations = context.GrilleEvaluationBPPTypologies.Where(a => a.employeeId == id);
                List<GrilleEvaluationBPPTypologie> tests = new List<GrilleEvaluationBPPTypologie>();
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

            public List<GrilleEvaluationBPPTypologie> GetEvaluationsBySenderId(int id)
            {
                ReportContext context = new ReportContext();

                var evaluations = context.GrilleEvaluationBPPTypologies.Where(a => a.senderId == id);
                List<GrilleEvaluationBPPTypologie> tests = new List<GrilleEvaluationBPPTypologie>();
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

            public List<GrilleEvaluationBPPTypologie> GetEvaluationsEmployeeBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin)
            {

                ReportContext context = new ReportContext();

                var evaluations = context.GrilleEvaluationBPPTypologies.Where(a => a.employeeId == id && (EntityFunctions.TruncateTime(a.dateTempEvaluation) >= dateDebut.Date && (EntityFunctions.TruncateTime(a.dateTempEvaluation)) <= dateFin.Date));
                List<GrilleEvaluationBPPTypologie> tests = new List<GrilleEvaluationBPPTypologie>();
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

            public List<GrilleEvaluationBPPTypologie> GetEvaluationsBetweenTwoDates(DateTime dateDebut, DateTime dateFin)
            {

                ReportContext context = new ReportContext();

                var evaluations = context.GrilleEvaluationBPPTypologies.Where(a => (EntityFunctions.TruncateTime(a.dateTempEvaluation) >= dateDebut.Date && (EntityFunctions.TruncateTime(a.dateTempEvaluation)) <= dateFin.Date));
                List<GrilleEvaluationBPPTypologie> tests = new List<GrilleEvaluationBPPTypologie>();
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

            public List<GrilleEvaluationBPPTypologie> GetEvaluationsSenderBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin)
            {

                ReportContext context = new ReportContext();

                var evaluations = context.GrilleEvaluationBPPTypologies.Where(a => a.senderId == id && (EntityFunctions.TruncateTime(a.dateTempEvaluation) >= dateDebut.Date && (EntityFunctions.TruncateTime(a.dateTempEvaluation)) <= dateFin.Date));
                List<GrilleEvaluationBPPTypologie> tests = new List<GrilleEvaluationBPPTypologie>();
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

            public GrilleEvaluationBPPTypologie getById(int? id)
            {
                ReportContext context = new ReportContext();
                var evaluation = context.GrilleEvaluationBPPTypologies.FirstOrDefault(a => a.Id == id);
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
