using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Repository;

namespace MyReports.Data.Infrastructure
{
    public interface IUnitOfWork : IDisposable
    {
        //IRepositoryBaseAsynch<T> getRepository<T>() where T : class;
        // IContratRepository ContratRepository { get; }
        IGroupesEmployeesRepository GroupesEmployeesRepository { get; }
        IAlerteRepository AlerteRepository { get; }
        IUtilisateurRepository UserRepository { get; }

        IIndicateurRepository IndicateurRepository { get; }
        IPlaningRepository PlaningRepository { get; }

        ITitreRepository TitreRepository { get; }

        IEmployeeRepository EmployeeRepository { get; }

        IGroupeRepository GroupeRepository { get; }
        IAttencanceHermesRepository AttendanceHermesRepository { get; }
        IAppelRepository AppelRepository { get; }
        IEventRepository EventRepository { get; }
        IEvaluationRepository EvaluationRepository { get; }
        IEvaluationQRRepository EvaluationQRRepository { get; }
        IEvaluationKLMORepository EvaluationKLMORepository { get; }
        IEvaluationBPPRepository EvaluationBPPRepository { get; }

        IEvaluationBPPTypologieRepository EvaluationBPPTypologieRepository { get; }

        IEvaluationBattonageRepository EvaluationBattonageRepository { get; }
        IEvaluationEnqueteAutoRepository EvaluationEnqueteAutoRepository { get; }
        IEvaluationFOSCGISIRepository EvaluationFOSCGISIRepository { get; }
        IEvaluationBOSCGISIRepository EvaluationBOSCGISIRepository { get; }
        IEvaluationFOSAMRCRepository EvaluationFOSAMRCRepository { get; }
        IEvaluationBOSAMRCRepository EvaluationBOSAMRCRepository { get; }
        IEvaluationAchatPublicRepository EvaluationAchatPublicRepository { get; }

        IIndicateursSAMRCRepository IndicateursSAMRCRepository { get; }
        IEvaluationPRVRepository EvaluationPRVRepository { get; }
        void CommitAsync();
        void Commit();

    }

}
