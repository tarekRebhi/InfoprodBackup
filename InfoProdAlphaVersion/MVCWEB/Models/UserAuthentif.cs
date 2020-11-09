using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCWEB.Models
{
    public class UserAuthentif
    {
        public string login { get; set; }
        //public string password { get; set; }
        
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public string UserName { get; set; }

    }
}