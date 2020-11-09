using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Data;
using MVCWEB.Models;
using Services;
using Domain.Entity;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using System.Globalization;
using System.Data.SqlClient;

namespace MVCWEB.Controllers
{
    [Authorize(Roles = "Manager,SuperManager")]
    public class HermesFunctionalitiesController : Controller
    {
        private ReportContext db = new ReportContext();
 
            private ApplicationSignInManager _signInManager;
            private ApplicationUserManager _userManager;
            private ApplicationRoleManager _roleManager;

            #region constructor and security
            public HermesFunctionalitiesController()
            {}
            public HermesFunctionalitiesController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
            {
                UserManager = userManager;
                SignInManager = signInManager;
                RoleManager = roleManager;
            }
            public ApplicationSignInManager SignInManager
            {
                get
                {
                    return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
                }
                private set
                {
                    _signInManager = value;
                }
            }

            public ApplicationUserManager UserManager
            {
                get
                {
                    return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                }
                private set
                {
                    _userManager = value;
                }
            }
            public ApplicationRoleManager RoleManager
            {
                get
                {
                    return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
                }
                private set
                {
                    _roleManager = value;
                }
            }
            #endregion

            // GET: HermesFunctionalities
            public ActionResult Index()
        {
            Employee empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            {
                String strbase64 = Convert.ToBase64String(empConnected.Content);
                String empConnectedImage = "data:" + empConnected.ContentType + ";base64," + strbase64;
                ViewBag.empConnectedImage = empConnectedImage;
            }
            ViewBag.nameEmpConnected = empConnected.UserName;
            ViewBag.pseudoNameEmpConnected = empConnected.pseudoName;
            return View();
        }
        #region GISI PROMO

        public ActionResult InclusionGisiPromo()
        {
            Employee empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            {
                String strbase64 = Convert.ToBase64String(empConnected.Content);
                String empConnectedImage = "data:" + empConnected.ContentType + ";base64," + strbase64;
                ViewBag.empConnectedImage = empConnectedImage;
            }
            ViewBag.nameEmpConnected = empConnected.UserName;
            ViewBag.pseudoNameEmpConnected = empConnected.pseudoName;
            ViewBag.AllTitres = GetTitres_GISI_PROMO();
            return View();
        }


        private static List<string> GetTitres_GISI_PROMO()
        {
            List<string> AllTitres = new List<string>();
            //Creates new DataTable instance 
            System.Data.DataTable tableTitres = new System.Data.DataTable();
            DataTable tableCodeOpe = new DataTable();
            DataTable tableCodeProv = new DataTable();
            DataTable tableDateInj = new DataTable();
            //Loads the database
            // string connexionString = "Data Source=10.9.7.6;Initial Catalog=ABONNEMENTS_PROD;Integrated Security=False;User ID=sa;Password=V0calc0mETAI;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            string connexionString = "Data Source=10.9.6.3;Initial Catalog=BDD_DIFFUSION;Integrated Security=False;User ID=sa;Password=V0calc0m;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection conn = new SqlConnection(connexionString);
            conn.Open();
            SqlDataAdapter adapter = new SqlDataAdapter("Select distinct TITRE_OPE from RECR_CLIENTS where TITRE_OPE <> '' and TITRE_OPE is not null", conn);
            adapter.Fill(tableTitres);
            adapter.Dispose();

            AllTitres = tableTitres.AsEnumerable().Select(r => r.Field<string>("TITRE_OPE")).ToList();
           // conn.Close();
            return AllTitres;
        }


        public JsonResult GetFiltresGISIPROMO(string SelectedTitre)
        {
            List<SelectListItem> CodesOpeItems = new List<SelectListItem>();
            List<SelectListItem> CodesProvItems = new List<SelectListItem>();
            List<SelectListItem> DatesInjItems = new List<SelectListItem>();
            List<Hermes> AllCodesOpe = new List<Hermes>();
            List<Hermes> AllCodesProv = new List<Hermes>();
            List<Hermes> AllDatesInj = new List<Hermes>();
            DataTable table = new DataTable();
            DataTable table2 = new DataTable();
            DataTable table3 = new DataTable();
            // string connexionString = "Data Source=10.9.7.6;Initial Catalog=ABONNEMENTS_PROD;Integrated Security=False;User ID=sa;Password=V0calc0mETAI;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            string connexionString = "Data Source=10.9.6.3;Initial Catalog=BDD_DIFFUSION;Integrated Security=False;User ID=sa;Password=V0calc0m;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection conn = new SqlConnection(connexionString);
            conn.Open();
            SqlDataAdapter adapter = new SqlDataAdapter("Select distinct CODE_OPE from RECR_CLIENTS where TITRE_OPE = '" + SelectedTitre + "'", conn);
            adapter.Fill(table);
            adapter.Dispose();
            //List of Codes OPE
            // AllCodesOpe = table.AsEnumerable().Select(r => r.Field<string>("CODE_OPE")).ToList();
            AllCodesOpe = (from tr in table.AsEnumerable()
                           select new Hermes()
                           {
                               CODE_OPE = tr.Field<string>("CODE_OPE")
                           }).ToList();
          
            CodesOpeItems.Insert(0, new SelectListItem { Text = "Sélectionner code opération", Value = "" });
            foreach (var item in AllCodesOpe)
            {
                CodesOpeItems.Add(new SelectListItem { Text = item.CODE_OPE, Value = item.CODE_OPE });
            }
            //List of Codes Prov
            SqlDataAdapter adapter2 = new SqlDataAdapter("Select distinct CODE_PROV_RELANCE from RECR_CLIENTS where TITRE_OPE = '" + SelectedTitre + "'", conn);
            adapter2.Fill(table2);
            adapter2.Dispose();

            AllCodesProv = (from tr in table2.AsEnumerable()
                           select new Hermes()
                           {
                               CODE_PROV = tr.Field<string>("CODE_PROV_RELANCE")
                           }).ToList();

            CodesProvItems.Insert(0, new SelectListItem { Text = "Sélectionner code prov", Value = "" });
            foreach (var item in AllCodesProv)
            {
                CodesProvItems.Add(new SelectListItem { Text = item.CODE_PROV, Value = item.CODE_PROV });
            }
            //List of Dates Inj
            SqlDataAdapter adapter3 = new SqlDataAdapter("Select distinct DATE_INJECTION from RECR_CLIENTS where TITRE_OPE = '" + SelectedTitre + "' and DATE_INJECTION is not null", conn);
            adapter3.Fill(table3);
            adapter3.Dispose();

            AllDatesInj = (from tr in table3.AsEnumerable()
                            select new Hermes()
                            {
                                DATE_INJECTION = tr.Field<string>("DATE_INJECTION")
                            }).ToList();

            DatesInjItems.Insert(0, new SelectListItem { Text = "Sélectionner date injection", Value = "" });
            foreach (var item in AllDatesInj)
            {
                DatesInjItems.Add(new SelectListItem { Text = item.DATE_INJECTION, Value = item.DATE_INJECTION });
            }
            conn.Close();
            return Json(new { CodesOpeItems, CodesProvItems, DatesInjItems });
        }

        public ActionResult InclureGISIPROMO(string SelectedTitre, string SelectedStatus, string SelectedCodeOpe, string SelectedCodeProv, string SelectedDateInj)
        {
            string connetionString = null;
            SqlConnection connection;
            SqlDataAdapter adapter = new SqlDataAdapter();
            string sql1 = null;
            string sql2 = null;
            string sql3 = null;
            connetionString = "Data Source=10.9.6.3;Initial Catalog=PROJECT-WEB;Integrated Security=False;User ID=sa;Password=V0calc0m;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            connection = new SqlConnection(connetionString);
            if(SelectedStatus == "Tous")
            {
                if(SelectedCodeOpe == "" && SelectedCodeProv == "" && SelectedDateInj == "")
                {

                   // SqlDataAdapter adapter0 = new SqlDataAdapter("Select distinct CODE_OPE from RECR_CLIENTS where TITRE_OPE = '" + SelectedTitre + "'", conn);
                    sql1 = "update TA set TA.PRIORITE = 1 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -10 and STATUS <> 98";
                    sql2 = "update TA set TA.PRIORITE = 0 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and STATUS <> 98";
                    sql3 = "update TA set TA.PRIORITE = 1 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and STATUS = 98";
                }
                // Code Ope combinations
                if (SelectedCodeOpe != "" && SelectedCodeProv == "" && SelectedDateInj == "")
                {
                    sql1 = "update TA set TA.PRIORITE = 1 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -10 and CODE_OPE = '"+ SelectedCodeOpe+ "' and STATUS <> 98";
                    sql2 = "update TA set TA.PRIORITE = 0 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_OPE = '" + SelectedCodeOpe + "' and STATUS <> 98";
                    sql3 = "update TA set TA.PRIORITE = 1 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_OPE = '" + SelectedCodeOpe + "' and STATUS = 98";
                }
                if (SelectedCodeOpe != "" && SelectedCodeProv != "" && SelectedDateInj == "")
                {
                    sql1 = "update TA set TA.PRIORITE = 1 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -10 and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" +SelectedCodeProv + "' and STATUS <> 98";
                    sql2 = "update TA set TA.PRIORITE = 0 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS <> 98";
                    sql3 = "update TA set TA.PRIORITE = 1 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS = 98";
                }
                if (SelectedCodeOpe != "" && SelectedCodeProv == "" && SelectedDateInj != "")
                {
                    sql1 = "update TA set TA.PRIORITE = 1 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -10 and CODE_OPE = '" + SelectedCodeOpe + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS <> 98";
                    sql2 = "update TA set TA.PRIORITE = 0 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_OPE = '" + SelectedCodeOpe + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS <> 98";
                    sql3 = "update TA set TA.PRIORITE = 1 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_OPE = '" + SelectedCodeOpe + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS = 98";
                }

                // Code prov combinations
                if (SelectedCodeOpe == "" && SelectedCodeProv != "" && SelectedDateInj == "")
                {
                    sql1 = "update TA set TA.PRIORITE = 1 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -10 and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS <> 98";
                    sql2 = "update TA set TA.PRIORITE = 0 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS <> 98";
                    sql3 = "update TA set TA.PRIORITE = 1 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS = 98";
                }
                if (SelectedCodeOpe == "" && SelectedCodeProv != "" && SelectedDateInj != "")
                {
                    sql1 = "update TA set TA.PRIORITE = 1 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -10 and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS <> 98";
                    sql2 = "update TA set TA.PRIORITE = 0 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS <> 98";
                    sql3 = "update TA set TA.PRIORITE = 1 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS = 98";
                }
                //date inj combinations
                if (SelectedCodeOpe == "" && SelectedCodeProv == "" && SelectedDateInj != "")
                {
                    sql1 = "update TA set TA.PRIORITE = 1 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -10 and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS <> 98";
                    sql2 = "update TA set TA.PRIORITE = 0 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS <> 98";
                    sql3 = "update TA set TA.PRIORITE = 1 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS = 98";
                }

                if (SelectedCodeOpe != "" && SelectedCodeProv != "" && SelectedDateInj != "")
                {
                    sql1 = "update TA set TA.PRIORITE = 1 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -10 and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '"+SelectedDateInj+ "' and STATUS <> 98";
                    sql2 = "update TA set TA.PRIORITE = 0 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS <> 98";
                    sql3 = "update TA set TA.PRIORITE = 1 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS = 98";
                }
            }
            if (SelectedStatus == "RR")
            {
                if (SelectedCodeOpe == "" && SelectedCodeProv == "" && SelectedDateInj == "")
                {
                    sql1 = "update TA set TA.PRIORITE = 1 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -10 and STATUS in (94,95)";
                    sql2 = "update TA set TA.PRIORITE = 0 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and STATUS in (94,95)";
                }
                // Code Ope combinations
                if (SelectedCodeOpe != "" && SelectedCodeProv == "" && SelectedDateInj == "")
                {
                    sql1 = "update TA set TA.PRIORITE = 1 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -10 and CODE_OPE = '" + SelectedCodeOpe + "' and STATUS in (94,95)";
                    sql2 = "update TA set TA.PRIORITE = 0 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_OPE = '" + SelectedCodeOpe + "' and STATUS in (94,95)";
                }
                if (SelectedCodeOpe != "" && SelectedCodeProv != "" && SelectedDateInj == "")
                {
                    sql1 = "update TA set TA.PRIORITE = 1 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -10 and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS in (94,95)";
                    sql2 = "update TA set TA.PRIORITE = 0 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS in (94,95)";
                }
                if (SelectedCodeOpe != "" && SelectedCodeProv == "" && SelectedDateInj != "")
                {
                    sql1 = "update TA set TA.PRIORITE = 1 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -10 and CODE_OPE = '" + SelectedCodeOpe + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS in (94,95)";
                    sql2 = "update TA set TA.PRIORITE = 0 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_OPE = '" + SelectedCodeOpe + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS in (94,95)";
                }

                // Code prov combinations
                if (SelectedCodeOpe == "" && SelectedCodeProv != "" && SelectedDateInj == "")
                {
                    sql1 = "update TA set TA.PRIORITE = 1 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -10 and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS in (94,95)";
                    sql2 = "update TA set TA.PRIORITE = 0 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS in (94,95)";
                }
                if (SelectedCodeOpe == "" && SelectedCodeProv != "" && SelectedDateInj != "")
                {
                    sql1 = "update TA set TA.PRIORITE = 1 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -10 and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS in (94,95)";
                    sql2 = "update TA set TA.PRIORITE = 0 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS in (94,95)";
                }
                //date inj combinations
                if (SelectedCodeOpe == "" && SelectedCodeProv == "" && SelectedDateInj != "")
                {
                    sql1 = "update TA set TA.PRIORITE = 1 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -10 and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS in (94,95)";
                    sql2 = "update TA set TA.PRIORITE = 0 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS in (94,95)";
                }

                if (SelectedCodeOpe != "" && SelectedCodeProv != "" && SelectedDateInj != "")
                {
                    sql1 = "update TA set TA.PRIORITE = 1 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -10 and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS in (94,95)";
                    sql2 = "update TA set TA.PRIORITE = 0 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS in (94,95)";
                }
            }
            if (SelectedStatus == "Sauf RR")
            {
                if (SelectedCodeOpe == "" && SelectedCodeProv == "" && SelectedDateInj == "")
                {
                    sql1 = "update TA set TA.PRIORITE = 1 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -10 and STATUS not in (94,95,98)";
                    sql2 = "update TA set TA.PRIORITE = 0 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and STATUS not in (94,95,98)";
                    sql3 = "update TA set TA.PRIORITE = 1 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and STATUS = 98";
                }
                // Code Ope combinations
                if (SelectedCodeOpe != "" && SelectedCodeProv == "" && SelectedDateInj == "")
                {
                    sql1 = "update TA set TA.PRIORITE = 1 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -10 and CODE_OPE = '" + SelectedCodeOpe + "' and STATUS not in (94,95,98)";
                    sql2 = "update TA set TA.PRIORITE = 0 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_OPE = '" + SelectedCodeOpe + "' and STATUS not in (94,95,98)";
                    sql3 = "update TA set TA.PRIORITE = 1 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_OPE = '" + SelectedCodeOpe + "' and STATUS = 98";
                }
                if (SelectedCodeOpe != "" && SelectedCodeProv != "" && SelectedDateInj == "")
                {
                    sql1 = "update TA set TA.PRIORITE = 1 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -10 and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS not in (94,95,98)";
                    sql2 = "update TA set TA.PRIORITE = 0 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS not in (94,95,98)";
                    sql3 = "update TA set TA.PRIORITE = 1 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS = 98";
                }
                if (SelectedCodeOpe != "" && SelectedCodeProv == "" && SelectedDateInj != "")
                {
                    sql1 = "update TA set TA.PRIORITE = 1 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -10 and CODE_OPE = '" + SelectedCodeOpe + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS not in (94,95,98)";
                    sql2 = "update TA set TA.PRIORITE = 0 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_OPE = '" + SelectedCodeOpe + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS not in (94,95,98)";
                    sql3 = "update TA set TA.PRIORITE = 1 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_OPE = '" + SelectedCodeOpe + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS = 98";
                }

                // Code prov combinations
                if (SelectedCodeOpe == "" && SelectedCodeProv != "" && SelectedDateInj == "")
                {
                    sql1 = "update TA set TA.PRIORITE = 1 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -10 and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS not in (94,95,98)";
                    sql2 = "update TA set TA.PRIORITE = 0 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS not in (94,95,98)";
                    sql3 = "update TA set TA.PRIORITE = 1 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS = 98";
                }
                if (SelectedCodeOpe == "" && SelectedCodeProv != "" && SelectedDateInj != "")
                {
                    sql1 = "update TA set TA.PRIORITE = 1 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -10 and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS not in (94,95,98)";
                    sql2 = "update TA set TA.PRIORITE = 0 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS not in (94,95,98)";
                    sql3 = "update TA set TA.PRIORITE = 1 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS = 98";
                }
                //date inj combinations
                if (SelectedCodeOpe == "" && SelectedCodeProv == "" && SelectedDateInj != "")
                {
                    sql1 = "update TA set TA.PRIORITE = 1 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -10 and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS not in (94,95,98)";
                    sql2 = "update TA set TA.PRIORITE = 0 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS not in (94,95,98)";
                    sql3 = "update TA set TA.PRIORITE = 1 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS = 98";
                }

                if (SelectedCodeOpe != "" && SelectedCodeProv != "" && SelectedDateInj != "")
                {
                    sql1 = "update TA set TA.PRIORITE = 1 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -10 and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS not in (94,95,98)";
                    sql2 = "update TA set TA.PRIORITE = 0 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS not in (94,95,98)";
                    sql3 = "update TA set TA.PRIORITE = 1 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS = 98";
                }
            }
            connection.Open();
            try
            {
                adapter.UpdateCommand = connection.CreateCommand();
                adapter.UpdateCommand.CommandText = sql1;
                adapter.UpdateCommand.ExecuteNonQuery();
                adapter.UpdateCommand.CommandText = sql2;
                adapter.UpdateCommand.ExecuteNonQuery();
                if (sql3 != null)
                {
                    adapter.UpdateCommand.CommandText = sql3;
                    adapter.UpdateCommand.ExecuteNonQuery();
                }
        
                ViewBag.res = "Inclusion des fiches Gisi Promo a été effectuée avec Sucèss!!!!";
                return PartialView("Result", ViewBag.res);
            }
            catch
            {
                ViewBag.res = "Echec d'Inclusion des fiches Gisi Promo!!";
                return PartialView("Result", ViewBag.res);
            }       
        }

        public ActionResult ExclusionGisiPromo()
        {
            Employee empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            {
                String strbase64 = Convert.ToBase64String(empConnected.Content);
                String empConnectedImage = "data:" + empConnected.ContentType + ";base64," + strbase64;
                ViewBag.empConnectedImage = empConnectedImage;
            }
            ViewBag.nameEmpConnected = empConnected.UserName;
            ViewBag.pseudoNameEmpConnected = empConnected.pseudoName;
            ViewBag.AllTitres = GetTitres_GISI_PROMO();
            return View();
        }

        public ActionResult ExclureGISIPROMO(string SelectedTitre, string SelectedStatus, string SelectedCodeOpe, string SelectedCodeProv, string SelectedDateInj)
        {
            string connetionString = null;
            SqlConnection connection;
            SqlDataAdapter adapter = new SqlDataAdapter();
            string sql1 = null;
            string sql2 = null;
            string sql3 = null;
            connetionString = "Data Source=10.9.6.3;Initial Catalog=PROJECT-WEB;Integrated Security=False;User ID=sa;Password=V0calc0m;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            connection = new SqlConnection(connetionString);
            if (SelectedStatus == "Tous")
            {
                if (SelectedCodeOpe == "" && SelectedCodeProv == "" && SelectedDateInj == "")
                {
                    sql1 = "update TA set TA.PRIORITE = -10 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 1 and STATUS <> 98";
                    sql2 = "update TA set TA.PRIORITE = -11 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 0 and STATUS <> 98";
                    sql3 = "update TA set TA.PRIORITE = -11 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE in (1,-33) and STATUS = 98";
                }
                // Code Ope combinations
                if (SelectedCodeOpe != "" && SelectedCodeProv == "" && SelectedDateInj == "")
                {
                    sql1 = "update TA set TA.PRIORITE = -10 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 1 and CODE_OPE = '" + SelectedCodeOpe + "' and STATUS <> 98";
                    sql2 = "update TA set TA.PRIORITE = -11 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 0 and CODE_OPE = '" + SelectedCodeOpe + "' and STATUS <> 98";
                    sql3 = "update TA set TA.PRIORITE = -11 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE in (1,-33) and CODE_OPE = '" + SelectedCodeOpe + "' and STATUS = 98";
                }
                if (SelectedCodeOpe != "" && SelectedCodeProv != "" && SelectedDateInj == "")
                {
                    sql1 = "update TA set TA.PRIORITE = -10 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 1 and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS <> 98";
                    sql2 = "update TA set TA.PRIORITE = -11 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 0 and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS <> 98";
                    sql3 = "update TA set TA.PRIORITE = -11 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE in (1,-33) and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS = 98";
                }
                if (SelectedCodeOpe != "" && SelectedCodeProv == "" && SelectedDateInj != "")
                {
                    sql1 = "update TA set TA.PRIORITE = -10 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 1 and CODE_OPE = '" + SelectedCodeOpe + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS <> 98";
                    sql2 = "update TA set TA.PRIORITE = -11 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 0 and CODE_OPE = '" + SelectedCodeOpe + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS <> 98";
                    sql3 = "update TA set TA.PRIORITE = -11 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE in (1,-33) and CODE_OPE = '" + SelectedCodeOpe + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS = 98";
                }

                // Code prov combinations
                if (SelectedCodeOpe == "" && SelectedCodeProv != "" && SelectedDateInj == "")
                {
                    sql1 = "update TA set TA.PRIORITE = -10 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 1 and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS <> 98";
                    sql2 = "update TA set TA.PRIORITE = -11 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 0 and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS <> 98";
                    sql3 = "update TA set TA.PRIORITE = -11 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE in (1,-33) and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS = 98";
                }
                if (SelectedCodeOpe == "" && SelectedCodeProv != "" && SelectedDateInj != "")
                {
                    sql1 = "update TA set TA.PRIORITE = -10 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 1 and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS <> 98";
                    sql2 = "update TA set TA.PRIORITE = -11 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 0 and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS <> 98";
                    sql3 = "update TA set TA.PRIORITE = -11 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE in (1,-33) and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS = 98";
                }
                //date inj combinations
                if (SelectedCodeOpe == "" && SelectedCodeProv == "" && SelectedDateInj != "")
                {
                    sql1 = "update TA set TA.PRIORITE = -10 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 1 and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS <> 98";
                    sql2 = "update TA set TA.PRIORITE = -11 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 0 and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS <> 98";
                    sql3 = "update TA set TA.PRIORITE = -11 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE in (1,-33) and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS = 98";
                }

                if (SelectedCodeOpe != "" && SelectedCodeProv != "" && SelectedDateInj != "")
                {
                    sql1 = "update TA set TA.PRIORITE = -10 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 1 and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS <> 98";
                    sql2 = "update TA set TA.PRIORITE = -11 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 0 and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS <> 98";
                    sql3 = "update TA set TA.PRIORITE = -11 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE in (1,-33) and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS = 98";
                }
            }
            if (SelectedStatus == "RR")
            {
                if (SelectedCodeOpe == "" && SelectedCodeProv == "" && SelectedDateInj == "")
                {
                    sql1 = "update TA set TA.PRIORITE = -10 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 1 and STATUS in (94,95)";
                    sql2 = "update TA set TA.PRIORITE = -11 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 0 and STATUS in (94,95)";
                }
                // Code Ope combinations
                if (SelectedCodeOpe != "" && SelectedCodeProv == "" && SelectedDateInj == "")
                {
                    sql1 = "update TA set TA.PRIORITE = -10 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 1 and CODE_OPE = '" + SelectedCodeOpe + "' and STATUS in (94,95)";
                    sql2 = "update TA set TA.PRIORITE = -11 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 0 and CODE_OPE = '" + SelectedCodeOpe + "' and STATUS in (94,95)";
                }
                if (SelectedCodeOpe != "" && SelectedCodeProv != "" && SelectedDateInj == "")
                {
                    sql1 = "update TA set TA.PRIORITE = -10 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 1 and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS in (94,95)";
                    sql2 = "update TA set TA.PRIORITE = -11 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 0 and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS in (94,95)";
                }
                if (SelectedCodeOpe != "" && SelectedCodeProv == "" && SelectedDateInj != "")
                {
                    sql1 = "update TA set TA.PRIORITE = -10 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 1 and CODE_OPE = '" + SelectedCodeOpe + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS in (94,95)";
                    sql2 = "update TA set TA.PRIORITE = -11 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 0 and CODE_OPE = '" + SelectedCodeOpe + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS in (94,95)";
                }

                // Code prov combinations
                if (SelectedCodeOpe == "" && SelectedCodeProv != "" && SelectedDateInj == "")
                {
                    sql1 = "update TA set TA.PRIORITE = -10 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 1 and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS in (94,95)";
                    sql2 = "update TA set TA.PRIORITE = -11 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 0 and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS in (94,95)";
                }
                if (SelectedCodeOpe == "" && SelectedCodeProv != "" && SelectedDateInj != "")
                {
                    sql1 = "update TA set TA.PRIORITE = -10 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 1 and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS in (94,95)";
                    sql2 = "update TA set TA.PRIORITE = -11 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 0 and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS in (94,95)";
                }
                //date inj combinations
                if (SelectedCodeOpe == "" && SelectedCodeProv == "" && SelectedDateInj != "")
                {
                    sql1 = "update TA set TA.PRIORITE = -10 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 1 and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS in (94,95)";
                    sql2 = "update TA set TA.PRIORITE = -11 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 0 and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS in (94,95)";
                }

                if (SelectedCodeOpe != "" && SelectedCodeProv != "" && SelectedDateInj != "")
                {
                    sql1 = "update TA set TA.PRIORITE = -10 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 1 and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS in (94,95)";
                    sql2 = "update TA set TA.PRIORITE = -11 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 0 and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS in (94,95)";
                }
            }
            if (SelectedStatus == "Sauf RR")
            {
                if (SelectedCodeOpe == "" && SelectedCodeProv == "" && SelectedDateInj == "")
                {
                    sql1 = "update TA set TA.PRIORITE = -10 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 1 and STATUS not in (94,95,98)";
                    sql2 = "update TA set TA.PRIORITE = -11 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 0 and STATUS not in (94,95,98)";
                    sql3 = "update TA set TA.PRIORITE = -11 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE in (1,-33) and STATUS = 98";
                }
                // Code Ope combinations
                if (SelectedCodeOpe != "" && SelectedCodeProv == "" && SelectedDateInj == "")
                {
                    sql1 = "update TA set TA.PRIORITE = -10 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 1 and CODE_OPE = '" + SelectedCodeOpe + "' and STATUS not in (94,95,98)";
                    sql2 = "update TA set TA.PRIORITE = -11 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 0 and CODE_OPE = '" + SelectedCodeOpe + "' and STATUS not in (94,95,98)";
                    sql3 = "update TA set TA.PRIORITE = -11 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE in (1,-33) and CODE_OPE = '" + SelectedCodeOpe + "' and STATUS = 98";
                }
                if (SelectedCodeOpe != "" && SelectedCodeProv != "" && SelectedDateInj == "")
                {
                    sql1 = "update TA set TA.PRIORITE = -10 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 1 and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS not in (94,95,98)";
                    sql2 = "update TA set TA.PRIORITE = -11 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 0 and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS not in (94,95,98)";
                    sql3 = "update TA set TA.PRIORITE = -11 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE in (1,-33) and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS = 98";
                }
                if (SelectedCodeOpe != "" && SelectedCodeProv == "" && SelectedDateInj != "")
                {
                    sql1 = "update TA set TA.PRIORITE = -10 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 1 and CODE_OPE = '" + SelectedCodeOpe + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS not in (94,95,98)";
                    sql2 = "update TA set TA.PRIORITE = -11 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 0 and CODE_OPE = '" + SelectedCodeOpe + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS not in (94,95,98)";
                    sql3 = "update TA set TA.PRIORITE = -11 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE in (1,-33) and CODE_OPE = '" + SelectedCodeOpe + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS = 98";
                }

                // Code prov combinations
                if (SelectedCodeOpe == "" && SelectedCodeProv != "" && SelectedDateInj == "")
                {
                    sql1 = "update TA set TA.PRIORITE = -10 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 1 and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS not in (94,95,98)";
                    sql2 = "update TA set TA.PRIORITE = -11 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 0 and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS not in (94,95,98)";
                    sql3 = "update TA set TA.PRIORITE = -11 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE in (1,-33) and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS = 98";
                }
                if (SelectedCodeOpe == "" && SelectedCodeProv != "" && SelectedDateInj != "")
                {
                    sql1 = "update TA set TA.PRIORITE = -10 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 1 and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS not in (94,95,98)";
                    sql2 = "update TA set TA.PRIORITE = -11 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 0 and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS not in (94,95,98)";
                    sql3 = "update TA set TA.PRIORITE = -11 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE in (1,-33) and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS = 98";
                }
                //date inj combinations
                if (SelectedCodeOpe == "" && SelectedCodeProv == "" && SelectedDateInj != "")
                {
                    sql1 = "update TA set TA.PRIORITE = -10 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 1 and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS not in (94,95,98)";
                    sql2 = "update TA set TA.PRIORITE = -11 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 0 and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS not in (94,95,98)";
                    sql3 = "update TA set TA.PRIORITE = -11 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE in (1,-33) and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS = 98";
                }

                if (SelectedCodeOpe != "" && SelectedCodeProv != "" && SelectedDateInj != "")
                {
                    sql1 = "update TA set TA.PRIORITE = -10 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 1 and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS not in (94,95,98)";
                    sql2 = "update TA set TA.PRIORITE = -11 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 0 and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS not in (94,95,98)";
                    sql3 = "update TA set TA.PRIORITE = -11 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE in (1,-33) and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS = 98";
                }
            }
            connection.Open();
            try
            {
                adapter.UpdateCommand = connection.CreateCommand();
                adapter.UpdateCommand.CommandText = sql1;
                adapter.UpdateCommand.ExecuteNonQuery();
                adapter.UpdateCommand.CommandText = sql2;
                adapter.UpdateCommand.ExecuteNonQuery();
                if (sql3 != null)
                {
                    adapter.UpdateCommand.CommandText = sql3;
                    adapter.UpdateCommand.ExecuteNonQuery();
                }

                ViewBag.res = "Exclusion des fiches Gisi Promo a été effectuée avec Sucèss!!";
                return PartialView("Result", ViewBag.res);
            }
            catch
            {
                ViewBag.res = "Echec d'Exclusion des fiches Gisi Promo!!";
                return PartialView("Result", ViewBag.res);
            }
        }
        public ActionResult RecyclageGisiPromo()
        {
            Employee empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            {
                String strbase64 = Convert.ToBase64String(empConnected.Content);
                String empConnectedImage = "data:" + empConnected.ContentType + ";base64," + strbase64;
                ViewBag.empConnectedImage = empConnectedImage;
            }
            ViewBag.nameEmpConnected = empConnected.UserName;
            ViewBag.pseudoNameEmpConnected = empConnected.pseudoName;
            ViewBag.AllTitres = GetTitres_GISI_PROMO();
            return View();
        }
        public ActionResult RecyclerGISIPROMO(string SelectedTitre, String[] SelectedStatus, string SelectedCodeOpe, string SelectedCodeProv, string SelectedDateInj)
        {
            string connetionString = null;
            SqlConnection connection;
            SqlDataAdapter adapter = new SqlDataAdapter();
            string sql1 = null;

            connetionString = "Data Source=10.9.6.3;Initial Catalog=PROJECT-WEB;Integrated Security=False;User ID=sa;Password=V0calc0m;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            connection = new SqlConnection(connetionString);
            connection.Open();
            foreach (var status in SelectedStatus)
            {
                if (SelectedCodeOpe == "" && SelectedCodeProv == "" && SelectedDateInj == "")
                {
                    if (status != "99")
                    {
                        sql1 = "update TA set TA.PRIORITE = -33, TA.STATUS = 98from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and STATUS = " + status + "";
                    }
                    else
                    {
                        sql1 = "update TA set TA.PRIORITE = -33, TA.STATUS = 98, TA.NIVABS = 0 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and STATUS = " + status + "";
                    }
                }
                // Code Ope combinations
                if (SelectedCodeOpe != "" && SelectedCodeProv == "" && SelectedDateInj == "")
                {
                    if (status != "99")
                    {
                        sql1 = "update TA set TA.PRIORITE = -33, TA.STATUS = 98 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and CODE_OPE = '" + SelectedCodeOpe + "' and STATUS = " + status + "";
                    }
                    else
                    {
                        sql1 = "update TA set TA.PRIORITE = -33 , TA.STATUS = 98, TA.NIVABS = 0 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and CODE_OPE = '" + SelectedCodeOpe + "' and STATUS = " + status + "";
                    }
                }
                if (SelectedCodeOpe != "" && SelectedCodeProv != "" && SelectedDateInj == "")
                {
                    if (status != "99")
                    {
                        sql1 = "update TA set TA.PRIORITE = -33, TA.STATUS = 98 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS = " + status + "";
                    }
                    else
                    {
                        sql1 = "update TA set TA.PRIORITE = -33 , TA.STATUS = 98, TA.NIVABS = 0 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS = " + status + "";
                    }
                }
                if (SelectedCodeOpe != "" && SelectedCodeProv == "" && SelectedDateInj != "")
                {
                    if (status != "99")
                    {
                        sql1 = "update TA set TA.PRIORITE = -33 , TA.STATUS = 98 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and CODE_OPE = '" + SelectedCodeOpe + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS = " + status + "";
                    }
                    else
                    {
                        sql1 = "update TA set TA.PRIORITE = -33 , TA.STATUS = 98, TA.NIVABS = 0 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and CODE_OPE = '" + SelectedCodeOpe + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS = " + status + "";
                    }
                }

                // Code prov combinations
                if (SelectedCodeOpe == "" && SelectedCodeProv != "" && SelectedDateInj == "")
                {
                    if (status != "99")
                    {
                        sql1 = "update TA set TA.PRIORITE = -33 , TA.STATUS = 98 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS = " + status + "";
                    }
                    else
                    {
                        sql1 = "update TA set TA.PRIORITE = -33 , TA.STATUS = 98, TA.NIVABS = 0 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS = " + status + "";
                    }
                }
                if (SelectedCodeOpe == "" && SelectedCodeProv != "" && SelectedDateInj != "")
                {
                    if (status != "99")
                    {
                        sql1 = "update TA set TA.PRIORITE = -33 , TA.STATUS = 98 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS = " + status + "";
                    }
                    else
                    {
                        sql1 = "update TA set TA.PRIORITE = -33 , TA.STATUS = 98, TA.NIVABS = 0 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS = " + status + "";
                    }
                }
                //date inj combinations
                if (SelectedCodeOpe == "" && SelectedCodeProv == "" && SelectedDateInj != "")
                {
                    if (status != "99")
                    {
                        sql1 = "update TA set TA.PRIORITE = -33 , TA.STATUS = 98 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS = " + status + "";
                    }
                    else
                    {
                        sql1 = "update TA set TA.PRIORITE = -33 , TA.STATUS = 98, TA.NIVABS = 0 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS = " + status + "";
                    }
                }

                if (SelectedCodeOpe != "" && SelectedCodeProv != "" && SelectedDateInj != "")
                {
                    if (status != "99")
                    {
                        sql1 = "update TA set TA.PRIORITE = -33 , TA.STATUS = 98 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS = " + status + "";
                    }
                    else
                    {
                        sql1 = "update TA set TA.PRIORITE = -33 , TA.STATUS = 98, TA.NIVABS = 0 from C4_FA_PROMO_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS = " + status + "";
                    }
                }

                try
                {
                    adapter.UpdateCommand = connection.CreateCommand();
                    adapter.UpdateCommand.CommandText = sql1;
                    adapter.UpdateCommand.ExecuteNonQuery();
                    ViewBag.res = "Recyclage des fiches Gisi Promo a été effectuée avec Sucèss!!!!";
                }
                catch
                {
                    ViewBag.res = "Echec du Recyclage des fiches Gisi Promo!!";
                }
            }
            return PartialView("Result", ViewBag.res);
        }
        #endregion

        #region GMT PROMO
        public ActionResult InclusionGmtPromo()
        {
            Employee empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            {
                String strbase64 = Convert.ToBase64String(empConnected.Content);
                String empConnectedImage = "data:" + empConnected.ContentType + ";base64," + strbase64;
                ViewBag.empConnectedImage = empConnectedImage;
            }
            ViewBag.nameEmpConnected = empConnected.UserName;
            ViewBag.pseudoNameEmpConnected = empConnected.pseudoName;
            ViewBag.AllTitres = GetTitres_GMT_PROMO();
            return View();
        }


        private static List<string> GetTitres_GMT_PROMO()
        {
            List<string> AllTitres = new List<string>();
            //Creates new DataTable instance 
            System.Data.DataTable tableTitres = new System.Data.DataTable();
            DataTable tableCodeOpe = new DataTable();
            DataTable tableCodeProv = new DataTable();
            DataTable tableDateInj = new DataTable();
            //Loads the database
            // string connexionString = "Data Source=10.9.7.6;Initial Catalog=ABONNEMENTS_PROD;Integrated Security=False;User ID=sa;Password=V0calc0mETAI;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            string connexionString = "Data Source=10.9.6.3;Initial Catalog=BDD_DIFFUSION;Integrated Security=False;User ID=sa;Password=V0calc0m;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection conn = new SqlConnection(connexionString);
            conn.Open();
            SqlDataAdapter adapter = new SqlDataAdapter("Select distinct TITRE from GMT_RECR_CLIENTS where TITRE <> '' and TITRE is not null", conn);
            adapter.Fill(tableTitres);
            adapter.Dispose();

            AllTitres = tableTitres.AsEnumerable().Select(r => r.Field<string>("TITRE")).ToList();
            // conn.Close();
            return AllTitres;
        }


        public JsonResult GetFiltresGMTPROMO(string SelectedTitre)
        {
            List<SelectListItem> CodesOpeItems = new List<SelectListItem>();
            List<SelectListItem> CodesProvItems = new List<SelectListItem>();
            List<SelectListItem> DatesInjItems = new List<SelectListItem>();
            List<Hermes> AllCodesOpe = new List<Hermes>();
            List<Hermes> AllCodesProv = new List<Hermes>();
            List<Hermes> AllDatesInj = new List<Hermes>();
            DataTable table = new DataTable();
            DataTable table2 = new DataTable();
            DataTable table3 = new DataTable();
            // string connexionString = "Data Source=10.9.7.6;Initial Catalog=ABONNEMENTS_PROD;Integrated Security=False;User ID=sa;Password=V0calc0mETAI;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            string connexionString = "Data Source=10.9.6.3;Initial Catalog=BDD_DIFFUSION;Integrated Security=False;User ID=sa;Password=V0calc0m;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            SqlConnection conn = new SqlConnection(connexionString);
            conn.Open();
            SqlDataAdapter adapter = new SqlDataAdapter("Select distinct CODE_OPE from GMT_RECR_CLIENTS where TITRE = '" + SelectedTitre + "'", conn);
            adapter.Fill(table);
            adapter.Dispose();
            //List of Codes OPE
            // AllCodesOpe = table.AsEnumerable().Select(r => r.Field<string>("CODE_OPE")).ToList();
            AllCodesOpe = (from tr in table.AsEnumerable()
                           select new Hermes()
                           {
                               CODE_OPE = tr.Field<string>("CODE_OPE")
                           }).ToList();

            CodesOpeItems.Insert(0, new SelectListItem { Text = "Sélectionner code opération", Value = "" });
            foreach (var item in AllCodesOpe)
            {
                CodesOpeItems.Add(new SelectListItem { Text = item.CODE_OPE, Value = item.CODE_OPE });
            }
            //List of Codes Prov
            SqlDataAdapter adapter2 = new SqlDataAdapter("Select distinct CODE_PROV from GMT_RECR_CLIENTS where TITRE = '" + SelectedTitre + "'", conn);
            adapter2.Fill(table2);
            adapter2.Dispose();

            AllCodesProv = (from tr in table2.AsEnumerable()
                            select new Hermes()
                            {
                                CODE_PROV = tr.Field<string>("CODE_PROV")
                            }).ToList();

            CodesProvItems.Insert(0, new SelectListItem { Text = "Sélectionner code prov", Value = "" });
            foreach (var item in AllCodesProv)
            {
                CodesProvItems.Add(new SelectListItem { Text = item.CODE_PROV, Value = item.CODE_PROV });
            }
            //List of Dates Inj
            SqlDataAdapter adapter3 = new SqlDataAdapter("Select distinct DATE_INJECTION from GMT_RECR_CLIENTS where TITRE = '" + SelectedTitre + "' and DATE_INJECTION is not null", conn);
            adapter3.Fill(table3);
            adapter3.Dispose();

            AllDatesInj = (from tr in table3.AsEnumerable()
                           select new Hermes()
                           {
                               DATE_INJECTION = tr.Field<string>("DATE_INJECTION")
                           }).ToList();

            DatesInjItems.Insert(0, new SelectListItem { Text = "Sélectionner date injection", Value = "" });
            foreach (var item in AllDatesInj)
            {
                DatesInjItems.Add(new SelectListItem { Text = item.DATE_INJECTION, Value = item.DATE_INJECTION });
            }
            conn.Close();
            return Json(new { CodesOpeItems, CodesProvItems, DatesInjItems });
        }

        public ActionResult InclureGMTPROMO(string SelectedTitre, string SelectedStatus, string SelectedCodeOpe, string SelectedCodeProv, string SelectedDateInj)
        {
            string connetionString = null;
            SqlConnection connection;
            SqlDataAdapter adapter = new SqlDataAdapter();
            string sql1 = null;
            string sql2 = null;
            string sql3 = null;
            connetionString = "Data Source=10.9.6.3;Initial Catalog=PROJECT-WEB;Integrated Security=False;User ID=sa;Password=V0calc0m;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            connection = new SqlConnection(connetionString);
            if (SelectedStatus == "Tous")
            {
                if (SelectedCodeOpe == "" && SelectedCodeProv == "" && SelectedDateInj == "")
                {
                    sql1 = "update TA set TA.PRIORITE = 1 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -10 and STATUS <> 98";
                    sql2 = "update TA set TA.PRIORITE = 0 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and STATUS <> 98";
                    sql3 = "update TA set TA.PRIORITE = 1 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and STATUS = 98";
                }
                // Code Ope combinations
                if (SelectedCodeOpe != "" && SelectedCodeProv == "" && SelectedDateInj == "")
                {
                    sql1 = "update TA set TA.PRIORITE = 1 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -10 and CODE_OPE = '" + SelectedCodeOpe + "' and STATUS <> 98";
                    sql2 = "update TA set TA.PRIORITE = 0 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_OPE = '" + SelectedCodeOpe + "' and STATUS <> 98";
                    sql3 = "update TA set TA.PRIORITE = 1 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_OPE = '" + SelectedCodeOpe + "' and STATUS = 98";
                }
                if (SelectedCodeOpe != "" && SelectedCodeProv != "" && SelectedDateInj == "")
                {
                    sql1 = "update TA set TA.PRIORITE = 1 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -10 and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS <> 98";
                    sql2 = "update TA set TA.PRIORITE = 0 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS <> 98";
                    sql3 = "update TA set TA.PRIORITE = 1 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS = 98";
                }
                if (SelectedCodeOpe != "" && SelectedCodeProv == "" && SelectedDateInj != "")
                {
                    sql1 = "update TA set TA.PRIORITE = 1 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -10 and CODE_OPE = '" + SelectedCodeOpe + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS <> 98";
                    sql2 = "update TA set TA.PRIORITE = 0 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_OPE = '" + SelectedCodeOpe + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS <> 98";
                    sql3 = "update TA set TA.PRIORITE = 1 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_OPE = '" + SelectedCodeOpe + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS = 98";
                }

                // Code prov combinations
                if (SelectedCodeOpe == "" && SelectedCodeProv != "" && SelectedDateInj == "")
                {
                    sql1 = "update TA set TA.PRIORITE = 1 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -10 and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS <> 98";
                    sql2 = "update TA set TA.PRIORITE = 0 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS <> 98";
                    sql3 = "update TA set TA.PRIORITE = 1 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS = 98";
                }
                if (SelectedCodeOpe == "" && SelectedCodeProv != "" && SelectedDateInj != "")
                {
                    sql1 = "update TA set TA.PRIORITE = 1 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -10 and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS <> 98";
                    sql2 = "update TA set TA.PRIORITE = 0 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS <> 98";
                    sql3 = "update TA set TA.PRIORITE = 1 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS = 98";
                }
                //date inj combinations
                if (SelectedCodeOpe == "" && SelectedCodeProv == "" && SelectedDateInj != "")
                {
                    sql1 = "update TA set TA.PRIORITE = 1 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -10 and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS <> 98";
                    sql2 = "update TA set TA.PRIORITE = 0 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS <> 98";
                    sql3 = "update TA set TA.PRIORITE = 1 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS = 98";
                }

                if (SelectedCodeOpe != "" && SelectedCodeProv != "" && SelectedDateInj != "")
                {
                    sql1 = "update TA set TA.PRIORITE = 1 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -10 and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS <> 98";
                    sql2 = "update TA set TA.PRIORITE = 0 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS <> 98";
                    sql3 = "update TA set TA.PRIORITE = 1 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS = 98";
                }
            }
            if (SelectedStatus == "RR")
            {
                if (SelectedCodeOpe == "" && SelectedCodeProv == "" && SelectedDateInj == "")
                {
                    sql1 = "update TA set TA.PRIORITE = 1 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -10 and STATUS in (94,95)";
                    sql2 = "update TA set TA.PRIORITE = 0 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and STATUS in (94,95)";
                }
                // Code Ope combinations
                if (SelectedCodeOpe != "" && SelectedCodeProv == "" && SelectedDateInj == "")
                {
                    sql1 = "update TA set TA.PRIORITE = 1 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -10 and CODE_OPE = '" + SelectedCodeOpe + "' and STATUS in (94,95)";
                    sql2 = "update TA set TA.PRIORITE = 0 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_OPE = '" + SelectedCodeOpe + "' and STATUS in (94,95)";
                }
                if (SelectedCodeOpe != "" && SelectedCodeProv != "" && SelectedDateInj == "")
                {
                    sql1 = "update TA set TA.PRIORITE = 1 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -10 and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS in (94,95)";
                    sql2 = "update TA set TA.PRIORITE = 0 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS in (94,95)";
                }
                if (SelectedCodeOpe != "" && SelectedCodeProv == "" && SelectedDateInj != "")
                {
                    sql1 = "update TA set TA.PRIORITE = 1 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -10 and CODE_OPE = '" + SelectedCodeOpe + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS in (94,95)";
                    sql2 = "update TA set TA.PRIORITE = 0 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_OPE = '" + SelectedCodeOpe + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS in (94,95)";
                }

                // Code prov combinations
                if (SelectedCodeOpe == "" && SelectedCodeProv != "" && SelectedDateInj == "")
                {
                    sql1 = "update TA set TA.PRIORITE = 1 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -10 and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS in (94,95)";
                    sql2 = "update TA set TA.PRIORITE = 0 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS in (94,95)";
                }
                if (SelectedCodeOpe == "" && SelectedCodeProv != "" && SelectedDateInj != "")
                {
                    sql1 = "update TA set TA.PRIORITE = 1 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -10 and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS in (94,95)";
                    sql2 = "update TA set TA.PRIORITE = 0 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS in (94,95)";
                }
                //date inj combinations
                if (SelectedCodeOpe == "" && SelectedCodeProv == "" && SelectedDateInj != "")
                {
                    sql1 = "update TA set TA.PRIORITE = 1 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -10 and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS in (94,95)";
                    sql2 = "update TA set TA.PRIORITE = 0 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS in (94,95)";
                }

                if (SelectedCodeOpe != "" && SelectedCodeProv != "" && SelectedDateInj != "")
                {
                    sql1 = "update TA set TA.PRIORITE = 1 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -10 and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS in (94,95)";
                    sql2 = "update TA set TA.PRIORITE = 0 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS in (94,95)";
                }
            }
            if (SelectedStatus == "Sauf RR")
            {
                if (SelectedCodeOpe == "" && SelectedCodeProv == "" && SelectedDateInj == "")
                {
                    sql1 = "update TA set TA.PRIORITE = 1 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -10 and STATUS not in (94,95,98)";
                    sql2 = "update TA set TA.PRIORITE = 0 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and STATUS not in (94,95,98)";
                    sql3 = "update TA set TA.PRIORITE = 1 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and STATUS = 98";
                }
                // Code Ope combinations
                if (SelectedCodeOpe != "" && SelectedCodeProv == "" && SelectedDateInj == "")
                {
                    sql1 = "update TA set TA.PRIORITE = 1 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -10 and CODE_OPE = '" + SelectedCodeOpe + "' and STATUS not in (94,95,98)";
                    sql2 = "update TA set TA.PRIORITE = 0 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_OPE = '" + SelectedCodeOpe + "' and STATUS not in (94,95,98)";
                    sql3 = "update TA set TA.PRIORITE = 1 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_OPE = '" + SelectedCodeOpe + "' and STATUS = 98";
                }
                if (SelectedCodeOpe != "" && SelectedCodeProv != "" && SelectedDateInj == "")
                {
                    sql1 = "update TA set TA.PRIORITE = 1 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -10 and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS not in (94,95,98)";
                    sql2 = "update TA set TA.PRIORITE = 0 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS not in (94,95,98)";
                    sql3 = "update TA set TA.PRIORITE = 1 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS = 98";
                }
                if (SelectedCodeOpe != "" && SelectedCodeProv == "" && SelectedDateInj != "")
                {
                    sql1 = "update TA set TA.PRIORITE = 1 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -10 and CODE_OPE = '" + SelectedCodeOpe + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS not in (94,95,98)";
                    sql2 = "update TA set TA.PRIORITE = 0 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_OPE = '" + SelectedCodeOpe + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS not in (94,95,98)";
                    sql3 = "update TA set TA.PRIORITE = 1 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_OPE = '" + SelectedCodeOpe + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS = 98";
                }

                // Code prov combinations
                if (SelectedCodeOpe == "" && SelectedCodeProv != "" && SelectedDateInj == "")
                {
                    sql1 = "update TA set TA.PRIORITE = 1 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -10 and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS not in (94,95,98)";
                    sql2 = "update TA set TA.PRIORITE = 0 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS not in (94,95,98)";
                    sql3 = "update TA set TA.PRIORITE = 1 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS = 98";
                }
                if (SelectedCodeOpe == "" && SelectedCodeProv != "" && SelectedDateInj != "")
                {
                    sql1 = "update TA set TA.PRIORITE = 1 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -10 and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS not in (94,95,98)";
                    sql2 = "update TA set TA.PRIORITE = 0 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS not in (94,95,98)";
                    sql3 = "update TA set TA.PRIORITE = 1 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS = 98";
                }
                //date inj combinations
                if (SelectedCodeOpe == "" && SelectedCodeProv == "" && SelectedDateInj != "")
                {
                    sql1 = "update TA set TA.PRIORITE = 1 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -10 and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS not in (94,95,98)";
                    sql2 = "update TA set TA.PRIORITE = 0 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS not in (94,95,98)";
                    sql3 = "update TA set TA.PRIORITE = 1 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS = 98";
                }

                if (SelectedCodeOpe != "" && SelectedCodeProv != "" && SelectedDateInj != "")
                {
                    sql1 = "update TA set TA.PRIORITE = 1 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -10 and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS not in (94,95,98)";
                    sql2 = "update TA set TA.PRIORITE = 0 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS not in (94,95,98)";
                    sql3 = "update TA set TA.PRIORITE = 1 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = -11 and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS = 98";
                }
            }
            connection.Open();
            try
            {
                adapter.UpdateCommand = connection.CreateCommand();
                adapter.UpdateCommand.CommandText = sql1;
                adapter.UpdateCommand.ExecuteNonQuery();
                adapter.UpdateCommand.CommandText = sql2;
                adapter.UpdateCommand.ExecuteNonQuery();
                if (sql3 != null)
                {
                    adapter.UpdateCommand.CommandText = sql3;
                    adapter.UpdateCommand.ExecuteNonQuery();
                }
                ViewBag.res = "Inclusion des fiches Gmt Promo a été effectuée avec Sucèss!!!!";
                return PartialView("Result", ViewBag.res);
            }
            catch
            {
                ViewBag.res = "Echec d'Inclusion des fiches Gmt Promo!!";
                return PartialView("Result", ViewBag.res);
            }
        }

        public ActionResult ExclusionGmtPromo()
        {
            Employee empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            {
                String strbase64 = Convert.ToBase64String(empConnected.Content);
                String empConnectedImage = "data:" + empConnected.ContentType + ";base64," + strbase64;
                ViewBag.empConnectedImage = empConnectedImage;
            }
            ViewBag.nameEmpConnected = empConnected.UserName;
            ViewBag.pseudoNameEmpConnected = empConnected.pseudoName;
            ViewBag.AllTitres = GetTitres_GMT_PROMO();
            return View();
        }

        public ActionResult ExclureGMTPROMO(string SelectedTitre, string SelectedStatus, string SelectedCodeOpe, string SelectedCodeProv, string SelectedDateInj)
        {
            string connetionString = null;
            SqlConnection connection;
            SqlDataAdapter adapter = new SqlDataAdapter();
            string sql1 = null;
            string sql2 = null;
            string sql3 = null;
            connetionString = "Data Source=10.9.6.3;Initial Catalog=PROJECT-WEB;Integrated Security=False;User ID=sa;Password=V0calc0m;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            connection = new SqlConnection(connetionString);
            if (SelectedStatus == "Tous")
            {
                if (SelectedCodeOpe == "" && SelectedCodeProv == "" && SelectedDateInj == "")
                {
                    sql1 = "update TA set TA.PRIORITE = -10 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 1 and STATUS <> 98";
                    sql2 = "update TA set TA.PRIORITE = -11 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 0 and STATUS <> 98";
                    sql3 = "update TA set TA.PRIORITE = -11 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE in (1,-33) and STATUS = 98";
                }
                // Code Ope combinations
                if (SelectedCodeOpe != "" && SelectedCodeProv == "" && SelectedDateInj == "")
                {
                    sql1 = "update TA set TA.PRIORITE = -10 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 1 and CODE_OPE = '" + SelectedCodeOpe + "' and STATUS <> 98";
                    sql2 = "update TA set TA.PRIORITE = -11 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 0 and CODE_OPE = '" + SelectedCodeOpe + "' and STATUS <> 98";
                    sql3 = "update TA set TA.PRIORITE = -11 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE in (1,-33) and CODE_OPE = '" + SelectedCodeOpe + "' and STATUS = 98";
                }
                if (SelectedCodeOpe != "" && SelectedCodeProv != "" && SelectedDateInj == "")
                {
                    sql1 = "update TA set TA.PRIORITE = -10 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 1 and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS <> 98";
                    sql2 = "update TA set TA.PRIORITE = -11 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 0 and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS <> 98";
                    sql3 = "update TA set TA.PRIORITE = -11 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE in (1,-33) and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS = 98";
                }
                if (SelectedCodeOpe != "" && SelectedCodeProv == "" && SelectedDateInj != "")
                {
                    sql1 = "update TA set TA.PRIORITE = -10 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 1 and CODE_OPE = '" + SelectedCodeOpe + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS <> 98";
                    sql2 = "update TA set TA.PRIORITE = -11 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 0 and CODE_OPE = '" + SelectedCodeOpe + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS <> 98";
                    sql3 = "update TA set TA.PRIORITE = -11 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE in (1,-33) and CODE_OPE = '" + SelectedCodeOpe + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS = 98";
                }

                // Code prov combinations
                if (SelectedCodeOpe == "" && SelectedCodeProv != "" && SelectedDateInj == "")
                {
                    sql1 = "update TA set TA.PRIORITE = -10 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 1 and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS <> 98";
                    sql2 = "update TA set TA.PRIORITE = -11 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 0 and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS <> 98";
                    sql3 = "update TA set TA.PRIORITE = -11 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE in (1,-33) and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS = 98";
                }
                if (SelectedCodeOpe == "" && SelectedCodeProv != "" && SelectedDateInj != "")
                {
                    sql1 = "update TA set TA.PRIORITE = -10 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 1 and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS <> 98";
                    sql2 = "update TA set TA.PRIORITE = -11 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 0 and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS <> 98";
                    sql3 = "update TA set TA.PRIORITE = -11 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE in (1,-33) and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS = 98";
                }
                //date inj combinations
                if (SelectedCodeOpe == "" && SelectedCodeProv == "" && SelectedDateInj != "")
                {
                    sql1 = "update TA set TA.PRIORITE = -10 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 1 and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS <> 98";
                    sql2 = "update TA set TA.PRIORITE = -11 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 0 and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS <> 98";
                    sql3 = "update TA set TA.PRIORITE = -11 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE in (1,-33) and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS = 98";
                }

                if (SelectedCodeOpe != "" && SelectedCodeProv != "" && SelectedDateInj != "")
                {
                    sql1 = "update TA set TA.PRIORITE = -10 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 1 and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS <> 98";
                    sql2 = "update TA set TA.PRIORITE = -11 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 0 and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS <> 98";
                    sql3 = "update TA set TA.PRIORITE = -11 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE in (1,-33) and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS = 98";
                }
            }
            if (SelectedStatus == "RR")
            {
                if (SelectedCodeOpe == "" && SelectedCodeProv == "" && SelectedDateInj == "")
                {
                    sql1 = "update TA set TA.PRIORITE = -10 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 1 and STATUS in (94,95)";
                    sql2 = "update TA set TA.PRIORITE = -11 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 0 and STATUS in (94,95)";
                }
                // Code Ope combinations
                if (SelectedCodeOpe != "" && SelectedCodeProv == "" && SelectedDateInj == "")
                {
                    sql1 = "update TA set TA.PRIORITE = -10 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 1 and CODE_OPE = '" + SelectedCodeOpe + "' and STATUS in (94,95)";
                    sql2 = "update TA set TA.PRIORITE = -11 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 0 and CODE_OPE = '" + SelectedCodeOpe + "' and STATUS in (94,95)";
                }
                if (SelectedCodeOpe != "" && SelectedCodeProv != "" && SelectedDateInj == "")
                {
                    sql1 = "update TA set TA.PRIORITE = -10 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 1 and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS in (94,95)";
                    sql2 = "update TA set TA.PRIORITE = -11 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 0 and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS in (94,95)";
                }
                if (SelectedCodeOpe != "" && SelectedCodeProv == "" && SelectedDateInj != "")
                {
                    sql1 = "update TA set TA.PRIORITE = -10 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 1 and CODE_OPE = '" + SelectedCodeOpe + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS in (94,95)";
                    sql2 = "update TA set TA.PRIORITE = -11 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 0 and CODE_OPE = '" + SelectedCodeOpe + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS in (94,95)";
                }

                // Code prov combinations
                if (SelectedCodeOpe == "" && SelectedCodeProv != "" && SelectedDateInj == "")
                {
                    sql1 = "update TA set TA.PRIORITE = -10 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 1 and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS in (94,95)";
                    sql2 = "update TA set TA.PRIORITE = -11 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 0 and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS in (94,95)";
                }
                if (SelectedCodeOpe == "" && SelectedCodeProv != "" && SelectedDateInj != "")
                {
                    sql1 = "update TA set TA.PRIORITE = -10 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 1 and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS in (94,95)";
                    sql2 = "update TA set TA.PRIORITE = -11 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 0 and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS in (94,95)";
                }
                //date inj combinations
                if (SelectedCodeOpe == "" && SelectedCodeProv == "" && SelectedDateInj != "")
                {
                    sql1 = "update TA set TA.PRIORITE = -10 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 1 and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS in (94,95)";
                    sql2 = "update TA set TA.PRIORITE = -11 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 0 and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS in (94,95)";
                }

                if (SelectedCodeOpe != "" && SelectedCodeProv != "" && SelectedDateInj != "")
                {
                    sql1 = "update TA set TA.PRIORITE = -10 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 1 and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS in (94,95)";
                    sql2 = "update TA set TA.PRIORITE = -11 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 0 and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS in (94,95)";
                }
            }
            if (SelectedStatus == "Sauf RR")
            {
                if (SelectedCodeOpe == "" && SelectedCodeProv == "" && SelectedDateInj == "")
                {
                    sql1 = "update TA set TA.PRIORITE = -10 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 1 and STATUS not in (94,95,98)";
                    sql2 = "update TA set TA.PRIORITE = -11 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 0 and STATUS not in (94,95,98)";
                    sql3 = "update TA set TA.PRIORITE = -11 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE in (1,-33) and STATUS = 98";
                }
                // Code Ope combinations
                if (SelectedCodeOpe != "" && SelectedCodeProv == "" && SelectedDateInj == "")
                {
                    sql1 = "update TA set TA.PRIORITE = -10 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 1 and CODE_OPE = '" + SelectedCodeOpe + "' and STATUS not in (94,95,98)";
                    sql2 = "update TA set TA.PRIORITE = -11 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 0 and CODE_OPE = '" + SelectedCodeOpe + "' and STATUS not in (94,95,98)";
                    sql3 = "update TA set TA.PRIORITE = -11 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE in (1,-33) and CODE_OPE = '" + SelectedCodeOpe + "' and STATUS = 98";
                }
                if (SelectedCodeOpe != "" && SelectedCodeProv != "" && SelectedDateInj == "")
                {
                    sql1 = "update TA set TA.PRIORITE = -10 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 1 and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS not in (94,95,98)";
                    sql2 = "update TA set TA.PRIORITE = -11 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 0 and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS not in (94,95,98)";
                    sql3 = "update TA set TA.PRIORITE = -11 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE in (1,-33) and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS = 98";
                }
                if (SelectedCodeOpe != "" && SelectedCodeProv == "" && SelectedDateInj != "")
                {
                    sql1 = "update TA set TA.PRIORITE = -10 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 1 and CODE_OPE = '" + SelectedCodeOpe + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS not in (94,95,98)";
                    sql2 = "update TA set TA.PRIORITE = -11 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 0 and CODE_OPE = '" + SelectedCodeOpe + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS not in (94,95,98)";
                    sql3 = "update TA set TA.PRIORITE = -11 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE in (1,-33) and CODE_OPE = '" + SelectedCodeOpe + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS = 98";
                }

                // Code prov combinations
                if (SelectedCodeOpe == "" && SelectedCodeProv != "" && SelectedDateInj == "")
                {
                    sql1 = "update TA set TA.PRIORITE = -10 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 1 and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS not in (94,95,98)";
                    sql2 = "update TA set TA.PRIORITE = -11 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 0 and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS not in (94,95,98)";
                    sql3 = "update TA set TA.PRIORITE = -11 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE in (1,-33) and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS = 98";
                }
                if (SelectedCodeOpe == "" && SelectedCodeProv != "" && SelectedDateInj != "")
                {
                    sql1 = "update TA set TA.PRIORITE = -10 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 1 and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS not in (94,95,98)";
                    sql2 = "update TA set TA.PRIORITE = -11 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 0 and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS not in (94,95,98)";
                    sql3 = "update TA set TA.PRIORITE = -11 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE in (1,-33) and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS = 98";
                }
                //date inj combinations
                if (SelectedCodeOpe == "" && SelectedCodeProv == "" && SelectedDateInj != "")
                {
                    sql1 = "update TA set TA.PRIORITE = -10 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 1 and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS not in (94,95,98)";
                    sql2 = "update TA set TA.PRIORITE = -11 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 0 and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS not in (94,95,98)";
                    sql3 = "update TA set TA.PRIORITE = -11 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE in (1,-33) and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS = 98";
                }

                if (SelectedCodeOpe != "" && SelectedCodeProv != "" && SelectedDateInj != "")
                {
                    sql1 = "update TA set TA.PRIORITE = -10 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 1 and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS not in (94,95,98)";
                    sql2 = "update TA set TA.PRIORITE = -11 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE = 0 and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS not in (94,95,98)";
                    sql3 = "update TA set TA.PRIORITE = -11 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and PRIORITE in (1,-33) and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS = 98";
                }
            }
            connection.Open();
            try
            {
                adapter.UpdateCommand = connection.CreateCommand();
                adapter.UpdateCommand.CommandText = sql1;
                adapter.UpdateCommand.ExecuteNonQuery();
                adapter.UpdateCommand.CommandText = sql2;
                adapter.UpdateCommand.ExecuteNonQuery();
                if (sql3 != null)
                {
                    adapter.UpdateCommand.CommandText = sql3;
                    adapter.UpdateCommand.ExecuteNonQuery();
                }
                ViewBag.res = "Exclusion des fiches Gmt Promo a été effectuée avec Sucèss!!!!";
                return PartialView("Result", ViewBag.res);
            }
            catch
            {
                ViewBag.res = "Echec d'Exclusion des fiches Gmt Promo!!";
                return PartialView("Result", ViewBag.res);
            }
        }

        public ActionResult RecyclageGmtPromo()
        {
            Employee empConnected = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            {
                String strbase64 = Convert.ToBase64String(empConnected.Content);
                String empConnectedImage = "data:" + empConnected.ContentType + ";base64," + strbase64;
                ViewBag.empConnectedImage = empConnectedImage;
            }
            ViewBag.nameEmpConnected = empConnected.UserName;
            ViewBag.pseudoNameEmpConnected = empConnected.pseudoName;
            ViewBag.AllTitres = GetTitres_GMT_PROMO();
            return View();
        }

        public ActionResult RecyclerGMTPROMO(string SelectedTitre, String[] SelectedStatus, string SelectedCodeOpe, string SelectedCodeProv, string SelectedDateInj)
        {
            string connetionString = null;
            SqlConnection connection;
            SqlDataAdapter adapter = new SqlDataAdapter();
            string sql1 = null;
      
            connetionString = "Data Source=10.9.6.3;Initial Catalog=PROJECT-WEB;Integrated Security=False;User ID=sa;Password=V0calc0m;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            connection = new SqlConnection(connetionString);
            connection.Open();
            foreach (var status in SelectedStatus)
            {
                if (SelectedCodeOpe == "" && SelectedCodeProv == "" && SelectedDateInj == "")
                {
                    if (status != "99")
                    {
                        sql1 = "update TA set TA.PRIORITE = -33, TA.STATUS = 98from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and STATUS = " + status + "";
                    }
                    else
                    {
                        sql1 = "update TA set TA.PRIORITE = -33, TA.STATUS = 98, TA.NIVABS = 0 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and STATUS = " + status + "";
                    }
                }
                // Code Ope combinations
                if (SelectedCodeOpe != "" && SelectedCodeProv == "" && SelectedDateInj == "")
                {
                    if (status != "99")
                    {
                        sql1 = "update TA set TA.PRIORITE = -33, TA.STATUS = 98 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and CODE_OPE = '" + SelectedCodeOpe + "' and STATUS = " + status + "";
                    }
                    else
                    {
                        sql1 = "update TA set TA.PRIORITE = -33 , TA.STATUS = 98, TA.NIVABS = 0 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and CODE_OPE = '" + SelectedCodeOpe + "' and STATUS = " + status + "";
                    }
                }
                if (SelectedCodeOpe != "" && SelectedCodeProv != "" && SelectedDateInj == "")
                {
                    if (status != "99")
                    {
                        sql1 = "update TA set TA.PRIORITE = -33, TA.STATUS = 98 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS = " + status + "";
                    }
                    else
                    {
                        sql1 = "update TA set TA.PRIORITE = -33 , TA.STATUS = 98, TA.NIVABS = 0 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS = " + status + "";
                    }
                }
                if (SelectedCodeOpe != "" && SelectedCodeProv == "" && SelectedDateInj != "")
                {
                    if (status != "99")
                    {
                        sql1 = "update TA set TA.PRIORITE = -33 , TA.STATUS = 98 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and CODE_OPE = '" + SelectedCodeOpe + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS = " + status + "";
                    }
                    else
                    {
                        sql1 = "update TA set TA.PRIORITE = -33 , TA.STATUS = 98, TA.NIVABS = 0 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and CODE_OPE = '" + SelectedCodeOpe + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS = " + status + "";
                    }
                }

                // Code prov combinations
                if (SelectedCodeOpe == "" && SelectedCodeProv != "" && SelectedDateInj == "")
                {
                    if (status != "99")
                    {
                        sql1 = "update TA set TA.PRIORITE = -33 , TA.STATUS = 98 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS = " + status + "";
                    }
                    else
                    {
                        sql1 = "update TA set TA.PRIORITE = -33 , TA.STATUS = 98, TA.NIVABS = 0 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and STATUS = " + status + "";
                    }
                }
                if (SelectedCodeOpe == "" && SelectedCodeProv != "" && SelectedDateInj != "")
                {
                    if (status != "99")
                    {
                        sql1 = "update TA set TA.PRIORITE = -33 , TA.STATUS = 98 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS = " + status + "";
                    }
                    else
                    {
                        sql1 = "update TA set TA.PRIORITE = -33 , TA.STATUS = 98, TA.NIVABS = 0 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS = " + status + "";
                    }
                }
                //date inj combinations
                if (SelectedCodeOpe == "" && SelectedCodeProv == "" && SelectedDateInj != "")
                {
                    if (status != "99")
                    {
                        sql1 = "update TA set TA.PRIORITE = -33 , TA.STATUS = 98 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS = " + status + "";
                    }
                    else
                    {
                        sql1 = "update TA set TA.PRIORITE = -33 , TA.STATUS = 98, TA.NIVABS = 0 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS = " + status + "";
                    }
                }

                if (SelectedCodeOpe != "" && SelectedCodeProv != "" && SelectedDateInj != "")
                {
                    if (status != "99")
                    {
                        sql1 = "update TA set TA.PRIORITE = -33 , TA.STATUS = 98 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS = " + status + "";
                    }
                    else
                    {
                        sql1 = "update TA set TA.PRIORITE = -33 , TA.STATUS = 98, TA.NIVABS = 0 from C4_FA_GMT_RECR_" + SelectedTitre + " as TA inner join Table_Client as TC on TA.INDICE = TC.INDICE where TITRE_OPE ='" + SelectedTitre + "' and CODE_OPE = '" + SelectedCodeOpe + "' and CODE_PROV_RELANCE = '" + SelectedCodeProv + "' and DATE_INJECTION = '" + SelectedDateInj + "' and STATUS = " + status + "";
                    }
                }

                try
                {
                    adapter.UpdateCommand = connection.CreateCommand();
                    adapter.UpdateCommand.CommandText = sql1;
                    adapter.UpdateCommand.ExecuteNonQuery();
                    ViewBag.res = "Recyclage des fiches Gmt Promo a été effectuée avec Sucèss!!!!";
                }
                catch
                {
                    ViewBag.res = "Echec du Recyclage des fiches Gmt Promo!!";

                }
            }
            return PartialView("Result", ViewBag.res);
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
