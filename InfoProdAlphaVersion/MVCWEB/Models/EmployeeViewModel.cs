using Domain.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCWEB.Models
{
    public class EmployeeViewModel
    {
        public int Id { get; set; }

        public int IdHermes { get; set; }

        public String userName { get; set; }

        public String pseudoName { get; set; }

        public int IdAD { get; set; }

        public String role { get; set; }
        public String Activite { get; set; }
        public virtual ICollection<Groupe> Group { get; set; }
        public virtual ICollection<Indicateur> Indicateurs { get; set; }
        public int? userId { get; set; }

        [ForeignKey("userId")]
        public virtual Utilisateur user { get; set; }

        public virtual ICollection<Utilisateur> Users { get; set; }
        public virtual List<Groupe> GroupeTests { get; set; }


        public List<SelectListItem> utilisateurs = new List<SelectListItem>();

        public List<SelectListItem> groupes = new List<SelectListItem>();
        public List<SelectListItem> groupesassocies = new List<SelectListItem>();
        public List<SelectListItem> roles = new List<SelectListItem>();
        public String userLogin { get; set; }

        public List<String> tests = new List<String>();
        public byte[] Content { get; set; }

        public String ContentType { get; set; }
        public String image { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }


        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        public String empId { get; set; }
        public String Url { get; set; }

    }
}