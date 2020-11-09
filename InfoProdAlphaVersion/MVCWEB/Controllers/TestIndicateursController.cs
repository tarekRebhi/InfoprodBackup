using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVCWEB;
using MVCWEB.Models;
using System.Web.Script.Serialization;

namespace MVCWEB.Controllers
{
    public class TestIndicateursController : Controller
    {
        //private DBContext db = new DBContext();

        // GET: TestIndicateurs
        //public ActionResult Index()
        //{
           
        //    return View(db.TestIndicateurs.ToList());
        //}


        // GET: TestIndicateurs
        //[HttpGet]
        //public ActionResult Hebdo() {
        //    var semaines = new List<SelectListItem>();
        //    semaines.Add(new SelectListItem
        //    { Text = "1", Value = "1" });
        //    semaines.Add(new SelectListItem
        //    { Text = "2", Value = "2" });
        //    semaines.Add(new SelectListItem
        //    { Text = "se3", Value = "3" });
        //    semaines.Add(new SelectListItem
        //    { Text = "SE4", Value = "4" });
        //    semaines.Add(new SelectListItem
        //    { Text = "SE5", Value = "5" });

        //    ViewBag.SemaineItems = semaines;
        //    TestIndicateur a = new TestIndicateur();
        //    a.semaines = semaines;
        //    return View(a);
        //}
        // [HttpPost]

    

        //public ActionResult GetSelected(string stateID)
        //{
        //    //Here I'll bind the list of cities corresponding to selected state's state id  

        //    //List<CITy> lstcity = new List<CITy>();
        //    //int stateiD = Convert.ToInt32(stateID);
        //    //using (CITYSTATEEntities cITYSTATEEntities = new CITYSTATEEntities())
        //    //{
        //    //    lstcity = (cITYSTATEEntities.CITIES.Where(x => x.StateId == stateiD)).ToList<CITy>();
        //    //}

        //    var semaines = new List<SelectListItem>();
        //    semaines.Add(new SelectListItem
        //    { Text = "SE1", Value = "1" });
        //    semaines.Add(new SelectListItem
        //    { Text = "se2", Value = "2" });
        //    semaines.Add(new SelectListItem
        //    { Text = "se3", Value = "3" });
        //    semaines.Add(new SelectListItem
        //    { Text = "SE4", Value = "4" });
        //    semaines.Add(new SelectListItem
        //    { Text = "SE5", Value = "5" });

        //    ViewBag.SemaineItems = semaines;


        //    System.Web.Script.Serialization.JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
        //    string result = javaScriptSerializer.Serialize(lstcity);
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}
       // [HttpPost]
       // public ActionResult Test(JsonResult semaineSel)
       // {

       //return Json(semaineSel);
       // }

        //public ActionResult Hebdo(string semaineSel)
        //{
           
        //    var semaines = new List<SelectListItem>();
        //    semaines.Add(new SelectListItem
        //    { Text = "SE1", Value = "1" });
        //    semaines.Add(new SelectListItem
        //    { Text = "se2", Value = "2" });
        //    semaines.Add(new SelectListItem
        //    { Text = "se3", Value = "3" });
        //    semaines.Add(new SelectListItem
        //    { Text = "SE4", Value = "4" });
        //    semaines.Add(new SelectListItem
        //    { Text = "SE5", Value = "5" });

        //    ViewBag.SemaineItems = semaines;

            //  var list = new SelectList(new[]
            //                                  {
            //                                    new {Value=1,Text="semaine1"},
            //                                    new{Value=2,Text="semaine2"},
            //                                    new{Value=3,Text="semaine3"},
            //                                },
            //                    "Value", "Text", 1);
            //ViewData["list"] = list;
            //var sportslist = new List<string>();

            //var SemainesList = new List<SelectListItem>();
            //SemainesList.Add(new SelectListItem { Value = "1", Text = "Semaine1" });
            //SemainesList.Add(new SelectListItem { Value = "2", Text = "Semaine2" });
            //SemainesList.Add(new SelectListItem { Value = "3", Text = "Semaine3" });
            //ViewData["SemainesList"] = SemainesList;
            //if (semaineSel == null)
            //{ semaineSel = "0"; }
            //    if (semaineSel != null)
            //{

                //Json(new { success = true, message = "Order updated successfully" }, JsonRequestBehavior.AllowGet);

                // var test = (semaineSel); 

        //        ViewBag.b = semaineSel.ToString();


        //        int test = int.Parse(semaineSel);

        //        double TotAccord = 0;
        //        double TotCA = 0;
        //        double TotAcw = 0;
        //        double TotLog = 0;
        //        double TotPreview = 0;
        //        double TotPauseBrief = 0;
        //        double TotPausePerso = 0;
        //        double TotOccupation = 0;
        //        double TotCommunication = 0;
        //        double TotAppelEmis = 0;
        //        double TotProdReel = 0;
        //        double tempsPresence = 0;
        //        int TotJourTravaillés = 0;
        //        var indicateurs = db.TestIndicateurs.ToList();
        //        foreach (var item in indicateurs)
        //        {

        //            if (item.semaine == test)
        //            {
        //                TotCA += item.CA;
        //                TotAccord += item.accord;
        //                TotAcw += item.tempsACW;
        //                TotLog += item.tempsLog;
        //                TotPreview += item.tempsPreview;
        //                TotPauseBrief += item.tempsPauseBrief;
        //                TotPausePerso += item.tempsPausePerso;
        //                TotOccupation += item.tempsComm + item.tempsAtt + item.tempsPreview;
        //                TotCommunication += item.tempsComm + item.tempsAtt;
        //                TotAppelEmis += item.appelEmis;
        //                TotProdReel += item.tempsComm + item.tempsAtt;
        //                tempsPresence += item.tempsLog / 3600;
        //                TotJourTravaillés += 1;
        //            }
        //        }

        //        double TauxVentesHebdo = Math.Round((TotAccord / TotCA), 2) * 100;
        //        ViewBag.TauxVentesHebdo = TauxVentesHebdo.ToString();

        //        double TauxACWHebdo = Math.Round((TotAcw / TotLog), 2) * 100;
        //        ViewBag.TauxACWHebdo = TauxACWHebdo.ToString();

        //        double TauxPreviewHebdo = Math.Round((TotPreview / TotLog), 3) * 100;
        //        ViewBag.TauxPreviewHebdo = TauxPreviewHebdo.ToString();

        //        double TauxPauseBriefHebdo = Math.Round((TotPauseBrief / TotLog), 3) * 100;
        //        ViewBag.TauxPauseBriefHebdo = TauxPauseBriefHebdo.ToString();

        //        double TauxPausePersoHebdo = Math.Round((TotPausePerso / TotLog), 3) * 100;
        //        ViewBag.TauxPausePersoHebdo = TauxPausePersoHebdo.ToString();

        //        double TauxOccupation = Math.Round((TotOccupation / TotLog), 3) * 100;
        //        ViewBag.TauxOccupation = TauxOccupation.ToString();

        //        double TauxComunication = Math.Round((TotCommunication / TotLog), 3) * 100;
        //        ViewBag.TauxComunication = TauxComunication.ToString();

        //        double TauxVenteParHeure = Math.Round((TotAccord / (TotLog / 3600)), 3) * 100;
        //        ViewBag.TauxVenteParHeure = TauxVenteParHeure.ToString();

        //        ViewBag.TotalAppelEmis = TotAppelEmis.ToString();
        //        ViewBag.TotalCA = TotCA.ToString();
        //        ViewBag.TotalAccord = TotAccord.ToString();
        //        ViewBag.TotalProdReel = TotProdReel.ToString();
        //        ViewBag.TempsPresenceParHeure = tempsPresence.ToString();
        //        ViewBag.NobreJourTravaillés = TotJourTravaillés.ToString();

        //        double ETPplanifie = (TotLog / 3600) / TotJourTravaillés / 8;
        //        ViewBag.ETPplanifie = ETPplanifie.ToString();

        //        double MoyenneAccord = TotAccord / TotJourTravaillés / ETPplanifie;
        //        ViewBag.MoyenneAccord = MoyenneAccord.ToString();

        //        double MoyenneAppels = TotAppelEmis / TotJourTravaillés / ETPplanifie;
        //        ViewBag.MoyenneAppels = MoyenneAppels.ToString();
        //    }
        //    return View();
      
        //}

        // GET: TestIndicateurs/Create
        //public ActionResult Mensuel()
        //{
        //    var list = new SelectList(new[]
        //                                 {
        //                                      new {Value=1,Text="Janvier"},
        //                                      new{Value=2,Text="Février"},
        //                                      new{Value=3,Text="Mars"},
        //                                  },
        //                   "Value", "Text", 1);
        //    ViewData["list"] = list;
        //    double TotAccord = 0;
        //    double TotCA = 0;
        //    var indicateurs = db.TestIndicateurs.ToList();
        //    foreach (var item in indicateurs)
        //    {
        //        if (item.mois == "Janvier")
        //        {
        //            TotCA += item.CA;
        //            TotAccord += item.accord;
        //        }
        //    }

        //    double TauxVenteMens = Math.Round((TotAccord / TotCA), 2);

        //    ViewBag.TauxVenteMens = TauxVenteMens.ToString();
        //    return View();
        //}


        // GET: TestIndicateurs/Edit/5
        //public ActionResult Annuelle()
        //{

        //    var list = new SelectList(new[]
        //                               {
        //                                      new {Value=1,Text="2018"},
        //                                      new{Value=2,Text="2017"},
        //                                  },
        //                 "Value", "Text", 1);
        //    double TotAccord = 0;
        //    double TotCA = 0;
        //    double TauxVenteAnn = 0;
        //    var indicateurs = db.TestIndicateurs.ToList();
        //    foreach (var item in indicateurs)
        //    {
        //        if (item.date.Year == 2018)
        //        {
        //            TotCA += item.CA;
        //            TotAccord += item.accord;
        //        }
        //    }

        //   TauxVenteAnn = Math.Round((TotAccord / TotCA), 2);

        //    ViewBag.TauxVenteAnn = TauxVenteAnn.ToString();
        //    return View();
        //}

        
        // GET: TestIndicateurs/Delete/5
        //public ActionResult Trimestriel()
        //{
        //    var list = new SelectList(new[]
        //                               {
        //                                      new {Value=1,Text="Trimestre1"},
        //                                      new{Value=2,Text="Trimestre2"},
        //                                      new{Value=3,Text="Trimestre3"},
        //                                      new{Value=3,Text="Trimestre4"},
        //                                  },
        //                 "Value", "Text", 1);
        //    double TotAccord = 0;
        //    double TotCA = 0;
        //    double TauxVenteTri1 = 0;
        //    var indicateurs = db.TestIndicateurs.ToList();
        //    foreach (var item in indicateurs)
        //    {
        //        if (item.date.Month == 1 || item.date.Month == 2 || item.date.Month == 3)
        //        {
        //            TotCA += item.CA;
        //            TotAccord += item.accord;
        //        }
        //    }

        //    TauxVenteTri1 = Math.Round((TotAccord / TotCA), 2);

        //    ViewBag.TauxVenteTri1 = TauxVenteTri1.ToString();
        //    return View();
        //}


        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
