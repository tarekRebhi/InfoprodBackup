using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity
{
   public class DT_INJECTED
    {
        public int Id { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(10)]
        public string ACTIVITE { get; set; }
        [Column(TypeName = "NVARCHAR")]
        [StringLength(255)]
        public string TITRE_OPERATION { get; set; }
        [Column(TypeName = "NVARCHAR")]
        [StringLength(50)]
        public string DATE_INJECTION { get; set; }
        public int TOTAL { get; set; }
        public int DATECOV { get; set; }
        public int Site { get; set; }
        public int Annee { get; set; }
        [Column(TypeName = "NVARCHAR")]
        [StringLength(21)]
        public string Trimestre { get; set; }
        [Column(TypeName = "NVARCHAR")]
        [StringLength(30)]
        public string Mois { get; set; }
        public int Semaine { get; set; }
    }
}
