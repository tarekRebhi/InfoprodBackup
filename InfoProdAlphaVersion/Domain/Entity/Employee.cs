using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
  public class Employee: IdentityUser<int, CustomUserLogin, CustomUserRole,
    CustomUserClaim>
    {
//TKey The type of the key.
//TLogin The type of the login.
// TRole The type of the role.
//TClaim The type of the claim.
        //public  int Id { get; set; } //this is a property not a variable
        public int IdHermes { get; set; }

        //public String userName { get; set; }

        public String pseudoName { get; set; }

        public String IdAD { get; set; }

        public String role { get; set; }
        public String Activite { get; set; }
        
        public String userLogin { get; set; }
        public byte[] Content { get; set; }

        public String ContentType { get; set; }
        public int? userId { get; set; }

        [ForeignKey("userId")]
        public virtual Utilisateur user { get; set; }

        public virtual ICollection<Utilisateur> Users { get; set; }
        public virtual ICollection<GroupesEmployees> employeesGroupes { get; set; }
        public virtual ICollection<Indicateur> Indicateurs { get; set; }

        public virtual ICollection<Event> events { get; set; }

        public virtual ICollection<GrilleEvaluation> evaluations { get; set; }
        public virtual ICollection<GrilleEvaluationQR> GrilleEvaluationQRs { get; set; }
        public virtual ICollection<GrilleEvaluationKLMO> GrilleEvaluationKLMOes { get; set; }
        public virtual ICollection<GrilleEvaluationBPP> GrilleEvaluationBPPs { get; set; }
        public virtual ICollection<GrilleEvaluationEnqueteAuto> GrilleEvaluationEnqueteAutoes { get; set; }
        public virtual ICollection<GrilleEvaluationFO> GrilleEvaluationFOSCGISI { get; set; }
        public virtual ICollection<GrilleEvaluationBO> GrilleEvaluationBOSCGISI { get; set; }
        //  public virtual ICollection<GrilleEvaluationFOSAMRC> GrilleEvaluationFOSAMRC { get; set; }
        // public virtual ICollection<GrilleEvaluationBOSAMRC> GrilleEvaluationBOSAMRC { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(
    UserManager<Employee, int> manager)
        {
            // Note the authenticationType must match the one defined in
            // CookieAuthenticationOptions.AuthenticationType 
            var userIdentity = await manager.CreateIdentityAsync(
                this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here 
            return userIdentity;
        }
        //public int getIdHermes()
        //{
        //    return IdHermes;
        //}
    }


    public class CustomUserRole : IdentityUserRole<int> {

        public int Id { get; set; }
    }
    public class CustomUserClaim : IdentityUserClaim<int> {
        //public int Id { get; set; }
    }
    public class CustomUserLogin : IdentityUserLogin<int> {
        public int Id { get; set; }
    }
    public class CustomRole : IdentityRole<int, CustomUserRole>, IRole<int>

    {
        public CustomRole() { }
        public CustomRole(string name) { Name = name; }
    }
}
