using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class Event
    {
        public Event()
        {
            this.groupes = new HashSet<Groupe>();
        }
        public int Id { get; set; }
        public string titre { get; set; }
        public string description { get; set; }
        [DataType(DataType.Date)]
        public DateTime dateDebut { get; set; }
        [DataType(DataType.Date)]
        public DateTime dateFin { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public string themeColor { get; set; }
        public int? employeeId { get; set; }

        [ForeignKey("employeeId")]
        public virtual Employee employee { get; set; }
        public virtual ICollection<Groupe> groupes { get; set; }

    }
}