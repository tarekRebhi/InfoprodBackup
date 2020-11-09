//using Domain.Entity;
//using Microsoft.AspNet.Identity;
//using Microsoft.AspNet.Identity.EntityFramework;
//using System;
//using System.Collections.Generic;
//using System.Data.Entity;
//using System.Linq;
//using System.Security.Claims;
//using System.Text;
//using System.Threading.Tasks;
//using System.Web.UI.WebControls;

//namespace Data
//{

//    public class ReportIdentityTestContext : IdentityDbContext<Employee, CustomRole,
//    int, CustomUserLogin, CustomUserRole, CustomUserClaim>
//    {
//        public ReportIdentityTestContext()
//                : base("TestDBConnexion")
//            {
//            }
//        //public DbSet<CustomUserRole> UserRoles { get; set; }
//        //public DbSet<CustomUserLogin> UserLogins { get; set; }
//        //public DbSet<CustomUserClaim> UserClaims{ get; set; }
//       public DbSet<CustomRole> CustomRoles { get; set; }

//        protected override void OnModelCreating(DbModelBuilder modelBuilder)
//        {
//            base.OnModelCreating(modelBuilder);


//        }
//        //public virtual ICollection<IRole> Roles { get; private set; }
//        //public virtual ICollection<Claim> Claims { get; private set; }
//        //public virtual ICollection<Login> Logins { get; private set; }
//        public static ReportIdentityTestContext Create()
//        {
//            return new ReportIdentityTestContext();
//        }
//        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
//        //{
//        //    base.OnModelCreating(modelBuilder);

//        //    modelBuilder.Entity<TUserRole>()
//        //    .HasKey(r => new { r.UserId, r.RoleId })
//        //    .ToTable("AspNetUserRoles");

//        //    modelBuilder.Entity<TUserLogin>()
//        //                .HasKey(l => new { l.LoginProvider, l.ProviderKey, l.UserId })
//        //                .ToTable("AspNetUserLogins");
//        //}
//    }
//}

