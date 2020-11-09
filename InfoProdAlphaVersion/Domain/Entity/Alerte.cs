using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
   public class Alerte
    {
        public int Id { get; set; }

        public String titreAlerte { get; set; }

        public String description { get; set; }

        [DataType(DataType.Date)]
        public DateTime dateCreation { get; set; }
        public String etatAlerte { get; set; }

        public String reponseAlerte { get; set; }
        [DataType(DataType.Date)]
        public DateTime dateReponse { get; set; }

        public int? indicateurId { get; set; }

        [ForeignKey("indicateurId")]
        public virtual Indicateur indicateur { get; set; }
        public int? senderId { get; set; }
        public int? reciverId { get; set; }

        public string status { get; set; }

        public string statusReponse { get; set; }

    }
}
