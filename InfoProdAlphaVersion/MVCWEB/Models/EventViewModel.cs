using Domain.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCWEB.Models
{
    public class EventViewModel
    {
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
        public string employeeName { get; set; }
        public string groupeName { get; set; }
        public int? employeeId { get; set; }

        [ForeignKey("employeeId")]
        public virtual Employee employee { get; set; }
        public virtual ICollection<Groupe> groupes { get; set; }

        public List<SelectListItem> utilisateurs = new List<SelectListItem>();

        public List<SelectListItem> groupesass = new List<SelectListItem>();
    }
}