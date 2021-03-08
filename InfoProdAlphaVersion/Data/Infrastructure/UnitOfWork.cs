using Data;
using Data.Repository;
using MyReports.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFinance.Data.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {

        private ReportContext dataContext;
        //private ReportIdentityTestContext identityContext;
        IDatabaseFactory dbFactory;


        public UnitOfWork(IDatabaseFactory dbFactory)
        {
            this.dbFactory = dbFactory;

        }

        protected ReportContext DataContext
        {
            get
            {       
                return dataContext = dbFactory.DataContext;
            }
        }
        //protected ReportIdentityTestContext IdentityContext
        //{
        //    get
        //    {
        //        return identityContext = dbFactory.IdentityContext;
        //    }
        //}

        public void Commit()
        {
            DataContext.SaveChanges();
            //IdentityContext.SaveChanges();
        }
        public void CommitAsync()
        {
            DataContext.SaveChangesAsync();
            //IdentityContext.SaveChangesAsync();

        }
        public void Dispose()
        {
            dbFactory.Dispose();
        }

        private IAlerteRepository alerteRepository;
        public IAlerteRepository AlerteRepository
        {
            get { return alerteRepository = new AlerteRepository(dbFactory); }
        }


        private IUtilisateurRepository userRepository;

        public IUtilisateurRepository UserRepository
        {
            get { return userRepository = new UtilisateurRepository(dbFactory); }
        }

        private IEmployeeRepository employeeRepository;

        public IEmployeeRepository EmployeeRepository
        {
            get { return employeeRepository = new EmployeeRepository(dbFactory); }
        }

        private ITitreRepository titreRepository;

        public ITitreRepository TitreRepository
        {
            get { return titreRepository = new TitreRepository(dbFactory); }
        }

        private IGroupeRepository groupeRepository;

        public IGroupeRepository GroupeRepository
        {
            get { return groupeRepository = new GroupeRepository(dbFactory); }
        }

        private IIndicateurRepository indicateurRepository;

        public IIndicateurRepository IndicateurRepository
        {
            get { return indicateurRepository = new IndicateurRepository(dbFactory); }
        }

        private IPlaningRepository planingRepository;

        public IPlaningRepository PlaningRepository
        {
            get { return planingRepository = new PlaningRepository(dbFactory); }
        }
        private IGroupesEmployeesRepository groupEmpRepository;

        public IGroupesEmployeesRepository GroupesEmployeesRepository
        {
            get { return groupEmpRepository = new GroupesEmployeesRepository(dbFactory); }
        }
        private IAttencanceHermesRepository attendanceHermesRepository;
        public IAttencanceHermesRepository AttendanceHermesRepository
        {
            get { return attendanceHermesRepository = new AttendanceHermesRepository(dbFactory); }
        }
        private IAppelRepository appelRepository;
        public IAppelRepository AppelRepository
        {
            get { return appelRepository = new AppelRepository(dbFactory); }
        }
        private IEventRepository eventRepository;
        public IEventRepository EventRepository
        {
            get { return eventRepository = new EventRepository(dbFactory); }
        }
        private IEvaluationRepository evaluationRepository;
        public IEvaluationRepository EvaluationRepository
        {
            get { return evaluationRepository = new EvaluationRepository(dbFactory); }
        }
        private IEvaluationQRRepository evaluationQRRepository;
        public IEvaluationQRRepository EvaluationQRRepository
        {
            get { return evaluationQRRepository = new EvaluationQRRepository(dbFactory); }
        }
        private IEvaluationKLMORepository evaluationKLMORepository;
        public IEvaluationKLMORepository EvaluationKLMORepository
        {
            get { return evaluationKLMORepository = new EvaluationKLMORepository(dbFactory); }
        }
        private IEvaluationBPPRepository evaluationBPPRepository;
        public IEvaluationBPPRepository EvaluationBPPRepository
        {
            get { return evaluationBPPRepository = new EvaluationBPPRepository(dbFactory); }
        }


        private IEvaluationBPPTypologieRepository evaluationBPPTypologieRepository;
        public IEvaluationBPPTypologieRepository EvaluationBPPTypologieRepository
        {
            get { return evaluationBPPTypologieRepository = new EvaluationBPPTypologieRepository(dbFactory); }
        }


        private IEvaluationBattonageRepository evaluationBattonageRepository;
        public IEvaluationBattonageRepository EvaluationBattonageRepository
        {
            get { return evaluationBattonageRepository = new EvaluationBattonageRepository(dbFactory); }
        }

        private IEvaluationEnqueteAutoRepository evaluationEnqueteAutoRepository;
        public IEvaluationEnqueteAutoRepository EvaluationEnqueteAutoRepository
        {
            get { return evaluationEnqueteAutoRepository = new EvaluationEnqueteAutoRepository(dbFactory); }
        }

        private IEvaluationFOSCGISIRepository evaluationFOSCGISIRepository;
        public IEvaluationFOSCGISIRepository EvaluationFOSCGISIRepository
        {
            get { return evaluationFOSCGISIRepository = new EvaluationFOSCGISIRepository(dbFactory); }
        }

        private IEvaluationBOSCGISIRepository evaluationBOSCGISIRepository;
        public IEvaluationBOSCGISIRepository EvaluationBOSCGISIRepository
        {
            get { return evaluationBOSCGISIRepository = new EvaluationBOSCGISIRepository(dbFactory); }
        }

        private IEvaluationFOSAMRCRepository evaluationFOSAMRCRepository;
        public IEvaluationFOSAMRCRepository EvaluationFOSAMRCRepository
        {
            get { return evaluationFOSAMRCRepository = new EvaluationFOSAMRCRepository(dbFactory); }
        }

        private IEvaluationBOSAMRCRepository evaluationBOSAMRCRepository;
        public IEvaluationBOSAMRCRepository EvaluationBOSAMRCRepository
        {
            get { return evaluationBOSAMRCRepository = new EvaluationBOSAMRCRepository(dbFactory); }
        }

        private IEvaluationAchatPublicRepository evaluationAchatPublicRepository;
        public IEvaluationAchatPublicRepository EvaluationAchatPublicRepository
        {
            get { return evaluationAchatPublicRepository = new EvaluationAchatPublicRepository(dbFactory); }
        }

        private IIndicateursSAMRCRepository indicateursSAMRCRepository;
        public IIndicateursSAMRCRepository IndicateursSAMRCRepository
        {
            get { return indicateursSAMRCRepository = new IndicateursSAMRCRepository(dbFactory); }
        }

        private IEvaluationPRVRepository evaluationPRVRepository;
        public IEvaluationPRVRepository EvaluationPRVRepository
        {
            get { return evaluationPRVRepository = new EvaluationPRVRepository(dbFactory); }
        }
    }
}
