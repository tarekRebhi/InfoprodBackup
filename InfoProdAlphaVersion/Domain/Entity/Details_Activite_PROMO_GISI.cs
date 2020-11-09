using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity
{

    public class Details_Activite_PROMO_GISI
    {
        public int Id { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(10)]
        public string ACTIVITE { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(3)]
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
        [Column(TypeName = "NVARCHAR")]
        [StringLength(255)]
        public string CODE_OPE { get; set; }
        [Column(TypeName = "NVARCHAR")]
        [StringLength(255)]
        public string CODE_PROV { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(62)]
        public string TEL { get; set; }
        [Column(TypeName = "NVARCHAR")]
        [StringLength(255)]
        public string CODE_PROV_RELANCE { get; set; }
        public int? ACCORD_TEMP { get; set; }
        public DateTime? ACCORD_TEMP_DATE { get; set; }
        [Column(TypeName = "NVARCHAR")]
        [StringLength(255)]
        public string ACCORD_TEMP_AGENT { get; set; }
        [Column(TypeName = "NVARCHAR")]
        [StringLength(255)]
        public string ACCORD_TEMP_CODE_OFFRE { get; set; }
        [Column(TypeName = "NVARCHAR")]
        [StringLength(255)]
        public string ACCORD_TEMP_CODE_TARIF { get; set; }
        [Column(TypeName = "NVARCHAR")]
        [StringLength(255)]
        public string ACCORD_TEMP_QTE { get; set; }
        [Column(TypeName = "NVARCHAR")]
        [StringLength(255)]
        public string ACCORD_TEMP_ITL { get; set; }
        [Column(TypeName = "VARCHAR")]
        [MaxLength]
        public string PERSO_LIB_OFFRE_1 { get; set; }
        [Column(TypeName = "NVARCHAR")]
        [StringLength(255)]
        public string PERSO_LIB_PRODUIT { get; set; }
        [Column(TypeName = "NVARCHAR")]
        [StringLength(50)]
        public string DATE_INJECTION { get; set; }
        public int? DATECOV { get; set; }
        public int? Site { get; set; }
        public Boolean? Argued { get; set; }
        public Boolean? Positive { get; set; }
        public int? Semaine { get; set; }
        [Column(TypeName = "NVARCHAR")]
        [StringLength(21)]
        public string Trimestre { get; set; }
        public int? Annee { get; set; }
    }
}
