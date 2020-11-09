
//using Domain.Entity;
//using MyReports.Data.Infrastracture;
//using MyReports.Data.Infrastructure;
//using System;
//using System.Collections.Generic;
//using System.Data.Entity.Core.Objects;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Data.Repository
//{
//    public class EvaluationPRVRepository : RepositoryBase<GrilleEvaluationPRV>, IEvaluationPRVRepository
//    {
//        public EvaluationPRVRepository(IDatabaseFactory dbFactory)
//            : base(dbFactory)
//        {

//        }
//        ReportContext context = new ReportContext();

//        public void DeleteEvaluations(int? id, int? empId)
//        {
//            GrilleEvaluationQR evaluation = context.GrilleEvaluationQRs.FirstOrDefault(a => a.Id == id && a.employeeId == empId);

//            context.GrilleEvaluationQRs.Remove(evaluation);

//            context.SaveChanges();

//        }

//        public List<GrilleEvaluationQR> GetEvaluationsByEmployee(int id)
//        {
//            ReportContext context = new ReportContext();

//            var evaluations = context.GrilleEvaluationQRs.Where(a => a.employeeId == id);
//            List<GrilleEvaluationQR> tests = new List<GrilleEvaluationQR>();
//            tests.AddRange(evaluations);
//            if (evaluations != null)
//            {
//                return tests;
//            }
//            else
//            {
//                return null;
//            }
//        }

//        public List<GrilleEvaluationQR> GetEvaluationsBySenderId(int id)
//        {
//            ReportContext context = new ReportContext();

//            var evaluations = context.GrilleEvaluationQRs.Where(a => a.senderId == id);
//            List<GrilleEvaluationQR> tests = new List<GrilleEvaluationQR>();
//            tests.AddRange(evaluations);
//            if (evaluations != null)
//            {
//                return tests;
//            }
//            else
//            {
//                return null;
//            }
//        }

//        //public List<GrilleEvaluation> GetEvaluationsEmployeeByDate(int id, DateTime date)
//        //{
//        //    ReportContext context = new ReportContext();

//        //    var evaluations = context.evaluations.Where(a => a.employeeId == id && EntityFunctions.TruncateTime(a.dateTempEvaluation) == date.Date);
//        //    List<GrilleEvaluation> tests = new List<GrilleEvaluation>();
//        //    tests.AddRange(evaluations);
//        //    if (evaluations != null)
//        //    {
//        //        return tests;
//        //    }
//        //    else
//        //    {
//        //        return null;
//        //    }
//        //}

//        public List<GrilleEvaluationQR> GetEvaluationsEmployeeBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin)
//        {

//            ReportContext context = new ReportContext();

//            var evaluations = context.GrilleEvaluationQRs.Where(a => a.employeeId == id && (EntityFunctions.TruncateTime(a.dateTempEvaluation) >= dateDebut.Date && (EntityFunctions.TruncateTime(a.dateTempEvaluation)) <= dateFin.Date));
//            List<GrilleEvaluationQR> tests = new List<GrilleEvaluationQR>();
//            tests.AddRange(evaluations);
//            if (evaluations != null)
//            {
//                return tests;
//            }
//            else
//            {
//                return null;
//            }
//        }

//        public List<GrilleEvaluationQR> GetEvaluationsBetweenTwoDates(DateTime dateDebut, DateTime dateFin)
//        {

//            ReportContext context = new ReportContext();

//            var evaluations = context.GrilleEvaluationQRs.Where(a => (EntityFunctions.TruncateTime(a.dateTempEvaluation) >= dateDebut.Date && (EntityFunctions.TruncateTime(a.dateTempEvaluation)) <= dateFin.Date));
//            List<GrilleEvaluationQR> tests = new List<GrilleEvaluationQR>();
//            tests.AddRange(evaluations);
//            if (evaluations != null)
//            {
//                return tests;
//            }
//            else
//            {
//                return null;
//            }
//        }

//        public List<GrilleEvaluationQR> GetEvaluationsSenderBetweenTwoDates(int id, DateTime dateDebut, DateTime dateFin)
//        {

//            ReportContext context = new ReportContext();

//            var evaluations = context.GrilleEvaluationQRs.Where(a => a.senderId == id && (EntityFunctions.TruncateTime(a.dateTempEvaluation) >= dateDebut.Date && (EntityFunctions.TruncateTime(a.dateTempEvaluation)) <= dateFin.Date));
//            List<GrilleEvaluationQR> tests = new List<GrilleEvaluationQR>();
//            tests.AddRange(evaluations);
//            if (evaluations != null)
//            {
//                return tests;
//            }
//            else
//            {
//                return null;
//            }
//        }

//        public GrilleEvaluationQR getById(int? id)
//        {
//            ReportContext context = new ReportContext();
//            var evaluation = context.GrilleEvaluationQRs.FirstOrDefault(a => a.Id == id);
//            if (evaluation != null)
//            {

//                return evaluation;
//            }
//            else
//            {
//                return null;
//            }
//        }
//    }
//}
