using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
   public class Titre
    {
        public int Id { get; set; }

        public String activite { get; set; }
        public String type { get; set; }
        public string libelle { get; set; }
        public String codeOperation { get; set; }
        public String codeProvRelance { get; set; }
        [Column(TypeName = "Date")]
        [DataType(DataType.Date)]
        public DateTime dateInjection { get; set; }
        public int nombreFichesInjectees { get; set; }
        public virtual ICollection<Indicateur> indicateurs { get; set; }
        public virtual ICollection<Planing> planings { get; set; }

    }
}
