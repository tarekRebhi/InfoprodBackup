using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity
{
   public class DetailsActivite
    {
        public int Id { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(10)]
        public String ACTIVITE { get; set; }
        public int? INDICE { get; set; }
        public int? PRIORITE { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(8)]
        public String DATE { get; set; }
        public int? Semaine { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(62)]
        public String TV { get; set; }
        public int? ID_TV { get; set; }
        public int? StatusCode { get; set; }
        [StringLength(400)]
        public String StatusText { get; set; }
        public Boolean? Positive { get; set; }
        public Boolean? Argued { get; set; }

    }
}
