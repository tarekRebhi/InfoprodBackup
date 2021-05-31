using Data.CustumConvention;
using Domain.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class ReportContext : IdentityDbContext<Employee, CustomRole,
    int, CustomUserLogin, CustomUserRole, CustomUserClaim>
    {
        public ReportContext() : base("name=TestDBConnexion")
        {

            //Database.SetInitializer<MyAlfrescoContext>(new ReportContextInitialiazor());

        }
        public DbSet<Alerte> alertes { get; set; }


        public DbSet<Utilisateur> utilisateurs { get; set; }
        public DbSet<Planing> planings { get; set; }
        public DbSet<AttendanceHermes> attendancesHermes { get; set; }
        public DbSet<AttendanceHermes_Campagnes> AttendanceHermes_Campagnes { get; set; }

        public DbSet<Titre> titres { get; set; }
        public DbSet<Indicateur> indicateurs { get; set; }

        public DbSet<Groupe> groupes { get; set; }
        public DbSet<GroupesEmployees> employeesGroupes { get; set; }
        public DbSet<Appel> appels { get; set; }
        public DbSet<Temps> temps { get; set; }

        public DbSet<Event> events { get; set; }
        public DbSet<DetailsActivite> Details_Activité { get; set; }
        public DbSet<GrilleEvaluation> evaluations { get; set; }
        public DbSet<Details_Activite_IPD> Details_Activite_IPD { get; set; }
        public DbSet<Details_Activite_PROMO_GISI> Details_Activite_PROMO_GISI { get; set; }
        public DbSet<Details_Activite_PROMO_GMT> Details_Activite_PROMO_GMT { get; set; }
        public DbSet<Details_Activite_REAB_GISI> Details_Activite_REAB_GISI { get; set; }
        public DbSet<Details_Activite_REAB_GMT> Details_Activite_REAB_GMT { get; set; }
        public DbSet<DT_INJECTED> DT_INJECTED { get; set; }
        public DbSet<Objectif> Objectifs { get; set; }

        public DbSet<GrilleEvaluationQR> GrilleEvaluationQRs { get; set; }
        public DbSet<GrilleEvaluationKLMO> GrilleEvaluationKLMOes { get; set; }
        public DbSet<GrilleEvaluationBPP> GrilleEvaluationBPPs { get; set; }
        public DbSet<GrilleEvaluationBPPTypologie> GrilleEvaluationBPPTypologies { get; set; }
        public DbSet<GrilleEvaluationEnqueteAuto> GrilleEvaluationEnqueteAutoes { get; set; }
        public DbSet<GrilleEvaluationFO> GrilleEvaluationFOSCGISI { get; set; }
        public DbSet<GrilleEvaluationBO> GrilleEvaluationBOSCGISI { get; set; }
        public DbSet<GrilleEvaluationFOSAMRC> GrilleEvaluationFOSAMRCs { get; set; }
        public DbSet<GrilleEvaluationHLAuto> GrilleEvaluationHLAutos { get; set; }
        public DbSet<GrilleEvaluationBOSAMRC> GrilleEvaluationBOSAMRCs { get; set; }
        public DbSet<GrilleEvaluationBattonage> GrilleEvaluationBattonages { get; set; }
        public DbSet<GrilleEvaluationAchatPublic> GrilleEvaluationAchatPublics { get; set; }
        public DbSet<GrilleEvaluationPRV> GrilleEvaluationPRVs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Add(new DateTime2Convention());
            /*modelBuilder.Entity<Details_Activite_REAB_GISI>()
                .Property(c => c.AI_DATE).HasColumnType("datetime2");*/
        }
        public static ReportContext Create()
        {
            return new ReportContext();
        }

        public System.Data.Entity.DbSet<Domain.Entity.Inventaire> Inventaires { get; set; }
        public System.Data.Entity.DbSet<Domain.Entity.Appels_Entrants_SamRc> Appels_Entrants_SamRc { get; set; }
        public System.Data.Entity.DbSet<Domain.Entity.Appels_Sortants_SamRc> Appels_Sortants_SamRc { get; set; }
        public System.Data.Entity.DbSet<Domain.Entity.AttendanceHermes_SamRc> AttendanceHermes_SamRc { get; set; }
        public System.Data.Entity.DbSet<Domain.Entity.Temps_SamRc> Temps_SamRc { get; set; }




        //public System.Data.Entity.DbSet<Domain.Entity.CustomRole> CustomRoles { get; set; } on declarant dbset CustomRole ici on va avoir une redondance de meme class parceque dans l'implementation de la classe en haut CustomRole est appelé et prendra le nom ROLE
    }
}
