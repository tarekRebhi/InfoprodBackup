using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
   public class Groupe
    {
        //public Groupe()
        //{
        //    this.events = new HashSet<Event>();
        //}
        public int Id { get; set; }
        public String nom { get; set; }


        public String responsable { get; set; }
        public virtual ICollection<GroupesEmployees> groupesEmployees { get; set; }
        public virtual ICollection<Event> events { get; set; }
        

    }
}
