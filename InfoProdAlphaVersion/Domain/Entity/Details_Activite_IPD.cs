using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Domain.Entity
{
   public class Details_Activite_IPD
    {
        public int Id { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(50)]
        public string NOM_OPERATION { get; set; }

        public int INDICE { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(62)]
        public string TV { get; set; }

        public int? ID_TV { get; set; }

        
        public int? STATUSGROUP { get; set; }

        public int? STATUS { get; set; }

        [Column(TypeName = "NVARCHAR")]
        [StringLength(400)]
        public string LIB_STATUS { get; set; }

        public int? DETAIL { get; set; }

        [Column(TypeName = "NVARCHAR")]
        [StringLength(400)]
        public string LIB_DETAIL { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(8)]
        public string DATE { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(4)]
        public string HEURE { get; set; }
       
        public int? Site { get; set; }

        public Boolean? Positive { get; set; }

        public Boolean? Argued { get; set; }
   
        [Column(TypeName = "NVARCHAR")]
        [StringLength(10)]
        public string ACTIVITE { get; set; }
   
    }
}
