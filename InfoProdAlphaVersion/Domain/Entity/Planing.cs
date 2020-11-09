using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class Planing
    {
        public int Id { get; set; }


        [DataType(DataType.Date)]
        public DateTime dateDebut { get; set; } // int dans la conception de départ

        [DataType(DataType.Date)]
        public DateTime dateFin { get; set; }

        public string heureDebut { get; set; }
        public string heureFin { get; set; }


        public virtual ICollection<Groupe> groupes { get; set; }




    }
}
