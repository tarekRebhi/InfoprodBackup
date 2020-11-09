using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class GroupesEmployees
    {
        public int Id { get; set; }
        public int? employeeId { get; set; }

        [ForeignKey("employeeId")]
        public virtual Employee employee { get; set; }
        public int? groupeId { get; set; }

        [ForeignKey("groupeId")]
        public virtual Groupe groupe { get; set; }
    }
}
