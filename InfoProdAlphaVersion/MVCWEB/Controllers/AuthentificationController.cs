using MVCWEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.DirectoryServices;
using Domain.Entity;
using Services;
using System.Data;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNetCore.Http.Authentication;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using Data;

namespace MVCWEB.Controllers
{
    public class AuthentificationController : Controller
    {
        // GET: Authentification
        IUtilisateurService service;
        IEmployeeService serviceEmployee;
        IGroupeEmployeeService serviceGroupeEmp;
        IAlerteService serviceAlerte;
        public static String loginIndex;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;


        public AuthentificationController()
        {

            service = new UtilisateurService();
            serviceEmployee = new EmployeeService();
            serviceGroupeEmp = new GroupesEmployeService();
            serviceAlerte = new AlerteService();
            //context = new ReportContext();
        }
        public AuthentificationController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
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

        [HttpGet]
        public ActionResult Index(string returnUrl)
        {

            Session.Clear();
            return View();
        }

        // GET: Authentification/Details/5
        [AllowAnonymous]
        public async Task<ActionResult> Connect(UserAuthentif userAuthe)
        {
            string login = userAuthe.login;
            string password = userAuthe.Password;
            if (password == null)
            {
                ViewBag.message = ("(*)Champ mot de passe obligatoire!");
                ViewBag.color = "red";
                return View("~/Views/Authentification/Index.cshtml");
            }
            else
            {
                try
                {
                    DirectoryEntry Ldap = new DirectoryEntry("LDAP://info.local", login, password);
                    DirectorySearcher searcher = new DirectorySearcher(Ldap);
                    searcher.Filter = "(&(objectClass=user)(sAMAccountName=" + login + "))";
                    SearchResult resultAD = searcher.FindOne();
                    DirectoryEntry DirEntry = resultAD.GetDirectoryEntry();
                }
                catch (Exception) //mauvaise catch il faut spécifier le type d'exception pour etre précis
                {
                    ViewBag.message = ("login ou mot de passe incorrect!");
                    ViewBag.color = "red";
                    return View("~/Views/Authentification/Index.cshtml");
                }

                if (!ModelState.IsValid)
                {
                    return View(userAuthe);
                }

                Employee emp = serviceEmployee.getByLoginUser(login);
                var groupesassociees = serviceGroupeEmp.getGroupeByIDEmployee(emp.Id);
                var groupesassociees_tests = groupesassociees.Select(o => o.nom).Distinct().ToList();
                
                List<string> GroupesTest = new List<string>();
                try
                {
                    await SignInManager.SignInAsync(emp, true, true);
                    Utilisateur util = new Utilisateur();
                    util.login = login;
                    util.logEntree = DateTime.Now;
                    service.Add(util);
                    service.SaveChange();
                }
                catch (NullReferenceException e)
                {
                    Console.WriteLine(e);
                    return RedirectToAction("Connect", "Authentification");

                }
                
                if (emp.Content != null)
                {
                    String strbase64 = Convert.ToBase64String(emp.Content);
                    String Url = "data:" + emp.ContentType + ";base64," + strbase64;
                    ViewBag.url = Url;
                    //Session["imageEmp"] = Url;

                }

                if (UserManager.IsInRole(emp.Id, "Admin"))
                {

                    return RedirectToAction("Index", "Home");

                }
                else if (UserManager.IsInRole(emp.Id, "Manager"))
                {
                   
                   return RedirectToAction("IndexManagerGroupes", "Home");

                }
                else if (UserManager.IsInRole(emp.Id, "IT"))
                {
                    return RedirectToAction("Index", "Inventaire");
                }
                else if (UserManager.IsInRole(emp.Id, "RH"))
                {
                    return RedirectToAction("Index", "Inventaire");
                }
                else if (UserManager.IsInRole(emp.Id, "SuperManager"))
                {
                   // Session["SuperMangerRole"] = "SuperMangerRole";
                    return RedirectToAction("IndexManagerGroupes", "Home");

                }
                //Agent: Agent_BPP
                else if (UserManager.IsInRole(emp.Id, "Agent"))
                {
                   return RedirectToAction("HistoriqueAgent", "EvaluationVecteurPlus");
                }
                else if (UserManager.IsInRole(emp.Id, "Agent_QR"))
                {
                    return RedirectToAction("HistoriqueAgent_QR", "EvaluationVecteurPlus");
                }
                else if (UserManager.IsInRole(emp.Id, "Agent_KLMO"))
                {
                    return RedirectToAction("HistoriqueAgent_KLMO", "EvaluationVecteurPlus");
                }
                else if (UserManager.IsInRole(emp.Id, "Agent_PRV"))
                {
                    return RedirectToAction("HistoriqueAgent_PRV", "EvaluationVecteurPlus");
                }
                else if (UserManager.IsInRole(emp.Id, "Agent_CustomerService"))
                {
                    return RedirectToAction("HistoriqueAgent", "EvaluationServiceClientAuto");
                }
                else if (UserManager.IsInRole(emp.Id, "Agent_SAMRC"))
                {
                    return RedirectToAction("HistoriqueAgent_SAMRC", "EvaluationServiceClientAuto");
                }
                else if (UserManager.IsInRole(emp.Id, "Agent_HL"))
                {
                    return RedirectToAction("HistoriqueAgent_HL", "EvaluationServiceClientAuto");
                }
                else if (UserManager.IsInRole(emp.Id, "Qualité"))
                {
                    return RedirectToAction("listeSites", "Superviseur");
                }
                else if (UserManager.IsInRole(emp.Id, "Agent Qualité"))
                {
                    return RedirectToAction("listeSitesAgentQualite", "Superviseur");
                }
                else if (UserManager.IsInRole(emp.Id, "Agent Qualité_CustomerService"))
                {
                    return RedirectToAction("listeSitesAgentQualite", "Superviseur");
                }
                else if (UserManager.IsInRole(emp.Id, "Agent Qualité_HL"))
                {
                    return RedirectToAction("listeSitesAgentQualite", "Superviseur");
                }
                else if (UserManager.IsInRole(emp.Id, "Agent Qualité_Diffusion"))
                {
                    return RedirectToAction("listeSitesAgentQualite", "Superviseur");
                }
                else if (UserManager.IsInRole(emp.Id, "Agent Qualité_AchatPublic"))
                {
                    return RedirectToAction("listeSitesAgentQualite", "Superviseur");
                }

                else if (UserManager.IsInRole(emp.Id, "Agent Qualité_PRV"))
                {
                    return RedirectToAction("listeSitesAgentQualite", "Superviseur");
                }

                else if (UserManager.IsInRole(emp.Id, "ManagerIPD"))
                {

                    return RedirectToAction("IPD_EtatCampagne", "Details_Qualifs_IPD");

                }
                else
                    ViewBag.message = ("login ou mot de passe incorrect!");
                ViewBag.color = "red";
                return View("~/Views/Authentification/Index.cshtml");
            }


            //switch (result)
            //{
            //    case SignInStatus.Success:

            //        return RedirectToAction("Index", "Home");
            //    case SignInStatus.LockedOut:
            //        return View("Lockout");

            //    case SignInStatus.Failure:
            //    default:
            //        ModelState.AddModelError("", "Invalid login attempt.");
            //        return RedirectToAction("Index", "Authentification");
            //}

            //
        }

        // GET: Authentification/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Authentification/Create
        [HttpPost]
        [CustomAuthorise(Roles = "Admin")]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Authentification/Edit/5
        [CustomAuthorise(Roles = "Admin")]

        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Authentification/Edit/5
        [HttpPost]
        [CustomAuthorise(Roles = "Admin")]

        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Authentification/Delete/5
        [CustomAuthorise(Roles = "Admin")]

        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Authentification/Delete/5
        [HttpPost]
        [CustomAuthorise(Roles = "Admin")]

        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        [AllowAnonymous]
        public ActionResult logout()
        {
            //Session.Clear();

            var user = UserManager.FindById(Int32.Parse(User.Identity.GetUserId()));
            Utilisateur a = service.getUserLastLogin(user.userLogin);

            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index");
            //Utilisateur user = service.getByTempSortie(Session["loginIndex"].ToString());
            //    Utilisateur utilisateur = service.getBylogin(Session["loginIndex"].ToString());
            //    utilisateur.logSortie = DateTime.Now;
            //    if (TryUpdateModel(utilisateur))
            //    {
            //        try
            //        {
            //            service.SaveChange();
            //            return RedirectToAction("Index");
            //        }
            //        catch (DataException)
            //        {
            //            ModelState.AddModelError("", "Erreur!!!!!");
            //        }
            //    }
            //    return RedirectToAction("Index");
            //}
        }
    }
}