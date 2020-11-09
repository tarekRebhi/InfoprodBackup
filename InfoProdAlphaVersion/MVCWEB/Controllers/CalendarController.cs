using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCWEB.Models;
using Domain.Entity;
using System.Globalization;
using Services;
using Data;


namespace MVCWEB.Models
{
    public class CalendarController : Controller
    {
        IPlaningService service;
        public CalendarController()
        {

            service = new PlaningService();
        }

        public JsonResult GetEvents()
        {
            using (ReportContext dc = new ReportContext())
            {
                var events = dc.events.ToList();
                foreach (var test in events)
                {
                    test.dateDebut = DateTime.Now;
                    test.dateFin = DateTime.Now;
                }
                return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }
        public ActionResult Index()
        {
            var planings = service.GetAll();
            List<Planing> fVM = new List<Planing>();


            foreach (var item in planings)
            {
                fVM.Add(item);
            }

                //fVM = fVM.Where(p => p.titre.ToLower().Contains(search.ToLower())).ToList<PlaningCal>();
                return View(fVM);   //fVM.Take(10)

        }

        public ActionResult EnregistrerPlaning(DateTime NewPlanDate, DateTime NewPlanDate2, string NewPlanTime, string NewPlanTime2, string planGroups)
        {
            Planing planing = new Planing
            {

                dateDebut = NewPlanDate,
                dateFin = NewPlanDate2,
                heureDebut = NewPlanTime,
                heureFin = NewPlanTime2

            };
            service.Add(planing);
            service.SaveChange();
            var planings = service.GetAll();
            List<Planing> fVM = new List<Planing>();


            foreach (var item in planings)
            {
                fVM.Add(item);
            }

            //fVM = fVM.Where(p => p.titre.ToLower().Contains(search.ToLower())).ToList<PlaningCal>();
             //fVM.Take(10)
            return View("~/Views/Calendar/index.cshtml", fVM);
        }

        public ActionResult Autorisation()
        {
           
            return View();   //fVM.Take(10)

        }
        public ActionResult Conge()
        {

            return View();   //fVM.Take(10)

        }

        //public static int GetRandomValue(int LowerBound, int UpperBound)
        //{
        //    Random rnd = new Random();
        //    return rnd.Next(LowerBound, UpperBound);
        //}

        ///// <summary>
        ///// sends back a date/time +/- 15 days from todays date
        ///// </summary>
        //public static DateTime GetRandomAppointmentTime(bool GoBackwards, bool Today)
        //{
        //    Random rnd = new Random(Environment.TickCount); // set a new random seed each call
        //    var baseDate = DateTime.Today;
        //    if (Today)
        //        return new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, rnd.Next(9, 18), rnd.Next(1, 6) * 5, 0);
        //    else
        //    {
        //        int rndDays = rnd.Next(1, 16);
        //        if (GoBackwards)
        //            rndDays = rndDays * -1; // make into negative number
        //        return new DateTime(baseDate.Year, baseDate.Month, baseDate.Day, rnd.Next(9, 18), rnd.Next(1, 6) * 5, 0).AddDays(rndDays);
        //    }
        //}

    }
}
