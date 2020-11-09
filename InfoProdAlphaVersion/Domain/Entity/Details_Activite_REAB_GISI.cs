using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity
{
    public class Details_Activite_REAB_GISI
    {
        public int Id { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(9)]
        public string ACTIVITE { get; set; }
        [Column(TypeName = "NVARCHAR")]
        [StringLength(255)]
        public string TITRE_OPERATION { get; set; }
        public int? indice { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(8)]
        public string DATE { get; set; }
        public int? ID_TV { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(62)]
        public string TV { get; set; }
        public int? STATUSGROUP { get; set; }
        public int? STATUS { get; set; }
        [Column(TypeName = "NVARCHAR")]
        [StringLength(400)]
        public string liB_STATUS { get; set; }
        [Column(TypeName = "NVARCHAR")]
        [StringLength(400)]
        public string lIB_DETAIL { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(60)]
        public string TEL { get; set; }
        public int? AI { get; set; }
        public DateTime? AI_DATE { get; set; }
        [Column(TypeName = "NVARCHAR")]
        [StringLength(255)]
        public string AI_AGENT { get; set; }
        [Column(TypeName = "NVARCHAR")]
        [StringLength(255)]
        public string AI_ART1_CODE { get; set; }
        [Column(TypeName = "NVARCHAR")]
        [StringLength(255)]
        public string AI_ART1_CODE_TARIF { get; set; }
        [Column(TypeName = "NVARCHAR")]
        [StringLength(255)]
        public string AI_ART1_QTE { get; set; }
        [Column(TypeName = "NVARCHAR")]
        [StringLength(255)]
        public string AI_ITL { get; set; }
        [Column(TypeName = "VARCHAR")]
        [MaxLength]
        public string AI_ART1_PERSO_A { get; set; }
        [Column(TypeName = "NVARCHAR")]
        [StringLength(50)]
        public string DATE_INJECTION { get; set; }
        public int? Site { get; set; }
        public int? DATECOV { get; set; }
        public Boolean? Positive { get; set; }
        public Boolean? Argued { get; set; }
        [Column(TypeName = "NVARCHAR")]
        [StringLength(255)]
        public string RANG { get; set; }
        [Column(TypeName = "NVARCHAR")]
        [StringLength(255)]
        public string NUM_ECHEANCE { get; set; }
        public int? Semaine { get; set; }
        [Column(TypeName = "NVARCHAR")]
        [StringLength(21)]
        public string Trimestre { get; set; }
        public int? Annee { get; set; }
    }
}
