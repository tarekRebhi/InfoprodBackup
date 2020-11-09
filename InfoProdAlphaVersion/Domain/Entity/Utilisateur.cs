using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Domain.Entity
{
    public class Utilisateur
    {
        public int Id { get; set; }
        public string login { get; set; }

        //public String password { get; set; }

        public string nomPrenom { get; set; }
        //public String prenom { get; set; }

        public string role { get; set; }

        public DateTime? logEntree { get; set; }
        public DateTime? logSortie { get; set; }

        //public String responsable { get; set; }

        public virtual ICollection<Groupe> groupes { get; set; }




    }
}
